using Foxminded.Curriculum.App.Services;
using Foxminded.Curriculum.App.ViewModels.SettingsViewModels;
using Foxminded.Curriculum.App.Views.SettingsPageViews;
using Foxminded.Curriculum.BLL.Services;
using Foxminded.Curriculum.App.Resources;
using System.Windows;

namespace Foxminded.Curriculum.App.Commands.GroupSettingsCommands;

public class EditGroupDialogCommand : RelayCommand
{
    public EditGroupDialogCommand(GroupSettingsViewModel groupSettingsViewModel, GroupService groupService,
        IDialogService dialogService)
        : base(parameter => ExecuteCommand(groupSettingsViewModel, groupService, dialogService))
    {
    }

    private static void ExecuteCommand(GroupSettingsViewModel groupSettingsViewModel, GroupService groupService,
        IDialogService dialogService)
    {
        var dialogWindow = new GroupDialogView();
        var dialogViewModel = new EditGroupDialogViewModel(dialogWindow, groupSettingsViewModel);
        dialogWindow.DataContext = dialogViewModel;

        dialogWindow.Owner = Application.Current.MainWindow;
        bool? result = dialogWindow.ShowDialog();

        _ = UpdateGroupIfConfirmedAsync(result, dialogViewModel, groupSettingsViewModel, groupService, dialogService);
    }

    private static async Task UpdateGroupIfConfirmedAsync(bool? result, EditGroupDialogViewModel dialogViewModel,
        GroupSettingsViewModel groupSettingsViewModel, GroupService groupService, IDialogService dialogService)
    {
        try
        {
            if (result == true && !string.IsNullOrEmpty(dialogViewModel.GroupName))
            {
                if (dialogViewModel.GroupName != groupSettingsViewModel.SelectedGroup.Name)
                    await UpdateGroupNameAsync(groupService, groupSettingsViewModel, dialogViewModel);

                if (dialogViewModel.SelectedCourse.Id != groupSettingsViewModel.SelectedGroup.Course_ID)
                    await UpdateGroupCourseAsync(groupService, groupSettingsViewModel, dialogViewModel);

                if (dialogViewModel.SelectedTeacher.Id != groupSettingsViewModel.SelectedGroup.Teacher_ID)
                    await UpdateTeacherInDatabaseAsync(groupService, groupSettingsViewModel, dialogViewModel);

                groupSettingsViewModel.Groups.Clear();
                _ = groupSettingsViewModel.LoadGroupsAsync();
            }
        }
        catch (ArgumentNullException)
        {
            dialogService.ShowCustomError(TechnicalMessage.NullExceptionText);
        }
    }

    private static async Task UpdateGroupCourseAsync(GroupService groupService, 
        GroupSettingsViewModel groupSettingsViewModel, EditGroupDialogViewModel dialogViewModel)
    {
        groupSettingsViewModel.SelectedGroup.Course_ID = dialogViewModel.SelectedCourse.Id;
        await groupService.UpdateGroupInDatabaseAsync(groupSettingsViewModel.SelectedGroup);
    }

    private static async Task UpdateGroupNameAsync(GroupService groupService,
        GroupSettingsViewModel groupSettingsViewModel, EditGroupDialogViewModel dialogViewModel)
    {
        groupSettingsViewModel.SelectedGroup.Name = dialogViewModel.GroupName;
        await groupService.UpdateGroupInDatabaseAsync(groupSettingsViewModel.SelectedGroup);
    }

    private static async Task UpdateTeacherInDatabaseAsync(GroupService groupService,
        GroupSettingsViewModel groupSettingsViewModel, EditGroupDialogViewModel dialogViewModel)
    {
        groupSettingsViewModel.SelectedGroup.Teacher_ID = dialogViewModel.SelectedTeacher.Id;
        await groupService.UpdateGroupInDatabaseAsync(groupSettingsViewModel.SelectedGroup);
    }
}
