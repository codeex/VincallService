using AutoMapper;
using IdentityModel;
using IdentityServer4;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Vincall.Application;
using Vincall.Application.Dtos;
using Vincall.Infrastructure;
using Vincall.Service.Models;
using Vincall.Service.WebApiServices;

namespace Vincall.Service.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class SSoController : ControllerBase
    {
        private readonly ICrudServices _services;
        private readonly IMapper _mapper;
        private readonly IComm100OauthClient _comm100OauthClient;

        private readonly GlobalSettingService _settingService;
        private readonly IConfiguration _config;
        private readonly HostProvider _hostProvider;
        private readonly ILogger<SSoController> _logger;

        public SSoController(ICrudServices services, IMapper mapper, IComm100OauthClient comm100OauthClient, GlobalSettingService settingService,IConfiguration config, HostProvider hostProvider, ILogger<SSoController> logger)
        {
            _services = services;
            _mapper = mapper;
            _comm100OauthClient = comm100OauthClient;
            _settingService = settingService;
            _config = config;
            _hostProvider = hostProvider;
            _logger = logger;
        }

       
        /// <summary>
        /// agentid comes from my db
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> JwtSignInAsync([FromQuery]JwtModel model)
        {
            var user = HttpContext.User;
            var account = user.Claims.FirstOrDefault(x => x.Type == "UserAccount")?.Value;
            var userComm100 = await _services.ReadSingleAsync<UserComm100>(x => x.Account == account);
            if (userComm100 == null)
            {
                return BadRequest($"No Bind user for account: {account}");
            }
            //create jwt by usr
            var token = CreataToken(userComm100);
            //redirect comm100 access_token

            var uri = BuildUrl(model.redirect_url, new Dictionary<string, StringValues>
            {
                {"jwt",token},
                {"relayState",model.RelayState }
            });
            return Redirect(uri.ToString());


        }
        private Uri BuildUrl(string url, IDictionary<string, StringValues> parameters)
        {
            var uri = new UriBuilder(url);

            var queryString = QueryHelpers.ParseQuery(uri.Query);
            if (parameters != null)
            {
                foreach (var para in parameters)
                {
                    if (queryString.ContainsKey(para.Key))
                    {
                        queryString[para.Key] = StringValues.Concat(queryString[para.Key], para.Value);
                    }
                    else
                    {
                        queryString[para.Key] = para.Value;
                    }
                }
            }
            if (queryString.ContainsKey(string.Empty))
            {
                queryString.Remove(string.Empty);
            }
            uri.Query = QueryString.Create(queryString).ToUriComponent();

            return uri.Uri;
        }


        /// <summary>
        /// oauth login /redirect
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> SignOnAsync([FromQuery]SsoModel model)
        {
            var user = HttpContext.User;

            var account = user.Claims.FirstOrDefault(x => x.Type == "UserAccount")?.Value;
            var userComm100 = await _services.ReadSingleAsync<UserComm100>(x => x.Account == account);
            if (userComm100 == null)
            {
                return BadRequest($"No Bind user for account: {account}");
            }
            //create jwt by usr
            var token = CreataToken(userComm100);
            //redirect comm100 access_token

            var uri = BuildUrl(model.ReturnUrl, new Dictionary<string, StringValues>
            {
                {"token","token" }
            });
            return Redirect(uri.ToString());


        }

        /// <summary>
        ///  login vincall ,oauth callback,  //302
        ///  write cookie
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ConnectInfoDto> ConnectInfo()
        {
            var oauthInfo = await _settingService.GetAsync("vincallconnect");
            return new ConnectInfoDto
            {
                ClientId = oauthInfo?.client_id,
                Domain = oauthInfo?.domain,
            };
        }
        [HttpGet]
        public async Task<ConnectInfoDto> Comm100ConnectInfo()
        {
            var oauthInfo = await _settingService.GetAsync("comm100");
            return new ConnectInfoDto
            {
                ClientId = oauthInfo?.client_id,
                Domain = oauthInfo?.domain,
            };
        }
        [HttpGet]
        public async Task<IActionResult> ConnectCallbackAsync([FromQuery]SsoCallModel model)
        {
            var errMsg = string.Empty;
            var oauthInfo = await _settingService.GetAsync("vincallconnect");
            if (string.IsNullOrEmpty(oauthInfo?.client_id))
            {
                return new BadRequestResult();
            }
            var returnUri = string.IsNullOrEmpty(model.ReturnUri) ? oauthInfo.redirect_logon : model.ReturnUri;

            if (string.IsNullOrEmpty(model?.Code))
            {
                errMsg = "invalid authorizatoin code";
            }
            else
            {
                
                try
                {
                    var queryString = HttpContext.Request.QueryString.Value;
                    var originalUrl = queryString.Substring(0, queryString.IndexOf("&code="));
                    _hostProvider.Host = model.Domain ?? oauthInfo.domain;
                    oauthInfo.grant_type = oauthInfo.grant_type ?? "authorization_code";
                    _logger.LogInformation($"redirect verify: {oauthInfo.redirect_uri }{ originalUrl}");
                    var tokenResult = await _comm100OauthClient.QueryAccessTokenAsync(model.SiteId, model.Code, oauthInfo.client_id, oauthInfo.client_secret, oauthInfo.grant_type, oauthInfo.redirect_uri + originalUrl);
                    var info = await _comm100OauthClient.GetProfileInfoAsync($"{tokenResult.token_type} {tokenResult.access_token}", model.SiteId);

                    //not accesstoken
                    if (!string.IsNullOrEmpty(info.AgentId ))
                    {
                        
                        var uriRedirect = BuildUrl(returnUri, new Dictionary<string, StringValues> {
                            {"access_token",tokenResult.access_token },                           
                            {"success","true" },
                        });
                        await ConnectComm100(Convert.ToInt32(info.SiteId));
                        await _settingService.SaveConnectTokenAsync(Convert.ToInt32(info.SiteId), tokenResult.access_token, tokenResult.refresh_token);
                        return Redirect(uriRedirect.ToString());
                    }
                    else
                    {
                        var uriRedirect = BuildUrl(returnUri, new Dictionary<string, StringValues> {
                            {"errMsg","unauthorization" },
                            {"success","false" },
                        });
                        return Redirect(uriRedirect.ToString());                    
                    }


                }
                catch (Exception ex)
                {
                    errMsg = $"comm100 oauth code :{model.Code},{ex.Message}";
                    _logger.LogError(ex, $"comm100 oauth code :{model.Code},{ex.Message}");
                }
            }

            var uri = BuildUrl(returnUri, new Dictionary<string, StringValues> {
                            {"success","false" },
                            {"errMsg", errMsg },
                        });
            return Redirect(uri?.ToString());
        }
        [HttpGet]     
        public async Task<IActionResult> CallbackAsync([FromQuery]SsoCallModel model)
        {
            var errMsg = string.Empty;
            var oauthInfo = await _settingService.GetAsync();
            var returnUri = string.IsNullOrEmpty(model.ReturnUri) ? oauthInfo.redirect_logon : model.ReturnUri;
            if (string.IsNullOrEmpty(oauthInfo?.client_id))
            {
                return new BadRequestResult();
            }

            if (string.IsNullOrEmpty(model?.Code))
            {
                errMsg = "invalid authorizatoin code";
            }
            else
            {
                try
                {
                    var queryString = HttpContext.Request.QueryString.Value;
                    var originalUrl = queryString.Substring(0, queryString.IndexOf("&code="));
                    _hostProvider.Host = model.Domain;    
                    var info = await GetTokenAsync(model.Code, model.SiteId, originalUrl);

                    //find user
                    var userComm100 = await _services.ReadSingleAsync<UserComm100>(x => x.SiteId == info.SiteId && x.ExternId == info.AgentId);
                    if (userComm100 == null)
                    {                       
                       
                        var uriRedirect = BuildUrl(returnUri, new Dictionary<string, StringValues> {                         
                            {"errMsg", $"No bind user to comm100: {info.AgentId},{info.SiteId}" },                           
                            {"success","false" },
                        });
                        return Redirect(uriRedirect.ToString());
                    }
                    else
                    {
                        var user = await _services.ReadSingleAsync<User>(x => x.Account == userComm100.Account);
                        if (user == null)
                        {
                            errMsg = $"No  user for account: {userComm100.Account}";
                        }
                        else
                        {                            
                            await VincallTokenController.WriteCookieAsync(HttpContext, user);
                            var uriRedirect = BuildUrl(returnUri, new Dictionary<string, StringValues> {
                                {"userId",user.Id.ToString() },
                                {"role", user.IsAdmin? "admin": "user" },
                                {"userAccount",user.Account },
                                {"userName",user.UserName },
                                {"success","true" },
                            });
                            return Redirect(uriRedirect.ToString());
                        }
                    }
                    
                    
                }
                catch(Exception ex)
                {
                    errMsg = $"comm100 oauth code :{model.Code},{ex.Message}";
                    _logger.LogError(ex, $"comm100 oauth code :{model.Code},{ex.Message}");
                }
            }

            var uri = BuildUrl(returnUri, new Dictionary<string, StringValues> {
                            {"success","false" },
                            {"errMsg", errMsg },
                        });
            return Redirect(uri?.ToString());
        }

      
        private async Task<Comm100Info> GetTokenAsync(string code,string siteId, string queryString)
        {
            var info = await _settingService.GetAsync();
            info.grant_type = info.grant_type ?? "authorization_code";            
            var tokenResult = await _comm100OauthClient.QueryAccessTokenAsync(siteId, code,info.client_id,info.client_secret,info.grant_type,info.redirect_uri + queryString);
            return await _comm100OauthClient.GetProfileInfoAsync($"{tokenResult.token_type} {tokenResult.access_token}", siteId);  
        }

        private string CreataToken( UserComm100 context)
        {
            var publicKey = @"-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAspSN5RVLenOHtsyUeOXL
YDJHF/oagxUBmT3z/2v/ytOKbJ3sLBUNpeUYbs1gbmRuac8jNHG6nbg+rZNhnEkd
s5NI7VdqhBLZtwZTmDDJeofnbMfRL7weX+L9WyYPjMsGTUDOW68Mp3zNlLIFRUWI
6pJvDXVdqB7eU6O4bAd2qfQnfnkpO9yhqSiGk+SOQreUxL8IzVV8dRCpldmiqzFS
h48kVOkvtmf8i9JrTnKVE5/yKxKQkzNWqaMa4exF2zRTVQ/fJz6gtLbPBH56SDsh
v2z4ZoyzsCactxTtvdnaqM66k0pSwQMZSJjSH1oW8zkub8RIRlDJrneEH3bifHWT
yQIDAQAB
-----END PUBLIC KEY-----
";
            var privateKey = _config["TokenPrivateKey"];
            RSA rsa = RSA.Create();
            rsa.ImportEncryptedPkcs8PrivateKey(Encoding.UTF8.GetBytes("vincall"), Convert.FromBase64String(privateKey), out _);
            var securityKey = new RsaSecurityKey(rsa);

            List<Claim> authClaims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject, context.Account??""),
                new Claim("AgentId", context.ExternId??""),
                new Claim("partnerId", context.PartnerId??""),
                new Claim(JwtClaimTypes.Email, context.Email??""),
                new Claim("siteId",context.SiteId??""),
                new Claim(ClaimTypes.NameIdentifier,context.Email??"")
            };
            var token = new JwtSecurityToken(  "auth", "vincall", authClaims, DateTime.UtcNow, DateTime.UtcNow.AddMinutes(10), new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256));
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;

        }

        private async Task ConnectComm100(int siteId)
        {
            var connectionState = await _services.ReadSingleAsync<VincallConnectionState>(item => item.SiteId == siteId);

            if (connectionState != null)
            {
                connectionState.Connected = true;

                await _services.UpdateAndSaveAsync<VincallConnectionState>(connectionState);
            }
            else
            {
                await _services.CreateAndSaveAsync<VincallConnectionState>(new VincallConnectionState
                {
                    Connected = true,
                    SiteId = siteId,
                    Server="Comm100"
                });
            }
        }


    }
}
