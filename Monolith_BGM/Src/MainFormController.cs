using BGM.Common;
using BGM.SftpUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.Logging;
using Monolith_BGM.DataAccess.DTO;
using Monolith_BGM.Src;
using Monolith_BGM.XMLService;
using System;
using System.Collections.Generic;
using Log = Serilog.Log;

namespace Monolith_BGM.Controllers
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
                ErrorOccurred?.Invoke($"Failed to load order dates: {ex.Message}");
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
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke($"Error during file download: {ex.Message}");
                Log.Error(ex, "Error during file operations");
            }
        }

        public async Task<List<PurchaseOrderDetailDto>> FetchXmlDetailsDataAsync()
        {
            return await Task.Run(() =>
            {
                List<PurchaseOrderDetailDto> allPurchaseOrderDetails = new List<PurchaseOrderDetailDto>();
                string localBaseDirectoryPath = _fileManager.GetBaseDirectoryPath();

                try
                {
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
                                allPurchaseOrderDetails.AddRange(purchaseOrderDetails.Details);
                            }
                            catch (Exception ex)
                            {
                                ErrorOccurred?.Invoke($"Failed to load order details: {ex.Message}");
                                _errorHandler.LogError(ex, "Error loading XML data", xmlFile);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorOccurred?.Invoke($"Failed to process XML files: {ex.Message}");
                }

                return allPurchaseOrderDetails;
            });
        }

        public async Task<List<PurchaseOrderHeaderDto>> FetchXmlHeadersDataAsync()
        {
            return await Task.Run(() =>
            {
                List<PurchaseOrderHeaderDto> allPurchaseOrderHeaders = new List<PurchaseOrderHeaderDto>();
                string headersDirectoryPath = Path.Combine(_fileManager.GetBaseDirectoryPath(), "headers");

                try
                {
                    if (Directory.Exists(headersDirectoryPath))  // Check if the headers directory exists
                    {
                        var xmlFiles = Directory.GetFiles(headersDirectoryPath, "*.xml", SearchOption.TopDirectoryOnly);
                        foreach (var xmlFile in xmlFiles)
                        {
                            try
                            {
                                var purchaseOrderHeaders = _xmlService.LoadFromXml<PurchaseOrderHeaders>(xmlFile);
                                allPurchaseOrderHeaders.AddRange(purchaseOrderHeaders.Headers);
                            }
                            catch (Exception ex)
                            {
                                //ErrorOccurred?.Invoke($"Failed to load order headers: {ex.Message}");
                                Log.Error(ex, "Error loading XML data: {XmlFile}", xmlFile);
                                _errorHandler.LogError(ex, "Error loading XML data", xmlFile);
                            }
                        }
                    }
                    else
                    {
                        ErrorOccurred?.Invoke("Headers directory does not exist.");
                    }
                }
                catch (Exception ex)
                {
                    ErrorOccurred?.Invoke($"Failed to process XML files: {ex.Message}");
                }

                return allPurchaseOrderHeaders;
            });
        }


        public async Task<bool> FetchAndSavePODetails(List<PurchaseOrderDetailDto> purchaseOrderDetailsDto)
        {
            try
            {
                var isSuccess = await _dataService.AddPurchaseOrderDetailsAsync(purchaseOrderDetailsDto);
                if (isSuccess)
                {
                    DataInitialized?.Invoke(purchaseOrderDetailsDto.Select(d => d.PurchaseOrderDetailId).ToList());
                    return true;
                }
                else
                {
                    Log.Error("No Purchase Order details saved.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke($"Error processing data: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> FetchAndSavePOHeaders(List<PurchaseOrderHeaderDto> purchaseOrderHeadersDto)
        {
            try
            {
                var isSuccess = await _dataService.AddPurchaseOrderHeadersAsync(purchaseOrderHeadersDto);
                if (isSuccess)
                {
                    DataInitialized?.Invoke(purchaseOrderHeadersDto.Select(d => d.PurchaseOrderId).ToList());
                    return true;
                }
                else
                {
                    Log.Error("No Purchase Order Headers saved.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke($"Error processing data: {ex.Message}");
                return false;
            }
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
                    await _dataService.UpdatePurchaseOrderStatus(summary.PurchaseOrderID, true, false, 0);  // 0 - Auto, 1 - Custom
                }
                Log.Information("XML files generated successfully!");
                return true;
            }

            return false;
        }

        private async Task<List<int>> AlreadyGenerated()
        {
            var generated = await _dataService.FetchPurchaseOrderIdGeneratedAsync();
            return generated;
        }

        private async Task<HashSet<int>> AlreadySent()
        {
            var sent = await _dataService.FetchAlreadySentPurchaseOrderIdsAsync();
            return sent;
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
                    await _dataService.UpdatePurchaseOrderStatus(summary.PurchaseOrderID, true, false, 1);  // Mark as processed, not sent, custom channel
                }

                _xmlService.GenerateXMLFiles(summariesToGenerate, startDate, endDate);

                string localBaseDirectoryXmlCreatedPath = _fileManager.GetBaseDirectoryXmlCreatedPath();
                MessageBox.Show($"XML file successfully created at {localBaseDirectoryXmlCreatedPath}");
                Log.Information("XML files created successfully in: {Path}", localBaseDirectoryXmlCreatedPath);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error generating XML files for dates {StartDate} to {EndDate}", startDate, endDate);
                ErrorOccurred?.Invoke($"Error generating XML files: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendDateGeneratedXml(DateTime? startDate = null, DateTime? endDate = null)
        {
            string localBaseDirectoryXmlCreatedPath = _fileManager.GetBaseDirectoryXmlCreatedPath();
            string fileName = $"PurchaseOrderSummariesGenerated_{startDate:yyyyMMdd}_to_{endDate:yyyyMMdd}.xml";
            string filePath = Path.Combine(localBaseDirectoryXmlCreatedPath, fileName);

            try
            {
                // Check if file exists
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("File not found: " + filePath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Log.Error("File not found: {FilePath}", filePath);
                    return false;
                }

                // Extract PurchaseOrderIDs from the XML
                var orderIdsFromXml = _xmlService.ExtractPurchaseOrderIdsFromXml(filePath);

                // Fetch already uploaded PurchaseOrderIDs
                var alreadyUploadedIds = await _dataService.FetchAlreadySentPurchaseOrderIdsAsync(); //FetchAlreadySentPurchaseOrderIdsAsync();

                // Determine which IDs need to be uploaded
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
                    await _dataService.UpdatePurchaseOrderStatus(id, true, true, 0);  // 0 - Auto, 1 - Custom
                }

                MessageBox.Show("File successfully sent: " + fileName, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Log.Information("File sent successfully: {FileName}", fileName);
                return true;
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke($"Error sending XML file: {ex.Message}");
                Log.Error(ex, "Error sending XML file");
                return false;
            }
        }


        public async void UploadAllHeaders()
        {
            string localBaseDirectoryPath = _fileManager.GetBaseDirectoryPath();
            string localDirectoryPath = _fileManager.GetSpecificPath("Headers");
            string remoteDirectoryPath = "/Uploaded/";

            var alreadySentIds = await AlreadySent();

            DateTime? latestDate = null;

            try
            {
                DirectoryInfo dir = new DirectoryInfo(localDirectoryPath);
                FileInfo[] files = dir.GetFiles("PurchaseOrderHeader*.xml");

                foreach (FileInfo file in files)
                {
                    int purchaseOrderId = int.Parse(Path.GetFileNameWithoutExtension(file.Name).Replace("PurchaseOrderHeader", ""));
                    if (alreadySentIds.Contains(purchaseOrderId))
                    {
                        Log.Information($"Skipping upload for {file.Name} as it's already sent.");
                        continue;  // Skip this file if it's already sent
                    }

                    string localFilePath = file.FullName;
                    string remoteFilePath = Path.Combine(remoteDirectoryPath, file.Name);
                    await _fileHandler.UploadFileAsync(localFilePath, remoteFilePath);
                    Log.Information($"Uploaded {file.Name} to {remoteFilePath}");

                    // Update the database with the upload status
                    await _dataService.UpdatePurchaseOrderStatus(purchaseOrderId, true, true, 0); // Mark as sent + processed

                    // Optionally update UI or handle latest date
                    DateTime? fileDate = await _dataService.GetLatestDateForPurchaseOrder(purchaseOrderId);
                    if (fileDate.HasValue && (latestDate == null || fileDate > latestDate))
                    {
                        latestDate = fileDate;
                        // Instead of Invoke, use an event to update UI in MainForm
                        LatestDateUpdated?.Invoke(fileDate.Value);
                    }
                }

                MessageBox.Show("All applicable files have been successfully uploaded.", "Upload Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error uploading files");
                MessageBox.Show("Failed to upload files: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public async Task<List<DateTime>> FetchDistinctOrderDatesAsync()
        {
            return await _dataService.FetchDistinctOrderDatesAsync();
        }

        public async Task DownloadFilesPODAsync(string remotePath, string localPath)
        {
            try
            {
                if (_fileHandler == null)
                    throw new InvalidOperationException("File handler is not initialized.");

                bool filesDownloaded = await _fileHandler.DownloadXmlFilesFromDirectoryAsync(remotePath, localPath);
                if (filesDownloaded)
                {
                    //StatusUpdated?.Invoke($"XML files from {remotePath} have been downloaded successfully!");
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
                ErrorOccurred?.Invoke($"Failed to download files: {ex.Message}");
                Log.Error(ex, "Error downloading files.");
            }
        }

        public async Task DownloadFilesPOHAsync(string remotePath, string localPath)
        {
            try
            {
                if (_fileHandler == null)
                    throw new InvalidOperationException("File handler is not initialized.");

                bool filesDownloaded = await _fileHandler.DownloadXmlFilesFromDirectoryAsync(remotePath, localPath);
                if (filesDownloaded)
                {
                    //StatusUpdated?.Invoke($"XML files from {remotePath} have been downloaded successfully!");
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
                ErrorOccurred?.Invoke($"Failed to download files: {ex.Message}");
                Log.Error(ex, "Error downloading files.");
            }
        }
    }
}
