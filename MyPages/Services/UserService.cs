using Microsoft.EntityFrameworkCore;
using MyPages.Entities;
using MyPages.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyPages.Services
{
    public interface IUserService
    {
        Task<User> Authenticate(string username, string password);
        Task<User> Create(string username, string password);
        Task<IEnumerable<User>> GetAll();
        Task<User> GetById(int id);
        Task<User> GetByUsername(string username);
        Task Update(User userParam, string password = null);
        Task Delete(int id);
    }

    public class UserService : Service, IUserService
    {
        public UserService() : base() { }
        public UserService(DataContext context) : base(context) { }

        public async Task<User> Authenticate(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                throw new ApplicationException(Properties.resultMessages.CredentialsInvalid);

            var user = await _context.Users.SingleOrDefaultAsync(x => x.Username == username);

            if (user == null)
                throw new ApplicationException(Properties.resultMessages.CredentialsInvalid);

            if (!PasswordHelpers.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                throw new ApplicationException(Properties.resultMessages.CredentialsInvalid);

            return user;
        }

        public async Task<User> Create(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ApplicationException(Properties.resultMessages.PasswordNull);

            if (string.IsNullOrWhiteSpace(username))
                throw new ApplicationException(Properties.resultMessages.UsernameNull);

            if (_context.Users.Any(x => x.Username == username))
                throw new ApplicationException(Properties.resultMessages.UsernameExists);

            PasswordHelpers.CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            var folder = new Page
            {
                Name = username + " pages"
            };

            var user = new User
            {
                Username = username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                MainPage = folder
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetById(int id)
        {
            return await _context.Users.SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<User> GetByUsername(string username)
        {
            return await _context
                .Users
                .Include(x => x.MainPage)
                .SingleOrDefaultAsync(x => x.Username == username);
        }

        public async Task Update(User userParam, string password = null)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Id == userParam.Id);

            if (user == null)
                throw new ApplicationException(Properties.resultMessages.UserNotFound);

            if (userParam.Username != user.Username)
            {
                if (await _context.Users.AnyAsync(x => x.Username == userParam.Username))
                    throw new ApplicationException(Properties.resultMessages.UsernameExists);
            }

            user.Username = userParam.Username;

            if (!string.IsNullOrWhiteSpace(password))
            {
                PasswordHelpers.CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Id == id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
