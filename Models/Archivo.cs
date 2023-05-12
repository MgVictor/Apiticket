using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiTicket.Models
{
    public class Archivo
    {
        public string name { get; set; }
        public string size { get; set; }
        public string path { get; set; }
        public string scode { get; set; }
        public string nuser { get; set; }
        public string path_gd { get; set; }
        public string tipo { get; set; }
        public string content { get; set; }
        public string mime { get; set; }
        public string scodejira { get; set; }
        //DEV CY -- INI
        public string nid { get; set; }//ID
        public string sstate { get; set; }//CODIGO: 1(NUEVO)  0(ELIMINAR)
        //DEV CY -- FIN
    }
}
