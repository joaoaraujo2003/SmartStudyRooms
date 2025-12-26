using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace SmartStudyRooms.API
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _apiKey;

        public ApiKeyMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _apiKey = config["ApiKey"];
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue("X-API-KEY", out var extractedKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API Key em falta");
                return;
            }

            if (!_apiKey.Equals(extractedKey))
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("API Key inválida");
                return;
            }

            await _next(context);
        }
    }
}
