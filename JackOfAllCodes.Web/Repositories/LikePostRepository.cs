using JackOfAllCodes.Web.DataAccess;
using JackOfAllCodes.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace JackOfAllCodes.Web.Repositories
{
    public class LikePostRepository : ILikePostRepository
    {
        private readonly BlogPostDBContext blogPostDbContext;

        public LikePostRepository(BlogPostDBContext blogPostDbContext)
        {
            this.blogPostDbContext = blogPostDbContext;
        }

        public async Task AddAsync(Like like)
        {
            await blogPostDbContext.Likes.AddAsync(like);
            await blogPostDbContext.SaveChangesAsync();
            return;
        }

        public async Task<Like?> GetAsync(Guid blogPostId, Guid userId)
        {
            var like = await blogPostDbContext.Likes.FirstOrDefaultAsync(l => l.BlogPostId == blogPostId && l.UserId == userId);

            if (like == null)
            {
                return null;
            }
            return like;
        }

        public async Task RemoveAsync(Like existingLike)
        {
            blogPostDbContext.Likes.Remove(existingLike);
            await blogPostDbContext.SaveChangesAsync();
            return;
        }

        public async Task<Like?> GetLikeCommentAsync(Guid commentId, Guid userId)
        {
            var like = await blogPostDbContext.Likes.FirstOrDefaultAsync(l => l.CommentId == commentId && l.UserId == userId);

            if (like == null)
            {
                return null;
            }
            return like;
        }
    }
}
