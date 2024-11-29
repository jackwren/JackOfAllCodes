using JackOfAllCodes.Web.Controllers;
using JackOfAllCodes.Web.Models;
using JackOfAllCodes.Web.Models.ViewModels;
using JackOfAllCodes.Web.Repositories;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using JackOfAllCodes.Web.Models.Domain;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace JackOfAllCodes.UnitTests.Controllers
{
    public class HomeControllerTests
    {
        private readonly Mock<ILogger<HomeController>> _mockLogger;
        private readonly Mock<IBlogPostRepository> _mockBlogPostRepository;
        private readonly Mock<ITagRepository> _mockTagRepository;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _mockLogger = new Mock<ILogger<HomeController>>();
            _mockBlogPostRepository = new Mock<IBlogPostRepository>();
            _mockTagRepository = new Mock<ITagRepository>();

            _controller = new HomeController(
                _mockLogger.Object,
                _mockBlogPostRepository.Object,
                _mockTagRepository.Object
            );
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithHomeViewModel()
        {
            // Arrange
            var mockBlogPosts = new List<BlogPost> { new BlogPost { PageTitle = "Test Post", Content = "Test Content" } };
            var mockTags = new List<Tag> { new Tag { Name = "Test Tag" } };

            _mockBlogPostRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(mockBlogPosts);
            _mockTagRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(mockTags);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);  // Check if it's a ViewResult
            var model = Assert.IsAssignableFrom<HomeViewModel>(viewResult.ViewData.Model);  // Check if the model is of type HomeViewModel
            Assert.Single(model.BlogPosts);  // Check if the BlogPosts list has one item
            Assert.Single(model.Tags);  // Check if the Tags list has one item
        }

        [Fact]
        public void Error_ReturnsViewResult_WithErrorViewModel()
        {
            // Arrange
            var mockHttpContext = new Mock<HttpContext>();
            var mockTraceIdentifier = "test-trace-id";
            mockHttpContext.Setup(c => c.TraceIdentifier).Returns(mockTraceIdentifier);

            // Mock Activity.Current if you want to check that Activity.Current is properly used
            var mockActivity = new Mock<Activity>(Activity.Current);

            // Set Activity.Current for the test
            Activity.Current = mockActivity.Object;

            // Mock the controller context with the mock HttpContext
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // Act
            var result = _controller.Error() as ViewResult;

            // Assert
            Assert.NotNull(result); // Ensure the result is not null
            Assert.IsType<ViewResult>(result); // Ensure the result is a ViewResult
            var model = result.Model as ErrorViewModel; // Ensure that the model is of type ErrorViewModel
            Assert.NotNull(model); // Ensure the model is not null
            Assert.Equal(mockTraceIdentifier, model.RequestId); // Ensure RequestId is the same as the trace identifier
        }
    }
}
