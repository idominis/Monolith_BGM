using AutoMapper;
using Monolith_BGM.DataAccess.DTO;
using Monolith_BGM.Models;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BGM.Common;
using Microsoft.EntityFrameworkCore.Internal;
using System.Configuration;
using System.Reflection.PortableExecutable;

/// <summary>
/// 
/// </summary>
public class DataService
{
    /// <summary>
    /// The database context
    /// </summary>
    private readonly BGM_dbContext _dbContext;
    /// <summary>
    /// The context factory
    /// </summary>
    private readonly IDbContextFactory<BGM_dbContext> _contextFactory;
    /// <summary>
    /// The mapper
    /// </summary>
    private readonly IMapper _mapper;
    /// <summary>
    /// The error handler
    /// </summary>
    private readonly ErrorHandlerService _errorHandler;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="message">The message.</param>
    public delegate void StatusUpdateHandler(string message);
    /// <summary>
    /// Occurs when [status updated].
    /// </summary>
    public event StatusUpdateHandler StatusUpdated;
    /// <summary>
    /// The status update service
    /// </summary>
    private readonly IStatusUpdateService _statusUpdateService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataService"/> class.
    /// </summary>
    /// <param name="dbContext">The database context.</param>
    /// <param name="contextFactory">The context factory.</param>
    /// <param name="mapper">The mapper.</param>
    /// <param name="errorHandler">The error handler.</param>
    /// <param name="statusUpdateService">The status update service.</param>
    public DataService(BGM_dbContext dbContext, IDbContextFactory<BGM_dbContext> contextFactory, IMapper mapper, ErrorHandlerService errorHandler, IStatusUpdateService statusUpdateService)
    {
        _dbContext = dbContext;
        _contextFactory = contextFactory;
        _mapper = mapper;
        _errorHandler = errorHandler;
        _statusUpdateService = statusUpdateService;
    }

    /// <summary>
    /// Adds the purchase order details to database asynchronous.
    /// </summary>
    /// <param name="purchaseOrderDetailsDto">The purchase order details dto.</param>
    /// <returns></returns>
    public async Task<bool> AddPurchaseOrderDetailsToDbAsync(List<PurchaseOrderDetailDto> purchaseOrderDetailsDto)
    {
        bool isSuccess = false;
        string logMessage;

        using var dbContext = _contextFactory.CreateDbContext();  // Create a new DbContext instance
        var existingIds = new HashSet<int>(dbContext.PurchaseOrderDetails.Select(p => p.PurchaseOrderDetailId));

        var newDetails = purchaseOrderDetailsDto
            .Where(dto => !existingIds.Contains(dto.PurchaseOrderDetailId))
            .Select(dto => _mapper.Map<PurchaseOrderDetail>(dto))
            .ToList();

        if (newDetails.Any())
        {
            dbContext.PurchaseOrderDetails.AddRange(newDetails);

            try
            {
                dbContext.SaveChanges();
                string detailIds = string.Join(", ", newDetails.Select(h => h.PurchaseOrderDetailId));
                logMessage = string.Format("Purchase order details: {0} loaded and saved successfully!", detailIds);
                isSuccess = true;
            }
            catch (DbUpdateException ex)
            {
                logMessage = "Error updating the database with new purchase order details.";
                Log.Error(ex, logMessage);
            }
        }
        else
        {
            logMessage = "No new purchase orders details to save.";
        }

        Log.Information(logMessage);
        return isSuccess;
    }

    /// <summary>
    /// Adds the purchase order headers to database asynchronous.
    /// </summary>
    /// <param name="purchaseOrderHeadersDto">The purchase order headers dto.</param>
    /// <returns></returns>
    public async Task<bool> AddPurchaseOrderHeadersToDbAsync(List<PurchaseOrderHeaderDto> purchaseOrderHeadersDto)
    {
        bool isSuccess = false;
        string logMessage;

        using var dbContext = _contextFactory.CreateDbContext();  // Create a new DbContext instance
        var existingIds = new HashSet<int>(dbContext.PurchaseOrderHeaders.Select(p => p.PurchaseOrderId));

        var newHeaders = purchaseOrderHeadersDto
            .Where(dto => !existingIds.Contains(dto.PurchaseOrderId))
            .Select(dto => _mapper.Map<PurchaseOrderHeader>(dto))
            .ToList();

        if (newHeaders.Any())
        {
            dbContext.PurchaseOrderHeaders.AddRange(newHeaders);

            try
            {
                dbContext.SaveChanges();
                string headerIds = string.Join(", ", newHeaders.Select(h => h.PurchaseOrderId));
                logMessage = string.Format("Purchase order headers: {0} loaded and saved successfully!", headerIds);
                isSuccess = true;
            }
            catch (DbUpdateException ex)
            {
                logMessage = "Error updating the database with new purchase order headers.";
                Log.Error(ex, logMessage);
            }
        }
        else
        {
            logMessage = "No new purchase orders headers to save.";
        }

        Log.Information(logMessage);
        return isSuccess;
    }

    /// <summary>
    /// Called when [status updated].
    /// </summary>
    /// <param name="message">The message.</param>
    protected virtual void OnStatusUpdated(string message)
    {
        StatusUpdated?.Invoke(message);
    }

