using Foxminded.Curriculum.App.Resources;
using Foxminded.Curriculum.App.Services;
using Foxminded.Curriculum.App.ViewModels.PersonSettingsViewModels;
using Foxminded.Curriculum.BLL.Services;

namespace Foxminded.Curriculum.App.Commands.PersonSettingsCommands;

public class DeleteStudentCommand : RelayCommand
{
    public DeleteStudentCommand(StudentsSettingsDialogViewModel studentSettingsViewModel, 
        StudentService studentService, IDialogService dialogService)
        : base(async parameter => await ExecuteCommandAsync(studentSettingsViewModel, studentService, dialogService))
    {
    }

    private static async Task ExecuteCommandAsync(StudentsSettingsDialogViewModel studentSettingsViewModel, 
        StudentService studentService, IDialogService dialogService)
    {
        try
        {
            await studentService.DeleteStudentFromDataBaseAsync(studentSettingsViewModel.SelectedStudent);
        }
        catch (ArgumentNullException)
        {
            dialogService.ShowCustomError(TechnicalMessage.NullExceptionText);
        }

        studentSettingsViewModel.Students.Clear();
        await studentSettingsViewModel.LoadStudentsForGroupAsync(studentSettingsViewModel.CurrentGroup.Id);
    }
}
