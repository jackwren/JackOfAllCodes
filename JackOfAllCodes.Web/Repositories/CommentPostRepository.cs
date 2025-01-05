using JackOfAllCodes.Web.DataAccess;
using JackOfAllCodes.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace JackOfAllCodes.Web.Repositories
{
    public class CommentPostRepository : ICommentPostRepository
    {
        private readonly BlogPostDBContext blogPostDBContext;

        public CommentPostRepository(BlogPostDBContext blogPostDBContext)
        {
            this.blogPostDBContext = blogPostDBContext;
        }

        public async Task<Comment?> GetAsync(Guid blogPostId, Guid commentId)
        {
            return await blogPostDBContext.Comment.FirstOrDefaultAsync(x => x.BlogPostId == blogPostId && x.Id == commentId);
        }

        public async Task AddAsync(Comment comment)
        {
            await blogPostDBContext.Comment.AddAsync(comment);
            await blogPostDBContext.SaveChangesAsync();
            return;
        }

        public async Task DeleteAsync(Comment comment)
        {
            blogPostDBContext.Comment.Remove(comment);
            await blogPostDBContext.SaveChangesAsync();
            return;
        }
    }
}