    /// <summary>
    /// Fetches the purchase order summaries.
    /// </summary>
    /// <returns></returns>
    public async Task<List<PurchaseOrderSummary>> FetchPurchaseOrderSummaries()
    {
        var viewData = await _dbContext.VPurchaseOrderSummaries.ToListAsync();

        return _mapper.Map<List<PurchaseOrderSummary>>(viewData);
    }

    /// <summary>
    /// Fetches the distinct order dates asynchronous.
    /// </summary>
    /// <returns></returns>
    public async Task<List<DateTime>> FetchDistinctOrderDatesAsync()
    {
        return await _dbContext.VPurchaseOrderSummaries
                               .Select(x => x.OrderDate.Date)
                               .Distinct()
                               .OrderBy(date => date)
                               .ToListAsync();
    }

    /// <summary>
    /// Fetches the purchase order summaries by date asynchronous.
    /// </summary>
    /// <param name="startDate">The start date.</param>
    /// <param name="endDate">The end date.</param>
    /// <returns></returns>
    public async Task<List<PurchaseOrderSummary>> FetchPurchaseOrderSummariesByDateAsync(DateTime startDate, DateTime endDate)
    {
        var viewData = await _dbContext.VPurchaseOrderSummaries
            .Where(p => p.OrderDate >= startDate && p.OrderDate <= endDate)
            .ToListAsync();

        return _mapper.Map<List<PurchaseOrderSummary>>(viewData);
    }

    /// <summary>
    /// Gets the latest date for purchase order.
    /// </summary>
    /// <param name="purchaseOrderId">The purchase order identifier.</param>
    /// <returns></returns>
    public async Task<DateTime?> GetLatestDateForPurchaseOrder(int purchaseOrderId)
    {
        var latestDate = await _dbContext.VPurchaseOrderSummaries
                                         .Where(x => x.PurchaseOrderId == purchaseOrderId)
                                         .Select(x => x.OrderDate)
                                         .FirstOrDefaultAsync();

        return latestDate;
    }

    /// <summary>
    /// Updates the purchase order status.
    /// </summary>
    /// <param name="purchaseOrderId">The purchase order identifier.</param>
    /// <param name="purchaseOrderDetailId">The purchase order detail identifier.</param>
    /// <param name="processed">if set to <c>true</c> [processed].</param>
    /// <param name="sent">if set to <c>true</c> [sent].</param>
    /// <param name="channel">The channel.</param>
    public async Task UpdatePurchaseOrderStatus(int purchaseOrderId, int purchaseOrderDetailId, bool processed, bool sent, int channel)
    {
        using var dbContext = _contextFactory.CreateDbContext();  // Create a new DbContext instance

        var existingEntity = await dbContext.PurchaseOrdersProcessedSents
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.PurchaseOrderDetailId == purchaseOrderDetailId);

        if (existingEntity != null)
        {
            dbContext.PurchaseOrdersProcessedSents.Attach(existingEntity);
            existingEntity.OrderProcessed = processed;
            existingEntity.OrderSent = sent;
            existingEntity.Channel = channel;
            existingEntity.ModifiedDate = DateTime.Now;
        }
        else
        {
            var newEntity = new PurchaseOrdersProcessedSent
            {
                PurchaseOrderId = purchaseOrderId,
                PurchaseOrderDetailId = purchaseOrderDetailId,
                OrderProcessed = processed,
                OrderSent = sent,
                Channel = channel,
                ModifiedDate = DateTime.Now
            };
            dbContext.PurchaseOrdersProcessedSents.Add(newEntity);
        }

        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Fetches the purchase order identifier generated asynchronous.
    /// </summary>
    /// <returns></returns>
    public async Task<List<int>> FetchPurchaseOrderIdGeneratedAsync()
    {
        return await _dbContext.PurchaseOrdersProcessedSents
                               .Where(x => x.OrderProcessed)  // Filter for OrderProcessed = true
                               .Select(x => x.PurchaseOrderId)
                               .Distinct()
                               .OrderBy(id => id)  // Order by PurchaseOrderId
                               .ToListAsync();
    }
    /// <summary>
    /// Fetches the already generated purchase order ids asynchronous.
    /// </summary>
    /// <returns></returns>
    public async Task<HashSet<int>> FetchAlreadyGeneratedPurchaseOrderIdsAsync()
    {
        return new HashSet<int>(await _dbContext.PurchaseOrdersProcessedSents
            .Where(x => x.OrderProcessed) // Filter for OrderProcessed = true
            .Select(x => x.PurchaseOrderId)
            .ToListAsync());
    }

    /// <summary>
    /// Fetches the already sent purchase order ids asynchronous.
    /// </summary>
    /// <returns></returns>
    public async Task<HashSet<int>> FetchAlreadySentPurchaseOrderIdsAsync()
    {
        return new HashSet<int>(await _dbContext.PurchaseOrdersProcessedSents
            .Where(x => x.OrderSent) // Filter for OrderSent = true
            .Select(x => x.PurchaseOrderId)
            .ToListAsync());
    }

    /// <summary>
    /// Fetches the already sent purchase order ids asynchronous.
    /// </summary>
    /// <param name="purchaseOrderIds">The purchase order ids.</param>
    /// <returns></returns>
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
