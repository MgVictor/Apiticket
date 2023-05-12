using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiTicket.Models
{
    public class DatosPagoRequest
    {
        public string P_SCODE { get; set; }
        public int P_IDSUBMOTIVO { get; set; }
        public string P_NAMESUBMOTIVO { get; set; }
        public string P_SNOTA_CRED { get; set; }
        public string P_STITULAR { get; set; }
        public string P_NTIPDOC_TITULAR { get; set; }
        public string P_SDOC_TITULAR { get; set; }
        public string P_NBANCO { get; set; }
        public string P_NMONEDA { get; set; }
        public string P_SMONTO { get; set; }
        public string P_NTIPO_CUENTA { get; set; }
        public string P_SCUENTA_DEST { get; set; }
        public string P_SCOMENTA_PAGO { get; set; }
        public int P_NID_GROUP { get; set; }
        public int P_NCANAL_INGRESO { get; set; }

        public string P_CANT_ADJUNT { get; set; }

        public List<Archivo> files { get; set; }

        public string scode { get; set; }

        public string flag_file { get; set; }//0 sin cambios 1 con cambios
        
        /*DEV DS INICIO*/
        //revisarlo
        public string P_SUB_MOTIVO { get; set; }
        public string P_RESPUESTA { get; set; }
        //public string SCODE { get; set; }
        //public string SCODE_JIRA { get; set; }
        //public List<Archivo> AdjuntosArch { get; set; }
        /*DEV DS FIN*/
    }
}
