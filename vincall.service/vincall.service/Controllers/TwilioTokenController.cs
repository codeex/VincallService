using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Twilio.Jwt.AccessToken;
using Vincall.Application;
using Vincall.Infrastructure;
using Vincall.Service.Cache;
using Vincall.Service.Models;

namespace vincall.service.Controllers
{
    public class TwilioTokenController : Controller
    {
        private readonly ICrudServices _services;
        private readonly ITwilioSettingCacheService _twilioSettingCacheService;
        private readonly ILogger<TwilioTokenController> _logger;


        public TwilioTokenController(ICrudServices services, ITwilioSettingCacheService twilioSettingCacheService, ILogger<TwilioTokenController> logger)
        {
            _services = services;
            _twilioSettingCacheService = twilioSettingCacheService;
            _logger = logger;
        }

        // GET: /token
        [Authorize]
        [HttpGet("TwilioToken")]
        public IActionResult GetTwilioToken(int agentId)
        {
            var twilioSetting = _twilioSettingCacheService.GetTwilioSettingCache().FirstOrDefault();
            var userAccount = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserAccount")?.Value;
            var user = _services.ReadSingleAsync<User>(x => x.Account == userAccount).GetAwaiter().GetResult();
            _logger.LogInformation($"user id--->{user.Id}   user.Account----->{user.Account}  user.IsAdmin------->{user.IsAdmin}");
            Agent agent = new Agent();
            string deviceNum = string.Empty;
            //Admin user can designate the agent with agentId.
            if (user.IsAdmin)
            {
                agent = _services.ReadSingleAsync<Agent>(x => x.Id == agentId).GetAwaiter().GetResult();
                deviceNum = agent.DeviceNumber;
            }
            else
            {
                agent = _services.ReadSingleAsync<Agent>(x => x.UserAccount == userAccount).GetAwaiter().GetResult();
                if (agent == null)
                {
                    _logger.LogError("No bind agent for the user!");
                    throw new Exception("No bind agent for the user!");
                }
                deviceNum = agent?.DeviceNumber??string.Empty;
            }
            var identity = deviceNum;
            //set agent state to available
            if (agent != null)
            {
                agent.State = 1;
                agent.UpdateDate = DateTime.UtcNow;
                _services.UpdateAndSaveAsync<Agent>(agent).GetAwaiter();
            }

            var grant = new VoiceGrant();
            grant.OutgoingApplicationSid = twilioSetting.AppSid;
            grant.IncomingAllow = true;

            var grants = new HashSet<IGrant>
            {
                { grant }
            };

            var token = new Token(
                twilioSetting.AccountSid,
                twilioSetting.ApiSid,
                twilioSetting.ApiSecret,
                identity,
                grants: grants).ToJwt();

            return Json(new { identity, token });
        }
    }
}
