namespace PUJASM.ERP.ViewModels.Inventory
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using Item;
    using MVVMFramework;
    using Models.StockCorrection;
    using Utilities;

    internal class StockAdjustmentNewEntryVM : ViewModelBase
    {
        private readonly StockAdjustmentVM _parentVM;

        #region New Entry Backing Fields
        private WarehouseVM _newEntryWarehouse;
        private ItemVM _newEntryProduct;
        private int _newEntryQuantity;
        private int _remainingStock;
        private string _newEntryRemainingStock;
        private ICommand _newEntryCommand;
        #endregion

        public StockAdjustmentNewEntryVM(StockAdjustmentVM parentVM)
        {
            _parentVM = parentVM;
            Warehouses = new ObservableCollection<WarehouseVM>();
            Products = new ObservableCollection<ItemVM>();
            UpdateWarehouses();
        }

        #region Collections
        public ObservableCollection<WarehouseVM> Warehouses { get; }

        public ObservableCollection<ItemVM> Products { get; }
        #endregion

        #region New Entry Properties
        public WarehouseVM NewEntryWarehouse
        {
            get { return _newEntryWarehouse; }
            set
            {
                SetProperty(ref _newEntryWarehouse, value, () => NewEntryWarehouse);
                if (_newEntryWarehouse == null) return;
                UpdateProducts();
            }
        }

        public ItemVM NewEntryProduct
        {
            get { return _newEntryProduct; }
            set
            {
                SetProperty(ref _newEntryProduct, value, () => NewEntryProduct);
                if (_newEntryProduct == null) return;
                SetRemainingStock();
            }
        }

        public int NewEntryQuantity
        {
            get { return _newEntryQuantity; }
            set { SetProperty(ref _newEntryQuantity, value, () => NewEntryQuantity); }
        }

        public string NewEntryRemainingStock
        {
            get { return _newEntryRemainingStock; }
            set { SetProperty(ref _newEntryRemainingStock, value, () => NewEntryRemainingStock); }
        }

        public ICommand NewEntryCommand
        {
            get
            {
                return _newEntryCommand ?? (_newEntryCommand = new RelayCommand(() =>
                {
                    if (!AreAllEntryFieldsFilled() || !IsQuantityValid()) return;
                    AddNewEntryToTransaction();
                    ResetEntryFields();
                }));
            }
        }
        #endregion

        #region New Entry Helper Methods
        public void UpdateWarehouses()
        {
            Warehouses.Clear();
            using (var context = new ERPContext())
            {
                var warehouses = context.Warehouses.OrderBy(warehouse => warehouse.Name);
                foreach (var warehouse in warehouses)
                    Warehouses.Add(new WarehouseVM { Model = warehouse });
            }
        }

        private void UpdateProducts()
        {
            Products.Clear();
            using (var context = new ERPContext())
            {
                var items = context.Inventory.OrderBy(item => item.Name);
                foreach (var item in items)
                    Products.Add(new ItemVM { Model = item });
            }
        }

        private void SetRemainingStock()
        {
            _remainingStock = UtilityMethods.GetRemainingStock(_newEntryProduct.Model, _newEntryWarehouse.Model);
            foreach (var line in _parentVM.DisplayedLines)
            {
                if (!line.Warehouse.ID.Equals(_newEntryWarehouse.ID) || 
                    !line.Item.ID.Equals(_newEntryProduct.ID) || line.Quantity > 0)
                    continue;
                _remainingStock -= -line.Quantity;
                break;
            }
            NewEntryRemainingStock = _remainingStock.ToString();
        }

        private bool AreAllEntryFieldsFilled()
        {
            if (_newEntryWarehouse != null && _newEntryProduct != null &&
                _newEntryQuantity != 0) return true;
            MessageBox.Show("Please enter all fields.", "Missing Field(s)", MessageBoxButton.OK);
            return false;
        }

        private bool IsQuantityValid()
        {

            var quantity = _newEntryQuantity;
            if (quantity >= 0 || -quantity <= _remainingStock) return true;
            MessageBox.Show("There is not enough stock.", "Insufficient Stock", MessageBoxButton.OK);
            return false;
        }

        private void AddNewEntryToTransaction()
        {
            foreach (var line in _parentVM.DisplayedLines)
            {
                if (!line.Item.ID.Equals(_newEntryProduct.ID) ||
                    !line.Warehouse.ID.Equals(_newEntryWarehouse.ID)) continue;
                line.Quantity += _newEntryQuantity;
                ResetEntryFields();
                return;
            }

            var newEntry = new StockAdjustmentTransactionLineVM
            {
                Model = new StockAdjustmentTransactionLine
                {
                    StockAdjustmentTransaction = _parentVM.Model,
                    Item = _newEntryProduct.Model,
                    Warehouse = _newEntryWarehouse.Model,
                    Quantity = _newEntryQuantity
                }
            };

            _parentVM.DisplayedLines.Add(newEntry);
        }


        public void ResetEntryFields()
        {
            NewEntryProduct = null;
            NewEntryQuantity = 0;
            NewEntryRemainingStock = null;
        }
        #endregion
    }
}
