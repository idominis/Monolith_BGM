using System;
using System.Windows.Forms;
using AutoMapper;
using Monolith_BGM.Models;
using Serilog;
using BGM.Common;
using Microsoft.EntityFrameworkCore;

namespace Monolith_BGM
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Initialize application configuration for WinForms
            ApplicationConfiguration.Initialize();

            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug() // Set the minimum log level to Debug for detailed logs
                .WriteTo.Console()    // Write logs to the console
                .WriteTo.File(@"C:\Logs\Monolith_BGM.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            // Log the start of the application
            Log.Information("Application Starting");

            var statusUpdateService = new StatusUpdateService();

            // Configure AutoMapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<PurchaseOrderDetailProfile>();
                cfg.AddProfile<PurchaseOrderHeaderProfile>();
                cfg.AddProfile<PurchaseOrderSummaryProfile>();
                cfg.AddProfile<PurchaseOrderSentProfile>();
            });
            var mapper = config.CreateMapper();
            //var dbContext = new BGM_dbContext(); // You'd typically have some setup or factory for DbContext
            var errorHandler = new ErrorHandlerService();  // Instantiate the error handler
            var optionsBuilder = new DbContextOptionsBuilder<BGM_dbContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=BGM_db;Trusted_Connection=True;MultipleActiveResultSets=true")
            .EnableSensitiveDataLogging();
            var dbContext = new BGM_dbContext(optionsBuilder.Options);

            var dataService = new DataService(dbContext, mapper, errorHandler, statusUpdateService);

            // Run the main form
            try
            {
                Application.Run(new MainForm(mapper, dataService, errorHandler, statusUpdateService));
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "An unexpected error occurred while running the application.");
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application exit
                Log.CloseAndFlush();
            }
        }
    }
}
