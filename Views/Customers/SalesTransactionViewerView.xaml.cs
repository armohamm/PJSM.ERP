using System.Windows.Controls;

namespace PUJASM.ERP.Views.Customers
{
    using System.Windows.Input;
    using ViewModels.Customers;

    /// <summary>
    /// Interaction logic for SalesTransactionViewerView.xaml
    /// </summary>
    public partial class SalesTransactionViewerView
    {
        public SalesTransactionViewerView()
        {
            InitializeComponent();
            var vm = new SalesTransactionViewerVM();
            DataContext = vm;
        }

        private void TextBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var MyTextBox = sender as TextBox;
            if (MyTextBox == null || MyTextBox.Text.Length <= 0) return;
            if (e.Key != Key.Enter) return;
            var binding = MyTextBox.GetBindingExpression(TextBox.TextProperty);
            binding?.UpdateSource();
        }
    }
}
