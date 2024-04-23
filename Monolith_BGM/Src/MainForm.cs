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
            string host = "192.168.100.163";
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

         
                //fileHandler.DownloadXmlFilesFromDirectory(remoteBaseDirectoryPath, localBaseDirectoryPath);
                //MessageBox.Show("All XML files have been downloaded successfully!");

                // Ask user if they want to exit the application
                //DialogResult exitResponse = MessageBox.Show("Do you want to exit the application?", "Exit Application", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            //if (exitResponse == DialogResult.Yes)
            //{
            //    Application.Exit(); // Close the application if the user chooses 'Yes'
            //}
            //else
            //{
                //var xmlLoader = new XmlDataLoader();
                //string directoryPath = @"C:\Users\Ivan\Documents\BGM_project\RebexTinySftpServer-Binaries-Latest\data_received";

                try
                {
                    // Get the latest directory
                string latestRemoteDirectoryPath = fileHandler.GetLatestRemoteDirectory(remoteBaseDirectoryPath);
                //string latestLocalDirectoryPath = Path.Combine(localBaseDirectoryPath, Path.GetFileName(localBaseDirectoryPath));
                string latestLocalDirectoryPath = fileHandler.GetLatestLocalDirectory(localBaseDirectoryPath);

                if()

                // Check if all files in the latest directory have been downloaded
                bool areAllFilesDownloaded = fileHandler.AreAllFilesDownloaded(latestRemoteDirectoryPath, latestLocalDirectoryPath);
                    if (!areAllFilesDownloaded)
                    {
                        fileHandler.GetLatestRemoteDirectory(remoteBaseDirectoryPath);
                        var areAllDownloaded = fileHandler.AreAllFilesDownloaded(latestRemoteDirectoryPath, latestLocalDirectoryPath);
                        // Download missing files from the latest directory
                        if (!areAllDownloaded)
                        {
                            fileHandler.DownloadXmlFilesFromDirectory(latestRemoteDirectoryPath, latestLocalDirectoryPath);
                            MessageBox.Show("New XML files have been downloaded successfully!");
                        }

                        // Rest of the code...
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to download XML files: " + ex.Message);
                }
            //}
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

    }
}
