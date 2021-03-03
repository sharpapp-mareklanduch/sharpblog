using SharpBlog.Client.ViewModels.Account;
using SharpBlog.Core.Dto;
using SharpBlog.Core.Dal;
using System.Threading.Tasks;

namespace SharpBlog.Client.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserDal _userDal;
        private readonly ISettingsService _settingsService;

        public UserService(
            IUserDal userDal,
            ISettingsService settingsService)
        {
            _userDal = userDal;
            _settingsService = settingsService;
        }

        public async Task<bool> IsNewUserNeeded()
        {
            return _settingsService.GetResetUser() || !(await _userDal.UserExists());
        }

        public async Task RegisterUser(RegisterViewModel model)
        {
            if (await IsNewUserNeeded())
            {
                var user = new UserDto
                {
                    Email = model.Email,
                    Name = model.Name,
                    Password = model.Password
                };

                await _userDal.RegisterUser(user);
                _settingsService.SetUserRegistered();
            }
        }
    }
}
