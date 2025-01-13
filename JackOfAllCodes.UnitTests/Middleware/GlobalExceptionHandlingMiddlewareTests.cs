using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using JackOfAllCodes.Web.Middleware;

namespace JackOfAllCodes.UnitTests.Middleware
{
    public class GlobalExceptionHandlingMiddlewareTests
    {
        private readonly Mock<ILogger<GlobalExceptionHandlingMiddleware>> _mockLogger;
        private readonly Mock<RequestDelegate> _mockNext;

        public GlobalExceptionHandlingMiddlewareTests()
        {
            // Mocking ILogger
            _mockLogger = new Mock<ILogger<GlobalExceptionHandlingMiddleware>>();

            // Mocking the next middleware in the pipeline
            _mockNext = new Mock<RequestDelegate>();
        }

        [Fact]
        public async Task InvokeAsync_Should_Return_InternalServerError_When_ExceptionIsThrown()
        {
            // Arrange
            var exceptionMiddleware = new GlobalExceptionHandlingMiddleware(_mockNext.Object, _mockLogger.Object);

            // Mocking HttpContext to simulate an exception thrown by the next middleware
            var httpContext = new DefaultHttpContext();
            _mockNext.Setup(next => next(It.IsAny<HttpContext>())).ThrowsAsync(new InvalidOperationException("Test exception"));

            // Act
            await exceptionMiddleware.InvokeAsync(httpContext);

            // Assert
            // Verifying that the response status code is set to 500 (InternalServerError)
            Assert.Equal(StatusCodes.Status500InternalServerError, httpContext.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_Should_CallNextMiddleware_When_NoExceptionIsThrown()
        {
            // Arrange
            var exceptionMiddleware = new GlobalExceptionHandlingMiddleware(_mockNext.Object, _mockLogger.Object);

            // Mocking HttpContext to simulate no exception thrown
            var httpContext = new DefaultHttpContext();
            _mockNext.Setup(next => next(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

            // Act
            await exceptionMiddleware.InvokeAsync(httpContext);

            // Assert
            // Verifying that the next middleware was called
            _mockNext.Verify(next => next(It.IsAny<HttpContext>()), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_Should_SetResponseContentTypeToJson_When_ExceptionIsThrown()
        {
            // Arrange
            var exceptionMiddleware = new GlobalExceptionHandlingMiddleware(_mockNext.Object, _mockLogger.Object);

            // Mocking HttpContext to simulate an exception thrown by the next middleware
            var httpContext = new DefaultHttpContext();
            _mockNext.Setup(next => next(It.IsAny<HttpContext>())).ThrowsAsync(new InvalidOperationException("Test exception"));

            // Act
            await exceptionMiddleware.InvokeAsync(httpContext);

            // Assert
            // Verifying that the content type is set to "application/json"
            Assert.Equal("application/json; charset=utf-8", httpContext.Response.ContentType);
        }
    }
}
