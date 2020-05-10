using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;

namespace Phtotex.Api.Middlewares
{
    public class JwtAuthorizationMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var principal = new ClaimsPrincipal();

            var result = await context.AuthenticateAsync(
                JwtBearerDefaults.AuthenticationScheme);

            if (result?.Principal != null)
            {
                principal.AddIdentities(result.Principal.Identities);
            }

            context.User = principal;

            await next(context);
        }
    }
}
