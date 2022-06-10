using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vincall.Infrastructure;

namespace Vincall.Service.Models
{
    public class Settings
    {
        public static List<Setting> All => new List<Setting>
        {
            new Setting
            {
                OptionKey="Working Hours",
                OptionValue="9:00-20:00",
                Type=1
            },
            new Setting
            {
                OptionKey="Job description",
                OptionValue=$"Hello world",
                Type=2
            },            

        };
    }
}
