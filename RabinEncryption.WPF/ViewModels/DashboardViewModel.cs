using RabinEncryption.Lib.Rabin;

namespace RabinEncryption.WPF.ViewModels
{
    public class DashboardViewModel : BaseViewModel, IDashboardViewModel
    {
        public readonly IRabinCryptoSystem RabinCryptoSystem;

        public DashboardViewModel(IRabinCryptoSystem rabinCryptoSystem)
        {
            RabinCryptoSystem = rabinCryptoSystem;
        }
    }
}