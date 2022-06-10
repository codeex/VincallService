using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Vincall.Infrastructure
{
    public class UserComm100
    {
        [Key]
        public long Id { get; set; }
        public string Account { get; set; }
        public string ExternId { get; set; }
        public string SiteId { get; set; }
        public string PartnerId { get; set; }
        public string Email { get; set; }
    }
}
