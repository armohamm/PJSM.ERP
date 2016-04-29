namespace PUJASM.ERP.ViewModels.Sales
{
    using Models.Inventory;
    using Models.Sales;
    using MVVMFramework;

    public class SalesReturnTransactionLineVM : ViewModelBase<SalesReturnTransactionLine>
    {
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

        public virtual SalesReturnTransaction SalesReturnTransaction
        {
            get { return Model.SalesReturnTransaction; }
            set
            {
                Model.SalesReturnTransaction = value;
                OnPropertyChanged("SalesReturnTransaction");
            }
        }

        public int Quantity
        {
            get { return Model.Quantity; }
            set
            {
                Model.Quantity = value;
                OnPropertyChanged("Quantity");
            }
        }

        public decimal SalesPrice
        {
            get { return Model.SalesPrice; }
            set
            {
                Model.SalesPrice = value;
                OnPropertyChanged("SalesPrice");
            }
        }

        public decimal ReturnPrice
        {
            get { return Model.ReturnPrice; }
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

        public decimal CostOfGoodsSold
        {
            get { return Model.CostOfGoodsSold; }
            set
            {
                Model.CostOfGoodsSold = value;
                OnPropertyChanged("CostOfGoodsSold");
            }
        }

        public void UpdateTotal()
        {
            OnPropertyChanged("Total");
        }
    }
}
