using Foxminded.Curriculum.App.Resources;
using Foxminded.Curriculum.App.Services;
using Foxminded.Curriculum.App.ViewModels.PersonSettingsViewModels;
using Foxminded.Curriculum.App.Views.PersonSettingsViews;
using Foxminded.Curriculum.BLL.Services;
using Foxminded.Curriculum.Domain.Entities;
using System.Windows;

namespace Foxminded.Curriculum.App.Commands.PersonSettingsCommands;

public class AddStudentDialogCommand : RelayCommand
{
    public AddStudentDialogCommand(StudentsSettingsDialogViewModel studentSettingsViewModel, StudentService service, 
        IDialogService dialogService)
        : base(parameter => ExecuteCommand(studentSettingsViewModel, service, dialogService))
    {
    }

    private static void ExecuteCommand(StudentsSettingsDialogViewModel studentSettingsViewModel, StudentService service, 
        IDialogService dialogService)
    {
        var dialogWindow = new PersonDialogView();
        var dialogViewModel = new AddStudentDialogViewModel(dialogWindow, studentSettingsViewModel);
        dialogWindow.DataContext = dialogViewModel;

        dialogWindow.Owner = Application.Current.MainWindow;
        bool? result = dialogWindow.ShowDialog();

        _ = SaveStudentIfConfirmedAsync(result, dialogViewModel, studentSettingsViewModel, service, dialogService);
    }

    private static async Task SaveStudentIfConfirmedAsync(bool? result, AddStudentDialogViewModel dialogViewModel,
        StudentsSettingsDialogViewModel studentSettingsViewModel, StudentService service, IDialogService dialogService)
    {
        try
        {
            if (result == true && !string.IsNullOrEmpty(dialogViewModel.FirstName) && !string.IsNullOrEmpty(dialogViewModel.LastName))
            {
                var newStudent = new Students
                {
                    First_Name = dialogViewModel.FirstName,
                    Last_Name = dialogViewModel.LastName,
                    Group = dialogViewModel.SelectedGroup,
                    Group_ID = dialogViewModel.SelectedGroup.Id,
                };

                await service.UploadStudentToGroupAsync(newStudent);

                studentSettingsViewModel.Students.Clear();
                _ = studentSettingsViewModel.LoadStudentsForGroupAsync(dialogViewModel.SelectedGroup.Id);
            }
        }
        catch (DuplicateWaitObjectException)
        {
            dialogService.ShowCustomError(TechnicalMessage.DuplicateExceptionStudentText);
        }
    }
}
