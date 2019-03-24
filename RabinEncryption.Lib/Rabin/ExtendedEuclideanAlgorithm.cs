namespace RabinEncryption.Lib.Rabin
{
    public static class ExtendedEuclideanAlgorithm
    {
        public static int[] Calculate(int a, int b)
        {
            if (b > a)
            {
                var temp = a;
                a = b;
                b = temp;
            }
            int x = 0, lastX = 1, y = 1, lastY = 0;
            while (b != 0)
            {
                var q = a / b;

                var temp1 = a % b;
                a = b;
                b = temp1;

                var temp2 = x;
                x = lastX - q * x;
                lastX = temp2;

                var temp3 = y;
                y = lastY - q * y;
                lastY = temp3;
            }
            return new[] { lastX, lastY, 1 };
        }
    }
}