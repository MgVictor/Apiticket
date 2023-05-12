using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiTicket.Models
{
    public class CommonResponse
    {
         public bool respuesta { get; set; }
         public List<string> mensajes { get; set; }
         public string  codigoRespuestaError { get; set; }
    }
}
