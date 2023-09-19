using System.Data;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.EntityFrameworkCore;
using API.DTOs;
using api.Controllers;
using Api.Data;
using Api.Entities;
using Api.DTOs;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;

        private const string  USER_PASSWORD_ERROR_MESSAGE = "Usuario o password incorrecta";
        public AccountController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto)
        {
            if (await ValidateUser(registerDto.UserName)) return BadRequest("Username is taken");

            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = registerDto.UserName,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }


        [HttpPost ("login")]
        public async Task<ActionResult<AppUser>> Login(LoginDtos loginDtos)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => 
                x.UserName.ToLower() == loginDtos.UserName.ToLower());

            if(user == null) return Unauthorized(USER_PASSWORD_ERROR_MESSAGE);

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var  computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDtos.Password)); 

            for(int i =0 ; i <computedHash.Length; i++)
            {
                if(computedHash[i] != user.PasswordHash[i]) return Unauthorized(USER_PASSWORD_ERROR_MESSAGE);
            }

            return user;
        }
        private async Task<bool> ValidateUser(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}