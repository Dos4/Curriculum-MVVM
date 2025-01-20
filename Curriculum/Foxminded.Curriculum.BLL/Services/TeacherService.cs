using Foxminded.Curriculum.DAL.Repositories;
using Foxminded.Curriculum.DAL;
using Foxminded.Curriculum.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Foxminded.Curriculum.BLL.Services;

public class TeacherService
{
    private readonly IDbUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<TeacherService> _logger;
    private IUnitOfWork? _unitOfWork;
    private IGenericRepository<Teachers>? _teachersRepository;

    public TeacherService(IDbUnitOfWorkFactory unitOfWorkFactory, ILogger<TeacherService> logger)
    {
        ArgumentNullException.ThrowIfNull(unitOfWorkFactory);
        ArgumentNullException.ThrowIfNull(logger);

        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;

        _logger.LogInformation("TeacherService initialized.");
    }

    public async Task<Teachers> GetTeachersById(int teacherId)
    {
        _logger.LogInformation("Fetching teacher with ID {TeacherId}.", teacherId);
        SetupConnection();

        var teacher = await _teachersRepository!.GetByIdAsync(teacherId);

        _logger.LogInformation("teacher fetched {TeacherId}.", teacherId);
        _unitOfWork!.Dispose();

        return teacher;
    }

    public async Task<IEnumerable<Teachers>> GetAllTeachersAsync()
    {
        _logger.LogInformation("Fetching all teachers.");
        SetupConnection();

        var teachers = await _teachersRepository!.GetAllAsync();

        _logger.LogInformation("{Count} teachers fetched.", teachers.Count());
        _unitOfWork!.Dispose();

        return teachers;
    }

    public async Task AddTeacherAsync(Teachers teacher)
    {
        ArgumentNullException.ThrowIfNull(teacher);
        _logger.LogInformation("Uploading teacher {FullName}.", teacher.FullName);
        SetupConnection();

        if (await _teachersRepository!.IsDuplicateAsync(teacher))
        {
            _logger.LogWarning("Duplicate teacher name detected: {GroupName}.", teacher.FullName);
            throw new DuplicateWaitObjectException();
        }

        await _teachersRepository!.AddAsync(teacher);

        _unitOfWork?.CommitAsync();
        _logger.LogInformation("Teacher {FullName} successfully uploaded.", teacher.FullName);
    }

    public async Task UpdateTeacherInDatabaseAsync(Teachers teacher)
    {
        ArgumentNullException.ThrowIfNull(teacher);
        _logger.LogInformation("Updating teacher {FullName}.", teacher.FullName);
        SetupConnection();

        if (await _teachersRepository!.IsDuplicateAsync(teacher))
        {
            _logger.LogWarning("Duplicate group name detected: {GroupName}.", teacher.FullName);
            throw new DuplicateWaitObjectException();
        }

        await _teachersRepository!.UpdateAsync(teacher);
        await _unitOfWork!.CommitAsync();

        _logger.LogInformation("Teacher {FullName} was succesfully updated.", teacher.FullName);
    }

    public async Task DeleteTeacherFromDataBaseAsync(Teachers teacher)
    {
        ArgumentNullException.ThrowIfNull(teacher);
        _logger.LogInformation("Deleting teacher {FullName} with ID {TeacherId}.", teacher.FullName, teacher.Id);
        SetupConnection();

        await _teachersRepository!.DeleteAsync(teacher);
        await _unitOfWork!.CommitAsync();

        _logger.LogInformation("Teacher {FullName} (ID: {TeacherId}) successfully deleted.", teacher.FullName, teacher.Id);
    }

    private void SetupConnection()
    {
        _logger.LogInformation("Setting up connection and initializing repositories.");
        _unitOfWork = _unitOfWorkFactory.CreateUnitOfWork();
        _teachersRepository = _unitOfWork.Repository<Teachers>();
    }
}
