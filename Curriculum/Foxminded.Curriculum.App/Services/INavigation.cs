namespace Foxminded.Curriculum.App.Services;

public interface INavigation
{
    void NavigateTo<TViewModel>() where TViewModel : class;
}
