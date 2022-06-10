using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vincall.Infrastructure;

namespace Vincall.Service.Models
{
    public class TwilioSettings
    {
        public static List<TwilioSetting> All => new List<TwilioSetting>
        {
            new TwilioSetting
            {
                AccountSid="accountSid",
                ApiSecret="1234",
                ApiSid="123",
                AppSid="3123",
                AuthToken="authtoken",
                CallId="4321"
            }
        };
    }
}
