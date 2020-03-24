using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SharpBlog.Core.Models;
using SharpBlog.Database;

namespace SharpBlog.Core.Services.Implementation
{
	public class UserService : IUserService
	{
		private readonly BlogContext _blogContext;
		private readonly IConfiguration _config;
		private readonly IHashService _hashService;

		public UserService(
			BlogContext blogContext,
			IConfiguration config,
			IHashService hashService)
		{
			_blogContext = blogContext;
			_config = config;
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
				PasswordHash = _hashService.Generate(user.Password)
			});

			await _blogContext.SaveChangesAsync();
		}

		public async Task<UserDto> Get(string email)
		{
			var entity = await _blogContext.Users.FirstOrDefaultAsync(u => u.Email == email);
			return new UserDto
			{
				Name = entity.Name,
				Email = entity.Email
			};
		}

		public async Task<bool> IsNewUserRequired()
		{
			bool.TryParse(_config["user:reset"], out var resetUserConfig);
			return resetUserConfig || !(await _blogContext.Users.AnyAsync());
		}

		public async Task<bool> ValidateUser(string email, string password)
		{
			return await VerifyPasswordHash(password);
		}

		private async Task<bool> VerifyPasswordHash(string password)
		{
			return _hashService.Generate(password) == (await _blogContext.Users.FirstOrDefaultAsync()).PasswordHash;
		}
	}
}
