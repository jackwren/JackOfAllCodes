using JackOfAllCodes.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace JackOfAllCodes.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IFileSystemService _fileSystem;
        private readonly IFileUploadService _fileUploadService;

        public ImagesController(IFileSystemService fileSystem, IFileUploadService fileUploadService)
        {
            _fileSystem = fileSystem;
            _fileUploadService = fileUploadService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile formFile, [FromForm] string folderPath)
        {
            try
            {
                var fileUrl = await _fileUploadService.UploadFileAsync(formFile, folderPath);
                return Ok(new { link = fileUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

}
