using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Stock;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDBContext _context;

        public StockRepository(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<List<Stock>> GetAllAsync(QueryObjects query)
        {
            // return await _context.Stocks.Include(x => x.Comments).ToListAsync();//INCLUDES APPLIED TO GET ALSO THE COMMENTS THAT COMES FROM ANOTHER ENDPOINT
            // var stocks = _context.Stocks.Include(x => x.Comments).AsQueryable();//New sintax to use the queryble filter 
            var stocks = _context.Stocks.Include(x => x.Comments).ThenInclude(x => x.appUser).AsQueryable();//We also added the then include which allow us to show the nested data from the relationship for example stock (father tree)-> comments (first nest) -> AppUser(nest inside comment that is a son of stock)
            if (!string.IsNullOrWhiteSpace(query.CompanyName))
            {
                stocks = stocks.Where(x => x.CompanyName.Contains(query.CompanyName));
            }

            if (!string.IsNullOrWhiteSpace(query.Symbol))
            {
                stocks = stocks.Where(x => x.Symbol.Contains(query.Symbol));
            }

            #region Filter by Order
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                if (query.SortBy.Equals("Symbol", StringComparison.OrdinalIgnoreCase))
                {
                    stocks = query.IsDecsending ? stocks.OrderByDescending(x => x.Symbol) : stocks.OrderBy(x => x.Symbol);
                }
            }
            #endregion
            
            #region Pagination applied
            var skipNumber = (query.PageNumber -1) * query.PageSize;// This logic is applied to show the register per the selected page skipping the registers that doesn't matter
            #endregion
            

            return await stocks.Skip(skipNumber).Take(query.PageSize).ToListAsync();
            
        }

        public async Task<Stock?> GetByIdAsync(int id)
        {
            // return await _context.Stocks.FindAsync(id);
            return await _context.Stocks.Include( x => x.Comments).FirstOrDefaultAsync(x => x.Id == id);//FindAsync is not allowed to use it with INCLUDE 
        }

        public async Task<Stock?> GetBySymbolAsync(string? symbol)
        {
            var x = await _context.Stocks.FirstOrDefaultAsync(x => x.Symbol == symbol);
            if (x == null)
            {
                return null;
            }
            return x;
        }

        public async Task<Stock> CreateAsync(Stock stockModel)
        {
            await _context.Stocks.AddAsync(stockModel);
            await _context.SaveChangesAsync();
            return stockModel;

        }

        public async Task<Stock?> DeleteAsync(int id)
        {
            var stockModel = await _context.Stocks.FirstOrDefaultAsync(z => z.Id == id);
            if (stockModel == null)
            {
                return null;
            }
            _context.Stocks.Remove(stockModel);
            await _context.SaveChangesAsync();

            return stockModel;
        }

        public async Task<Stock?> UpdateAsync(int id, UpdateStockRequestDto updateDto)
        {
            var stockModel = await _context.Stocks.FindAsync(id);
            if (stockModel == null)
            {
                return null;
            }
            stockModel.Symbol = updateDto.Symbol;
            stockModel.CompanyName = updateDto.CompanyName;
            stockModel.Purchase = updateDto.Purchase;
            stockModel.LastDiv = updateDto.Purchase;
            stockModel.Industry = updateDto.Industry;
            stockModel.MarketCap = updateDto.MarketCap;

            await _context.SaveChangesAsync();

            return stockModel;
        }

        public Task<bool> StockExits(int id)
        {
            return _context.Stocks.AnyAsync(s => s.Id == id);
        }
    }
}