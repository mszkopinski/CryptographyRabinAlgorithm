using System.Collections.Generic;

namespace RabinEncryption.Lib.Rabin
{
    public interface IRabinCryptoSystem
    {
        int P { get; }
        int Q { get; }
        int PublicKey { get; }
        int Encrypt(int plainText, int publicKey);
        int Decrypt(int cipher, int p, int q);
        string DecodeMessage(IEnumerable<int> encoded);
    }
}