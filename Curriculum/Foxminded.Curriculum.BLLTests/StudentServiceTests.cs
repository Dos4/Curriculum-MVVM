using Foxminded.Curriculum.BLL.Services;
using Foxminded.Curriculum.DAL;
using Foxminded.Curriculum.DAL.Repositories;
using Foxminded.Curriculum.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Foxminded.Curriculum.BLLTests;

[TestClass]
public class StudentServiceTests
{
    private Mock<IDbUnitOfWorkFactory>? _unitOfWorkFactoryMock;
    private Mock<IUnitOfWork>? _unitOfWorkMock;
    private Mock<IGenericRepository<Students>>? _repositoryMock;
    private Mock<ILogger<StudentService>>? _loggerMock;
    private StudentService? _studentService;
    private Groups? _testGroup;
    private List<Students>? _testStudentsCollection;

    [TestInitialize]
    public void Setup()
    {
        _unitOfWorkFactoryMock = new Mock<IDbUnitOfWorkFactory>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _repositoryMock = new Mock<IGenericRepository<Students>>();
        _loggerMock = new Mock<ILogger<StudentService>>();
        var testCourse = new Courses { Name = "testCourse", Id = 1 };
        _testGroup = new Groups { Id = 1, Name = "testGroup", Course = testCourse, Course_ID = testCourse.Id };
        _testStudentsCollection = new List<Students>()
        {
            new Students()
            {
                Id = 1,
                First_Name = "TestFirstName1",
                Last_Name = "TestLastName1",
                Group = _testGroup,
                Group_ID = _testGroup.Id,
            },

            new Students()
            {
                Id = 2,
                First_Name = "TestFirstName2",
                Last_Name = "TestLastName2",
                Group = _testGroup,
                Group_ID = _testGroup.Id,
            }
        };

        _unitOfWorkFactoryMock
            .Setup(factory => factory.CreateUnitOfWork())
            .Returns(_unitOfWorkMock.Object);

        _unitOfWorkMock
            .Setup(uow => uow.Repository<Students>())
            .Returns(_repositoryMock.Object);

        _studentService = new StudentService(_unitOfWorkFactoryMock.Object, _loggerMock.Object);
    }

    [TestMethod]
    public async Task GetAllStudentsAsync_ReturnsAllStudents()
    {
        _repositoryMock!
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(_testStudentsCollection!);

        var result = await _studentService!.GetAllStudentsAsync();

        Assert.IsNotNull(result);

        CollectionAssert.AreEqual(_testStudentsCollection, result.ToList());
        _repositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
    }

    [TestMethod]
    public async Task GetStudentsForGroupAsync_ReturnsStudentsForSpecificGroup()
    {
        _repositoryMock!
            .Setup(r => r.GetByConditionAsync(
                It.Is<string>(columnName => columnName == "GROUP_ID"),
                It.Is<object>(value => (int)value == _testGroup!.Id)))
            .ReturnsAsync(_testStudentsCollection!);

        var result = await _studentService!.GetStudentsForGroupAsync(_testGroup!.Id);

        Assert.IsNotNull(result);

        CollectionAssert.AreEqual(_testStudentsCollection, result.ToList());
        _repositoryMock.Verify(repo => repo.GetByConditionAsync(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
    }

    [TestMethod]
    public async Task UploadStudentToGroupAsync_AddsStudentToGroup()
    {
        var newStudent = new Students
        {
            Id = 3,
            First_Name = "NewStudent",
            Last_Name = "LastName",
            Group = _testGroup!,
            Group_ID = _testGroup!.Id,
        };

        await _studentService!.UploadStudentToGroupAsync(newStudent);

        _repositoryMock!.Verify(repo => repo.IsDuplicateAsync(It.IsAny<Students>()), Times.Once);
        _repositoryMock!.Verify(r => r.AddAsync(It.Is<Students>(s => s.First_Name == newStudent.First_Name && s.Last_Name == newStudent.Last_Name &&
            s.Group_ID == newStudent.Group_ID)), Times.Once);
        _unitOfWorkMock!.Verify(uow => uow.CommitAsync(), Times.Once);
    }

    [TestMethod]
    public async Task UpdateStudentInDatabaseAsync_UpdatesStudent()
    {
        var student = _testStudentsCollection!.First();
        student.Last_Name = "NewLastName";

        _repositoryMock!
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(_testStudentsCollection!);

        await _studentService!.UpdateStudentInDatabaseAsync(student);

        _repositoryMock!.Verify(repo => repo.IsDuplicateAsync(It.IsAny<Students>()), Times.Once);
        _repositoryMock!.Verify(r => r.UpdateAsync(It.Is<Students>(s => s.First_Name == student.First_Name && s.Last_Name == student.Last_Name &&
            s.Group_ID == student.Group_ID)), Times.Once);
        _unitOfWorkMock!.Verify(uow => uow.CommitAsync(), Times.Once);
    }

    [TestMethod]
    public async Task DeleteStudentFromDataBaseAsync_DeletesStudent()
    {
        var student = _testStudentsCollection!.First();

        await _studentService!.DeleteStudentFromDataBaseAsync(student);

        _repositoryMock!.Verify(r => r.DeleteAsync(It.Is<Students>(s => s.Id == student.Id)), Times.Once);
        _unitOfWorkMock!.Verify(uow => uow.CommitAsync(), Times.Once);
    }

    [TestMethod]
    public async Task UploadStudentToGroupAsync_WhenDuplicateFound_ShouldThrowException()
    {
        var duplicateStudent = _testStudentsCollection!.First();

        _repositoryMock!.Setup(r => r.IsDuplicateAsync(It.IsAny<Students>())).Returns(Task.FromResult(true));

        await Assert.ThrowsExceptionAsync<DuplicateWaitObjectException>(() => 
            _studentService!.UploadStudentToGroupAsync(duplicateStudent));

        _repositoryMock!.Verify(repo => repo.IsDuplicateAsync(It.IsAny<Students>()), Times.Once);
        _repositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Students>()), Times.Never);
    }
}
