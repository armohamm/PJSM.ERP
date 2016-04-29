namespace PUJASM.ERP.ViewModels
{
    using Models;
    using MVVMFramework;

    public class SupplierVM : ViewModelBase<Supplier>
    {
        public int ID => Model.ID;

        public string Name
        {
            get { return Model.Name; }
            set
            {
                Model.Name = value;
                OnPropertyChanged("Name");
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

        public bool IsActive
        {
            get { return Model.IsActive; }
            set
            {
                Model.IsActive = value;
                OnPropertyChanged("IsActive");
            }
        }

        public int GSTID
        {
            get { return Model.GSTID; }
            set
            {
                Model.GSTID = value;
                OnPropertyChanged("GSTID");
            }
        }

        public void UpdatePropertiesToUI()
        {
            OnPropertyChanged("Name");
            OnPropertyChanged("Address");
            OnPropertyChanged("IsActive");
            OnPropertyChanged("GSTID");
        }
    }
}
