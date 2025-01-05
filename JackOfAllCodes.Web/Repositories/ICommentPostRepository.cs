using JackOfAllCodes.Web.Models.Domain;

namespace JackOfAllCodes.Web.Repositories
{
    public interface ICommentPostRepository
    {
        Task<Comment?> GetAsync(Guid blogPostId, Guid commentId);
        Task AddAsync(Comment comment);
        Task DeleteAsync(Comment comment);
    }
}