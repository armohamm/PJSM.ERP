namespace PUJASM.ERP.ViewModels.Inventory
{
    using MVVMFramework;
    using Models.Inventory;
    using Models.StockCorrection;

    internal class StockAdjustmentTransactionLineVM : ViewModelBase<StockAdjustmentTransactionLine>
    {
        public StockAdjustmentTransaction StockAdjustmentTransaction => Model.StockAdjustmentTransaction;

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

        public int Quantity
        {
            get { return Model.Quantity; }
            set
            {
                Model.Quantity = value;
                OnPropertyChanged("Quantity");
            }
        }
    }
}
