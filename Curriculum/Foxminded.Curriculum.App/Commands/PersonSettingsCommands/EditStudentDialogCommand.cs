using Foxminded.Curriculum.App.Resources;
using Foxminded.Curriculum.App.Services;
using Foxminded.Curriculum.App.ViewModels.PersonSettingsViewModels;
using Foxminded.Curriculum.App.Views.PersonSettingsViews;
using Foxminded.Curriculum.BLL.Services;
using Foxminded.Curriculum.Domain.Entities;
using System.Windows;

namespace Foxminded.Curriculum.App.Commands.PersonSettingsCommands;

public class EditStudentDialogCommand : RelayCommand
{
    public EditStudentDialogCommand(StudentsSettingsDialogViewModel studentSettingsViewModel, StudentService service, 
        IDialogService dialogService)
        : base(parameter => ExecuteCommand(studentSettingsViewModel, service, dialogService))
    {
    }

    private static void ExecuteCommand(StudentsSettingsDialogViewModel studentSettingsViewModel, StudentService service, 
        IDialogService dialogService)
    {
        var dialogWindow = new PersonDialogView();
        var dialogViewModel = new EditStudentDialogViewModel(dialogWindow, studentSettingsViewModel);
        dialogWindow.DataContext = dialogViewModel;

        dialogWindow.Owner = Application.Current.MainWindow;
        bool? result = dialogWindow.ShowDialog();

        try
        {
            _ = UpdateStudentIfConfirmedAsync(result, dialogViewModel, studentSettingsViewModel, service);
        }
        catch (DuplicateWaitObjectException)
        {
            dialogService.ShowCustomError(TechnicalMessage.DuplicateExceptionStudentText);
        }
        catch (ArgumentNullException)
        {
            dialogService.ShowCustomError(TechnicalMessage.NullExceptionText);
        }
    }

    private static async Task UpdateStudentIfConfirmedAsync(bool? result, EditStudentDialogViewModel dialogViewModel,
        StudentsSettingsDialogViewModel studentSettingsViewModel, StudentService service)
    {
        if (result == true && IsValid(dialogViewModel))
        {
            var selectedStudent = studentSettingsViewModel.SelectedStudent;

            if (dialogViewModel.FirstName != selectedStudent.First_Name || dialogViewModel.LastName != selectedStudent.Last_Name ||
                dialogViewModel.SelectedGroup.Id != selectedStudent.Group_ID)
            {
                await service.UpdateStudentInDatabaseAsync(GetUpdatedStudent(dialogViewModel, selectedStudent.Id));

                studentSettingsViewModel.Students.Clear();
                await studentSettingsViewModel.LoadStudentsForGroupAsync(dialogViewModel.SelectedGroup!.Id);
            }
        }
    }

    private static bool IsValid(EditStudentDialogViewModel dialogViewModel)
    {
        return !string.IsNullOrEmpty(dialogViewModel.FirstName) && !string.IsNullOrEmpty(dialogViewModel.LastName) &&
               dialogViewModel.SelectedGroup != null;
    }

    private static Students GetUpdatedStudent(EditStudentDialogViewModel modifiedStudent, int id)
    {
        return new Students()
        {
            Id = id,
            First_Name = modifiedStudent.FirstName,
            Last_Name = modifiedStudent.LastName,
            Group = modifiedStudent.SelectedGroup,
            Group_ID = modifiedStudent.SelectedGroup.Id,
        };
    }
}
