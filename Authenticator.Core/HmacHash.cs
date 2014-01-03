using System;
using System.Collections.Generic;
using System.Linq;

namespace Authenticator.Core
{
    public class HmacHash
    {
        private const int Length = 20;
        private readonly byte[] _data;

        public HmacHash(IEnumerable<byte> data) : this(data.ToArray()) {}

        public HmacHash(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");
            if (data.Length != Length)
                throw new ArgumentException("Data must be exactly 160 bits (20 bytes).");

            _data = data.Copy();
        }

        public byte[] RawBytes
        {
            get { return _data.Copy(); }
        }

        public int TruncateToDigits(int digits)
        {
            if (digits < 6 || digits > 8)
                throw new ArgumentOutOfRangeException("digits", "Number of digits must be between 6 and 8");

            int offset = _data[19] & 0x0f;

            int value = (_data[offset] &0x7f) << 24 | 
                _data[offset + 1] << 16 | 
                _data[offset + 2]  << 8 | 
                _data[offset + 3];

            var divisor = DivisorForDigits(digits);
            return value%divisor;
        }

        private static int DivisorForDigits(int digits)
        {
            digits -= 6;
            const int minDivisor = 1000000;
            int divisor = minDivisor;
            while (digits-- > 0)
                divisor *= 10;
            return divisor;
        }

        public override string ToString()
        {
            return String.Join("", _data.Select(b => b.ToString("x2")));
        }
    }
}