﻿using JackOfAllCodes.Web.Models;
using JackOfAllCodes.Web.Models.Domain;
using JackOfAllCodes.Web.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using System.Security.Claims;
using RegisterRequest = JackOfAllCodes.Web.Models.ViewModels.RegisterRequest;

namespace JackOfAllCodes.Web.Services
{
    public class AccountService : IAccountService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EmailSenderService _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountService(
            IHttpContextAccessor httpContextAccessor,
            EmailSenderService emailSender,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _emailSender = emailSender;
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

        public async Task<ApplicationUser?> GetCurrentUser()
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

        public async Task<ApplicationUser?> UpdateUser(ApplicationUser? user)
        {
            if (user == null) return null;

            await _userManager.UpdateAsync(user);

            return user;
        } 

        public async Task<ServiceResponse> ResetUserPassword(string email, string callbackUrl)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new ServiceResponse { Success = false, Message = "User not found" };
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var codedToken = Uri.EscapeDataString(token);

            var resetUrlWithToken = $"{callbackUrl}&token={codedToken}";

            await _emailSender.SendPasswordResetEmail(email, resetUrlWithToken);

            return new ServiceResponse { Success = true, Message = "Reset successful" };
        }

        public async Task<ServiceResponse> SetNewUserPassword(SetNewPasswordViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return new ServiceResponse { Success = false, Message = "User not found." };
            }

            var decodedToken = Uri.UnescapeDataString(model.Token);

            var resetResult = await _userManager.ResetPasswordAsync(user, decodedToken, model.Password);

            if (!resetResult.Succeeded)
            {
                var errors = string.Join("; ", resetResult.Errors.Select(e => e.Description));
                return new ServiceResponse { Success = false, Message = $"Password reset failed: {errors}" };
            }

            return new ServiceResponse { Success = true, Message = "Password reset successful." };
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}