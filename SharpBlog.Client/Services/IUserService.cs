using SharpBlog.Client.ViewModels.Account;
using System.Threading.Tasks;

namespace SharpBlog.Client.Services
{
    public interface IUserService
    {
        Task<bool> IsNewUserNeeded();
        Task RegisterUser(RegisterViewModel registerViewModel);
    }
}
