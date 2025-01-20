using Foxminded.Curriculum.App.Services;
using Foxminded.Curriculum.App.ViewModels.HomeViewModels;

namespace Foxminded.Curriculum.App.Commands;

public class NavigateHomeCommand : RelayCommand
{
    private readonly INavigation _navigation;

    public NavigateHomeCommand(INavigation navigation)
        : base(parameter => ExecuteCommand(navigation))
    {
        _navigation = navigation;
    }

    private static void ExecuteCommand(INavigation navigation)
    {
        navigation.NavigateTo<HomeViewModel>();
    }
}
