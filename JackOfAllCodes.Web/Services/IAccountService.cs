﻿using JackOfAllCodes.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;

namespace JackOfAllCodes.Web.Services
{
    public interface IAccountService
    {
        Task<ServiceResponse> RegisterAsync(RegisterRequest request);

        Task<ServiceResponse> LoginAsync(LoginRequest request);

        Task<IdentityUser?> GetCurrentUser();

        Task LogoutAsync();
    }
}
