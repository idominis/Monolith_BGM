using Microsoft.Extensions.DependencyInjection;
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

        // Entity Framework DbContext - AddScoped ensures a separate DbContext instance per request
        //services.AddDbContext<BGM_dbContext>(options =>
        //    options.UseSqlServer(configuration.GetConnectionString("BGMDatabase")), ServiceLifetime.Scoped);
        services.AddDbContext<BGM_dbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("BGMDatabase"));
            options.EnableSensitiveDataLogging();
        }, ServiceLifetime.Scoped);
        services.AddDbContextFactory<BGM_dbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("BGMDatabase")));


        // SFTP Configuration remains Singleton since it's a shared resource
        services.AddSingleton<SftpClientManager>(provider =>
            new SftpClientManager(
                configuration["SftpConfig:Host"],
                configuration["SftpConfig:Username"],
                configuration["SftpConfig:Password"]
            ));

        // SFTP File Handler - retains Singleton since it may manage shared connections
        services.AddSingleton<SftpFileHandler>(provider =>
        {
            var clientManager = provider.GetRequiredService<SftpClientManager>();
            var statusUpdateService = provider.GetRequiredService<IStatusUpdateService>();
            return new SftpFileHandler(clientManager, statusUpdateService);
        });

        // Common Services
        
        services.AddScoped<IStatusUpdateService, StatusUpdateService>();
        services.AddScoped<IXmlService, XmlService>();
        services.AddScoped<FileManager>();
        services.AddScoped<IXmlService>(provider =>
            new XmlService(
                provider.GetRequiredService<FileManager>(),
                configuration["SchemaPaths:PurchaseOrderDetails"]
            ));
        services.AddScoped<ErrorHandlerService>();
        services.AddScoped<DataService>();
        services.AddScoped<MainFormController>();
        services.AddSingleton<MainForm>();

        return services.BuildServiceProvider();
    }
}
