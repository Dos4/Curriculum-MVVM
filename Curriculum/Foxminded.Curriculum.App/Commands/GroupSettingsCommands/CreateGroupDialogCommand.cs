using Foxminded.Curriculum.App.Services;
using Foxminded.Curriculum.App.ViewModels.SettingsViewModels;
using Foxminded.Curriculum.App.Views.SettingsPageViews;
using Foxminded.Curriculum.BLL.Services;
using Foxminded.Curriculum.Domain.Entities;
using Foxminded.Curriculum.App.Resources;
using System.Windows;

namespace Foxminded.Curriculum.App.Commands.GroupSettingsCommands;

public class CreateGroupDialogCommand : RelayCommand
{
    public CreateGroupDialogCommand(GroupSettingsViewModel groupSettingsViewModel, GroupService groupService,
        IDialogService dialogService)
        : base(parameter => ExecuteCommand(groupSettingsViewModel, groupService, dialogService))
    {
    }

    private static void ExecuteCommand(GroupSettingsViewModel groupSettingsViewModel, GroupService groupService,
        IDialogService dialogService)
    {
        var dialogWindow = new GroupDialogView();
        var dialogViewModel = new CreateGroupDialogViewModel(dialogWindow, groupSettingsViewModel);
        dialogWindow.DataContext = dialogViewModel;

        dialogWindow.Owner = Application.Current.MainWindow;
        bool? result = dialogWindow.ShowDialog();

        _ = SaveGroupIfConfirmedAsync(result, dialogViewModel, groupSettingsViewModel, groupService, dialogService);
    }

    private static async Task SaveGroupIfConfirmedAsync(bool? result, CreateGroupDialogViewModel dialogViewModel,
        GroupSettingsViewModel groupSettingsViewModel, GroupService service, IDialogService dialogService)
    {
        try
        {
            if (result == true && !string.IsNullOrEmpty(dialogViewModel.GroupName))
            {
                var groups = await service.GetGroupsAsync();

                var newGroup = new Groups
                {
                    Name = dialogViewModel.GroupName,
                    Course = dialogViewModel.SelectedCourse,
                    Course_ID = dialogViewModel.SelectedCourse.Id,
                    Teacher = dialogViewModel.SelectedTeacher,
                    Teacher_ID = dialogViewModel.SelectedTeacher.Id,
                };
                await service.AddGroupToDataBase(newGroup);

                groupSettingsViewModel.Groups.Clear();

                _ = groupSettingsViewModel.LoadGroupsAsync();
            }
        }
        catch (DuplicateWaitObjectException)
        {
            dialogService.ShowCustomError(TechnicalMessage.DuplicateExceptionGroupText);
        }
        catch (ArgumentNullException)
        {
            dialogService.ShowCustomError(TechnicalMessage.NullExceptionText);
        }
    }
}
