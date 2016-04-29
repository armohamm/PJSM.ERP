namespace PUJASM.ERP.ViewModels.Master
{
    using System.Collections.ObjectModel;
    using System.Data.Entity;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using Ledger;
    using Models.Accounting;
    using MVVMFramework;
    using Utilities;

    internal class MasterLedgerVM : ViewModelBase
    {
        private string _selectedClass;

        private string _newEntryName;
        private string _newEntryGroup;
        private ICommand _newEntryCommand;

        public MasterLedgerVM()
        {
            DisplayAccounts = new ObservableCollection<LedgerAccountVM>();
            Classes = new ObservableCollection<string>
            {
                "All", "Asset", "Liability", "Equity", "Expense", "Revenue"
            };
            Groups = new ObservableCollection<string>
            {
                "Bank", "Operating Expense", "Accounts Receivable", "Accounts Payable"
            };

            SelectedClass = Classes.FirstOrDefault();
        }

        #region Collections
        public ObservableCollection<LedgerAccountVM> DisplayAccounts { get; }

        public ObservableCollection<string> Classes { get; }

        public ObservableCollection<string> Groups { get; }
        #endregion

        public string SelectedClass
        {
            get { return _selectedClass; }
            set
            {
                DisplayAccounts.Clear();
                SetProperty(ref _selectedClass, value, "SelectedClass");

                if (value == "All")
                {
                    using (var context = new ERPContext())
                    {
                        var accounts = context.Ledger_Accounts
                            .Include("LedgerGeneral")
                            .Where(e => !e.Name.Equals("- Accounts Payable"))
                            .OrderBy(e => e.Class)
                            .ThenBy(e => e.Notes)
                            .ThenBy(e => e.Name);

                        foreach (var account in accounts)
                            DisplayAccounts.Add(new LedgerAccountVM { Model = account });
                    }
                }

                else
                {
                    DisplayAccounts.Clear();
                    using (var context = new ERPContext())
                    {
                        var accounts = context.Ledger_Accounts
                            .Where(e => e.Class == value && !e.Name.Equals("- Accounts Payable"))
                            .Include("LedgerGeneral")
                            .OrderBy(e => e.Class)
                            .ThenBy(e => e.Notes)
                            .ThenBy(e => e.Name);
                        foreach (var account in accounts)
                            DisplayAccounts.Add(new LedgerAccountVM { Model = account });
                    }
                }
            }
        }

        #region New Entry Properties
        public string NewEntryName
        {
            get { return _newEntryName; }
            set { SetProperty(ref _newEntryName, value, () => NewEntryName); }
        }

        public string NewEntryGroup
        {
            get { return _newEntryGroup; }
            set { SetProperty(ref _newEntryGroup, value, () => NewEntryGroup); }
        }

        public ICommand NewEntryCommand
        {
            get
            {
                return _newEntryCommand ?? (_newEntryCommand = new RelayCommand(() =>
                {
                    if (MessageBox.Show("Confirm adding this account?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.No) return;

                    using (var context = new ERPContext())
                    {
                        CreateNewAccount(context);
                        context.SaveChanges();
                    }

                    MessageBox.Show("Successfully added account!", "Success", MessageBoxButton.OK);
                    ResetEntryFields();
                }));
            }
        }
        #endregion

        private void CreateNewAccount(ERPContext context)
        {
            LedgerAccount newAccount;
                
            if (_newEntryGroup.Equals("Bank"))
            {
                newAccount = new LedgerAccount
                {
                    Name = _newEntryName,
                    Class = "Asset",
                    Notes = "Current Asset",
                    LedgerAccountBalances = new ObservableCollection<LedgerAccountBalance>()
                };
            }

            else if (_newEntryGroup.Equals("Operating Expense"))
            {
                newAccount = new LedgerAccount
                {
                    Name = _newEntryName,
                    Class = "Expense",
                    Notes = "Operating Expense",
                    LedgerAccountBalances = new ObservableCollection<LedgerAccountBalance>()
                };
            }

            else if (_newEntryGroup.Equals("Accounts Receivable"))
            {
                newAccount = new LedgerAccount
                {
                    Name = _newEntryName,
                    Class = "Asset",
                    Notes = "Accounts Receivable",
                    LedgerAccountBalances = new ObservableCollection<LedgerAccountBalance>()
                };
            }

            else if (_newEntryGroup.Equals("Accounts Payable"))
            {
                newAccount = new LedgerAccount
                {
                    Name = _newEntryName,
                    Class = "Liability",
                    Notes = "Accounts Payable",
                    LedgerAccountBalances = new ObservableCollection<LedgerAccountBalance>()
                };
            }

            else
            {
                newAccount = new LedgerAccount
                {
                    Name = _newEntryName,
                    Class = "Expense",
                    Notes = "Operating Expense",
                    LedgerAccountBalances = new ObservableCollection<LedgerAccountBalance>()
                };
            }

            var newAccountGeneralLedger = new LedgerGeneral
            {
                LedgerAccount = newAccount,
                PeriodYear = context.Ledger_General.First().PeriodYear,
                Period = context.Ledger_General.First().Period
            };

            var newAccountBalances = new LedgerAccountBalance
            {
                LedgerAccount = newAccount,
                PeriodYear = context.Ledger_General.First().PeriodYear
            };

            newAccount.LedgerGeneral = newAccountGeneralLedger;
            newAccount.LedgerAccountBalances.Add(newAccountBalances);
            context.Ledger_Accounts.Add(newAccount);
        }

        private void ResetEntryFields()
        {
            NewEntryName = null;
            NewEntryGroup = null;
        }
    }
}
