using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace Phtotex.Api.Middlewares
{
    public class CookieAuthorizationMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var principal = new ClaimsPrincipal();

            var result = await context.AuthenticateAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            if (result?.Principal != null)
            {
                principal.AddIdentities(result.Principal.Identities);
            }

            context.User = principal;

            await next(context);
        }
    }
}
