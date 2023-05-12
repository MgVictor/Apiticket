using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiTicket.Models
{
    public class RR1_Cabecera
    {
        public string SCODE { get; set; }
        public string NYEAR { get; set; }
        public string NTRIMESTRE { get; set; }
        public string TOTREC { get; set; }
        public string TOTPROM { get; set; }
        public string TOTOPER { get; set; }
        
        //SUCAVE
        public string IdFormato { get; set; }
        public string IdAnexo { get; set; }
        public string CodigoSBSEmpresaVigilada { get; set; }
        public string FechaReporte { get; set; }
        public string CodigoExpresionMonto { get; set; }
        public string DatoControl { get; set; }
    }
}
