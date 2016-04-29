namespace PUJASM.ERP.ViewModels.Reports
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using Models;
    using Models.Sales;
    using MVVMFramework;
    using Sales;
    using Utilities;

    internal class OverallSalesReportVM : ViewModelBase
    {
        #region Backing Fields
        private CustomerVM _selectedCustomer;
        private DateTime _fromDate;
        private DateTime _toDate;
        private decimal _total;
        private decimal _remaining;

        private ICommand _printCommand;
        private ICommand _displayCommand;
        #endregion

        public OverallSalesReportVM()
        {
            Customers = new ObservableCollection<CustomerVM>();
            DisplayedSalesTransactions = new ObservableCollection<SalesTransactionVM>();
            var currentDate = UtilityMethods.GetCurrentDate();
            _fromDate = currentDate.AddDays(-currentDate.Day + 1);
            _toDate = currentDate;
            UpdateCustomers();
        }

        public ObservableCollection<SalesTransactionVM> DisplayedSalesTransactions { get; }

        public ObservableCollection<CustomerVM> Customers { get; }

        #region Properties
        public CustomerVM SelectedCustomer
        {
            get { return _selectedCustomer; }
            set { SetProperty(ref _selectedCustomer, value, () => SelectedCustomer); }
        }

        public DateTime FromDate
        {
            get { return _fromDate; }
            set
            {
                if (_toDate < value)
                {
                    MessageBox.Show("Please select a valid date range.", "Invalid Date Range", MessageBoxButton.OK);
                    return;
                }

                SetProperty(ref _fromDate, value, "FromDate");
            }
        }

        public DateTime ToDate
        {
            get { return _toDate; }
            set
            {
                if (_fromDate > value)
                {
                    MessageBox.Show("Please select a valid date range.", "Invalid Date Range", MessageBoxButton.OK);
                    return;
                }

                SetProperty(ref _toDate, value, "ToDate");
            }
        }

        public decimal Total
        {
            get { return _total; }
            set { SetProperty(ref _total, value, () => Total); }
        }
        #endregion

        #region Commands
        public ICommand DisplayCommand
        {
            get
            {
                return _displayCommand ?? (_displayCommand = new RelayCommand(() =>
                {
                    if (_selectedCustomer == null) return;
                    UpdateDisplayLines();
                    UpdateCustomers();
                }));
            }
        }

        public ICommand PrintCommand
        {
            get
            {
                return _printCommand ?? (_printCommand = new RelayCommand(() =>
                {
                    if (DisplayedSalesTransactions.Count == 0) return;
                    //ShowPrintWindow();
                }));
            }
        }
        #endregion

        #region Helper Methods
        private void UpdateCustomers()
        {
            var oldSelectedCustomer = _selectedCustomer;

            Customers.Clear();
            Customers.Add(new CustomerVM { Model = new Customer { ID = "-1", FirstName = "All" } });

            using (var context = new ERPContext())
            {
                var customersFromDatabase = context.Customers.OrderBy(customer => customer.FirstName);
                foreach (var customer in customersFromDatabase)
                    Customers.Add(new CustomerVM {Model = customer});
            }

            UpdateSelectedCustomer(oldSelectedCustomer);
        }

        private void UpdateSelectedCustomer(CustomerVM oldSelectedCustomer)
        {
            if (oldSelectedCustomer == null) return;
            SelectedCustomer = Customers.FirstOrDefault(customer => customer.ID.Equals(oldSelectedCustomer.ID));
        }

        private void UpdateDisplayLines()
        {
            _total = 0;
            _remaining = 0;
            DisplayedSalesTransactions.Clear();

            var searchCondition = GetSearchConditionAccordingToSelectedCustomer();

            using (var context = new ERPContext())
            {
                var salesTransactionsFromDatabase = context.SalesTransactions
                    .Include("User")
                    .Include("Customer")
                    .Where(searchCondition)
                    .OrderBy(salesTransaction => salesTransaction.Date)
                    .ThenBy(salesTransaction => salesTransaction.SalesTransactionID);

                foreach (var salesTransaction in salesTransactionsFromDatabase)
                {
                    DisplayedSalesTransactions.Add(new SalesTransactionVM {Model = salesTransaction});
                    _total += salesTransaction.Total;
                }
            }

            UpdateUITotalAndRemaining();
        }

        private Func<SalesTransaction, bool> GetSearchConditionAccordingToSelectedCustomer()
        {
            if (!_selectedCustomer.FirstName.Equals("All"))
                return salesTransaction =>
                    salesTransaction.Customer.FirstName.Equals(_selectedCustomer.Name)
                    && salesTransaction.Date >= _fromDate && salesTransaction.Date <= _toDate;

            return salesTransaction => salesTransaction.Date >= _fromDate && salesTransaction.Date <= _toDate;
        }

        private void UpdateUITotalAndRemaining()
        {
            OnPropertyChanged("Total");
        }

        //private void ShowPrintWindow()
        //{
        //    var overallSalesReportWindow = new OverallSalesReportWindow(DisplayedSalesTransactions)
        //    {
        //        Owner = Application.Current.MainWindow,
        //        WindowStartupLocation = WindowStartupLocation.CenterOwner
        //    };
        //    overallSalesReportWindow.Show();
        //}
        #endregion
    }
}
