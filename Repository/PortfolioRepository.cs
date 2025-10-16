using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly ApplicationDBContext _dbContext;
        public PortfolioRepository(ApplicationDBContext applicationDBContext)
        {
            _dbContext = applicationDBContext;
        }
        public async Task<List<Stock>> GetUserPortfolioAsync(AppUser user)// It returns the list of Stocks by user
        {
            return await _dbContext.Portfolios.Where(p => p.AppUserId == user.Id)//this access to the portfolios table bring all the portfolio by User then return with the select the list of Stocks for each register returned
            .Select(stock => new Stock
            {
                Id = stock.StockId,
                Symbol = stock.Stock.Symbol,
                CompanyName = stock.Stock.CompanyName,
                Purchase = stock.Stock.Purchase,
                LastDiv = stock.Stock.LastDiv,
                Industry = stock.Stock.Industry,
                MarketCap = stock.Stock.MarketCap
            }).ToListAsync();

        }

        public async Task<Portfolio> CreateAsync(Portfolio portfolio)
        {
            await _dbContext.Portfolios.AddAsync(portfolio);
            await _dbContext.SaveChangesAsync();

            return portfolio;
        }

        public async Task<Portfolio> DeleteAsync(AppUser appUser, string symbol)
        {
            var portfolioModel = await _dbContext.Portfolios.FirstOrDefaultAsync(p => p.AppUserId == appUser.Id && p.Stock.Symbol.ToLower() == symbol.ToLower());

            if (portfolioModel == null)
            {
                return null;
            }
            _dbContext.Portfolios.Remove(portfolioModel);

            await _dbContext.SaveChangesAsync();

            return portfolioModel;
        }
    }
}