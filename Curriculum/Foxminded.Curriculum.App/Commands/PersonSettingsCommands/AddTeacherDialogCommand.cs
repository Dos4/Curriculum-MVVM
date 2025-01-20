using Foxminded.Curriculum.App.Resources;
using Foxminded.Curriculum.App.Services;
using Foxminded.Curriculum.App.ViewModels.PersonSettingsViewModels;
using Foxminded.Curriculum.App.Views.PersonSettingsViews;
using Foxminded.Curriculum.BLL.Services;
using Foxminded.Curriculum.Domain.Entities;
using System.Windows;

namespace Foxminded.Curriculum.App.Commands.PersonSettingsCommands;

public class AddTeacherDialogCommand : RelayCommand
{
    public AddTeacherDialogCommand(TeacherSettingsDialogViewModel teacherSettingsViewModel, TeacherService service, 
        IDialogService dialogService)
        : base(parameter => ExecuteCommand(teacherSettingsViewModel, service, dialogService))
    {
    }

    private static void ExecuteCommand(TeacherSettingsDialogViewModel teacherSettingsViewModel, TeacherService service,
        IDialogService dialogService)
    {
        var dialogWindow = new PersonDialogView();
        var dialogViewModel = new AddTeacherDialogViewModel(dialogWindow, teacherSettingsViewModel);
        dialogWindow.DataContext = dialogViewModel;

        dialogWindow.Owner = Application.Current.MainWindow;
        bool? result = dialogWindow.ShowDialog();

        _ = SaveStudentIfConfirmedAsync(result, dialogViewModel, teacherSettingsViewModel, service, dialogService);
    }

    private static async Task SaveStudentIfConfirmedAsync(bool? result, AddTeacherDialogViewModel dialogViewModel,
        TeacherSettingsDialogViewModel teachersSettigsViewModel, TeacherService service, IDialogService dialogService)
    {
        try
        {
            if (result == true && !string.IsNullOrEmpty(dialogViewModel.FirstName) && !string.IsNullOrEmpty(dialogViewModel.LastName))
            {
                var newTeacher = new Teachers
                {
                    First_Name = dialogViewModel.FirstName,
                    Last_Name = dialogViewModel.LastName,
                };
                await service.AddTeacherAsync(newTeacher);

                teachersSettigsViewModel.Teachers.Clear();
                _ = teachersSettigsViewModel.LoadTeacherAsync(dialogViewModel.SelectedGroup.Teacher_ID);
            }
        }
        catch (DuplicateWaitObjectException)
        {
            dialogService.ShowCustomError(TechnicalMessage.DuplicateExceptionTeacherText);
        }
    }
}
