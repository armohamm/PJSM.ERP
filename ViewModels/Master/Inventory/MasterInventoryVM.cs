namespace PUJASM.ERP.ViewModels.Master.Inventory
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using Item;
    using Models.Inventory;
    using MVVMFramework;
    using Utilities;
    using Utilities.ModelHelpers;
    using Views.Master.Inventory;

    internal class MasterInventoryVM : ViewModelBase
    {
        #region Backing Fields
        private bool _isActiveChecked;
        private ItemCategoryVM _selectedCategory;
        private ItemVM _selectedItem;
        private SupplierVM _selectedSupplier;
        private ItemVM _selectedLine;
        private ICommand _displayCommand;
        private ICommand _activateItemCommand;
        private ICommand _editItemCommand;
        #endregion

        public MasterInventoryVM() 
        {
            Categories = new ObservableCollection<ItemCategoryVM>();
            Items = new ObservableCollection<ItemVM>();
            Suppliers = new ObservableCollection<SupplierVM>();
            DisplayedItems = new ObservableCollection<ItemVM>();
            UpdateCategories();
            UpdateSuppliers();
            SelectedCategory = Categories.FirstOrDefault();
            SelectedItem = Items.FirstOrDefault();
            UpdateDisplayedItems();
            _isActiveChecked = false;
            NewEntryVM = new MasterInventoryNewEntryVM(this);
        }

        public MasterInventoryNewEntryVM NewEntryVM { get; }

        #region Colletions
        public ObservableCollection<ItemCategoryVM> Categories { get; } 

        public ObservableCollection<ItemVM> Items { get; }

        public ObservableCollection<SupplierVM> Suppliers { get; } 

        public ObservableCollection<ItemVM> DisplayedItems { get; } 
        #endregion

        #region Properties
        public bool IsActiveChecked
        {
            get { return _isActiveChecked; }
            set { SetProperty(ref _isActiveChecked, value, () => IsActiveChecked); }
        }

        public ItemCategoryVM SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                Items.Clear();
                SelectedItem = null;
                SetProperty(ref _selectedCategory, value, () => SelectedCategory);
                if (_selectedCategory == null) return;
                UpdateItems();
            }
        }

        public ItemVM SelectedItem
        {
            get { return _selectedItem; }
            set { SetProperty(ref _selectedItem, value, () => SelectedItem); }
        }

        public SupplierVM SelectedSupplier
        {
            get { return _selectedSupplier; }
            set
            {
                SelectedCategory = null;
                SetProperty(ref _selectedSupplier, value, () => SelectedSupplier);
            }
        }

        public ItemVM SelectedLine
        {
            get { return _selectedLine; }
            set { SetProperty(ref _selectedLine, value, () => SelectedLine); }
        }
        #endregion

        #region Commands
        public ICommand DisplayCommand
        {
            get
            {
                return _displayCommand ?? (_displayCommand = new RelayCommand(() =>
                {
                    if (_selectedItem == null && _selectedSupplier == null) return;
                    UpdateDisplayedItems();
                    UpdateCategories();
                    UpdateSuppliers();
                }));
            }
        }

        public ICommand ActivateItemCommand
        {
            get
            {
                return _activateItemCommand ?? (_activateItemCommand = new RelayCommand(() =>
                {
                    if (!IsThereLineSelected() || !IsConfirmationYes()) return;
                    if (_selectedLine.IsActive) InventoryHelper.DeactivateItemInDatabase(_selectedLine.Model);
                    else InventoryHelper.ActivateItemInDatabase(_selectedLine.Model);
                    _selectedLine.IsActive = !IsActiveChecked;
                }));
            }
        }

        public ICommand EditItemCommand
        {
            get
            {
                return _editItemCommand ?? (_editItemCommand = new RelayCommand(() =>
                {
                    if (!IsThereLineSelected()) return;
                    ShowEditWindow();
                    UpdateItems();
                }));
            }
        }
        #endregion

        #region Helper Methods
        private void UpdateCategories()
        {
            var oldSelectedCategory = _selectedCategory;

            using (var context = new ERPContext())
            {
                Categories.Clear();
                var allCategory = new ItemCategory { ID = -1, Name = "All" };
                Categories.Add(new ItemCategoryVM { Model = allCategory });
                var categoriesFromDatabase = context.Categories.OrderBy(category => category.Name);
                foreach (var category in categoriesFromDatabase)
                    Categories.Add(new ItemCategoryVM { Model = category });
            }

            UpdateSelectedCategory(oldSelectedCategory);
        }

        private void UpdateSelectedCategory(ItemCategoryVM oldSelectedCategory)
        {
            if (oldSelectedCategory == null) return;
            SelectedCategory = Categories.SingleOrDefault(category => category.ID.Equals(oldSelectedCategory.ID));
        }

        public void UpdateItems()
        {
            var oldSelectedItem = _selectedItem;

            using (var context = new ERPContext())
            {
                Items.Clear();
                var allItem = new Item { ID = "-1", Name = "All" };
                Items.Add(new ItemVM { Model = allItem });
                var itemsFromDatabase = _selectedCategory.Name.Equals("All")
                    ? context.Inventory
                        .Include("Suppliers")
                        .Include("ItemCategory")
                        .OrderBy(item => item.Name)
                    : context.Inventory
                        .Include("Suppliers")
                        .Include("ItemCategory")
                        .Where(
                            item => item.ItemCategory.ID.Equals(_selectedCategory.ID))
                        .OrderBy(item => item.Name);
                foreach (var item in itemsFromDatabase)
                    Items.Add(new ItemVM { Model = item, SelectedSupplier = item.Suppliers.FirstOrDefault() });
            }

            UpdateSelectedItem(oldSelectedItem);
        }

        private void UpdateSelectedItem(ItemVM oldSelectedItem)
        {
            if (oldSelectedItem == null) return;
            SelectedItem = Items.SingleOrDefault(item => item.ID.Equals(oldSelectedItem.ID));
        }

        private void UpdateSuppliers()
        {
            var oldSelectedSupplier = _selectedSupplier;

            using (var context = new ERPContext())
            {
                Suppliers.Clear();
                var suppliersFromDatabase = context.Suppliers.OrderBy(supplier => supplier.Name);
                foreach (var supplier in suppliersFromDatabase)
                    Suppliers.Add(new SupplierVM { Model = supplier });
            }

            UpdateSelectedSupplier(oldSelectedSupplier);
        }

        private void UpdateSelectedSupplier(SupplierVM oldSelectedSupplier)
        {
            if (oldSelectedSupplier == null) return;
            SelectedSupplier = Suppliers.SingleOrDefault(supplier => supplier.ID.Equals(oldSelectedSupplier.ID));
        }

        public void UpdateDisplayedItems()
        {
            DisplayedItems.Clear();
            if (_selectedItem != null)
                LoadSelectedItemToDisplayedItems();
            else
                LoadSelectedSupplierItemsToDisplayedItems();
        }

        private void LoadSelectedItemToDisplayedItems()
        {
            if (_selectedItem.Name.Equals("All"))
            {
                foreach (var item in Items.Where(item => !item.Name.Equals("All")))
                    DisplayedItems.Add(item);    
            }
            else 
                DisplayedItems.Add(_selectedItem);
        }

        private void LoadSelectedSupplierItemsToDisplayedItems()
        {
            using (var context = new ERPContext())
            {
                var supplierFromDatabase =
                    context.Suppliers.Include("Items").Single(supplier => supplier.ID.Equals(_selectedSupplier.ID));
                foreach (var item in supplierFromDatabase.Items.ToList().OrderBy(item => item.Name))
                    DisplayedItems.Add(new ItemVM {Model = item});
            }  
        }

        private bool IsThereLineSelected()
        {
            if (_selectedLine != null) return true;
            MessageBox.Show("Please select a line.", "No Selection", MessageBoxButton.OK);
            return false;
        }

        private static bool IsConfirmationYes()
        {
            return MessageBox.Show("Confirm activating/deactivating item?", "Confirmation", MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        private void ShowEditWindow()
        {
            var vm = new MasterInventoryEditVM(_selectedLine);
            var editWindow = new MasterInventoryEditView(vm)
            {
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            editWindow.ShowDialog();
        }
        #endregion
    }
}
