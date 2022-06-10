using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vincall.Infrastructure;

namespace Vincall.Service.Models
{
    public class GlobalSettings
    {
        public static List<GlobalSetting> All => new List<GlobalSetting>
        {
            //comm100 oauth
            new GlobalSetting
            {
                Key="client_id",
                Value="F39DEFBC-FE17-4091-9541-1F39B79ACEDB",
                Type="comm100"
            },
            new GlobalSetting
            {
                Key="client_secret",
                Value="E6E18C39-1A0D-4A14-82A3-7672D87859DF",
                Type="comm100"
            },
            new GlobalSetting
            {
                Key="grant_type",
                Value="authorization_code",
                Type="comm100"
            },
            new GlobalSetting
            {
                Key="redirect_uri",
                Value="https://api.vincall.net/api/sso/callback",
                Type="comm100"
            },
            new GlobalSetting
            {
               Key="redirect_logon",
                Value="https://www.vincall.net/oauth/logon",
                Type="comm100"
            },

            //vincall oauth
            new GlobalSetting
            {
                Key="client_id",
                Value="vincall",
                Type="vincall"
            },
            new GlobalSetting
            {
                Key="client_secret",
                Value="vincall",
                Type="vincall"
            },
            new GlobalSetting
            {
                Key="grant_type",
                Value="authorization_code",
                Type="vincall"
            },
            new GlobalSetting
            {
                Key="redirect_uri",
                Value="https://api.vincall.net/open/login/callback",
                Type="vincall"
            },
            new GlobalSetting
            {
               Key="redirect_logon",
                Value="https://voipdash.comm100dev.io/ui/10000/app/siteApps/vincallCallback.html",
                Type="vincall"
            },
            //connect comm100 oauth
            new GlobalSetting
            {
                Key="client_id",
                Value="F39DEFBC-FE17-4091-9541-1F39B79ACEDE",
                Type="connect"
            },
            new GlobalSetting
            {
                Key="client_secret",
                Value="E6E18C39-1A0D-4A14-82A3-7672D87859DE",
                Type="connect"
            },
            new GlobalSetting
            {
                Key="grant_type",
                Value="authorization_code",
                Type="connect"
            },
            new GlobalSetting
            {
                Key="redirect_uri",
                Value="https://api.vincall.net/open/login/comm100callback",
                Type="connect"
            },

            //vincall connect to comm100

            new GlobalSetting
            {
                Key="client_id",
                Value="F39DEFBC-FE17-4091-9541-1F39B79ACEDB",
                Type="vincallconnect"
            },
            new GlobalSetting
            {
                Key="client_secret",
                Value="E6E18C39-1A0D-4A14-82A3-7672D87859DF",
                Type="vincallconnect"
            },
            new GlobalSetting
            {
                Key="grant_type",
                Value="authorization_code",
                Type="vincallconnect"
            },
            new GlobalSetting
            {
                Key="redirect_uri",
                Value="https://api.vincall.net/api/sso/connectcallback",
                Type="vincallconnect"
            },
            new GlobalSetting
            {
               Key="redirect_logon",
                Value="https://www.vincall.net/oauth/connect",
                Type="vincallconnect"
            },

            //installcode 
            new GlobalSetting
            {
               Key="agentConsole",
                Value="https://voipdash.comm100dev.io",
                Type="installcode"
            },
            new GlobalSetting
            {
               Key="controlPanel",
                Value="https://center.comm100.io",
                Type="installcode"
            },
            new GlobalSetting
            {
               Key="agentAppId",
                Value="7B89F67C-5C58-4C4A-9AD5-37903D4DC75A",
                Type="installcode"
            },
            new GlobalSetting
            {
               Key="controlAppId",
                Value="12345678-90ab-cdef-1234-567890abcdef",
                Type="installcode"
            },
        };
    }
}
