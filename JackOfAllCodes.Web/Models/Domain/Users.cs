namespace JackOfAllCodes.Web.Models.Domain
{
    public class Users
    {
        public int UserId { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
