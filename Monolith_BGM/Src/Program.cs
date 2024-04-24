using System;
using System.Windows.Forms;
using AutoMapper;
using Serilog;

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
              //.WriteTo.File("logs/Monolith_BGM.log", rollingInterval: RollingInterval.Day) // Write logs to a file with daily rolling
                .WriteTo.File(@"C:\Logs\Monolith_BGM.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            // Log the start of the application
            Log.Information("Application Starting");

            // Configure AutoMapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<PurchaseOrderDetailProfile>();
            });
            var mapper = config.CreateMapper();

            // Run the main form
            try
            {
                Application.Run(new MainForm(mapper));
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
