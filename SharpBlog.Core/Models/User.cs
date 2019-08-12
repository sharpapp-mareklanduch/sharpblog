using System.ComponentModel.DataAnnotations;

namespace SharpBlog.Core.Models
{
    public class User
    {
		public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
