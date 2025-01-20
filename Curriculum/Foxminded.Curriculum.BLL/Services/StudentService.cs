using Foxminded.Curriculum.DAL;
using Foxminded.Curriculum.DAL.Repositories;
using Foxminded.Curriculum.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Foxminded.Curriculum.BLL.Services;

public class StudentService
{
    private readonly IDbUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<StudentService> _logger;
    private IUnitOfWork? _unitOfWork;
    private IGenericRepository<Students>? _studentsRepository;

    public StudentService(IDbUnitOfWorkFactory unitOfWorkFactory, ILogger<StudentService> logger)
    {
        ArgumentNullException.ThrowIfNull(unitOfWorkFactory);
        ArgumentNullException.ThrowIfNull(logger);

        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;

        _logger.LogInformation("StudentService initialized.");
    }

    public async Task<IEnumerable<Students>> GetAllStudentsAsync()
    {
        _logger.LogInformation("Fetching all students.");
        SetupConnection();

        var students = await _studentsRepository!.GetAllAsync();

        _logger.LogInformation("{Count} students fetched.", students.Count());
        _unitOfWork!.Dispose();
        return students;
    }

    public async Task<IEnumerable<Students>> GetStudentsForGroupAsync(int id)
    {
        _logger.LogInformation("Fetching students for group with ID {GroupId}.", id);
        SetupConnection();

        var students = await _studentsRepository!.GetByConditionAsync("GROUP_ID", id);

        _logger.LogInformation("{Count} students fetched for group ID {GroupId}.", students.Count(), id);
        _unitOfWork!.Dispose();
        return students;
    }

    public async Task UploadStudentToGroupAsync(Students student)
    {
        ArgumentNullException.ThrowIfNull(student);
        _logger.LogInformation("Uploading student {FirstName} {LastName} to group with ID {GroupId}.",
            student.First_Name, student.Last_Name, student.Group_ID);
        SetupConnection();

        if (await _studentsRepository!.IsDuplicateAsync(student))
        {
            _logger.LogWarning("Duplicate student name detected: {GroupName}.", student.FullName);
            throw new DuplicateWaitObjectException();
        }

        await _studentsRepository!.AddAsync(student);

        _unitOfWork?.CommitAsync();
        _logger.LogInformation("Student {FirstName} {LastName} successfully uploaded to group ID {GroupId}.",
            student.First_Name, student.Last_Name, student.Group_ID);
    }

    public async Task UpdateStudentInDatabaseAsync(Students student)
    {
        ArgumentNullException.ThrowIfNull(student);
        _logger.LogInformation("Updating student {StudentFullName}.", student.FullName);
        SetupConnection();

        if (await _studentsRepository!.IsDuplicateAsync(student))
        {
            _logger.LogWarning("Duplicate group name detected while updating: {GroupName}.", student.FullName);
            throw new DuplicateWaitObjectException();
        }

        await _studentsRepository!.UpdateAsync(student);
        await _unitOfWork!.CommitAsync();

        _logger.LogInformation("Student {StudentFullName} was succesfully updated", student.FullName);
    }

    public async Task DeleteStudentFromDataBaseAsync(Students student)
    {
        ArgumentNullException.ThrowIfNull(student);
        _logger.LogInformation("Deleting student {FullName} with ID {StudentId}.", student.FullName, student.Id);
        SetupConnection();

        await _studentsRepository!.DeleteAsync(student);
        await _unitOfWork!.CommitAsync();

        _logger.LogInformation("Student {FullName} (ID: {StudentId}) successfully deleted.", student.FullName, student.Id);
    }

    private void SetupConnection()
    {
        _logger.LogInformation("Setting up connection and initializing repositories.");
        _unitOfWork = _unitOfWorkFactory.CreateUnitOfWork();
        _studentsRepository = _unitOfWork.Repository<Students>();
    }
}
