using System.Windows;
using RabinEncryption.WPF.Base;
using StructureMap;

namespace RabinEncryption.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            IOCContainer.Initialize();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            IContainer container = ObjectFactory.Container;
            MainWindow = container.GetInstance<MainWindow>();
            if (MainWindow != null)
            {
                MainWindow.Content = PageFactory.Get(PageName.Dashboard);
                MainWindow.ShowDialog();
            }
        }
    }
}
