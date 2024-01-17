using QuoterApp.Models;
using System;

namespace QuoterApp
{
    public class MarketOrderEventArgs : EventArgs
    {
        public MarketOrder MarketOrder { get; }

        public MarketOrderEventArgs(MarketOrder marketOrder)
        {
            MarketOrder = marketOrder;
        }
    }
}
