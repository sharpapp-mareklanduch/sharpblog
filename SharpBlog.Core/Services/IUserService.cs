using System.Threading.Tasks;
using SharpBlog.Core.Models;

namespace SharpBlog.Core.Services
{
	public interface IUserService
	{
		Task RegisterUser(UserDto user);
		Task<UserDto> GetAdmin();
		Task<bool> IsNewUserRequired();
		Task<bool> ValidateUser(string email, string password);
		Task<bool> ChangePassword(string oldPassword, string newPassword, string confirmNewPassword);
		Task<bool> ValidatePasswordChange(string lastPasswordChangeDate);
        Task UpdateUser(string name, string email);
	}
}
	