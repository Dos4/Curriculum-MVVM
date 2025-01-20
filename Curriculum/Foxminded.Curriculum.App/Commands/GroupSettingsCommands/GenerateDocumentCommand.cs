using Foxminded.Curriculum.App.Services;
using Foxminded.Curriculum.App.ViewModels.SettingsViewModels;
using Foxminded.Curriculum.BLL.Services;
using Foxminded.Curriculum.Domain.Entities;
using Foxminded.Curriculum.App.Resources;
using Microsoft.Win32;
using PdfSharp.Drawing;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace Foxminded.Curriculum.App.Commands.GroupSettingsCommands;

public class GenerateDocumentCommand : RelayCommand
{
    public GenerateDocumentCommand(GroupSettingsViewModel groupSettingsViewModel, StudentService studentService, 
        IDialogService dialogService) 
        : base(parameter => _ = ExecuteCommand(groupSettingsViewModel, studentService, dialogService))
    {
    }

    private async static Task ExecuteCommand(GroupSettingsViewModel groupSettingsViewModel, StudentService studentService, 
        IDialogService dialogService)
    {
        SaveFileDialog saveFileDialog = new SaveFileDialog
        {
            FileName = $"{groupSettingsViewModel.SelectedGroup.Name}_Students",
            DefaultExt = ".docx",
            Filter = "Word Documents (.docx)|*.docx|PDF Documents (.pdf)|*.pdf",
            InitialDirectory = AppContext.BaseDirectory
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            var students = await studentService.GetStudentsForGroupAsync(groupSettingsViewModel.SelectedGroup.Id);
            string filePath = saveFileDialog.FileName;
            string fileType = System.IO.Path.GetExtension(filePath).ToLower();

            var course = groupSettingsViewModel.Courses.FirstOrDefault(c => c.Id == 
                groupSettingsViewModel.SelectedGroup.Course_ID);

            GenerateGroupFile(groupSettingsViewModel.SelectedGroup, students, filePath, fileType, course!, dialogService);
        }
    }

    public static void GenerateGroupFile(Groups selectedGroup, IEnumerable<Students> students, string filePath, 
        string fileType, Courses course, IDialogService dialogService)
    {
        try
        {
            if (fileType.Equals(".docx", StringComparison.OrdinalIgnoreCase))
            {
                GenerateDocxFile(selectedGroup, students, filePath, course);
            }
            else if (fileType.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                GeneratePdfFile(selectedGroup, students, filePath, course);
            }
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

    private static void GenerateDocxFile(Groups selectedGroup, IEnumerable<Students> students, string filePath, 
        Courses course)
    {
        using (var doc = DocX.Create(filePath))
        {
            doc.InsertParagraph(ProgramInterface.CoursesText + ": " + course.Name)
               .FontSize(16)
               .Bold()
               .Alignment = Alignment.center;

            doc.InsertParagraph(ProgramInterface.GroupsText + ": " + selectedGroup.Name)
               .FontSize(14)
               .Bold()
               .SpacingAfter(20);

            doc.InsertParagraph(ProgramInterface.StudentsText + ":")
           .FontSize(12)
           .Bold()
           .SpacingAfter(10);

            var numberedList = doc.AddList(null, 0, ListItemType.Numbered);
            foreach (var student in students)
            {
                doc.AddListItem(numberedList, student.FullName);
            }
            doc.InsertList(numberedList);

            doc.Save();
        }
    }

    private static void GeneratePdfFile(Groups selectedGroup, IEnumerable<Students> students, string filePath,
        Courses course)
    {
        using (var document = new PdfSharp.Pdf.PdfDocument())
        {
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var fontTitle = new XFont("Arial", 16, XFontStyleEx.Bold);
            var fontText = new XFont("Arial", 12, XFontStyleEx.Regular);

            gfx.DrawString(ProgramInterface.CoursesText + ": " + course.Name, fontTitle, XBrushes.Black,
                new XRect(0, 0, page.Width.Point, 30), XStringFormats.TopCenter);

            gfx.DrawString(ProgramInterface.GroupsText + ": " + selectedGroup.Name, fontText, XBrushes.Black,
                new XRect(40, 40, page.Width.Point, 30), XStringFormats.CenterLeft);

            gfx.DrawString(ProgramInterface.StudentsText + ": ", fontText, XBrushes.Black,
                new XPoint(40, 80));

            int yOffset = 100;
            int counter = 1;
            foreach (var student in students)
            {
                gfx.DrawString($"{counter}. {student.FullName}", fontText, XBrushes.Black,
                    new XPoint(60, yOffset));
                yOffset += 20;
                counter++;
            }

            document.Save(filePath);
        }
    }
}
