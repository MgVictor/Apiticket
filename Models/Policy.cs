using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiTicket.Models
{
    public class Policy
    {
        public int NroPoliza { get; set; }
        public Estado Estado { get; set; }
        public string FecInicioVigencia { get; set; }
        public string FecFinVigencia { get; set; }
    }
}
