using System;
using System.Windows.Forms;
using AutoMapper;
using Monolith_BGM.Models;
using Serilog;
using BGM.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using BGM.SftpUtilities;
using Microsoft.Extensions.DependencyInjection;

namespace Monolith_BGM
{
    internal static class Program
    {
        public static IConfiguration Configuration { get; private set; }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            InitializeConfiguration();
            ConfigureLogging();
            var serviceProvider = Startup.ConfigureServices(Configuration);

            var mainForm = serviceProvider.GetService<MainForm>();
            ConfigureServicesAndRun();
            Application.Run(mainForm);
        }

        private static void InitializeConfiguration()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
        }

        private static void ConfigureLogging()
        {
            string logFilePath = Configuration["Serilog:WriteTo:1:Args:path"]; // Adjust index based on the correct position in the JSON array

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .WriteTo.Console()
                .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("Application Starting");
        }

        private static void ConfigureServicesAndRun()
        {
            var serviceProvider = Startup.ConfigureServices(Configuration);

            // Ensure MainForm is correctly resolved along with its dependencies
            var mainForm = serviceProvider.GetService<MainForm>();
            Application.Run(mainForm);
        }
    }
}
