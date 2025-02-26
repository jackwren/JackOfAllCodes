using System.ComponentModel.DataAnnotations;

namespace JackOfAllCodes.Web.Models.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}