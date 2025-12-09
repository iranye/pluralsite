using FriendStorage.UI.ViewModel;
using System.Windows;

namespace FriendStorage.UI.View;

public partial class MainWindow : Window
{
    private readonly MainViewModel viewModel;

    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        this.Loaded += MainWindow_Loaded;
        this.viewModel = viewModel;
        DataContext = this.viewModel;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        viewModel.Load();
    }


}
