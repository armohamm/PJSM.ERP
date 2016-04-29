namespace PUJASM.ERP.Views.Accounting
{
    using ViewModels.Accounting;

    /// <summary>
    /// Interaction logic for BankTransactionView.xaml
    /// </summary>
    public partial class CashBankTransactionView
    {
        public CashBankTransactionView()
        {
            InitializeComponent();
            var vm = new CashBankTransactionVM();
            DataContext = vm;
        }
    }
}
