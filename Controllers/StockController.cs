using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Stock;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/stock")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IStockRepository _stockRepository;
        public StockController(ApplicationDBContext context, IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] QueryObjects query)
        {
            // var stocks = await _context.Stocks.ToListAsync();
            var stocks = await _stockRepository.GetAllAsync(query);//Now with the Queryble we can also use this parameter allowing in the function add more parameters to filter usinf for example swagger
            var stockDto = stocks.Select(s => s.ToStockDto()).ToList();

            return Ok(stockDto);
        }

        [HttpGet("{id:int}")]

        public async Task<IActionResult> GetById(int id)
        {
            //var stocks = await _context.Stocks.FindAsync(id);
            var stocks = await _stockRepository.GetByIdAsync(id);
            if (stocks == null)
            {
                return NotFound("");
            }
            return Ok(stocks.ToStockDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStockRequestDto stockDto)
        {
            if (!ModelState.IsValid)//Controller Base provide the modelState and it has the porpuse of verify if all the Data Anotations Validations are ok or not 
            {
                return BadRequest(ModelState);
            }

            var stockModel = stockDto.ToStockFromCreateDTO();
            await _stockRepository.CreateAsync(stockModel);
            // await _context.Stocks.AddAsync(stockModel);
            // await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = stockModel.Id }, stockModel.ToStockDto());//Created action return a 201 code, the nameof explain the URL wich from where we can access
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateStockRequestDto updateDto)
        {
            // var stockModel = await _context.Stocks.FirstOrDefaultAsync(s => s.Id == id);
            // if (stockModel == null)
            // {
            //     return NotFound();
            // }

            // stockModel.Symbol = updateDto.Symbol;
            // stockModel.CompanyName = updateDto.CompanyName;
            // stockModel.Purchase = updateDto.Purchase;
            // stockModel.LastDiv = updateDto.Purchase;
            // stockModel.Industry = updateDto.Industry;
            // stockModel.MarketCap = updateDto.MarketCap;

            // await _context.SaveChangesAsync();
            if (!ModelState.IsValid)//Controller Base provide the modelState and it has the porpuse of verify if all the Data Anotations Validations are ok or not 
            {
                return BadRequest(ModelState);
            }

            var stockModel = await _stockRepository.UpdateAsync(id, updateDto);
            return Ok(stockModel?.ToStockDto());
        }

        [HttpDelete]
        [Route("{id:int}")]

        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            // var stockModel = await _context.Stocks.FirstOrDefaultAsync(x => x.Id == id);
            // if (stockModel == null)
            // {
            //     return NotFound();
            // }
            // _context.Remove(stockModel);//This section remove the record object from the table 

            // await _context.SaveChangesAsync();

            await _stockRepository.DeleteAsync(id);

            return NoContent();//it doesn't provide a response since it's a delete action
        }
    }
}