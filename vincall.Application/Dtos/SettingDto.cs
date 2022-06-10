using System;
using System.Collections.Generic;
using System.Text;

namespace Vincall.Application.Dtos
{
    public class SettingDto
    {
        public int Id { get; set; }
        public string OptionKey { get; set; }
        public string OptionValue { get; set; }
        public int? Type { get; set; }
    }
}
