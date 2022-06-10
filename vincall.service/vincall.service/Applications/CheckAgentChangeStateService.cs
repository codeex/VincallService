using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vincall.Application;
using Vincall.Infrastructure;
using Vincall.Service.Services;

namespace Vincall.Service.Applications
{
    public class CheckAgentChangeStateService : ICheckAgentChangeStateService
    {

        private readonly ILogger<CheckAgentChangeStateService> _logger;
        private ICrudServices _services;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly Timer _timer;
        public CheckAgentChangeStateService(IServiceScopeFactory serviceScopeFactory, ILogger<CheckAgentChangeStateService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _timer = new Timer(OnTimeEvent, null, 20000, 30 * 1000);
        }

        private void OnTimeEvent(object sender)
        {
            try
            {
                CheckAgentChangeState();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex,ex.Message);
            }
        }
        public void CheckAgentChangeState()
        {
            using (var serviceScope = _serviceScopeFactory.CreateScope())
            {
                _services = serviceScope.ServiceProvider.GetService<ICrudServices>();
                var stashAgentTime = DateTime.UtcNow.AddSeconds(-30);
                var agents = _services.ReadMany<Agent>(x => x.UpdateDate < stashAgentTime)?.ToList()??new List<Agent>();
                if (agents.Count > 0)
                {
                    foreach (var agent in agents)
                    {
                        agent.State = 0;
                        _services.UpdateAndSaveAsync<Agent>(agent).GetAwaiter();
                    }
                }
                
            }
        }
    }
}
