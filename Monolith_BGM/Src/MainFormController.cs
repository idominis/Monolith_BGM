using AutoMapper;
using BGM.Common;
using BGM.SftpUtilities;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.Logging;
using Monolith_BGM.DataAccess.DTO;
using Monolith_BGM.Models;
using Monolith_BGM.Src;
using Monolith_BGM.XMLService;
using System;
using System.Collections.Generic;
using System.Linq;
using static Monolith_BGM.DataAccess.DTO.PurchaseOrderHeaderDto;
using Log = Serilog.Log;

namespace Monolith_BGM.Src
{
    public class MainFormController
    {
        private readonly DataService _dataService;
        private readonly FileManager _fileManager;
        private readonly IXmlService _xmlService;
        private readonly ErrorHandlerService _errorHandler;
        private readonly SftpFileHandler _fileHandler;
        private readonly IStatusUpdateService _statusUpdateService;

        public event Action<List<DateTime>>? DatesInitialized;
        public event Action<List<int>>? DataInitialized;
        public event Action<string>? ErrorOccurred;
        public event Action<DateTime> LatestDateUpdated;

        public MainFormController(DataService dataService, FileManager fileManager, IXmlService xmlService, ErrorHandlerService errorHandler, SftpFileHandler sftpFileHandler, IStatusUpdateService statusUpdateService)
        {
            _dataService = dataService;
            _fileManager = fileManager;
            _xmlService = xmlService;
            _errorHandler = errorHandler;
            _fileHandler = sftpFileHandler;
            _statusUpdateService = statusUpdateService;
        }

        public async void InitializeDataAsync()
        {
            try
            {
                var dates = await _dataService.FetchDistinctOrderDatesAsync();
                DatesInitialized?.Invoke(dates);
            }
            catch (Exception ex)
            {
                string message = "Failed to load order dates";
                _errorHandler.LogError(ex, message, null, "Initialization");
                ErrorOccurred?.Invoke($"{message}: {ex.Message}");
            }
        }
        /// <summary>Downloads the files for PurchaseOrderDetails and PurchaseOrderHeaders asynchronous.</summary>
        public async Task DownloadFilesForPODAndPOHAsync()
        {
            try
            {
                string localBaseDirectoryPath = _fileManager.GetBaseDirectoryPath();
                string localHeadersPath = _fileManager.GetSpecificPath("headers");
                string remoteDetailsDirectoryPath = _fileManager.GetRemoteDetailsDirectoryPath();
                string remoteHeadersDirectoryPath = _fileManager.GetRemoteHeadersDirectoryPath();

                await DownloadFilesPODAsync(remoteDetailsDirectoryPath, localBaseDirectoryPath); // Download POD files
                await DownloadFilesPOHAsync(remoteHeadersDirectoryPath, localHeadersPath); // Download POH files

                // Validation
                var validatePurchaseOrderDetails = await FetchXmlDetailsDataAsync();
                var validatePurchaseOrderHeaderss = await FetchXmlHeadersDataAsync();
            }
            catch (Exception ex)
            {
                _errorHandler.LogError(ex, "Error during file download", null, "FileDownload");
                ErrorOccurred?.Invoke($"Error during file download: {ex.Message}");
            }
        }

