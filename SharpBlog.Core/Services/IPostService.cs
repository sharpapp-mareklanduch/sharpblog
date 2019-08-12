using System.Collections.Generic;
using System.Threading.Tasks;
using SharpBlog.Core.Models;

namespace SharpBlog.Core.Services
{
    public interface IPostService
    {
	    Task<Post> AddOrUpdate(Post post);
	    Task<List<Post>> GetAllPublished();
	    Task<List<Post>> GetAll();
	    Task<Post> Get(int id);
	    Task Delete(int id);
    }
}