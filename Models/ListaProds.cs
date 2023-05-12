using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiTicket.Models
{
    public class ListaProds
    {
        public string status { get; set; }
        public List<Product> data { get; set; }
    }
}
