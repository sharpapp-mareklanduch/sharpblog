using System;

namespace SharpBlog.Database.Models
{
	public class User : Entity<int>
	{
		public string Name { get; set; }
		public string Email { get; set; }
		public string PasswordHash { get; set; }
        public DateTime LastPasswordChangeDate { get; set; }
	}
}
