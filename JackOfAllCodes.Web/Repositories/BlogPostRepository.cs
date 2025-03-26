using JackOfAllCodes.Web.DataAccess;
using JackOfAllCodes.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace JackOfAllCodes.Web.Repositories
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly BlogPostDBContext blogPostDbContext;

        public BlogPostRepository(BlogPostDBContext blogPostDbContext)
        {
            this.blogPostDbContext = blogPostDbContext;
        }

        public async Task<BlogPost> AddAsync(BlogPost blogPost)
        {
            await blogPostDbContext.AddAsync(blogPost);
            await blogPostDbContext.SaveChangesAsync();
            return blogPost;

        }

        public async Task<BlogPost?> DeleteAsync(Guid id)
        {
            var blogPost = await blogPostDbContext.BlogPost.FindAsync(id);

            if (blogPost != null)
            {
                blogPostDbContext.BlogPost.Remove(blogPost);
                await blogPostDbContext.SaveChangesAsync();
                return blogPost;
            }

            return null;
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync()
        {
            return await blogPostDbContext.BlogPost
                .Include(x => x.Tag)
                .Include(x => x.Likes)
                .Include(x => x.Comments)
                    .ThenInclude(c => c.Likes)
                .ToListAsync();
        }

        public async Task<BlogPost?> GetAsync(Guid id)
        {
            return await blogPostDbContext.BlogPost
                .Include(x => x.Tag)
                .Include(x => x.Likes)
                .Include(x => x.Comments)
                    .ThenInclude(c => c.Likes)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<BlogPost?> GetUrlHandleAsync(string urlHandle)
        {
            return await blogPostDbContext.BlogPost
                .Include(x => x.Tag)
                .Include(x => x.Likes)
                .Include(x => x.Comments)
                    .ThenInclude(c => c.Likes)
                .FirstOrDefaultAsync(x => x.UrlHandle == urlHandle);
        }

        public async Task<BlogPost?> UpdateAsync(BlogPost blogPost)
        {
            //Use database call to find if blogpost exists with passed in ID.
            //create new object with this data, to be updated later
            var existingBlog = await blogPostDbContext.BlogPost
                .Include(x => x.Tag)
                .Include(x => x.Likes)
                .Include(x => x.Comments)
                    .ThenInclude(c => c.Likes)
                .FirstOrDefaultAsync(x => x.Id == blogPost.Id);

            //if it does exist, update the values in new existingBlog object. 
            if (existingBlog != null)
            {
                existingBlog.Id = blogPost.Id;
                existingBlog.Heading = blogPost.Heading;
                existingBlog.PageTitle = blogPost.PageTitle;
                existingBlog.Content = blogPost.Content;
                existingBlog.ShortDescription = blogPost.ShortDescription;
                existingBlog.FeaturedImageUrl = blogPost.FeaturedImageUrl;
                existingBlog.UrlHandle = blogPost.UrlHandle;
                existingBlog.PublishedDate = blogPost.PublishedDate;
                existingBlog.Author = blogPost.Author;
                existingBlog.Visible = blogPost.Visible;
                existingBlog.Tag = blogPost.Tag;

                await blogPostDbContext.SaveChangesAsync();
                return existingBlog;
            };

            return null;
        }
    }
}
