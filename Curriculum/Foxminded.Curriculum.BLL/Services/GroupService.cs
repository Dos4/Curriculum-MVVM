using Foxminded.Curriculum.DAL;
using Foxminded.Curriculum.DAL.Repositories;
using Foxminded.Curriculum.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Foxminded.Curriculum.BLL.Services;

public class GroupService
{
    private readonly IDbUnitOfWorkFactory _unitOfWorkFactory;
    private readonly ILogger<GroupService> _logger;
    private IUnitOfWork? _unitOfWork;
    private IGenericRepository<Groups>? _groupRepository;

    public GroupService(IDbUnitOfWorkFactory unitOfWorkFactory, ILogger<GroupService> logger)
    {
        ArgumentNullException.ThrowIfNull(unitOfWorkFactory);
        ArgumentNullException.ThrowIfNull(logger);

        _unitOfWorkFactory = unitOfWorkFactory;
        _logger = logger;

        _logger.LogInformation("GroupService initialized.");
    }

    public async Task<IEnumerable<Groups>> GetGroupsForCourseAsync(int id)
    {
        _logger.LogInformation("Fetching groups for course with ID {CourseId}.", id);
        SetupConnection();

        var groups = await _groupRepository!.GetByConditionAsync("COURSE_ID", id);

        _logger.LogInformation("{Count} groups fetched for course ID {CourseId}.", groups.Count(), id);
        _unitOfWork?.Dispose();
        return groups;
    }

    public async Task<IEnumerable<Groups>> GetGroupsAsync()
    {
        _logger.LogInformation("Fetching all groups.");
        SetupConnection();

        var groups = await _groupRepository!.GetAllAsync();

        _logger.LogInformation("{Count} groups fetched.", groups.Count());
        _unitOfWork?.Dispose();
        return groups;
    }

    public async Task AddGroupToDataBase(Groups group)
    {
        ArgumentNullException.ThrowIfNull(group);
        _logger.LogInformation("Adding new group to database: {GroupName}.", group.Name);
        SetupConnection();

        if (await _groupRepository!.IsDuplicateAsync(group))
        {
            _logger.LogWarning("Duplicate group name detected: {GroupName}.", group.Name);
            throw new DuplicateWaitObjectException();
        }

        await _groupRepository!.AddAsync(group);
        await _unitOfWork!.CommitAsync();

        _logger.LogInformation("Group {GroupName} successfully added to the database.", group.Name);
    }

    public async Task DeleteGroupFromDataBaseAsync(Groups group)
    {
        ArgumentNullException.ThrowIfNull(group);
        _logger.LogInformation("Deleting group with ID {GroupId} and name {GroupName}.", group.Id, group.Name);
        SetupConnection();

        await _groupRepository!.DeleteAsync(group);
        await _unitOfWork!.CommitAsync();

        _logger.LogInformation("Group {GroupName} (ID: {GroupId}) successfully deleted.", group.Name, group.Id);
    }

    public async Task UpdateGroupInDatabaseAsync(Groups group)
    {
        ArgumentNullException.ThrowIfNull(group);
        _logger.LogInformation("Updating group {GroupName}", group.Name);
        SetupConnection();

        if (await _groupRepository!.IsDuplicateAsync(group))
        {
            _logger.LogWarning("Duplicate group name detected during update: {GroupName}.", group.Name);
            throw new DuplicateWaitObjectException();
        }

        await _groupRepository!.UpdateAsync(group);
        await _unitOfWork!.CommitAsync();

        _logger.LogInformation("Group {GroupName} successfully updated", group.Name);
    }

    private void SetupConnection()
    {
        _logger.LogInformation("Setting up connection and initializing repositories.");
        _unitOfWork = _unitOfWorkFactory.CreateUnitOfWork();
        _groupRepository = _unitOfWork.Repository<Groups>();
    }
}
