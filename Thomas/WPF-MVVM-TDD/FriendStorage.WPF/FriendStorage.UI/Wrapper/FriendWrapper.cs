using FriendStorage.Model;
using FriendStorage.UI.ViewModel;
using System;
using System.Runtime.CompilerServices;

namespace FriendStorage.UI.Wrapper
{
    public class FriendWrapper : ViewModelBase
    {
        private Friend friend;
        private bool isChanged;

        public FriendWrapper(Friend friend)
        {
            this.friend = friend;
        }

        public Friend Model { get { return friend; } }

        public bool IsChanged
        {
            get { return isChanged; }
            private set
            {
                isChanged = value;
                OnPropertyChanged();
            }
        }

        public void AcceptChanges()
        {
            IsChanged = false;
        }

        public int Id
        {
            get { return friend.Id; }
        }

        public string FirstName
        {
            get { return friend.FirstName; }
            set
            {
                friend.FirstName = value;
                OnPropertyChanged();
            }
        }

        public string LastName
        {
            get { return friend.LastName; }
            set
            {
                friend.LastName = value;
                OnPropertyChanged();
            }
        }

        public DateTime? Birthday
        {
            get { return friend.Birthday; }
            set
            {
                friend.Birthday = value;
                OnPropertyChanged();
            }
        }

        public bool IsDeveloper
        {
            get { return friend.IsDeveloper; }
            set
            {
                friend.IsDeveloper = value;
                OnPropertyChanged();
            }
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName != nameof(IsChanged))
            {
                IsChanged = true;
            }
        }
    }
}
