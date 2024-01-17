using NUnit.Framework;
using QuoterApp.Cache;
using QuoterApp.MarketOrderSource;
using QuoterApp.Models;

namespace QuoterApp.Tests
{
    public class MarketOrderSourceTests
    {
        private HardcodedMarketOrderSource _marketOrderSource;

        [SetUp]
        public void Setup()
        {
            _marketOrderSource = new HardcodedMarketOrderSource();
        }


        [Test]
        public void GetNextMarketOrder_ReturnsNextMarketOrder_IsNotNull()
        {
            var nextMarketOrder = _marketOrderSource.GetNextMarketOrder();

            Assert.IsNotNull(nextMarketOrder);
            Assert.IsInstanceOf<MarketOrder>(nextMarketOrder);
        }
    }
}