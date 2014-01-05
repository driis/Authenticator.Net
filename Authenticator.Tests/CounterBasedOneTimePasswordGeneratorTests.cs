using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Authenticator.Core;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Authenticator.Tests
{
    [TestFixture]
    public class CounterBasedOneTimePasswordGeneratorTests : WithFixture
    {
        [Test]
        [TestCase(6)]
        [TestCase(7)]
        [TestCase(8)]
        public void NextPassword_GeneratesNumericPassword(int digits)
        {
            var secret = Fixture.CreateMany<byte>(100).ToArray();
            long count = A<long>();
            Func<long> counter = () => count++;

            var sut = new CounterBasedOneTimePasswordGenerator(secret, digits, counter);

            string password = sut.NextPassword();

            Assert.That(password.Length, Is.EqualTo(digits));
            Assert.That(Regex.IsMatch(password, "^\\d+$"), Is.True);
        }

        [Test]
        public void NextPassword_SameCounterValue_ReturnsSamePassword()
        {
            var secret = Fixture.CreateMany<byte>(100).ToArray();
            long count = A<long>();
            Func<long> counter = () => count;

            var sut = new CounterBasedOneTimePasswordGenerator(secret, 6, counter);

            string first = sut.NextPassword();
            string second = sut.NextPassword();

            Assert.That(first, Is.EqualTo(second));
        }

        [Test]
        public void NextPassword_IncrementingCounterValue_ReturnsDifferentPasswords()
        {
            var secret = Fixture.CreateMany<byte>(100).ToArray();
            long count = A<long>();
            Func<long> counter = () => count++;

            var sut = new CounterBasedOneTimePasswordGenerator(secret, 6, counter);

            string first = sut.NextPassword();
            string second = sut.NextPassword();

            Assert.That(first, Is.Not.EqualTo(second));
        }

        [Test]
        // Test vectors from RFC 4226 Appendix D: https://tools.ietf.org/html/rfc4226#appendix-D
        [TestCase(0, "755224")]
        [TestCase(1, "287082")]
        [TestCase(2, "359152")]
        [TestCase(3, "969429")]
        [TestCase(4, "338314")]
        [TestCase(5, "254676")]
        [TestCase(6, "287922")]
        [TestCase(7, "162583")]
        [TestCase(8, "399871")]
        [TestCase(9, "520489")]
        public void NextPassword_KnownVector_GeneratesCorrectPassword(int counterValue, string expectedPassword)
        {
            byte[] secret = Encoding.ASCII.GetBytes("12345678901234567890");
            var sut = new CounterBasedOneTimePasswordGenerator(secret, 6, () => counterValue);

            string result = sut.NextPassword();

            Assert.That(result, Is.EqualTo(expectedPassword));
        }
    }
}