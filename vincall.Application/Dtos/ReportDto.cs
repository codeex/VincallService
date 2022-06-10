using System;
using System.Collections.Generic;
using System.Text;

namespace Vincall.Application.Dtos
{
    public class ReportDto
    {
        public int AgentId { get; set; }

        public string UserName { get; set; }

        public int IncomingCall { get; set; }

        public int OutboundCall { get; set; }

        public int? CallTime { get; set; }
    }

    public class ReportResult
    {
        public List<ReportDto> Reports { get; set; }

        public int Count { get; set; }
    }
}
