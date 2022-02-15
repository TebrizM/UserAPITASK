using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserAPI.Apps.ManageApp.DTOs.AccountDtos;
using UserAPI.Data.Entities;
using UserAPI.Services;

namespace UserAPI.Apps.ManageApp.Controllers
{
    [Route("manage/api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IJwtService _service;

        public AccountsController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager, IConfiguration configuration, IJwtService service)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _service = service;
        }
        //[HttpGet("")]
        //public IActionResult CreateRole()
        //{
        //    var result1 = _roleManager.CreateAsync(new IdentityRole("Admin")).Result;
        //    var result2 = _roleManager.CreateAsync(new IdentityRole("Member")).Result;

        //    var user1 = new AppUser { FullName = "Tabriz Mammadov", UserName = "Wachter" };
        //    var user2 = new AppUser { FullName = "Super Admin", UserName = "SuperAdmin" };
        //    var result3 = _userManager.CreateAsync(user1, "Tebriz123").Result;
        //    var result4 = _userManager.CreateAsync(user2, "Admin123").Result;

        //    var result5 = _userManager.AddToRoleAsync(user1, "Member").Result;
        //    var result6 = _userManager.AddToRoleAsync(user2, "Admin").Result;


        //    return Ok();
        //}
        /// <summary>
        /// This endpoint returns a token
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST api/admin/accounts/login
        ///     {
        ///         "username":"SuperAdmin",
        ///         "password":"Admin123"
        ///     }
        /// </remarks>
        /// <param name="loginDto"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            AppUser user = await _userManager.FindByNameAsync(loginDto.UserName);

            if (user == null) return NotFound();

            if (!await _userManager.CheckPasswordAsync(user, loginDto.Password))
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);


            string tokenStr = _service.Generate(user, roles, _configuration);


            return Ok(new { token = tokenStr });
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {

            AppUser user = await _userManager.FindByNameAsync(registerDto.UserName);
            if (user != null) return BadRequest();


            user = new AppUser
            {
                FullName = registerDto.FullName,
                UserName = registerDto.UserName,
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded) return BadRequest();

            var roleResult = await _userManager.AddToRoleAsync(user, "Admin");

            if (!roleResult.Succeeded) return BadRequest();

            await _signInManager.SignInAsync(user, true);

            return Ok();
        }

        [HttpPost("editprofile")]
        public async Task<IActionResult> EditProfile(EditDto editDto)
        {
            if (editDto == null)
            {
                return NotFound();
            }

            AppUser member = await _userManager.FindByNameAsync(User.Identity.Name);


            if (member.UserName != editDto.UserName && _userManager.Users.Any(x => x.NormalizedUserName == editDto.UserName.ToUpper()))
            {
                return BadRequest("username is already exists");
            }
            if (member.Email != editDto.Email && _userManager.Users.Any(x => x.NormalizedEmail == editDto.Email.ToUpper()))
            {
                return BadRequest("email is already exists");
            }
            
            member.UserName = editDto.UserName;
            member.Email = editDto.Email;
            member.FullName = editDto.FullName;

            var result = await _userManager.UpdateAsync(member);

            return NoContent();
        }
    }
}
