using System.ComponentModel.DataAnnotations.Schema;

namespace JackOfAllCodes.Web.Models.Domain
{
    [Table("tag")]
    public class Tag
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public ICollection<BlogPost> BlogPost { get; set; }
    }
}
