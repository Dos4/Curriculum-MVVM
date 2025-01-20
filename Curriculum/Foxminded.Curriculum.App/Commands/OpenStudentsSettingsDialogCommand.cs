using Foxminded.Curriculum.App.Services;
using Foxminded.Curriculum.App.ViewModels.HomeViewModels;
using Foxminded.Curriculum.App.ViewModels.PersonSettingsViewModels;
using Foxminded.Curriculum.App.Views.PersonSettingsViews;
using Foxminded.Curriculum.BLL.Services;
using System.Windows;

namespace Foxminded.Curriculum.App.Commands;

public class OpenStudentsSettingsDialogCommand : RelayCommand
{
    public OpenStudentsSettingsDialogCommand(GroupsViewModel groupsViewModel, StudentsViewModel studentsViewModel, 
        StudentService studentService, IDialogService dialogService) 
        : base(parameter => ExecuteCommand(groupsViewModel,studentsViewModel, studentService, dialogService))
    {
    }

    private static void ExecuteCommand(GroupsViewModel groupsViewModel, StudentsViewModel studentsViewModel, 
        StudentService studentService, IDialogService dialogService)
    {
        var dialogWindow = new StudentsSettingsDialogView();
        var dialogViewModel = new StudentsSettingsDialogViewModel(dialogWindow, groupsViewModel, studentsViewModel, 
            studentService, dialogService);
        dialogWindow.DataContext = dialogViewModel;

        dialogWindow.Owner = Application.Current.MainWindow;
        bool? result = dialogWindow.ShowDialog();
    }
}
