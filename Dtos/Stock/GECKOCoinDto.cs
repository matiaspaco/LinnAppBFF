using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Stock
{
    public class GECKOCoinDto
    {
        public class Category
        {
            public string id { get; set; }
            public string name { get; set; }
        }

        public class Coin
        {
            public string id { get; set; }
            public string name { get; set; }
            public string api_symbol { get; set; }
            public string symbol { get; set; }
            public int? market_cap_rank { get; set; }
            public string thumb { get; set; }
            public string large { get; set; }
        }

        public class Exchange
        {
            public string id { get; set; }
            public string name { get; set; }
            public string market_type { get; set; }
            public string thumb { get; set; }
            public string large { get; set; }
        }

        public class Nft
        {
            public string id { get; set; }
            public string name { get; set; }
            public string symbol { get; set; }
            public string thumb { get; set; }
        }

        public class Root
        {
            public List<Coin> coins { get; set; }
            public List<Exchange> exchanges { get; set; }
            public List<object> icos { get; set; }
            public List<Category> categories { get; set; }
            public List<Nft> nfts { get; set; }
        }
    }
}