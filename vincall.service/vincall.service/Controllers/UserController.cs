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
    public class UserController: ControllerBase
    {
        private readonly ICrudServices _services;
        private readonly IMapper _mapper;
        public UserController(ICrudServices services, IMapper mapper)
        {
            _services = services;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet("user/{id}")]
        public async Task<UserDto> QueryUserAsync(int id)
        {
            var user = await _services.ReadSingleAsync<User>(id);
            var userdto = _mapper.Map<UserDto>(user);
            return userdto;
        }

        [Authorize]
        [HttpGet("users")]
        public async Task<UserResult> QueryUsersAsync(int pageSize=0, int pageNum=0)
        {
            var result = new UserResult();
            var users = _services.ReadManyNoTracked<User>();
            result.Count = users.Count();
            if (pageSize != 0 || pageNum != 0)
            {
                users = users.Page<User>(pageNum, pageSize);
            }
            result.Users = _mapper.Map<List<UserDto>>(users.ToList());
            return await Task.FromResult<UserResult>(result);
        }

        [Authorize]
        [HttpPatch("user/{id}")]
        public async Task<UserDto> UpdateUserAsync(int id,[FromBody]User newUser)
        {
            var oldUser = await _services.ReadSingleAsync<User>(id);
            oldUser.Password = Md5Helper.Md5(newUser.Password);
            await _services.UpdateAndSaveAsync<User>(oldUser);
            var user = await _services.ReadSingleAsync<User>(id);
            return _mapper.Map<UserDto>(user);
        }

        [Authorize]
        [HttpPost("user")]
        public async Task<UserDto> CreateUserAsync([FromBody]User newUser)
        {
            newUser.Password= Md5Helper.Md5(newUser.Password);
            var user=await _services.CreateAndSaveAsync<User>(newUser);
            return _mapper.Map<UserDto>(user);
        }

        [Authorize]
        [HttpDelete("user/{id}")]
        public async Task RemoveUserAsync(int id)
        {
            await _services.DeleteAndSaveAsync<User>(id);
        }

        [Authorize]
        [HttpPost("users:delete")]
        public async Task RemoveUsersAsync(List<int> ids)
        {
            foreach(var id in ids)
            {
                await _services.DeleteAndSaveAsync<User>(id);
            }
        }


        [Authorize]
        [HttpGet("userInfo")]
        public async Task<UserDto> GetCurrentUserInfo()
        {
            var userAccount = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserAccount")?.Value;
            var user = await _services.ReadSingleAsync<User>(x => x.Account == userAccount);
            return _mapper.Map<UserDto>(user);
        }


    }
}
