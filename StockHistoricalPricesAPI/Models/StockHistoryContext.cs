using System;
using Microsoft.EntityFrameworkCore;

namespace StockHistoricalPricesAPI.Models
{
	public class StockHistoryContext : DbContext
    {
		
        public StockHistoryContext(DbContextOptions<StockHistoryContext> options)
        : base(options)
        {
        }

        public DbSet<StockHistory> StockHistories { get; set; } = null!;
    }
}

