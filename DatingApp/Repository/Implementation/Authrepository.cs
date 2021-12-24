using DatingApp.Models;
using DatingApp.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Repository.Implementation
{
    public class Authrepository : IAuthRepository
    {
        //private readonly DattingAppDbContext _dattingAppDbContext;
        //public Authrepository(DattingAppDbContext dattingAppDbContext)
        //{
        //    _dattingAppDbContext = dattingAppDbContext;
        //}
        public async Task<User> Login(string username, string password)
        {
            using (var _context = new DattingAppDbContext())
            {
                User user = new User();
                try
                {
                    user = await _context.User.Include(p => p.Photos).FirstOrDefaultAsync(x => x.Username == username);

                    if (user == null)
                        return null;
                    if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                        return null;                    
                }
                catch(Exception ex)
                {
                    Console.Out.WriteLine(ex.StackTrace);
                }
                return user;
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for (int i = 0; i<computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i]) return false;
                }
            }
            return true;

        }

        public async Task<User> Register(User user, string password)
        {
            using (var _context = new DattingAppDbContext())
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                user.Id = Guid.NewGuid().ToString();
                try
                {
                    await _context.AddAsync(user);
                    await _context.SaveChangesAsync();
                } catch(Exception ex)
                {
                    
                }

                return user;
            }
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(string username)
        {
            using (var _context = new DattingAppDbContext())
            {
                if (await _context.User.AnyAsync(x => x.Username == username))
                    return true;

                return false;
            }
        }
    }
}
