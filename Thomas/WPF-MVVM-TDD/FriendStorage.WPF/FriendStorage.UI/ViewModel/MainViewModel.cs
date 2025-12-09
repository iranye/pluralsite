using FriendStorage.UI.Command;
using FriendStorage.UI.Events;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace FriendStorage.UI.ViewModel;

public class MainViewModel : ViewModelBase
{
    private IFriendEditViewModel? selectedFriendEditViewModel;
    private readonly IFriendEditViewModelFactory friendEditViewModelFactory;

    public MainViewModel(INavigationViewModel navigationViewModel, IFriendEditViewModelFactory friendEditViewModelFactory, IEventAggregator eventAggregator)
    {
        NavigationViewModel = navigationViewModel ?? throw new ArgumentNullException(nameof(navigationViewModel));
        this.friendEditViewModelFactory = friendEditViewModelFactory ?? throw new ArgumentNullException(nameof(friendEditViewModelFactory));
        eventAggregator.GetEvent<OpenFriendEditViewEvent>().Subscribe(OnOpenFriendEditView);
        eventAggregator.GetEvent<FriendDeletedEvent>().Subscribe(OnFriendDeleted);
        AddFriendCommand = new DelegateCommand(OnAddFriendExecute);
        CloseFriendTabCommand = new DelegateCommand(OnCloseFriendTabExecute);
    }

    private void OnFriendDeleted(int id)
    {
        var vm = FriendEditViewModels.FirstOrDefault(vm => vm.Friend.Id == id);
        if (vm != null) 
        {
            FriendEditViewModels.Remove(vm);
        }
    }

    public ICommand CloseFriendTabCommand { get; private set; }
    public ICommand AddFriendCommand { get; private set; }

    public INavigationViewModel NavigationViewModel { get; private set; }

    public ObservableCollection<IFriendEditViewModel> FriendEditViewModels { get; private set; } = new ObservableCollection<IFriendEditViewModel>();

    public  IFriendEditViewModel? SelectedFriendEditViewModel
    {
        get { return selectedFriendEditViewModel; }
        set
        {
            selectedFriendEditViewModel = value;
            OnPropertyChanged();
        }
    }

    private void OnAddFriendExecute(object obj)
    {
        SelectedFriendEditViewModel = CreateAndLoadFriendEditViewModel();
    }

    private void OnCloseFriendTabExecute(object obj)
    {
        var friendEditViewModel = obj as IFriendEditViewModel;
        if (friendEditViewModel is not null)
        {
            FriendEditViewModels.Remove(friendEditViewModel);
        }
    }

    private void OnOpenFriendEditView(int friendId)
    {
        var friendEditVm = FriendEditViewModels.FirstOrDefault(vm => vm.Friend.Id == friendId);
        friendEditVm ??= CreateAndLoadFriendEditViewModel(friendId);
        // null-coalesce, SAME AS:
        // if (friendEditVm == null)
        // {
        //     friendEditVm = CreateAndLoadFriendEditViewModel(friendId);
        // }
        SelectedFriendEditViewModel = friendEditVm;
    }

    private IFriendEditViewModel CreateAndLoadFriendEditViewModel(int? friendId=null)
    {
        var friendEditVm = friendEditViewModelFactory.Create();
        friendEditVm.Load(friendId);
        FriendEditViewModels.Add(friendEditVm);
        return friendEditVm;
    }

    public void Load()
    {
        NavigationViewModel.Load();
    }
}
