using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;

using Apiraiser.Models;
using Apiraiser.Mappings;
using Apiraiser.Enums;
using Apiraiser.Helpers;
using Apiraiser.Services;

namespace Apiraiser.Interfaces
{
    public interface IMemoryCacheService
    {
        Task<APIResult> Get(string key);

        void Set(string key, APIResult entry, MemoryCacheEntryOptions options = null);

        void Remove(string key);
    }
}