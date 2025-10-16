using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Extensions;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/portfolio")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IStockRepository _stockRepository;

        private readonly IPortfolioRepository _portfolioRepository;
        public PortfolioController(UserManager<AppUser> userManager, IStockRepository stockRepository, IPortfolioRepository portfolioRepository)
        {
            _userManager = userManager;
            _stockRepository = stockRepository;
            _portfolioRepository = portfolioRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserPortfolio()
        {
            var username = User.GetUserName();//the User(given by deafult) is inherited from the ControllerBase and ClaimsPrincipal where we can get the CLAIMS 
            var userObj = await _userManager.FindByNameAsync(username);
            if (userObj == null)
            {
                return NotFound("User without portfolio.");
            }
            var userPortfolio = await _portfolioRepository.GetUserPortfolioAsync(userObj);//_stockRepository.GetStockAsync(username);

            return Ok(userPortfolio);
        }

        [HttpPost]
        [Authorize]

        public async Task<IActionResult> AddPortfolio(string symbol)
        {
            var userName = User.GetUserName();
            var userObj = await _userManager.FindByNameAsync(userName);

            var stockObj = await _stockRepository.GetBySymbolAsync(symbol);

            if (stockObj == null)
            {
                return BadRequest("Stock not found");
            }

            var existingPortfolio = await _portfolioRepository.GetUserPortfolioAsync(userObj);

            if (existingPortfolio.Find(x => x.Symbol.ToLower() == symbol.ToLower()) != null) { return BadRequest("Stock already added in the portfolio."); }

            var portfoliModel = new Portfolio
            {
                AppUserId = userObj.Id,
                StockId = stockObj.Id,

            };

            await _portfolioRepository.CreateAsync(portfoliModel);

            if (portfoliModel == null)
            {
                return StatusCode(500, "Something went wrong during the creation.");
            }
            //return CreatedAtAction(nameof(GetUserPortfolio), ...);

            return Created();
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteStockPortfolioAsync(string symbol)
        {
            var userName = User.GetUserName();
            var userObj = await _userManager.FindByNameAsync(userName);
            if (userObj == null)
            {
                return BadRequest("User not found");
            }

            var existingPortfolio = await _portfolioRepository.GetUserPortfolioAsync(userObj);

            var stockToBeDeleted = existingPortfolio.Where(x => x.Symbol.ToLower() == symbol.ToLower());
            if (stockToBeDeleted.Count() > 0)
            {
                await _portfolioRepository.DeleteAsync(userObj, symbol);
            }
            else
            {
                return StatusCode(500, "Stock doesn't exists in your portfolio.");
            }

            return Ok(stockToBeDeleted);

        }

    }
}