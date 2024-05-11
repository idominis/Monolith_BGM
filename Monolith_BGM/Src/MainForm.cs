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
        #region Fields
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
        private RichTextBox _richTextBoxLogs;
        private ErrorHandlerService _errorHandlerService;
        private DateTime _startDate;
        private DateTime _endDate;
        #endregion

        #region Constructor
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
            InitializeRichTextBoxLogs();
            ConfigureLogging();
            SetupInitialValues();

            _controller = controller;
            _statusUpdateService = statusUpdateService;
            _errorHandler = errorHandlerService;

            RegisterEventHandlers();
            LoadDataAsync();
        }
        #endregion

        #region Initialization
        private void SetupInitialValues()
        {
            radioButtonOff.Checked = true;
            createXmlDbRadioButtonOff.Checked = true;
            saveToDbRadioButtonOff.Checked = true;
            this.StartPosition = FormStartPosition.CenterScreen;
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
            autoGenerateXmlTimer.AutoReset = true;
        }

        private void InitializeSaveToDbTimer()
        {
            saveToDbTimer = new System.Timers.Timer(10000); // Set interval to 10 seconds
            saveToDbTimer.Elapsed += SaveToDbTimer_Elapsed;
            saveToDbTimer.AutoReset = true;
        }

        private void InitializeRichTextBoxLogs()
        {
            _richTextBoxLogs = new RichTextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                Dock = DockStyle.Fill
            };
            Controls.Add(_richTextBoxLogs);
        }

        private void ConfigureLogging()
        {
            var richTextBoxSink = new RichTextBoxSink(richTextBoxLogs);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console() // Optional, keeps logging to console if needed
                .WriteTo.File("log.txt") // Optional, keeps writing to file
                .WriteTo.Sink(richTextBoxSink) // Custom sink for RichTextBox
                .CreateLogger();

            _errorHandlerService = new ErrorHandlerService();
        }

        private void RegisterEventHandlers()
        {
            _controller.DatesInitialized += Controller_DatesInitialized;
            _controller.ErrorOccurred += Controller_ErrorOccurred;
            _controller.LatestDateUpdated += UpdateLatestDate;
            _controller.ErrorOccurred += ShowErrorMessage;
            _statusUpdateService.StatusUpdated += UpdateStatusMessage;
            comboBoxStartDate.SelectedIndexChanged += ComboBoxStartDate_SelectedIndexChanged;
            comboBoxEndDate.SelectedIndexChanged += ComboBoxEndDate_SelectedIndexChanged;
        }
        #endregion

        #region Event Handlers
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
        #endregion

        #region Timer Events
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
        #endregion

        #region Service Control
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
        #endregion

        #region Data Management
        private async Task<bool> SavePODToDb(List<PurchaseOrderDetailDto> allPurchaseOrderDetails)
        {
            bool isSuccess = false;
            string statusMessage;

            try
            {
                if (allPurchaseOrderDetails == null || allPurchaseOrderDetails.Count == 0)
                {
                    statusMessage = "No Details XMLs found";
                }
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
        #endregion

        #region Utility Methods
        public void AppendLog(string message, bool isError = false)
        {
            richTextBoxLogs.Invoke(new Action(() =>
            {
                if (isError)
                {
                    richTextBoxLogs.SelectionColor = Color.Red;
                }
                else
                {
                    richTextBoxLogs.SelectionColor = Color.Black;
                }

                richTextBoxLogs.AppendText($"{DateTime.Now}: {message}{Environment.NewLine}");
                richTextBoxLogs.SelectionStart = richTextBoxLogs.Text.Length;
                richTextBoxLogs.ScrollToCaret(); // Auto-scroll to bottom
            }));
        }
        #endregion

        #region Date Range Selection
        private void DateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            _startDate = dateTimePicker1.Value;
            if (_startDate != DateTime.MinValue && _endDate != DateTime.MinValue)
            {
                SearchPOWithinDateRange();
            }
        }

        private void DateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            _endDate = dateTimePicker2.Value;
            if (_startDate != DateTime.MinValue && _endDate != DateTime.MinValue)
            {
                SearchPOWithinDateRange();
            }
        }

        private async void SearchPOWithinDateRange()
        {
            try
            {
                if (_startDate > _endDate)
                {
                    MessageBox.Show("Start date must be before or equal to the end date.", "Invalid Date Range", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var purchaseOrders = await _controller.SearchPurchaseOrdersWithinDateRangeAsync(_startDate, _endDate);

                if (purchaseOrders != null && purchaseOrders.Count > 0)
                {
                    AppendLog($"{purchaseOrders.Count} purchase orders found for the date range {_startDate:yyyy-MM-dd} to {_endDate:yyyy-MM-dd}");
                }
                else
                {
                    AppendLog($"No purchase orders found for the date range {_startDate:yyyy-MM-dd} to {_endDate:yyyy-MM-dd}");
                }
            }
            catch (Exception ex)
            {
                _errorHandler.LogError(ex, $"Failed to fetch purchase orders for the date range {_startDate:yyyy-MM-dd} to {_endDate:yyyy-MM-dd}");
                AppendLog($"Error fetching purchase orders for the date range {_startDate:yyyy-MM-dd} to {_endDate:yyyy-MM-dd}: {ex.Message}", true);
            }
        }
        #endregion

        #region Application Exit
        private void exitAppButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to exit the application?",
                "Exit Application",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
        #endregion

        #region XML Generation and Sending
        private async void CreateDataRangePOS_Click(object sender, EventArgs e)
        {
            try
            {
                if (_startDate > _endDate)
                {
                    MessageBox.Show("Start date must be before or equal to the end date.", "Invalid Date Range", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var success = await _controller.GenerateXml(_startDate, _endDate);

                if (success)
                    _statusUpdateService.RaiseStatusUpdated("XML files generated successfully!");
                else
                {
                    _statusUpdateService.RaiseStatusUpdated("No data found to generate XML files or files already generated!");
                }
            }
            catch (Exception ex)
            {
                _errorHandler.LogError(ex, "An error occurred generating XML files.");
                _statusUpdateService.RaiseStatusUpdated("An error occurred generating XML files", ex);
            }
        }

        private async void SendXmlDateGeneratedButton_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime startDate = DateTime.Parse(comboBoxStartDate.SelectedItem.ToString());
                DateTime endDate = DateTime.Parse(comboBoxEndDate.SelectedItem.ToString());

                var success = await _controller.SendComboDateGeneratedXml(startDate, endDate);

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

        private async void SendDataRangeButton_Click(object sender, EventArgs e)
        {
            try
            {
                var success = await _controller.SendDateGeneratedXml(_startDate, _endDate);

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
        #endregion

        #region Buttons onn / off

        private void SaveToDbRadioButtonOn_CheckedChanged(object sender, EventArgs e)
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

        private void SaveToDbRadioButtonOff_CheckedChanged(object sender, EventArgs e)
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

        private async void RadioButtonOn_CheckedChanged(object sender, EventArgs e)
        {
            // Upload all Purchase Orders when the radio button is checked
            if (radioButtonOn.Checked)
            {
                radioButtonOff.Checked = false;
                await _controller.UploadAllHeaders();
            }
        }

        private void RadioButtonOff_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonOff.Checked)
            {
                radioButtonOn.Checked = false;
            }
        }

        private void CreateXmlDbRadioButtonOn_CheckedChanged(object sender, EventArgs e)
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

        private void CreateXmlDbRadioButtonOff_CheckedChanged(object sender, EventArgs e)
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

        #endregion


        #region Cleanup
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
        #endregion
      
    }
}
