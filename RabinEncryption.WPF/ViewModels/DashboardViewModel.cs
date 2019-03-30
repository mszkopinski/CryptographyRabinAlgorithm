using System;
using System.Linq;
using System.Windows.Input;
using RabinEncryption.Lib.Rabin;
using RabinEncryption.WPF.Base;

namespace RabinEncryption.WPF.ViewModels
{
    public class DashboardViewModel : BaseViewModel, IDashboardViewModel
    {
        const int P = 167, Q = 151;
        const int CompositeNumber = P * Q;

        public string EncryptedString
        {
            get => encryptedString;
            set { encryptedString = value; OnPropertyChanged("EncryptedString"); }
        }

        string encryptedString;

        public string DecryptedString
        {
            get => decryptedString;
            set { decryptedString = value; OnPropertyChanged("DecryptedString"); }
        }

        string decryptedString;

        public string StringToEncrypt { get; set; }
        public string StringToDecrypt { get; set; }

        public ICommand EncryptMessageCommand
            => new RelayCommand(OnMessageEncrypted, null);

        public ICommand DecryptMessageCommand
            => new RelayCommand(OnMessageDecrypted, null);

        public ICommand CopyToClipboardCommand
            => new RelayCommand(OnCopiedToClipboard, null);

        public readonly IRabinCryptoSystem RabinCryptoSystem;

        public DashboardViewModel(IRabinCryptoSystem rabinCryptoSystem)
        {
            RabinCryptoSystem = rabinCryptoSystem ?? throw new ArgumentNullException(nameof(IRabinCryptoSystem));
        }

        void OnMessageEncrypted(object sender)
        {
            if (String.IsNullOrEmpty(StringToEncrypt)) return;
            var encryptedMessage = StringToEncrypt
                .Select(c => RabinCryptoSystem.Encrypt(c, CompositeNumber))
                .ToList();
            EncryptedString = String.Join("", encryptedMessage);
        }

        void OnMessageDecrypted(object sender)
        {
            if (String.IsNullOrEmpty(StringToDecrypt)) return;
            var decryptedMessage = StringToEncrypt
                .Select(c => RabinCryptoSystem.Decrypt(c, P, Q))
                .ToList();
            DecryptedString = RabinCryptoSystem.DecodeMessage(decryptedMessage);
        }

        void OnCopiedToClipboard(object sender)
        {
            
            System.Windows.Clipboard.SetText(EncryptedString);
        }
    }
}