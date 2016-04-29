namespace PUJASM.ERP.Utilities
{
    using System;
    using System.Windows;
    using System.Linq;
    using Models;
    using Models.Inventory;
    using Views;

    public static class UtilityMethods
    {
        public static void CloseForemostWindow()
        {
            var editWindow = Application.Current.Windows[Application.Current.Windows.Count - 1];
            editWindow?.Close();
        }

        public static DateTime GetCurrentDate()
        {
            DateTime currentDate;
            using (var context = new ERPContext())
            {
                var date = context.Dates.SingleOrDefault(e => e.Name.Equals("Current"));
                if (date != null)
                    currentDate = date.DateTime;
                else
                {
                    currentDate = DateTime.Now.Date;
                    context.Dates.Add(new Date {DateTime = DateTime.Now.Date, Name = "Current"});
                    context.SaveChanges();
                }
            }
            return currentDate;
        }

        public static bool GetVerification()
        {
            Application.Current.MainWindow.IsEnabled = false;
            var window = new VerificationWindow();
            window.ShowDialog();
            Application.Current.MainWindow.IsEnabled = true;
            var isVerified = Application.Current.TryFindResource("IsVerified");
            return isVerified != null;
        }

        public static int GetRemainingStock(Item item, Warehouse warehouse)
        {
            using (var context = new ERPContext())
            {
                var stock = context.Stocks.SingleOrDefault(e => e.ItemID.Equals(item.ID) && e.WarehouseID.Equals(warehouse.ID));
                return stock?.Pieces ?? 0;
            }
        }
    }
}
