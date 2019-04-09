namespace RabinEncryption.Lib.Extensions
{
    public static class Utils
    {
        /// <summary>
        /// a^d mod n
        /// </summary>
        public static long ModPow(long a, long d, long n)
        {
            long res = 1;
            for (int i = 0; i < d; i++)
            {
                res = res * a;
                res = res % n;
            }
            return res % n;
        }
    }
}