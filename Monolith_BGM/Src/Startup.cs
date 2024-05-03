using Microsoft.Extensions.DependencyInjection;
using Monolith_BGM.Controllers;
//using Monolith_BGM.Services;
using BGM.Common;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Monolith_BGM.Models;
using Monolith_BGM;
using BGM.SftpUtilities;
using Monolith_BGM.XMLService;
using Monolith_BGM.Src;

public class Startup
{
    public static IServiceProvider ConfigureServices(IConfiguration configuration)
    {
        var services = new ServiceCollection();

        // AutoMapper Configuration
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new PurchaseOrderDetailProfile());
            cfg.AddProfile(new PurchaseOrderHeaderProfile());
            cfg.AddProfile(new PurchaseOrderSummaryProfile());
            cfg.AddProfile(new PurchaseOrderProcessedSentProfile());
        });
        IMapper mapper = mapperConfig.CreateMapper();
        services.AddSingleton(mapper);

        // Entity Framework DbContext
        services.AddDbContext<BGM_dbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("BGMDatabase")));

        // SFTP Configuration
        services.AddSingleton<SftpClientManager>(provider =>
            new SftpClientManager(
                configuration["SftpConfig:Host"],
                configuration["SftpConfig:Username"],
                configuration["SftpConfig:Password"]
            ));

        // SFTP File Handler
        services.AddSingleton<SftpFileHandler>(provider =>
        {
            var clientManager = provider.GetRequiredService<SftpClientManager>();
            var statusUpdateService = provider.GetRequiredService<IStatusUpdateService>();
            return new SftpFileHandler(clientManager, statusUpdateService);
        });

        // Common Services
        services.AddSingleton<IStatusUpdateService, StatusUpdateService>();
        services.AddSingleton<IXmlService, XmlService>();
        services.AddSingleton<FileManager>();
        services.AddSingleton<ErrorHandlerService>();
        services.AddSingleton<DataService>();
        services.AddSingleton<MainFormController>();
        services.AddSingleton<MainForm>();

        return services.BuildServiceProvider();
    }


}
