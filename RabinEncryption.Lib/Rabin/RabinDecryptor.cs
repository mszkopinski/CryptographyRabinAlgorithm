namespace RabinEncryption.Lib.Rabin
{
    public interface IRabinDecryptor
    {
        BigNumber[] GetDecryptedValues(int i, BigNumber[] encryptedMsgBlocks, BigNumber p, BigNumber q,
            BigNumber publicKey);
    }

    public class RabinDecryptor : IRabinDecryptor
    {
        public BigNumber[] GetDecryptedValues(int i, BigNumber[] encryptedMsgBlocks, BigNumber p, BigNumber q, BigNumber publicKey)
        {
            var result = new BigNumber[4];

            BigNumber m_p1 = encryptedMsgBlocks[i].ModPow((p + BigNumber.One) / BigNumber.Four, p);
            BigNumber m_q1 = encryptedMsgBlocks[i].ModPow((q + BigNumber.One) / BigNumber.Four, q);
            var m_p2 = p - m_p1;
            var m_q2 = q - m_q1;

            BigNumber[] ext = ExtendedEuclideanAlgorithm(p, q);
            var y_p = ext[1];
            var y_q = ext[2];

            result[0] = (y_p * p * m_q1 + y_q * q * m_p1) % publicKey;
            result[1] = (y_p * p * m_q2 + y_q * q * m_p1) % publicKey;
            result[2] = (y_p * p * m_q1 + y_q * q * m_p2) % publicKey;
            result[3] = (y_p * p * m_q2 + y_q * q * m_p2) % publicKey;

            return result;
        }

        BigNumber[] ExtendedEuclideanAlgorithm(BigNumber a, BigNumber b)
        {
            BigNumber s = BigNumber.Zero;
            BigNumber old_s = BigNumber.One;
            BigNumber t = BigNumber.One;
            BigNumber old_t = BigNumber.Zero;
            BigNumber r = b;
            BigNumber old_r = a;
            while (r != BigNumber.Zero)
            {
                BigNumber q = old_r / r;
                BigNumber tr = r;
                r = old_r - q * r;
                old_r = tr;

                BigNumber ts = s;
                s = old_s - q * s;
                old_s = ts;

                BigNumber tt = t;
                t = old_t - q * t;
                old_t = tt;
            }
            return new[] { old_r, old_s, old_t };
        }
    }
}