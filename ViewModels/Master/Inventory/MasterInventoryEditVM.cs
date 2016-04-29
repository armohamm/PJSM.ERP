namespace PUJASM.ERP.ViewModels.Master.Inventory
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using Item;
    using MVVMFramework;
    using Models;
    using Models.Inventory;
    using Utilities;
    using Utilities.ModelHelpers;
    using ViewModels;
    using Views.Master.Inventory;

    public class MasterInventoryEditVM : ViewModelBase
    {
        #region Backing Fields
        private string _editID;
        private string _editName;
        private ItemCategoryVM _editCategory;
        private decimal _editPurchasePrice;
        private decimal _editSalesPrice;
        private SupplierVM _editSelectedSupplier;
        private ICommand _editAddSupplierCommand;
        private ICommand _editDeleteSupplierCommand;
        private ICommand _editConfirmCommand;
        #endregion

        public MasterInventoryEditVM(ItemVM editingItem)
        {
            EditingItem = editingItem;
            EditSuppliers = new ObservableCollection<SupplierVM>();
            Categories = new ObservableCollection<ItemCategoryVM>();
            UpdateCategories();
            SetDefaultEditProperties();
        }

        #region Collections
        public ObservableCollection<ItemCategoryVM> Categories { get; }

        public ObservableCollection<SupplierVM> EditSuppliers { get; }
        #endregion

        #region Properties
        public ItemVM EditingItem { get; }

        public string EditID
        {
            get { return _editID; }
            set { SetProperty(ref _editID, value, () => EditID); }
        }

        public string EditName
        {
            get { return _editName; }
            set { SetProperty(ref _editName, value, () => EditName); }
        }

        public ItemCategoryVM EditCategory
        {
            get { return _editCategory; }
            set { SetProperty(ref _editCategory, value, "EditCategory"); }
        }

        public decimal EditSalesPrice
        {
            get { return _editSalesPrice; }
            set { SetProperty(ref _editSalesPrice, value, () => EditSalesPrice); }
        }

        public decimal EditPurchasePrice
        {
            get { return _editPurchasePrice; }
            set { SetProperty(ref _editPurchasePrice, value, () => EditPurchasePrice); }
        }

        public SupplierVM EditSelectedSupplier
        {
            get { return _editSelectedSupplier; }
            set { SetProperty(ref _editSelectedSupplier, value, "EditSelectedSupplier"); }
        }
        #endregion

        #region Commands
        public ICommand EditAddSupplierCommand => _editAddSupplierCommand ?? (_editAddSupplierCommand = new RelayCommand(ShowEditAddSupplierWindow));

        public ICommand EditDeleteSupplierCommand
        {
            get
            {
                return _editDeleteSupplierCommand ?? (_editDeleteSupplierCommand = new RelayCommand(() =>
                {
                    EditSuppliers.Remove(_editSelectedSupplier);
                }));
            }
        }

        public ICommand EditConfirmCommand
        {
            get
            {
                return _editConfirmCommand ?? (_editConfirmCommand = new RelayCommand(() =>
                {
                    if (!IsEditConfirmationYes() && !AreEditFieldsValid()) return;
                    var originalItem = EditingItem.Model;
                    var editedItemCopy = MakeEditedItem();
                    InventoryHelper.SaveItemEditsToDatabase(originalItem, editedItemCopy);
                    UpdateEditingItemUIValues();
                    UtilityMethods.CloseForemostWindow();
                }));
            }
        }
        #endregion

        #region Helper Methods
        private Item MakeEditedItem()
        {
            var editedItem = new Item
            {
                ID = _editID,
                ItemCategory = _editCategory.Model,
                Name = _editName,
                PurchasePrice = _editPurchasePrice,
                SalesPrice = _editSalesPrice,
                Suppliers = new ObservableCollection<Supplier>()
            };

            foreach (var supplier in EditSuppliers.ToList())
                editedItem.Suppliers.Add(supplier.Model);

            return editedItem;
        }

        private void SetDefaultEditProperties()
        {
            EditID = EditingItem.ID;
            EditName = EditingItem.Name;
            EditCategory = Categories.SingleOrDefault(category => category.ID.Equals(EditingItem.ItemCategory.ID));
            EditSalesPrice = EditingItem.SalesPrice;
            EditPurchasePrice = EditingItem.PurchasePrice;
            UpdateEditSuppliers();
            EditSelectedSupplier = EditSuppliers.FirstOrDefault();
        }

        private void UpdateCategories()
        {
            Categories.Clear();
            using (var context = new ERPContext())
            {
                var categoriesFromDatabase = context.Categories.OrderBy(category => category.Name);
                foreach (var category in categoriesFromDatabase)
                    Categories.Add(new ItemCategoryVM {Model = category});
            }
        }

        private void UpdateEditSuppliers()
        {
            EditSuppliers.Clear();
            foreach (var supplier in EditingItem.Suppliers.OrderBy(supplier => supplier.Name))
                EditSuppliers.Add(new SupplierVM { Model = supplier });
        }


        private void ShowEditAddSupplierWindow()
        {
            var vm = new MasterInventoryEditAddSupplierVM(EditSuppliers);
            var editAddSupplierWindow = new MasterInventoryEditAddSupplierView(vm)
            {
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            editAddSupplierWindow.ShowDialog();
        }


        private static bool IsEditConfirmationYes()
        {
            return MessageBox.Show("Confirm edit?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes;
        }

        private bool AreEditFieldsValid()
        {
            if (_editID != null && _editName != null) return true;
            MessageBox.Show("Please enter all fields.", "Missing Field(s)", MessageBoxButton.OK);
            return false;
        }

        private void UpdateEditingItemUIValues()
        {
            var editedItem = MakeEditedItem();
            var itemTo = EditingItem.Model;
            InventoryHelper.DeepCopyItemProperties(editedItem, itemTo);
            EditingItem.Suppliers.Clear();
            foreach (var supplier in EditSuppliers)
                EditingItem.Suppliers.Add(supplier.Model);
            EditingItem.UpdatePropertiesToUI();
        }
        #endregion
    }
}
