using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using api.Interfaces;
using api.Models;
using Microsoft.IdentityModel.Tokens;

namespace api.Service
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        private readonly SymmetricSecurityKey _key;// it's the key from the appsettings.json
        public TokenService(IConfiguration configuration)//this IConfiguration is accessing to appsettings.json configuration since we are going to use some config from there
        {
            _configuration = configuration;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SigningKey"]));
        }
        public string CreateToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, user.UserName)
            };

            //singing credentials

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),//parameter mail and user name
                Expires = DateTime.Now.AddDays(7),//lifetime of the bearer token
                SigningCredentials = creds,//the hash of the key
                Issuer = _configuration["JWT:Issuer"],//configuration in the appsettings.json
                Audience = _configuration["JWT:Audience"]//configuration in the appsettings.json
            };

            var tokenHandler = new JwtSecurityTokenHandler();//here we have the instance the object to acces tthe methods to create the token

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);//writenToken return the token like a string 

        }
    }
}