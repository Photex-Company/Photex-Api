using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Phtotex.Api.Extensions
{
    public static class HttpContextExtensions
    {
        public static long? GetUserId(this HttpContext context)
            => long.TryParse(context?.User?.Claims?.FirstOrDefault(x => x.Type == "UserId")?.Value, out var result)
                ? result
                : (long?)null;
    }
}
