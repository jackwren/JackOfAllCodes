using JackOfAllCodes.Web.Models;
using JackOfAllCodes.Web.Models.Domain;
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

        Task LogoutAsync();
    }
}
