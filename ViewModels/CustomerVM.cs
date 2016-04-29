using MVVMFramework;
using PUJASM.ERP.Models.Sales;
using System;
using System.Collections.Generic;

namespace PUJASM.ERP.ViewModels
{
    using Models;

#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    public class CustomerVM : ViewModelBase<Customer>
    #pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    {
        public string ID
        {
            get { return Model.ID; }
            set
            {
                Model.ID = value;
                OnPropertyChanged("ID");
            }
        }

        public string Name
        {
            get { return Model.FirstName + " " + Model.LastName; }
        }

        public string FirstName
        {
            get { return Model.FirstName; }
            set
            {
                Model.FirstName = value;
                OnPropertyChanged("FirstName");
            }
        }

        public string LastName
        {
            get { return Model.LastName; }
            set
            {
                Model.LastName = value;
                OnPropertyChanged("LastName");
            }
        }

        public string Address
        {
            get { return Model.Address; }
            set
            {
                Model.Address = value;
                OnPropertyChanged("Address");
            }
        }

        public string Telephone
        {
            get { return Model.Telephone; }
            set
            {
                Model.Telephone = value;
                OnPropertyChanged("Telephone");
            }
        }

        public string Email
        {
            get { return Model.Email; }
            set
            {
                Model.Email = value;
                OnPropertyChanged("Email");
            }
        }

        public int Points
        {
            get { return Model.Points; }
            set
            {
                Model.Points = value;
                OnPropertyChanged("Points");
            }
        }

        public DateTime Expiry
        {
            get { return Model.Expiry; }
            set
            {
                Model.Expiry = value;
                OnPropertyChanged("Expiry");
            }
        }

        public List<SalesTransaction> SalesTransactions
        {
            get { return Model.SalesTransactions; }
        }

        public bool IsActive
        {
            get { return Model.IsActive; }
            set
            {
                Model.IsActive = value;
                OnPropertyChanged("IsActive");
            }
        }

        public void UpdatePropertiesToUI()
        {
            OnPropertyChanged("Name");
            OnPropertyChanged("FirstName");
            OnPropertyChanged("LastName");
            OnPropertyChanged("Address");
            OnPropertyChanged("Telephone");
            OnPropertyChanged("Email");
        }

        #pragma warning disable 659
        public override bool Equals(object obj)
        #pragma warning restore 659
        {
            if (obj == null) return false;
            var customer = obj as CustomerVM;
            return customer != null && ID.Equals(customer.ID);
        }
    }
}
