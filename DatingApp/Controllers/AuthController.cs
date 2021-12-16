using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.common;
using DatingApp.DTOS;
using DatingApp.Models;
using DatingApp.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _IAuthRepository;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthRepository IAuthRepository, IConfiguration configuration)
        {
            _IAuthRepository = IAuthRepository;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(UserForRegisterDTO userForRegister)
        {
            userForRegister.Username = userForRegister.Username.ToLower();

            if (await _IAuthRepository.UserExists(userForRegister.Username))
                return BadRequest("Username Already Exists");

            User user = new User();
            user.Username = userForRegister.Username;
            var createdUser = _IAuthRepository.Register(user, userForRegister.Password);
            return StatusCode(201);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(UserForLoginDTO userForLoginDTO)
        {

                var userExist = await _IAuthRepository.Login(userForLoginDTO.Username.ToLower(), userForLoginDTO.Password);

                if (userExist == null)
                    return Unauthorized();

                var claims = new[]
                {
                new Claim(ClaimTypes.NameIdentifier, userExist.Id),
                new Claim(ClaimTypes.Name, userExist.Username)
            };

                string checkVal = _configuration.GetSection("AppSettings:Token").Value;
                byte[] val = Encoding.UTF8.GetBytes(checkVal);
                var key = new SymmetricSecurityKey(val);

                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddDays(1),
                    SigningCredentials = credentials
                };

                var tokenHandler = new JwtSecurityTokenHandler();

                var token = tokenHandler.CreateToken(tokenDescriptor);

                return Ok(new
                {
                    token = tokenHandler.WriteToken(token)
                });
        }
    }
}