using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;

namespace Authenticator.Core
{
    public class CounterBasedOneTimePasswordGenerator : IOneTimePasswordGenerator
    {
        private readonly int _digits;
        private readonly Func<long> _counter;
        private readonly HMAC _hashAlgorithm;

        public CounterBasedOneTimePasswordGenerator(byte[] secret, int digits, Func<long> counter) : this(secret, digits, counter, HashMode.HMACSHA1) 
        {}

        internal CounterBasedOneTimePasswordGenerator(byte[] secret, int digits, Func<long> counter, HashMode hashMode)
        {
            _digits = digits;
            _counter = counter;
            secret = secret.Copy();
            switch (hashMode)
            {
                case HashMode.HMACSHA1:
                    _hashAlgorithm = new HMACSHA1(secret);
                    break;
                case HashMode.HMACSHA256:
                    _hashAlgorithm = new HMACSHA256(secret);
                    break;
                case HashMode.HMACSHA512:
                    _hashAlgorithm = new HMACSHA512(secret);
                    break;
            }
        }

        public string NextPassword()
        {
            long c = _counter();            
            byte[] countBytes = BitConverter.GetBytes(c).Reverse().ToArray();
            var hash = new HmacHash(_hashAlgorithm.ComputeHash(countBytes));
            int passwordValue = hash.TruncateToDigits(_digits);
            return passwordValue.ToString("D" + _digits, CultureInfo.InvariantCulture);
        }
    }
}