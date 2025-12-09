using FriendStorage.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FriendStorage.UI.Factory;

public interface IDataServiceFactory
{
    IDataService? GetDataService(string storageType);
}

public class DataServiceFactory : IDataServiceFactory
{
    private readonly IEnumerable<IDataService> dataServices;

    public DataServiceFactory(IEnumerable<IDataService> dataServices)
    {
        this.dataServices = dataServices ?? throw new ArgumentNullException(nameof(dataServices));
    }

    public IDataService? GetDataService(string storageType)
    {
        return dataServices.FirstOrDefault(ds => ds.StorageType == storageType);
    }
}
