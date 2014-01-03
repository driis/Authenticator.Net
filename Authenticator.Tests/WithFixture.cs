using System;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Authenticator.Tests
{
    public class WithFixture
    {
        private Lazy<Fixture> _fixture;

        public Fixture Fixture
        {
            get { return _fixture.Value; }
        }

        [SetUp]
        public void FixtureSetUp()
        {
            _fixture = new Lazy<Fixture>();
        }

        public T A<T>()
        {
            return Fixture.Create<T>();
        }
    }
}