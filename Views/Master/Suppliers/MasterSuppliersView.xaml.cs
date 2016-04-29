namespace PUJASM.ERP.Views.Master.Suppliers
{
    using ViewModels.Master.Suppliers;

    /// <summary>
    /// Interaction logic for MasterSuppliersView.xaml
    /// </summary>
    public partial class MasterSuppliersView
    {
        public MasterSuppliersView()
        {
            InitializeComponent();
            var vm = new MasterSuppliersVM();
            DataContext = vm;
        }
    }
}
