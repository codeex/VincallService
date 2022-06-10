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
    [Route("[controller]")]
    public class GlobalSettingController : ControllerBase
    {
        private readonly ICrudServices _services;
        private readonly IMapper _mapper;

        public GlobalSettingController(ICrudServices services, IMapper mapper)
        {
            _services = services;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet]
        public async Task<List<GlobalSettingDto>> GetGlobalSettingsByType(string type)
        {
            var globalSettings = _services.ReadMany<GlobalSetting>(x => x.Type==type)?.ToList() ?? new List<GlobalSetting>();
            return _mapper.Map<List<GlobalSettingDto>>(globalSettings);
        }

        [Authorize]
        [HttpPost("{id}")]
        public async Task<GlobalSettingDto> UpdateGlobalSettingsByType(long id, GlobalSettingDto settingDto)
        {
            var setting = _mapper.Map<GlobalSetting>(settingDto);
            setting.Id = id;
            await _services.UpdateAndSaveAsync<GlobalSetting>(setting);
            var settingResult = await _services.ReadSingleAsync<GlobalSetting>(id);
            return _mapper.Map<GlobalSettingDto>(settingResult);
        }
    }
}
