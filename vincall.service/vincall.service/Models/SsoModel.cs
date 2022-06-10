using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vincall.Service.Models
{
    public class SsoModel
    {
        public string AgentId { get; set; }
        public int SiteId { get; set; }
        public string Domain { get; set; }
        public string ReturnUrl { get; set; }
    }
}
