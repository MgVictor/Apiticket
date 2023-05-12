using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiTicket.Models
{
    public class TicketSiniestro
    {
        public int? codTipoOperacion { get; set; }
        public int? nroSiniestro { get; set; }
        public string nroCaso { get; set; }
        public string fecRegistro { get; set; }
        public string codTipoTramite { get; set; }
        public string codEstado { get; set; }
        public Poliza Poliza { get; set; }
        public Cliente Asegurado {get; set;}
        public Preapertura Preapertura {get; set;}
        public Cliente Titular {get; set;}
        public List<Beneficiario> listBeneficiario {get;set;}
        public List<Cobertura> listCobertura {get; set;}
        public string fecDeclaracion {get; set;}
        public Decimal? monReserva {get; set;}
        public string usuario {get; set;}
    }
}
