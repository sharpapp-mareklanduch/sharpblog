using System.Collections.Generic;
using System.Threading.Tasks;
using SharpBlog.Core.Dto;

namespace SharpBlog.Core.Dal
{
    public interface IPostDal
    {
	    Task<PostDto> AddOrUpdate(PostDto post);
		Task<IEnumerable<PostDto>> GetAll();
		Task<IEnumerable<PostDto>> GetByCategory(string name);
		Task<PostDto> Get(int id);
	    Task Delete(int id);
    }
}