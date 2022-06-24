using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching;


using Apiraiser.Databases;
using Apiraiser.Enums;
using Apiraiser.Constants;
using Apiraiser.Interfaces;
using Apiraiser.Models;

namespace Apiraiser.Services
{
    public class MemoryCacheService : BaseService, IMemoryCacheService
    {

        public MemoryCacheService(IConfiguration configuration, IMemoryCache memoryCache) : base(configuration)
        {
            this.memoryCache = memoryCache;
        }

        protected static MemoryCacheEntryOptions defaultOptions = new MemoryCacheEntryOptions
        {

            //AbsoluteExpiration = DateTimeOffset.Now.AddHours(5),
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(5)
        };

        protected readonly IMemoryCache memoryCache;

        public virtual Task<APIResult> Get(string key)
        {
            APIResult entry = null;
            memoryCache.TryGetValue(key, out entry);
            return Task.FromResult(entry);
        }

        public void Set(string key, APIResult entry, MemoryCacheEntryOptions options = null)
        {
            memoryCache.Set(key, entry, options ?? defaultOptions);
        }

        public void Remove(string key)
        {
            memoryCache.Remove(key);
        }
    }

}