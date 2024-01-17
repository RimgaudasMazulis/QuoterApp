using NUnit.Framework;
using QuoterApp.Cache;
using QuoterApp.MarketOrderSource;
using QuoterApp.Models;
using QuoterApp.Quoter;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace QuoterApp.Tests
{
    public class QuoterTests
    {
        private HardcodedMarketOrderSource _marketOrderSource;
        private ICache _marketOrdersCache;
        private IQuoter _quoter;

        [SetUp]
        public void Setup()
        {
            _marketOrderSource = new HardcodedMarketOrderSource();
            _marketOrdersCache = new Cache.Cache();

            GetMarketOrders()
                .GroupBy(order => order.InstrumentId)
                .Select(group => new { InstrumentId= group.Key, Orders = group.ToList()})
                .ToList()
                .ForEach(o => _marketOrdersCache.Add(o.InstrumentId, o.Orders));

            _quoter = new YourQuoter(_marketOrderSource, _marketOrdersCache);            
        }

        [Test]
        public void GetQuote_ValidInstrumentIdAndValidQuantity_ReturnsQuote()
        {
            var marketOrder = new MarketOrder { InstrumentId = "DK50782120", Price = 99.81, Quantity = 421 };

            var result = _quoter.GetQuote(marketOrder.InstrumentId, 10);

            Assert.That(result, Is.EqualTo(marketOrder.Price));
        }

        [Test]
        public void GetQuote_InvalidInstrumentId_ReturnsZero()
        {
            var result = _quoter.GetQuote("ABC123", 10);

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void GetQuote_ValidInstrumentIdAndTooHighQuantity_ReturnsZero()
        {
            var result = _quoter.GetQuote("ABC123", 1000);

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void GetVolumeWeightedAveragePrice_ValidInstrumentId_ReturnsPrice()
        {
            var marketOrders = new List<MarketOrder>()
            {
                new MarketOrder { InstrumentId = "DK50782120", Price = 99.81, Quantity = 421 },
                new MarketOrder { InstrumentId = "DK50782120", Price = 100.001, Quantity = 900 }
            };

            var result = _quoter.GetVolumeWeightedAveragePrice(marketOrders.FirstOrDefault().InstrumentId);

            Assert.That(result, Is.EqualTo(CalculateWeightedAverage(marketOrders)));
        }

        [Test]
        public void GetVolumeWeightedAveragePrice_InvalidInstrumentId_ReturnsPrice()
        {
            var result = _quoter.GetVolumeWeightedAveragePrice("ABC123");

            Assert.That(result, Is.EqualTo(0));
        }

        private double CalculateWeightedAverage(List<MarketOrder> marketOrders)
        {
            var totalPrice = marketOrders.Sum(order => order.Quantity * order.Price);
            var totalQuantity = marketOrders.Sum(order => order.Quantity);

            return totalPrice / totalQuantity;
        }

        private List<MarketOrder> GetMarketOrders()
        {
            return new List<MarketOrder>() 
            {
                new MarketOrder {InstrumentId = "BA79603015", Price = 102.997, Quantity = 12 },
                new MarketOrder {InstrumentId = "BA79603015", Price = 103.2, Quantity = 60 },
                new MarketOrder {InstrumentId = "AB73567490", Price = 103.25, Quantity = 79 },
                new MarketOrder {InstrumentId = "AB73567490", Price = 95.5, Quantity = 14 },
                new MarketOrder {InstrumentId = "BA79603015", Price = 98.0, Quantity = 1 },
                new MarketOrder {InstrumentId = "AB73567490", Price = 100.7, Quantity = 17 },
                new MarketOrder {InstrumentId = "DK50782120", Price = 100.001, Quantity = 900 },
                new MarketOrder {InstrumentId = "DK50782120", Price = 99.81, Quantity = 421 },
            };
        }
    }
}