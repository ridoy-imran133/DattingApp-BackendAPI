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
using System.Security.Claims;
using DatingApp.Models;
using DatingApp.Helpers;

namespace DatingApp.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
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
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            var vCurrentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            //for sorting
            var mUserFromRepo = await _IDattingRepository.GetUser(vCurrentUserId);
            userParams.UserId = vCurrentUserId;
            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = mUserFromRepo.Gender == "male" ? "female" : "male";
            }


            var users = await _IDattingRepository.GetUsers(userParams);

            var userReturn = _IMapper.Map<IEnumerable<UserListDTO>>(users);

            Response.AddPagination(users.CurrentPage, users.PageSize,
                users.TotalCount, users.TotalPage);
            return Ok(userReturn);
        }

        [HttpGet]
        [Route("user")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _IDattingRepository.GetUser(id);

            if (user != null)
            {
                var userRet = _IMapper.Map<UserDetailsDTO>(user);
                return Ok(userRet);
            }

            return BadRequest();
            
        }

        [HttpPost]
        [Route("updateUser")]
        public async Task<IActionResult> UpdateUser(UserForUpdateDTO userForUpdate)
        {
            bool val = _IDattingRepository.SaveEditProfileData(userForUpdate);
            //if(id != User.FindFirst(ClaimTypes.NameIdentifier).Value)
            //{
            //    return Unauthorized();
            //}

            //var userFromRepo = await _IDattingRepository.GetUser(id);
            //_IMapper.Map(userForUpdate, userFromRepo);
            //if (await _IDattingRepository.SaveAll())
            //    return NoContent();

            //throw new Exception($"Updating user {id} failed on save");

            return Ok(val);
        }

        [HttpPost]
        [Route("{id}/like/{receiptId}")]
        public async Task<IActionResult> LikeUser(string id, string receiptId)
        {
            if (id != User.FindFirst(ClaimTypes.NameIdentifier).Value)
                return Unauthorized();

            var like = await _IDattingRepository.GetLike(id, receiptId);

            if (like != null)
                return BadRequest("You already like this user.");

            if (await _IDattingRepository.GetUser(receiptId) == null)
                return NotFound();

            bool result = _IDattingRepository.SaveLike(id, receiptId);

            if (result)
                return Ok();
            return BadRequest("Failed to Like");
        }
    }
}