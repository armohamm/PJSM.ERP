using Microsoft.Win32;
using MySql.Data.MySqlClient;
using System.ComponentModel;
using System.Configuration;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace PUJASM.ERP.Views.Menu
{
    using System.Linq;
    using Utilities;

    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        bool isRunning = false;
        bool isDone = false;
        string filename = null;

        public SettingsView()
        {
            InitializeComponent();
        }

        private void Export()
        {
            string constring = ConfigurationManager.ConnectionStrings["ERPContext"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(constring))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    using (MySqlBackup mb = new MySqlBackup(cmd))
                    {
                        cmd.Connection = conn;
                        conn.Open();
                        mb.ExportToFile(filename);
                        mb.ExportInfo.RecordDumpTime = true;
                        conn.Close();
                    }
                    MessageBox.Show("Backup is successful", "Success", MessageBoxButton.OK);
                }
                isDone = true;
            }
        }

        private void Browse(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            var dlg = new SaveFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".sql";
            dlg.Filter = "sql Files (*.sql)|*.sql";

            // Display OpenFileDialog by calling ShowDialog method 
            bool? result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                filename = dlg.FileName;
                backupPath.Text = filename;
            }
        }

        private void Backup(object sender, RoutedEventArgs e)
        {
            if (filename == null)
            {
                MessageBox.Show("Please select a path to backup.", "Empty Backup Path", MessageBoxButton.OK);
                return;
            }

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;

            if (!isRunning)
            {
                isRunning = true;
                isDone = false;
                pbStatus.IsIndeterminate = true;
                worker.RunWorkerAsync();
                pbStatus.IsIndeterminate = false;
            }
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Export();
            for (int i = 0; i <= 50; i++)
            {
                if (isDone)
                {
                    (sender as BackgroundWorker).ReportProgress(100);
                    isRunning = false;
                    return;
                }
                (sender as BackgroundWorker).ReportProgress(i);
                Thread.Sleep(100);
            }
            // Wait for backup to be done if it isn't yet
            while (!isDone) { }
            isRunning = false;
        }

        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbStatus.Value = e.ProgressPercentage * 2;
        }

        private void Decrease_Day_Button_Clicked(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(
                $"The date now is: {UtilityMethods.GetCurrentDate().ToString("dd-MM-yyyy")} \n Confirm decreasing day?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;

            //if (!UtilityMethods.GetVerification()) return;

            using (var context = new ERPContext())
            {
                var currentDate = context.Dates.FirstOrDefault(x => x.Name.Equals("Current"));
                if (currentDate != null) currentDate.DateTime = currentDate.DateTime.AddDays(-1);
                context.SaveChanges();
            }
        }

        private void Increase_Day_Button_Clicked(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(
                $"The date now is: {UtilityMethods.GetCurrentDate().ToString("dd-MM-yyyy")} \n Confirm increasing day?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;

            using (var context = new ERPContext())
            {
                var currentDate = context.Dates.FirstOrDefault(x => x.Name.Equals("Current"));
                if (currentDate != null)
                {
                    currentDate.DateTime = currentDate.DateTime.AddDays(1);
                }
                context.SaveChanges();
            }
        }
    }
}
