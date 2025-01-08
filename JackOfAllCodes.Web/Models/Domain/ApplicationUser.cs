using Microsoft.AspNetCore.Identity;

namespace JackOfAllCodes.Web.Models.Domain
{
    public class ApplicationUser : IdentityUser
    {
        // You can add custom properties here if needed, like Full Name, Profile Picture, etc.
        public string? ProfilePictureUrl { get; set; } 
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<Like>? Likes { get; set; }
    }
}
