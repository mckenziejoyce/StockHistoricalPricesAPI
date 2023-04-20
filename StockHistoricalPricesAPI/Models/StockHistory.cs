using System;
namespace StockHistoricalPricesAPI.Models
{
	public class StockHistory
	{
        // Unique ID for Stock History Object 
        public string id { get; set; }
        // Associated symbol or ticker
        public string symbol { get; set; }

        // Date Information is being returned for 
        public string priceDate { get; set; }

        // Fully adjusted for historical dates
        public float fclose { get; set; }
        public float fhigh { get; set; }
        public float flow { get; set; }
        public float fopen { get; set; }
        public float fvolume { get; set; }

        // Adjusted data for historical dates. Split adjusted only.
        public float close { get; set; }
        public float high { get; set; }
        public float low { get; set; }
        public float open { get; set; }
        public float volume { get; set; }

        // Unadjusted data for historical dates
        public float uclose { get; set; }
        public float uhigh { get; set; }
        public float ulow { get; set; }
        public float uopen { get; set; }
        public float uvolume { get; set; }
    }
}

