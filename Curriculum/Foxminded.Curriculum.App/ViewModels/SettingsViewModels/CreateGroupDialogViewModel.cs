using Foxminded.Curriculum.App.Commands;
using Foxminded.Curriculum.Domain.Entities;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Foxminded.Curriculum.App.ViewModels.SettingsViewModels;

public class CreateGroupDialogViewModel : ViewModelBase
{
    private readonly Window _dialogWindow;

    public ObservableCollection<Courses> Courses { get; }
    public ObservableCollection<Teachers> Teachers { get; }
    public ICommand SaveGroupCommand { get; }
    public ICommand CancelCommand { get; }
    public bool CanSaveGroup => !string.IsNullOrWhiteSpace(GroupName) && SelectedCourse != null && SelectedTeacher != null;

    public string GroupName
    {
        get => _groupName;
        set
        {
            if (_groupName != value)
            {
                _groupName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanSaveGroup));
            }
        }
    }

    public Teachers SelectedTeacher
    {
        get => _selectedTeacher!;
        set
        {
            if (_selectedTeacher != value)
            {
                _selectedTeacher = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanSaveGroup));
            }
        }
    }

    public Courses SelectedCourse
    {
        get => _selectedCourse!;
        set
        {
            if (_selectedCourse != value)
            {
                _selectedCourse = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanSaveGroup));
            }
        }
    }

    private string _groupName = string.Empty;
    private Courses? _selectedCourse;
    private Teachers? _selectedTeacher;

    public CreateGroupDialogViewModel(Window dialogWindow, GroupSettingsViewModel groupSettingsViewModel)
    {
        Courses = groupSettingsViewModel.Courses;
        Teachers = groupSettingsViewModel.Teachers;
        _dialogWindow = dialogWindow;

        SaveGroupCommand = new RelayCommand(execute => Save(), canExecute => CanSaveGroup);
        CancelCommand = new RelayCommand(execute => Cancel());

        PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(GroupName) || e.PropertyName == nameof(SelectedCourse))
            {
                OnPropertyChanged(nameof(CanSaveGroup));
            }
        };
    }

    private void Save()
    {
        _dialogWindow.DialogResult = true;
        _dialogWindow.Close();
    }

    private void Cancel()
    {
        _dialogWindow.DialogResult = false;
        _dialogWindow.Close();
    }
}
