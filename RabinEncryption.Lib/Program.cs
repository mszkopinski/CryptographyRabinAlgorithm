using System;
using System.Linq;

namespace RabinEncryption.Lib
{
    class Program
    {
        const int P = 167, Q = 151;
        const int CompositeNumber = P * Q;

        static void Main(string[] args)
        {
            string msg = "SAMPLE MESSAGE";
            var encryptedMessage = msg
                .Select(c => Rabin.Rabin.Encrypt(c, CompositeNumber))
                .ToList();

            for (int i = 0; i < encryptedMessage.Count; ++i)
            {
                Console.WriteLine(encryptedMessage[i]);
            }

            var decryptedMessage = encryptedMessage
                .Select(e => Rabin.Rabin.Decrypt(e, P, Q))
                .ToList();

            Console.WriteLine(Rabin.Rabin.DecodeMessage(decryptedMessage));
        }
    }
}
