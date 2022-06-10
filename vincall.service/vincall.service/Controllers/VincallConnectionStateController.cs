using AutoMapper;
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
    [Route("ConnectState")]
    [ApiController]
    public class VincallConnectionStateController:ControllerBase
    {
        private readonly ICrudServices _services;
        private readonly IMapper _mapper;
        public VincallConnectionStateController(ICrudServices services, IMapper mapper)
        {
            _services = services;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<VincallConnectionStateDto> Get(int siteId)
        {
            var connectionState = await _services.ReadSingleAsync<VincallConnectionState>(item => item.SiteId == siteId);
            if (connectionState == null)
            {
                return new VincallConnectionStateDto() {
                    SiteId = siteId,
                    Connected=false
                };
            }

            var result = _mapper.Map<VincallConnectionStateDto>(connectionState);
            return result;
        }

        //[Authorize]
        [HttpPut("disconnect")]
        public async Task<VincallConnectionStateDto> Set(int siteId)
        {
            var connectionState = await _services.ReadSingleAsync<VincallConnectionState>(item => item.SiteId == siteId);

            if (connectionState != null)
            {
                connectionState.Connected = false;

                await _services.UpdateAndSaveAsync<VincallConnectionState>(connectionState);
            }

            var connectionStateNew = await _services.ReadSingleAsync<VincallConnectionState>(item => item.SiteId == siteId);

            if (connectionStateNew == null)
            {
                return new VincallConnectionStateDto()
                {
                    SiteId = siteId,
                    Connected = false
                };
            }

            var result = _mapper.Map<VincallConnectionStateDto>(connectionStateNew);

            return result;
        }
    }
}
