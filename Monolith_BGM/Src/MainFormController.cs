using BGM.Common;
using BGM.SftpUtilities;
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

        public event Action<List<DateTime>>? DatesInitialized;
        public event Action<List<int>>? DataInitialized;
        public event Action<string>? ErrorOccurred;

        public MainFormController(DataService dataService, FileManager fileManager, IXmlService xmlService, ErrorHandlerService errorHandler, SftpFileHandler sftpFileHandler)
        {
            _dataService = dataService;
            _fileManager = fileManager;
            _xmlService = xmlService;
            _errorHandler = errorHandler;
            _fileHandler = sftpFileHandler;
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

        private async Task<List<int>> AlreadySent()
        {
            var sent = await _dataService.FetchPurchaseOrderIdSentAsync();
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

            try
            {
                string fileName = $"PurchaseOrderSummariesGenerated_{startDate:yyyyMMdd}_to_{endDate:yyyyMMdd}.xml";

                // Full path to the file
                string filePath = Path.Combine(localBaseDirectoryXmlCreatedPath, fileName);

                // Construct the remote path
                string remotePath = Path.Combine("/Uploaded/", fileName);

                if (File.Exists(filePath))
                {
                    // Use SftpFileHandler to upload the file to a specific folder on the server
                    await _fileHandler.UploadFileAsync(filePath, remotePath);
                    MessageBox.Show("File successfully sent: " + fileName, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Log.Information("File sent successfully: {FileName}", fileName);
                }
                else
                {
                    MessageBox.Show("File not found: " + filePath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Log.Error("File not found: {FilePath}", filePath);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke($"Error generating XML files: {ex.Message}");
                Log.Error(ex, "Error generating XML files");
                return false;
            }


        }
    }
}