        public async Task<List<PurchaseOrderDetailDto>> FetchXmlDetailsDataAsync()
        {
            List<PurchaseOrderDetailDto> allPurchaseOrderDetails = new List<PurchaseOrderDetailDto>();
            string localBaseDirectoryPath = _fileManager.GetBaseDirectoryPath();
            string invalidDataDirectoryPath = Path.Combine(localBaseDirectoryPath, "data_received_invalid");
            Directory.CreateDirectory(invalidDataDirectoryPath);  // Ensure the invalid directory exists

            IEnumerable<string> directories = Directory.GetDirectories(localBaseDirectoryPath, "*", SearchOption.AllDirectories);
            foreach (string dir in directories)
            {
                if (dir.EndsWith("headers")) continue;  // Skip the headers directory

                var xmlFiles = Directory.GetFiles(dir, "*.xml");
                foreach (var xmlFile in xmlFiles)
                {
                    try
                    {
                        var purchaseOrderDetails = _xmlService.LoadFromXml<PurchaseOrderDetails>(xmlFile);
                        if (purchaseOrderDetails == null)
                        {
                            MoveInvalidFile(xmlFile, invalidDataDirectoryPath);
                            continue;
                        }

                        bool hasInvalidEntries = false;
                        foreach (var detail in purchaseOrderDetails.Details)
                        {
                            ValidationResult results = new PurchaseOrderDetailValidator().Validate(detail);
                            if (!results.IsValid)
                            {
                                Log.Information($"Validation failed for {xmlFile}. Reason: {string.Join("; ", results.Errors.Select(e => e.ErrorMessage))}");
                                hasInvalidEntries = true;
                            }
                            else
                            {
                                allPurchaseOrderDetails.Add(detail);
                            }
                        }

                        if (hasInvalidEntries)
                        {
                            MoveInvalidFile(xmlFile, invalidDataDirectoryPath);
                        }
                    }
                    catch (Exception ex)
                    {
                        _errorHandler.LogError(ex, "Error loading or processing XML data", xmlFile, "XMLProcessing");
                    }
                }
            }

            return allPurchaseOrderDetails;
        }

        private void MoveInvalidFile(string sourceFilePath, string targetDirectory)
        {
            string targetPath = Path.Combine(targetDirectory, Path.GetFileName(sourceFilePath));
            File.Move(sourceFilePath, targetPath, true);
            Log.Information($"Moved invalid XML file to {targetPath}");
        }
        public async Task<List<PurchaseOrderHeaderDto>> FetchXmlHeadersDataAsync()
        {
            List<PurchaseOrderHeaderDto> allPurchaseOrderHeaders = new List<PurchaseOrderHeaderDto>();
            string localBaseDirectoryPath = _fileManager.GetBaseDirectoryPath();
            string invalidDataDirectoryPath = Path.Combine(localBaseDirectoryPath, "data_received_invalid");
            Directory.CreateDirectory(invalidDataDirectoryPath); // Ensure the invalid directory exists
            string headersDirectoryPath = Path.Combine(localBaseDirectoryPath, "headers");

            if (Directory.Exists(headersDirectoryPath)) // Check if the headers directory exists
            {
                var xmlFiles = Directory.GetFiles(headersDirectoryPath, "*.xml", SearchOption.TopDirectoryOnly);

                foreach (var xmlFile in xmlFiles)
                {
                    try
                    {
                        var purchaseOrderHeaders = _xmlService.LoadFromXml<PurchaseOrderHeaders>(xmlFile);

                        if (purchaseOrderHeaders == null)
                        {
                            MoveInvalidFile(xmlFile, invalidDataDirectoryPath);
                            continue;
                        }

                        bool hasInvalidEntries = false;

                        foreach (var header in purchaseOrderHeaders.Headers)
                        {
                            ValidationResult results = new PurchaseOrderHeaderValidator().Validate(header);
                            if (!results.IsValid)
                            {
                                Log.Information($"Validation failed for {xmlFile}. Reason: {string.Join("; ", results.Errors.Select(e => e.ErrorMessage))}");
                                hasInvalidEntries = true;
                            }
                            else
                            {
                                allPurchaseOrderHeaders.Add(header);
                            }
                        }

                        // If there are any invalid entries, move the whole file to the invalid directory
                        if (hasInvalidEntries)
                        {
                            MoveInvalidFile(xmlFile, invalidDataDirectoryPath);
                        }
                    }
                    catch (Exception ex)
                    {
                        _errorHandler.LogError(ex, "Error loading or processing XML data", xmlFile, "XMLProcessing");
                        MoveInvalidFile(xmlFile, invalidDataDirectoryPath);
                    }
                }
            }
            else
            {
                ErrorOccurred?.Invoke("Headers directory does not exist.");
                Log.Warning("Headers directory does not exist: {DirectoryPath}", headersDirectoryPath);
            }

            return allPurchaseOrderHeaders;
        }

