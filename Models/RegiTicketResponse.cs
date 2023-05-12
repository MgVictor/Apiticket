using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiTicket.Models
{
    public class RegiTicketResponse : CommonResponse
    {
        public string Codigo { get; set; }
        public string mensaje { get; set; }
    }
}
