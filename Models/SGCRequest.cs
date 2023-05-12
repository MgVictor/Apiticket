using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiTicket.Models
{
    public class SGCRequest
    {
        public string usuario { get; set; }
        public string grupoComercial { get; set; }
        public string search { get; set; }
        public string type { get; set; }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        //public int paginado {get;set;}
    }
}
