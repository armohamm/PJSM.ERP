namespace PUJASM.ERP.ViewModels.Master.Customers
{
    using System.Windows;
    using System.Windows.Input;
    using MVVMFramework;
    using Models;
    using Utilities;
    using Utilities.ModelHelpers;
    using ViewModels;

    public class MasterCustomersEditVM : ViewModelBase
    {
        private readonly CustomerVM _editingCustomer;

        #region Backing Fields
        private string _editID;
        private string _editFirstName;
        private string _editLastName;
        private string _editAddress;
        private string _editTelephone;
        private string _editEmail;
        private ICommand _editConfirmCommand;
        #endregion

        public MasterCustomersEditVM(CustomerVM editingCustomer)
        {
            _editingCustomer = editingCustomer;
            SetDefaultEditProperties();
        }
         
        #region Properties
        public string EditID
        {
            get { return _editID; }
            set { SetProperty(ref _editID, value, () => EditID); }
        }

        public string EditFirstName
        {
            get { return _editFirstName; }
            set { SetProperty(ref _editFirstName, value, () => EditFirstName); }
        }

        public string EditLastName
        {
            get { return _editLastName; }
            set { SetProperty(ref _editLastName, value, () => EditLastName); }
        }

        public string EditAddress
        {
            get { return _editAddress; }
            set { SetProperty(ref _editAddress, value, () => EditAddress); }
        }

        public string EditTelephone
        {
            get { return _editTelephone; }
            set { SetProperty(ref _editTelephone, value, () => EditTelephone); }
        }

        public string EditEmail
        {
            get { return _editEmail; }
            set { SetProperty(ref _editEmail, value, () => EditEmail); }
        }
        #endregion

        public ICommand EditConfirmCommand
        {
            get
            {
                return _editConfirmCommand ?? (_editConfirmCommand = new RelayCommand(() =>
                {
                    if (!IsEditConfirmationYes() && !AreEditFieldsValid()) return;
                    var editingCustomer = _editingCustomer.Model;
                    var editedCustomerCopy = MakeEditedCustomer();
                    CustomerHelper.SaveCustomerEditsToDatabase(editingCustomer, editedCustomerCopy);
                    UpdateEditingCustomerUIValues();
                    UtilityMethods.CloseForemostWindow();
                }));
            }
        }

        #region Helper Methods
        private void SetDefaultEditProperties()
        {
            EditID = _editingCustomer.ID;
            EditFirstName = _editingCustomer.FirstName;
            EditLastName = _editingCustomer.LastName;
            EditAddress = _editingCustomer.Address;
            EditTelephone = _editingCustomer.Telephone;
            EditEmail = _editingCustomer.Email;
        }

        private Customer MakeEditedCustomer()
        {
            return new Customer
            {
                ID = _editID,
                FirstName = _editFirstName,
                LastName =  _editLastName,
                Address = _editAddress,
                Telephone = _editTelephone,
                Email = _editEmail,
                Points = _editingCustomer.Points,
                Expiry =  _editingCustomer.Expiry,
                IsActive = _editingCustomer.IsActive
            };
        }
       
        private static bool IsEditConfirmationYes()
        {
            return MessageBox.Show("Confirm edit?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes;
        }

        private bool AreEditFieldsValid()
        {
            return _editFirstName != null && _editLastName != null && 
                _editAddress != null && _editTelephone != null && 
                _editEmail != null;
        }

        private void UpdateEditingCustomerUIValues()
        {
            var editedCustomer = MakeEditedCustomer();
            var customerTo = _editingCustomer.Model;
            CustomerHelper.DeepCopyCustomerProperties(editedCustomer, ref customerTo);
            _editingCustomer.UpdatePropertiesToUI();
        }
        #endregion
    }
}
