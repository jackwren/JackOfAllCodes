
namespace JackOfAllCodes.Web.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IFileSystemService _fileSystemService;

        public FileUploadService(IFileSystemService fileSystemService)
        {
            _fileSystemService = fileSystemService;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folderPath)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid file");

            var rootPath = _fileSystemService.GetCurrentDirectory();
            var filePath = Path.Combine(rootPath, $"wwwroot/images/{folderPath}", file.FileName);
            var directory = Path.GetDirectoryName(filePath);

            if (!_fileSystemService.DirectoryExists(directory))
            {
                _fileSystemService.CreateDirectory(directory);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/images/{folderPath}/{file.FileName}";
        }
    }
}
