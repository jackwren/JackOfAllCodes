using System.ComponentModel.DataAnnotations.Schema;

namespace JackOfAllCodes.Web.Models.Domain
{
    [Table("comment")]
    public class Comment
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid BlogPostId { get; set; }
        public BlogPost BlogPost { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string UserName { get; set; }
        public bool IsVisible { get; set; }
        public ICollection<Like> Likes { get; set; }
    }
}
