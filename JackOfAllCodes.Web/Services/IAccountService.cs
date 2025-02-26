using JackOfAllCodes.Web.Models;
using JackOfAllCodes.Web.Models.Domain;
using JackOfAllCodes.Web.Models.ViewModels;
using Microsoft.AspNetCore.Identity.Data;
using RegisterRequest = JackOfAllCodes.Web.Models.ViewModels.RegisterRequest;

namespace JackOfAllCodes.Web.Services
{
    public interface IAccountService
    {
        Task<ServiceResponse> RegisterAsync(RegisterRequest request);

        Task<ServiceResponse> LoginAsync(LoginRequest request);

        Task<ApplicationUser?> GetCurrentUser();

        Task<ApplicationUser?> UpdateUser(ApplicationUser? user);

        Task<ServiceResponse> ResetUserPassword(string email, string callbackUrl);

        Task<ServiceResponse> SetNewUserPassword(SetNewPasswordViewModel model);

        Task LogoutAsync();
    }
}
