using Foxminded.Curriculum.BLL.Services;
using Foxminded.Curriculum.Domain.Entities;
using System.Collections.ObjectModel;

namespace Foxminded.Curriculum.App.ViewModels.HomeViewModels;

public class GroupsViewModel : ViewModelBase
{
    public ObservableCollection<Groups> Groups { get; set; }
    public Groups SelectedGroup
    {
        get { return _selectedGroup; }
        set
        {
            _selectedGroup = value;
            OnPropertyChanged();
        }
    }

    private Groups _selectedGroup;
    private GroupService _groupService;

    public GroupsViewModel(GroupService groupService)
    {
        _groupService = groupService;
        _selectedGroup = null!;
        Groups = new ObservableCollection<Groups>();
    }
}
