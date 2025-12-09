using FriendStorage.Model;
using FriendStorage.UI.Command;
using FriendStorage.UI.DataProvider;
using FriendStorage.UI.Dialogs;
using FriendStorage.UI.Events;
using FriendStorage.UI.Wrapper;
using Prism.Events;
using System;
using System.Windows;
using System.Windows.Input;

namespace FriendStorage.UI.ViewModel;

public interface IFriendEditViewModel
{
    void Load(int? friendId);
    FriendWrapper Friend { get; }
}

public class FriendEditViewModel : ViewModelBase, IFriendEditViewModel
{
    private FriendWrapper? friend;
    private IFriendDataProvider friendDataProvider;
    private readonly IEventAggregator eventAggregator;
    private readonly IMessageDialogService messageDialogService;

    public FriendEditViewModel(IFriendDataProvider friendDataProvider, IEventAggregator eventAggregator, IMessageDialogService messageDialogService)
    {
        this.friendDataProvider = friendDataProvider ?? throw new ArgumentNullException(nameof(friendDataProvider));
        this.eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
        this.messageDialogService = messageDialogService ?? throw new ArgumentNullException(nameof(messageDialogService));
        DeleteCommand = new DelegateCommand(OnDeleteExecute, OnDeleteCanExecute);
        SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
    }

    public ICommand DeleteCommand { get; private set; }
    public ICommand SaveCommand { get; private set; }

    public FriendWrapper Friend
    {
        get { return friend; }
        private set
        {
            friend = value;
            OnPropertyChanged();
        }
    }

    private void OnDeleteExecute(object obj)
    {
        var result = messageDialogService.ShowYesNoDialog("Delete Friend", 
            $"Do you really want to delete the friend '{Friend.FirstName} {Friend.LastName}'");
        if (result == MessageDialogResult.Yes)
        {
            friendDataProvider.DeleteFriend(Friend.Id);
            eventAggregator.GetEvent<FriendDeletedEvent>().Publish(Friend.Id);
        }
    }

    private bool OnDeleteCanExecute(object arg)
    {
        return Friend != null && Friend.Id > 0;
    }

    private void OnSaveExecute(object obj)
    {
        friendDataProvider.SaveFriend(Friend.Model);
        Friend.AcceptChanges();
        eventAggregator.GetEvent<FriendSavedEvent>().Publish(Friend.Model);
    }

    private bool OnSaveCanExecute(object obj)
    {
        return Friend != null && Friend.IsChanged;
    }

    public void Load(int? friendId)
    {
        var friend = friendId == null ? new Friend() : friendDataProvider.GetFriendById(friendId.Value);

        Friend = new FriendWrapper(friend);
        Friend.PropertyChanged += Friend_PropertyChanged;
        ResetCommands();
    }

    private void ResetCommands()
    {
        ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        ((DelegateCommand)DeleteCommand).RaiseCanExecuteChanged();
    }

    private void Friend_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        ResetCommands();
    }
}
