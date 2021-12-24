﻿using DatingApp.DTOS;
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

        public async Task<IEnumerable<User>> GetUsers()
        {
            using (var _context = new DattingAppDbContext())
            {

                var users = await _context.User.Include(p => p.Photos).ToListAsync();

                return users;
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
