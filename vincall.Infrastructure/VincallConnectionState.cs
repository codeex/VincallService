using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Vincall.Infrastructure
{
    public class VincallConnectionState
    {
        [Key]
        public int SiteId { get; set; }

        public bool Connected { get; set; }

        public string Server { get; set; }
    }
}
