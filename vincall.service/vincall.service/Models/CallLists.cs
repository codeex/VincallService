using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vincall.Infrastructure;

namespace Vincall.Service.Models
{
    public class CallLists
    {
        public static List<CallList> All => new List<CallList>
        {
            new CallList
            {
                CallTime=10,
                ExtensionNumber="1234",
                From="4321",
                To="110",
            },           
        };
    }
}
