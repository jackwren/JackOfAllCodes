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
        private readonly Mock<IFileUploadService> _mockFileUploadService;

        public ImagesControllerTests()
        {
            _mockFileSystem = new Mock<IFileSystemService>();
            _mockFileUploadService = new Mock<IFileUploadService>();
            _controller = new ImagesController(_mockFileSystem.Object, _mockFileUploadService.Object);
        }

        [Fact]
        public async Task UploadImage_ReturnsOk_WithFileUrl_WhenFileIsValid()
        {
            // Arrange
            var fileName = "testimage.jpg";
            var mockFile = new Mock<IFormFile>();
            var fileContent = new byte[] { 1, 2, 3, 4, 5 }; // Dummy content
            var stream = new MemoryStream(fileContent);

            mockFile.Setup(f => f.Length).Returns(fileContent.Length);
            mockFile.Setup(f => f.FileName).Returns(fileName);
            mockFile.Setup(f => f.OpenReadStream()).Returns(stream);
            mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default)).Returns(Task.CompletedTask);

            var folderPath = "images/blogs"; // Example folder path
            var expectedFileUrl = $"https://example.com/{folderPath}/{fileName}";

            // Mock _fileUploadService
            _mockFileUploadService
                .Setup(service => service.UploadFileAsync(mockFile.Object, folderPath))
                .ReturnsAsync(expectedFileUrl);

            // Act
            var result = await _controller.UploadImage(mockFile.Object, folderPath);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult);

            _mockFileUploadService.Verify(service => service.UploadFileAsync(mockFile.Object, folderPath), Times.Once);
        }

        [Fact]
        public async Task UploadImage_ReturnsBadRequest_WhenNoFileUploaded()
        {
            // Arrange
            IFormFile nullFile = null;
            var folderPath = "images/blogs";

            _mockFileUploadService
                .Setup(service => service.UploadFileAsync(It.IsAny<IFormFile>(), It.IsAny<string>()))
                .ThrowsAsync(new ArgumentException("No file uploaded.")); // Simulate exception for invalid input

            // Act
            var result = await _controller.UploadImage(nullFile, folderPath);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("No file uploaded.", badRequestResult.Value);

            _mockFileUploadService.Verify(service => service.UploadFileAsync(It.IsAny<IFormFile>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task UploadImage_ReturnsBadRequest_WhenServiceThrowsException()
        {
            // Arrange
            var fileName = "testimage.jpg";
            var mockFile = new Mock<IFormFile>();
            var fileContent = new byte[] { 1, 2, 3, 4, 5 };
            var stream = new MemoryStream(fileContent);

            mockFile.Setup(f => f.Length).Returns(fileContent.Length);
            mockFile.Setup(f => f.FileName).Returns(fileName);
            mockFile.Setup(f => f.OpenReadStream()).Returns(stream);
            mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default)).Returns(Task.CompletedTask);

            var folderPath = "images/blogs";

            // Mock _fileUploadService to throw an exception
            _mockFileUploadService
                .Setup(service => service.UploadFileAsync(mockFile.Object, folderPath))
                .ThrowsAsync(new Exception("An error occurred while uploading the file."));

            // Act
            var result = await _controller.UploadImage(mockFile.Object, folderPath);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("An error occurred while uploading the file.", badRequestResult.Value);

            // Verify service interaction
            _mockFileUploadService.Verify(service => service.UploadFileAsync(mockFile.Object, folderPath), Times.Once);
        }
    }
}
