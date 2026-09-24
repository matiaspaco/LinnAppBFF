using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using api.Dtos.Stock;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;

namespace api.Service
{
    public class FMPService : IFMPService
    {
        private HttpClient _httpClient;
        private IConfiguration _config;

        public FMPService(HttpClient httpClient, IConfiguration config)//this is configured in the program.cs
        {
            _httpClient = httpClient;
            _config = config;
        }


        #region API call implementation: FMP API external endpoint, deserialize object, etc.
        //we should here call the FMP api to get the data of the stock with the symbol
        public async Task<Stock> FindStockBySymbolAsync(string symbol)
        {
            try
            {
                var encodedSymbol = WebUtility.UrlEncode(symbol);
                var result = await _httpClient.GetAsync($"https://financialmodelingprep.com/stable/profile?symbol={encodedSymbol}&apikey={_config["FMPKey"]}");//We took the API KEY value from the appsetting.json 
                if (result.IsSuccessStatusCode)
                {
                    var content = await result.Content.ReadAsStringAsync();
                    var tasks = JsonConvert.DeserializeObject<FMPStockDto[]>(content);//We set as a List the response because the Api RETURNS A List
                    var stock = tasks[0];//We assing the first of the List to the var stock value
                    if (stock != null)
                    {
                        return stock.ToStockFromFMPStockDTO();
                    }

                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
        #endregion

        //Solo tengo que llamar apis, para eso tengo que entender como va a ser la llamada a mi backEn y con que parametros es decir que es lo que espero
        //tner en cuenta promesas para esto por que tengo que esperar una respuesta de LAS apis al mismo tiempo y ahi ordenarlas.

        public async Task<List<FMPStockDto>> FindStocksToBeJoined(string companyName)
        {
            var stockProfileCompleteList = new List<FMPStockDto>();
            var encodedName = WebUtility.UrlEncode(companyName);
            var stockNameList = await _httpClient.GetAsync($"https://financialmodelingprep.com/stable/search-name?query={encodedName}&limit=10&apikey={_config["FMPKey"]}");

            if (stockNameList.IsSuccessStatusCode)
            {
                var content = await stockNameList.Content.ReadAsStringAsync();
                var stockList = JsonConvert.DeserializeObject<FMPStockDto[]>(content);

                foreach (var stockName in stockList!)
                {
                    var encodedSymbol = WebUtility.UrlEncode(stockName.symbol);

                    var stockProfileResponse = await _httpClient.GetAsync($"https://financialmodelingprep.com/stable/profile?symbol={encodedSymbol}&apikey={_config["FMPKey"]}");
                    var stockProfileContent = await stockProfileResponse.Content.ReadAsStringAsync();

                    if (stockProfileContent == "[]")
                    {
                        FMPStockDto stockProfileComplete = new FMPStockDto
                        {
                            symbol = stockName.symbol,
                            companyName = stockName.companyName,
                            currency = stockName.currency,
                            exchangeFullName = stockName.exchangeFullName,
                            exchange = stockName.exchange,
                            image = "",
                            defaultImage = false
                        };

                        stockProfileCompleteList.Add(stockProfileComplete);
                    }
                    else
                    {
                        var stockProfileTask = JsonConvert.DeserializeObject<FMPStockDto[]>(stockProfileContent);
                        var stockProfile = stockProfileTask![0];

                        FMPStockDto stockProfileComplete = new FMPStockDto
                        {
                            symbol = stockName.symbol,
                            companyName = stockName.companyName,
                            currency = stockName.currency,
                            exchangeFullName = stockName.exchangeFullName,
                            exchange = stockName.exchange,
                            image = stockProfile!.image,
                            defaultImage = stockProfile.defaultImage
                        };
                        stockProfileCompleteList.Add(stockProfileComplete);
                    }
                }
            }

            return stockProfileCompleteList;
        }
    }
}