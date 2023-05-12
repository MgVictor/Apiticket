using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiTicket.Models
{
    public class SubMotivo
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        
        public int CodigoSBS { get; set; }
        public string DescripcionSBS { get; set; }
        //DEV EC - INICIO
        public string CobranTram { get; set; }
        //DEV EC - FIN
        public Motivo Motivo { get; set; }

    }
}
