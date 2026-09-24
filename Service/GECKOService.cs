using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using api.Dtos.Stock;
using api.Interfaces;
using Newtonsoft.Json;

namespace api.Service
{
    public class GECKOService : IGECKOService
    {
        private HttpClient _httpClient;

        public GECKOService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // public async Task<List<FMPStockDto>> FindCriptosAndJoinToCurrentList(string assetName, List<FMPStockDto> criptoProfileCompleteList)
        public async Task<List<FMPStockDto>> FindCriptosAndJoinToCurrentList(string assetName)
        {
            List<FMPStockDto> criptoProfileCompleteList = new List<FMPStockDto>();
            var encodedName = WebUtility.UrlEncode(assetName);
            var responseCryptoAssets = await _httpClient.GetAsync($"https://api.coingecko.com/api/v3/search?query={encodedName}");
            var responseCryptoAssetsStr = await responseCryptoAssets.Content.ReadAsStringAsync();
            var responseCryptoAssetsDes = JsonConvert.DeserializeObject<GECKOCoinDto.Root>(responseCryptoAssetsStr);

            foreach (var coin in responseCryptoAssetsDes!.coins)
            {
                FMPStockDto cryptoProfile = new FMPStockDto
                {
                    symbol = coin.symbol,
                    companyName = coin.name,
                    currency = "CRYPTO CURRENCY",
                    exchangeFullName = "Exchange Full Name in Details.",
                    exchange = "Exchange Name in Details.",
                    image = coin.large,
                    defaultImage = false,
                };
                criptoProfileCompleteList.Add(cryptoProfile);
            }

            return criptoProfileCompleteList;
        }

        /*OPCION IMPORTANTE para craer un nuevo servicio que llame tanto al GECKO como al FMP asi cada uno es completamente independiente
        // En el controlador
        [HttpGet("{name}")]
        public async Task<IActionResult> GetStockAndCryptoByNameAsync([FromRoute] string name)
        {
            var stocksTask = _fmpService.SearchStocksAsync(name);
            var cryptosTask = _geckoService.SearchCryptoAsync(name);
            
            await Task.WhenAll(stocksTask, cryptosTask);
            
            // Lista unificada del mismo tipo
            var combinedList = (await stocksTask).Concat(await cryptosTask).ToList();
            
            return Ok(combinedList);
        }
        
        */
    }
}