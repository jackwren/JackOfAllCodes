using JackOfAllCodes.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace JackOfAllCodes.Web.DataAccess
{
    public class BlogPostDBContext : DbContext
    {
        public BlogPostDBContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<BlogPost> BlogPost { get; set; }

        public DbSet<Tag> Tag { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Loop through each entity type and set table and column names to lowercase
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                // Set the table name to lowercase
                entity.SetTableName(entity.GetTableName().ToLowerInvariant());

                // Set each property column name to lowercase
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(property.Name.ToLowerInvariant());
                }
            }
        }
    }
}
