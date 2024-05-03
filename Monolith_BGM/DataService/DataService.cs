using AutoMapper;
using Monolith_BGM.DataAccess.DTO;
using Monolith_BGM.Models;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BGM.Common;

public class DataService
{
    private readonly BGM_dbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ErrorHandlerService _errorHandler;
    public delegate void StatusUpdateHandler(string message);
    public event StatusUpdateHandler StatusUpdated;
    private readonly IStatusUpdateService _statusUpdateService;

    public DataService(BGM_dbContext dbContext, IMapper mapper, ErrorHandlerService errorHandler, IStatusUpdateService statusUpdateService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _errorHandler = errorHandler;
        _statusUpdateService = statusUpdateService;
    }


    public async Task <bool> AddPurchaseOrderDetailsAsync(List<PurchaseOrderDetailDto> purchaseOrderDetailsDto)
    {
        // Fetch existing IDs from the database
        var existingIds = new HashSet<int>(_dbContext.PurchaseOrderDetails.Select(p => p.PurchaseOrderDetailId));

        // Filter DTOs first before mapping
        var filteredDtos = purchaseOrderDetailsDto.Where(dto => !existingIds.Contains(dto.PurchaseOrderDetailId)).ToList();
        var allDetails = filteredDtos.Select(dto => _mapper.Map<PurchaseOrderDetail>(dto)).ToList();

        // Filter out details that already exist in the database
        var newDetails = allDetails.Where(d => !existingIds.Contains(d.PurchaseOrderDetailId)).ToList();

        // Add and save new entries
        if (newDetails.Any())
        {
            _dbContext.PurchaseOrderDetails.AddRange(newDetails);
            await _dbContext.SaveChangesAsync();
            Log.Information("Purchase orders details loaded and saved successfully!");
            return true;
        }
        else
        {
            Log.Information("No new purchase orders details to save.");
            return false;
        }
    }

    public async Task<bool> AddPurchaseOrderHeadersAsync(List<PurchaseOrderHeaderDto> purchaseOrderHeadersDto)
{
    _dbContext.ChangeTracker.Clear(); // Clear the change tracker to avoid conflicts

    // Fetch existing IDs from the database to avoid attaching duplicates
    var existingIds = await _dbContext.PurchaseOrderHeaders
                                      .AsNoTracking()
                                      .Select(p => p.PurchaseOrderId)
                                      .ToListAsync();

    var newHeaders = new List<PurchaseOrderHeader>();

    foreach (var dto in purchaseOrderHeadersDto)
    {
        if (!existingIds.Contains(dto.PurchaseOrderId))
        {
            var newHeader = _mapper.Map<PurchaseOrderHeader>(dto);
            newHeaders.Add(newHeader);
        }
    }

    if (newHeaders.Any())
    {
            //_dbContext.PurchaseOrderHeaders.AddRange(newHeaders);
            for (int i = 0; i < newHeaders.Count; i++)
            {
                _dbContext.PurchaseOrderHeaders.Add(newHeaders[i]); // TESTING PURPOSES
                await _dbContext.SaveChangesAsync();
            }

            await _dbContext.SaveChangesAsync();
        Log.Information("Purchase orders headers loaded and saved successfully!");
        return true;
    }
    else
    {
        Log.Information("No new purchase orders headers to save.");
        return false;
    }
}


    protected virtual void OnStatusUpdated(string message)
    {
        StatusUpdated?.Invoke(message);
    }

    public async Task<List<PurchaseOrderSummary>> FetchPurchaseOrderSummaries()
    {
        // Fetch data from the database view
        var viewData = await _dbContext.VPurchaseOrderSummaries.ToListAsync();

        // Map data from view model to DTO
        return _mapper.Map<List<PurchaseOrderSummary>>(viewData);
    }

    public async Task<List<DateTime>> FetchDistinctOrderDatesAsync()
    {
        return await _dbContext.VPurchaseOrderSummaries
                               .Select(x => x.OrderDate.Date)
                               .Distinct()
                               .OrderBy(date => date)
                               .ToListAsync();
    }

    public async Task<List<PurchaseOrderSummary>> FetchPurchaseOrderSummariesByDateAsync(DateTime startDate, DateTime endDate)
    {
        var viewData = await _dbContext.VPurchaseOrderSummaries
            .Where(p => p.OrderDate >= startDate && p.OrderDate <= endDate)
            .ToListAsync();

        // Map data from entity to DTO
        return _mapper.Map<List<PurchaseOrderSummary>>(viewData);
    }

    public async Task<DateTime?> GetLatestDateForPurchaseOrder(int purchaseOrderId)
    {
        var latestDate = await _dbContext.VPurchaseOrderSummaries
                                          .Where(x => x.PurchaseOrderId == purchaseOrderId)
                                          .Select(x => x.OrderDate)
                                          .FirstOrDefaultAsync();

        return latestDate;
    }

    public async Task UpdatePurchaseOrderStatus(int purchaseOrderId, int purchaseOrderDetailId, bool processed, bool sent, int channel)
    {
        var orderSentDto = new PurchaseOrdersProcessedSent
        {
            PurchaseOrderId = purchaseOrderId,
            PurchaseOrderDetailId = purchaseOrderDetailId,
            OrderProcessed = processed,
            OrderSent = sent,
            Channel = channel,
            ModifiedDate = DateTime.Now
        };

        // Map the DTO to the entity
        var orderSentEntry = _mapper.Map<PurchaseOrdersProcessedSent>(orderSentDto);

        _dbContext.PurchaseOrdersProcessedSents.Add(orderSentEntry);
        await _dbContext.SaveChangesAsync();
    }


    public async Task<List<int>> FetchPurchaseOrderIdGeneratedAsync()
    {
        return await _dbContext.PurchaseOrdersProcessedSents
                               .Where(x => x.OrderProcessed)  // Filter for OrderProcessed = true
                               .Select(x => x.PurchaseOrderId)
                               .Distinct()
                               .OrderBy(id => id)  // Order by PurchaseOrderId
                               .ToListAsync();
    }
    public async Task<HashSet<int>> FetchAlreadyGeneratedPurchaseOrderIdsAsync()
    {
        return new HashSet<int>(await _dbContext.PurchaseOrdersProcessedSents
            .Where(x => x.OrderProcessed) // Filter for OrderProcessed = true
            .Select(x => x.PurchaseOrderId)
            .ToListAsync());
    }

    public async Task<HashSet<int>> FetchAlreadySentPurchaseOrderIdsAsync()
    {
        return new HashSet<int>(await _dbContext.PurchaseOrdersProcessedSents
            .Where(x => x.OrderSent) // Filter for OrderSent = true
            .Select(x => x.PurchaseOrderId)
            .ToListAsync());
    }

    public async Task<List<int>> FetchAlreadySentPurchaseOrderIdsAsync(List<int> purchaseOrderIds)
    {
        var sentIds = await _dbContext.PurchaseOrdersProcessedSents
                                      .Where(x => x.OrderSent && purchaseOrderIds.Contains(x.PurchaseOrderId))
                                      .Select(x => x.PurchaseOrderId)
                                      .Distinct()
                                      .ToListAsync();
        return sentIds;
    }


}
