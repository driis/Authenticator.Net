using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Authenticator.Core;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Authenticator.Tests
{
    [TestFixture]
    public class TimeBasedOneTimePasswordGeneratorTests : WithFixture
    {
        [Test]
        [TestCase(6)]
        [TestCase(7)]
        [TestCase(8)]
        public void NextPassword_WithDigitCount_GeneratesPasswordWithCorrectNumberOfNumericDigits(int length)
        {
            var sut = new TimeBasedOneTimePasswordGenerator(Fixture.CreateMany<byte>(20).ToArray(), length, () => DateTime.Now);

            string result = sut.NextPassword();

            Assert.That(result, Is.Not.Null, "Null returned from NextPassword");
            Assert.That(Regex.Match(result,"^\\d+$").Success, Is.True, "Password contains non-numeric characters");
            Assert.That(result.Length, Is.EqualTo(length), "Wrong length password generated");
        }

        [Test]
        // Test values from RFC6238: https://tools.ietf.org/html/rfc6238#appendix-B
        [TestCase("1970-01-01 00:00:59", "94287082")]
        [TestCase("2005-03-18 01:58:29", "07081804")]
        [TestCase("2005-03-18 01:58:31", "14050471")]
        [TestCase("2009-02-13 23:31:30", "89005924")]
        [TestCase("2033-05-18 03:33:20", "69279037")]
        [TestCase("2603-10-11 11:33:20", "65353130")]
        public void NextPassword_KnownTimeAndSecret_GeneratesCorrectPassword(string timeString, string expectedPassword)
        {
            DateTime instant = DateTime.ParseExact(timeString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            byte[] secret = Encoding.ASCII.GetBytes("12345678901234567890");
            var sut = new TimeBasedOneTimePasswordGenerator(secret, 8, () => instant);

            string result = sut.NextPassword();

            Assert.That(result, Is.EqualTo(expectedPassword));
        }
    }
}