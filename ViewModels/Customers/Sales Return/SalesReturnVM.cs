namespace PUJASM.ERP.ViewModels.Customers.Sales_Return
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using Models.Sales;
    using MVVMFramework;
    using Sales;
    using Utilities;
    using Utilities.ModelHelpers;

    public class SalesReturnVM : ViewModelBase<SalesReturnTransaction>
    {
        #region Backing Fields
        private bool _notEditing;

        private string _salesReturnTransactionID;
        private decimal _salesReturnTransactionNetTotal;
        private DateTime _salesReturnTransactionDate;

        private string _selectedSalesTransactionID;
        private SalesTransactionLineVM _selectedSalesTransactionLine;
        private CustomerVM _selectedSalesTransactionCustomer;

        private SalesReturnTransactionLineVM _selectedLine;

        private ICommand _newCommand;
        private ICommand _saveCommand;
        private ICommand _deleteLineCommand;
        #endregion

        public SalesReturnVM()
        {
            _notEditing = true;

            Model = new SalesReturnTransaction();

            DisplayedSalesTransactionLines = new ObservableCollection<SalesTransactionLineVM>();
            DisplayedSalesReturnTransactionLines = new ObservableCollection<SalesReturnTransactionLineVM>();

            _salesReturnTransactionDate = UtilityMethods.GetCurrentDate().Date;
            SetNewSalesReturnTransactionID();

            NewEntryVM = new SalesReturnNewEntryVM(this);
        }

        public SalesReturnNewEntryVM NewEntryVM { get; }

        #region Collections
        public ObservableCollection<SalesTransactionLineVM> DisplayedSalesTransactionLines { get; }

        public ObservableCollection<SalesReturnTransactionLineVM> DisplayedSalesReturnTransactionLines { get; }
        #endregion

        public bool NotEditing
        {
            get { return _notEditing; }
            set { SetProperty(ref _notEditing, value, "NotEditing"); }
        }

        #region Sales Return Transaction Properties
        public string SalesReturnTransactionID
        {
            get { return _salesReturnTransactionID; }
            set
            {
                using (var context = new ERPContext())
                {
                    var salesReturnTransactionFromDatabase =  context.SalesReturnTransactions
                        .Include("SalesReturnTransactionLines")
                        .Include("SalesReturnTransactionLines.Warehouse")
                        .Include("SalesReturnTransactionLines.Item")
                        .SingleOrDefault(
                        salesReturnTransaction => salesReturnTransaction.SalesReturnTransactionID.Equals(value));

                    if (!IsSalesReturnTransactionInDatabase(salesReturnTransactionFromDatabase)) return;
                    SetEditTransactionMode(salesReturnTransactionFromDatabase);
                }

                SetProperty(ref _salesReturnTransactionID, value, () => SalesReturnTransactionID);
            }
        }

        public DateTime SalesReturnTransactionDate
        {
            get { return _salesReturnTransactionDate; }
            set { SetProperty(ref _salesReturnTransactionDate, value, "SalesReturnTransactionDate"); }
        }

        public decimal SalesReturnTransactionNetTotal
        {
            get {  return _salesReturnTransactionNetTotal;  }
            set { SetProperty(ref _salesReturnTransactionNetTotal, value, "SalesReturnTransactionNetTotal"); }
        }
        #endregion

        #region Selected Sales Transaction Properties
        public string SelectedSalesTransactionID
        {
            get { return _selectedSalesTransactionID; }
            set
            {
                SetProperty(ref _selectedSalesTransactionID, value, () => SelectedSalesTransactionID);
                if (_selectedSalesTransactionID == null) return;
                using (var context = new ERPContext())
                {
                    var salesTransactionFromDatabase = context.SalesTransactions
                        .Include("Customer")
                        .Include("SalesTransactionLines")
                        .Include("SalesTransactionLines.Item")
                        .Include("SalesTransactionLines.Warehouse")
                        .SingleOrDefault(transaction => transaction.SalesTransactionID.Equals(value));

                    if (!IsSalesTransactionInDatabase(salesTransactionFromDatabase)) return;
                    SetNewTransactionMode(salesTransactionFromDatabase);
                }
            }
        }
    
        public CustomerVM SelectedSalesTransactionCustomer
        {
            get { return _selectedSalesTransactionCustomer; }
            set { SetProperty(ref _selectedSalesTransactionCustomer, value, () => SelectedSalesTransactionCustomer); }
        }

        public SalesTransactionLineVM SelectedSalesTransactionLine
        {
            get { return _selectedSalesTransactionLine; }
            set
            {
                SetProperty(ref _selectedSalesTransactionLine, value, "SelectedSalesTransactionLine");
                if (_selectedSalesTransactionLine != null)
                    UpdateReturnEntryProperties();
            }
        }
        #endregion

        public SalesReturnTransactionLineVM SelectedLine
        {
            get { return _selectedLine; }
            set { SetProperty(ref _selectedLine, value, () => SelectedLine); }
        }

        #region Commands
        public ICommand NewCommand => _newCommand ?? (_newCommand = new RelayCommand(ResetTransaction));

        public ICommand SaveCommand
        {
            get
            {
                return _saveCommand ?? (_saveCommand = new RelayCommand(() =>
                {
                    if (DisplayedSalesReturnTransactionLines.Count == 0 || MessageBox.Show("Confirm transaction?", "Confirmation", MessageBoxButton.YesNo) !=
                        MessageBoxResult.Yes) return;
                    SetNewSalesReturnTransactionID();   // To avoid simultaneous input into the same ID
                    SetSalesReturnTransactionModelPropertiesToVMProperties();
                    SalesReturnTransactionHelper.AddSalesReturnTransactionToDatabase(Model);
                    if (SalesReturnTransactionHelper.IsLastSaveSuccessful) MessageBox.Show("Successfully added transaction!", "Success", MessageBoxButton.OK);
                    ResetTransaction();
                }));
            }
        }

        public ICommand DeleteLineCommand
        {
            get
            {
                return _deleteLineCommand ?? (_deleteLineCommand = new RelayCommand(() =>
                {
                    if (_selectedLine != null &&
                        MessageBox.Show("Confirm Deletion?", "Confirmation", MessageBoxButton.YesNo)
                        == MessageBoxResult.No)
                        return;
                    DisplayedSalesReturnTransactionLines.Remove(_selectedLine);
                    Debug.Assert(_selectedLine != null, "_selectedLine != null");
                    _salesReturnTransactionNetTotal -= _selectedLine.Total;
                    UpdateUINetTotal();
                }));
            }
        }
        #endregion

        #region Helper Methods
        private static bool IsSalesReturnTransactionInDatabase(SalesReturnTransaction salesReturnTransaction)
        {
            if (salesReturnTransaction != null) return true;
            MessageBox.Show("Sales return transaction does not exists.", "Invalid ID", MessageBoxButton.OK);
            return false;
        }

        private void SetEditTransactionMode(SalesReturnTransaction salesReturnTransaction)
        {
            Model = salesReturnTransaction;
            SelectedSalesTransactionID = Model.SalesTransaction.SalesTransactionID;
            SalesReturnTransactionDate = Model.Date;

            _salesReturnTransactionNetTotal = 0;
            foreach (var line in Model.SalesReturnTransactionLines)
            {
                DisplayedSalesReturnTransactionLines.Add(new SalesReturnTransactionLineVM {Model = line});
                _salesReturnTransactionNetTotal += line.Total;
            }
            UpdateUINetTotal();

            NotEditing = false;
        }

        private static bool IsSalesTransactionInDatabase(SalesTransaction salesTransaction)
        {
            if (salesTransaction != null) return true;
            MessageBox.Show("Please check if the transaction has been issued or exists.", "Invalid Sales Transaction", MessageBoxButton.OK);
            return false;
        }

        private void SetNewTransactionMode(SalesTransaction salesTransaction)
        {
            Model.SalesTransaction = salesTransaction;
            UpdateSalesTransactionLines(salesTransaction);
            if (Model.SalesTransaction.Customer != null)
                SelectedSalesTransactionCustomer = new CustomerVM { Model = Model.SalesTransaction.Customer };
            SelectedSalesTransactionLine = null;
            NotEditing = true;
        }

        private void UpdateSalesTransactionLines(SalesTransaction salesTransaction)
        {
            DisplayedSalesTransactionLines.Clear();
            foreach (var line in salesTransaction.SalesTransactionLines.ToList())
                DisplayedSalesTransactionLines.Add(new SalesTransactionLineVM { Model = line });
        }
        
        private void SetNewSalesReturnTransactionID()
        {
            var year = _salesReturnTransactionDate.Year;
            var month = _salesReturnTransactionDate.Month;
            var newEntryID = "MR" + (long)((year - 2000) * 100 + month) * 1000000;

            string lastEntryID = null;
            using (var context = new ERPContext())
            {
                var latestSalesReturnTransaction = context.SalesReturnTransactions.Where(
                    transaction => string.Compare(transaction.SalesReturnTransactionID, newEntryID, StringComparison.Ordinal) >= 0)
                    .OrderByDescending(transaction => transaction.SalesReturnTransactionID)
                    .FirstOrDefault();
                if (latestSalesReturnTransaction != null) lastEntryID = latestSalesReturnTransaction.SalesReturnTransactionID;
            }

            if (lastEntryID != null) newEntryID = "MR" + (Convert.ToInt64(lastEntryID.Substring(2)) + 1);

            Model.SalesReturnTransactionID = newEntryID;
            _salesReturnTransactionID = newEntryID;
            OnPropertyChanged("SalesReturnTransactionID");
        }

        private void UpdateReturnEntryProperties()
        {
            NewEntryVM.SalesReturnNewEntryProduct = _selectedSalesTransactionLine.Item.Name;
            NewEntryVM.SalesReturnNewEntryQuantity = 0;
            NewEntryVM.SalesReturnNewEntryPrice = 0;
        }

        private void SetSalesReturnTransactionModelPropertiesToVMProperties()
        {
            Model.Date = _salesReturnTransactionDate;
            Model.NetTotal = _salesReturnTransactionNetTotal;
            AddDisplayedSalesReturnTransactionLinesModelsIntoSalesReturnTransactionModel();
        }

        private void AddDisplayedSalesReturnTransactionLinesModelsIntoSalesReturnTransactionModel()
        {
            foreach (var salesReturnTransactionLine in DisplayedSalesReturnTransactionLines)
                Model.SalesReturnTransactionLines.Add(salesReturnTransactionLine.Model);
        }

        private void ResetTransaction()
        {
            DisplayedSalesReturnTransactionLines.Clear();
            DisplayedSalesTransactionLines.Clear();

            Model = new SalesReturnTransaction();
            NotEditing = true;

            SetNewSalesReturnTransactionID();
            SalesReturnTransactionDate = UtilityMethods.GetCurrentDate().Date;
            SalesReturnTransactionNetTotal = 0;

            SelectedSalesTransactionID = null;
            SelectedSalesTransactionCustomer = null;
            SelectedSalesTransactionLine = null;

            NewEntryVM.SalesReturnNewEntryProduct = null;
            NewEntryVM.SalesReturnNewEntryQuantity = 0;
            NewEntryVM.SalesReturnNewEntryPrice = 0;
        }

        private void UpdateUINetTotal()
        {

            OnPropertyChanged("SalesReturnTransactionNetTotal");
        }
        #endregion
    }
}
