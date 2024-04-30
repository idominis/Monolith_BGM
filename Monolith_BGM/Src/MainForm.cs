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
using Monolith_BGM.Controllers;
using Monolith_BGM.XMLService;
using Monolith_BGM.Src;

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
        private SftpFileHandler _fileHandler;
        private MainFormController _controller;
        private readonly IXmlService _xmlService;
        private FileManager _fileManager;

        public MainForm(MainFormController controller, IMapper mapper, DataService dataService, ErrorHandlerService errorHandler, IStatusUpdateService statusUpdateService, SftpFileHandler fileHandler, IXmlService xmlService)
        {
            InitializeComponent();
            _mapper = mapper;
            _controller = controller;
            _dataService = dataService;
            _statusUpdateService = statusUpdateService;
            _fileHandler = fileHandler;
            _errorHandler = errorHandler;
            _xmlService = xmlService;
            _fileManager = new FileManager();
            _controller.DataInitialized += Controller_DataInitialized;
            _controller.ErrorOccurred += Controller_ErrorOccurred;
            LoadDataAsync();
            comboBoxStartDate.SelectedIndexChanged += ComboBoxStartDate_SelectedIndexChanged;
            comboBoxEndDate.SelectedIndexChanged += ComboBoxEndDate_SelectedIndexChanged;
            
        }

        private void Controller_DataInitialized(List<DateTime> orderDates)
        {
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

            ValidateDateSelection();
        }

        private void Controller_ErrorOccurred(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private async void LoadDataAsync()
        {
            try
            {
                var orderDates = await _dataService.FetchDistinctOrderDatesAsync();
                Controller_DataInitialized(orderDates);
            }
            catch (Exception ex)
            {
                Controller_ErrorOccurred("Failed to load order dates: " + ex.Message);
            }
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

        private void ComboBoxStartDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateDateSelection();
        }

        private void ComboBoxEndDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateDateSelection();
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
                    MessageBox.Show("Start date must be before the end date. Adjusting end date to match start date");
                }
            }
        }

        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            string localBaseDirectoryPath = _fileManager.GetBaseDirectoryPath();
            string localHeadersPath = _fileManager.GetSpecificPath("headers");
            string remoteDetailsDirectoryPath = _fileManager.GetRemoteDetailsDirectoryPath();
            string remoteHeadersDirectoryPath = _fileManager.GetRemoteHeadersDirectoryPath();

            DownloadFiles(remoteDetailsDirectoryPath, localBaseDirectoryPath); // Download POD files
            DownloadFiles(remoteHeadersDirectoryPath, localHeadersPath); // Download POH files
        }

        private void DownloadFiles(string remotePath, string localPath)
        {
            // Add breakpoint here to check the state of _fileHandler
            if (_fileHandler == null)
                throw new InvalidOperationException("File handler is not initialized.");

            bool filesDownloaded = _fileHandler.DownloadXmlFilesFromDirectory(remotePath, localPath);
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
            List<PurchaseOrderDetailDto> allPurchaseOrderDetails = new List<PurchaseOrderDetailDto>();
            string localBaseDirectoryPath = _fileManager.GetBaseDirectoryPath();
            try
            {
                // Collect all PurchaseOrderDetails from XML files
                var xmlFiles = Directory.GetFiles(localBaseDirectoryPath, "*.xml", SearchOption.AllDirectories);
                foreach (var xmlFile in xmlFiles)
                {
                    try
                    {
                        var purchaseOrderDetails = _xmlService.LoadFromXml<PurchaseOrderDetails>(xmlFile);
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
            List<PurchaseOrderHeaderDto> allPurchaseOrderHeaders = new List<PurchaseOrderHeaderDto>();
            string localBaseDirectoryPath = _fileManager.GetBaseDirectoryPath();
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
                        var purchaseOrderHeaders = _xmlService.LoadFromXml<PurchaseOrderHeaders>(xmlFile);
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
            try
            {
                var summaries = await _dataService.FetchPurchaseOrderSummaries();
                _xmlService.GenerateXMLFiles(summaries);
                _statusUpdateService.RaiseStatusUpdated("XML files generated successfully!");
            }
            catch (Exception ex)
            {
                _statusUpdateService.RaiseStatusUpdated("An error occurred generating XML files");
            }
        }



        private void RadioButtonOn_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonOn.Checked)
            {
                radioButtonOff.Checked = false;
                UploadAllHeaders();
            }
        }

        private void RadioButtonOff_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonOff.Checked)
            {
                radioButtonOn.Checked = false;
            }
        }

        private async void GenerateXmlButton_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime startDate = DateTime.Parse(comboBoxStartDate.SelectedItem.ToString());
                DateTime endDate = DateTime.Parse(comboBoxEndDate.SelectedItem.ToString());

                var summaries = await _dataService.FetchPurchaseOrderSummariesByDateAsync(startDate, endDate);

                string localBaseDirectoryXmlCreatedPath = _fileManager.GetBaseDirectoryXmlCreatedPath();

                _xmlService.GenerateXMLFiles(summaries, startDate, endDate);

                MessageBox.Show($"XML file successfully created at {localBaseDirectoryXmlCreatedPath}"); 
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private async void SendXmlButton_Click(object sender, EventArgs e)
        {
            try
            {
                string localBaseDirectoryXmlCreatedPath = _fileManager.GetBaseDirectoryXmlCreatedPath();

                // If using date pickers for dynamic file names
                DateTime startDate = DateTime.Parse(comboBoxStartDate.SelectedItem.ToString());
                DateTime endDate = DateTime.Parse(comboBoxEndDate.SelectedItem.ToString());
                string fileName = $"PurchaseOrderSummariesGenerated_{startDate:yyyyMMdd}_to_{endDate:yyyyMMdd}.xml";

                // Full path to the file
                string filePath = Path.Combine(localBaseDirectoryXmlCreatedPath, fileName);

                // Construct the remote path
                string remotePath = Path.Combine("/Uploaded/", fileName);

                // Check if the file exists
                if (File.Exists(filePath))
                {
                    // Use SftpFileHandler to upload the file to a specific folder on the server
                    await _fileHandler.UploadFileAsync(filePath, remotePath);
                    MessageBox.Show("File successfully sent: " + fileName, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("File not found: " + filePath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to send file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void UploadAllHeaders()
        {
            string localBaseDirectoryPath = _fileManager.GetBaseDirectoryPath();
            string localDirectoryPath = _fileManager.GetSpecificPath("Headers");

            string remoteDirectoryPath = "/Uploaded/";  // RebexTinySftpServer\data\Uploaded

            DateTime? latestDate = null;

            try
            {
                DirectoryInfo dir = new DirectoryInfo(localDirectoryPath);
                FileInfo[] files = dir.GetFiles("PurchaseOrderHeader*.xml");

                foreach (FileInfo file in files)
                {
                    string localFilePath = file.FullName;
                    string remoteFilePath = Path.Combine(remoteDirectoryPath, file.Name);
                    await _fileHandler.UploadFileAsync(localFilePath, remoteFilePath);
                    Log.Information($"Uploaded {file.Name} to {remoteFilePath}");

                    // Extract PurchaseOrderID from the filename
                    int purchaseOrderId = int.Parse(Path.GetFileNameWithoutExtension(file.Name).Replace("PurchaseOrderHeader", ""));

                    // Update the database with the upload status
                    await _dataService.UpdatePurchaseOrderSentStatus(purchaseOrderId, true, true, 0);  // 0 - Auto, 1 - Custom

                    // Optionally update UI or handle latest date
                    DateTime? fileDate = await _dataService.GetLatestDateForPurchaseOrder(purchaseOrderId);
                    if (fileDate.HasValue && (latestDate == null || fileDate > latestDate))
                    {
                        latestDate = fileDate;
                        Invoke(new Action(() => {
                            autoSendTextBox.Text = latestDate.Value.ToString("yyyy-MM-dd");
                        }));
                    }
                }

                MessageBox.Show("All files have been successfully uploaded.", "Upload Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error uploading files");
                MessageBox.Show("Failed to upload files: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
