using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockHistoricalPricesAPI.Models;

namespace StockHistoricalPricesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockHistoryController : ControllerBase
    {
        private readonly StockHistoryContext _context;

        public StockHistoryController(StockHistoryContext context)
        {
            _context = context;
        }

        // Get all information on stocks in DB
        // GET: api/StockHistory
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StockHistory>>> GetStockHistories()
        {
          if (_context.StockHistories == null)
          {
              return NotFound();
          }
            return await _context.StockHistories.ToListAsync();
        }
        

        // Get all the historical data for stock matching ID - Used to build Post Response
        // GET: api/StockHistory/Stocks/MSFT_2023-05-17
        [HttpGet("Stocks/{id}")]
        public async Task<ActionResult<StockHistory>> GetStockHistory(string id)
        {
            if (_context.StockHistories == null)
            {
                return NotFound();
            }
            
            var stockHistory = await _context.StockHistories.FindAsync(id);

            if (stockHistory == null)
            {
                return NotFound();
            }

            return stockHistory;
        }

        // Get the daily returns for days specified 
        // GET: api/StockHistory/DailyReturns
        [HttpGet("DailyReturns/{symbol}")]
        public async Task<ActionResult<IEnumerable<DailyReturns>>> GetStockHistoriesDateRange(string symbol, string? fromDate, string? toDate)
        {
          if (_context.StockHistories == null)
          {
              return NotFound();
          }
            // Check Dates
            DateTime startDate;
            DateTime endDate;
            DateTime currentDate;
            // Range must be 5 years or less 
            int maxNumberofDaysRange = 365 * 5;
            startDate = fromDate != null ? Convert.ToDateTime(fromDate) : new DateTime(DateTime.Now.Year, 1, 1);
            endDate = toDate != null ? Convert.ToDateTime(toDate) : DateTime.Now;
            // If start date is after end date
            if(endDate < startDate)
            {
                return Problem("Invalid Parameters, toDate must be after fromDate.");
            }
            if((endDate - startDate).TotalDays > maxNumberofDaysRange)
            {
                return Problem("Invalid Parameters, Range must be 5 years or less.");
            }
            // For each day in range, check if stock info exists for that symbol and date, if true add data to list for return
            currentDate = startDate;
            List<DailyReturns> returnsForStock = new List<DailyReturns>();
            while (currentDate <= endDate)
            {
                
                string stockID = symbol + "_" + currentDate.ToString("yyyy-MM-dd");
                if (StockHistoryExists(stockID)){
                    var stockData = await _context.StockHistories.FindAsync(stockID);
                    // Data should be in JSON format that includes calculated daily returns, symbol, and date  
                    returnsForStock.Add(StockHistoryToDailyReturns(stockData));
                }
                currentDate = currentDate.AddDays(1);
            }
            if(returnsForStock.Count == 0)
            {
                return NotFound();
            }
            return returnsForStock;
        }

        // Add Stock History Object to Dataset - Used for testing 
        // POST: api/StockHistory
        [HttpPost]
        public async Task<ActionResult<StockHistory>> PostStockHistory(StockHistory stockHistory)
        {
          if (_context.StockHistories == null)
          {
              return Problem("Entity set 'StockHistoryContext.StockHistories'  is null.");
          }
            // Create unique ID for Object using symbol and priceDate
            stockHistory.id = stockHistory.symbol + "_" + stockHistory.priceDate;
            _context.StockHistories.Add(stockHistory);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (StockHistoryExists(stockHistory.id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            // Returns JSON representation of object just created with POST 
            return CreatedAtAction(nameof(GetStockHistory), new { id = stockHistory.id }, stockHistory);
        }

        // Checks if data on the stock exists using unique ID 
        private bool StockHistoryExists(string id)
        {
            return (_context.StockHistories?.Any(e => e.id == id)).GetValueOrDefault();
        }

        // Creates a Daily Returns object representation of stock 
        private DailyReturns StockHistoryToDailyReturns(StockHistory stockHistory)
        {
            return new DailyReturns
            {
                symbol = stockHistory.symbol,
                date = stockHistory.priceDate,
                splitAdjustedReturn = stockHistory.close - stockHistory.open,
                fullyAdjustedReturn = stockHistory.fclose - stockHistory.fopen,
                unadjustedReturn = stockHistory.uclose - stockHistory.uopen
            };
        }
        
    }
}
