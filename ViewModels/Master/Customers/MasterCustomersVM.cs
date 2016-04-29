namespace PUJASM.ERP.ViewModels.Master.Customers
{
    using System.Collections.ObjectModel;
    using System.Data.Entity;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using Models;
    using MVVMFramework;
    using Utilities;
    using Views.Master;
    using MasterCustomersEditView = Views.Master.Customers.MasterCustomersEditView;

    public class MasterCustomersVM : ViewModelBase
    {
        #region Backing Fields
        private bool _isActiveChecked;
        private CustomerVM _selectedCustomer;
        private string _selectedCity;
        private CustomerVM _selectedLine;

        private ICommand _searchCommand;
        private ICommand _editCustomerCommand;
        private ICommand _activateCustomerCommand;
        #endregion

        public MasterCustomersVM()
        {
            Customers = new ObservableCollection<CustomerVM>();
            Cities = new ObservableCollection<string>();
            DisplayedCustomers = new ObservableCollection<CustomerVM>();

            UpdateCustomers();
            SelectedCustomer = Customers.FirstOrDefault();

            _isActiveChecked = true;
            UpdateDisplayedCustomers();
            NewEntryVM = new MasterCustomersNewEntryVM(this);
        }

        public MasterCustomersNewEntryVM NewEntryVM { get; }

        #region Collections
        public ObservableCollection<CustomerVM> Customers { get; }

        public ObservableCollection<string> Cities { get; }

        public ObservableCollection<CustomerVM> DisplayedCustomers { get; }
        #endregion

        #region Properties
        public bool IsActiveChecked
        {
            get { return _isActiveChecked; }
            set { SetProperty(ref _isActiveChecked, value, "IsActiveChecked"); }
        }

        public CustomerVM SelectedCustomer
        {
            get { return _selectedCustomer; }
            set { SetProperty(ref _selectedCustomer, value, "SelectedCustomer"); }
        }

        public string SelectedCity
        {
            get { return _selectedCity; }
            set
            {
                SetProperty(ref _selectedCity, value, "SelectedCity");

                if (_selectedCity == null) return;

                SelectedCustomer = null;
                Customers.Clear();
            }
        }

        public CustomerVM SelectedLine
        {
            get { return _selectedLine; }
            set { SetProperty(ref _selectedLine, value, "SelectedLine"); }
        }
        #endregion

        #region Commands
        public ICommand SearchCommand
        {
            get
            {
                return _searchCommand ?? (_searchCommand = new RelayCommand(() =>
                {
                    UpdateDisplayedCustomers();
                }));
            }
        }

        public ICommand EditCustomerCommand
        {
            get
            {
                return _editCustomerCommand ?? (_editCustomerCommand = new RelayCommand(() =>
                {
                    if (!IsThereLineSelected()) return;
                    ShowEditWindow();
                }));
            }
        }

        public ICommand ActivateCustomerCommand
        {
            get
            {
                return _activateCustomerCommand ?? (_activateCustomerCommand = new RelayCommand(() =>
                {
                    if (!IsThereLineSelected() || !IsConfirmationYes()) return;
                    if (_selectedLine.IsActive) DeactivateCustomerInDatabase(_selectedLine.Model);
                    else ActivateCustomerInDatabase(_selectedLine.Model);
                    _selectedLine.IsActive = !_isActiveChecked;
                }));
            }
        }
        #endregion

        #region Helper Methods 
        public void UpdateCustomers()
        {
            var oldSelectedCustomer = _selectedCustomer;

            Customers.Clear();
            using (var context = new ERPContext())
            {
                var allCustomer = new Customer {ID = "-1", FirstName = "All", LastName = ""};
                Customers.Add(new CustomerVM { Model = allCustomer });
                var customersFromDatabase = context.Customers.OrderBy(customer => customer.FirstName);
                foreach (var customer in customersFromDatabase)
                    Customers.Add(new CustomerVM { Model = customer });
            }

            UpdateSelectedCustomer(oldSelectedCustomer);
        }

        private void UpdateSelectedCustomer(CustomerVM oldSelectedCustomer)
        {
            if (oldSelectedCustomer == null) return;
            SelectedCustomer = Customers.SingleOrDefault(customer => customer.ID.Equals(oldSelectedCustomer.ID));
        }

        public void UpdateDisplayedCustomers()
        {
            DisplayedCustomers.Clear();
            using (var context = new ERPContext())
            {
                if (_selectedCustomer.FirstName.Equals("All"))
                {
                    var customersFromDatabase = context.Customers.Where(customer => customer.IsActive.Equals(_isActiveChecked)).OrderBy(customer => customer.FirstName);
                    foreach (var customer in customersFromDatabase)
                        DisplayedCustomers.Add(new CustomerVM {Model = customer});
                }
                else
                {
                    var customerFromDatabase =
                        context.Customers.Single(customer => customer.ID.Equals(_selectedCustomer.ID) && customer.IsActive.Equals(_isActiveChecked));
                    DisplayedCustomers.Add(new CustomerVM {Model = customerFromDatabase});
                }

            }
        }

        public static void DeactivateCustomerInDatabase(Customer customer)
        {
            using (var context = new ERPContext())
            {
                context.Entry(customer).State = EntityState.Modified;
                customer.IsActive = false;
                context.SaveChanges();
            }
        }

        public static void ActivateCustomerInDatabase(Customer customer)
        {
            using (var context = new ERPContext())
            {
                context.Entry(customer).State = EntityState.Modified;
                customer.IsActive = true;
                context.SaveChanges();
            }
        }

        private void ShowEditWindow()
        {
            var vm = new MasterCustomersEditVM(_selectedLine);
            var editWindow = new MasterCustomersEditView(vm)
            {
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            editWindow.ShowDialog();
        }

        private bool IsThereLineSelected()
        {
            if (_selectedLine != null) return true;
            MessageBox.Show("Please select a line.", "No Selection", MessageBoxButton.OK);
            return false;
        }

        private static bool IsConfirmationYes()
        {
            return MessageBox.Show("Confirm activating/deactivating customer?", "Confirmation", MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes;
        }
        #endregion
    }
}
