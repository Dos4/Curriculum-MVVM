using Foxminded.Curriculum.DAL;
using Foxminded.Curriculum.DAL.Repositories;
using Foxminded.Curriculum.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Foxminded.Curriculum.BLL.Services;

public class CourseService
{
    private readonly ILogger<CourseService> _logger;
    private IUnitOfWork _unitOfWork;
    private IGenericRepository<Courses> _courses;

    public CourseService(IDbUnitOfWorkFactory unitOfWorkFactory, ILogger<CourseService> logger)
    {
        ArgumentNullException.ThrowIfNull(unitOfWorkFactory);
        ArgumentNullException.ThrowIfNull(logger);
        
        _logger = logger;
        _unitOfWork = unitOfWorkFactory.CreateUnitOfWork();
        _courses = _unitOfWork.Repository<Courses>();

        _logger.LogInformation("CourseService initialized.");
    }

    public async Task<IEnumerable<Courses>> GetCoursesAsync()
    {
         _logger.LogInformation("Fetching all courses.");
        return await _courses.GetAllAsync();
    }
}
