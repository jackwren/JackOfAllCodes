using JackOfAllCodes.Web.Models;
using JackOfAllCodes.Web.Models.ViewModels;
using JackOfAllCodes.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace JackOfAllCodes.UnitTests.Controllers
{
    public class AccountControllerTests
    {
        private readonly Mock<IAccountService> _mockAccountService;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            _mockAccountService = new Mock<IAccountService>();
            _controller = new AccountController(_mockAccountService.Object);
        }

        [Fact]
        public void Register_Get_ReturnsViewResult()
        {
            // Act
            var result = _controller.Register();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async Task Register_Post_ReturnsViewResult_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Error", "ModelState is invalid");
            var request = new RegisterRequest();

            // Act
            var result = await _controller.Register(request);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(request, viewResult.Model);
        }

        [Fact]
        public async Task Register_Post_ReturnsRedirectToActionResult_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var request = new RegisterRequest();
            _mockAccountService.Setup(service => service.RegisterAsync(request))
                .ReturnsAsync(new ServiceResponse { Success = true });

            // Act
            var result = await _controller.Register(request);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("RegistrationSuccess", redirectResult.ActionName);
        }

        [Fact]
        public async Task Register_Post_ReturnsViewResult_WhenRegistrationFails()
        {
            // Arrange
            var request = new RegisterRequest();
            _mockAccountService.Setup(service => service.RegisterAsync(request))
                .ReturnsAsync(new ServiceResponse { Success = false, Message = "Error" });

            // Act
            var result = await _controller.Register(request);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(request, viewResult.Model);
            Assert.Equal("Error", viewResult.ViewData["ErrorMessage"]);
        }

        [Fact]
        public void Login_Get_ReturnsViewResult()
        {
            // Act
            var result = _controller.Login();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async Task Login_Post_ReturnsViewResult_WhenLoginFails()
        {
            // Arrange
            var request = new Microsoft.AspNetCore.Identity.Data.LoginRequest
            {
                Email = "test@example.com",
                Password = "password"
            };
            _mockAccountService.Setup(service => service.LoginAsync(request))
                .ReturnsAsync(new ServiceResponse { Success = false, Message = "Error" });

            // Act
            var result = await _controller.Login(request);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(request, viewResult.Model);
            Assert.Equal("Error", viewResult.ViewData["ErrorMessage"]);
        }
    }
}