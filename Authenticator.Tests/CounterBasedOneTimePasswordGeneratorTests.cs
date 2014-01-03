using System;
using System.Linq;
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
    }
}