using FriendStorage.Model;
using FriendStorage.UI.DataProvider;
using FriendStorage.UI.Dialogs;
using FriendStorage.UI.Events;
using FriendStorage.UI.ViewModel;
using FriendStorage.UITests.Extensions;
using Moq;
using Prism.Events;

namespace FriendStorage.UITests.ViewModel;

public class FriendEditViewModelTests
{
    private const int friendId = 5;
    private Mock<IFriendDataProvider> friendDataProviderMock;
    private FriendEditViewModel friendEditViewModel;
    private Mock<FriendSavedEvent> friendSavedEventMock;
    private Mock<FriendDeletedEvent> friendDeletedEventMock;
    private Mock<IEventAggregator> eventAggregatorMock;
    private Mock<IMessageDialogService> messageDialogServiceMock;

    public FriendEditViewModelTests()
    {
        friendSavedEventMock = new Mock<FriendSavedEvent>();
        friendDeletedEventMock = new Mock<FriendDeletedEvent>();

        eventAggregatorMock = new Mock<IEventAggregator>();
        eventAggregatorMock.Setup(ea => ea.GetEvent<FriendSavedEvent>())
            .Returns(friendSavedEventMock.Object);
        eventAggregatorMock.Setup(ea => ea.GetEvent<FriendDeletedEvent>())
            .Returns(friendDeletedEventMock.Object);

        friendDataProviderMock = new Mock<IFriendDataProvider>();
        friendDataProviderMock.Setup(dp => dp.GetFriendById(friendId))
            .Returns(new Friend { Id = friendId, FirstName = "Thomas" });

        messageDialogServiceMock = new Mock<IMessageDialogService>();
        friendEditViewModel = new FriendEditViewModel(friendDataProviderMock.Object, eventAggregatorMock.Object, messageDialogServiceMock.Object);
    }

    [Fact]
    public void ShouldLoadFriend()
    {
        friendEditViewModel.Load(friendId);

        Assert.NotNull(friendEditViewModel.Friend);
        Assert.Equal(friendId, friendEditViewModel.Friend.Id);

        friendDataProviderMock.Verify(dp => dp.GetFriendById(friendId), Times.Once);
    }

    [Fact]
    public void ShouldRaisePropertyChangedEventForFriend()
    {
        var fired = friendEditViewModel.IsPropertyChangedFired(
            () => friendEditViewModel.Load(friendId),
            nameof(friendEditViewModel.Friend));

        Assert.True(fired);
    }

    [Fact]
    public void ShouldDisableSaveCommandWhenFriendIsLoaded()
    {
        friendEditViewModel.Load(friendId);

        Assert.False(friendEditViewModel.SaveCommand.CanExecute(null));
    }

    [Fact]
    public void ShouldEnableSaveCommandWhenFriendIsChanged()
    {
        friendEditViewModel.Load(friendId);

        friendEditViewModel.Friend.FirstName = "Changed";

        Assert.True(friendEditViewModel.SaveCommand.CanExecute(null));
    }

    [Fact]
    public void ShouldDisableSaveCommandWithoutLoad()
    {
        Assert.False(friendEditViewModel.SaveCommand.CanExecute(null));
    }

    [Fact]
    public void ShouldRaiseCanExecuteChangedForSaveCommandWhenFriendIsChanged()
    {
        friendEditViewModel.Load(friendId);
        var fired = false;
        friendEditViewModel.SaveCommand.CanExecuteChanged += (s, e) => fired = true;
        friendEditViewModel.Friend.FirstName = "Changed";
        Assert.True(fired);
    }

    [Fact]
    public void ShouldRaiseCanExecuteChangedForSaveCommandAfterLoad()
    {
        var fired = false;
        friendEditViewModel.SaveCommand.CanExecuteChanged += (s, e) => fired = true;
        friendEditViewModel.Load(friendId);
        Assert.True(fired);
    }

    [Fact]
    public void ShouldCallSaveMethodOfDataProviderWhenSaveCommandIsExecuted()
    {
        friendEditViewModel.Load(friendId);
        friendEditViewModel.Friend.FirstName = "Changed";

        friendEditViewModel.SaveCommand.Execute(null);
        friendDataProviderMock.Verify(dp => dp.SaveFriend(friendEditViewModel.Friend.Model), Times.Once);
    }

    [Fact]
    public void ShouldAcceptChangesWhenSaveCommandIsExecuted()
    {
        friendEditViewModel.Load(friendId);
        friendEditViewModel.Friend.FirstName = "Changed";

        friendEditViewModel.SaveCommand.Execute(null);
        Assert.False(friendEditViewModel.Friend.IsChanged);
    }

