using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using QuoterApp.Cache;
using QuoterApp.Database;
using QuoterApp.MarketOrderSource;
using QuoterApp.Models;

namespace QuoterApp.Quoter
{
    public class YourQuoter : IQuoter, IDisposable
    {
        private readonly IMarketOrderSource _marketOrderSource;
        private readonly ICache _marketOrdersCache;
        private readonly object lockObject = new object();
        private readonly Thread _orderFetchingThread;
        private bool disposed = false;

        public YourQuoter(IMarketOrderSource marketOrderSource, ICache marketOrdersCache)
        {
            _marketOrderSource = marketOrderSource ?? throw new ArgumentNullException(nameof(marketOrderSource));
            _marketOrdersCache = marketOrdersCache ?? throw new ArgumentNullException(nameof(marketOrdersCache));

            CacheMarketOrdersFromDb();
            marketOrderSource.MarketOrderReceived += OnMarketOrderReceived;

            _orderFetchingThread = new Thread(FetchMarketOrders);
            _orderFetchingThread.Start();
        }

        public double GetQuote(string instrumentId, int quantity)
        {
            try
            {
                ValidateParameters(instrumentId, quantity);

                double bestPrice = 0;

                lock (lockObject)
                {
                    var cachedOrders = _marketOrdersCache.Get<List<MarketOrder>>(instrumentId);

                    if (cachedOrders == null || cachedOrders.Count == 0)
                    {
                        return bestPrice;
                    }

                    var relevantOrders = cachedOrders.Where(order => order.InstrumentId == instrumentId && order.Quantity >= quantity).ToList();

                    if (relevantOrders.Count == 0)
                    {
                        return 0;
                    }

                    bestPrice = relevantOrders.Min(order => order.Price);
                }

                return bestPrice;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Argument error in GetQuote: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetQuote: {ex.Message}");
                throw;
            }
        }

        public double GetVolumeWeightedAveragePrice(string instrumentId)
        {
            try
            {
                ValidateInstrumentId(instrumentId);

                var relevantOrders = new List<MarketOrder>();

                lock (lockObject)
                {
                    var cachedOrders = _marketOrdersCache.Get<List<MarketOrder>>(instrumentId);

                    if (cachedOrders == null || cachedOrders.Count == 0)
                    {
                        return 0;
                    }

                    relevantOrders = cachedOrders.ToList();
                }

                var totalPrice = relevantOrders.Sum(order => order.Quantity * order.Price);
                var totalQuantity = relevantOrders.Sum(order => order.Quantity);

                return totalPrice / totalQuantity;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Argument error in GetVolumeWeightedAveragePrice: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetVolumeWeightedAveragePrice: {ex.Message}");
                throw;
            }
        }

        private void OnMarketOrderReceived(object sender, MarketOrderEventArgs e)
        {
            try
            {
                FileCsvHelper.Write(new List<MarketOrder>() { e.MarketOrder }, Constants.Constants.MarketOrdersDbPath);
                UpdateMarketOrdersCache(e.MarketOrder);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating market orders cache: {ex.Message}");
                throw;
            }
        }

        private void FetchMarketOrders()
        {
            try
            {
                while (true)
                {
                    if (disposed)
                    {
                        break;
                    }

                    _marketOrderSource.GetNextMarketOrder();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching market orders: {ex.Message}");
                throw;
            }
        }

        private void UpdateMarketOrdersCache(MarketOrder order)
        {
            try
            {
                lock (lockObject)
                {
                    var existingOrders = _marketOrdersCache.Get<List<MarketOrder>>(order.InstrumentId) ?? new List<MarketOrder>();
                    existingOrders.Add(order);
                    _marketOrdersCache.Add(order.InstrumentId, existingOrders);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating market orders cache: {ex.Message}");
                throw;
            }
        }

        private void CacheMarketOrdersFromDb()
        {
            try
            {
                lock (lockObject)
                {
                    FileCsvHelper.Read(Constants.Constants.MarketOrdersDbPath, (MarketOrder marketOrder) =>
                    {
                        UpdateMarketOrdersCache(marketOrder);
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while caching market orders from Db: {ex.Message}");
                throw;
            }
        }

        private void ValidateParameters(string instrumentId, int quantity)
        {
            if (string.IsNullOrEmpty(instrumentId))
            {
                throw new ArgumentException("InstrumentId cannot be null or empty.", nameof(instrumentId));
            }

            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be greater than zero.");
            }
        }

        private void ValidateInstrumentId(string instrumentId)
        {
            if (string.IsNullOrEmpty(instrumentId))
            {
                throw new ArgumentException("InstrumentId cannot be null or empty.", nameof(instrumentId));
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    StopFetchingThread();
                }

                disposed = true;
            }
        }

        private void StopFetchingThread()
        {
            try
            {
                _orderFetchingThread.Interrupt(); // This will wake up the thread if it's sleeping
                _orderFetchingThread.Join(); // Wait for the thread to finish
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error stopping the fetching thread: {ex.Message}");
            }
        }
    }
}
