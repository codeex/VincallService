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
    public class SettingController : ControllerBase
    {
        private readonly ICrudServices _services;
        private readonly IMapper _mapper;
        public SettingController(ICrudServices services, IMapper mapper)
        {
            _services = services;
            _mapper = mapper;
        }
        [Authorize]
        [HttpGet("Setting/{id}")]
        public async Task<SettingDto> QuerySettingAsync(int id)
        {
            var setting = await _services.ReadSingleAsync<Setting>(id);
            var settingDto = _mapper.Map<SettingDto>(setting);
            return settingDto;
        }

        [Authorize]
        [HttpGet("Settings")]
        public async Task<List<SettingDto>> QuerySettingsAsync()
        {
            var Settings = _services.ReadManyNoTracked<Setting>();
            return await Task.FromResult<List<SettingDto>>(_mapper.Map<List<SettingDto>>(Settings.ToList()));
        }

        [Authorize]
        [HttpPatch("Setting/{id}")]
        public async Task<SettingDto> UpdateSettingAsync(int id, [FromBody]Setting newSetting)
        {
            await _services.UpdateAndSaveAsync<Setting>(newSetting);
            var Setting = await _services.ReadSingleAsync<Setting>(id);
            return _mapper.Map<SettingDto>(Setting);
        }
      
    }
}
