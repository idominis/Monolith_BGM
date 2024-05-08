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
        private readonly ErrorHandlerService _errorHandler;
        private readonly IStatusUpdateService _statusUpdateService;
        private SftpFileHandler _fileHandler;
        private MainFormController _controller;
        private readonly IXmlService _xmlService;
        private FileManager _fileManager;
        private System.Timers.Timer autoGenerateXmlTimer;
        private System.Timers.Timer saveToDbTimer;

        private static readonly object _lockObj = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="statusUpdateService">The status update service.</param>
        public MainForm(MainFormController controller, IStatusUpdateService statusUpdateService, ErrorHandlerService errorHandlerService)
        {
            InitializeComponent();
            InitializeAutoGenerateXmlTimer();
            InitializeSaveToDbTimer();
            radioButtonOff.Checked = true;
            createXmlDbRadioButtonOff.Checked = true;
            saveToDbRadioButtonOff.Checked = true;

            _controller = controller;
            _statusUpdateService = statusUpdateService;
            _errorHandler = errorHandlerService;

            _controller.DatesInitialized += Controller_DatesInitialized;
            _controller.ErrorOccurred += Controller_ErrorOccurred;
            _controller.LatestDateUpdated += UpdateLatestDate;
            _controller.ErrorOccurred += ShowErrorMessage;

            LoadDataAsync();
            _statusUpdateService.StatusUpdated += UpdateStatusMessage;
            comboBoxStartDate.SelectedIndexChanged += ComboBoxStartDate_SelectedIndexChanged;
            comboBoxEndDate.SelectedIndexChanged += ComboBoxEndDate_SelectedIndexChanged;
            createXmlDbRadioButtonOn.CheckedChanged += createXmlDbRadioButtonOn_CheckedChanged;
        }


        /// <summary>Controllers the dates initialized.</summary>
        /// <param name="orderDates">The order dates.</param>
        private void Controller_DatesInitialized(List<DateTime> orderDates)
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

        private void UpdateLatestDate(DateTime latestDate)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateLatestDate(latestDate)));
                return;
            }
            autoSendTextBox.Text = latestDate.ToString("yyyy-MM-dd");
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

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private async void LoadDataAsync()
        {
            try
            {
                var orderDates = await _controller.FetchDistinctOrderDatesAsync();
                Controller_DatesInitialized(orderDates);
            }
            catch (Exception ex)
            {
                _errorHandler.LogError(ex, "Failed to load order dates.");
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

        private void InitializeAutoGenerateXmlTimer()
        {
            autoGenerateXmlTimer = new System.Timers.Timer(10000); // Set interval to 10 seconds
            autoGenerateXmlTimer.Elapsed += AutoGenerateXmlTimer_Elapsed;
            autoGenerateXmlTimer.AutoReset = true; // Ensure the timer fires repeatedly every 10 seconds
        }

        private void InitializeSaveToDbTimer()
        {
            saveToDbTimer = new System.Timers.Timer(10000); // Set interval to 10 seconds
            saveToDbTimer.Elapsed += SaveToDbTimer_Elapsed;
            saveToDbTimer.AutoReset = true;
        }

        private async void SaveToDbTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!saveToDbRadioButtonOn.Checked)
            {
                saveToDbTimer.Stop();
                return;
            }

            var fetchingHeaderXmlData = await _controller.FetchXmlHeadersDataAsync();
            var fetchingDetailXmlData = await _controller.FetchXmlDetailsDataAsync();

            var isDetailSaved = await SavePODToDb(fetchingDetailXmlData);
            if (isDetailSaved)
            {
                _statusUpdateService.RaiseStatusUpdated("POD files saved to DB!");
            }
            else
            {
                _statusUpdateService.RaiseStatusUpdated("No POD files to save to DB!");
            }
            var isHeaderSaved = await SavePOHToDb(fetchingHeaderXmlData);
            if (isHeaderSaved)
            {
                _statusUpdateService.RaiseStatusUpdated("POH files saved to DB!");
            }
            else
            {
                _statusUpdateService.RaiseStatusUpdated("No POH files to save to DB!");
            }
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

        private async void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            // Stop the timer to prevent further invocations while handling errors
            timer.Stop();

            try
            {
                await _controller.DownloadFilesForPODAndPOHAsync();
            }
            catch (Exception ex)
            {
                _errorHandler.LogError(ex, "Error during file download", null, "FileDownload");

                // Show a MessageBox to ask whether the user wants to retry or exit
                DialogResult result = MessageBox.Show(
                    "An error occurred during file download. Do you want to retry or exit the application?",
                    "File Download Error",
                    MessageBoxButtons.RetryCancel,
                    MessageBoxIcon.Error);

                if (result == DialogResult.Cancel)
                {
                    // Exit the application if the user chooses to cancel
                    Application.Exit();
                    return;
                }
            }

            // If retry is selected or no errors occurred, restart the timer
            timer.Start();
        }

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

        private async Task<bool> SavePODToDb(List<PurchaseOrderDetailDto> allPurchaseOrderDetails)
        {

            bool isSuccess = false;
            string statusMessage;

            //var res = await _controller.SavePODetailsToDb(allPurchaseOrderDetails);

            try
            {
                if (allPurchaseOrderDetails == null || allPurchaseOrderDetails.Count == 0)
                {
                    statusMessage = "No Details XMLs found";
                }
                //else if (_controller.SavePOHeadersToDb(allPurchaseOrderHeaders).Result)
                else if (await _controller.SavePODetailsToDb(allPurchaseOrderDetails))
                {
                    statusMessage = "POD files saved to DB!";
                    isSuccess = true;
                }
                else
                {
                    statusMessage = "No POD files to save to DB!";
                }
            }
            catch (Exception ex)
            {
                _errorHandler.LogError(ex, "Error processing POD XML files.");
                statusMessage = "Error saving POD to database.";
            }

            _statusUpdateService.RaiseStatusUpdated(statusMessage);
            return isSuccess;

        }

        private async Task<bool> SavePOHToDb(List<PurchaseOrderHeaderDto> allPurchaseOrderHeaders)
        {         
            bool isSuccess = false;
            string statusMessage;

            try
                {
                    if (allPurchaseOrderHeaders == null || allPurchaseOrderHeaders.Count == 0)
                    {
                        statusMessage = "No Headers XMLs found";
                    }
                    else if (_controller.SavePOHeadersToDb(allPurchaseOrderHeaders).Result)
                    //else if (res)
                    {
                        statusMessage = "POH files saved to DB!";
                        isSuccess = true;
                    }
                    else
                    {
                        statusMessage = "POH files failed saving to DB!";
                    }
                }
                catch (Exception ex)
                {
                    _errorHandler.LogError(ex, "Error processing POH XML files.");
                    statusMessage = "Error saving POH to database.";
                }

                _statusUpdateService.RaiseStatusUpdated(statusMessage);
                return isSuccess;
            
        }

        private async void CreatePOSXMLsButton_ClickAsync(object sender, EventArgs e)
        {
            try
            {
                var success = await _controller.GenerateXmlFromDbAsync();

                if (success)
                    _statusUpdateService.RaiseStatusUpdated("XML files generated successfully!");
                else
                {
                    _statusUpdateService.RaiseStatusUpdated("No data found to generate XML files");
                }
            }
            catch (Exception ex)
            {
                _errorHandler.LogError(ex, "An error occurred generating XML files.");
                _statusUpdateService.RaiseStatusUpdated("An error occurred generating XML files", ex);
            }
        }

        private async void GenerateXmlByDateButton_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime startDate = DateTime.Parse(comboBoxStartDate.SelectedItem.ToString());
                DateTime endDate = DateTime.Parse(comboBoxEndDate.SelectedItem.ToString());

                bool success = await _controller.GenerateXml(startDate, endDate);

                if (success)
                {
                    _statusUpdateService.RaiseStatusUpdated("XML files generated successfully!");
                }
                else
                {
                    _statusUpdateService.RaiseStatusUpdated("No data found to generate XML files.");
                }
            }
            catch (Exception ex)
            {
                _errorHandler.LogError(ex, "An error occurred while generating XML files by date.");
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void RadioButtonOn_CheckedChanged(object sender, EventArgs e)
        {
            // Upload all Purchase Orders when the radio button is checked
            if (radioButtonOn.Checked)
            {
                radioButtonOff.Checked = false;
                _controller.UploadAllHeaders();
            }
        }

        private void RadioButtonOff_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonOff.Checked)
            {
                radioButtonOn.Checked = false;
            }
        }

        private async void SendXmlDateGeneratedButton_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime startDate = DateTime.Parse(comboBoxStartDate.SelectedItem.ToString());
                DateTime endDate = DateTime.Parse(comboBoxEndDate.SelectedItem.ToString());

                var success = await _controller.SendDateGeneratedXml(startDate, endDate);

                if (success)
                {
                    _statusUpdateService.RaiseStatusUpdated("XML files sent to remote directory successfully!");
                }
                else
                {
                    _statusUpdateService.RaiseStatusUpdated("XML files not sent");
                }
            }
            catch (Exception ex)
            {
                _errorHandler.LogError(ex, "Failed to send XML files.");
                MessageBox.Show("Failed to send file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _statusUpdateService.RaiseStatusUpdated("XML files not sent");
                Log.Error(ex, "Failed to send XML files");
            }
        }

        private void createXmlDbRadioButtonOn_CheckedChanged(object sender, EventArgs e)
        {
            if (createXmlDbRadioButtonOn.Checked)
            {
                autoGenerateXmlTimer.Start();
            }
            else
            {
                autoGenerateXmlTimer.Stop();
            }
        }

        private void createXmlDbRadioButtonOff_CheckedChanged(object sender, EventArgs e)
        {
            if (createXmlDbRadioButtonOff.Checked)
            {
                autoGenerateXmlTimer.Stop();
            }
            else
            {
                autoGenerateXmlTimer.Start();
            }
        }

        private async void AutoGenerateXmlTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!createXmlDbRadioButtonOn.Checked)
            {
                autoGenerateXmlTimer.Stop();
                return;
            }

            try
            {
                var success = await _controller.GenerateXmlFromDbAsync();
                if (success)
                {
                    _statusUpdateService.RaiseStatusUpdated("XML files generated successfully!");
                }
                else
                {
                    _statusUpdateService.RaiseStatusUpdated("No data found to generate XML files.");
                }
            }
            catch (Exception ex)
            {
                _errorHandler.LogError(ex, "Error generating XML files.");
                _statusUpdateService.RaiseStatusUpdated("An error occurred generating XML files", ex);
                Log.Error(ex, "Error generating XML files");
            }
        }

        private void saveToDbRadioButtonOn_CheckedChanged(object sender, EventArgs e)
        {
            if (saveToDbRadioButtonOn.Checked)
            {
                saveToDbTimer.Start();
            }
            else
            {
                saveToDbTimer.Stop();
            }
        }

        private void saveToDbRadioButtonOff_CheckedChanged(object sender, EventArgs e)
        {
            if (saveToDbRadioButtonOff.Checked)
            {
                saveToDbTimer.Stop();
            }
            else
            {
                saveToDbTimer.Start();
            }
        }

        private async void SavePODToDbButton_Click(object sender, EventArgs e)
        {
            //await SavePODToDb();
            // TESTING PURPOSES ONLY
            // Disable the button to prevent multiple clicks
            SavePODToDbButton.Enabled = false;

            try
            {
                var fetchingDetailXmlData = await _controller.FetchXmlDetailsDataAsync(); // Separate responsibility
                var isSaved = await SavePODToDb(fetchingDetailXmlData);
                if (isSaved)
                {
                    _statusUpdateService.RaiseStatusUpdated("POD files saved to DB!");
                }
                else
                {
                    _statusUpdateService.RaiseStatusUpdated("No POD files to save to DB!");
                }
            }
            finally
            {
                // Re-enable the button after execution
                SavePODToDbButton.Enabled = true;
            }
        }

        private async void SavePOHToDbButton_Click(object sender, EventArgs e)
        {
            // TESTING PURPOSES ONLY
            // Disable the button to prevent multiple clicks
            SavePOHToDbButton.Enabled = false;

            try
            {
                var fetchingHeaderXmlData = await _controller.FetchXmlHeadersDataAsync(); // Separate responsibility
                var isSaved = await SavePOHToDb(fetchingHeaderXmlData);
                if (isSaved)
                {
                    _statusUpdateService.RaiseStatusUpdated("POH files saved to DB!");
                }
                else
                {
                    _statusUpdateService.RaiseStatusUpdated("No POH files to save to DB!");
                }
            }
            finally
            {
                // Re-enable the button after execution
                SavePOHToDbButton.Enabled = true;
            }
        }

    }
}
