using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiTicket.Models
{
    public class Campo
    {
        public string id { get; set; }
        public object value { get; set; }
    }

    public class ResponseListaJIRA
    {
        public string id { get; set; }
        public List<Campo> fields { get; set; }
    }
}
