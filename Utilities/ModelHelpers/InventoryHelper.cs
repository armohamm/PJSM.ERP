namespace PUJASM.ERP.Utilities.ModelHelpers
{
    using System.Data.Entity;
    using System.Windows;
    using Models.Inventory;
    using System.Linq;
    using System.Transactions;

    public static class InventoryHelper
    {
        public static void AddItemToDatabase(Item item)
        {
            var success = true;
            var context = new ERPContext();
            try
            {
                AddItemToDatabaseContext(context, item);
                context.SaveChanges();
            }
            catch
            {
                success = false;
                MessageBox.Show("The item ID is already being used.", "Invalid ID", MessageBoxButton.OK);
            }
            finally
            {
                if (success)
                    MessageBox.Show("Successfully added item!", "Success", MessageBoxButton.OK);
                context.Dispose();
            }
        }

        public static void SaveItemEditsToDatabase(Item editingItem, Item editedItem)
        {
            using (var ts = new TransactionScope())
            {
                var context = new ERPContext();
                SaveItemEditsToDatabaseContext(context, editingItem, editedItem);
                ts.Complete();
            }
        }

        public static void DeepCopyItemProperties(Item fromItem, Item toItem)
        {
            toItem.Name = fromItem.Name;
            toItem.ItemCategory = fromItem.ItemCategory;
            toItem.PurchasePrice = fromItem.PurchasePrice;
            toItem.SalesPrice = fromItem.SalesPrice;
        }

        public static void DeactivateItemInDatabase(Item item)
        {
            using (var context = new ERPContext())
            {
                context.Entry(item).State = EntityState.Modified;
                item.IsActive = false;
                context.SaveChanges();
            }
        }

        public static void ActivateItemInDatabase(Item item)
        {
            using (var context = new ERPContext())
            {
                context.Entry(item).State = EntityState.Modified;
                item.IsActive = true;
                context.SaveChanges();
            }
        }

        #region Add Item Helper Methods
        private static void AddItemToDatabaseContext(ERPContext context, Item item)
        {
            AttachItemSupplierToDatabaseContext(context, item);
            AttachItemCategoryToDatabaseContext(context, item);
            context.Inventory.Add(item);
        }

        private static void AttachItemCategoryToDatabaseContext(ERPContext context, Item item)
        {
            item.ItemCategory = context.Categories.Single(category => category.ID.Equals(item.ItemCategory.ID));
        }

        private static void AttachItemSupplierToDatabaseContext(ERPContext context, Item item)
        {
            var supplierToBeAttached = item.Suppliers.First();
            item.Suppliers.RemoveAt(0);
            supplierToBeAttached = context.Suppliers.FirstOrDefault(e => e.ID.Equals(supplierToBeAttached.ID));
            item.Suppliers.Add(supplierToBeAttached);
        }
        #endregion

        #region Edit Item Helper Methods
        private static void SaveItemEditsToDatabaseContext(ERPContext context, Item editingItem, Item editedItem)
        {
            editingItem = context.Inventory.Single(item => item.ID.Equals(editingItem.ID));
            DeepCopyItemProperties(editedItem, editingItem);
            editingItem.ItemCategory = context.Categories.Single(category => category.ID.Equals(editingItem.ItemCategory.ID));
            AssignEditedItemSuppliersToEditingSupplier(context, editingItem, editedItem);
            context.SaveChanges();
        }

        private static void AssignEditedItemSuppliersToEditingSupplier(ERPContext context, Item editingItem, Item editedItem)
        {
            // Assign editedItem's suppliers to editingItem's 
            var editingSuppliers = editingItem.Suppliers;
            editingItem.Suppliers.ToList().ForEach(supplier => editingSuppliers.Remove(supplier));
            foreach (var attachedSupplier in editedItem.Suppliers.ToList()
                .Select(supplierToBeAdded => context.Suppliers.First(supplier => supplier.ID.Equals(supplierToBeAdded.ID))))
                editingItem.Suppliers.Add(attachedSupplier);
        }

        #endregion
    }
}
