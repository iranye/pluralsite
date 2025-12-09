using WiredBrainCoffee.CustomersApp.Model;

namespace WiredBrainCoffee.CustomersApp.ViewModel
{
    public class CustomerItemViewModel : ViewModelBase
    {
        private readonly Customer model;

        public CustomerItemViewModel(Customer model)
        {
            this.model = model;
        }

        public int Id => model.Id;

        public string? FirstName
        {
            get => model.FirstName;
            set
            {
                if (model.FirstName != value)
                {
                    model.FirstName = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(DisplayMember);
                }
            }
        }

        public string? LastName
        {
            get => model.LastName;
            set
            {
                if (model.LastName != value)
                {
                    model.LastName = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(DisplayMember);
                }
            }
        }

        public bool IsDeveloper
        {
            get => model.IsDeveloper;
            set
            {
                if (model.IsDeveloper != value)
                {
                    model.IsDeveloper = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string DisplayMember
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }

        public override string ToString()
        {
            return DisplayMember;
        }
    }
}
