using Foxminded.Curriculum.App.Services;
using Foxminded.Curriculum.App.ViewModels.SettingsViewModels;
using Foxminded.Curriculum.BLL.Services;
using Foxminded.Curriculum.App.Resources;

namespace Foxminded.Curriculum.App.Commands.GroupSettingsCommands;

public class DeleteGroupCommand : RelayCommand
{
    public DeleteGroupCommand(GroupSettingsViewModel groupSettingsViewModel, GroupService groupService,
        StudentService studentService, TeacherService teacherService, IDialogService dialogService)
        : base(async parameter => await ExecuteCommandAsync(groupSettingsViewModel, groupService,
            studentService,teacherService, dialogService))
    {
    }

    private static async Task ExecuteCommandAsync(GroupSettingsViewModel groupSettingsViewModel, GroupService groupService,
        StudentService studentService, TeacherService teacherService, IDialogService dialogService)
    {
        try
        {
            await CheckStudentsInGroup(studentService, groupSettingsViewModel);

            await groupService.DeleteGroupFromDataBaseAsync(groupSettingsViewModel.SelectedGroup);
            var groups = await groupService.GetGroupsAsync();

            groupSettingsViewModel.Groups.Clear();
            foreach (var group in groups)
                groupSettingsViewModel.Groups.Add(group);
        }
        catch (InvalidOperationException)
        {
            dialogService.ShowCustomError(TechnicalMessage.DeleteGroupWithStudentsInExceptionText);
        }
    }

    private static async Task CheckStudentsInGroup(StudentService studentService, 
        GroupSettingsViewModel groupSettingsViewModel)
    {
        var studentsInGroup = await studentService.GetStudentsForGroupAsync(groupSettingsViewModel.SelectedGroup.Id);
        if (studentsInGroup.Any())
            throw new InvalidOperationException();
    }
}
