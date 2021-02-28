using System;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;

namespace SharpBlog.Common.Services.Implementation
{
	public class HashService : IHashService
	{
		private readonly IConfiguration _config;

		public HashService(IConfiguration config)
		{
			_config = config;
		}

		public string Generate(string text)
		{
			var salt = Encoding.UTF8.GetBytes(_config["user:salt"]);

			var hashBytes = KeyDerivation.Pbkdf2(
				text,
				salt,
				KeyDerivationPrf.HMACSHA256,
				10000,
				32);

			return BitConverter.ToString(hashBytes);
		}
	}
}
