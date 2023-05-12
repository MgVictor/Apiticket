using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiTicket.Models
{
    public class RR1
    {
        public List<RR1_Cabecera> Cabecera_Reporte { get; set; }
        public List<RR1_Detalle> Detalle_Reporte { get; set; }
        public List<RR1_Detalle> Totales_Reporte { get; set; }

    }
}
