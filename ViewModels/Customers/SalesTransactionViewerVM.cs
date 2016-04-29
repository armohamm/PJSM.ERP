namespace PUJASM.ERP.ViewModels.Customers
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using MVVMFramework;
    using Sales;
    using Utilities;

    internal class SalesTransactionViewerVM : ViewModelBase
    {
        private string _salesTransactionID;
        private ICommand _clearCommand;
        private decimal _salesTransactionGrossTotal;
        private decimal _salesTransactionDiscount;
        private decimal _salesTransactionNetTotal;
        private DateTime _salesTransactionDate;

        public SalesTransactionViewerVM() 
        {
            SalesTransactionLines = new ObservableCollection<SalesTransactionLineVM>();
            _salesTransactionDate = DateTime.MinValue;
        }

        public ObservableCollection<SalesTransactionLineVM> SalesTransactionLines { get; }

        public string SalesTransactionID
        {
            get { return _salesTransactionID; }
            set
            {
                SetProperty(ref _salesTransactionID, value, () => SalesTransactionID);
                if (_salesTransactionID == null) return;
                DisplaySalesTransaction();
            }
        }

        public DateTime SalesTransactionDate
        {
            get { return _salesTransactionDate; }
            set { SetProperty(ref _salesTransactionDate, value, () => SalesTransactionDate); }
        }

        public decimal SalesTransactionGrossTotal
        {
            get { return _salesTransactionGrossTotal; }
            set { SetProperty(ref _salesTransactionGrossTotal, value, () => SalesTransactionGrossTotal); }
        }

        public decimal SalesTransactionDiscount
        {
            get { return _salesTransactionDiscount; }
            set { SetProperty(ref _salesTransactionDiscount, value, () => SalesTransactionDiscount); }
        }

        public decimal SalesTransactionNetTotal
        {
            get { return _salesTransactionNetTotal; }
            set { SetProperty(ref _salesTransactionNetTotal, value, () => SalesTransactionNetTotal); }
        }

        public ICommand ClearCommand => _clearCommand ?? (_clearCommand = new RelayCommand(ClearSalesTransaction));

        private void ClearSalesTransaction()
        {
            SalesTransactionID = null;
            SalesTransactionDate = DateTime.MinValue;
            SalesTransactionGrossTotal = 0;
            SalesTransactionDiscount = 0;
            SalesTransactionNetTotal = 0;
            SalesTransactionLines.Clear();
        }

        private void DisplaySalesTransaction()
        {
            using (var context = new ERPContext())
            {
                var salesTransactionFromDatabase =
                    context.SalesTransactions
                    .Include("SalesTransactionLines")
                    .Include("SalesTransactionLines.Item")
                    .Include("SalesTransactionLines.Warehouse")
                    .SingleOrDefault(
                        transaction => transaction.SalesTransactionID.Equals(_salesTransactionID));

                if (salesTransactionFromDatabase == null)
                {
                    MessageBox.Show("Sales transaction could not be found.", "Invalid ID", MessageBoxButton.OK);
                    return;
                }

                foreach (var line in salesTransactionFromDatabase.SalesTransactionLines)
                    SalesTransactionLines.Add(new SalesTransactionLineVM {Model = line});

                SalesTransactionDate = salesTransactionFromDatabase.Date;
                SalesTransactionGrossTotal = salesTransactionFromDatabase.GrossTotal;
                SalesTransactionDiscount = salesTransactionFromDatabase.Discount;
                SalesTransactionNetTotal = salesTransactionFromDatabase.Total;
            }
        }
    }
}
