using BGM.SftpUtilities;
using Monolith_BGM.DataAccess.DTO;
using Monolith_BGM.Models;
using System;
using System.Windows.Forms;
using AutoMapper;

namespace Monolith_BGM
{
    public partial class MainForm : Form
    {
        private System.Timers.Timer timer;
        private SftpClientManager clientManager;
        private SftpFileHandler fileHandler;
        private readonly IMapper _mapper;


        public MainForm(IMapper mapper)
        {
            _mapper = mapper;
            InitializeComponent();
            InitializeSftp();
        }

        private void InitializeSftp()
        {
            string host = "192.168.56.1";
            string username = "tester";
            string password = "password";

            clientManager = new SftpClientManager(host, username, password);
            fileHandler = new SftpFileHandler(clientManager);
        }

        private void InitializeTimer()
        {
            if (timer == null)
            {
                timer = new System.Timers.Timer(10000); // Interval in milliseconds
                timer.Elapsed += OnTimedEvent;
                timer.AutoReset = true;
            }
            timer.Enabled = true; // Ensure the timer is enabled.
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
            List<PurchaseOrderDetailDto> allPurchaseOrderDetails = new List<PurchaseOrderDetailDto>();

            try
            {
                // Collect all PurchaseOrderDetails from XML files
                var xmlFiles = Directory.GetFiles(baseDirectoryPath, "*.xml", SearchOption.AllDirectories);
                foreach (var xmlFile in xmlFiles)
                {
                    var purchaseOrderDetails = xmlLoader.LoadFromXml<PurchaseOrderDetails>(xmlFile);
                    allPurchaseOrderDetails.AddRange(purchaseOrderDetails.Details);
                }

                // Save the purchase orders to the database
                SavePurchaseOrderDetails(allPurchaseOrderDetails);
                MessageBox.Show("Purchase orders loaded and saved successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading XML data: {ex.Message}");
            }
        }

        public void SavePurchaseOrderDetails(List<PurchaseOrderDetailDto> allDetailsDto)
        {
            using (var context = new BGM_dbContext())
            {
                // Fetch existing IDs from the database
                var existingIds = new HashSet<int>(context.PurchaseOrderDetails.Select(p => p.PurchaseOrderDetailId));

                // Filter DTOs first before mapping
                var filteredDtos = allDetailsDto.Where(dto => !existingIds.Contains(dto.PurchaseOrderDetailId)).ToList();
                var allDetails = filteredDtos.Select(dto => _mapper.Map<PurchaseOrderDetail>(dto)).ToList();


                // Filter out details that already exist in the database
                var newDetails = allDetails.Where(d => !existingIds.Contains(d.PurchaseOrderDetailId)).ToList();

                // Add and save new entries
                if (newDetails.Any())
                {
                    context.PurchaseOrderDetails.AddRange(newDetails);
                    context.SaveChanges();
                }
                else
                {
                    MessageBox.Show("No new purchase orders to save.");
                }
            }
        }


        private void ServiceStartButton_Click(object sender, EventArgs e)
        {
            if (timer == null)
            {
                InitializeTimer();
            }

            if (!timer.Enabled)
            {
                timer.Start();
                MessageBox.Show("The service has been started.");
            }
            else
            {
                MessageBox.Show("The service is running.");
            }
        }

        private void ServiceStopButton_Click(object sender, EventArgs e)
        {
            if (timer != null && timer.Enabled)
            {
                timer.Stop();
                MessageBox.Show("The service has been stopped.");
            }
            else
            {
                MessageBox.Show("The service is not running.");
            }
        }
    }
}
