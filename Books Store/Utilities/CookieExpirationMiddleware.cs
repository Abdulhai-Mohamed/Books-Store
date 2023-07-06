using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace Books_Store.Utilities
{
    public class CookieExpirationMiddleware
    {
        private readonly RequestDelegate _next;

        public CookieExpirationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext HttpContext)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                //    var CookieExpirationDate = HttpContext.Request.Cookies[".AspNetCore.Application.Identity"].Expires.ToString("O");
                //    var CookieExpirationDate = HttpContext.Request.Cookies.

                //    var authenticationProperties = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                //    var expiresUtc = authenticationProperties.ExpiresUtc;
                //    if (expiresUtc.HasValue)
                //    {
                //        var cookieExpirationDate = expiresUtc.Value.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                //        HttpContext.Items["CookieExpirationDate"] = cookieExpirationDate;
                //        HttpContext.Response.Cookies.Append("CookieExpirationDate", cookieExpirationDate);
                //    }
            }
            await _next(HttpContext);// calling next middleware
        }
    }

}
