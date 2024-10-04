using Microsoft.AspNetCore.Mvc;

namespace JackOfAllCodes.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile formFile)
        {
            if (formFile == null || formFile.Length == 0)
                return BadRequest("No file uploaded.");

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", formFile.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await formFile.CopyToAsync(stream);
            }

            var fileUrl = $"/images/{formFile.FileName}";
            return Ok(new { link = fileUrl });
        }
    }
}
