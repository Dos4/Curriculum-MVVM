using Foxminded.Curriculum.BLL.Services;
using Foxminded.Curriculum.DAL;
using Foxminded.Curriculum.DAL.Repositories;
using Foxminded.Curriculum.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Foxminded.Curriculum.BLLTests;

[TestClass]
public class GroupServiceTests
{
    private Mock<IDbUnitOfWorkFactory>? _unitOfWorkFactoryMock;
    private Mock<IUnitOfWork>? _unitOfWorkMock;
    private Mock<IGenericRepository<Groups>>? _repositoryMock;
    private Mock<ILogger<GroupService>>? _loggerMock;
    private GroupService? _groupService;
    private Courses? _testCourse;
    private Teachers? _testTeacher;
    private List<Groups>? _testGroupsCollection;

    [TestInitialize]
    public void Setup()
    {
        _unitOfWorkFactoryMock = new Mock<IDbUnitOfWorkFactory>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _repositoryMock = new Mock<IGenericRepository<Groups>>();
        _loggerMock = new Mock<ILogger<GroupService>>();
        _testCourse = new Courses { Name = "testCourse", Id = 1 };
        _testTeacher = new Teachers { First_Name = "testTeacherName", Last_Name = "testTeacherLastName", Id = 1 };
        _testGroupsCollection = new List<Groups>()
        {
            new Groups()
            {
                Name = "TestGroup1",
                Course = _testCourse,
                Course_ID = _testCourse.Id,
                Teacher = _testTeacher,
                Teacher_ID = _testTeacher.Id,
            },

            new Groups()
            {
                Name = "TestGroup2",
                Course = _testCourse,
                Course_ID = _testCourse.Id,
                Teacher = _testTeacher,
                Teacher_ID = _testTeacher.Id,
            },
        };

        _unitOfWorkFactoryMock
            .Setup(factory => factory.CreateUnitOfWork())
            .Returns(_unitOfWorkMock.Object);

        _unitOfWorkMock
            .Setup(uow => uow.Repository<Groups>())
            .Returns(_repositoryMock.Object);

        _groupService = new GroupService(_unitOfWorkFactoryMock.Object, _loggerMock.Object);
    }

    [TestMethod]
    public async Task GetGroupsForCourseAsync_ReturnsGroups()
    {
        _repositoryMock!
            .Setup(r => r.GetByConditionAsync(
                It.Is<string>(columnName => columnName == "COURSE_ID"),
                It.Is<object>(value => (int)value == _testCourse!.Id)))
            .ReturnsAsync(_testGroupsCollection!);

        var result = await _groupService!.GetGroupsForCourseAsync(_testCourse!.Id);

        Assert.AreEqual(2, result.Count());
        Assert.AreEqual("TestGroup1", result.First().Name);
        Assert.AreEqual(_testTeacher!.Id, result.First().Teacher_ID);

        _unitOfWorkMock!.Verify(u => u.Dispose(), Times.Once());
    }

    [TestMethod]
    public async Task GetGroupsAsync_ReturnsGroups()
    {
        _repositoryMock!
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(_testGroupsCollection!);

        var result = await _groupService!.GetGroupsAsync();

        Assert.AreEqual(2, result.Count());
        Assert.AreEqual("TestGroup1", result.First().Name);
        Assert.AreEqual(_testTeacher!.Id, result.First().Teacher_ID);

        _unitOfWorkMock!.Verify(u => u.Dispose(), Times.Once());
    }

    [TestMethod]
    public async Task AddGroupToDataBase_ShouldInsertGroup()
    {
        var group = new Groups 
        { 
            Id = 1, 
            Name = "New Group", 
            Course_ID = _testCourse!.Id, 
            Course = _testCourse, 
            Teacher = _testTeacher, 
            Teacher_ID = _testTeacher!.Id,
        };

        await _groupService!.AddGroupToDataBase(group);

        _repositoryMock!.Verify(repo => repo.IsDuplicateAsync(It.IsAny<Groups>()), Times.Once);
        _repositoryMock!.Verify(r => r.AddAsync(It.Is<Groups>(g => g.Name == group.Name && g.Course_ID == group.Course_ID &&
            g.Teacher_ID == group.Teacher_ID)), Times.Once);
        _unitOfWorkMock!.Verify(uow => uow.CommitAsync(), Times.Once);
    }

    [TestMethod]
    public async Task DeleteGroupFromDataBaseAsync_ShouldDeleteGroup()
    {
        var group = new Groups { Id = 1, Name = "Group to Delete", Course_ID = _testCourse!.Id, Course = _testCourse };

        await _groupService!.DeleteGroupFromDataBaseAsync(group);

        _repositoryMock!.Verify(r => r.DeleteAsync(It.Is<Groups>(g => g.Id == group.Id)), Times.Once);
        _unitOfWorkMock!.Verify(uow => uow.CommitAsync(), Times.Once);
    }

    [TestMethod]
    public async Task UpdateGroupInDatabaseAsync_ShouldUpdateName()
    {
        var updatedGroup = new Groups 
        { 
            Name = "Updated Group", 
            Course = new Courses { Name = "newCourse", Id = 2 },
            Course_ID = _testCourse!.Id, 
            Teacher = _testTeacher, 
            Teacher_ID = _testTeacher!.Id,
        };

        await _groupService!.UpdateGroupInDatabaseAsync(updatedGroup);

        _repositoryMock!.Verify(repo => repo.IsDuplicateAsync(It.IsAny<Groups>()), Times.Once);
        _repositoryMock!.Verify(r => r.UpdateAsync(It.Is<Groups>(g => g.Name == updatedGroup.Name && g.Course_ID == updatedGroup.Course_ID 
            && g.Teacher_ID == updatedGroup.Teacher_ID)), Times.Once);
        _unitOfWorkMock!.Verify(uow => uow.CommitAsync(), Times.Once);
    }

    [TestMethod]
    public async Task AddGroupToDataBase_WhenDuplicateFound_ShouldThrowException()
    {
        var group = new Groups { Id = 1, Name = "Group 1", Course = _testCourse!, Course_ID = _testCourse!.Id };
        var groupsList = new List<Groups>
        {
            new Groups { Id = 2, Name = "Group 1", Course = _testCourse, Course_ID = _testCourse.Id }
        };

        _repositoryMock!.Setup(r => r.IsDuplicateAsync(It.IsAny<Groups>())).Returns(Task.FromResult(true));

        await Assert.ThrowsExceptionAsync<DuplicateWaitObjectException>(() => _groupService!.AddGroupToDataBase(group));

        _repositoryMock!.Verify(repo => repo.IsDuplicateAsync(It.IsAny<Groups>()), Times.Once);
        _repositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Groups>()), Times.Never);
    }
}
