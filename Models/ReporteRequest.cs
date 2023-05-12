using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiTicket.Models
{
    public class ReporteRequest
    {
        public string codigo { get; set; }
        public string tipo { get; set; }
        public string trimestre { get; set; }
        public string anio { get; set; }
        public int? definitivo { get; set; }
        public string usario { get; set; }
        public string fecha { get; set; }
        public string estado { get; set; }
        public string operaciones { get; set; }

    }
}
