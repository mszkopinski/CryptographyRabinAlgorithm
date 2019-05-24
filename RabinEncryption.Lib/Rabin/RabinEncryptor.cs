using System.Linq;
using RabinEncryption.Lib.Extensions;

namespace RabinEncryption.Lib.Rabin
{
    public interface IRabinEncryptor
    {
        BigNumber[] Encrypt(byte[] cipherText, BigNumber privateKey, BigNumber publicKey);
    }

    public class RabinEncryptor : IRabinEncryptor
    {
        public BigNumber[] Encrypt(byte[] cipherText, BigNumber privateKey, BigNumber publicKey)
        {
            while (cipherText.Length % 61 != 0)
            {
                cipherText = cipherText.Concat(new byte[] { 0 }).ToArray();
            }

            var blocksLength = cipherText.Length / 61;
            var cipherTextBlockBytes = new byte[blocksLength, 64];

            for (int i = 0; i < blocksLength; i++)
            {
                for (int j = 0; j < 61; j++)
                    cipherTextBlockBytes[i, j] = cipherText[i * 61 + j];

                var privateKeyBytes = privateKey.GetBytes();
                cipherTextBlockBytes[i, 61] = privateKeyBytes[privateKeyBytes.Length - 3];
                cipherTextBlockBytes[i, 62] = privateKeyBytes[privateKeyBytes.Length - 2];
                cipherTextBlockBytes[i, 63] = privateKeyBytes[privateKeyBytes.Length - 1];
            }

            var messageBlocks = new BigNumber[blocksLength];
            for (int i = 0; i < blocksLength; i++)
            {
                messageBlocks[i] = new BigNumber(cipherTextBlockBytes.GetRow(i).ToArray());
            }

            var length = cipherTextBlockBytes.LongLength / 64;
            var encryptedMessageBlocks = new BigNumber[length];

            for (int i = 0; i < length; ++i)
            {
                encryptedMessageBlocks[i] = messageBlocks[i].ModPow(BigNumber.Two, publicKey);
            }
            return encryptedMessageBlocks;
        }
    }
}