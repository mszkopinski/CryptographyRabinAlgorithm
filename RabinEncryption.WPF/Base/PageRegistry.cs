using System.Windows.Controls;
using StructureMap.Configuration.DSL;
using RabinEncryption.WPF.ViewModels;
using RabinEncryption.WPF.Views;

namespace RabinEncryption.WPF.Base
{
    public class PageRegistry : Registry
    {
        public PageRegistry()   
        {
            Register<DashboardView, DashboardViewModel>(PageName.Dashboard);
        }

        void Register<TControl, TViewModel>(PageName pageName)
            where TControl : ContentControl where TViewModel : BaseViewModel
        {
            For<ContentControl>().Singleton().Use<TControl>().Named(pageName.ToString());
            For<BaseViewModel>().Singleton().Use<TViewModel>().Named(pageName.ToString());
        }
    }
}