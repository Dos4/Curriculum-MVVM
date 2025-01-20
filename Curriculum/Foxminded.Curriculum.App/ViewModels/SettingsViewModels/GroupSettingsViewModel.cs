using Foxminded.Curriculum.App.Commands;
using Foxminded.Curriculum.App.Commands.GroupSettingsCommands;
using Foxminded.Curriculum.App.Services;
using Foxminded.Curriculum.App.ViewModels.HomeViewModels;
using Foxminded.Curriculum.BLL.Services;
using Foxminded.Curriculum.Domain.Entities;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Foxminded.Curriculum.App.ViewModels.SettingsViewModels;

public class GroupSettingsViewModel : ViewModelBase
{
    public ObservableCollection<Groups> Groups { get; set; }
    public ObservableCollection<Courses> Courses { get; set; }
    public ObservableCollection<Teachers> Teachers { get; set; }
    public ICommand NavigateHomeCommand { get; }
    public ICommand CreateGroupCommand { get; }
    public ICommand DeleteGroupCommand { get; }
    public ICommand EditGroupCommand { get; }
    public ICommand ExportStudentsToCsvCommand { get; }
    public ICommand ImportStudentsFromCsvCommand { get; }
    public ICommand GenerateDocumentCommand { get; }
    public Groups SelectedGroup
    {
        get => _selectedGroup;
        set
        {
            if (_selectedGroup != value)
            {
                _selectedGroup = value;
                OnPropertyChanged();
            }
        }
    }

    private Groups _selectedGroup;
    private GroupService _groupService;
    private StudentService _studentService;
    private TeacherService _teacherService;

    public GroupSettingsViewModel(GroupService groupService, INavigation navigation, ObservableCollection<Courses> courses,
        StudentService studentService, TeacherService teacherService, IDialogService dialogService, ObservableCollection<Teachers> teachers)
    {
        _groupService = groupService;
        _studentService = studentService;
        _teacherService = teacherService;

        _selectedGroup = null!;
        Groups = new ObservableCollection<Groups>();
        Courses = courses;
        Teachers = teachers;

        _ = LoadTeachersAsync();
        _ = LoadGroupsAsync();

        NavigateHomeCommand = new NavigateHomeCommand(navigation);
        CreateGroupCommand = new CreateGroupDialogCommand(this, _groupService, dialogService);
        DeleteGroupCommand = new DeleteGroupCommand(this, _groupService, _studentService, teacherService, dialogService);
        EditGroupCommand = new EditGroupDialogCommand(this, _groupService, dialogService);
        ExportStudentsToCsvCommand = new ExportStudentsToCsvCommand(this, _studentService, dialogService);
        ImportStudentsFromCsvCommand = new ImportStudentsFromCsvCommand(this, _studentService, dialogService);
        GenerateDocumentCommand = new GenerateDocumentCommand(this, studentService, dialogService);

        PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(SelectedGroup))
                OnPropertyChanged(nameof(CanClick));
        };
    }

    public bool CanClick => SelectedGroup != null;

    public async Task LoadGroupsAsync()
    {
        Groups.Clear();
        var groups = await _groupService.GetGroupsAsync();
        foreach (var group in groups)
        {
            group.Course = Courses.FirstOrDefault(c => c.Id == group.Course_ID)!;
            group.Teacher = Teachers.FirstOrDefault(c => c.Id == group.Teacher_ID);
            Groups.Add(group);
        }
    }

    public async Task LoadTeachersAsync()
    {
        Teachers.Clear();
        var teachers = await _teacherService.GetAllTeachersAsync();
        foreach (var teacher in teachers)
        {
            Teachers.Add(teacher);
        }
    }
}
