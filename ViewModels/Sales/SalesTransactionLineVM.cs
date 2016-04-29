namespace PUJASM.ERP.ViewModels.Sales
{
    using Models.Inventory;
    using Models.Sales;
    using MVVMFramework;

    #pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    public class SalesTransactionLineVM : ViewModelBase<SalesTransactionLine>
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

        public SalesTransaction SalesTransaction
        {
            get { return Model.SalesTransaction; }
            set
            {
                Model.SalesTransaction = value;
                OnPropertyChanged("SalesTransaction");
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
                UpdateTotal();
                OnPropertyChanged("Quantity");
            }
        }

        public decimal SalesPrice
        {
            get { return Model.SalesPrice; }
            set
            {
                Model.SalesPrice = value;
                Total = (Model.SalesPrice - Model.Discount) * Model.Quantity;
                OnPropertyChanged("SalesPrice");
            }
        }

        public decimal Discount
        {
            get { return Model.Discount; }
            set
            {
                Model.Discount = value <= 0 ? 0 : value;
                Total = (Model.SalesPrice - Model.Discount) * Model.Quantity;
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

        public decimal NetTotal => GetNetLinePrice() * Model.Quantity;

        public override bool Equals(object obj)
        {
            var line = obj as SalesTransactionLineVM;
            if (line == null) return false;
            return Item.ID.Equals(line.Item.ID) &&
                   Warehouse.ID.Equals(line.Warehouse.ID) &&
                   SalesPrice.Equals(line.SalesPrice) &&
                   Discount.Equals(line.Discount);
        }

        #region Helper Methods
        public SalesTransactionLineVM Clone()
        {
            var cloneLine = new SalesTransactionLine
            {
                SalesTransaction = Model.SalesTransaction,
                Item = Model.Item,
                Warehouse = Model.Warehouse,
                SalesPrice = Model.SalesPrice,
                Discount = Model.Discount,
                Quantity = Model.Quantity,
                Total = Model.Total
            };
            return new SalesTransactionLineVM { Model = cloneLine };
        }

        public decimal GetNetLinePrice()
        {
            var lineDiscount = Model.Discount;
            var lineSalesPrice = Model.SalesPrice;
            if (lineSalesPrice - lineDiscount == 0) return 0;
            var fractionOfTransaction = Model.Quantity * (lineSalesPrice - lineDiscount) / Model.SalesTransaction.GrossTotal;
            var fractionOfTransactionDiscount = fractionOfTransaction * Model.SalesTransaction.Discount / Model.Quantity;
            var netLinePrice = lineSalesPrice - lineDiscount - fractionOfTransactionDiscount;
            return netLinePrice;
        }

        public void UpdateTotal()
        {
            Total = (Model.SalesPrice - Model.Discount) * Model.Quantity;
        }
        #endregion
    }
}
