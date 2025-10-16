using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace api.Models
{
    public class AppUser : IdentityUser// we use the Identity user to acces all the properties from the EMAIL and we add some properties more here in the class
    {
        public List<Portfolio> Portfolios { get; set; } = new List<Portfolio>();//We add this since we are going to use a join to have a relation many to many with the Stocks and UserId 
    }
}