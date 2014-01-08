using System;
using System.Linq;

namespace Authenticator.Core
{
    public static class Base32
    {
        private static readonly char[] characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567".ToArray();
        private const char Pad = '=';

        public static byte[] BytesFromEncodedString(string encodedString)
        {
            if (encodedString.Length % 8 != 0)
                throw new ArgumentException ("Encoded data is not valid Base32.", "encodedString");

            encodedString = encodedString.ToUpperInvariant();
            string encodedWithoutPadding = encodedString.Replace("=", "");
            int length = (encodedWithoutPadding.Length * 5) / 8;
            var result = new byte[length];

            int blocks = encodedString.Length/8;
            for (int block = 0; block < blocks; block++)
            {
                long x = 0;
                int ch = 0;
                for (; ch < 8; ch++)
                {
                    char c = encodedString[ch+block*8];
                    int value = Array.IndexOf(characters, c);
                    if (value == -1 && c != Pad)
                        throw new ArgumentException("Encoded data is not valid Base32.", "encodedString");
                    if (c == Pad)
                        break;
                    x <<= 5;
                    x |= value;
                }
                
                int offset = block*5;
                x >>= (ch*5)%8; // remove zero bits
                for (int b = (ch*5) / 8 ; b > 0; b--)
                {
                    result[offset] = (byte) ((x >> ((b-1)*8)) & 0xff);
                    offset++;
                }
            }

            return result;
        }
    }
}