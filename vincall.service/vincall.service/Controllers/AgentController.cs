using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class AgentController : ControllerBase
    {
        private readonly ICrudServices _services;
        private readonly IMapper _mapper;
        public AgentController(ICrudServices services, IMapper mapper)
        {
            _services = services;
            _mapper = mapper;
        }
        [Authorize]
        [HttpGet("Agent/{id}")]
        public async Task<AgentDto> QueryAgentAsync(int id)
        {
            var agent = await _services.ReadSingleAsync<Agent>(id);
            var agentDto = _mapper.Map<AgentDto>(agent);
            return agentDto;
        }

        [Authorize]
        [HttpGet("Agents")]
        public async Task<AgentResult> QueryAgentsAsync( string keywords, int pageSize=0, int pageNum=0)
        {
            var result = new AgentResult();
            
            var agents = _services.ReadManyNoTracked<Agent>();
            if (!string.IsNullOrEmpty(keywords))
            {
                agents = agents.Where(x => x.UserAccount.Contains(keywords));
            }
            result.Count = agents.Count();
            if (pageSize != 0||pageNum != 0)
            {
                agents = agents.Page<Agent>(pageNum, pageSize);
            }
            result.Agents = _mapper.Map<List<AgentDto>>(agents.ToList());
            return await Task.FromResult<AgentResult>(result);
        }

        [Authorize]
        [HttpPatch("Agent/{id}")]
        public async Task<AgentDto> UpdateAgentAsync(int id, [FromBody]Agent newAgent)
        {
            await _services.UpdateAndSaveAsync<Agent>(newAgent);
            var agent = await _services.ReadSingleAsync<Agent>(id);
            return _mapper.Map<AgentDto>(agent);
        }

        [Authorize]
        [HttpGet("Agent/{id}/updatetime")]
        public async Task<AgentDto> UpdateAgentUpdateDateAsync(int id)
        {
            var agent=await _services.ReadSingleAsync<Agent>(id);
            agent.UpdateDate = DateTime.UtcNow;
            await _services.UpdateAndSaveAsync<Agent>(agent);
            return _mapper.Map<AgentDto>(agent);
        }

        [Authorize]
        [HttpGet("Agent/{id}/updateStatusToOnline")]
        public async Task<AgentDto> UpdateAgentStatusToOnlineAsync(int id)
        {
            var agent = await _services.ReadSingleAsync<Agent>(id);
            agent.State = 1;
            await _services.UpdateAndSaveAsync<Agent>(agent);
            return _mapper.Map<AgentDto>(agent);
        }


        [Authorize]
        [HttpGet("Agent/{id}/updateStatusToChat")]
        public async Task<AgentDto> UpdateAgentStatusToChatAsync(int id)
        {
            var agent = await _services.ReadSingleAsync<Agent>(id);
            agent.State = 3;
            await _services.UpdateAndSaveAsync<Agent>(agent);
            return _mapper.Map<AgentDto>(agent);
        }
        [Authorize]
        [HttpPost("Agent")]
        public async Task<AgentDto> CreateAgentAsync([FromBody]AgentWithoutIdDto newAgent)
        {
            var agent = _mapper.Map<Agent>(newAgent);
            agent.State = 0;
            var agentResult = await _services.CreateAndSaveAsync<Agent>(agent);
            return _mapper.Map<AgentDto>(agentResult);
        }

        [Authorize]
        [HttpDelete("Agent/{id}")]
        public async Task RemoveAgentAsync(int id)
        {
            await _services.DeleteAndSaveAsync<Agent>(id);
        }

        [Authorize]
        [HttpPost("Agents:delete")]
        public async Task RemoveAgentsAsync(List<int> ids)
        {
            foreach (var id in ids)
            {
                await _services.DeleteAndSaveAsync<Agent>(id);
            }
        }

        [Authorize]
        [HttpPut("Agent/{id}")]
        public async Task<AgentDto> BindUserAsync(int id, [FromBody]User user)
        {
            var agent = await _services.ReadSingleAsync<Agent>(id);
            agent.UserAccount = user.Account;
            await _services.UpdateAndSaveAsync(agent);
            var newAgent = await _services.ReadSingleAsync<Agent>(id);
            return _mapper.Map<AgentDto>(newAgent);
        }
    


    }
}
