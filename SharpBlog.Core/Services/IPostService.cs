using System.Collections.Generic;
using System.Threading.Tasks;
using SharpBlog.Core.Models;

namespace SharpBlog.Core.Services
{
    public interface IPostService
    {
	    Task<PostDto> AddOrUpdate(PostDto post);
		Task<IEnumerable<PostDto>> GetAll();
		Task<IEnumerable<PostDto>> GetByCategory(string name);
		Task<PostDto> Get(int id);
	    Task Delete(int id);
    }
}