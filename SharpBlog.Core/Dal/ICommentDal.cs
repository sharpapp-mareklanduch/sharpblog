using System.Threading.Tasks;
using SharpBlog.Core.Models;

namespace SharpBlog.Core.Dal
{
    public interface ICommentDal
    {
        Task<CommentDto> Add(CommentDto comment);
        Task Delete(int id);
    }
}