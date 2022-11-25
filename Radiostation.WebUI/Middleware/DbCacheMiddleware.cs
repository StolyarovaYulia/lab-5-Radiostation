using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Radiostation.DAL.Entities;
using Radiostation.DAL.Repositories;

namespace Radiostation.WebUI.Middleware
{
    //Компонент middleware для выполнения кэширования
    public class DbCacheMiddleware
    {
        private readonly IMemoryCache _memoryCache;
        private readonly RequestDelegate _next;
        private readonly string _cacheKey;

        public DbCacheMiddleware(RequestDelegate next, IMemoryCache memoryCache, string cacheKey = "Cars 10")
        {
            _next = next;
            _memoryCache = memoryCache;
            _cacheKey = cacheKey;
        }

        public Task Invoke(HttpContext httpContext, IBaseRepository<Track> tracksService)
        {
            if (!_memoryCache.TryGetValue(_cacheKey, out IEnumerable<Track> tracks))
            {
                tracks = tracksService.GetEntities().Take(10).ToList();
                _memoryCache.Set(_cacheKey, tracks,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(2 * 14 + 240)));
            }

            return _next(httpContext);
        }
    }

    public static class DbCacheMiddlewareExtensions
    {
        public static IApplicationBuilder UseOperatinCache(this IApplicationBuilder builder, string cacheKey)
        {
            return builder.UseMiddleware<DbCacheMiddleware>(cacheKey);
        }
    }
}