using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces;
using api.Service;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{

    [Route("api/assets/search")]
    [ApiController]
    public class MultiSourceAssetsController : ControllerBase
    {
        private readonly IFMPService _FMPService;

        private readonly IGECKOService _GECKOService;

        public MultiSourceAssetsController(IFMPService fMPService, IGECKOService geCKOService)
        {
            _FMPService = fMPService;
            _GECKOService = geCKOService;
        }


        [HttpGet("{name}")]
        public async Task<IActionResult> GetStockAndCryptoByNameAsync([FromRoute] string name)
        {

            var stocksList = _FMPService.FindStocksToBeJoined(name);
            var criptoList = _GECKOService.FindCriptosAndJoinToCurrentList(name);

            await Task.WhenAll(stocksList, criptoList); // uso un await compuesto para evitar repetir en cada una de las llamadas de los servicios
            
            var completeAssetsList = (await stocksList ).Concat(await criptoList).ToList();//we use LinQ
            return Ok(completeAssetsList);
            // var stocksList = await _FMPService.FindStocksToBeJoined(name);
            // if (stocksList == null)
            // {
            //     return NotFound();
            // }

            // var stocksAndCriptosList = await _GECKOService.FindCriptosAndJoinToCurrentList(name, stocksList);

            // if (stocksAndCriptosList == null)
            // {
            //     return NotFound();
            // }
            // return Ok(stocksAndCriptosList);
        }

    }
}