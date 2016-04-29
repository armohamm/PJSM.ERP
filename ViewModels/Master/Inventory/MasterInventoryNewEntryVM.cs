namespace PUJASM.ERP.ViewModels.Master.Inventory
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using Item;
    using Models;
    using Models.Inventory;
    using MVVMFramework;
    using Utilities;
    using Utilities.ModelHelpers;

    internal class MasterInventoryNewEntryVM : ViewModelBase
    {
        private readonly MasterInventoryVM _parentVM;

        #region Backing Fields
        private string _newEntryID;
        private string _newEntryName;
        private ItemCategoryVM _newEntryCategory;
        private SupplierVM _newEntrySupplier;
        private decimal _newEntryPurchasePrice;
        private decimal _newEntrySalesPrice;
        private ICommand _newEntryCommand;
        private ICommand _cancelEntryCommand;
        #endregion

        public MasterInventoryNewEntryVM(MasterInventoryVM parentVM)
        {
            _parentVM = parentVM;
            Categories = new ObservableCollection<ItemCategoryVM>();
            Suppliers = new ObservableCollection<SupplierVM>();
            UpdateCategories();
            UpdateSuppliers();
        }

        #region Collections
        public ObservableCollection<ItemCategoryVM> Categories { get; }

        public ObservableCollection<SupplierVM> Suppliers { get; } 
        #endregion

        #region Properties
        public string NewEntryID
        {
            get { return _newEntryID; }
            set { SetProperty(ref _newEntryID, value, () => NewEntryID); }
        }

        public string NewEntryName
        {
            get { return _newEntryName; }
            set { SetProperty(ref _newEntryName, value, () => NewEntryName); }
        }

        public ItemCategoryVM NewEntryCategory
        {
            get { return _newEntryCategory; }
            set { SetProperty(ref _newEntryCategory, value, () => NewEntryCategory); }
        }

        public SupplierVM NewEntrySupplier
        {
            get { return _newEntrySupplier; }
            set { SetProperty(ref _newEntrySupplier, value, () => NewEntrySupplier); }
        }

        public decimal NewEntryPurchasePrice
        {
            get { return _newEntryPurchasePrice; }
            set { SetProperty(ref _newEntryPurchasePrice, value, () => NewEntryPurchasePrice); }
        }

        public decimal NewEntrySalesPrice
        {
            get { return _newEntrySalesPrice; }
            set { SetProperty(ref _newEntrySalesPrice, value, () => NewEntrySalesPrice); }
        }
        #endregion

        #region Commands
        public ICommand NewEntryCommand
        {
            get
            {
                return _newEntryCommand ?? (_newEntryCommand = new RelayCommand(() =>
                {
                    if (!AreAllEntryFieldsFilled() || !AreAllPriceEntryFieldsValid()) return;
                    if (MessageBox.Show("Confirm adding this product?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
                    var newEntryItem = MakeNewEntryItem();
                    InventoryHelper.AddItemToDatabase(newEntryItem);
                    ResetEntryFields();
                    _parentVM.UpdateItems();
                    _parentVM.UpdateDisplayedItems();
                }));
            }
        }

        public ICommand CancelEntryCommand => _cancelEntryCommand ?? (_cancelEntryCommand = new RelayCommand(ResetEntryFields));
        #endregion

        #region Helper Methods
        private void UpdateCategories()
        {
            using (var context = new ERPContext())
            {
                Categories.Clear();
                var categoriesFromDatabase = context.Categories.OrderBy(category => category.Name);
                foreach (var category in categoriesFromDatabase)
                    Categories.Add(new ItemCategoryVM {Model = category});
            }
        }

        private void UpdateSuppliers()
        {
            using (var context = new ERPContext())
            {
                Suppliers.Clear();
                var suppliersFromDatabase = context.Suppliers.OrderBy(supplier => supplier.Name);
                foreach (var supplier in suppliersFromDatabase)
                    Suppliers.Add(new SupplierVM { Model = supplier });
            }
        }

        private bool AreAllEntryFieldsFilled()
        {
            if (_newEntryID != null && _newEntryName != null &&
                _newEntryCategory != null && _newEntrySupplier != null)
                return true;

            MessageBox.Show("Please enter all fields", "Missing Fields", MessageBoxButton.OK);
            return false;
        }

        private bool AreAllPriceEntryFieldsValid()
        {
            if (_newEntrySalesPrice >= 0 && _newEntryPurchasePrice >= 0)
                return true;
            MessageBox.Show("Please check that all price fields are valid.", "Invalid Field(s)", MessageBoxButton.OK);
            return false;
        }

        private Item MakeNewEntryItem()
        {
            var newEntryItem = new Item
            {
                ID = _newEntryID,
                Name = _newEntryName,
                ItemCategory = _newEntryCategory.Model,
                PurchasePrice = _newEntryPurchasePrice,
                SalesPrice = _newEntrySalesPrice,
                Suppliers = new ObservableCollection<Supplier>()
            };
            newEntryItem.Suppliers.Add(_newEntrySupplier.Model);
            return newEntryItem;
        }

        private void ResetEntryFields()
        {
            NewEntryID = null;
            NewEntryName = null;
            NewEntryCategory = null;
            NewEntrySupplier = null;
            NewEntryPurchasePrice = 0;
            NewEntrySalesPrice = 0;
            UpdateCategories();
            UpdateSuppliers();
        }
        #endregion
    }
}
