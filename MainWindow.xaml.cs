using FirstFloor.ModernUI.Presentation;
using System.Windows.Media;

namespace PUJASM.ERP
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.Threading;
    using FirstFloor.ModernUI;
    using Utilities;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly string _connectionString;
        public MainWindow()
        {
            ModernUIHelper.TrySetPerMonitorDpiAware();
            AppearanceManager.Current.AccentColor = Colors.Red;
            InitializeComponent();

            _connectionString =
                ConfigurationManager.ConnectionStrings["ERPContext"].ConnectionString.Substring(7).Split(';')[0];

            RunUpdateTitleLoop();
        }

        private void RunUpdateTitleLoop()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerAsync();
        }


        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                Thread.Sleep(1000);
                Dispatcher.Invoke(() =>
                {
                    try
                    {
                        SetTitle();
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                });
            }
        }

        private void SetTitle()
        {
            Title = "Puja Supermarket - User: " + ", Server: " + _connectionString + ", Date: " + UtilityMethods.GetCurrentDate().ToString("dd-MM-yyyy");
        }
    }
}