        //public async Task<List<PurchaseOrderHeaderDto>> FetchXmlHeadersDataAsync()
        //{

        //    List<PurchaseOrderHeaderDto> allPurchaseOrderHeaders = new List<PurchaseOrderHeaderDto>();
        //    string headersDirectoryPath = Path.Combine(_fileManager.GetBaseDirectoryPath(), "headers");

        //    try
        //    {
        //        if (Directory.Exists(headersDirectoryPath))  // Check if the headers directory exists
        //        {
        //            var xmlFiles = Directory.GetFiles(headersDirectoryPath, "*.xml", SearchOption.TopDirectoryOnly);
        //            foreach (var xmlFile in xmlFiles)
        //            {
        //                try
        //                {
        //                    var purchaseOrderHeaders = _xmlService.LoadFromXml<PurchaseOrderHeaders>(xmlFile);
        //                    allPurchaseOrderHeaders.AddRange(purchaseOrderHeaders.Headers);
        //                }
        //                catch (Exception ex)
        //                {
        //                    _errorHandler.LogError(ex, "Error loading XML data", xmlFile, "XMLProcessing");
        //                }
        //            }
        //        }
        //        else
        //        {
        //            ErrorOccurred?.Invoke("Headers directory does not exist.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _errorHandler.LogError(ex, "Failed to process XML files", null, "XMLProcessing");
        //        ErrorOccurred?.Invoke($"Failed to process XML files: {ex.Message}");
        //    }

        //return allPurchaseOrderHeaders;
        //}

        public async Task<bool> SavePODetailsToDb(List<PurchaseOrderDetailDto> purchaseOrderDetailsDto)
        {
            bool isSuccess = false;

            try
            {
                isSuccess = await _dataService.AddPurchaseOrderDetailsToDbAsync(purchaseOrderDetailsDto);
                if (isSuccess)
                {
                    //DataInitialized?.Invoke(purchaseOrderHeadersDto.Select(d => d.PurchaseOrderId).ToList());
                    MessageBox.Show("Purchase Order Details saved successfully.");
                }
                else
                {
                    Log.Error("No Purchase Order Details saved.");
                }
            }
            catch (Exception ex)
            {
                _errorHandler.LogError(ex, "Error processing Purchase Order Details data", null, "Database");
                ErrorOccurred?.Invoke($"Error processing data: {ex.Message}");
            }

            return isSuccess;
        }

        public async Task<bool> SavePOHeadersToDb(List<PurchaseOrderHeaderDto> purchaseOrderHeadersDto)
        {
            bool isSuccess = false;

            try
            {
                isSuccess = await _dataService.AddPurchaseOrderHeadersToDbAsync(purchaseOrderHeadersDto);
                if (isSuccess)
                {
                    //DataInitialized?.Invoke(purchaseOrderHeadersDto.Select(d => d.PurchaseOrderId).ToList());
                    MessageBox.Show("Purchase Order Headers saved successfully.");
                }
                else
                {
                    Log.Error("No Purchase Order Headers saved.");
                }
            }
            catch (Exception ex)
            {
                _errorHandler.LogError(ex, "Error processing Purchase Order Headers data", null, "Database");
                ErrorOccurred?.Invoke($"Error processing data: {ex.Message}");
            }

            return isSuccess;
        }

        public async Task<bool> GenerateXmlFromDbAsync()  // CreatePOSXMLsButton_ClickAsync
        {
            var alreadyGeneratedIds = await AlreadyGenerated();

            var allSummaries = await _dataService.FetchPurchaseOrderSummaries();
            var summariesToGenerate = allSummaries.Where(summary => !alreadyGeneratedIds.Contains(summary.PurchaseOrderID)).ToList();

            if (summariesToGenerate.Any())
            {
                _xmlService.GenerateXMLFiles(summariesToGenerate);

                foreach (var summary in summariesToGenerate) // Update the status of the generated POs
                {
                    try
                    {
                        await _dataService.UpdatePurchaseOrderStatus(summary.PurchaseOrderID, summary.PurchaseOrderDetailID, true, false, 0);  // 0 - Auto, 1 - Custom
                    }
                    catch (Exception ex)
                    {
                        _errorHandler.LogError(ex, "Error updating purchase order status", null, "Database");
                    }
                }
                Log.Information("XML files generated successfully!");
                return true;
            }

            return false;
        }


