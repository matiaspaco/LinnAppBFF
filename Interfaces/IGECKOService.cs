using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Stock;

namespace api.Interfaces
{
    public interface IGECKOService
    {
        // Task<List<FMPStockDto>> FindCriptosAndJoinToCurrentList(string assetName, List<FMPStockDto> stocksList);
        Task<List<FMPStockDto>> FindCriptosAndJoinToCurrentList(string assetName);
    }
}