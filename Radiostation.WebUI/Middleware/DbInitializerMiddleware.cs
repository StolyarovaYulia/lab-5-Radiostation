using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Radiostation.DAL;

namespace Radiostation.WebUI.Middleware
{
    public class DbInitializerMiddleware
    {
        private readonly RequestDelegate _next;

        public DbInitializerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context, IServiceProvider serviceProvider, RadiostationContext dbContext)
        {
            if (!context.Session.Keys.Contains("starting"))
            {
                dbContext.Initialize();
                context.Session.SetString("starting", "Yes");
            }

            return _next.Invoke(context);
        }
    }

    public static class DbInitializerExtensions
    {
        public static IApplicationBuilder UseDbInitializer(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DbInitializerMiddleware>();
        }
    }
}