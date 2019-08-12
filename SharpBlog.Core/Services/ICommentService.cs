using System.Threading.Tasks;
using SharpBlog.Core.Models;

namespace SharpBlog.Core.Services
{
    public interface ICommentService
    {
        Task<Comment> Add(Comment comment);
        Task Delete(int id);
    }
}