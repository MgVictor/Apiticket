using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiTicket.Models
{

    public class TicketResponse {
        public bool respuesta { get; set; }
        public string mensaje  {get; set;}
        public string codTicket {get; set;}
        public List<Error> errorList {get; set;}

        public TicketResponse (){
            errorList = new List<Error> ();
        }

    }

}