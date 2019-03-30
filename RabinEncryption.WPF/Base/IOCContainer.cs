﻿using RabinEncryption.Lib.Rabin;
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
                IRabinCryptoSystem rabinCryptoSystem = new RabinCryptoSystem();
                var dashboardVM = new DashboardViewModel(rabinCryptoSystem);
                x.For<MainWindow>().Use<MainWindow>();
                x.For<IRabinCryptoSystem>().Use(rabinCryptoSystem);
                x.For<IDashboardViewModel>().Use(dashboardVM);
                x.For<DashboardView>().Use<DashboardView>();
            });

            PageFactory.Initialize(new PageRegistry());
        }
    }
}