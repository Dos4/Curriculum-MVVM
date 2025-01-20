using Foxminded.Curriculum.App.Services;
using Foxminded.Curriculum.App.ViewModels.SettingsViewModels;

namespace Foxminded.Curriculum.App.Commands;

public class NavigateGroupSettingsCommand : RelayCommand
{
    private readonly INavigation _navigation;

    public NavigateGroupSettingsCommand(INavigation navigation)
        : base(parameter => ExecuteCommand(navigation))
    {
        _navigation = navigation;
    }

    private static void ExecuteCommand(INavigation navigation)
    {
        navigation.NavigateTo<GroupSettingsViewModel>();
    }
}
