namespace PUJASM.ERP.Views.Master
{
    using ViewModels.Master;

    /// <summary>
    /// Interaction logic for LedgerView.xaml
    /// </summary>
    public partial class MasterLedgersView
    {
        public MasterLedgersView()
        {
            InitializeComponent();
            var vm = new MasterLedgerVM();
            DataContext = vm;
        }
    }
}
