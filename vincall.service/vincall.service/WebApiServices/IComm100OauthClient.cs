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
    public interface IComm100OauthClient : IHttpApi
    {
        /// <summary>
        /// redirect_uri in place end param.
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="code"></param>
        /// <param name="client_id"></param>
        /// <param name="client_secret"></param>
        /// <param name="grant_type"></param>
        /// <param name="redirect_uri"></param>
        /// <returns></returns>
        [HttpPost("/oauth/token")]
        [HostFilterAttribute]
        Task<TokenResult> QueryAccessTokenAsync([PathQuery]string siteId, [FormContent]string code, [FormContent]string client_id, [FormContent]string client_secret, [FormContent]string grant_type, [FormContent]string redirect_uri);

        /// <summary>
        /// Bearer type
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet("/oauth/userinfo")]
        [HostFilterAttribute]
        ITask<Comm100Info> GetProfileInfoAsync([Header("Authorization")]string token,[PathQuery]string siteId);

        [HttpPost("/oauth/token")]
        [HostFilterAttribute]
        Task<TokenResult> RefreshTokenAsync([PathQuery]int siteId, [FormContent]string client_id, [FormContent]string client_secret, [FormContent]string grant_type, [FormContent]string refresh_token);


    }
}
