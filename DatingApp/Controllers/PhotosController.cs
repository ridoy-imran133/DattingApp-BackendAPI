using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.DTOS;
using DatingApp.Helpers;
using DatingApp.Models;
using DatingApp.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDattingRepository _IDattingRepository;
        private readonly IMapper _IMapper;
        private readonly IOptions<CloudinarySettings> _ICloudinarySettings;
        public List<Photo> photos = new List<Photo>();

        private Cloudinary _cloudinary;
        public PhotosController(IDattingRepository iDattingRepository, IMapper iMapper,
                                IOptions<CloudinarySettings> iCloudinarySettings)
        {
            _IDattingRepository = iDattingRepository;
            _IMapper = iMapper;
            _ICloudinarySettings = iCloudinarySettings;
            this.photos = new List<Photo>();
            //Add Cloudinary Account
            Account acc = new Account(
                    _ICloudinarySettings.Value.CloudName,
                    _ICloudinarySettings.Value.APIKey,
                    _ICloudinarySettings.Value.APISecret
                );
            _cloudinary = new Cloudinary(acc);
        }

        [HttpGet("{id}")]
        [ActionName(nameof(GetPhoto))]
        //[HttpGet("/{id}", Name = nameof(GetPhoto))]
        public async Task<IActionResult> GetPhoto(string id)
        {
            var photoFromRepo = await _IDattingRepository.GetPhoto(id);

            var photo = _IMapper.Map<PhotoForReturnDTO>(photoFromRepo);
            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(string userId,
            [FromForm]PhotoForCreationDTO photoForCreationDTO)
        {
            var userFromRepo = await _IDattingRepository.GetUser(userId);

            var file = photoForCreationDTO.File;

            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }

            photoForCreationDTO.Url = uploadResult.Uri.ToString();
            photoForCreationDTO.PublicId = uploadResult.PublicId;

            var photo = _IMapper.Map<Photo>(photoForCreationDTO);
            photo.UserId = userId;

            if (!userFromRepo.Photos.Any(u => u.IsMain))
                photo.IsMain = true;

            userFromRepo.Photos.Add(photo);
            foreach (Photo p in userFromRepo.Photos)
            {
                photos.Add(p);
            }
            
            bool retValue = _IDattingRepository.SavePhotos(userId, photos);
            if (retValue)
            {
                var photoToReturn = _IMapper.Map<PhotoForReturnDTO>(photo);
                //return CreatedAtRoute("GetPhoto", new { id = photo.Id }, photoToReturn);
                return CreatedAtAction(nameof(GetPhoto), new { id = photo.Id }, photoToReturn);
            }

            return BadRequest("Photo Will not saved");

        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(string userId, string id)
        {
            var userFromRepo = await _IDattingRepository.GetUser(userId);

            if (!userFromRepo.Photos.Any(p => p.Id == id))
                return Unauthorized();
            var photoFromRepo = await _IDattingRepository.GetPhoto(id);
            if (photoFromRepo.IsMain)
                return BadRequest("This is already the main photo");

            var currentMainPhoto = await _IDattingRepository.GetMainPhotoForUser(userId);
            if(currentMainPhoto != null)
            {
                currentMainPhoto.IsMain = false;
                _IDattingRepository.UpdatePhoto(currentMainPhoto);
            }
            photoFromRepo.IsMain = true;
            bool val = _IDattingRepository.UpdatePhoto(photoFromRepo);

            if (val)
            {
                return NoContent();
            }
            return BadRequest("Could not set photo to main");

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(string userId, string id)
        {
            var userFromRepo = await _IDattingRepository.GetUser(userId);

            if (!userFromRepo.Photos.Any(p => p.Id == id))
                return Unauthorized();
            var photoFromRepo = await _IDattingRepository.GetPhoto(id);
            if (photoFromRepo.IsMain)
                return BadRequest("You can not delete your main photo");

            if (photoFromRepo.PublicId != null)
            {
                var deleteParams = new DeletionParams(photoFromRepo.PublicId);

                var result = _cloudinary.Destroy(deleteParams);
                if (result.Result == "ok")
                {
                    bool val = _IDattingRepository.DeletePhoto(photoFromRepo);
                    if (val)
                    {
                        return Ok();
                    }
                }
            }

            if(photoFromRepo.PublicId == null)
            {
                bool val = _IDattingRepository.DeletePhoto(photoFromRepo);
                if (val)
                {
                    return Ok();
                }
            }

            return BadRequest("Failed to Delete");
        }
    }
}