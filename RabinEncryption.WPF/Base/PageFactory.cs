using System.Windows.Controls;
using RabinEncryption.WPF.ViewModels;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace RabinEncryption.WPF.Base
{
    public static class PageFactory
    {
        public static void Initialize(Registry registry)
        {
            ObjectFactory.Configure(o => o.AddRegistry(registry));
        }

        public static ContentControl Get(PageName pageName)
        {
            ContentControl control = ObjectFactory.GetNamedInstance<ContentControl>(pageName.ToString());

            control.Loaded += (s, e) => {
                control.DataContext = ObjectFactory.GetNamedInstance<BaseViewModel>(pageName.ToString());
            };

            return control;
        }
    }

    public enum PageName
    {
        Unknown = 0,
        MainWindow = 1,
        Dashboard = 2,
    }
}