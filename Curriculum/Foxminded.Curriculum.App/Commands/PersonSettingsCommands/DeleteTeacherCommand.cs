using Foxminded.Curriculum.App.Resources;
using Foxminded.Curriculum.App.Services;
using Foxminded.Curriculum.App.ViewModels.PersonSettingsViewModels;
using Foxminded.Curriculum.BLL.Services;

namespace Foxminded.Curriculum.App.Commands.PersonSettingsCommands;

public class DeleteTeacherCommand : RelayCommand
{
    public DeleteTeacherCommand(TeacherSettingsDialogViewModel teacherSettingsViewModel, 
        TeacherService teacherService, IDialogService dialogService)
        : base(async parameter => await ExecuteCommandAsync(teacherSettingsViewModel, teacherService, dialogService))
    {
    }

    private static async Task ExecuteCommandAsync(TeacherSettingsDialogViewModel teacherSettingsViewModel, 
        TeacherService teacherService, IDialogService dialogService)
    {
        try
        {
            await teacherService.DeleteTeacherFromDataBaseAsync(teacherSettingsViewModel.SelectedTeacher);
        }
        catch (ArgumentNullException)
        {
            dialogService.ShowCustomError(TechnicalMessage.NullExceptionText);
        }

        teacherSettingsViewModel.Teachers.Clear();
        await teacherSettingsViewModel.LoadTeacherAsync(teacherSettingsViewModel.CurrentGroup.Id);
    }
}
