using System.Threading.Tasks;
using BooksRentalSystem.Common;
using BooksRentalSystem.Common.Services.Identity;
using Microsoft.AspNetCore.Http;

namespace BooksRentalSystem.Admin.Middlewares
{
    public class JwtCookieAuthenticationMiddleware : IMiddleware
    {
        private readonly ICurrentTokenService _currentToken;

        public JwtCookieAuthenticationMiddleware(ICurrentTokenService currentToken)
        {
            _currentToken = currentToken;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var token = context.Request.Cookies[InfrastructureConstants.AuthenticationCookieName];

            if (token != null)
            {
                _currentToken.Set(token);

                context.Request.Headers.Append(InfrastructureConstants.AuthorizationHeaderName,
                    $"{InfrastructureConstants.AuthorizationHeaderValuePrefix} {token}");
            }

            await next.Invoke(context);
        }
    }
}
