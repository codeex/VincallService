using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vincall.Service.Models;
using WebApiClientCore;
using WebApiClientCore.Attributes;

namespace Vincall.Service.WebApiServices
{
    [LoggingFilter]
    public interface IComm100ApiClient : IHttpApi
    {

        /// <summary>
        /// Bearer type
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("/api/global/agents")]
        [HostFilterAttribute]
        ITask<List<Comm100Agent>> GetAgentsAsync([Header("Authorization")]string token,[PathQuery]int siteId);

    }   
}
