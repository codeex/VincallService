using IdentityModel;
using IdentityServer4;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Vincall.Application;
using Vincall.Application.Dtos;
using Vincall.Infrastructure;
using Vincall.Service.WebApiServices;

namespace Vincall.Service.Controllers
{
    public class VincallTokenController: ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly ICrudServices _services;

        public VincallTokenController( IConfiguration configuration, ICrudServices services)
        {
            _configuration = configuration;
            _services = services;
        }

        [HttpPost("VincallToken")]
        public async Task<VincallToken> GetVincallTokenAsync([FromBody]VincallPassword vincallPassword)
        {
            if (string.IsNullOrEmpty(vincallPassword?.username))
            {
                throw new ArgumentNullException("vincallPassword");
            }

            var user = await _services.ReadSingleAsync<User>(x => x.Account == vincallPassword.username);
            var valid = ValidateUser(user, vincallPassword.password);
            if (!valid)
            {
                throw new UnauthorizedAccessException("vincallPassword");
            }

            return  await WriteCookieAsync(HttpContext, user);
        }

        private bool ValidateUser(User user, string password)
        {
            if (user?.Password == null) return false;
            return user.Password.Equals(Md5Helper.Md5(password), StringComparison.CurrentCulture);
        }

        public static async Task<VincallToken> WriteCookieAsync(HttpContext context, User user)
        {
            
            var userId = user.Id.ToString();
            var role = user.IsAdmin ? "admin" : "user";
            var userName = user.UserName;
            var account = user.Account;
            var isu = new IdentityServerUser(account)
            {
                IdentityProvider = IdentityServerConstants.LocalIdentityProvider,
                AuthenticationTime = DateTime.UtcNow
            };
            isu.AuthenticationMethods.Add(OidcConstants.AuthenticationMethods.Password);
            isu.AdditionalClaims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
            isu.AdditionalClaims.Add(new Claim(ClaimTypes.Role, role));
            isu.AdditionalClaims.Add(new Claim(ClaimTypes.Name, userName));
            isu.AdditionalClaims.Add(new Claim("UserAccount", account));
            await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, isu.CreatePrincipal());

            var tokenResult = new VincallToken()
            {
                access_token = "test",
                refresh_token = "test",

            };
            tokenResult.userId = userId;
            tokenResult.role = role;
            tokenResult.userName = userName;
            tokenResult.userAccount = account;
            return tokenResult;
        }


        [HttpPut("VincallToken")]
        public async Task<VincallToken> RefreshVincallTokenAsync([FromBody]VincallRefreshToken vincallRefreshToken)
        {
            throw new NotImplementedException();
        }

        [HttpGet("logout")]
        public Task LogourAsync()
        {
            return HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
