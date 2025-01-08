using JackOfAllCodes.Web.Models;
using JackOfAllCodes.Web.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using System.Security.Claims;
using RegisterRequest = JackOfAllCodes.Web.Models.ViewModels.RegisterRequest;

namespace JackOfAllCodes.Web.Services
{
    public class AccountService : IAccountService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountService(
            IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public async Task<ServiceResponse> RegisterAsync(RegisterRequest request)
        {
            var userExists = await _userManager.FindByEmailAsync(request.Email);
            if (userExists != null)
            {
                return new ServiceResponse { Success = false, Message = "User already exists" };
            }

            var user = new ApplicationUser { UserName = request.Username, Email = request.Email };

            // Create the user with a hashed password
            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                var roleExists = await _roleManager.RoleExistsAsync("User");
                if (!roleExists)
                {
                    await _roleManager.CreateAsync(new IdentityRole("User"));
                }
                await _userManager.AddToRoleAsync(user, "User");
            }

            if (!result.Succeeded)
            {
                return new ServiceResponse { Success = false, Message = "Registration failed: " + string.Join(", ", result.Errors.Select(e => e.Description)) };
            }

            return new ServiceResponse { Success = true, Message = "Registration successful" };
        }

        public async Task<ServiceResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return new ServiceResponse { Success = false, Message = "Invalid credentials" };
            }

            var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, false);

            if (!result.Succeeded)
            {
                return new ServiceResponse { Success = false, Message = "Login failed" };
            }

            return new ServiceResponse { Success = true, Message = "Login successful", UserId = user.Id.ToString() };
        }

        public async Task<IdentityUser?> GetCurrentUser()
        {
            var currentClaim = _httpContextAccessor.HttpContext?.User;

            if (currentClaim == null || !currentClaim.Identity.IsAuthenticated)
            {
                return null;
            }

            var userId = currentClaim.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return null;
            }

            var user = await _userManager.FindByIdAsync(userId);
            return user;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
