using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vincall.Application;
using Vincall.Application.Dtos;
using Vincall.Infrastructure;

namespace Vincall.Service.Cache
{
    public class TwilioSettingCacheService: ITwilioSettingCacheService
    {
        private readonly ICrudServices _services;

        public IMemoryCache _memoryCache;
        public TwilioSettingCacheService(IMemoryCache memoryCache, ICrudServices services)
        {
            _memoryCache = memoryCache;
            _services = services;
        }

        public List<TwilioSetting> GetTwilioSettingCache()
        {
            var key = "TwilioSetting";
            var data = _memoryCache.GetOrCreateAsync<List<TwilioSetting>>(key, c =>
            {
                return Task.FromResult<List<TwilioSetting>>(_services.ReadManyNoTracked<TwilioSetting>().ToList());
            });
            return data.GetAwaiter().GetResult();
        }
    }
}
