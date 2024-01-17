using QuoterApp.MarketOrderSource;
using QuoterApp.Quoter;
using System;
using System.Threading;

namespace QuoterApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var marketOrderSource = new HardcodedMarketOrderSource();
                var marketOrdersCache = new Cache.Cache();

                var gq = new YourQuoter(marketOrderSource, marketOrdersCache);
                var qty = 120;

                Thread.Sleep(10000);

                var quote = gq.GetQuote("DK50782120", qty);
                var vwap = gq.GetVolumeWeightedAveragePrice("DK50782120");

                Console.WriteLine($"Quote: {quote}, {quote / (double)qty}");
                Console.WriteLine($"Average Price: {vwap}");
                Console.WriteLine();
                Console.WriteLine($"Done");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unhandled exception at the entry point: {ex.Message}");
                Environment.Exit(1);
            }

        }
    }
}
