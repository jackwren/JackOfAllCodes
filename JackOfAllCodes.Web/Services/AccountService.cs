using JackOfAllCodes.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using System.Security.Claims;
using RegisterRequest = JackOfAllCodes.Web.Models.ViewModels.RegisterRequest;

namespace JackOfAllCodes.Web.Services
{
    public class AccountService : IAccountService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountService(
            IHttpContextAccessor httpContextAccessor,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        // Register User
        public async Task<ServiceResponse> RegisterAsync(RegisterRequest request)
        {
            // Check if user exists
            var userExists = await _userManager.FindByEmailAsync(request.Email);
            if (userExists != null)
            {
                return new ServiceResponse { Success = false, Message = "User already exists" };
            }

            // Create new user
            var user = new IdentityUser { UserName = request.Username, Email = request.Email };

            // Create the user with a hashed password
            var result = await _userManager.CreateAsync(user, request.Password);

            // Apply user to Role.
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

        // Login User (using cookie authentication)
        public async Task<ServiceResponse> LoginAsync(LoginRequest request)
        {
            // Fetch user by email
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return new ServiceResponse { Success = false, Message = "Invalid credentials" };
            }

            // Sign in user and create authentication cookie
            var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, false);
            if (!result.Succeeded)
            {
                return new ServiceResponse { Success = false, Message = "Login failed" };
            }

            return new ServiceResponse { Success = true, Message = "Login successful", UserId = user.Id.ToString() };
        }

        // Get Current User (using cookie authentication context)
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

            // Fetch the user using UserManager
            var user = await _userManager.FindByIdAsync(userId);
            return user;
        }

        // Logout User
        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
