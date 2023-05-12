using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiTicket.Models
{

  public class Beneficiario {
        public string codTipoDoc { get; set; }
        public string nroDocumento { get; set; }  
        public int? tipoBeneficiario {get; set;}
  }

}