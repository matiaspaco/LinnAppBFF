using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Account;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;

        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, SignInManager<AppUser> signInManager)//We initialize the constructor userManager for then implement the identity properties and funcionality
        {
            _userManager = userManager;
            _tokenService = tokenService;//We initialize the constructor tokenService to access
            _signInManager = signInManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.UserName.ToLower());

            if (user == null || user.LockoutEnabled)
            {
                return Unauthorized("User not found");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
            {                
                //await _userManager.AccessFailedAsync(user);
                return Unauthorized("User Name not found and/ or password incorrect.");
            }

            return Ok(new NewUserDto
            {
                UserName = user.UserName,
                Email = user.Email,
                Token = _tokenService.CreateToken(user)
            }
            );
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

        [HttpPut("update")]
        //[Authorize]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdateUserDto updateUserDto)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == updateUserDto.UserName.ToLower());
                if (user == null || user.LockoutEnabled)
                {
                    return BadRequest("user not found.");
                }

                bool checkedPsw = await _userManager.CheckPasswordAsync(user!, updateUserDto.CurrentPassword!);
                if (checkedPsw)
                {
                    var resultPswUpdate = await _userManager.ChangePasswordAsync(user!, updateUserDto.CurrentPassword!, updateUserDto.NewPassword!);
                    if (!resultPswUpdate.Succeeded)
                    {
                        return BadRequest("An error ocurred during the password update.");
                    }
                    return Ok(new
                    {
                        UserName = user!.UserName,
                        message = "Password updated successfully."
                    });
                }
                else
                {
                    //return BadRequest("The current password doesn't match");
                    
                    return BadRequest("The current password doesn't match");
                }
            }
            catch (System.Exception e)
            {

                return StatusCode(500, "An internal server error occurred.");
            }
        }

        [HttpDelete("delete")]
        //[Authorize]
        public async Task<IActionResult> DeleteUser([FromBody] DeleteUserDto deleteUser)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(value => value.UserName == deleteUser.UserName!.ToLower());
            if (user == null)
                return NotFound($"{deleteUser.UserName} user not found.");

            bool pswValid = await _userManager.CheckPasswordAsync(user, deleteUser.CurrentPassword!);
            if (!pswValid)
                return BadRequest($"{deleteUser.UserName} incorrect password.");

            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.MaxValue;

            var deletedObj = await _userManager.UpdateAsync(user);

            if (deletedObj != null)
            {
                return Ok(new { message = "User deleted successfully." });
            }
            else
            {
                return BadRequest(new { message = "Error during the User delete proccess." });
            }
        }
    }
}