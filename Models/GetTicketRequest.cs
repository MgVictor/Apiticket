using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiTicket.Models
{
    public class GetTicketRequest
    {
        public int tipoDocumento { get; set; }
        public string documento { get; set; }
        public string Usuario { get; set; }
        public string fechaInicio { get; set; }
        public string fechaFin { get; set; }
        public int viaRecep { get; set; }
        public int Ramo { get; set; }
        public int Producto { get; set; }
        public int Estado { get; set; }
    }
}
