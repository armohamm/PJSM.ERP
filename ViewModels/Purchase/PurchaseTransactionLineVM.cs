namespace PUJASM.ERP.ViewModels.Purchase
{
    using System.Linq;
    using Models.Inventory;
    using Models.Purchase;
    using MVVMFramework;
    using Utilities;

    public class PurchaseTransactionLineVM : ViewModelBase<PurchaseTransactionLine>
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

        public PurchaseTransaction PurchaseTransaction
        {
            get { return Model.PurchaseTransaction; }
            set
            {
                Model.PurchaseTransaction = value;
                OnPropertyChanged("PurchaseTransaction");
            }
        }

        public string PurchaseTransactionID => Model.PurchaseTransaction.PurchaseID;

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

        public decimal Discount
        {
            get { return Model.Discount; }
            set
            {
                Model.Discount = value;
                OnPropertyChanged("Discount");
                UpdateTotal();
            }
        }

        public decimal PurchasePrice
        {
            get { return Model.PurchasePrice; }
            set
            {
                Model.PurchasePrice = value;
                OnPropertyChanged("PurchasePrice");
                UpdateTotal();
            }
        }

        public decimal Total
        {
            get
            {
                return Model.Total;
            }
            set
            {
                Model.Total = value;
                OnPropertyChanged("Total");
            }
        }

        public int SoldOrReturned
        {
            get { return Model.SoldOrReturned; }
            set
            {
                Model.SoldOrReturned = value;
                OnPropertyChanged("SoldOrReturned");
            }
        }
            
        public int GetStock()
        {
            using (var context = new ERPContext())
            {
                var itemStock = context.Stocks.FirstOrDefault(stock => stock.ItemID.Equals(Model.Item.ID) && stock.WarehouseID.Equals(Model.Warehouse.ID));
                return itemStock?.Pieces ?? 0;
            }
        }
       
        public decimal GetNetDiscount()
        {
            var lineDiscount = Model.Discount;
            var lineSalesPrice = Model.PurchasePrice;
            if (lineSalesPrice - lineDiscount == 0) return 0;
            var fractionOfTransaction = Model.Quantity * (lineSalesPrice - lineDiscount) / Model.PurchaseTransaction.GrossTotal;
            var fractionOfTransactionDiscount = fractionOfTransaction * Model.PurchaseTransaction.Discount / Model.Quantity;
            var discount = lineDiscount + fractionOfTransactionDiscount;
            return discount;
        }

        public PurchaseTransactionLineVM Clone()
        {
            var newLine = new PurchaseTransactionLine
            {
                Item = Model.Item,
                Warehouse = Model.Warehouse,
                PurchasePrice = Model.PurchasePrice,
                Discount = Model.Discount,
                SoldOrReturned = Model.SoldOrReturned,
                Total = Model.Total
            };

            return new PurchaseTransactionLineVM { Model = newLine };
        }

        public void UpdateTotal()
        {
            Total = (Model.PurchasePrice - Model.Discount) * Model.Quantity;
        }
    }
}
