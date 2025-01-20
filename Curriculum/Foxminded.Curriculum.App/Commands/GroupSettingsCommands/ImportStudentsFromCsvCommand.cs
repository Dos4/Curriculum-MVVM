using Foxminded.Curriculum.App.Services;
using Foxminded.Curriculum.App.ViewModels.SettingsViewModels;
using Foxminded.Curriculum.BLL.Services;
using Foxminded.Curriculum.Domain.Entities;
using Foxminded.Curriculum.App.Resources;
using System.IO;
using System.Text;

namespace Foxminded.Curriculum.App.Commands.GroupSettingsCommands;

public class ImportStudentsFromCsvCommand : RelayCommand
{
    public ImportStudentsFromCsvCommand(GroupSettingsViewModel groupSettingsViewModel, StudentService studentService, 
        IDialogService dialogService) 
        : base(parameter => _ = ExecuteCommandAsync(groupSettingsViewModel, studentService, dialogService))
    {
    }

    private static async Task ExecuteCommandAsync(GroupSettingsViewModel groupSettingsViewModel, 
        StudentService studentService, IDialogService dialogService)
    {
        try
        {
            await CheckGroupForStudents(studentService, groupSettingsViewModel);

            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv",
                Title = ProgramInterface.ImportFromCSVText,
            };

            if (openFileDialog.ShowDialog() != true) return;

            string filePath = openFileDialog.FileName;

            var studentsToImport = await ParseCsvFileAsync(filePath, groupSettingsViewModel);
            foreach (var student in studentsToImport)
            {
                student.Group_ID = groupSettingsViewModel.SelectedGroup.Id;
                await studentService.UploadStudentToGroupAsync(student);
            }
        }
        catch (InvalidOperationException)
        {
            dialogService.ShowCustomError(TechnicalMessage.DeleteGroupWithStudentsInExceptionText);
        }
        catch (UnauthorizedAccessException)
        {
            dialogService.ShowCustomError(TechnicalMessage.AccessToDirectoryExceptionText);
        }
        catch (ArgumentException)
        {
            dialogService.ShowCustomError(TechnicalMessage.CantFoundPathExceptionText);
        }
        catch(FormatException)
        {
            dialogService.ShowCustomError(TechnicalMessage.FormatExceptionText);
        }
    }

    private static async Task<IEnumerable<Students>> ParseCsvFileAsync(string filePath, GroupSettingsViewModel viewModel)
    {
        var lines = await File.ReadAllLinesAsync(filePath, Encoding.UTF8);

        var studentsToImport = new List<Students>();

        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            var parts = line.Split(',');

            if (parts.Length != 2)
                throw new FormatException();

            var firstName = parts[0].Trim();
            var lastName = parts[1].Trim();

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
                throw new FormatException();

            var student = new Students
            {
                First_Name = firstName,
                Last_Name = lastName,
                Group = viewModel.SelectedGroup,
                Group_ID = viewModel.SelectedGroup.Id,
            };

            studentsToImport.Add(student);
        }

        return studentsToImport;
    }

    private static async Task CheckGroupForStudents(StudentService studentService, 
        GroupSettingsViewModel groupSettingsViewModel)
    {
        var studentsInGroup = await studentService.GetStudentsForGroupAsync(groupSettingsViewModel.SelectedGroup.Id);
        if (studentsInGroup.Any())
            throw new InvalidOperationException();
    }
}
