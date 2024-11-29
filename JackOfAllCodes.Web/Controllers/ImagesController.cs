using JackOfAllCodes.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace JackOfAllCodes.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IFileSystemService _fileSystem;

        public ImagesController(IFileSystemService fileSystem)
        {
            _fileSystem = fileSystem;
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile formFile)
        {
            if (formFile == null || formFile.Length == 0)
                return BadRequest("No file uploaded.");

            var filePath = Path.Combine(_fileSystem.GetCurrentDirectory(), "wwwroot/images", formFile.FileName);
            var directory = Path.GetDirectoryName(filePath);

            if (!_fileSystem.DirectoryExists(directory))
            {
                _fileSystem.CreateDirectory(directory);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await formFile.CopyToAsync(stream);
            }

            var fileUrl = $"/images/{formFile.FileName}";
            return Ok(new { link = fileUrl });
        }
    }

}
