using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vincall.Infrastructure;

namespace Vincall.Service.Models
{
    public class Agents
    {
        public static List<Agent> All => new List<Agent>
        {
            new Agent
            {
                UserAccount="Peter",
                DeviceNumber="1234",
                Remark="string",
                State=1
            },
            new Agent
            {
                UserAccount="Tom",
                DeviceNumber="12348",
                Remark="string",
                State=1
            },
            new Agent
            {
                UserAccount="Grubby",
                DeviceNumber="12346",
                Remark="string",
                State=1
            },
            new Agent
            {
                UserAccount="Jerry",
                DeviceNumber="1",
                Remark="string",
                State=1
            },
            new Agent
            {
                UserAccount="Mary",
                DeviceNumber="2",
                Remark="string",
                State=1
            },
            new Agent
            {
                UserAccount="Dahan1",
                DeviceNumber="3",
                Remark="string",
                State=1
            },                      
        };
    }
}
