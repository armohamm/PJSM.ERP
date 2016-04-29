namespace PUJASM.ERP.ViewModels.Sales
{
    using System;
    using Models;
    using Models.Sales;
    using MVVMFramework;

    #pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    public class SalesTransactionVM : ViewModelBase<SalesTransaction>
    {
        public string SalesTransactionID => Model.SalesTransactionID;

        public Customer Customer => Model.Customer;

        public decimal Total => Model.Total;

        public DateTime Date => Model.Date;

        public string Note => Model.Notes;

        public User User => Model.User;

        public override bool Equals(object obj)
        {
            var line = obj as SalesTransactionVM;
            return line != null && SalesTransactionID.Equals(line.SalesTransactionID);
        }
    }
}
