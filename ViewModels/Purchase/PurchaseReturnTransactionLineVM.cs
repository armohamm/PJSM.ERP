namespace PUJASM.ERP.ViewModels.Purchase
{
    using Models.Inventory;
    using Models.Purchase;
    using MVVMFramework;

    internal class PurchaseReturnTransactionLineVM : ViewModelBase<PurchaseReturnTransactionLine>
    {
        public PurchaseReturnTransaction PurchaseReturnTransaction
        {
            get { return Model.PurchaseReturnTransaction; }
            set
            {
                Model.PurchaseReturnTransaction = value;
                OnPropertyChanged("PurchaseReturnTransaction");
            }
        }

        public Item Item
        {
            get { return Model.Item; }
            set
            {
                Model.Item = value;
                OnPropertyChanged("Item");
            }
        }

        public Warehouse Warehouse
        {
            get { return Model.Warehouse; }
            set
            {
                Model.Warehouse = value;
                OnPropertyChanged("Warehouse");
            }
        }

        public Warehouse ReturnWarehouse
        {
            get { return Model.ReturnWarehouse; }
            set
            {
                Model.ReturnWarehouse = value;
                OnPropertyChanged("ReturnWarehouse");
            }
        }

        public int Quantity
        {
            get { return Model.Quantity; }
            set
            {
                Model.Quantity = value;
                OnPropertyChanged("Quantity");
                OnPropertyChanged("Pieces");
                OnPropertyChanged("Units");
            }
        }

        public int Pieces => Model.Quantity;

        public int Units => Model.Quantity;

        public decimal PurchasePrice
        {
            get
            {
                return Model.PurchasePrice;
            }
            set
            {
                Model.PurchasePrice = value;
                OnPropertyChanged("PurchasePrice");
            }
        }

        public decimal ReturnPrice
        {
            get
            {
                return Model.ReturnPrice;
            }
            set
            {
                Model.ReturnPrice = value;
                OnPropertyChanged("ReturnPrice");
            }
        }

        public decimal Discount
        {
            get { return Model.Discount; }
            set
            {
                Model.Discount = value;
                OnPropertyChanged("Discount");
            }
        }

        public decimal Total
        {
            get { return Model.Total; }
            set
            {
                Model.Total = value;
                OnPropertyChanged("Total");
            }
        }

        public void UpdateTotal()
        {
            OnPropertyChanged("Total");
        }
    }
}

