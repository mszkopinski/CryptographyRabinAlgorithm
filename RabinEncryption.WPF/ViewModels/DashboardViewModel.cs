using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;
using RabinEncryption.Lib.Generators;
using RabinEncryption.Lib.Rabin;
using RabinEncryption.WPF.Base;

namespace RabinEncryption.WPF.ViewModels
{
    public class DashboardViewModel : BaseViewModel, IDashboardViewModel
    {
        public string PublicKey
        {
            get => publicKey;
            set { publicKey = value; OnPropertyChanged(nameof(PublicKey)); }
        }

        string publicKey;

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

        public readonly IRabinEncryptor RabinEncryptor;
        public readonly IRabinDecryptor RabinDecryptor;
        Tuple<long, long, long> currentKeys;

        public DashboardViewModel(IRabinEncryptor rabinEncryptor, IRabinDecryptor rabinDecryptor)
        {
            RabinEncryptor = rabinEncryptor ?? throw new ArgumentNullException(nameof(IRabinEncryptor));
            RabinDecryptor = rabinDecryptor ?? throw new ArgumentNullException(nameof(IRabinEncryptor));
            OnRefreshKeys(null);
        }

        readonly List<Task<long>> decryptionTasks = new List<Task<long>>();
        readonly List<Task<Tuple<long, long>>> encryptionTasks = new List<Task<Tuple<long, long>>>();
        State currentState = State.String;

        async void OnMessageEncrypted(object sender)
        {
            if (StringToEncrypt.Length == 0) return;

            var bytes = new List<byte>();
            var splittedString = StringToEncrypt.Split(new[] {" "}, StringSplitOptions.None);
            if (splittedString.All(s => byte.TryParse(s, out _)))
            {
                bytes = splittedString.Select(byte.Parse).ToList();
                currentState = State.File;
            }
            else
            {
                bytes = Encoding.UTF32.GetBytes(StringToEncrypt).ToList();
                currentState = State.String;
            }

            DecryptedString = String.Empty;
            EncryptedString = String.Empty;

            encryptionTasks.Clear();
            if (bytes.Count != 0)
            {
                foreach (var b in bytes)
                {
                    if (b == 0)
                    {
                        encryptionTasks.Add(Task.Factory.StartNew(() => new Tuple<long, long>(0, 0)));
                        continue;
                    }

                    var encryptedMsg = RabinEncryptor.Encrypt(b, currentKeys.Item3, out var breakSize);
                    foreach (var value in encryptedMsg)
                    {
                        encryptionTasks.Add(Task.Factory.StartNew(() =>
                            new Tuple<long, long>(value, breakSize)));
                    }
                }
            }

            await Task.WhenAll(encryptionTasks);
            var encryptionResults = new List<Tuple<long, long>>(encryptionTasks.Select(t => t.Result));

            decryptionTasks.Clear();
            foreach (var res in encryptionResults)
            {
                var ciphterText = res.Item1;
                var breakSize = res.Item2;
                if (ciphterText == 0)
                {
                    decryptionTasks.Add(Task.Factory.StartNew(() => (long)0));
                    continue;
                }
                decryptionTasks.Add(Task.Factory.StartNew(() => RabinDecryptor.Decrypt(new List<long> { ciphterText }, currentKeys.Item1, currentKeys.Item2,
                    currentKeys.Item3, breakSize)));
            }

            EncryptedString = string.Join(" ", encryptionResults.Select(res => res.Item1));
            CanDecrypt = true;
        }

        async void OnMessageDecrypted(object sender)
        {
            if (decryptionTasks.Count == 0) return;

            await Task.WhenAll(decryptionTasks);
            var decryptionResults = new List<long>(decryptionTasks.Select(t => t.Result));
            if (currentState != State.String)
            {
                DecryptedString = string.Join(" ", decryptionResults);
            }
            else
            {
                DecryptedString = string.Join(" ",
                    Encoding.UTF32.GetString(decryptionResults.Select(l => (byte) l).ToArray()));
            }
        }

        void OnCopiedToClipboard(object sender)
        {
            System.Windows.Clipboard.SetText(EncryptedString);
        }

        void OnRefreshKeys(object sender)
        {
            currentKeys = RabinKeysGenerator.GetKeys();
            PublicKey = currentKeys.Item3.ToString();
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