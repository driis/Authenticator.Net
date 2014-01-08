using System.Text;
using Authenticator.Core;
using NUnit.Framework;

namespace Authenticator.Tests
{
    [TestFixture]
    public class Base32Tests
    {
        [Test]
        [TestCase("", "")]
        [TestCase("mzxw6ytb", "fooba")]
        [TestCase("MZXW6YTB", "fooba")]
        [TestCase("MZXW6YTBMZXW6YTB", "foobafooba")]
        [TestCase("MY======", "f")]
        [TestCase("MZXQ====", "fo")]
        public void Base32_CanDecodeFromBase32String(string base32, string expected)
        {
            byte[] result = Base32.BytesFromEncodedString(base32);

            string actual = Encoding.ASCII.GetString(result);
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}