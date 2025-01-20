using Foxminded.Curriculum.App.Services;
using Foxminded.Curriculum.App.ViewModels.SettingsViewModels;
using Foxminded.Curriculum.BLL.Services;
using Foxminded.Curriculum.App.Resources;
using Microsoft.Win32;
using System.IO;
using System.Text;

namespace Foxminded.Curriculum.App.Commands.GroupSettingsCommands;

public class ExportStudentsToCsvCommand : RelayCommand
{
    public ExportStudentsToCsvCommand(GroupSettingsViewModel groupSettingsViewModel, StudentService studentService, 
        IDialogService dialogService) 
        : base(parameter => _ = ExecuteCommandAsync(groupSettingsViewModel, studentService, dialogService))
    {
    }

    private static async Task ExecuteCommandAsync(GroupSettingsViewModel groupSettingsViewModel, 
        StudentService studentService, IDialogService dialogService)
    {
        SaveFileDialog saveFileDialog = new SaveFileDialog
        {
            FileName = $"Students_{groupSettingsViewModel.SelectedGroup.Name}.csv",
            DefaultExt = ".csv",
            Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
            InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            string filePath = saveFileDialog.FileName;
            await ExportStudentsToFileAsync(groupSettingsViewModel.SelectedGroup.Id, studentService, filePath, dialogService);
        }
    }

    private static async Task ExportStudentsToFileAsync(int groupId, StudentService studentService, string filePath,
        IDialogService dialogService)
    {
        try
        {
            var students = await studentService.GetStudentsForGroupAsync(groupId);

            var studentData = new List<string> { $"{ProgramInterface.FirstNameText}, {ProgramInterface.LastNameText}" };

            studentData.AddRange(students.Select(s => $"{s.First_Name}, {s.Last_Name}"));

            await File.WriteAllLinesAsync(filePath, studentData, Encoding.UTF8);
        }
        catch (UnauthorizedAccessException)
        {
            dialogService.ShowCustomError(TechnicalMessage.AccessToDirectoryExceptionText);
        }
        catch (ArgumentException)
        {
            dialogService.ShowCustomError(TechnicalMessage.CantFoundPathExceptionText);
        }
    }
}
