using AutoMapper;

namespace Monolith_BGM
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<PurchaseOrderDetailProfile>();
            });

            var mapper = config.CreateMapper();

            Application.Run(new MainForm(mapper));
        }
    }
}