using JackOfAllCodes.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace JackOfAllCodes.Web.DataAccess
{
    public class BlogPostDBContext : DbContext
    {
        public BlogPostDBContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Tag> tag { get; set; }

        // PostgresSQL sets columns to lowercase. Using FluentAPI to map correct properties.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tag>()
                .Property(t => t.Id)
                .HasColumnName("id");

            modelBuilder.Entity<Tag>()
                .Property(t => t.Name)
                .HasColumnName("name");

            modelBuilder.Entity<Tag>()
                .Property(t => t.DisplayName)
                .HasColumnName("displayname");
        }
    }
}
