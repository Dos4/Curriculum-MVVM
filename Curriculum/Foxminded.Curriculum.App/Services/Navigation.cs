using Foxminded.Curriculum.App.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace Foxminded.Curriculum.App.Services;

public class Navigation : INavigation
{
    private readonly IServiceProvider _serviceProvider;
    private readonly MainViewModel _mainViewModel;

    public Navigation(IServiceProvider serviceProvider, MainViewModel mainViewModel)
    {
        _serviceProvider = serviceProvider;
        _mainViewModel = mainViewModel;
    }

    public void NavigateTo<TViewModel>() where TViewModel : class
    {
        _mainViewModel.CurrentViewModel = _serviceProvider.GetRequiredService<TViewModel>();
    }
}
