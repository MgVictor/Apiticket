using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiTicket.Models
{
    public class Adjunto
    {
        internal string path_gd { get; set; }
        public string name { get; set; }
        public string size { get; set; }
        public string path { get; set; }
        public string tipo { get; set; }
        public string content { get; set; }
        public string mime { get; set; }
        public string scodejira { get; set; }
        public string scode { get; set; }
    }
}