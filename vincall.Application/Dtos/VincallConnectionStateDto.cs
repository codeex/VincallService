using System;
using System.Collections.Generic;
using System.Text;

namespace Vincall.Application.Dtos
{
    public class VincallConnectionStateDto
    {
        public int SiteId { get; set; }

        public bool Connected { get; set; }

        public string Server { get; set; }
    }
}
