using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiTicket.Models
{
    public class ParamReporte
    {
        public string year { get; set; }
        public string mesini { get; set; }
        public string mesfin { get; set; }
        public string year_ant { get; set; }
        public string mesini_ant { get; set; }
        public string mesfin_ant { get; set; }
        public string scode { get; set; }
        public string tipoReporte { get; set; }
    }
}
