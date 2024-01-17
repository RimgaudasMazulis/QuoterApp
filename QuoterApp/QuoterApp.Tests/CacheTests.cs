using NUnit.Framework;
using QuoterApp.Cache;

namespace QuoterApp.Tests
{
    public class CacheTests
    {
        private ICache _cache;

        [SetUp]
        public void Setup()
        {
            _cache = new Cache.Cache();
        }

        [Test]
        public void CacheAdd_AddsObject()
        {
            var testKey = "testKey";
            var testValue = "testValue";

            _cache.Add(testKey, testValue);

            var result = _cache.Contains(testKey);

            Assert.That(result, Is.True);
        }

        [Test]
        public void CacheGet_ReturnsObject()
        {
            var testKey = "testKey";
            var testValue = "testValue";

            _cache.Add(testKey, testValue);

            var result = _cache.Get<string>(testKey);

            Assert.That(result, Is.EqualTo(testValue));
        }

        [Test]
        public void CacheGet_ReturnsNull()
        {
            var testKey = "testKey";
            var testValue = "testValue";

            _cache.Add(testKey, testValue);

            var result = _cache.Get<string>($"{testKey}test");

            Assert.IsNull(result);
            Assert.That(result, !Is.EqualTo(testValue));
        }

        [Test]
        public void CacheRemove_ReturnsNull()
        {
            var testKey = "testKey";
            var testValue = "testValue";

            _cache.Add(testKey, testValue);

            var result = _cache.Get<string>(testKey);

            Assert.IsNotNull(result);

            _cache.Remove(testKey);

            result = _cache.Get<string>(testKey);

            Assert.IsNull(result);
        }

        [Test]
        public void CacheContains_ReturnsTrue()
        {
            var testKey = "testKey";
            var testValue = "testValue";

            _cache.Add(testKey, testValue);

            var result = _cache.Contains(testKey);

            Assert.IsTrue(result);
        }

        [Test]
        public void CacheContains_ReturnsFalse()
        {
            var testKey = "testKey";
            var testValue = "testValue";

            _cache.Add(testKey, testValue);

            var result = _cache.Contains($"{testKey}test");

            Assert.IsFalse(result);
        }
    }
}