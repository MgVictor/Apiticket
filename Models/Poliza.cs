using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiTicket.Models
{

  public class Poliza {
        public int? codRamo { get; set; }
        public string codProducto { get; set; }
        public string nroPoliza { get; set; }
        public string nroCertificado { get; set; }
        public string codEstado { get; set; }
        public string codTipoContr { get; set; }
        public string nroExpendiente { get; set; }
        public int? nroDiasHospitalarios { get; set; }
     
  }

}