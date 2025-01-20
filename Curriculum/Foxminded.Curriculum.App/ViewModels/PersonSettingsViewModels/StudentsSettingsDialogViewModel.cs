using Foxminded.Curriculum.App.Commands;
using Foxminded.Curriculum.App.Commands.PersonSettingsCommands;
using Foxminded.Curriculum.App.Services;
using Foxminded.Curriculum.App.ViewModels.HomeViewModels;
using Foxminded.Curriculum.BLL.Services;
using Foxminded.Curriculum.Domain.Entities;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Foxminded.Curriculum.App.ViewModels.PersonSettingsViewModels;

public class StudentsSettingsDialogViewModel : ViewModelBase
{
    public ObservableCollection<Students> Students { get; set; }
    public ObservableCollection<Groups> Groups { get; set; }
    public Groups CurrentGroup => _currentGroup;
    public ICommand CloseCommand { get; }
    public ICommand AddStudentCommand { get; }
    public ICommand EditStudentCommand { get; }
    public ICommand DeleteStudentCommand { get; }
    public Students SelectedStudent
    {
        get => _selectedStudent;
        set
        {
            if (_selectedStudent != value)
            {
                _selectedStudent = value;
                OnPropertyChanged();
            }
        }
    }
    private Students _selectedStudent;
    private StudentService _studentService;
    private Groups _currentGroup;
    private Window _dialogWindow;

    public StudentsSettingsDialogViewModel(Window dialogWindow, GroupsViewModel groupsViewModel, 
        StudentsViewModel studentsViewModel, StudentService studentService, IDialogService dialogService)
    {
        Students = studentsViewModel.Students;
        Groups = groupsViewModel.Groups;
        _studentService = studentService;
        _currentGroup = groupsViewModel.SelectedGroup;
        _selectedStudent = null!;
        _dialogWindow = dialogWindow;

        AddStudentCommand = new AddStudentDialogCommand(this, _studentService, dialogService);
        EditStudentCommand = new EditStudentDialogCommand(this, _studentService, dialogService);
        DeleteStudentCommand = new DeleteStudentCommand(this, _studentService, dialogService);

        CloseCommand = new RelayCommand(execute => Close());

        PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(SelectedStudent))
                OnPropertyChanged(nameof(CanClick));
        };
    }

    public bool CanClick => SelectedStudent != null;

    public async Task LoadStudentsForGroupAsync(int id)
    {
        var students = await _studentService.GetStudentsForGroupAsync(id);
        foreach (var student in students)
        {
            student.Group = Groups.FirstOrDefault(g => g.Id == student.Group_ID)!;
            Students.Add(student);
        }
    }

    private void Close() => _dialogWindow.Close();
}
