using Moq;
using Prism.Events;
using FriendStorage.UI.ViewModel;
using FriendStorage.UI.Events;
using FriendStorage.Model;
using FriendStorage.UITests.Extensions;
using FriendStorage.UI.Wrapper;

namespace FriendStorage.UITests.ViewModel;

public class MainViewModelTests
{
    private MainViewModel mainViewModel;
    private Mock<INavigationViewModel> navigationViewModelMock;
    private Mock<IEventAggregator> eventAggregatorMock;
    private OpenFriendEditViewEvent openFriendEditViewEvent;
    private FriendDeletedEvent friendDeletedEvent;
    private List<Mock<IFriendEditViewModel>> friendEditViewModelMocks;
    private Mock<IFriendEditViewModelFactory> friendEditViewModelFactoryMock;

    public MainViewModelTests()
    {
        navigationViewModelMock = new Mock<INavigationViewModel>();
        friendEditViewModelMocks = new List<Mock<IFriendEditViewModel>>();
        friendEditViewModelFactoryMock = new Mock<IFriendEditViewModelFactory>();
        friendEditViewModelFactoryMock.Setup(f => f.Create()).Returns(CreateFriendEditViewModel());

        openFriendEditViewEvent = new OpenFriendEditViewEvent();
        friendDeletedEvent = new FriendDeletedEvent();
        eventAggregatorMock = new Mock<IEventAggregator>();
        eventAggregatorMock.Setup(ea => ea.GetEvent<OpenFriendEditViewEvent>())
            .Returns(openFriendEditViewEvent);
        eventAggregatorMock.Setup(ea => ea.GetEvent<FriendDeletedEvent>())
            .Returns(friendDeletedEvent);
        mainViewModel = new MainViewModel(navigationViewModelMock.Object, friendEditViewModelFactoryMock.Object, eventAggregatorMock.Object);
    }

    [Fact]
    public void ShouldCallLoadMethodOfNavigationViewModel()
    {
        mainViewModel.Load();
        navigationViewModelMock.Verify(vm => vm.Load(), Times.Once);
    }

    [Fact]
    public void ShouldAddFriendEditViewModelAndLoadAndSelectIt()
    {
        const int friendId = 7;
        openFriendEditViewEvent.Publish(friendId);

        Assert.Single(mainViewModel.FriendEditViewModels);
        var friendEditVm = mainViewModel.FriendEditViewModels.First();
        Assert.Equal(friendEditVm, mainViewModel.SelectedFriendEditViewModel);
        friendEditViewModelMocks.First().Verify(vm => vm.Load(friendId), Times.Once);
    }

    [Fact]
    public void ShouldAddFriendEditViewModelsOnlyOnce()
    {
        openFriendEditViewEvent.Publish(5);
        openFriendEditViewEvent.Publish(5);
        openFriendEditViewEvent.Publish(6);
        openFriendEditViewEvent.Publish(7);
        openFriendEditViewEvent.Publish(7);

        Assert.Equal(3, mainViewModel.FriendEditViewModels.Count);
    }

    [Fact]
    public void ShouldAddFriendEditViewModelAndLoadItWithIdNullAndSelectIt()
    {
        mainViewModel.AddFriendCommand.Execute(null);
        Assert.Equal(1, mainViewModel.FriendEditViewModels.Count);
        var friendEditVm = mainViewModel.FriendEditViewModels.First();
        Assert.Equal(friendEditVm, mainViewModel.SelectedFriendEditViewModel);
        friendEditViewModelMocks.First().Verify(vm => vm.Load(null), Times.Once);
    }

    [Fact]
    public void ShouldRaisePropertyChangedeventForSelectedFriendEditViewModel()
    {
        var friendEditVmMock = new Mock<IFriendEditViewModel>();
        var fired = mainViewModel.IsPropertyChangedFired(() =>
        {
            mainViewModel.SelectedFriendEditViewModel = friendEditVmMock.Object;
        }, nameof(mainViewModel.SelectedFriendEditViewModel));

        Assert.True(fired);
    }

    [Fact]
    public void ShouldRemoveFriendEditViewModelOnCloseFriendTabCommand()
    {
        openFriendEditViewEvent.Publish(7);

        var friendEditVm = mainViewModel.SelectedFriendEditViewModel;

        mainViewModel.CloseFriendTabCommand.Execute(friendEditVm);

        Assert.Equal(0, mainViewModel.FriendEditViewModels.Count);
    }

    private IFriendEditViewModel CreateFriendEditViewModel()
    {
        var friendEditViewModelMock = new Mock<IFriendEditViewModel>();
        friendEditViewModelMock.Setup(vm => vm.Load(It.IsAny<int?>()))
            .Callback<int?>(friendId =>
            {
                friendEditViewModelMock.Setup(vm => vm.Friend)
                    .Returns(new FriendWrapper(new Friend { Id = friendId.HasValue ? friendId.Value : 0 }));
            });
        friendEditViewModelMocks.Add(friendEditViewModelMock);
        return friendEditViewModelMock.Object;
    }

    [Fact]
    public void ShouldRemoveFriendEditViewModelOnFriendDeletedEvent()
    {
        const int deletedFriendId = 7;
        openFriendEditViewEvent.Publish(deletedFriendId);

        // TODO: open multiple FriendEditViewModels then delete 1 (factory.Create() currently results in multiples all having the same Friend.Id)
        // openFriendEditViewEvent.Publish(8);
        // openFriendEditViewEvent.Publish(9);
        Assert.Single(mainViewModel.FriendEditViewModels);

        friendDeletedEvent.Publish(deletedFriendId);

        Assert.Empty(mainViewModel.FriendEditViewModels);
        Assert.True(mainViewModel.FriendEditViewModels.All(vm => vm.Friend.Id != deletedFriendId));
    }
}

