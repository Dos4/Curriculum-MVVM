using Foxminded.Curriculum.App.ViewModels;

namespace Foxminded.Curriculum.App.ViewModel;

public class MainViewModel : ViewModelBase
{
    private object? _currentViewModel;

    public object? CurrentViewModel
    {
        get => _currentViewModel;
        set
        {
            _currentViewModel = value!;
            OnPropertyChanged();
        }
    }
}
