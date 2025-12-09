using FriendStorage.UI.Command;
using FriendStorage.UI.Events;
using Prism.Events;
using System;
using System.Windows.Input;

namespace FriendStorage.UI.ViewModel;

public class NavigationItemViewModel : ViewModelBase
{
    private string displayMember = String.Empty;
    private readonly IEventAggregator eventAggregator;
    public int Id { get; set; }
    public ICommand OpenFriendEditViewCommand { get; private set; }

    public NavigationItemViewModel(int id, string displayMember, IEventAggregator eventAggregator)
    {
        Id = id;
        DisplayMember = displayMember;
        this.eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
        OpenFriendEditViewCommand = new DelegateCommand(OnFriendEditViewExecute);
    }

    public string DisplayMember
    {
        get
        {
            return displayMember;
        }

        set
        {
            displayMember = value;
            OnPropertyChanged();
        }
    }

    public override string ToString()
    {
        return DisplayMember;
    }

    private void OnFriendEditViewExecute(object obj)
    {
        eventAggregator.GetEvent<OpenFriendEditViewEvent>()
            .Publish(Id);
    }
}
