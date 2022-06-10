using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Vincall.Application;
using Vincall.Infrastructure;
using Vincall.Service.Cache;
using Vincall.Service.Services;

namespace Vincall.Service.Applications
{
    public class MigrateCallLogsService : IMigrateCallLogsService
    {
        private readonly Timer _timer;

        private readonly IServiceScopeFactory _serviceScopeFactory;
        private ICrudServices _services;
        private ITwilioSettingCacheService _twilioSettingCacheService;
        private readonly ILogger<MigrateCallLogsService> _logger;
        public MigrateCallLogsService(IServiceScopeFactory serviceScopeFactory, ILogger<MigrateCallLogsService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _timer = new Timer(OnTimeEvent, null, 20000, 60 * 1000);
        }
        private void OnTimeEvent(object sender)
        {
            MigrateCallLogs();
        }
        public void MigrateCallLogs()
        {
            using (var serviceScope = _serviceScopeFactory.CreateScope())
            {
                try
                {
                    _twilioSettingCacheService = serviceScope.ServiceProvider.GetService<ITwilioSettingCacheService>();
                    _services = serviceScope.ServiceProvider.GetService<ICrudServices>();
                    var context = serviceScope.ServiceProvider.GetRequiredService<VincallDBContext>();
                    var twilioSetting = _twilioSettingCacheService.GetTwilioSettingCache().FirstOrDefault();
                    var deviceNum = twilioSetting.CallId;
                    var latestTime = _services.ReadManyNoTracked<CallList>().OrderByDescending(x => x.CreateDate).FirstOrDefault().CreateDate;
                    TwilioClient.Init(twilioSetting.AccountSid, twilioSetting.AuthToken);
                    var calls = CallResource.Read(startTimeAfter: latestTime)?.Where(x=>x.StartTime>latestTime).ToList() ?? new List<CallResource>();
                    if (calls.Count == 0)
                    {
                        _logger.LogInformation("*************calls no data");
                        return;
                    }
                    foreach (var call in calls)
                    {

                        var callLog = ChangeFromAndTo(call, deviceNum);
                        context.CallLists.Add(callLog);
                        _logger.LogInformation(string.Format("callLog---->from {0} to {1} create on {2} duration {3} callname {4}", callLog.From, callLog.To, callLog.CreateDate, callLog.CallTime,call.CallerName));
                    }
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "call log error");
                }
            }


        }

        private CallList ChangeFromAndTo(CallResource call,string deviceNum)
        {
            
            var from = call.From;
            var ext = string.Empty;
            var to = call.To;
            if ((!string.IsNullOrEmpty(from))&&from.StartsWith("client:"))
            {
                ext = from.Substring(from.IndexOf(":") + 1);
                from = "EXT:" + ext;
            }
            if ((!string.IsNullOrEmpty(to)) && to.StartsWith("client:"))
            {
                to = "EXT:" + to.Substring(to.IndexOf(":") + 1);
            }
            if (string.IsNullOrEmpty(to))
            {
                to = deviceNum;
            }
            var callLog = new CallList()
            {
                From = from,
                To = to,
                ExtensionNumber = ext,
                CreateDate = call.StartTime.Value,
                CallTime = Convert.ToInt32(call.Duration)
            };
            return callLog;
        }
    }
}
