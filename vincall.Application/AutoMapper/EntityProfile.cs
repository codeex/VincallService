using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Vincall.Application.Dtos;
using Vincall.Infrastructure;

namespace Vincall.Application.AutoMapper
{
    public class EntityProfile: Profile
    {
        public EntityProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<Agent, AgentDto>();
            CreateMap<GlobalSetting, GlobalSettingDto>();
            CreateMap<GlobalSettingDto, GlobalSetting>();
            CreateMap<CallList, CallListDto>();
            CreateMap<Setting, SettingDto>();
            CreateMap<TwilioSetting, TwilioSettingDto>();
            CreateMap<AgentWithoutIdDto, Agent>();
            CreateMap<UserComm100, UserMappingDto>()
                .ForMember(x=>x.UserAccount,x=>x.MapFrom(o=>o.Account))
                .ForMember(x => x.Comm100AgentId, x => x.MapFrom(o => o.ExternId))
                .ForMember(x => x.Comm100Email, x => x.MapFrom(o => o.Email)).ReverseMap();
            CreateMap<VincallConnectionState, VincallConnectionStateDto>();
           
        }
    }
}
