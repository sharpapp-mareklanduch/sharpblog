using System.Threading.Tasks;
using SharpBlog.Core.Models;

namespace SharpBlog.Core.Services
{
	public interface IUserService
	{
		Task RegisterUser(User user);
		Task<User> Get(string email);
		Task<bool> IsNewUserRequired();
		Task<bool> ValidateUser(string email, string password);
	}
}
	