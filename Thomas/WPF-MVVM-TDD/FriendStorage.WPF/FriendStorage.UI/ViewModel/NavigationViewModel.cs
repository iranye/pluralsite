using FriendStorage.Model;
using FriendStorage.UI.DataProvider;
using FriendStorage.UI.Events;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace FriendStorage.UI.ViewModel;

public class NavigationViewModel : ViewModelBase, INavigationViewModel
{
    private readonly INavigationDataProvider navigationDataProvider;
    private readonly IEventAggregator eventAggregator;

    public NavigationViewModel(INavigationDataProvider navigationDataProvider, IEventAggregator eventAggregator)
    {
        this.navigationDataProvider = navigationDataProvider ?? throw new ArgumentNullException(nameof(navigationDataProvider));
        this.eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
        eventAggregator.GetEvent<FriendSavedEvent>().Subscribe(OnFriendSaved);
        eventAggregator.GetEvent<FriendDeletedEvent>().Subscribe(OnFriendDeleted);
        Friends = new ObservableCollection<NavigationItemViewModel>();
    }

    private void OnFriendDeleted(int id)
    {
        var navItem = Friends.FirstOrDefault(x => x.Id == id);
        if (navItem != null)
        {
            Friends.Remove(navItem);
        }
    }

    private void OnFriendSaved(Friend friend)
    {
        var navigationItem = Friends.FirstOrDefault(n => n.Id == friend.Id);
        var displayMember = $"{friend.FirstName} {friend.LastName}";
        if (navigationItem is not null)
        {
            navigationItem.DisplayMember = displayMember;
        }
        else
        {
            Friends.Add(new NavigationItemViewModel(friend.Id, displayMember, eventAggregator));
        }
    }

    public void Load()
    {
        Friends.Clear();
        foreach (var friend in navigationDataProvider.GetAllFriends())
        {
            Friends.Add(new NavigationItemViewModel(friend.Id, friend.DisplayMember, eventAggregator));
        }
    }

    public ObservableCollection<NavigationItemViewModel> Friends { get; private set; }
}
