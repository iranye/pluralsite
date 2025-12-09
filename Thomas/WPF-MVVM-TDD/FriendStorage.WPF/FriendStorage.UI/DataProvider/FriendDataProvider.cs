using FriendStorage.Model;
using FriendStorage.UI.Factory;

namespace FriendStorage.UI.DataProvider;

public class FriendDataProvider : IFriendDataProvider
{
    private const string dataServiceType = "File";
    private IDataServiceFactory dataServiceFactory;

    public FriendDataProvider(IDataServiceFactory dataServiceFactory)
    {
        this.dataServiceFactory = dataServiceFactory;
    }

    public Friend GetFriendById(int id)
    {
        using var dataService = dataServiceFactory.GetDataService(dataServiceType);
        return dataService is null ? new Friend() : dataService.GetFriendById(id);
    }

    public void SaveFriend(Friend friend)
    {
        using var dataService = dataServiceFactory.GetDataService(dataServiceType);
        if (dataService is null)
        {
            return;
        }
        dataService.SaveFriend(friend);
    }

    public void DeleteFriend(int id)
    {
        using var dataService = dataServiceFactory.GetDataService(dataServiceType);
        if (dataService is null)
        {
            return;
        }
        dataService.DeleteFriend(id);
    }
}
