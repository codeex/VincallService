using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Vincall.Application;
using Vincall.Application.Dtos;
using Vincall.Infrastructure;
using Vincall.Service.Cache;

namespace Vincall.Service.Controllers
{
    public class ReportController : ControllerBase
    {
        private readonly ICrudServices _services;
        private readonly IMapper _mapper;
        private readonly ITwilioSettingCacheService _twilioSettingCacheService;

        public ReportController(ICrudServices services, IMapper mapper, ITwilioSettingCacheService twilioSettingCacheService)
        {
            _services = services;
            _mapper = mapper;
            _twilioSettingCacheService = twilioSettingCacheService;
        }

        [Authorize]
        [HttpGet("report")]
        public async Task<ReportDto> QueryReportAsync()
        {
            var users = _services.ReadManyNoTracked<User>().ToList();
            var agents = _services.ReadManyNoTracked<Agent>().ToList();
            var userId = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value);
            return await GetReportFromUserIdAsync(userId,users,agents);
        }

        [Authorize]
        [HttpGet("reports")]
        public async Task<ReportResult> QueryReportsAsync(int pageSize = 0, int pageNum = 0)
        {
            var result = new ReportResult();
            var users = _services.ReadManyNoTracked<User>().ToList();
            var agents = _services.ReadManyNoTracked<Agent>().ToList();
            var userIds = users.Select(x=>x.Id).ToList();
            var reports = new List<ReportDto>();
            foreach(var id in userIds)
            {
                var rnd = new Random();
                var report = await GetReportFromUserIdAsync(id,users,agents);
                if (report != null)
                {
                    report.IncomingCall = rnd.Next(50, 100) * report.IncomingCall / 100;
                    report.OutboundCall = rnd.Next(50, 100) * report.OutboundCall / 100;
                    report.CallTime = rnd.Next(50, 100) * report.CallTime??0 / 100;
                    reports.Add(report);
                }
            }
            result.Count = reports.Count;
            if (pageSize != 0 || pageNum != 0)
            {
                reports = reports.Page<ReportDto>(pageNum, pageSize);
            }
            result.Reports = reports;
            return result;
        }


        [Authorize]
        [HttpGet("calllistsForToday")]
        public async Task<CallListResult> GetCallListsForTodayAsync(int pageSize = 0, int pageNum = 0)
        {
            var report = new CallListResult();
            var callLists =  _services.ReadMany<CallList>(x => x.CreateDate.Date == DateTime.UtcNow.Date).OrderByDescending(x=>x.CreateDate).ToList();
            report.Count = callLists.Count;
            if (pageSize != 0 || pageNum != 0)
            {
                callLists = callLists.Page<CallList>(pageNum, pageSize);
            }
            report.CallLists = _mapper.Map<List<CallListDto>>(callLists);
            return report;
        }




        public async Task<ReportDto> GetReportFromUserIdAsync(int userId,List<User> users, List<Agent> agents)
        {
            var user = users.First(x=>x.Id==userId);
            var agent = agents.FirstOrDefault(x => x.UserAccount == user.Account);
            if (agent == null)
            {
                return null;
            }
            var callLists = _services.ReadMany<CallList>(x => (x.ExtensionNumber == agent.DeviceNumber ||String.IsNullOrEmpty(x.ExtensionNumber)));//Todo: ExtensionNum is null
            var localNumber = _twilioSettingCacheService.GetTwilioSettingCache().FirstOrDefault().CallId;
            var incomming = callLists.Where(x => !x.From.Contains(localNumber)).Count();
            var outbound = callLists.Where(x => x.From.Contains(localNumber)).Count();
            var report = new ReportDto()
            {
                AgentId = agent.Id,
                CallTime = callLists.Select(x => x.CallTime).Sum(),
                IncomingCall = incomming,
                OutboundCall = outbound,
                UserName = user.UserName
            };
            return report;
        }
    }
}
