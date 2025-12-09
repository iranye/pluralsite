using FriendStorage.UI.DataProvider;
using FriendStorage.UI.Dialogs;
using Prism.Events;
using System;

namespace FriendStorage.UI.ViewModel;

public interface IFriendEditViewModelFactory
{
    IFriendEditViewModel Create();
}

public class FriendEditViewModelFactory : IFriendEditViewModelFactory
{
    private readonly IFriendDataProvider friendDataProvider;
    private readonly IEventAggregator eventAggregator;
    private readonly IMessageDialogService messageDialogService;

    public FriendEditViewModelFactory(IFriendDataProvider friendDataProvider, IEventAggregator eventAggregator, IMessageDialogService messageDialogService)
    {
        // TODO: Probably an anti-pattern having this injected only to use in object ctor of the Create method
        // Maybe not: accepted answer does it:
        // https://stackoverflow.com/questions/31950362/factory-method-with-di-and-ioc
        this.friendDataProvider = friendDataProvider ?? throw new ArgumentNullException(nameof(friendDataProvider));
        this.eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
        this.messageDialogService = messageDialogService ?? throw new ArgumentNullException(nameof(messageDialogService));
    }

    public IFriendEditViewModel Create()
    {
        return new FriendEditViewModel(friendDataProvider, eventAggregator, messageDialogService);
    }
}
