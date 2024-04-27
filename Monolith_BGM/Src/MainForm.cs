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
        private ErrorHandlerService _errorHandler;
        private readonly IStatusUpdateService _statusUpdateService;

        private string purchasingOrdersPath = @"\PurchasingOrders";
        private string purchasingHeadersPath = @"\PurchasingOrdersHeaders";
        private string localBaseDirectoryPath = @"C:\Users\Ivan\Documents\BGM_project\RebexTinySftpServer-Binaries-Latest\data_received";

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
            string host = "192.168.75.1";
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
        }

        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            DownloadFiles(purchasingOrdersPath, localBaseDirectoryPath);
            DownloadFiles(purchasingHeadersPath, Path.Combine(localBaseDirectoryPath, "Headers"));
        }

        private void DownloadFiles(string remotePath, string localPath)
        {
            try
            {
                bool filesDownloaded = fileHandler.DownloadXmlFilesFromDirectory(remotePath, localPath);
                if (filesDownloaded)
                {
                    _statusUpdateService.RaiseStatusUpdated($"XML files from {remotePath} have been downloaded successfully!");
                    Log.Information($"XML files from {remotePath} have been downloaded successfully!");
                }
                else
                {
                    _statusUpdateService.RaiseStatusUpdated($"No new XML files in {remotePath}.");
                    Log.Information($"No new XML files in {remotePath}.");
                }
            }
            catch (Exception ex)
            {
                _errorHandler.LogError(ex, $"Failed to download XML files from {remotePath}.");
            }
        }


        // Override the OnFormClosing method to clean up the timer
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
            }
            _statusUpdateService.StatusUpdated -= UpdateStatusMessage;
            base.OnFormClosing(e); // Call the base class method
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

        private void ServiceStartButton_Click(object sender, EventArgs e)
        {
            if (timer == null || !timer.Enabled)
            {
                InitializeTimer();
                _statusUpdateService.RaiseStatusUpdated("The service has been started.");
                Log.Information("The service has been started.");
            }
            else
            {
                _statusUpdateService.RaiseStatusUpdated("The service is already running.");
                Log.Information("The service is already running.");
            }
        }

        private void ServiceStopButton_Click(object sender, EventArgs e)
        {
            if (timer != null && timer.Enabled)
            {
                timer.Stop();
                _statusUpdateService.RaiseStatusUpdated("The service has been stopped.");
                Log.Information("The service has been stopped.");
            }
            else
            {
                _statusUpdateService.RaiseStatusUpdated("The service is not running.");
                Log.Information("The service is not running.");
            }
        }

        private async void SavePODToDbButton_Click(object sender, EventArgs e)
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
                if (await _dataService.AddPurchaseOrderDetailsAsync(allPurchaseOrderDetails))
                    _statusUpdateService.RaiseStatusUpdated("POD files saved to DB!");
                else
                    _statusUpdateService.RaiseStatusUpdated("POD files failed saving to DB!");
            }
            catch (Exception ex)
            {
                _errorHandler.LogError(ex, "Error processing POD XML files.");
            }
        }

        private async void SavePOHToDbButton_Click(object sender, EventArgs e)
        {
            var xmlLoader = new XmlService();
            List<PurchaseOrderHeaderDto> allPurchaseOrderHeaders = new List<PurchaseOrderHeaderDto>();

            // Define the path to the 'Headers' directory inside the local base directory
            string headersDirectoryPath = Path.Combine(localBaseDirectoryPath, "Headers");

            try
            {
                // Collect all PurchaseOrderHeaders from XML files within the 'Headers' directory
                var xmlFiles = Directory.GetFiles(headersDirectoryPath, "*.xml", SearchOption.AllDirectories);
                foreach (var xmlFile in xmlFiles)
                {
                    try
                    {
                        var purchaseOrderHeaders = xmlLoader.LoadFromXml<PurchaseOrderHeaders>(xmlFile);
                        allPurchaseOrderHeaders.AddRange(purchaseOrderHeaders.Headers);
                    }
                    catch (Exception ex)
                    {
                        _errorHandler.LogError(ex, "Error loading POH XML data", xmlFile);
                    }
                }

                // Save the purchase orders to the database
                if (await _dataService.AddPurchaseOrderHeadersAsync(allPurchaseOrderHeaders))
                    _statusUpdateService.RaiseStatusUpdated("POH files saved to DB!");
                else
                    _statusUpdateService.RaiseStatusUpdated("POH files failed saving to DB!");
            }
            catch (Exception ex)
            {
                _errorHandler.LogError(ex, "Error processing POH XML files.");
            }
        }

    }
}
