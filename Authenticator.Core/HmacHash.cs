using System;
using System.Collections;
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

        public override string ToString()
        {
            return String.Join("", _data.Select(b => b.ToString("x2")));
        }
    }
}