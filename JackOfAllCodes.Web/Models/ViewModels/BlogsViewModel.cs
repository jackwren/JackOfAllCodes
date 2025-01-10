using JackOfAllCodes.Web.Models.Domain;

namespace JackOfAllCodes.Web.Models.ViewModels
{
    public class BlogsViewModel
    {
        public BlogPost BlogPost { get; set; }

        public IEnumerable<ApplicationUser> Users { get; set; }
    }
}
