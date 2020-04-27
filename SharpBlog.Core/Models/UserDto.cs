using System;

namespace SharpBlog.Core.Models
{
    public class UserDto
    {
		public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime LastPasswordChangeDate { get; set; }
    }
}
