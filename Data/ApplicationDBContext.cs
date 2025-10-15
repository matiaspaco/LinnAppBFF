using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    // public class ApplicationDBContext : DbContext
    public class ApplicationDBContext : IdentityDbContext<AppUser>//this new implementation belongs to the identity library which allow us to use EMAIL to handling users with JWT is the same DBContext but adapted for emails users
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }

        public DbSet<Stock> Stocks { get; set; } // DbSet is used to manipulate the tables
        public DbSet<Comment> Comments { get; set; }

        #region identity Roles Implementation for the DB to Register Users
        protected override void OnModelCreating(ModelBuilder builder)//OnModelCreating belongs to Identity library
        {
            base.OnModelCreating(builder);
            //We define here the types of roles that we are going to use
            List<IdentityRole> roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Name = "User",
                    NormalizedName = "USER"
                }
            };
            builder.Entity<IdentityRole>().HasData(roles);
        }
        #endregion
    }
}