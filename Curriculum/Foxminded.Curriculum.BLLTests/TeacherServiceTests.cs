using Foxminded.Curriculum.BLL.Services;
using Foxminded.Curriculum.DAL;
using Foxminded.Curriculum.DAL.Repositories;
using Foxminded.Curriculum.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Foxminded.Curriculum.BLLTests;

[TestClass]
public class TeacherServiceTests
{
    private Mock<IDbUnitOfWorkFactory>? _unitOfWorkFactoryMock;
    private Mock<IUnitOfWork>? _unitOfWorkMock;
    private Mock<IGenericRepository<Teachers>>? _teachersRepositoryMock;
    private Mock<ILogger<TeacherService>>? _loggerMock;
    private TeacherService? _teacherService;
    private List<Teachers>? _testTeachersCollection;

    [TestInitialize]
    public void Setup()
    {
        _unitOfWorkFactoryMock = new Mock<IDbUnitOfWorkFactory>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _teachersRepositoryMock = new Mock<IGenericRepository<Teachers>>();
        _loggerMock = new Mock<ILogger<TeacherService>>();
        _testTeachersCollection = new List<Teachers>
        {
            new Teachers 
            { 
                Id = 1, 
                First_Name = "TestFirstName1", 
                Last_Name = "TestLastName1", 
            },
            new Teachers 
            { 
                Id = 2, 
                First_Name = "TestFirstName2", 
                Last_Name = "TestLastName2", 
            }
        };

        _unitOfWorkFactoryMock
            .Setup(factory => factory.CreateUnitOfWork())
            .Returns(_unitOfWorkMock.Object);

        _unitOfWorkMock
            .Setup(uow => uow.Repository<Teachers>())
            .Returns(_teachersRepositoryMock.Object);

        _teacherService = new TeacherService(_unitOfWorkFactoryMock.Object, _loggerMock.Object);
    }

    [TestMethod]
    public async Task GetAllTeachersAsync_ReturnsAllTeachers()
    {
        _teachersRepositoryMock!
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(_testTeachersCollection!);

        var result = await _teacherService!.GetAllTeachersAsync();

        Assert.IsNotNull(result);
        CollectionAssert.AreEqual(_testTeachersCollection, result.ToList());
        _teachersRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    [TestMethod]
    public async Task UploadTeacherInDatabaseAsync_AddsTeacherToGroup()
    {
        var newTeacher = new Teachers
        {
            Id = 3,
            First_Name = "NewTeacher",
            Last_Name = "LastName",
        };

        await _teacherService!.AddTeacherAsync(newTeacher);

        _teachersRepositoryMock!.Verify(repo => repo.IsDuplicateAsync(It.IsAny<Teachers>()), Times.Once);
        _teachersRepositoryMock!.Verify(r => r.AddAsync(It.Is<Teachers>(t => t.First_Name == newTeacher.First_Name && t.Last_Name == newTeacher.Last_Name)), 
            Times.Once);
        _unitOfWorkMock!.Verify(uow => uow.CommitAsync(), Times.Once);
    }

    [TestMethod]
    public async Task UpdateTeacherInDatabaseAsync_UpdatesTeacherName()
    {
        var teacher = _testTeachersCollection!.First();

        await _teacherService!.UpdateTeacherInDatabaseAsync(teacher);

        _teachersRepositoryMock!.Verify(repo => repo.IsDuplicateAsync(It.IsAny<Teachers>()), Times.Once);
        _teachersRepositoryMock!.Verify(r => r.UpdateAsync(It.Is<Teachers>(t => t.First_Name == teacher.First_Name && t.Last_Name == teacher.Last_Name)), 
            Times.Once);
        _unitOfWorkMock!.Verify(uow => uow.CommitAsync(), Times.Once);
    }

    [TestMethod]
    public async Task DeleteTeacherFromDataBaseAsync_DeletesTeacher()
    {
        var teacher = _testTeachersCollection!.First();

        await _teacherService!.DeleteTeacherFromDataBaseAsync(teacher);

        _teachersRepositoryMock!.Verify(r => r.DeleteAsync(It.Is<Teachers>(t => t.Id == teacher.Id)), Times.Once);
        _unitOfWorkMock!.Verify(uow => uow.CommitAsync(), Times.Once);
    }

    [TestMethod]
    public async Task UploadTeacherAsync_WhenDuplicateFound_ShouldThrowException()
    {
        var duplicateTeacher = _testTeachersCollection!.First();

        _teachersRepositoryMock!.Setup(r => r.IsDuplicateAsync(It.IsAny<Teachers>())).Returns(Task.FromResult(true));

        await Assert.ThrowsExceptionAsync<DuplicateWaitObjectException>(() =>
            _teacherService!.UpdateTeacherInDatabaseAsync(duplicateTeacher));

        _teachersRepositoryMock!.Verify(repo => repo.IsDuplicateAsync(It.IsAny<Teachers>()), Times.Once);
        _teachersRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Teachers>()), Times.Never);
    }
}
