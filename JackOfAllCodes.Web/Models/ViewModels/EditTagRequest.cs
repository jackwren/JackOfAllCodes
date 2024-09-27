namespace JackOfAllCodes.Web.Models.ViewModels
{
    public class EditTagRequest
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string DisplayName { get; set; }
    }
}
