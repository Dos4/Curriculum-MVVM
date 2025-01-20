using Foxminded.Curriculum.BLL.Services;
using Foxminded.Curriculum.DAL;
using Foxminded.Curriculum.DAL.Repositories;
using Foxminded.Curriculum.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Foxminded.Curriculum.BLLTests;

[TestClass]
public class CourseServiceTests
{
    private Mock<IDbUnitOfWorkFactory>? _unitOfWorkFactoryMock;
    private Mock<IUnitOfWork>? _unitOfWorkMock;
    private Mock<IGenericRepository<Courses>>? _coursesRepositoryMock;
    private Mock<ILogger<CourseService>>? _loggerMock;
    private CourseService? _courseService;

    [TestInitialize]
    public void Setup()
    {
        _unitOfWorkFactoryMock = new Mock<IDbUnitOfWorkFactory>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _coursesRepositoryMock = new Mock<IGenericRepository<Courses>>();
        _loggerMock = new Mock<ILogger<CourseService>>();

        _unitOfWorkFactoryMock
            .Setup(factory => factory.CreateUnitOfWork())
            .Returns(_unitOfWorkMock.Object);

        _unitOfWorkMock
            .Setup(uow => uow.Repository<Courses>())
            .Returns(_coursesRepositoryMock.Object);

        _courseService = new CourseService(_unitOfWorkFactoryMock.Object, _loggerMock.Object);
    }

    [TestMethod]
    public async Task GetCoursesAsync_ReturnsCourses()
    {
        var coursesList = new List<Courses>
            {
                new Courses { Name = "testName1", Id = 1 },
                new Courses {Name = "testName2",  Id = 2 }
            };

        _coursesRepositoryMock!
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(coursesList);

        var result = await _courseService!.GetCoursesAsync();

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
        Assert.AreEqual(1, result.First().Id);

        _coursesRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
    }
}