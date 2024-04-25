using AutoMapper;
using Monolith_BGM.DataAccess.DTO;
using Monolith_BGM.Models;
using Serilog;

public class DataService
{
    private readonly BGM_dbContext _dbContext;
    private readonly IMapper _mapper;

    public DataService(BGM_dbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task AddPurchaseOrderDetailsAsync(List<PurchaseOrderDetailDto> purchaseOrderDetailsDto)
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
            MessageBox.Show("Purchase orders loaded and saved successfully!");
            Log.Information("Purchase orders loaded and saved successfully!");
        }
        else
        {
            MessageBox.Show("No new purchase orders to save.");
            Log.Information("No new purchase orders to save.");
        }
    }
}
