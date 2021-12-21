using AutoMapper;
using DatingApp.DTOS;
using DatingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Helpers
{
    public class AutoMaperProfile : Profile
    {
        public AutoMaperProfile()
        {
            CreateMap<User, UserListDTO>()
                .ForMember(dest => dest.PhotoUrl, opt =>
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url));
                //.ForMember(dest => dest.Age, opt =>
                //    opt.MapFrom(src => src?.DateOfBirth?.CalculateAge()));
            CreateMap<User, UserDetailsDTO>()
                .ForMember(dest => dest.PhotoUrl, opt =>
                opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url));
            CreateMap<Photo, PhotoDTO>();
            CreateMap<UserForUpdateDTO, User>();
        }
    }
}
