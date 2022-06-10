using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Vincall.Application;
using Vincall.Application.Dtos;
using Vincall.Infrastructure;
using Vincall.Service.Models;
using Vincall.Service.WebApiServices;
using WebApiClientCore.Exceptions;

namespace Vincall.Service.Controllers
{
    
    [ApiController]
    [Route("[controller]")]
    public class Comm100AgentController : ControllerBase
    {
        private readonly ICrudServices _services;
        private readonly IMapper _mapper;
        private readonly GlobalSettingService _settingService;
        private readonly IComm100ApiClient _comm100Client;
        private readonly HostProvider _hostProvider;
        private readonly IComm100OauthClient _comm100OauthClient;

        public Comm100AgentController(ICrudServices services, IMapper mapper, GlobalSettingService settingService, IComm100ApiClient comm100Client, HostProvider hostProvider, IComm100OauthClient comm100OauthClient)
        {
            _services = services;
            _mapper = mapper;
            _settingService = settingService;
            _comm100Client = comm100Client;
            _hostProvider = hostProvider;
            _comm100OauthClient = comm100OauthClient;
        }
        [Authorize]
        [HttpGet("Agents")]
        public async Task<IActionResult> QueryAgentAsync([FromQuery]int siteId)
        {
            var domain = await _settingService.GetApiDomain();
            _hostProvider.Host = domain;
            var oldItems = await _services.ReadMany<GlobalSetting>(x => x.Type == $"{siteId}-connect").ToListAsync();
            if(oldItems.Count == 0)
            {
                return Ok(new List<Comm100Agent>());
            }

            var token = oldItems.FirstOrDefault(x => x.Key == "accessToken")?.Value;
            var refreshToken = oldItems.FirstOrDefault(x => x.Key == "refreshToken")?.Value;
            if(string.IsNullOrEmpty(token))
            {
                return Unauthorized();
            }
            try
            {
                var list = await _comm100Client.GetAgentsAsync("Bearer " + token, siteId);
                return Ok(list);
            }
            catch(HttpRequestException ex) 
            {
                var innerEx = (ex.InnerException as ApiResponseStatusException);
                if(innerEx?.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    
                    //refresh token
                    var result = await RefreshTokenAsync(siteId, refreshToken);
                    if(result == null)
                    {
                        return Unauthorized();
                    }
                    _hostProvider.Host = domain;
                    await _settingService.SaveConnectTokenAsync(siteId, result.access_token, result.refresh_token);
                    var list = await _comm100Client.GetAgentsAsync("Bearer " + result.access_token, siteId);
                    return Ok(list);
                }

                throw;
            }
            
        }

        private async Task<TokenResult> RefreshTokenAsync(int siteId,string refreshToken)
        {
            var oauthInfo = await _settingService.GetAsync("vincallconnect");
            if (string.IsNullOrEmpty(oauthInfo?.client_id))
            {
                return null;
            }
            _hostProvider.Host = oauthInfo.domain;
            var tokenResult = await _comm100OauthClient.RefreshTokenAsync(siteId, oauthInfo.client_id, oauthInfo.client_secret, "refresh_token", refreshToken);
            return tokenResult;
        }

        


    }
}
