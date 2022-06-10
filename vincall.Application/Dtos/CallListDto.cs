using System;
using System.Collections.Generic;
using System.Text;

namespace Vincall.Application.Dtos
{
    public class CallListDto
    {
        public int Id { get; set; }
        public string ExtensionNumber { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public int? CallTime { get; set; }
        public DateTime? CreateDate { get; set; }
    }

    public class CallListResult
    {
        public List<CallListDto> CallLists { get; set; }

        public int Count { get; set; }
    }
}
