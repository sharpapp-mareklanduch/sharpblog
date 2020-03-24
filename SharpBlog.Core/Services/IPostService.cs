using System.Collections.Generic;
using System.Threading.Tasks;
using SharpBlog.Core.Models;

namespace SharpBlog.Core.Services
{
    public interface IPostService
    {
	    Task<PostDto> AddOrUpdate(PostDto post);
	    Task<List<PostDto>> GetAll();
	    Task<PostDto> Get(int id);
	    Task Delete(int id);
    }
}