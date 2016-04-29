namespace PUJASM.ERP.ViewModels.Suppliers.Purchase_Return
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using Item;
    using Models.Purchase;
    using MVVMFramework;
    using Utilities;
    using ViewModels.Purchase;

    internal class PurchaseReturnNewEntryVM : ViewModelBase
    {
        private readonly PurchaseReturnVM _parentVM;

        #region Backing Fields
        private WarehouseVM _purchaseReturnEntryWarehouse;
        private string _purchaseReturnEntryProduct;
        private int _purchaseReturnEntryQuantity;
        private string _purchaseReturnEntryAvailableQuantity;
        private decimal _purchaseReturnEntryPrice;
        private ICommand _purchaseReturnEntryAddCommand;
        #endregion

        public PurchaseReturnNewEntryVM(PurchaseReturnVM parentVM)
        {
            _parentVM = parentVM;
            _purchaseReturnEntryAvailableQuantity = "0";
        }

        public ObservableCollection<WarehouseVM> Warehouses => _parentVM.Warehouses;

        public ObservableCollection<PurchaseReturnTransactionLineVM> PurchaseReturnTransactionLines
            => _parentVM.PurchaseReturnTransactionLines;

        #region Return Entry Properties
        public WarehouseVM PurchaseReturnEntryWarehouse
        {
            get { return _purchaseReturnEntryWarehouse; }
            set
            {
                SetProperty(ref _purchaseReturnEntryWarehouse, value, () => PurchaseReturnEntryWarehouse);
                if (_purchaseReturnEntryWarehouse == null) return;
                SetPurchaseReturnEntryAvailableQuantity();
            }
        }

        public string PurchaseReturnEntryProduct
        {
            get { return _purchaseReturnEntryProduct; }
            set { SetProperty(ref _purchaseReturnEntryProduct, value, () => PurchaseReturnEntryProduct); }
        }

        public int PurchaseReturnEntryQuantity
        {
            get { return _purchaseReturnEntryQuantity; }
            set { SetProperty(ref _purchaseReturnEntryQuantity, value, () => PurchaseReturnEntryQuantity); }
        }

        public string PurchaseReturnEntryAvailableQuantity
        {
            get { return _purchaseReturnEntryAvailableQuantity; }
            set
            {
                SetProperty(ref _purchaseReturnEntryAvailableQuantity, value, () => PurchaseReturnEntryAvailableQuantity);
            }
        }

        public decimal PurchaseReturnEntryPrice
        {
            get { return _purchaseReturnEntryPrice; }
            set { SetProperty(ref _purchaseReturnEntryPrice, value, () => PurchaseReturnEntryPrice); }
        }

        public ICommand PurchaseReturnEntryAddCommand
        {
            get
            {
                return _purchaseReturnEntryAddCommand ?? (_purchaseReturnEntryAddCommand = new RelayCommand(() =>
                {
                    if (!IsTherePurchaseTransactionLineSelected() || !IsReturnEntryPriceValid() ||
                        !IsReturnEntryQuantityValid()) return;
                    AddEntryToPurchaseReturnTransactionLines();
                    ResetEntryFields();
                }));
            }
        }
        #endregion

        #region Helper Methods
        private bool IsTherePurchaseTransactionLineSelected()
        {
            if (_purchaseReturnEntryProduct != null && _parentVM.SelectedPurchaseTransactionLine != null) return true;
            MessageBox.Show("No product is selected", "Invalid Command", MessageBoxButton.OK);
            return false;
        }

        private bool IsReturnEntryPriceValid()
        {
            var maximumReturnPrice = _parentVM.SelectedPurchaseTransactionLine.PurchasePrice -
                                     _parentVM.SelectedPurchaseTransactionLine.GetNetDiscount();
            if (_purchaseReturnEntryPrice >= 0 && _purchaseReturnEntryPrice <= maximumReturnPrice) return true;
            MessageBox.Show($"The valid return price is 0 - {maximumReturnPrice}", "Invalid Command",
                MessageBoxButton.OK);
            return false;
        }

        private bool IsReturnEntryQuantityValid()
        {
            var availableReturnQuantity = GetAvailableReturnQuantity();
            if (_purchaseReturnEntryQuantity <= availableReturnQuantity && _purchaseReturnEntryQuantity > 0)
                return true;
            MessageBox.Show(
                $"The valid return quantity is {availableReturnQuantity}",
                "Invalid Quantity Input", MessageBoxButton.OK);
            return false;
        }

        private void SetPurchaseReturnEntryAvailableQuantity()
        {
            var availableQuantity = GetAvailableReturnQuantity();
            PurchaseReturnEntryAvailableQuantity = availableQuantity.ToString();
        }

        private int GetAvailableReturnQuantity()
        {
            var availableQuantity = _parentVM.SelectedPurchaseTransactionLine.Quantity -
                                    _parentVM.SelectedPurchaseTransactionLine.SoldOrReturned;
            var stock = UtilityMethods.GetRemainingStock(_parentVM.SelectedPurchaseTransactionLine.Item,
                _purchaseReturnEntryWarehouse.Model);
            if (stock < availableQuantity) availableQuantity = stock;
            foreach (var line in PurchaseReturnTransactionLines)
            {
                if (line.Item.ID.Equals(_parentVM.SelectedPurchaseTransactionLine.Item.ID) &&
                    line.Warehouse.ID.Equals(_parentVM.SelectedPurchaseTransactionLine.Warehouse.ID) &&
                    line.Discount.Equals(_parentVM.SelectedPurchaseTransactionLine.Discount) &&
                    line.PurchasePrice.Equals(_parentVM.SelectedPurchaseTransactionLine.PurchasePrice))
                {
                    availableQuantity -= line.Quantity;
                }
            }
            return availableQuantity;
        }

        private bool IsPurchaseReturnTransactionLineCombinableWithNewEntry(PurchaseReturnTransactionLineVM line)
        {
            return line.Item.ID.Equals(_parentVM.SelectedPurchaseTransactionLine.Item.ID) &&
                   line.Warehouse.ID.Equals(_parentVM.SelectedPurchaseTransactionLine.Warehouse.ID) &&
                   line.ReturnWarehouse.ID.Equals(_purchaseReturnEntryWarehouse.ID) &&
                   line.PurchasePrice.Equals(_parentVM.SelectedPurchaseTransactionLine.PurchasePrice) &&
                   line.Discount.Equals(_parentVM.SelectedPurchaseTransactionLine.Discount) &&
                   line.ReturnPrice.Equals(_purchaseReturnEntryPrice);
        }

        private void AddEntryToPurchaseReturnTransactionLines()
        {
            foreach (
                var line in PurchaseReturnTransactionLines.Where(IsPurchaseReturnTransactionLineCombinableWithNewEntry))
            {
                CombinePurchaseReturnLineWithNewEntry(line);
                return;
            }
            var newEntryPurchaseReturnLineVM = MakeNewEntryPurchaseReturnTransactionLine();
            PurchaseReturnTransactionLines.Add(newEntryPurchaseReturnLineVM);
            _parentVM.PurchaseReturnTransactionNetTotal += newEntryPurchaseReturnLineVM.Total;
        }

        private void CombinePurchaseReturnLineWithNewEntry(PurchaseReturnTransactionLineVM line)
        {
            line.Quantity += _purchaseReturnEntryQuantity;
            line.Total += _purchaseReturnEntryQuantity * _purchaseReturnEntryPrice;
            _parentVM.PurchaseReturnTransactionNetTotal += _purchaseReturnEntryQuantity * _purchaseReturnEntryPrice;
        }

        private PurchaseReturnTransactionLineVM MakeNewEntryPurchaseReturnTransactionLine()
        {
            var purchaseReturnTransactionLine = new PurchaseReturnTransactionLine
            {
                PurchaseReturnTransaction = _parentVM.Model,
                Item = _parentVM.SelectedPurchaseTransactionLine.Item,
                Warehouse = _parentVM.SelectedPurchaseTransactionLine.Warehouse,
                PurchasePrice = _parentVM.SelectedPurchaseTransactionLine.PurchasePrice,
                Discount = _parentVM.SelectedPurchaseTransactionLine.Discount,
                ReturnPrice = _purchaseReturnEntryPrice,
                Quantity = _purchaseReturnEntryQuantity,
                Total = _purchaseReturnEntryQuantity*_purchaseReturnEntryPrice,
                ReturnWarehouse = _purchaseReturnEntryWarehouse.Model
            };
            return new PurchaseReturnTransactionLineVM {Model = purchaseReturnTransactionLine};
        }

        public void ResetEntryFields()
        {
            _parentVM.SelectedPurchaseTransactionLine = null;
            PurchaseReturnEntryProduct = null;
            PurchaseReturnEntryQuantity = 0;
            PurchaseReturnEntryPrice = 0;
            PurchaseReturnEntryAvailableQuantity = "0";
            PurchaseReturnEntryWarehouse = null;
        }

        #endregion
    }
}
