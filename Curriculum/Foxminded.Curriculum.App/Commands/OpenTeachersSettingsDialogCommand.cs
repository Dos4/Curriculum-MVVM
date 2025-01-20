using Foxminded.Curriculum.App.Services;
using Foxminded.Curriculum.App.ViewModels.HomeViewModels;
using Foxminded.Curriculum.App.ViewModels.PersonSettingsViewModels;
using Foxminded.Curriculum.App.Views.PersonSettingsViews;
using Foxminded.Curriculum.BLL.Services;
using System.Windows;

namespace Foxminded.Curriculum.App.Commands;

public class OpenTeachersSettingsDialogCommand : RelayCommand
{
    public OpenTeachersSettingsDialogCommand(GroupsViewModel groupsViewModel, TeachersViewModel teachersViewModel, 
        TeacherService teachersService, IDialogService dialogService)
        : base(parameter => ExecuteCommand(groupsViewModel, teachersViewModel, teachersService, dialogService))
    {
    }

    private static void ExecuteCommand(GroupsViewModel groupsViewModel, TeachersViewModel teachersViewModel, 
        TeacherService teachersService, IDialogService dialogService)
    {
        var dialogWindow = new TeachersSettingsDialogView();
        var dialogViewModel = new TeacherSettingsDialogViewModel(dialogWindow, groupsViewModel, teachersViewModel, 
            teachersService, dialogService);
        dialogWindow.DataContext = dialogViewModel;

        dialogWindow.Owner = Application.Current.MainWindow;
        bool? result = dialogWindow.ShowDialog();
    }
}
