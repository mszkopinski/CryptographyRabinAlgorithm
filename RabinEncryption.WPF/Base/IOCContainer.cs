using RabinEncryption.Lib.Rabin;
using RabinEncryption.WPF.ViewModels;
using RabinEncryption.WPF.Views;
using StructureMap;

namespace RabinEncryption.WPF.Base
{
    public static class IOCContainer
    {
        public static void Initialize()
        {
            ObjectFactory.Initialize(x =>
            {
                var rabinEncryptor = new RabinEncryptor();
                var rabinDecryptor = new RabinDecryptor();

                var dashboardVM = new DashboardViewModel(rabinEncryptor, rabinDecryptor);
                x.For<MainWindow>().Use<MainWindow>();
                x.For<IRabinEncryptor>().Use(rabinEncryptor);
                x.For<IRabinDecryptor>().Use(rabinDecryptor);
                x.For<IDashboardViewModel>().Use(dashboardVM);
                x.For<DashboardView>().Use<DashboardView>();
            });

            PageFactory.Initialize(new PageRegistry());
        }
    }
}