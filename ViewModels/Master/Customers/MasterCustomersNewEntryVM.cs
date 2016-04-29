namespace PUJASM.ERP.ViewModels.Master.Customers
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using Models;
    using MVVMFramework;
    using Utilities;
    using Utilities.ModelHelpers;

    public class MasterCustomersNewEntryVM : ViewModelBase
    {
        private readonly MasterCustomersVM _parentVM;

        #region Backing Fields
        private string _newEntryID;
        private string _newEntryFirstName;
        private string _newEntryLastName;
        private string _newEntryAddress;
        private string _newEntryTelephone;
        private string _newEntryEmail;
        private ICommand _newEntryCommand;
        private ICommand _cancelEntryCommand;
        #endregion

        public MasterCustomersNewEntryVM(MasterCustomersVM parentVM)
        {
            _parentVM = parentVM;
            SetNewEntryID();
        }

        #region Properties
        public string NewEntryID
        {
            get { return _newEntryID; }
            set { SetProperty(ref _newEntryID, value, () => NewEntryID); }
        }

        public string NewEntryFirstName
        {
            get { return _newEntryFirstName; }
            set { SetProperty(ref _newEntryFirstName, value, () => NewEntryFirstName); }
        }

        public string NewEntryLastName
        {
            get { return _newEntryLastName; }
            set { SetProperty(ref _newEntryLastName, value, () => NewEntryLastName); }
        }

        public string NewEntryAddress
        {
            get { return _newEntryAddress; }
            set { SetProperty(ref _newEntryAddress, value, () => NewEntryAddress); }
        }

        public string NewEntryTelephone
        {
            get { return _newEntryTelephone; }
            set { SetProperty(ref _newEntryTelephone, value, () => NewEntryTelephone); }
        }

        public string NewEntryEmail
        {
            get { return _newEntryEmail; }
            set { SetProperty(ref _newEntryEmail, value, () => NewEntryEmail); }
        }
        #endregion

        #region Commands
        public ICommand NewEntryCommand
        {
            get
            {
                return _newEntryCommand ?? (_newEntryCommand = new RelayCommand(() =>
                {
                    if (!IsNewEntryCommandChecksSuccessful() || !IsConfirmationYes()) return;
                    var newCustomer = MakeNewEntryCustomer();
                    CustomerHelper.AddCustomerAlongWithItsLedgerToDatabase(newCustomer);
                    ResetEntryFields();
                    _parentVM.UpdateCustomers();
                    _parentVM.UpdateDisplayedCustomers();
                }));
            }
        }

        public ICommand CancelEntryCommand => _cancelEntryCommand ?? (_cancelEntryCommand = new RelayCommand(ResetEntryFields));
        #endregion

        #region Helper Methods
        private void SetNewEntryID()
        {
            using (var context = new ERPContext())
            {
                var lastEntryCustomer = context.Customers.OrderByDescending(customer => customer.ID).FirstOrDefault();
                if (lastEntryCustomer == null) NewEntryID = "00000000";
                else
                {
                    var lastEntryID = int.Parse(lastEntryCustomer.ID);
                    NewEntryID = (lastEntryID + 1).ToString().PadLeft(8, '0');
                }
            }
        }

        private bool IsNewEntryCommandChecksSuccessful()
        {
            if (_newEntryFirstName != null && _newEntryLastName != null &&
                _newEntryAddress != null && _newEntryTelephone != null &&
                _newEntryEmail != null) return true;
            MessageBox.Show("Please enter all fields.", "Missing Field(s)", MessageBoxButton.OK);
            return false;
        }

        private static bool IsConfirmationYes()
        {
            return MessageBox.Show("Confirm adding customer?", "Confirmation", MessageBoxButton.YesNo) ==
                   MessageBoxResult.Yes;
        }

        private Customer MakeNewEntryCustomer()
        {
            SetNewEntryID(); // To prevent duplication
            return new Customer
            {
                ID = _newEntryID,
                FirstName = _newEntryFirstName,
                LastName = _newEntryLastName,
                Address = _newEntryAddress,
                Telephone = _newEntryTelephone,
                Email = _newEntryEmail,
                Expiry = UtilityMethods.GetCurrentDate().AddYears(1),
                IsActive = true
            };
        }

        private void ResetEntryFields()
        {
            SetNewEntryID();
            NewEntryFirstName = null;
            NewEntryLastName = null;
            NewEntryAddress = null;
            NewEntryTelephone = null;
            NewEntryEmail = null;
        }
        #endregion
    }
}
