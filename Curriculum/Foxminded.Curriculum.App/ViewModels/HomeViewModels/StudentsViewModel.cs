using Foxminded.Curriculum.BLL.Services;
using Foxminded.Curriculum.Domain.Entities;
using System.Collections.ObjectModel;

namespace Foxminded.Curriculum.App.ViewModels.HomeViewModels;

public class StudentsViewModel : ViewModelBase
{
    public ObservableCollection<Students> Students { get; set; }
    public Students SelectedStudent
    {
        get { return _selectedStudent; }
        set
        {
            _selectedStudent = value;
            OnPropertyChanged();
        }
    }

    private Students _selectedStudent;
    private StudentService _studentService;

    public StudentsViewModel(StudentService studentService)
    {
        Students = new ObservableCollection<Students>();
        _studentService = studentService;
        _selectedStudent = null!;
    }
}
