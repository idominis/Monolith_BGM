using Microsoft.EntityFrameworkCore;
using Monolith_BGM.Models;

public class ApplicationDbContext : DbContext
{
    public DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Specify the connection string to your database
        optionsBuilder.UseSqlServer("YourConnectionString");
    }
}
