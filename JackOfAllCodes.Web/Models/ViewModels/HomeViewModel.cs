using JackOfAllCodes.Web.Models.Domain;

namespace JackOfAllCodes.Web.Models.ViewModels
{
    public class HomeViewModel
    {
        public required IEnumerable<Tag> Tags { get; set; }
        public required IEnumerable<BlogPost> BlogPosts { get; set; }
    }
}
