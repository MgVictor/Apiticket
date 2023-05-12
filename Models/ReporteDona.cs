using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiTicket.Models
{
    public class ReporteDona
    {
        public string MES { get; set; }
        public int? REGTRA { get; set; }
        public int? CERRTRA { get; set; }
        public int? REGSOL { get; set; }
        public int? CERRSOL { get; set; }
        public int? REGCON { get; set; }
        public int? CERRCON { get; set; }
        public int? REGREC { get; set; }
        public int? CERRREC { get; set; }
    }
}
