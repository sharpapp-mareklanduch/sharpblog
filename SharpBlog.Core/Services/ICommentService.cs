using System.Threading.Tasks;
using SharpBlog.Core.Models;

namespace SharpBlog.Core.Services
{
    public interface ICommentService
    {
        Task<CommentDto> Add(CommentDto comment);
        Task Delete(int id);
    }
}