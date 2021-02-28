using System.Threading.Tasks;
using SharpBlog.Common.Models;

namespace SharpBlog.Common.Dal
{
    public interface ICommentDal
    {
        Task<CommentDto> Add(CommentDto comment);
        Task Delete(int id);
    }
}