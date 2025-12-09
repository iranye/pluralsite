using FriendStorage.Model;

namespace FriendStorage.DataAccess
{
    public interface IDataService : IDisposable
    {
        String StorageType { get; }

        Friend GetFriendById(int friendId);

        void SaveFriend(Friend friend);

        void DeleteFriend(int friendId);

        IEnumerable<LookupItem> GetAllFriends();
    }
}
