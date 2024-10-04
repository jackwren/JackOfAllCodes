using JackOfAllCodes.Web.DataAccess;
using JackOfAllCodes.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace JackOfAllCodes.Web.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly BlogPostDBContext blogPostDbContext;

        public TagRepository(BlogPostDBContext blogPostDbContext)
        {
            this.blogPostDbContext = blogPostDbContext;
        }

        public async Task<Tag> AddAsync(Tag tag)
        {
            await blogPostDbContext.Tag.AddAsync(tag);
            await blogPostDbContext.SaveChangesAsync();
            return tag;
        }

        public async Task<Tag?> DeleteAsync(Guid id)
        {
            var data = await blogPostDbContext.Tag.FindAsync(id);

            if (data != null)
            {
                blogPostDbContext.Tag.Remove(data);
                await blogPostDbContext.SaveChangesAsync();
                return data;
            }
            else
            {
                return null;
            }
        }

        public async Task<IEnumerable<Tag>> GetAllAsync()
        {
            return await blogPostDbContext.Tag.ToListAsync();
        }

        public async Task<Tag?> GetAsync(Guid id)
        {
            return await blogPostDbContext.Tag.FirstOrDefaultAsync(x => x.Id == id);

        }

        public async Task<Tag?> UpdateAsync(Tag tag)
        {
            var currentTag = await blogPostDbContext.Tag.FindAsync(tag.Id);
            if (currentTag != null)
            {
                currentTag.Name = tag.Name;
                currentTag.DisplayName = tag.DisplayName;

                await blogPostDbContext.SaveChangesAsync();
                return currentTag;
            }
            else
            {
                return null;
            }
        }
    }
}