    [Fact]
    public void ShouldPublishFriendSavedEventWhenSaveCommandIsExecuted()
    {
        friendEditViewModel.Load(friendId);
        friendEditViewModel.Friend.FirstName = "Changed";

        friendEditViewModel.SaveCommand.Execute(null);
        friendSavedEventMock.Verify(e => e.Publish(friendEditViewModel.Friend.Model), Times.Once);
    }

    [Fact]
    public void ShouldCreateNewFriendWhenNullIsPassedToLoadMethod()
    {
        friendEditViewModel.Load(null);

        Assert.NotNull(friendEditViewModel.Friend);
        Assert.Equal(0, friendEditViewModel.Friend.Id);
        Assert.Empty(friendEditViewModel.Friend.FirstName);
        Assert.Empty(friendEditViewModel.Friend.LastName);
        Assert.Null(friendEditViewModel.Friend.Birthday);
        Assert.False(friendEditViewModel.Friend.IsDeveloper);

        friendDataProviderMock.Verify(dp => dp.GetFriendById(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void ShouldEnableDeleteCommandForExistingFriend()
    {
        friendEditViewModel.Load(friendId);
        Assert.True(friendEditViewModel.DeleteCommand.CanExecute(null));
    }

    [Fact]
    public void ShouldDisableDeleteCommandForNewFriend()
    {
        friendEditViewModel.Load(null);
        Assert.False(friendEditViewModel.DeleteCommand.CanExecute(null));
    }

    [Fact]
    public void ShouldDisableDeleteCommandWithoutLoad()
    {
        Assert.False(friendEditViewModel.DeleteCommand.CanExecute(null));
    }

    [Fact]
    public void ShouldRaiseCanExecuteChangedForDeleteCommandWhenAcceptingChanges()
    {
        friendEditViewModel.Load(friendId);
        var fired = false;
        friendEditViewModel.Friend.FirstName = "Changed";
        friendEditViewModel.DeleteCommand.CanExecuteChanged += (s, e) => fired = true;
        friendEditViewModel.Friend.AcceptChanges();
        Assert.True(fired);
    }

    [Fact]
    public void ShouldRaiseCanExecuteChangedForDeleteCommandAfterLoad()
    {
        var fired = false;
        friendEditViewModel.DeleteCommand.CanExecuteChanged += (s, e) => fired = true;
        friendEditViewModel.Load(friendId);
        Assert.True(fired);
    }

    [Theory]
    [InlineData(MessageDialogResult.Yes, 1)]
    [InlineData(MessageDialogResult.No, 0)]
    public void ShouldCallDeleteFriendWhenDeleteCommandIsExecuted(MessageDialogResult result, int expectedDeleteFriendCalls)
    {
        messageDialogServiceMock.Setup(ds => ds.ShowYesNoDialog(It.IsAny<string>(), It.IsAny<string>())).Returns(result);
        friendEditViewModel.Load(friendId);
        friendEditViewModel.DeleteCommand.Execute(null);

        friendDataProviderMock.Verify(dp => dp.DeleteFriend(friendId), Times.Exactly(expectedDeleteFriendCalls));
        messageDialogServiceMock.Verify(ds => ds.ShowYesNoDialog(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Theory]
    [InlineData(MessageDialogResult.Yes, 1)]
    [InlineData(MessageDialogResult.No, 0)]
    public void ShouldPublishFriendDeletedEventWhenDeleteCommandIsExecuted(MessageDialogResult result, int expectedDeleteFriendCalls)
    {
        messageDialogServiceMock.Setup(ds => ds.ShowYesNoDialog(It.IsAny<string>(), It.IsAny<string>())).Returns(result);

        friendEditViewModel.Load(friendId);
        friendEditViewModel.DeleteCommand.Execute(null);

        friendDeletedEventMock.Verify(ea => ea.Publish(friendId), Times.Exactly(expectedDeleteFriendCalls));
        messageDialogServiceMock.Verify(ds => ds.ShowYesNoDialog(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void ShouldDisplayCorrectMessageInDeleteDialog()
    {
        friendEditViewModel.Load(friendId);

        var f = friendEditViewModel.Friend;
        f.FirstName = "Thomas";
        f.LastName = "Huber";

        friendEditViewModel.DeleteCommand.Execute(null);

        messageDialogServiceMock.Verify(d => d.ShowYesNoDialog("Delete Friend",
          $"Do you really want to delete the friend '{f.FirstName} {f.LastName}'"),
          Times.Once);
    }
}
