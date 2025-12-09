using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using WiredBrainCoffee.CustomersApp.Command;
using WiredBrainCoffee.CustomersApp.Data;
using WiredBrainCoffee.CustomersApp.Model;

namespace WiredBrainCoffee.CustomersApp.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ICustomerDataProvider customerDataProvider;
        private CustomerItemViewModel? selectedCustomer;
        
        // The XamlRoot must be provided by the view (e.g. MainWindow) so dialogs can be shown.
        public XamlRoot? XamlRoot { get; set; }

        public MainViewModel(ICustomerDataProvider customerDataProvider)
        {
            this.customerDataProvider = customerDataProvider;
            AddCommand = new DelegateCommand(Add);
            DeleteCommand = new DelegateCommand(Delete, CanDelete);
        }

        public DelegateCommand AddCommand { get; }

        public DelegateCommand DeleteCommand { get; }

        public ObservableCollection<CustomerItemViewModel> Customers { get; } = new();

        public CustomerItemViewModel? SelectedCustomer
        {
            get => selectedCustomer;
            set
            {
                if (selectedCustomer != value)
                {
                    selectedCustomer = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(IsCustomerSelected));
                    DeleteCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool IsCustomerSelected => SelectedCustomer is not null;

        public async Task LoadAsync()
        {
            if (Customers.Any())
            {
                return;
            }

            var customers = await customerDataProvider.GetAllAsync();
            if (customers is not null)
            {
                foreach (var customer in customers)
                {
                    Customers.Add(new CustomerItemViewModel(customer));
                }
            }
        }

        private void Add(object? parameter)
        {
            var customer = new Customer {  FirstName = "New", LastName = "Customer" };
            var viewModel = new CustomerItemViewModel(customer);
            Customers.Add(viewModel);
            SelectedCustomer = viewModel;
        }

        private async void Delete(object? parameter)
        {
            if (SelectedCustomer is not null)
            {
                var messageDialog = new ContentDialog
                {
                    Title = "Confirm Delete",
                    Content = "Are you sure you want to delete the selected customer?",
                    PrimaryButtonText = "OK",
                    CloseButtonText = "Cancel",
                    XamlRoot = this.XamlRoot
                };

                var result = await messageDialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    Customers.Remove(SelectedCustomer);
                    SelectedCustomer = null;
                }
            }
        }

        private bool CanDelete(object? parameter)
        {
            return IsCustomerSelected;
        }
    }
}
