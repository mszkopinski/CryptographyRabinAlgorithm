using System;

namespace RabinEncryption.Lib.Generators
{
    public class RandomNumberGenerator
    {
        readonly long maximum;
        long value, nextValue;

        public RandomNumberGenerator(long max)
        {
            maximum = max;
            value = (GetCurrentMilli() % maximum);
        }

        public long GetNextValue()
        {
            nextValue = (32719 * value + 2133) % maximum;
            if (nextValue % 2 == 0)
            {
                value = (GetCurrentMilli() % maximum);
            }
            else
            {
                value = nextValue % maximum;
            }
            return value;
        }

        long GetCurrentMilli()
        {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            return (long)timeSpan.TotalMilliseconds;
        }
    }
}