﻿namespace PUJASM.ERP.ViewModels.Customers.Sales_Return
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using Models.Sales;
    using MVVMFramework;
    using Sales;
    using Utilities;
    using Utilities.ModelHelpers;

    public class SalesReturnNewEntryVM : ViewModelBase
    {
        private readonly SalesReturnVM _parentVM;

        private string _salesReturnNewEntryProduct;
        private int _salesReturnNewEntryQuantity;
        private decimal _salesReturnNewEntryPrice;
        private ICommand _salesReturnNewEntryAddCommand;

        public SalesReturnNewEntryVM(SalesReturnVM parentVM)
        {
            _parentVM = parentVM;
        }

        #region Sales Return New Entry Properties

        public string SalesReturnNewEntryProduct
        {
            get { return _salesReturnNewEntryProduct; }
            set { SetProperty(ref _salesReturnNewEntryProduct, value, () => SalesReturnNewEntryProduct); }
        }

        public int SalesReturnNewEntryQuantity
        {
            get { return _salesReturnNewEntryQuantity; }
            set { SetProperty(ref _salesReturnNewEntryQuantity, value, () => SalesReturnNewEntryQuantity); }
        }

        public decimal SalesReturnNewEntryPrice
        {
            get { return _salesReturnNewEntryPrice; }
            set { SetProperty(ref _salesReturnNewEntryPrice, value, () => SalesReturnNewEntryPrice); }
        }
        #endregion

        public ICommand SalesReturnNewEntryAddCommand
        {
            get
            {
                return _salesReturnNewEntryAddCommand ?? (_salesReturnNewEntryAddCommand = new RelayCommand(() =>
                {
                    if (!IsThereSalesTransactionLineSelected() || !IsReturnNewEntryPriceValid() ||
                        !IsReturnNewEntryQuantityValid()) return;
                    AddEntryToDisplayedSalesReturnTransactionLines();
                    ResetEntryFields();
                }));
            }
        }

        #region Helper Methods
        private bool IsThereSalesTransactionLineSelected()
        {
            if (_salesReturnNewEntryProduct != null && _parentVM.SelectedSalesTransactionLine != null) return true;
            MessageBox.Show("No product is selected", "Invalid Command", MessageBoxButton.OK);
            return false;
        }

        private bool IsReturnNewEntryPriceValid()
        {
            var maximumReturnPrice = _parentVM.SelectedSalesTransactionLine.GetNetLinePrice();
            if (_salesReturnNewEntryPrice >= 0 && _salesReturnNewEntryPrice <= maximumReturnPrice) return true;
            MessageBox.Show($"The valid return price is 0 - {maximumReturnPrice}", "Invalid Command",
                MessageBoxButton.OK);
            return false;
        }

        private bool IsReturnNewEntryQuantityValid()
        {
            var availableReturnQuantity = GetAvailableReturnQuantity();
            if (_salesReturnNewEntryQuantity <= availableReturnQuantity && _salesReturnNewEntryQuantity > 0) return true;
            MessageBox.Show(
                $"The available return amount for {_parentVM.SelectedSalesTransactionLine.Item.Name} is {availableReturnQuantity}",
                "Invalid Return Quantity", MessageBoxButton.OK);
            return false;
        }

        private int GetAvailableReturnQuantity()
        {
            using (var context = new ERPContext())
            {
                var availableReturnQuantity = _parentVM.SelectedSalesTransactionLine.Quantity;

                var selectedSalesTransactionReturnedItems =
                    context.SalesReturnTransactionLines.Where(
                        line => line.SalesReturnTransaction.SalesTransaction.SalesTransactionID
                            .Equals(_parentVM.Model.SalesTransaction.SalesTransactionID) &&
                                line.ItemID.Equals(_parentVM.SelectedSalesTransactionLine.Item.ID))
                        .ToList();

                availableReturnQuantity = selectedSalesTransactionReturnedItems.Aggregate(availableReturnQuantity,
                    (current, item) => current - item.Quantity);

                return _parentVM.DisplayedSalesReturnTransactionLines.Where(
                    line => line.Item.ID.Equals(_parentVM.SelectedSalesTransactionLine.Item.ID)
                            && line.Warehouse.ID.Equals(_parentVM.SelectedSalesTransactionLine.Warehouse.ID)
                            && line.SalesPrice.Equals(_parentVM.SelectedSalesTransactionLine.SalesPrice)
                            && line.Discount.Equals(_parentVM.SelectedSalesTransactionLine.Discount))
                    .Aggregate(availableReturnQuantity, (current, l) => current - l.Quantity);
            }
        }

        private void AddEntryToDisplayedSalesReturnTransactionLines()
        {
            foreach (
                var salesReturnTransactionLine in
                    _parentVM.DisplayedSalesReturnTransactionLines.Where(IsSalesReturnLineAbleToCombineWithNewEntry))
            {
                CombineSalesReturnLineWithNewEntry(salesReturnTransactionLine);
                return;
            }

            var newEntrySalesReturnLineVM = MakeNewEntrySalesReturnTransactionLine();
            newEntrySalesReturnLineVM.CostOfGoodsSold =
                SalesReturnTransactionLineHelper.GetSalesReturnTransactionLineCOGS(newEntrySalesReturnLineVM.Model);
            _parentVM.DisplayedSalesReturnTransactionLines.Add(newEntrySalesReturnLineVM);
            _parentVM.SalesReturnTransactionNetTotal += newEntrySalesReturnLineVM.Total;
        }

        private bool IsSalesReturnLineAbleToCombineWithNewEntry(SalesReturnTransactionLineVM salesReturnTransactionLine)
        {
            return salesReturnTransactionLine.Item.ID.Equals(_parentVM.SelectedSalesTransactionLine.Item.ID) &&
                   salesReturnTransactionLine.Warehouse.ID.Equals(_parentVM.SelectedSalesTransactionLine.Warehouse.ID) &&
                   salesReturnTransactionLine.SalesPrice.Equals(_parentVM.SelectedSalesTransactionLine.SalesPrice) &&
                   salesReturnTransactionLine.Discount.Equals(_parentVM.SelectedSalesTransactionLine.Discount) &&
                   salesReturnTransactionLine.ReturnPrice.Equals(_salesReturnNewEntryPrice);
        }

        private void CombineSalesReturnLineWithNewEntry(SalesReturnTransactionLineVM salesReturnTransactionLine)
        {
            var salesReturnNewEntryQuantity = _salesReturnNewEntryQuantity;
            salesReturnTransactionLine.Quantity += salesReturnNewEntryQuantity;
            salesReturnTransactionLine.Total += salesReturnNewEntryQuantity*_salesReturnNewEntryPrice;
            salesReturnTransactionLine.CostOfGoodsSold =
                SalesReturnTransactionLineHelper.GetSalesReturnTransactionLineCOGS(salesReturnTransactionLine.Model);
            _parentVM.SalesReturnTransactionNetTotal += salesReturnNewEntryQuantity*
                                                        salesReturnTransactionLine.Model.SalesPrice;
        }

        private SalesReturnTransactionLineVM MakeNewEntrySalesReturnTransactionLine()
        {
            var salesReturnNewEntryQuantity = _salesReturnNewEntryQuantity;
            var salesReturnTransactionLine = new SalesReturnTransactionLine
            {
                SalesReturnTransaction = _parentVM.Model,
                Item = _parentVM.SelectedSalesTransactionLine.Item,
                Warehouse = _parentVM.SelectedSalesTransactionLine.Warehouse,
                SalesPrice = _parentVM.SelectedSalesTransactionLine.SalesPrice,
                Discount = _parentVM.SelectedSalesTransactionLine.Discount,
                ReturnPrice = _salesReturnNewEntryPrice,
                Quantity = salesReturnNewEntryQuantity,
                Total = salesReturnNewEntryQuantity*_salesReturnNewEntryPrice
            };
            return new SalesReturnTransactionLineVM {Model = salesReturnTransactionLine};
        }

        private void ResetEntryFields()
        {
            _parentVM.SelectedSalesTransactionLine = null;
            SalesReturnNewEntryProduct = null;
            SalesReturnNewEntryQuantity = 0;
            SalesReturnNewEntryPrice = 0;
        }

        #endregion
    }
}
