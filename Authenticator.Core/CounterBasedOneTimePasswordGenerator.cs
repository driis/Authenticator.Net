using System;
using System.Globalization;
using System.Security.Cryptography;

namespace Authenticator.Core
{
    public class CounterBasedOneTimePasswordGenerator : IOneTimePasswordGenerator
    {
        private readonly int _digits;
        private readonly Func<long> _counter;
        private readonly byte[] _secret;

        public CounterBasedOneTimePasswordGenerator(byte[] secret, int digits, Func<long> counter)
        {
            _secret = secret.Copy();
            _digits = digits;
            _counter = counter;
        }

        public string NextPassword()
        {
            long c = _counter();
            var hashAlgorithm = new HMACSHA1(_secret);
            byte[] countBytes = BitConverter.GetBytes(c);
            var hash = new HmacHash(hashAlgorithm.ComputeHash(countBytes));
            int passwordValue = hash.TruncateToDigits(_digits);
            return passwordValue.ToString("D" + _digits, CultureInfo.InvariantCulture);
        }
    }
}