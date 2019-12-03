using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kard.Extensions
{
    public static class MemoryCacheExtensions
    {
      
        public static T GetCache<T>(this IMemoryCache memoryCache,string cacheKey, Func<T> cacheFunc,long? absoluteSeconds=null, long? slidingSeconds =null)
        {
            Check.NotNull(memoryCache, nameof(memoryCache));

            var cacheValue = memoryCache.GetOrCreate(cacheKey, (cacheEntry) => {
                if (absoluteSeconds.HasValue)
                {
                    cacheEntry.SetAbsoluteExpiration(TimeSpan.FromSeconds(absoluteSeconds.Value));
                }
                if (slidingSeconds.HasValue)
                {
                    cacheEntry.SetSlidingExpiration(TimeSpan.FromSeconds(slidingSeconds.Value));
                }
                return cacheFunc.Invoke();
            });
            return cacheValue;
        }
    }
}
