using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    public class AccountsController : BaseApiController
    {
        public DataContext _context { get; set; }
        public ITokenService _tokenservice { get; set; }
        public AccountsController(DataContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenservice = tokenService;
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.UserName.ToLower()))
            {
                return BadRequest("Username is taken!");
            }
            using HMACSHA512 hmac = new HMACSHA512();

            AppUser newUser = new AppUser()
            {
                UserName = registerDto.UserName,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };
            _context.Add(newUser);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                UserName = newUser.UserName,
                Token = _tokenservice.CreateToken(newUser)
            };
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            AppUser user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == loginDto.UserName);
            if (user == null)
            {
                return Unauthorized();
            }
            using HMACSHA512 hmac = new HMACSHA512(user.PasswordSalt);

            byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                {
                    return Unauthorized("invalid password");
                }
            }
            return new UserDto
            {
                UserName = user.UserName,
                Token = _tokenservice.CreateToken(user)
            };
        }
        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
        }
    }
}
