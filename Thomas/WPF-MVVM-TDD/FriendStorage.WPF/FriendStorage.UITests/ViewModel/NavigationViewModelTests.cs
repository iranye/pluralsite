using FriendStorage.Model;
using FriendStorage.UI.DataProvider;
using FriendStorage.UI.Events;
using FriendStorage.UI.ViewModel;
using Moq;
using Prism.Events;

namespace FriendStorage.UITests.ViewModel;

public class NavigationViewModelTests
{
    NavigationViewModel navigationViewModel;
    private FriendSavedEvent friendSavedEvent;
    private FriendDeletedEvent friendDeletedEvent;

    public NavigationViewModelTests()
    {
        var navigationDataProviderMock = new Mock<INavigationDataProvider>();
        navigationDataProviderMock.Setup(dp => dp.GetAllFriends())
            .Returns(new List<LookupItem>
            {
                    new LookupItem {Id = 1, DisplayMember = "Julia"},
                    new LookupItem {Id = 2, DisplayMember = "Thomas"}
            });

        friendSavedEvent = new FriendSavedEvent();
        friendDeletedEvent = new FriendDeletedEvent();
        var eventAggregatorMock = new Mock<IEventAggregator>();
        eventAggregatorMock.Setup(ea => ea.GetEvent<FriendSavedEvent>()).Returns(friendSavedEvent);
        eventAggregatorMock.Setup(ea => ea.GetEvent<FriendDeletedEvent>()).Returns(friendDeletedEvent);
        navigationViewModel = new NavigationViewModel(navigationDataProviderMock.Object, eventAggregatorMock.Object);
    }

    [Fact]
    public void ShouldLoadFriends()
    {
        navigationViewModel.Load();

        Assert.Equal(2, navigationViewModel.Friends.Count);

        var friend = navigationViewModel.Friends.SingleOrDefault(f => f.Id == 1);
        Assert.Equal("Julia", friend?.DisplayMember);

        var friend2 = navigationViewModel.Friends.SingleOrDefault(f => f.Id == 2);
        Assert.Equal("Thomas", friend2?.DisplayMember);
    }

    [Fact]
    public void ShouldLoadFriendsOnlyOnce()
    {
        navigationViewModel.Load();
        navigationViewModel.Load();

        Assert.Equal(2, navigationViewModel.Friends.Count);

        var friend = navigationViewModel.Friends.SingleOrDefault(f => f.Id == 1);
        Assert.Equal("Julia", friend?.DisplayMember);

        var friend2 = navigationViewModel.Friends.SingleOrDefault(f => f.Id == 2);
        Assert.Equal("Thomas", friend2?.DisplayMember);
    }

    [Fact]
    public void ShouldUpdateNavigationItemWhenFriendIsSaved()
    {
        navigationViewModel.Load();

        var expectedDisplayStr = "Anna Huber";
        Assert.Null(navigationViewModel.Friends.FirstOrDefault(f => f.DisplayMember == expectedDisplayStr));
        var navigationItem = navigationViewModel.Friends.First();

        var friendId = navigationItem.Id;

        friendSavedEvent.Publish(
          new Friend
          {
              Id = friendId,
              FirstName = "Anna",
              LastName = "Huber"
          });

        Assert.Equal(expectedDisplayStr, navigationItem.DisplayMember);
    }

    [Fact]
    public void ShouldAddNavigationItemWhenAddedFriendIsSaved()
    {
        navigationViewModel.Load();

        const int newFriendId = 97;

        friendSavedEvent.Publish(new Friend
        {
            Id = newFriendId,
            FirstName = "Anna",
            LastName = "Huber"
        });

        Assert.Equal(3, navigationViewModel.Friends.Count);

        var addedItem = navigationViewModel.Friends.SingleOrDefault(f => f.Id == newFriendId);
        Assert.NotNull(addedItem);
        Assert.Equal("Anna Huber", addedItem.DisplayMember);
    }

    [Fact]
    public void ShouldRemoveNavigationItemWhenFriendIsDeleted()
    {
        navigationViewModel.Load();

        var deletedFriendId = navigationViewModel.Friends.First().Id;

        friendDeletedEvent.Publish(deletedFriendId);

        Assert.Equal(1, navigationViewModel.Friends.Count);
        Assert.NotEqual(deletedFriendId, navigationViewModel.Friends.Single().Id);
    }
}
