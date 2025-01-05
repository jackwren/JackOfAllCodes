using System.ComponentModel.DataAnnotations;

namespace JackOfAllCodes.Web.Models.ViewModels
{
    public class RegisterRequest
    {
        [Required]
        [StringLength(25, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
