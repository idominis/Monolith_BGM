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

    public async Task <bool> AddPurchaseOrderHeadersAsync(List<PurchaseOrderHeaderDto> purchaseOrderHeadersDto)
    {
        // Fetch existing IDs from the database
        var existingIds = new HashSet<int>(_dbContext.PurchaseOrderHeaders.Select(p => p.PurchaseOrderId));

        // Filter DTOs first before mapping
        var filteredDtos = purchaseOrderHeadersDto.Where(dto => !existingIds.Contains(dto.PurchaseOrderId)).ToList();
        var allHeaders = filteredDtos.Select(dto => _mapper.Map<PurchaseOrderHeader>(dto)).ToList();

        // Filter out details that already exist in the database
        var newDetails = allHeaders.Where(d => !existingIds.Contains(d.PurchaseOrderId)).ToList();

        // Add and save new entries
        if (newDetails.Any())
        {
            _dbContext.PurchaseOrderHeaders.AddRange(newDetails);
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

    public async Task PerformOperation()
    {
        try
        {
            // Operation logic
            OnStatusUpdated("Operation successful");
        }
        catch (Exception ex)
        {
            OnStatusUpdated("Operation failed: " + ex.Message);
        }
    }
}
