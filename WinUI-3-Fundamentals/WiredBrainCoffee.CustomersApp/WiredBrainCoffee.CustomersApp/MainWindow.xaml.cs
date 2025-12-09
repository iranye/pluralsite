using Microsoft.UI.Xaml;
using WiredBrainCoffee.CustomersApp.Data;
using WiredBrainCoffee.CustomersApp.ViewModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WiredBrainCoffee.CustomersApp
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Title = "WiredBrain Coffee Customers App";
            ViewModel = new MainViewModel(new CustomerDataProvider());
            root.Loaded += Root_Loaded;
        }

        private async void Root_Loaded(object sender, RoutedEventArgs e)
        {
            // Ensure the view model gets a valid XamlRoot (available after the element is loaded)
            ViewModel.XamlRoot = this.root.XamlRoot;
            await ViewModel.LoadAsync();
        }

        public MainViewModel ViewModel { get; }

        private void ButtonToggleTheme_Click(object sender, RoutedEventArgs e)
        {
            root.RequestedTheme = root.RequestedTheme == ElementTheme.Light ? ElementTheme.Dark : ElementTheme.Light;
        }
    }
}
