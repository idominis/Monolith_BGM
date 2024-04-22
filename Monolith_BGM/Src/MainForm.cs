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
            string host = "192.168.0.128";
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
        }

        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            string remoteBaseDirectoryPath = @"\PurchasingOrders";
            string localBaseDirectoryPath = @"C:\Users\Ivan\Documents\BGM_project\RebexTinySftpServer-Binaries-Latest\data_received";

            // Call method to download all XML files from the directory
            try
            {
                fileHandler.DownloadXmlFilesFromDirectory(remoteBaseDirectoryPath, localBaseDirectoryPath);
                MessageBox.Show("All XML files have been downloaded successfully!");

                // Ask user if they want to exit the application
                DialogResult exitResponse = MessageBox.Show("Do you want to exit the application?", "Exit Application", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (exitResponse == DialogResult.Yes)
                {
                    Application.Exit(); // Close the application if the user chooses 'Yes'
                }
                else
                {
                    var xmlLoader = new XmlDataLoader();
                    string directoryPath = @"C:\Users\Ivan\Documents\BGM_project\RebexTinySftpServer-Binaries-Latest\data_received";

                    // Ensure the directory exists
                    if (!Directory.Exists(directoryPath))
                    {
                        MessageBox.Show("Directory does not exist.");
                        return;
                    }

                    //var purchaseOrderDetails = xmlLoader.LoadFromXml(filePath);
                    //var purchaseOrders = purchaseOrderDetails.Details;
                    // Process each XML file in the directory
                    var allPurchaseOrders = new List<PurchaseOrderDetail>();
                    foreach (string filePath in Directory.GetFiles(directoryPath, "*.xml"))
                    {
                        var purchaseOrderDetails = xmlLoader.LoadFromXml(filePath);
                        var purchaseOrders = purchaseOrderDetails.Details;
                        //var purchaseOrders = xmlLoader.LoadFromXml<PurchaseOrderDetail>(filePath);
                        //allPurchaseOrders.AddRange(purchaseOrders);  // Aggregate orders from all files
                    }

                    // Process purchaseOrders as needed
                    // Example: Display count of loaded purchase orders
                    MessageBox.Show($"Total purchase orders loaded: {allPurchaseOrders.Count}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to download XML files: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //var xmlLoader = new XmlDataLoader();
            //string directoryPath = @"C:\Users\Ivan\Documents\BGM_project\RebexTinySftpServer-Binaries-Latest\data_received";

            //// Ensure the directory exists
            //if (!Directory.Exists(directoryPath))
            //{
            //    MessageBox.Show("Directory does not exist.");
            //    return;
            //}

            //// Process each XML file in the directory
            //var allPurchaseOrders = new List<PurchaseOrderDetail>();
            //foreach (string filePath in Directory.GetFiles(directoryPath, "*.xml"))
            //{
            //    var purchaseOrders = xmlLoader.LoadFromXml<PurchaseOrderDetail>(filePath);
            //    allPurchaseOrders.AddRange(purchaseOrders);  // Aggregate orders from all files
            //}

            //// Process purchaseOrders as needed
            //// Example: Display count of loaded purchase orders
            //MessageBox.Show($"Total purchase orders loaded: {allPurchaseOrders.Count}");
        }

    }
}