        private async Task<List<int>> AlreadyGenerated()
        {
            return await _dataService.FetchPurchaseOrderIdGeneratedAsync();           
        }

        private async Task<HashSet<int>> AlreadySent()
        {
            return await _dataService.FetchAlreadySentPurchaseOrderIdsAsync();          
        }

        public async Task<bool> GenerateXml(DateTime? startDate = null, DateTime? endDate = null)
        {
            var alreadyGeneratedIds = await AlreadyGenerated();
            var allSummaries = await _dataService.FetchPurchaseOrderSummariesByDateAsync(startDate.GetValueOrDefault(), endDate.GetValueOrDefault());
            var summariesToGenerate = allSummaries.Where(summary => !alreadyGeneratedIds.Contains(summary.PurchaseOrderID)).ToList();

            if (!summariesToGenerate.Any())
            {
                MessageBox.Show("No PO summaries available for the given date range.");
                return false;
            }

            try
            {
                foreach (var summary in summariesToGenerate)
                {
                    await _dataService.UpdatePurchaseOrderStatus(summary.PurchaseOrderID, summary.PurchaseOrderDetailID, true, false, 1);  // Mark as processed, not sent, custom channel
                }

                _xmlService.GenerateXMLFiles(summariesToGenerate, startDate, endDate);

                string localBaseDirectoryXmlCreatedPath = _fileManager.GetBaseDirectoryXmlCreatedPath();
                MessageBox.Show($"XML file successfully created at {localBaseDirectoryXmlCreatedPath}");
                Log.Information("XML files created successfully in: {Path}", localBaseDirectoryXmlCreatedPath);

                return true;
            }
            catch (Exception ex)
            {
                _errorHandler.LogError(ex, "Error generating XML files");
                ErrorOccurred?.Invoke($"Error generating XML files: {ex.Message}");
                return false;
            }
        }

