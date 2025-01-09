namespace JackOfAllCodes.Web.Services
{
    public interface IFileSystemService
    {
        string GetCurrentDirectory();
        Task CopyFileAsync(Stream sourceStream, string filePath);
        bool DirectoryExists(string path);
        void CreateDirectory(string path);
        void DeleteDirectory(string path);
    }
}
