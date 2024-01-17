using QuoterApp.Models;
using System;

namespace QuoterApp.MarketOrderSource
{
    /// <summary>
    /// Interface to access market orders
    /// </summary>
    public interface IMarketOrderSource
    {
        /// <summary>
        /// Blocking method that will return next available market order.
        /// </summary>
        /// <returns>Market order containing InstrumentId, Price and Quantity</returns>
        public MarketOrder GetNextMarketOrder();

        /// <summary>
        /// An event used to invoke a following function to be called 
        /// </summary>
        event EventHandler<MarketOrderEventArgs> MarketOrderReceived;
    }
}
