using Foxminded.Curriculum.BLL.Services;
using Foxminded.Curriculum.Domain.Entities;
using System.Collections.ObjectModel;

namespace Foxminded.Curriculum.App.ViewModels.HomeViewModels;

public class TeachersViewModel : ViewModelBase
{
    public ObservableCollection<Teachers> Teachers { get; set; }
    public Teachers SelectedTeacher
    {
        get { return _selectedTeacher; }
        set
        {
            _selectedTeacher = value;
            OnPropertyChanged();
        }
    }

    private Teachers _selectedTeacher;
    private TeacherService _teacherService;
  
    public TeachersViewModel(TeacherService teacherService, ObservableCollection<Teachers> teachers)
    {
        Teachers = teachers;
        _teacherService = teacherService;
        _selectedTeacher = null!;
    }
}