        /// <summary>Sends the date generated XML.</summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public async Task<bool> SendComboDateGeneratedXml(DateTime? startDate = null, DateTime? endDate = null)
        {
            string localBaseDirectoryXmlCreatedPath = _fileManager.GetBaseDirectoryXmlCreatedPath();
            string fileName = $"PurchaseOrderSummariesGenerated_{startDate:yyyyMMdd}_to_{endDate:yyyyMMdd}.xml";
            string filePath = Path.Combine(localBaseDirectoryXmlCreatedPath, fileName);

            // Check if file exists
            if (!File.Exists(filePath))
            {
                ErrorOccurred?.Invoke("File not found: " + filePath);
                Log.Error("File not found: {FilePath}", filePath);
                return false;
            }

            try
            {
                var orderIdsFromXml = _xmlService.ExtractPurchaseOrderIdsFromXml(filePath);
                var alreadyUploadedIds = await _dataService.FetchAlreadySentPurchaseOrderIdsAsync();

                var idsToUpload = orderIdsFromXml.Where(id => !alreadyUploadedIds.Contains(id)).ToList();

                if (!idsToUpload.Any())
                {
                    MessageBox.Show("All Purchase Orders in the file have already been sent.", "Nothing to Send", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                string remotePath = Path.Combine("/Uploaded/", fileName);
                await _fileHandler.UploadFileAsync(filePath, remotePath);

                // Update the database with the upload status for the newly uploaded IDs
                foreach (var id in idsToUpload)
                {
                    var detailIds = _xmlService.ExtractPurchaseOrderDetailIdsFromXml(filePath);
                    foreach (var detailsId in detailIds)
                    {
                        await _dataService.UpdatePurchaseOrderStatus(id, detailsId, true, true, 0);  // 0 - Auto, 1 - Custom
                    }
                }

                MessageBox.Show("File successfully sent: " + fileName, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Log.Information("File sent successfully: {FileName}", fileName);
                return true;
            }
            catch (Exception ex)
            {
                _errorHandler.LogError(ex, "Error sending XML file", null, "FileUpload");
                ErrorOccurred?.Invoke($"Error sending XML file: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendDateGeneratedXml(DateTime startDate, DateTime endDate)
        {
            // Path where the XML files are generated and will be uploaded from
            string localBaseDirectoryXmlCreatedPath = _fileManager.GetBaseDirectoryXmlCreatedPath();

            try
            {
                // Retrieve all purchase order summaries within the specified date range
                List<PurchaseOrderSummary> purchaseOrderSummaries = await _dataService.FetchPurchaseOrderSummariesByDateAsync(startDate, endDate);

                // Fetch already uploaded purchase order IDs to avoid duplicate uploads
                HashSet<int> alreadySentIds = await _dataService.FetchAlreadySentPurchaseOrderIdsAsync();

                // Filter out already sent purchase orders to get only those that need to be uploaded
                List<PurchaseOrderSummary> idsToUpload = purchaseOrderSummaries
                    .Where(summary => !alreadySentIds.Contains(summary.PurchaseOrderID))
                    .ToList();

                if (!idsToUpload.Any())
                {
                    MessageBox.Show("All Purchase Orders in the specified date range have already been sent.", "Nothing to Send", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                // Generate the XML files for the filtered summaries
                _xmlService.GenerateXMLFiles(idsToUpload);

                // Loop through the summaries and create/upload the corresponding XML files
                foreach (var summary in idsToUpload)
                {
                    // Create the file name dynamically using the PurchaseOrderID
                    string fileName = $"PurchaseOrderGenerated_{summary.PurchaseOrderID}.xml";
                    string filePath = Path.Combine(localBaseDirectoryXmlCreatedPath, fileName);

                    // Determine the remote directory path where the file will be uploaded
                    string remotePath = Path.Combine("/Uploaded/", fileName);

                    // Upload the file to the remote path
                    await _fileHandler.UploadFileAsync(filePath, remotePath);

                    // Extract the detail IDs from the generated XML file
                    List<int> detailIds = _xmlService.ExtractPurchaseOrderDetailIdsFromXml(filePath);

                    // Update the database with the upload status for each detail
                    foreach (int detailsId in detailIds)
                    {
                        await _dataService.UpdatePurchaseOrderStatus(summary.PurchaseOrderID, detailsId, true, true, 0); // 0 - Auto, 1 - Custom
                    }
                }

                MessageBox.Show("Files successfully sent for the specified date range.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Log.Information("Files sent successfully for the specified date range.");

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to send Purchase Order XML files for the specified date range.");
                MessageBox.Show($"Failed to send Purchase Order files: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>Uploads all headers.</summary>
        public async Task UploadAllHeaders()
        {
            string localBaseDirectoryPath = _fileManager.GetBaseDirectoryXmlCreatedPath();
            string remoteDirectoryPath = "/Uploaded/";

            var alreadySentIds = await AlreadySent();
            var alreadyGeneratedIds = await _dataService.FetchAlreadyGeneratedPurchaseOrderIdsAsync();

            DateTime? latestDate = null;

            try
            {
                foreach (var orderId in alreadyGeneratedIds)
                {
                    string fileName = $"PurchaseOrderGenerated_{orderId}.xml";
                    string filePath = Path.Combine(localBaseDirectoryPath, fileName);

                    if (!File.Exists(filePath))
                    {
                        Log.Warning($"File not found: {filePath}");
                        continue;
                    }

                    var orderIdsFromXml = _xmlService.ExtractPurchaseOrderIdsFromXml(filePath);
                    foreach (var id in orderIdsFromXml)
                    {
                        if (alreadySentIds.Contains(id))
                        {
                            Log.Information($"PurchaseOrderId already sent: {id}");
                            continue;
                        }

                        string remoteFilePath = Path.Combine(remoteDirectoryPath, fileName);
                        await _fileHandler.UploadFileAsync(filePath, remoteFilePath);

                        var detailIds = _xmlService.ExtractPurchaseOrderDetailIdsFromXml(filePath);
                        foreach (var detailsId in detailIds)
                        {
                            await _dataService.UpdatePurchaseOrderStatus(id, detailsId, true, true, 0);
                        }
                        Log.Information($"PurchaseOrderId sent: {id}");
                    }

                    DateTime? fileDate = await _dataService.GetLatestDateForPurchaseOrder(orderId);
                    if (fileDate.HasValue && (latestDate == null || fileDate > latestDate))
                    {
                        latestDate = fileDate;
                        LatestDateUpdated?.Invoke(fileDate.Value);
                    }
                }
                _statusUpdateService.RaiseStatusUpdated("All headers have been uploaded successfully.");
            }
            catch (Exception ex)
            {
                _errorHandler.LogError(ex, "Error uploading files", null, "FileUpload");
                ErrorOccurred?.Invoke("Failed to upload files: " + ex.Message);
            }
        }


        public async Task<List<DateTime>> FetchDistinctOrderDatesAsync()
        {
            return await _dataService.FetchDistinctOrderDatesAsync();
        }

        public async Task DownloadFilesPODAsync(string remotePath, string localPath)
        {
            bool shouldRetry = true;

            while (shouldRetry)
            {
                try
                {
                    if (_fileHandler == null)
                        throw new InvalidOperationException("File handler is not initialized.");

                    bool filesDownloaded = await _fileHandler.DownloadXmlFilesFromDirectoryAsync(remotePath, localPath);

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

                    shouldRetry = false;
                }
                catch (Exception ex)
                {
                    _errorHandler.LogError(ex, "Failed to download files", null, "FileDownload");
                    ErrorOccurred?.Invoke($"Failed to download files: {ex.Message}");

                    // Display a message box to let the user choose whether to retry or exit
                    DialogResult result = MessageBox.Show(
                        "Failed to download files. Do you want to retry or exit the application?",
                        "Download Error",
                        MessageBoxButtons.RetryCancel,
                        MessageBoxIcon.Error);

                    if (result == DialogResult.Cancel)
                    {
                        // Exit the application if the user chooses to cancel
                        Application.Exit();
                        return;
                    }
                }
            }
        }

        public async Task DownloadFilesPOHAsync(string remotePath, string localPath)
        {
            bool shouldRetry = true;

            while (shouldRetry)
            {
                try
                {
                    if (_fileHandler == null)
                        throw new InvalidOperationException("File handler is not initialized.");

                    bool filesDownloaded = await _fileHandler.DownloadXmlFilesFromDirectoryAsync(remotePath, localPath);

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

                    // Exit the loop after a successful download
                    shouldRetry = false;
                }
                catch (Exception ex)
                {
                    _errorHandler.LogError(ex, "Failed to download files", null, "FileDownload");
                    ErrorOccurred?.Invoke($"Failed to download files: {ex.Message}");

                    // Display a message box to let the user choose whether to retry or exit
                    DialogResult result = MessageBox.Show(
                        "Failed to download files. Do you want to retry or exit the application?",
                        "Download Error",
                        MessageBoxButtons.RetryCancel,
                        MessageBoxIcon.Error);

                    if (result == DialogResult.Cancel)
                    {
                        // Exit the application if the user chooses to cancel
                        Application.Exit();
                        return;
                    }
                }
            }
        }

        public async Task<List<PurchaseOrderSummary>> SearchPurchaseOrdersWithinDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var purchaseOrders = await _dataService.FetchPurchaseOrderSummariesByDateAsync(startDate, endDate);

            return purchaseOrders;
        }   

    }
}
