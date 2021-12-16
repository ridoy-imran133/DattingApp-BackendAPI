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

        public Task<bool> SaveAll()
        {
            throw new NotImplementedException();
        }
    }
}
