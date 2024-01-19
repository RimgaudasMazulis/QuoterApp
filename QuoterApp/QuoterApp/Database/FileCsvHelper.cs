using CsvHelper;
using CsvHelper.Configuration;
using QuoterApp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace QuoterApp.Database
{
    public class FileCsvHelper
    {
        public static IEnumerable<MarketOrder> Read(string filePath, Action<MarketOrder> action)
        {
            try
            {
                var fileExists = File.Exists(filePath);
                if (!fileExists)
                {
                    return new List<MarketOrder>();
                }

                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var marketOrders =  csv.GetRecords<MarketOrder>();

                    foreach (var marketOrder in marketOrders)
                    {
                        action(marketOrder);
                    }

                    return marketOrders;
                }                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while reading market orders from CSV, path:{filePath} : {ex.Message}");
                throw;
            }
        }

        public static void Write(IEnumerable<MarketOrder> marketOrders, string filePath)
        {
            try
            {
                var fileExists = File.Exists(filePath);
                var isNotEmpty = new FileInfo(filePath).Length != 0;

                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = !isNotEmpty,
                };

                using (var stream = File.Open(filePath, fileExists ? FileMode.Append : FileMode.Create))
                using (var writer = new StreamWriter(stream))
                using (var csv = new CsvWriter(writer, config))
                {
                    csv.WriteRecords(marketOrders);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while writing market orders to CSV, path: filePath : {ex.Message}");
                throw;
            } 

        }
    }
}
