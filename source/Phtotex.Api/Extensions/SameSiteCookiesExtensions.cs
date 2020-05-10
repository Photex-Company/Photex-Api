using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Phtotex.Api.Extensions
{
    public static class SameSiteCookiesExtensions
    {
        public const SameSiteMode Unspecified = (SameSiteMode)(-1);

        public static void CheckSameSite(HttpContext httpContext, CookieOptions options)
        {
            options.SameSite = httpContext.CheckSameSiteMode();
        }

        /// <summary>
        /// Based on https://docs.microsoft.com/en-us/aspnet/core/security/samesite?view=aspnetcore-2.2#sob
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static SameSiteMode CheckSameSiteMode(this HttpContext httpContext)
        {
#if DEBUG
            return SameSiteMode.Lax;
#endif
            return CheckForReleaseMode(httpContext);
        }

        internal static SameSiteMode CheckForReleaseMode(HttpContext httpContext)
        {
            var userAgent = httpContext.Request?.Headers?["User-Agent"].ToString();

            if (string.IsNullOrEmpty(userAgent))
            {
                return SameSiteMode.None;
            }

            return (userAgent.Contains("CPU iPhone OS 12") ||
                   userAgent.Contains("iPad; CPU OS 12") ||
                   userAgent.Contains("Chrome/5") ||
                   userAgent.Contains("Chrome/6") ||
                   (userAgent.Contains("Macintosh; Intel Mac OS X 10_14") &&
                    userAgent.Contains("Version/") &&
                    userAgent.Contains("Safari"))) ? Unspecified : SameSiteMode.None;
        }
    }
}
