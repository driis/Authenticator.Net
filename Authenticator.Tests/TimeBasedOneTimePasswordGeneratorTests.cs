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
            const string secretAscii = "12345678901234567890";
            const HashMode mode = HashMode.HMACSHA1;

            TestPasswordGeneration(timeString, expectedPassword, secretAscii, mode);
        }

        [Test]
        [TestCase("1970-01-01 00:00:59", "46119246")]
        [TestCase("2005-03-18 01:58:29", "68084774")]
        [TestCase("2005-03-18 01:58:31", "67062674")]
        [TestCase("2009-02-13 23:31:30", "91819424")]
        [TestCase("2033-05-18 03:33:20", "90698825")]
        [TestCase("2603-10-11 11:33:20", "77737706")]
        public void NextPassword_SHA256_KnownTimeAndSecret_GeneratesCorrectPassword(string timeString, string expectedPassword)
        {
            const string secretAscii = "12345678901234567890123456789012";
            const HashMode mode = HashMode.HMACSHA256;

            TestPasswordGeneration(timeString, expectedPassword, secretAscii, mode);
        }

        [Test]
        [TestCase("1970-01-01 00:00:59", "90693936")]
        [TestCase("2005-03-18 01:58:29", "25091201")]
        [TestCase("2005-03-18 01:58:31", "99943326")]
        [TestCase("2009-02-13 23:31:30", "93441116")]
        [TestCase("2033-05-18 03:33:20", "38618901")]
        [TestCase("2603-10-11 11:33:20", "47863826")]
        public void NextPassword_SHA512_KnownTimeAndSecret_GeneratesCorrectPassword(string timeString, string expectedPassword)
        {
            const string secretAscii = "1234567890123456789012345678901234567890123456789012345678901234";
            const HashMode mode = HashMode.HMACSHA512;

            TestPasswordGeneration(timeString, expectedPassword, secretAscii, mode);
        }

        private static void TestPasswordGeneration(string timeString, string expectedPassword, string secretAscii, HashMode mode)
        {
            byte[] secret = Encoding.ASCII.GetBytes(secretAscii);
            DateTime instant = DateTime.ParseExact(timeString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            var sut = new TimeBasedOneTimePasswordGenerator(secret, 8, () => instant, mode);

            string result = sut.NextPassword();

            Assert.That(result, Is.EqualTo(expectedPassword));
        }
    }
}