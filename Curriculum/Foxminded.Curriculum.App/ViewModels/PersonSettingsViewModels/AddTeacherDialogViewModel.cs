using Foxminded.Curriculum.App.Commands;
using Foxminded.Curriculum.Domain.Entities;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Foxminded.Curriculum.App.ViewModels.PersonSettingsViewModels;

public class AddTeacherDialogViewModel : ViewModelBase
{
    private readonly Window _dialogWindow;
    public ICommand SavePersonCommand { get; }
    public ICommand CancelCommand { get; }
    public ObservableCollection<Groups> Groups { get; }
    public bool CanSavePerson => !string.IsNullOrWhiteSpace(FirstName) && !string.IsNullOrWhiteSpace(LastName);
    public Visibility VisibilityGroupChoiceBox
    {
        get => _visibilityGroupChoiceButton;
        set
        {
            if (_visibilityGroupChoiceButton != value)
            {
                _visibilityGroupChoiceButton = value;
                OnPropertyChanged();
            }
        }
    }

    public string FirstName
    {
        get => _teacherFirstName!;
        set
        {
            if (_teacherFirstName != value)
            {
                _teacherFirstName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanSavePerson));
            }
        }
    }

    public string LastName
    {
        get => _teacherLastName!;
        set
        {
            if (_teacherLastName != value)
            {
                _teacherLastName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanSavePerson));
            }
        }
    }

    public Groups SelectedGroup
    {
        get => _selectedGroup!;
        set
        {
            if (_selectedGroup != value)
            {
                _selectedGroup = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanSavePerson));
            }
        }
    }

    private string? _teacherFirstName;
    private string? _teacherLastName;
    private Visibility _visibilityGroupChoiceButton;
    private Groups? _selectedGroup;

    public AddTeacherDialogViewModel(Window dialogWindow, TeacherSettingsDialogViewModel teacherViewModel)
    {

        Groups = teacherViewModel.Groups;
        _selectedGroup = teacherViewModel.CurrentGroup;

        _dialogWindow = dialogWindow;

        VisibilityGroupChoiceBox = Visibility.Collapsed;
        SavePersonCommand = new RelayCommand(execute => Save(), canExecute => CanSavePerson);
        CancelCommand = new RelayCommand(execute => Cancel());

        PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(FirstName) || e.PropertyName == nameof(LastName))
                OnPropertyChanged(nameof(CanSavePerson));
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
