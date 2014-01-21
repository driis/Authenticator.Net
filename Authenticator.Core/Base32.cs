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

        public static string EncodeBytes(byte[] data)
        {
            int bitLength = data.Length*8;            
            int encodedCharCount = bitLength/5 + (bitLength % 5 == 0 ? 0 : 1);
            int blockCount = (data.Length/5) + (data.Length%5 == 0 ? 0 : 1);
            int padChars = 8 - (encodedCharCount%8 == 0 ? 8 : encodedCharCount % 8);
            var buffer = new char[encodedCharCount + padChars];

            int bufferPos = 0;
            for (int block = 0; block < blockCount ; block++)
            {
                long acc = 0;
                int bytesMoved = 0;
                for (int ch = block*5; ch < (block + 1)*5 && ch < data.Length ; ch++)
                {
                    acc <<= 8;
                    acc |= data[ch];
                    bytesMoved++;
                }

                int bitsToMove = bytesMoved*8;
                int zeroPadLastByte = bitsToMove%5 == 0 ? 0 : 5 - (bitsToMove%5);
                acc <<= zeroPadLastByte;
                bitsToMove += zeroPadLastByte;
                
                while(bitsToMove > 0)
                {
                    bitsToMove -= 5;                    
                    long idx = (acc >> bitsToMove) & 0x1f;
                    buffer[bufferPos++] = characters[idx];
                }
            }
            while (bufferPos < encodedCharCount + padChars)
                buffer[bufferPos++] = Pad;

            return new String(buffer);            
        }
    }
}