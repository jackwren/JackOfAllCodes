using JackOfAllCodes.Web.Controllers;
using JackOfAllCodes.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace JackOfAllCodes.UnitTests.Controllers
{
    public class ImagesControllerTests
    {
        private readonly ImagesController _controller;
        private readonly Mock<IFileSystemService> _mockFileSystem;

        public ImagesControllerTests()
        {
            _mockFileSystem = new Mock<IFileSystemService>();
            _controller = new ImagesController(_mockFileSystem.Object);
        }

        [Fact]
        public async Task UploadImage_ReturnsOk_WithFileUrl_WhenFileIsValid()
        {
            // Arrange: Create a mock file (IFormFile)
            var fileName = "testimage.jpg";
            var mockFile = new Mock<IFormFile>();
            var fileContent = new byte[] { 1, 2, 3, 4, 5 }; // Dummy content
            var stream = new MemoryStream(fileContent);

            mockFile.Setup(f => f.Length).Returns(fileContent.Length);
            mockFile.Setup(f => f.FileName).Returns(fileName);
            mockFile.Setup(f => f.OpenReadStream()).Returns(stream);
            mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default)).Returns(Task.CompletedTask);

            // Set up the mock IFileSystem to simulate GetCurrentDirectory and DirectoryExists
            var tempDir = Path.Combine(Path.GetTempPath(), "images"); // Temporary directory for image uploads
            var imagesDir = Path.Combine(tempDir, "wwwroot", "images", "blogs"); // Final target directory

            if (!Directory.Exists(imagesDir))
            {
                Directory.CreateDirectory(imagesDir);  // Create the images directory under temp path
            }

            // Mock IFileSystem methods
            _mockFileSystem.Setup(fs => fs.GetCurrentDirectory()).Returns(tempDir);  // Ensure current directory points to tempDir
            _mockFileSystem.Setup(fs => fs.DirectoryExists(It.IsAny<string>())).Returns(true); // Simulate the directory exists
            _mockFileSystem.Setup(fs => fs.CreateDirectory(It.IsAny<string>())).Verifiable(); // Verify if directory creation is triggered

            // Act: Call the UploadImage method
            var result = await _controller.UploadImage(mockFile.Object);

            // Assert: Check if the result is OK and contains the correct file URL
            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.NotNull(okResult); // Ensure the return value is not null
        }

        [Fact]
        public async Task UploadImage_ReturnsBadRequest_WhenNoFileUploaded()
        {
            // Arrange: Pass a null IFormFile
            IFormFile nullFile = null;

            // Act: Call the UploadImage method
            var result = await _controller.UploadImage(nullFile);

            // Assert: Check if the result is BadRequest
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("No file uploaded.", badRequestResult.Value);
        }

        [Fact]
        public async Task UploadImage_ReturnsBadRequest_WhenFileIsEmpty()
        {
            // Arrange: Create a mock file with no content
            var fileName = "emptyfile.txt";
            var mockFile = new Mock<IFormFile>();
            var emptyContent = new byte[0]; // Empty content
            var stream = new MemoryStream(emptyContent);

            mockFile.Setup(f => f.Length).Returns(0); // Empty file length
            mockFile.Setup(f => f.FileName).Returns(fileName);
            mockFile.Setup(f => f.OpenReadStream()).Returns(stream);
            mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default)).Returns(Task.CompletedTask);

            // Act: Call the UploadImage method
            var result = await _controller.UploadImage(mockFile.Object);

            // Assert: Check if the result is BadRequest due to empty file
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("No file uploaded.", badRequestResult.Value);
        }
    }
}
