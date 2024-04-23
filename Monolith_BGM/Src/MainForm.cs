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

        private void button1_Click(object sender, EventArgs e)
        {
            // Path to the XML file
            string filePath = @"path\to\your\file.xml";
            var xmlLoader = new XmlDataLoader();
            var purchaseOrders = xmlLoader.LoadFromXml<PurchaseOrderDetail>(filePath);

            // Save to database
            SavePurchaseOrderDetails(purchaseOrders);

            MessageBox.Show("Data loaded and saved successfully!");
        }

    }
}
