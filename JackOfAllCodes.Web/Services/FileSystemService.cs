namespace JackOfAllCodes.Web.Services
{
    public class FileSystemService : IFileSystemService
    {
        public string GetCurrentDirectory() => Directory.GetCurrentDirectory();

        public async Task CopyFileAsync(Stream sourceStream, string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await sourceStream.CopyToAsync(fileStream);
            }
        }

        public bool DirectoryExists(string path) => Directory.Exists(path);

        public void CreateDirectory(string path) => Directory.CreateDirectory(path);
    }
}
