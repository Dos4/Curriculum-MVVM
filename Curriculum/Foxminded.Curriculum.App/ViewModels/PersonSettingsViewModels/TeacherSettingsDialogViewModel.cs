using Foxminded.Curriculum.App.Commands;
using Foxminded.Curriculum.App.ViewModels.HomeViewModels;
using Foxminded.Curriculum.BLL.Services;
using Foxminded.Curriculum.Domain.Entities;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;
using Foxminded.Curriculum.App.Commands.PersonSettingsCommands;
using Foxminded.Curriculum.App.Services;

namespace Foxminded.Curriculum.App.ViewModels.PersonSettingsViewModels;

public class TeacherSettingsDialogViewModel : ViewModelBase
{
    public ObservableCollection<Teachers> Teachers { get; set; }
    public ObservableCollection<Groups> Groups { get; set; }
    public Groups CurrentGroup => _currentGroup;
    public ICommand CloseCommand { get; }
    public ICommand AddTeacherCommand { get; }
    public ICommand EditTeacherCommand { get; }
    public ICommand DeleteTeacherCommand { get; }
    public Teachers SelectedTeacher
    {
        get => _selectedTeacher;
        set
        {
            if (_selectedTeacher != value)
            {
                _selectedTeacher = value;
                OnPropertyChanged();
            }
        }
    }
    private Teachers _selectedTeacher;
    private TeacherService _teacherService;
    private Groups _currentGroup;
    private Window _dialogWindow;

    public TeacherSettingsDialogViewModel(Window dialogWindow, GroupsViewModel groupsViewModel, 
        TeachersViewModel teachersViewModel, TeacherService studentService, IDialogService dialogService)
    {
        Groups = groupsViewModel.Groups;
        _currentGroup = groupsViewModel.SelectedGroup;
        Teachers = teachersViewModel.Teachers;
        _teacherService = studentService;
        _selectedTeacher = null!;
        _dialogWindow = dialogWindow;

        AddTeacherCommand = new AddTeacherDialogCommand(this, _teacherService, dialogService);
        EditTeacherCommand = new EditTeacherDialogCommand(this, _teacherService, dialogService);
        DeleteTeacherCommand = new DeleteTeacherCommand(this, _teacherService, dialogService);

        CloseCommand = new RelayCommand(execute => Close());

        PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(SelectedTeacher))
                OnPropertyChanged(nameof(CanClick));
        };
    }

    public bool CanClick => SelectedTeacher != null;

    public async Task LoadTeacherAsync(int id)
    {
        var teacher = await _teacherService.GetTeachersById(id);
        Teachers.Add(teacher);
    }

    private void Close() => _dialogWindow.Close();
}
