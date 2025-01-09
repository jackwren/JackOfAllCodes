namespace JackOfAllCodes.Web.Services
{
    public interface IFileUploadService
    {
        Task<string> UploadFileAsync(IFormFile file, string folderPath);
    }
}