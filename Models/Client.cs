using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiTicket.Models
{
    public class Client
    {
        public string name { get; set; }        
        public string tipodoc { get; set; }
        public string documento { get; set; }
        public string correo { get; set; }
        public string direccion { get; set; }
        public string ubigeo { get; set; }

    }
}
