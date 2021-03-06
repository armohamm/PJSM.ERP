﻿using System.Windows.Controls;
using System.Windows.Input;

namespace PUJASM.ERP
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private void ComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            var MyComboBox = sender as ComboBox;
            if (MyComboBox == null || MyComboBox.Text.Length <= 0) return;
            if (e.Key != Key.Enter) return;
            var binding = MyComboBox.GetBindingExpression(ComboBox.SelectedItemProperty);
            binding?.UpdateSource();
        }
    }
}
