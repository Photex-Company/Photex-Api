using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Photex.Core.Contracts;
using Photex.Core.Contracts.Models;
using Photex.Core.Contracts.Requests;
using Photex.Core.Interfaces;
using Photex.Database.Entities;

namespace Photex.Core.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountService(
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<ClaimsIdentity> Authenticate(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Username);

            if (user != null)
            {
                if (await _userManager.CheckPasswordAsync(user, request.Password))
                {
                    await _userManager.ResetAccessFailedCountAsync(user);
                    var claims = await _userManager.GetClaimsAsync(user);
                    claims.Add(new Claim("UserId", user.Id.ToString()));
                    return new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                }

                await _userManager.AccessFailedAsync(user);
            }

            return null;
        }

        public async Task<UserModel> GetUserData(long userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return new UserModel
            {
                Id = userId,
                Username = user.UserName
            };
        }

        public async Task<UserModel> Register(RegistrationRequest request)
        {
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                throw new ArgumentException($"Wrong parameters. Reason: {result.Errors.Select(error => $"{error.Code}: {error.Description}")}");
            }

            return new UserModel
            {
                Username = request.Email,
                Id = (await _userManager.FindByEmailAsync(request.Email)).Id
            };
        }
    }
}
