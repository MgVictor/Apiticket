using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiTicket.Models.Reportes
{
    public class RR3
    {
        public List<RR3_Cabecera> Cabecera_Reporte { get; set; }
        public List<RR3_Detalle> Detalle_Reporte { get; set; }
        public List<RR3_Detalle> Totales_Reporte { get; set; }
    }
}
