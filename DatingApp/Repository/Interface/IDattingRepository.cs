using DatingApp.DTOS;
using DatingApp.Helpers;
using DatingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Repository.Interface
{
    public interface IDattingRepository
    {
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;

        Task<bool> SaveAll();
        public bool SaveEditProfileData(UserForUpdateDTO userForUpdate);
        //Task<IEnumerable<User>> GetUsers();

        // For Pagination
        Task<PagedList<User>> GetUsers(UserParams userParams);
        Task<User> GetUser(string pUserId);
        Task<Photo> GetPhoto(string id);
        bool SavePhotos(string userId ,IList<Photo> photos);
        bool SaveSinglePhotos(string userId, Photo p);
        Task<Photo> GetMainPhotoForUser(string userId);
        bool UpdatePhoto(Photo photo);
        bool DeletePhoto(Photo photo);
        public void SaveLogUserActivity(User user);
        Task<Like> GetLike(string userId, string recipientId);
        bool SaveLike(string userId, string recipientId);
    }
}
