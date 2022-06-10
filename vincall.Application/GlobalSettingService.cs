using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vincall.Application.Oauth;
using Vincall.Infrastructure;

namespace Vincall.Application
{
    public class GlobalSettingService
    {
        private readonly IMemoryCache _cache;
        private readonly ICrudServices _service;

        public GlobalSettingService(IMemoryCache cache,ICrudServices service)
        {
            _cache = cache;
            _service = service;
        }

        public  Task<Comm100Oauth> GetAsync(string type = "comm100")
        {
            return  _cache.GetOrCreateAsync(type, async cache =>
            {
                var settings = await _service.ReadMany<GlobalSetting>(x => x.Type == type).ToListAsync();
                cache.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                return new Comm100Oauth
                {
                    client_id = settings.FirstOrDefault(x => string.Equals(x.Key, "client_id", StringComparison.InvariantCultureIgnoreCase))?.Value,
                    client_secret = settings.FirstOrDefault(x => string.Equals(x.Key, "client_secret", StringComparison.InvariantCultureIgnoreCase))?.Value,
                    scope = settings.FirstOrDefault(x => string.Equals(x.Key, "scope", StringComparison.InvariantCultureIgnoreCase))?.Value,
                    grant_type = settings.FirstOrDefault(x => string.Equals(x.Key, "grant_type", StringComparison.InvariantCultureIgnoreCase))?.Value ?? "authorization_code",
                    redirect_uri = settings.FirstOrDefault(x => string.Equals(x.Key, "redirect_uri", StringComparison.InvariantCultureIgnoreCase))?.Value,
                    redirect_logon = settings.FirstOrDefault(x => string.Equals(x.Key, "redirect_logon", StringComparison.InvariantCultureIgnoreCase))?.Value,
                    domain = settings.FirstOrDefault(x => string.Equals(x.Key, "domain", StringComparison.InvariantCultureIgnoreCase))?.Value,
                };
            });
            
        }

        public Task<string> GetApiDomain()
        {
            return _cache.GetOrCreateAsync("apiDomain", async cache =>
            {
                var settings = await _service.ReadMany<GlobalSetting>(x => x.Type == "installcode").ToListAsync();
                cache.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                return settings.FirstOrDefault(x => string.Equals(x.Key, "agentConsole", StringComparison.InvariantCultureIgnoreCase))?.Value;
            });

        }

        public async Task SaveConnectTokenAsync(int siteId, string accessToken, string refreshToken)
        {
            var oldItems = await _service.ReadMany<GlobalSetting>(x => x.Type == $"{siteId}-connect").ToListAsync();
            if (oldItems.Exists(x => x.Key == "accessToken"))
            {
                var item = oldItems.First(x => x.Key == "accessToken");
                item.Value = accessToken;
            }
            else
            {
                await _service.CreateAndSaveAsync<GlobalSetting>(new GlobalSetting
                {
                    Type = $"{siteId}-connect",
                    Key = "accessToken",
                    Value = accessToken,

                });
            }
            if (oldItems.Exists(x => x.Key == "refreshToken"))
            {
                var item = oldItems.First(x => x.Key == "refreshToken");
                item.Value = refreshToken;
            }
            else
            {
                await _service.CreateAndSaveAsync<GlobalSetting>(new GlobalSetting
                {
                    Type = $"{siteId}-connect",
                    Key = "refreshToken",
                    Value = refreshToken,
                });
            }

            await _service.Context.SaveChangesAsync();
        }

    }
}
