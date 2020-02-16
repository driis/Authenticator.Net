using System;

namespace Authenticator.Tests
{
    public class Randomness
    {
        private static readonly Random Rand  = new Random();

        public static byte[] GetBytes(int count)
        {
            byte[] buf = new byte[count];
            Rand.NextBytes(buf);
            return buf;
        }

        public static long GetLong()
        {
            return (long) Rand.NextDouble();
        }
    }
}