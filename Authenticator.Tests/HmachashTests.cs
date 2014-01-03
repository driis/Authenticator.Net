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
        private const int HashLengthInBytes = 20;

        [Test]
        public void Ctor_NullData_ThrowsArgumentNullException()
        {
            Assert.That(() => new HmacHash(null), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        [TestCase(1)]
        [TestCase(19)]
        [TestCase(21)]
        public void Ctor_WrongLength_ThrowsArgumentException(int length)
        {
            Assert.That(() => new HmacHash(Fixture.CreateMany<byte>(length).ToArray()), Throws.ArgumentException);
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
            
        }
    }
}