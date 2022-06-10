using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vincall.Service.Models
{
    public class SsoCallModel
    {

        public string Code { get; set; }
        public string AgentId { get; set; }
        public string SiteId { get; set; }
        public string Domain { get; set; }
        public string Scope { get; set; }
        public string Token { get;  set; }

        public string ReturnUri { get; set; }
    }
}
