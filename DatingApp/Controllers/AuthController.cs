using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
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
        private readonly IDattingRepository _IDattingRepository;
        private readonly IConfiguration _configuration;
        private readonly IMapper _IMapper;

        public AuthController(IAuthRepository IAuthRepository, IConfiguration configuration, IMapper mapper, IDattingRepository dattingRepository)
        {
            _IAuthRepository = IAuthRepository;
            _IDattingRepository = dattingRepository;
            _configuration = configuration;
            _IMapper = mapper;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(UserForRegisterDTO userForRegister)
        {
            try
            {
                userForRegister.Username = userForRegister.Username.ToLower();

                if (await _IAuthRepository.UserExists(userForRegister.Username))
                    return BadRequest("Username Already Exists");

                var userToCreate = _IMapper.Map<User>(userForRegister);

                //User user = new User();
                //user.Username = userForRegister.Username;
                if (userToCreate != null)
                    userToCreate.Age = DateTime.Now.Subtract(Convert.ToDateTime(userToCreate.DateOfBirth)).Days / 365;
                var createdUser = await _IAuthRepository.Register(userToCreate, userForRegister.Password);

                var userToReturn = _IMapper.Map<UserDetailsDTO>(createdUser);

                string currentUserId = userToReturn.Id;

                //var val = CreatedAtRoute("https://localhost:44353/User/user", new { controller = "User", id = createdUser.Id }, userToReturn);
                var user = await _IDattingRepository.GetUser(currentUserId);
                return Ok(user);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }            
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

                var user = _IMapper.Map<UserListDTO>(userExist);

                return Ok(new
                {
                    token = tokenHandler.WriteToken(token),
                    user
                });
        }
    }
}