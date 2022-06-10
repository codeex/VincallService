using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vincall.Infrastructure;

namespace Vincall.Service.Cache
{
    public interface ITwilioSettingCacheService
    {
        List<TwilioSetting> GetTwilioSettingCache();
    }
}
