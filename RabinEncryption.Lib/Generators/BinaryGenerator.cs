using System.Collections.Generic;

namespace RabinEncryption.Lib.Generators
{
    public static class BinaryGenerator
    {
        public static List<int> GenerateBinary(long number)
        {
            var binary = new List<int>();
            while (number > 0)
            {
                var binEquivalent = (int) number % 2;
                binary.Add(binEquivalent);
                number /= 2;
            }
            return binary;
        }
    }
}