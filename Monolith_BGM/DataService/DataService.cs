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

    public DataService(BGM_dbContext dbContext, IMapper mapper, ErrorHandlerService errorHandler)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _errorHandler = errorHandler;
    }

    public async Task AddPurchaseOrderDetailsAsync(List<PurchaseOrderDetailDto> purchaseOrderDetailsDto)
    {
        // Clear the DbContext to avoid tracking issues
        _dbContext.ChangeTracker.Clear();

        // Map DTOs to Entity Models
        var purchaseOrderDetails = purchaseOrderDetailsDto.Select(dto => _mapper.Map<PurchaseOrderDetail>(dto)).ToList();

        foreach (var detail in purchaseOrderDetails)
        {
            // Attempt to fetch the entity from the database to handle updates if exists
            var existingEntity = await _dbContext.PurchaseOrderDetails
                .FindAsync(detail.PurchaseOrderId, detail.PurchaseOrderDetailId);

            if (existingEntity == null)
            {
                // If the entity doesn't exist, add it
                _dbContext.PurchaseOrderDetails.Add(detail);
            }
            else
            {
                // If it exists, update the existing entity with new values
                _dbContext.Entry(existingEntity).CurrentValues.SetValues(detail);
            }
        }

        // Save changes to the database
        try
        {
            if (_dbContext.ChangeTracker.HasChanges())
            {
                await _dbContext.SaveChangesAsync();
                MessageBox.Show("Purchase orders loaded and saved successfully!");
                Log.Information("Purchase orders loaded and saved successfully!");
            }
            else
            {
                MessageBox.Show("No new purchase orders to save.");
                Log.Information("No new purchase orders to save.");
            }
        }
        catch (DbUpdateException ex)
        {
            // Log and handle the exception appropriately
            _errorHandler.LogError(ex, "Failed to update the database.");
            MessageBox.Show("Failed to update the database: " + ex.Message);
        }
    }
}
