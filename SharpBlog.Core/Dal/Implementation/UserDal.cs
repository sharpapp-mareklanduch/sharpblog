using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SharpBlog.Common.Models;
using SharpBlog.Common.Services;
using SharpBlog.Database;

namespace SharpBlog.Common.Dal.Implementation
{
    public class UserDal : IUserDal
    {
        private readonly BlogContext _blogContext;
        private readonly IHashService _hashService;

        public UserDal(
            BlogContext blogContext,
            IHashService hashService)
        {
            _blogContext = blogContext;
            _hashService = hashService;
        }

        public async Task RegisterUser(UserDto user)
        {
            var userEntity = await _blogContext.Users.FirstOrDefaultAsync();
            if (userEntity != null)
            {
                _blogContext.Users.Remove(userEntity);
            }

            await _blogContext.Users.AddAsync(new Database.Models.User()
            {
                Name = user.Name,
                Email = user.Email,
                PasswordHash = _hashService.Generate(user.Password),
                LastPasswordChangeDate = DateTime.UtcNow
            });

            await _blogContext.SaveChangesAsync();
        }

        public async Task<UserDto> GetAdmin()
        {
            var entity = await _blogContext.Users.FirstOrDefaultAsync();
            if (entity == null)
            {
                return null;
            }
            return new UserDto
            {
                Name = entity.Name,
                Email = entity.Email,
                LastPasswordChangeDate = entity.LastPasswordChangeDate
            };
        }

        public async Task<bool> UserExists()
        {
            return await _blogContext.Users.AnyAsync();
        }

        public async Task<bool> ValidateUser(string email, string password)
        {
            var entity = await _blogContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (entity == null)
            {
                return false;
            }
            if (entity.Email != email) {
                return false;
            }
            return VerifyPasswordHash(password, entity.PasswordHash);
        }

        public async Task<bool> ChangePassword(string oldPassword, string newPassword, string confirmNewPassword)
        {
            var userEntity = await _blogContext.Users.FirstOrDefaultAsync();
            if (userEntity == null)
            {
                return false;
            }

            var oldPasswordHash = _hashService.Generate(oldPassword);
            if (oldPasswordHash != userEntity.PasswordHash)
            {
                return false;
            }

            if (newPassword != confirmNewPassword)
            {
                return false;
            }

            var newPasswordHash = _hashService.Generate(newPassword);
            userEntity.PasswordHash = newPasswordHash;
            userEntity.LastPasswordChangeDate = DateTime.UtcNow;
            await _blogContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ValidatePasswordChange(string passwordChangeDate)
        {
            var userEntity = await _blogContext.Users.FirstOrDefaultAsync();
            if (userEntity == null)
            {
                return true;
            }
            var passwordChangeDateTime = DateTime.Parse(passwordChangeDate);

            return passwordChangeDateTime.Date <= userEntity.LastPasswordChangeDate.Date
            && Math.Floor(passwordChangeDateTime.TimeOfDay.TotalSeconds) < Math.Floor(userEntity.LastPasswordChangeDate.TimeOfDay.TotalSeconds);
        }

        public async Task UpdateUser(string name, string email)
        {
            var userEntity = await _blogContext.Users.FirstOrDefaultAsync();
            if (userEntity == null)
            {
                return;
            }

            userEntity.Name = name;
            userEntity.Email = email;
            await _blogContext.SaveChangesAsync();
        }

        private bool VerifyPasswordHash(string password, string passwordHash)
        {
            return _hashService.Generate(password) == passwordHash;
        }
    }
}
