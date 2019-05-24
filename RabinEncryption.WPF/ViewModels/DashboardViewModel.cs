using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;
using RabinEncryption.Lib.Extensions;
using RabinEncryption.Lib.Generators;
using RabinEncryption.Lib.Rabin;
using RabinEncryption.WPF.Base;

namespace RabinEncryption.WPF.ViewModels
{
    public class DashboardViewModel : BaseViewModel, IDashboardViewModel
    {
        public string PublicKeyString
        {
            get => publicKeyString;
            set { publicKeyString = value; OnPropertyChanged(nameof(PublicKeyString)); }
        }

        string publicKeyString;

        public string StringToEncrypt
        {
            get => stringToEncrypt;
            set { stringToEncrypt = value; OnPropertyChanged(nameof(StringToEncrypt)); }
        }

        string stringToEncrypt;

        public string EncryptedString
        {
            get => encryptedString;
            set { encryptedString = value; OnPropertyChanged(nameof(EncryptedString)); }
        }

        string encryptedString;

        public string DecryptedString
        {
            get => decryptedString;
            set { decryptedString = value; OnPropertyChanged(nameof(DecryptedString)); }
        }

        string decryptedString;

        public bool CanDecrypt
        {
            get => canDecrypt;
            set { canDecrypt = value; OnPropertyChanged(nameof(CanDecrypt)); }
        }

        bool canDecrypt;

        public ICommand LoadFileCommand => new RelayCommand(OnLoadFile, null);
        public ICommand SaveFileCommand => new RelayCommand(OnSaveFile, null);
        public ICommand EncryptMessageCommand => new RelayCommand(OnMessageEncrypted, null);
        public ICommand DecryptMessageCommand => new RelayCommand(OnMessageDecrypted, null);
        public ICommand CopyToClipboardCommand => new RelayCommand(OnCopiedToClipboard, null);
        public ICommand RefreshKeysCommand => new RelayCommand(OnRefreshKeys, null);
        public ICommand RefreshPublicKeyCommand => new RelayCommand(OnRefreshPublicKey, null);

        public readonly IRabinEncryptor RabinEncryptor;
        public readonly IRabinDecryptor RabinDecryptor;

        private byte[] decryptedBytes = null;
        private byte[] encryptedBytes = null;
        private byte[] bytesToEncrypt = null;
        private BigNumber[] messageBlocks;
        private BigNumber[] encryptedMessageBlocks;

        private BigNumber p;
        private BigNumber q;
        private BigNumber publicKey;

        public DashboardViewModel(IRabinEncryptor rabinEncryptor, IRabinDecryptor rabinDecryptor)
        {
            RabinEncryptor = rabinEncryptor ?? throw new ArgumentNullException(nameof(IRabinEncryptor));
            RabinDecryptor = rabinDecryptor ?? throw new ArgumentNullException(nameof(IRabinEncryptor));
            OnRefreshKeys(null);
        }

        async void OnMessageEncrypted(object sender)
        {
            if (StringToEncrypt.Length == 0) return;
            bytesToEncrypt = Encoding.UTF32.GetBytes(StringToEncrypt);

            await Task.Factory.StartNew(() =>
            {
                encryptedMessageBlocks = RabinEncryptor.Encrypt(bytesToEncrypt, p, publicKey);
            });

            var bytes = encryptedMessageBlocks.SelectMany(bigNum => bigNum.GetBytes()).ToArray();
            EncryptedString = bytes.AsString();
            CanDecrypt = true;
        }

        async void OnMessageDecrypted(object sender)
        {
            var encryptedBlocksLength = encryptedMessageBlocks.Length;
            var bytesToTest = new BigNumber[encryptedBlocksLength];
            var privateKeyBytes = p.GetBytes();

            await Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < encryptedBlocksLength; i++)
                {
                    var results = RabinDecryptor.GetDecryptedValues(i, encryptedMessageBlocks, p, q, publicKey);
                    bytesToTest[i] = results.SingleOrDefault(res =>
                    {
                        var resBytes = res.GetBytes();
                        return resBytes[resBytes.Length - 1] == privateKeyBytes[privateKeyBytes.Length - 1]
                               && resBytes[resBytes.Length - 2] ==
                               privateKeyBytes[privateKeyBytes.Length - 2]
                               && resBytes[resBytes.Length - 3] ==
                               privateKeyBytes[privateKeyBytes.Length - 3];
                    });
                      
                }
            });

            decryptedBytes = new byte[0];
            for (int i = 0; i < bytesToTest.Length; i++)
            {
                decryptedBytes = decryptedBytes.Concat(bytesToTest[i].GetBytes().CutLastThreeBytes()).ToArray();
            }

            var recoveredString = Encoding.UTF32.GetString(decryptedBytes);
            recoveredString = recoveredString.Replace("\0", "");
            recoveredString = recoveredString.Substring(0, recoveredString.Length - 1); 
            DecryptedString = recoveredString;
        }

        void OnCopiedToClipboard(object sender)
        {
            System.Windows.Clipboard.SetText(EncryptedString);
        }

        void OnRefreshKeys(object sender)
        {
            p = RabinKeysGenerator.GenerateKey();
            q = RabinKeysGenerator.GenerateKey();
            publicKey = p * q;
            PublicKeyString = publicKey.ToString();
        }

        void OnRefreshPublicKey(object sender)
        {

        }

        void OnLoadFile(object sender)
        {
            var dialog = new OpenFileDialog();
            bool? res = dialog.ShowDialog();
            if (res != true) return;
            var fileName = dialog.FileName;
            StringToEncrypt = string.Join(" ", File.ReadAllBytes(fileName).Select(b => (long)b));
        }

        void OnSaveFile(object sender)
        {
            var dialog = new SaveFileDialog();
            bool? res = dialog.ShowDialog();
            if (res != true) return;
            var fileName = dialog.FileName;
            var bytes = DecryptedString.Split(new string[] {" "}, StringSplitOptions.None).Select(byte.Parse).ToArray();
            File.WriteAllBytes(fileName, bytes);
        }

        enum State
        {
            String,
            File
        };
    }
}