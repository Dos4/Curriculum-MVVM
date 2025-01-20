namespace Foxminded.Curriculum.DAL;

public interface IDbUnitOfWorkFactory
{
    IUnitOfWork CreateUnitOfWork();
}
