using Foxminded.Curriculum.App.Commands;
using Foxminded.Curriculum.Domain.Entities;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Foxminded.Curriculum.App.ViewModels.PersonSettingsViewModels;

public class EditStudentDialogViewModel : ViewModelBase
{
    private readonly Window _dialogWindow;

    public ObservableCollection<Groups> Groups { get; }
    public ICommand SavePersonCommand { get; }
    public ICommand CancelCommand { get; }
    public bool CanSavePerson => !string.IsNullOrWhiteSpace(FirstName) && !string.IsNullOrWhiteSpace(LastName);

    public string FirstName
    {
        get => _studentFirstName;
        set
        {
            if (_studentFirstName != value)
            {
                _studentFirstName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanSavePerson));
            }
        }
    }

    public string LastName
    {
        get => _studentLastName;
        set
        {
            if (_studentLastName != value)
            {
                _studentLastName = value;
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

    private string _studentFirstName;
    private string _studentLastName;
    private Groups? _selectedGroup;

    public EditStudentDialogViewModel(Window dialogWindow, StudentsSettingsDialogViewModel studentViewModel)
    {
        Groups = studentViewModel.Groups;
        _studentFirstName = studentViewModel.SelectedStudent.First_Name;
        _studentLastName = studentViewModel.SelectedStudent.Last_Name;
        _selectedGroup = studentViewModel.CurrentGroup;
        _dialogWindow = dialogWindow;

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
