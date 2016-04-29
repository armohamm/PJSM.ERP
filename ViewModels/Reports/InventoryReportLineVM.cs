namespace PUJASM.ERP.ViewModels.Reports
{
    using System.Linq;
    using Models;
    using Models.Inventory;
    using MVVMFramework;

    public class InventoryReportLineVM : ViewModelBase
    {
        private Item _item;
        private int _quantity;

        public InventoryReportLineVM(Item item)
        {
            _item = item;
        }

        public Item Item
        {
            get { return _item; }
            set { SetProperty(ref _item, value, () => Item); }
        }

        public decimal SalesPrice => _item.SalesPrice;

        public decimal PurchasePrice => _item.PurchasePrice;

        public int Quantity
        {
            get { return _quantity; }
            set { SetProperty(ref _quantity, value, () => Quantity); }
        }

        public decimal InventoryValue => _item.PurchasePrice * _quantity;

        public Supplier SelectedSupplier => _item.Suppliers.FirstOrDefault();
    }
}
