using Foxminded.Curriculum.App.Resources;
using Foxminded.Curriculum.App.Services;
using Foxminded.Curriculum.App.ViewModels.PersonSettingsViewModels;
using Foxminded.Curriculum.App.Views.PersonSettingsViews;
using Foxminded.Curriculum.BLL.Services;
using Foxminded.Curriculum.Domain.Entities;
using System.Windows;

namespace Foxminded.Curriculum.App.Commands.PersonSettingsCommands;

public class EditTeacherDialogCommand : RelayCommand
{
    public EditTeacherDialogCommand(TeacherSettingsDialogViewModel teacherSettingsViewModel, TeacherService service,
        IDialogService dialogService)
        : base(parameter => ExecuteCommand(teacherSettingsViewModel, service, dialogService))
    {
    }

    private static void ExecuteCommand(TeacherSettingsDialogViewModel teacherSettingsViewModel, TeacherService service,
        IDialogService dialogService)
    {
        var dialogWindow = new PersonDialogView();
        var dialogViewModel = new EditTeacherDialogViewModel(dialogWindow, teacherSettingsViewModel);
        dialogWindow.DataContext = dialogViewModel;

        dialogWindow.Owner = Application.Current.MainWindow;
        bool? result = dialogWindow.ShowDialog();

        try
        {
            _ = UpdateStudentIfConfirmedAsync(result, dialogViewModel, teacherSettingsViewModel, service);
        }
        catch (DuplicateWaitObjectException)
        {
            dialogService.ShowCustomError(TechnicalMessage.DuplicateExceptionTeacherText);
        }
        catch (ArgumentNullException)
        {
            dialogService.ShowCustomError(TechnicalMessage.NullExceptionText);
        }
    }

    private static async Task UpdateStudentIfConfirmedAsync(bool? result, EditTeacherDialogViewModel dialogViewModel,
        TeacherSettingsDialogViewModel teacherSettingsViewModel, TeacherService service)
    {
        if (result == true && IsValid(dialogViewModel))
        {
            var selectedTeacher = teacherSettingsViewModel.SelectedTeacher;
            var currentGroup = teacherSettingsViewModel.CurrentGroup;

            if (dialogViewModel.FirstName != selectedTeacher.First_Name && dialogViewModel.LastName != selectedTeacher.Last_Name)
            {
                await service.UpdateTeacherInDatabaseAsync(GetUpdatedTeacher(dialogViewModel));

                teacherSettingsViewModel.Teachers.Clear();
                await teacherSettingsViewModel.LoadTeacherAsync(dialogViewModel.SelectedGroup!.Teacher_ID);
            }

            teacherSettingsViewModel.Teachers.Clear();
            await teacherSettingsViewModel.LoadTeacherAsync(dialogViewModel.SelectedGroup!.Id);
        }
    }

    private static bool IsValid(EditTeacherDialogViewModel dialogViewModel)
    {
        return !string.IsNullOrEmpty(dialogViewModel.FirstName) && !string.IsNullOrEmpty(dialogViewModel.LastName);
    }

    private static Teachers GetUpdatedTeacher(EditTeacherDialogViewModel modifiedTeacher)
    {
        return new Teachers()
        {
            First_Name = modifiedTeacher.FirstName,
            Last_Name = modifiedTeacher.LastName,
        };
    }
}
