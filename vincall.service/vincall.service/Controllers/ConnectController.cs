using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vincall.Application;
using Vincall.Application.Dtos;
using Vincall.Infrastructure;

namespace Vincall.Service.Controllers
{
    [ApiController]
    public class ConnectController: ControllerBase
    {
        private readonly ICrudServices _services;
        private readonly IMapper _mapper;
        public ConnectController(ICrudServices services, IMapper mapper)
        {
            _services = services;
            _mapper = mapper;
        }

        

        [Authorize]
        [HttpGet("UserMapping/{siteId}")]
        public async Task<IEnumerable<UserMappingDto>> GetUserMapping(string siteId)
        {
            var mappings = await _services.ReadMany<UserComm100>(x => x.SiteId == siteId).ToListAsync();
            var users = await _services.ReadManyNoTracked<User>().ToListAsync();
            return mappings.Select(x => new UserMappingDto
            {
                Comm100AgentId = x.ExternId,
                Comm100Email = x.Email,
                UserAccount = x.Account,
                UserName = users.FirstOrDefault(o=>o.Account.Equals(x.Account,StringComparison.InvariantCultureIgnoreCase))?.UserName
            });
        }

        [Authorize]
        [HttpPut("UserMapping/{siteId}")]
        public async Task<IEnumerable<UserMappingDto>> UpdateUserMapping(string siteId, [FromBody]List<UserMappingDto> mappings)
        {
            var sources = _mapper.Map<List<UserComm100>>(mappings?.ToList());
            sources = sources?.Select(x =>
            {
                x.SiteId = siteId;
                return x;
            }).ToList();
            var userComm100es = await _services.DeleteAllThenInsertAndSaveAsync(sources);

            var result = _mapper.Map<List<UserMappingDto>>(userComm100es?.ToList());
            var users = await _services.ReadManyNoTracked<User>().ToListAsync();
            result = result?.Select(x =>
            {                
                x.UserName = users.FirstOrDefault(o => o.Account.Equals(x.UserAccount, StringComparison.InvariantCultureIgnoreCase))?.UserName;
                return x;
            }).ToList();

            return await Task.FromResult<IEnumerable<UserMappingDto>>(result);
        }
    }
}
