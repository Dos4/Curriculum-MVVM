using Foxminded.Curriculum.BLL.Services;
using Foxminded.Curriculum.Domain.Entities;
using System.Collections.ObjectModel;

namespace Foxminded.Curriculum.App.ViewModels.HomeViewModels;

public class CoursesViewModel : ViewModelBase
{
    public ObservableCollection<Courses> Courses { get; set; }
    public Courses SelectedCourse
    {
        get { return _selectedCourse; }
        set
        {
            _selectedCourse = value;
            OnPropertyChanged();
        }
    }

    private Courses _selectedCourse;
    private CourseService _courseService;

    public CoursesViewModel(CourseService courseService, ObservableCollection<Courses> courses)
    {
        Courses = courses;
        _selectedCourse = null!;
        _courseService = courseService;

        _ = LoadCoursesAsync();
    }

    private async Task LoadCoursesAsync()
    {
        var courses = await _courseService.GetCoursesAsync();
        foreach (var course in courses)
        {
            Courses.Add(course);
        }
    }
}

