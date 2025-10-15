using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Account;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;

        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService)//We initialize the constructor userManager for then implement the identity properties and funcionality
        {
            _userManager = userManager;
            _tokenService = tokenService;//We initialize the constructor tokenService to access
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var AppUser = new AppUser
                {
                    UserName = registerDto.UserName,
                    Email = registerDto.Email,
                };

                var createdUser = await _userManager.CreateAsync(AppUser, registerDto.Password);

                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(AppUser, "User");
                    if (roleResult.Succeeded)
                    {
                        //return Ok("User Created");
                        return Ok(
                            new NewUserDto
                            {
                                Email = AppUser.UserName,
                                UserName = AppUser.Email,
                                Token = _tokenService.CreateToken(AppUser)
                            }
                        );
                    }
                    else
                    {
                        return BadRequest("Error creating the user.");
                    }
                }
                else
                {
                    return StatusCode(500, createdUser.Errors);
                }
            }
            catch (System.Exception e)
            {
                return StatusCode(500, e);
            }
        }
    }
}