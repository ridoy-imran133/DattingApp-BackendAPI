using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DatingApp.Repository.Interface;
using AutoMapper;
using DatingApp.DTOS;

namespace DatingApp.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IDattingRepository _IDattingRepository;
        private readonly IMapper _IMapper;

        public UserController(IDattingRepository iDattingRepository, IMapper iMapper)
        {
            _IDattingRepository = iDattingRepository;
            _IMapper = iMapper;
        }

        [HttpGet]
        [Route("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _IDattingRepository.GetUsers();

            var userReturn = _IMapper.Map<IEnumerable<UserListDTO>>(users);

            return Ok(userReturn);
        }

        [HttpGet]
        [Route("user")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _IDattingRepository.GetUser(id);

            var userRet = _IMapper.Map<UserDetailsDTO>(user);

            return Ok(userRet);
        }
    }
}