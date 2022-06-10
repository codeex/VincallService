using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Twilio.TwiML;
using Twilio.TwiML.Voice;
using System.Text.RegularExpressions;
using Vincall.Service.Models;
using Vincall.Service.Cache;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Vincall.Application;
using Vincall.Infrastructure;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace vincall.service.Controllers
{
    public class VoiceController : Controller
    {

        private readonly ITwilioSettingCacheService _twilioSettingCacheService;
        private readonly ICrudServices _services;

        private ILogger<VoiceController> _logger;

        public VoiceController(ITwilioSettingCacheService twilioSettingCacheService, ICrudServices services, ILogger<VoiceController> logger)
        {
            _twilioSettingCacheService=twilioSettingCacheService;
            _services = services;
            _logger = logger;
        }

        // POST: /voice

        //[Authorize]
        [HttpPost("voice")]
        public async Task<IActionResult> Index(string to, string callingDeviceIdentity)
        {

            _logger.LogInformation($"step1: enter voice  To:{to}-----id:{callingDeviceIdentity}");
            var twilioSetting = _twilioSettingCacheService.GetTwilioSettingCache().FirstOrDefault();
            var callerId = twilioSetting.CallId;
            var twiml = new VoiceResponse();

            // someone calls into my Twilio Number, there is no thisDeviceIdentity passed to the /voice endpoint 
            if (string.IsNullOrEmpty(callingDeviceIdentity))
            {

                _logger.LogInformation($"branch1: id is null------------");
                var calledagent = _services.ReadMany<Agent>(x => x.State == 1).FirstOrDefault();
                var avaliableIndentity = calledagent?.DeviceNumber;
                if (avaliableIndentity == null)
                {
                    _logger.LogInformation($"branch1.1: avaliableAgentid is null------------");
                    var exceptionResult = new VoiceResponse();
                    exceptionResult.Say("Sorry, there are no valiable agents now.");
                    return Content(exceptionResult.ToString(), "text/xml");
                }
                var dial = new Dial();
                var client = new Client();
                client.Identity(avaliableIndentity);
                dial.Append(client);
                twiml.Append(dial);
                calledagent.State = 2;
                await _services.UpdateAndSaveAsync<Agent>(calledagent);

                _logger.LogInformation($"branch1.3: huru:  to: {to}, callingDeviceIdentity: {callingDeviceIdentity}, thisDevice.Identity: {avaliableIndentity}");

            }
            //else if (callingDeviceIdentity != avaliableIndentity)
            //{
            //    var dial = new Dial();
            //    var client = new Client();
            //    client.Identity(avaliableIndentity);
            //    dial.Append(client);
            //    twiml.Append(dial);
            //}
            // if the POST request contains your browser device's identity
            // make an outgoing call to either another client or a number
            else
            {
                _logger.LogInformation($"branch2.1: huchu id:{callingDeviceIdentity}------------");
                var callingAgent = await _services.ReadSingleAsync<Agent>(x => x.DeviceNumber == callingDeviceIdentity);
                callingAgent.State = 2;
                await _services.UpdateAndSaveAsync<Agent>(callingAgent);
                var dial = new Dial(callerId: callerId);

                // check if the 'To' property in the POST request is
                // a client name or a phone number
                // and dial appropriately using either Number or Client
                _logger.LogInformation($"branch2.2: huchu id:{callingDeviceIdentity}------------");
                if (Regex.IsMatch(to, "^[\\d\\+\\-\\(\\) ]+$"))
                {
                    Console.WriteLine("Match is true");
                    dial.Number(to);

                    _logger.LogInformation($"branch2.3: huchu id:{to}------------");
                }
                else
                {
                    if (to.StartsWith("EXT:"))
                    {
                        var client = new Client();
                        client.Identity(to.Substring(4));
                        dial.Append(client);
                        _logger.LogInformation($"branch2.2: huchu id:{to}------------");
                    }
                    else
                    {
                        var exceptionResult = new VoiceResponse();
                        exceptionResult.Say("Sorry, the operation don't support.");
                        return Content(exceptionResult.ToString(), "text/xml");
                    }

                }

                twiml.Append(dial);

                _logger.LogInformation($"huchu:  to: {to}, callingDeviceIdentity: {callingDeviceIdentity}, thisDevice.Identity: {callingDeviceIdentity}");
            }

            Console.WriteLine(twiml.ToString());
            _logger.LogInformation($"-----voiceresult: {twiml.ToString()}");

            return Content(twiml.ToString(), "text/xml");
        }
    }
}
