using System.Numerics;

namespace RabinEncryption.Lib
{
    public interface IEncryptionAlgorithm
    {
        BigInteger[] GenerateKey(int bitLenght);
        BigInteger Encrypt(BigInteger valueToEncrypt, BigInteger publicKey);
        BigInteger[] Decrypt(BigInteger encryptedValue, BigInteger privateKey, BigInteger privateKey2);
    }
}