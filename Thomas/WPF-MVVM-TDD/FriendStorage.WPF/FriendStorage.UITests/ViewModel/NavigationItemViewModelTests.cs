using FriendStorage.UI.Events;
using FriendStorage.UI.ViewModel;
using FriendStorage.UITests.Extensions;
using Moq;
using Prism.Events;

namespace FriendStorage.UITests.ViewModel;

public class NavigationItemViewModelTests
{
    const int friendId = 7;
    private Mock<IEventAggregator> eventAggregatorMock;
    private NavigationItemViewModel navigationItemViewModel;

    public NavigationItemViewModelTests()
    {
        eventAggregatorMock = new Mock<IEventAggregator>();
        navigationItemViewModel = new NavigationItemViewModel(friendId, "Thomas", eventAggregatorMock.Object);
    }

    [Fact]
    public void ShouldPublishOpenFriendEditViewEvent()
    {
        var eventMock = new Mock<OpenFriendEditViewEvent>();
        eventAggregatorMock.Setup(ea => ea.GetEvent<OpenFriendEditViewEvent>())
            .Returns(eventMock.Object);

        navigationItemViewModel.OpenFriendEditViewCommand.Execute(null);
        eventMock.Verify(e => e.Publish(friendId), Times.Once);
    }

    [Fact]
    public void ShouldRaisePropertyChangedEventForDisplayMember()
    {
        var fired = navigationItemViewModel.IsPropertyChangedFired(() =>
        {
            navigationItemViewModel.DisplayMember = "Changed";
        }, nameof(navigationItemViewModel.DisplayMember));

        Assert.True(fired);
    }
}
