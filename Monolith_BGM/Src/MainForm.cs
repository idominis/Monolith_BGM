using BGM.SftpUtilities;
//using Monolith_BGM.DataAccess;
using Monolith_BGM.Models;
using System;
using System.Windows.Forms;

namespace Monolith_BGM
{
    public partial class MainForm : Form
    {
        private System.Timers.Timer timer;
        private SftpClientManager clientManager;
        private SftpFileHandler fileHandler;

        public MainForm()
        {
            InitializeComponent();
            InitializeSftp();
            InitializeTimer();
        }

        private void InitializeSftp()
        {
            string host = "192.168.0.140";
            string username = "tester";
            string password = "password";

            clientManager = new SftpClientManager(host, username, password);
            fileHandler = new SftpFileHandler(clientManager);
        }

        private void InitializeTimer()
        {
            timer = new System.Timers.Timer(10000); // Interval in milliseconds
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;

            // Call OnTimedEvent immediately
            OnTimedEvent(timer, null);
        }

        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            string remoteBaseDirectoryPath = @"\PurchasingOrders";
            string localBaseDirectoryPath = @"C:\Users\Ivan\Documents\BGM_project\RebexTinySftpServer-Binaries-Latest\data_received";

            try
            {
                // Download XML files from the directory
                bool newFilesDownloaded = fileHandler.DownloadXmlFilesFromDirectory(remoteBaseDirectoryPath, localBaseDirectoryPath);
                
                if (newFilesDownloaded)
                {
                    MessageBox.Show("XML files have been downloaded successfully!");
                }
                else
                {
                    MessageBox.Show("No new XML files.");
                }

                // Rest of the code...
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to download XML files: " + ex.Message);
            }

        }

        private void SaveToDbButton_Click(object sender, EventArgs e)
        {
            var xmlLoader = new XmlDataLoader();
            string baseDirectoryPath = @"C:\Users\Ivan\Documents\BGM_project\RebexTinySftpServer-Binaries-Latest\data_received";

            try
            {
                // Get the list of directories in the base directory
                var directories = Directory.GetDirectories(baseDirectoryPath);

                foreach (var directory in directories)
                {
                    // Get the list of XML files in the directory
                    var xmlFiles = Directory.GetFiles(directory, "*.xml");

                    foreach (var xmlFile in xmlFiles)
                    {
                        // Load the XML file
                        var purchaseOrderDetails = xmlLoader.LoadFromXml(xmlFile);

                        // Save the purchase orders to the database
                        SavePurchaseOrderDetails(purchaseOrderDetails.Details);
                    }
                }

                MessageBox.Show("Purchase orders loaded and saved successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading XML data: {ex.Message}");
            }
        }

        public void SavePurchaseOrderDetails(IEnumerable<PurchaseOrderDetail> details)
        {
            using (var context = new BGM_dbContext())
            {
                foreach (var detail in details)
                {
                    if (!IsPurchaseOrderDetailExists(detail.ProductId))
                    {
                        context.PurchaseOrderDetails.Add(detail);
                    }
                }
                context.SaveChanges();
            }
        }

        public bool IsPurchaseOrderDetailExists(int purchaseOrderDetailId)
        {
            using (var context = new BGM_dbContext())
            {
                return context.PurchaseOrderDetails.Any(p => p.PurchaseOrderDetailId == purchaseOrderDetailId);
            }
        }


    }
}
