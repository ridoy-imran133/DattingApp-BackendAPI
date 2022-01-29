using DatingApp.DTOS;
using DatingApp.Helpers;
using DatingApp.Models;
using DatingApp.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Repository.Implementation
{
    public class DattingRepository : IDattingRepository
    {
        //private readonly DattingAppDbContext _context;

        //public DattingRepository(DattingAppDbContext context)
        //{
        //    _context = context;
        //}

        public void Add<T>(T entity) where T : class
        {
            using (var _context = new DattingAppDbContext())
            {
                _context.Add(entity);
            }
        }

        public void Delete<T>(T entity) where T : class
        {
            using (var _context = new DattingAppDbContext())
            {
                _context.Remove(entity);
            }
        }

        public async Task<Photo> GetPhoto(string id)
        {
            using (var _context = new DattingAppDbContext())
            {
                var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);
                return photo;
            }
        }

        public async Task<User> GetUser(string pUserId)
        {
            using (var _context = new DattingAppDbContext())
            {
                var user = await _context.User.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == pUserId);

                return user;
            }
        }

        //public async Task<IEnumerable<User>> GetUsers()
        //{
        //    using (var _context = new DattingAppDbContext())
        //    {

        //        var users = await _context.User.Include(p => p.Photos).ToListAsync();

        //        return users;
        //    }
        //}

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            using (var _context = new DattingAppDbContext())
            {
                // Normally use
                //var users = _context.User.Include(p => p.Photos);

                //For Sorting
                var users = _context.User.Include(p => p.Photos).OrderByDescending(u => u.LastActive).AsQueryable();
                users = users.Where(u => u.Id != userParams.UserId);
                users = users.Where(u => u.Gender == userParams.Gender);
                if(userParams.MinAge !=18 || userParams.MaxAge != 99)
                {
                    var minDOB = DateTime.Today.AddYears(-userParams.MaxAge - 1);
                    var maxDOB = DateTime.Today.AddYears(-userParams.MinAge);

                    users = users.Where(u => u.DateOfBirth >= minDOB && u.DateOfBirth <= maxDOB);
                }

                if (!string.IsNullOrEmpty(userParams.OrderBy))
                {
                    switch (userParams.OrderBy)
                    {
                        case "created":
                            users = users.OrderByDescending(u => u.CreatedDate);
                            break;
                        default:
                            users = users.OrderByDescending(u => u.LastActive);
                            break;
                    }
                }

                //Pagination
                return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
            }
        }

        public async Task<bool> SaveAll()
        {
            using (var _context = new DattingAppDbContext())
            {
                return await _context.SaveChangesAsync() > 0;
            }
        }

        public bool SaveEditProfileData(UserForUpdateDTO userForUpdate)
        {
            using (var _context = new DattingAppDbContext())
            {
                var user = _context.User.FirstOrDefault(x => x.Id == userForUpdate.Id);
                user.Introduction = userForUpdate.Introduction;
                user.LookingFor = userForUpdate.LookingFor;
                user.Interests = userForUpdate.Interests;
                user.City = userForUpdate.City;
                user.Country = userForUpdate.Country;

                _context.User.Update(user);
                _context.SaveChanges();
                return true;
            }
        }

        public void SaveLogUserActivity(User user)
        {
            using (var _context = new DattingAppDbContext())
            {
                _context.User.Update(user);
                _context.SaveChanges();
            }
        }

        public bool SavePhotos(string userId, IList<Photo> photos)
        {
            using (var _context = new DattingAppDbContext())
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        IList<Photo> allPhotos = _context.Photos.Where(x => x.UserId == userId).ToList();
                        _context.Photos.RemoveRange(allPhotos);

                        foreach(Photo p in photos)
                        {
                            Photo photo = new Photo();
                            photo.Id = Guid.NewGuid().ToString();
                            photo.Url = p.Url;
                            photo.UserId = userId;
                            photo.Description = p.Description;
                            photo.DateAdded = p.DateAdded;
                            photo.PublicId = p.PublicId;
                            photo.IsMain = p.IsMain;
                            _context.Photos.AddRange(photo);
                            _context.SaveChanges();
                            _context.Entry(photo).State = EntityState.Detached;
                        }
                        _context.SaveChanges();
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        public bool SaveSinglePhotos(string userId, Photo p)
        {
            using (var _context = new DattingAppDbContext())
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {                                       
                        Photo photo = new Photo();
                        photo.Id = Guid.NewGuid().ToString();
                        photo.Url = p.Url;
                        photo.UserId = userId;
                        photo.Description = p.Description;
                        photo.DateAdded = p.DateAdded;
                        photo.PublicId = p.PublicId;
                        photo.IsMain = p.IsMain;
                        _context.Photos.Add(photo);
                        _context.SaveChanges();
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        public async Task<Photo> GetMainPhotoForUser(string userId)
        {
            using (var _context = new DattingAppDbContext())
            {
                return await _context.Photos.Where(u => u.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
            }
        }

        public bool UpdatePhoto(Photo photo)
        {
            using (var _context = new DattingAppDbContext())
            {
                _context.Photos.Update(photo);
                _context.SaveChanges();
                return true;
            }
        }

        public bool DeletePhoto(Photo photo)
        {
            using (var _context = new DattingAppDbContext())
            {
                _context.Remove(photo);
                _context.SaveChanges();
                return true;
            }
        }
    }
}
