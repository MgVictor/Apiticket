using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiTicket.Models
{
    public class GenerateResponse
    {
        public int P_NCODE { get; set; }
        public string P_SMESSAGE { get; set; }
        public Object data { get; set; }
    }
}
