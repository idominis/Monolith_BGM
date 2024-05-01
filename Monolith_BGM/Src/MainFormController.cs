using BGM.Common;
using Monolith_BGM.DataAccess.DTO;
using Monolith_BGM.Src;
using Monolith_BGM.XMLService;
using System;
using System.Collections.Generic;

namespace Monolith_BGM.Controllers
{
    public class MainFormController
    {
        private readonly DataService _dataService;
        private readonly FileManager _fileManager;
        private readonly IXmlService _xmlService;
        private readonly ErrorHandlerService _errorHandler;

        public event Action<List<DateTime>>? DatesInitialized;
        public event Action<List<int>>? DataInitialized;
        public event Action<string>? ErrorOccurred;

        public MainFormController(DataService dataService, FileManager fileManager, IXmlService xmlService, ErrorHandlerService errorHandler)
        {
            _dataService = dataService;
            _fileManager = fileManager;
            _xmlService = xmlService;
            _errorHandler = errorHandler;
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

        public async Task<List<PurchaseOrderDetailDto>> FetchXmlDataAsync()
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
                    ErrorOccurred?.Invoke("Failed to save order details.");
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
                    ErrorOccurred?.Invoke("Failed to save order headers.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke($"Error processing data: {ex.Message}");
                return false;
            }
        }
    }
}
