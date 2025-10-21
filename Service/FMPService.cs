using System;
using System.Collections.Generic;
using System.Linq;
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
                var result = await _httpClient.GetAsync($"https://financialmodelingprep.com/stable/profile?symbol={symbol}&apikey={_config["FMPKey"]}");//We took the API KEY value from the appsetting.json 
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
    }
}