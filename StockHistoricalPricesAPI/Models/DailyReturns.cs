using System;
namespace StockHistoricalPricesAPI.Models
{
	public class DailyReturns
	{
		public string symbol { get; set; }
		public string date { get; set; }
        // Daily returns calculated using split adjusted, fully adjusted, and unadjusted data
        public float splitAdjustedReturn { get; set; }
        public float fullyAdjustedReturn { get; set; }
        public float unadjustedReturn { get; set; }
	}
}

