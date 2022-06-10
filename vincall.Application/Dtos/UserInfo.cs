using System;
using System.Collections.Generic;
using System.Text;

namespace Vincall.Application.Dtos
{
    public class UserInfo
    {
        public string role { get; set; }
        public string email { get; set; }
        public string sub { get; set; }
        public string ddp { get; set; }
        public string agentId { get; set; }
        public string userId { get; set; }
        public string siteId { get; set; }
        public string partnerId { get; set; }
        public string thumbprint { get; set; }
        public string arm { get; set; }
        public long auth_time { get; set; }
        public string success { get; set; }
    }
}
