using Foxminded.Curriculum.App.Commands;
using Foxminded.Curriculum.App.Services;
using Foxminded.Curriculum.BLL.Services;
using Foxminded.Curriculum.Domain.Entities;
using System.Windows;
using System.Windows.Input;

namespace Foxminded.Curriculum.App.ViewModels.HomeViewModels;

public class HomeViewModel : ViewModelBase
{
    public CoursesViewModel CoursesViewModel { get; }
    public GroupsViewModel GroupsViewModel { get; }
    public TeachersViewModel TeachersViewModel { get; }
    public StudentsViewModel StudentsViewModel { get; }

    public Visibility VisibilityGroupSettingsButton
    {
        get => _visibilityGroupSettingsButton;
        set
        {
            if (_visibilityGroupSettingsButton != value)
            {
                _visibilityGroupSettingsButton = value;
                OnPropertyChanged();
            }
        }
    }

    public Visibility VisibilityPersonSettingsButton
    {
        get => _visibilityPersonSettingsButton;
        set
        {
            if (_visibilityPersonSettingsButton != value)
            {
                _visibilityPersonSettingsButton = value;
                OnPropertyChanged();
            }
        }
    }
    public ICommand NavigateGroupSettingsCommand { get; }
    public ICommand OpenStudentsSettingsDialogCommand { get; }
    public ICommand OpenTeachersSettingsDialogCommand { get; }

    private GroupService _groupService;
    private StudentService _studentService;
    private TeacherService _teacherService;

    private Visibility _visibilityGroupSettingsButton;
    private Visibility _visibilityPersonSettingsButton;

    public HomeViewModel(CoursesViewModel coursesViewModel,
                            GroupsViewModel groupsViewModel,
                                TeachersViewModel teachersViewModel,
                                    StudentsViewModel studentsViewModel, GroupService groupService,
                                        StudentService studentService, TeacherService teacherService,
                                            INavigation navigation, IDialogService dialogService)
    {
        CoursesViewModel = coursesViewModel;
        GroupsViewModel = groupsViewModel;
        TeachersViewModel = teachersViewModel;
        StudentsViewModel = studentsViewModel;

        _groupService = groupService;
        _studentService = studentService;
        _teacherService = teacherService;

        _visibilityGroupSettingsButton = Visibility.Hidden;
        _visibilityPersonSettingsButton = Visibility.Hidden;

        NavigateGroupSettingsCommand = new NavigateGroupSettingsCommand(navigation);
        OpenStudentsSettingsDialogCommand = new OpenStudentsSettingsDialogCommand(GroupsViewModel, StudentsViewModel, 
            _studentService, dialogService);
        OpenTeachersSettingsDialogCommand = new OpenTeachersSettingsDialogCommand(GroupsViewModel, TeachersViewModel, 
            _teacherService, dialogService);

        TeachersViewModel.Teachers.Clear();
        CoursesViewModel.PropertyChanged += CoursesViewModel_PropertyChanged!;
        GroupsViewModel.PropertyChanged += GroupsViewModel_PropertyChanged!;
    }

    private async void CoursesViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CoursesViewModel.SelectedCourse))
        {
            await UpdateGroupsForSelectedCourse(CoursesViewModel.SelectedCourse);
            VisibilityGroupSettingsButton = Visibility.Visible;
            VisibilityPersonSettingsButton = Visibility.Hidden;
        }
    }

    private async void GroupsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(GroupsViewModel.SelectedGroup))
        {
            await UpdateStudentsForSelectedGroup(GroupsViewModel.SelectedGroup);
            await UpdateTeacherForSelectedGroup(GroupsViewModel.SelectedGroup);
            VisibilityPersonSettingsButton = Visibility.Visible;
        }
    }

    private async Task UpdateGroupsForSelectedCourse(Courses selectedCourse)
    {
        if (selectedCourse != null)
        {
            GroupsViewModel.Groups.Clear();
            StudentsViewModel.Students.Clear();
            TeachersViewModel.Teachers.Clear();

            var groupsForCourse = await _groupService.GetGroupsForCourseAsync(selectedCourse.Id);
            foreach (var group in groupsForCourse)
                GroupsViewModel.Groups.Add(group);
        }
    }

    private async Task UpdateStudentsForSelectedGroup(Groups selectedGroup)
    {
        if (selectedGroup != null)
        {
            StudentsViewModel.Students.Clear();

            var studentsForGroup = await _studentService.GetStudentsForGroupAsync(selectedGroup.Id);
            foreach (var student in studentsForGroup)
                StudentsViewModel.Students.Add(student);
        }
    }

    private async Task UpdateTeacherForSelectedGroup(Groups selectedGroup)
    {
        if (selectedGroup != null)
        {
            TeachersViewModel.Teachers.Clear();

            var TeacherInGroup = await _teacherService.GetTeachersById(selectedGroup.Teacher_ID);
            TeachersViewModel.Teachers.Add(TeacherInGroup);
        }
    }
}
