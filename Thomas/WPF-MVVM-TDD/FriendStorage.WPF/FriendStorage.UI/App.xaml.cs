using FriendStorage.DataAccess;
using FriendStorage.UI.DataProvider;
using FriendStorage.UI.Dialogs;
using FriendStorage.UI.Factory;
using FriendStorage.UI.View;
using FriendStorage.UI.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Prism.Events;
using System.Windows;

namespace FriendStorage.UI;

public partial class App : Application
{
    private readonly ServiceProvider serviceProvider;

    public App()
    {
        ServiceCollection services = new();
        ConfigureServices(services);
        serviceProvider = services.BuildServiceProvider();

    }
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var mainWindow = serviceProvider.GetService<MainWindow>();
        mainWindow?.Show();
    }

    private void ConfigureServices(ServiceCollection services)
    {
        services.AddTransient<MainWindow>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<IMessageDialogService, MessageDialogService>();
        services.AddTransient<INavigationViewModel, NavigationViewModel>();

        services.AddTransient<IDataService, FileDataService>();
        services.AddTransient<IDataServiceFactory, DataServiceFactory>();

        services.AddTransient<INavigationDataProvider, NavigationDataProvider>();
        services.AddTransient<IFriendDataProvider, FriendDataProvider>();

        services.AddSingleton<IEventAggregator, EventAggregator>();
        services.AddTransient<IFriendEditViewModel, FriendEditViewModel>();

        services.AddSingleton<IFriendEditViewModelFactory, FriendEditViewModelFactory>();

        // TODO: use AddScoped or AddSingleton
        services.AddTransient<IDataService, FileDataService>();
    }
}
