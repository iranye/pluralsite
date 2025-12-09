using FriendStorage.Model;
using FriendStorage.UI.Factory;
using System.Collections.Generic;

namespace FriendStorage.UI.DataProvider;

public class NavigationDataProvider : INavigationDataProvider
{
    private const string dataServiceType = "File";
    private readonly IDataServiceFactory dataServiceFactory;

    public NavigationDataProvider(IDataServiceFactory dataServiceFactory)
    {
        this.dataServiceFactory = dataServiceFactory;
    }

    public IEnumerable<LookupItem> GetAllFriends()
    {
        using var dataService = dataServiceFactory.GetDataService(dataServiceType);
        if (dataService is null)
        {
            return new List<LookupItem>();
        }

        return dataService.GetAllFriends();
    }
}
