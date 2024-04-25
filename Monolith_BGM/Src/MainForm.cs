using BGM.SftpUtilities;
using Monolith_BGM.DataAccess.DTO;
using Monolith_BGM.Models;
using System;
using System.Windows.Forms;
using AutoMapper;
using Microsoft.VisualBasic.Logging;
using Serilog;
using Log = Serilog.Log;
using Microsoft.EntityFrameworkCore;
using BGM.Common;
using System.Data;

namespace Monolith_BGM
{
    public partial class MainForm : Form
    {
        private System.Timers.Timer timer;
        private SftpClientManager clientManager;
        private SftpFileHandler fileHandler;
        private readonly IMapper _mapper;
        private readonly DataService _dataService;
        private string remoteBaseDirectoryPath = @"\PurchasingOrders";
        private string localBaseDirectoryPath = @"C:\Users\Ivan\Documents\BGM_project\RebexTinySftpServer-Binaries-Latest\data_received";
        private ErrorHandlerService _errorHandler;
        private readonly IStatusUpdateService _statusUpdateService;

        public MainForm(IMapper mapper, DataService dataService, ErrorHandlerService errorHandler, IStatusUpdateService statusUpdateService)
        {                              
            _mapper = mapper;
            _dataService = dataService;
            _errorHandler = errorHandler;
            _statusUpdateService = statusUpdateService; // Make sure this is assigned before calling any method that uses it
            InitializeComponent();
            _statusUpdateService.StatusUpdated += UpdateStatusMessage; // Subscribe to events after it is assigned
            InitializeSftp();
        }

        private void InitializeSftp()
        {
            string host = "192.168.56.1";
            string username = "tester";
            string password = "password";

            clientManager = new SftpClientManager(host, username, password);
            fileHandler = new SftpFileHandler(clientManager, _statusUpdateService);
        }

        private void InitializeTimer()
        {
            if (timer == null)
            {
                timer = new System.Timers.Timer(10000); // Interval in milliseconds
                timer.Elapsed += OnTimedEvent;
                timer.AutoReset = true;
            }
            timer.Enabled = true;

            // Call OnTimedEvent immediately
            //OnTimedEvent(timer, null);
        }

        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                // Download XML files from the directory
                bool newFilesDownloaded = fileHandler.DownloadXmlFilesFromDirectory(remoteBaseDirectoryPath, localBaseDirectoryPath);

                if (newFilesDownloaded)
                {
                    _statusUpdateService.RaiseStatusUpdated("XML files have been downloaded successfully!");
                    Log.Information("XML files have been downloaded successfully!");
                }
                else
                {
                    _statusUpdateService.RaiseStatusUpdated("No new XML files");
                    Log.Information("No new XML files.");
                }
            }
            catch (Exception ex)
            {
                _errorHandler.LogError(ex, "Failed to download XML files.");
            }
        }

        // Override the OnFormClosing method to clean up the timer
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (timer != null)
            {
                timer.Stop();  
                timer.Dispose();
                timer = null;
            }

            _statusUpdateService.StatusUpdated -= UpdateStatusMessage;
            base.OnFormClosing(e); // Call the base class method
        }

        private async void SaveToDbButton_Click(object sender, EventArgs e)
        {
            var xmlLoader = new XmlService();
            
            List<PurchaseOrderDetailDto> allPurchaseOrderDetails = new List<PurchaseOrderDetailDto>();

            try
            {
                // Collect all PurchaseOrderDetails from XML files
                var xmlFiles = Directory.GetFiles(localBaseDirectoryPath, "*.xml", SearchOption.AllDirectories);
                foreach (var xmlFile in xmlFiles)
                {
                    try
                    {
                        var purchaseOrderDetails = xmlLoader.LoadFromXml<PurchaseOrderDetails>(xmlFile);
                        allPurchaseOrderDetails.AddRange(purchaseOrderDetails.Details);
                    }
                    catch (Exception ex)
                    {
                        _errorHandler.LogError(ex, "Error loading XML data", xmlFile);
                    }
                }

                // Save the purchase orders to the database
                await _dataService.AddPurchaseOrderDetailsAsync(allPurchaseOrderDetails);
            }
            catch (Exception ex)
            {
                _errorHandler.LogError(ex, "Error processing XML files.");
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
                _statusUpdateService.RaiseStatusUpdated("The service has been started");
                Log.Information("The service has been started.");
            }
            else
            {
                _statusUpdateService.RaiseStatusUpdated("The service is running");
                Log.Information("The service is running.");
            }
        }

        private void ServiceStopButton_Click(object sender, EventArgs e)
        {
            if (timer != null && timer.Enabled)
            {
                timer.Stop();
                _statusUpdateService.RaiseStatusUpdated("The service has been stopped");
                Log.Information("The service has been stopped.");
            }
            else
            {
                _statusUpdateService.RaiseStatusUpdated("The service is not running");
                Log.Information("The service is not running."); 
            }
        }

        private void UpdateStatusMessage(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => toolStripStatusLabel.Text = message));
            }
            else
            {
                toolStripStatusLabel.Text = message;
            }
        }
    }
}
