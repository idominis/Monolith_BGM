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
            _statusUpdateService = statusUpdateService;
            InitializeComponent();

            comboBoxStartDate.SelectedIndexChanged += ComboBoxStartDate_SelectedIndexChanged;
            comboBoxEndDate.SelectedIndexChanged += ComboBoxEndDate_SelectedIndexChanged;

            // Register the Load event
            this.Load += MainForm_Load;
            _statusUpdateService.StatusUpdated += UpdateStatusMessage;
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

        private async void MainForm_Load(object sender, EventArgs e)
        {
            await PopulateOrderDateDropdowns();
        }

        private void ComboBoxStartDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateDateSelection();
        }

        private void ComboBoxEndDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateDateSelection();
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

        private async void CreatePOSXMLsButton_ClickAsync(object sender, EventArgs e)
        {
            var xmlLoader = new XmlService();

            try
            {
                var summaries = await _dataService.FetchPurchaseOrderSummaries();
                xmlLoader.GenerateXMLFiles(summaries);
                _statusUpdateService.RaiseStatusUpdated("XML files generated successfully!");
            }
            catch (Exception ex)
            {
                _statusUpdateService.RaiseStatusUpdated("An error occurred generating XML files");
            }
        }

        private async Task PopulateOrderDateDropdowns()
        {
            try
            {
                var orderDates = await _dataService.FetchDistinctOrderDatesAsync();
                comboBoxStartDate.Items.Clear();
                comboBoxEndDate.Items.Clear();

                foreach (var date in orderDates)
                {
                    comboBoxStartDate.Items.Add(date.ToString("yyyy-MM-dd"));
                    comboBoxEndDate.Items.Add(date.ToString("yyyy-MM-dd"));
                }

                if (comboBoxStartDate.Items.Count > 0)
                    comboBoxStartDate.SelectedIndex = 0;  // Select first item by default
                if (comboBoxEndDate.Items.Count > 0)
                    comboBoxEndDate.SelectedIndex = comboBoxEndDate.Items.Count - 1;  // Select last item by default
            }
            catch (Exception ex)
            {
                _statusUpdateService.RaiseStatusUpdated("Failed to load order dates");
            }
        }

        private void ValidateDateSelection()
        {
            if (comboBoxStartDate.SelectedItem != null && comboBoxEndDate.SelectedItem != null)
            {
                var startDate = DateTime.Parse(comboBoxStartDate.SelectedItem.ToString());
                var endDate = DateTime.Parse(comboBoxEndDate.SelectedItem.ToString());

                if (startDate > endDate)
                {
                    comboBoxEndDate.SelectedItem = comboBoxStartDate.SelectedItem;
                    _statusUpdateService.RaiseStatusUpdated("Start date must be before the end date. Adjusting end date to match start date");
                }
            }
        }

        private void radioButtonOn_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonOn.Checked)
            {
                radioButtonOff.Checked = false;
            }
        }

        private void radioButtonOff_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonOff.Checked)
            {
                radioButtonOn.Checked = false;
            }
        }

        private async void generateXmlButton_Click(object sender, EventArgs e)
        {
            var xmlLoader = new XmlService();
            try
            {
                DateTime startDate = DateTime.Parse(comboBoxStartDate.SelectedItem.ToString());
                DateTime endDate = DateTime.Parse(comboBoxEndDate.SelectedItem.ToString());

                var summaries = await _dataService.FetchPurchaseOrderSummariesByDateAsync(startDate, endDate);

                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PurchaseOrders.xml");

                xmlLoader.GenerateXMLFiles(summaries, startDate, endDate);

                MessageBox.Show($"XML file successfully created at {filePath}"); // TODO - consider showing a dialog with the file path
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private async void sendXmlButton_Click(object sender, EventArgs e)
        {
            try
            {
                string basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                               "BGM_project", "RebexTinySftpServer-Binaries-Latest", "data", "Xmls_Created");

                // If using date pickers for dynamic file names
                DateTime startDate = DateTime.Parse(comboBoxStartDate.SelectedItem.ToString());
                DateTime endDate = DateTime.Parse(comboBoxEndDate.SelectedItem.ToString());
                string fileName = $"PurchaseOrderSummaries_{startDate:yyyyMMdd}_to_{endDate:yyyyMMdd}.xml";

                // Full path to the file
                string filePath = Path.Combine(basePath, fileName);

                // Construct the remote path
                string remotePath = Path.Combine("/Uploaded/", fileName); // Ensure this path is correctly handled by your SFTP server

                // Check if the file exists
                if (File.Exists(filePath))
                {
                    // Use SftpFileHandler to upload the file to a specific folder on the server
                    await fileHandler.UploadFileAsync(filePath, remotePath);
                    MessageBox.Show("File successfully sent: " + fileName, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);// TODO - consider showing a dialog with the file path
                }
                else
                {
                    MessageBox.Show("File not found: " + filePath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);// TODO - consider showing a dialog with the file path
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to send file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);// TODO - consider showing a dialog with the file path
            }
        }



    }
}
