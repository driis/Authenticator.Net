using System;
using System.Linq;
using Authenticator.Core;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Authenticator.Tests
{
    [TestFixture]
    public class HmachashTests : WithFixture
    {
        // Example hash from RFC4226 section 5.4.
        private static readonly byte[] ExampleHash = {
            0x1f, 0x86, 0x98, 0x69, 0x0e, 0x02, 0xca, 0x16, 0x61, 0x85, 0x50, 0xef, 0x7f, 0x19, 0xda, 0x8e, 0x94, 0x5b, 0x55, 0x5a
        };

        private const int HashLengthInBytes = 20;

        [Test]
        public void Ctor_NullData_ThrowsArgumentNullException()
        {
            Assert.That(() => new HmacHash(null), Throws.InstanceOf<ArgumentNullException>());
        }
        
        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(19)]
        public void Ctor_WrongLength_ThrowsArgumentException(int length)
        {
            Assert.That(() => new HmacHash(Fixture.CreateMany<byte>(length).ToArray()), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }
        
        [Test]
        public void Ctor_CorrectLength_DoesNotThrow()
        {
            var hash = new HmacHash(new byte[HashLengthInBytes]);

            Assert.That(hash, Is.Not.Null);
        }

        [Test]
        public void Ctor_MutatingSourceArray_InnerDataIsPreserved()
        {
            byte[] data = Fixture.CreateMany<byte>(HashLengthInBytes).ToArray();
            byte[] original = data.Copy();
            var hash = new HmacHash(data);

            data[5] = Fixture.Create<byte>();
            CollectionAssert.AreEqual(original, hash.RawBytes);
        }

        [Test]
        public void RawBytes_ReturnsCopyOfOriginalData()
        {
            byte[] data = Fixture.CreateMany<byte>(HashLengthInBytes).ToArray();

            var hash = new HmacHash(data);

            var rawBytes = hash.RawBytes;

            Assert.That(rawBytes, Is.Not.SameAs(hash.RawBytes));
            CollectionAssert.AreEqual(rawBytes, hash.RawBytes);
            rawBytes[9] = Fixture.Create<byte>();
            CollectionAssert.AreEqual(data, hash.RawBytes);
        }

        [Test]
        public void ToString_ReturnsFormattedString()
        {
            var hash = new HmacHash(new byte[HashLengthInBytes]);

            string str = hash.ToString();

            Assert.That(str.Length, Is.EqualTo(2 * HashLengthInBytes));
            Assert.That(str, Is.EqualTo(new string('0', 2 * HashLengthInBytes)));
        }

        [Test]
        public void ToString_ReturnsFormattedString_WithCorrectValues()
        {
            var hash = new HmacHash(Enumerable.Range(0xa0,20).Select(x => (byte)x));
            const string expected = "a0a1a2a3a4a5a6a7a8a9aaabacadaeafb0b1b2b3";
            string str = hash.ToString();

            Assert.That(str.Length, Is.EqualTo(2 * HashLengthInBytes));
            Assert.That(str, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(6,   872921)]
        [TestCase(7,  7872921)]
        [TestCase(8, 57872921)]
        public void TruncateToDigits_ValidNumberOfDigits_TruncatesCorrectly(int digits, int expected)
        {
            var hash = new HmacHash(ExampleHash);

            int result = hash.TruncateToDigits(digits);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(5)]
        [TestCase(9)]
        [TestCase(0)]
        public void TruncateToDigits_InvalidNumberOfDigits_Throws(int digits)
        {
            var hash = new HmacHash(ExampleHash);

            Assert.That(() => hash.TruncateToDigits(digits), Throws.InstanceOf<ArgumentOutOfRangeException>());
        }
    }
}