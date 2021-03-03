using System.Threading.Tasks;
using SharpBlog.Core.Models;

namespace SharpBlog.Core.Dal
{
	public interface IUserDal
	{
		Task RegisterUser(UserDto user);
		Task<UserDto> GetAdmin();
		Task<bool> UserExists();
		Task<bool> ValidateUser(string email, string password);
		Task<bool> ChangePassword(string oldPassword, string newPassword, string confirmNewPassword);
		Task<bool> ValidatePasswordChange(string lastPasswordChangeDate);
        Task UpdateUser(string name, string email);
	}
}
	