using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiTicket.Models
{
    public class Alerta
    {
        public long NID { get; set; }
        public int NTYPE { get; set; }
        public string SMESSAGE { get; set; }
        public string SLINK { get; set; }
    }
}
