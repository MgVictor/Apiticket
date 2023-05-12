using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiTicket.Models
{
    public class RegContractRequest
    {
        //BODY
        public string summary = "";
        public int tramiteSin { get; set; }
        public int cobertura { get; set; }
        public int tipoPago { get; set; }
        public int ramo = 66;
        public string nombreAseg { get; set; }
        public int tipoDocAseg { get; set; }
        public string nroDocAseg { get; set; }
        public string correoAseg { get; set; }
        public int celAseg { get; set; }
        public string dirAseg { get; set; }
        public string nombreContacto { get; set; }
        public int tipoDocContac { get; set; }
        public string nroDocContac{get;set;}
        public string correoContac{get;set;}
        public int celContac{get;set;}
        public string dirContac{get;set;}
        public string banco{get;set;}
        public string cuentaDestino{get;set;}
        public string cuentaCCI{get;set;}
        public DateTime fechaSiniestro{get;set;}
        public DateTime fechaRecepcion{get;set;}
        public List<Adjunto> adjunto {get;set;}
        // ------------------------------------------------------------
        public int Estado { get; set; }
        public string NID_TICK_PREST { get; set; }
        public string NID_TBL_TICK_TYPE_JIRA { get; set; }
        public string Canal { get; set; }
        public int Tipo { get; set; }
        public string Usuario { get; set; }
        public string Poliza { get; set; }
        public Via ViaRespuesta { get; set; }
        public Via ViaRecepcion { get; set; }
        public SubMotivo SubMotivo { get; set; }
        public string Monto { get; set; }
        public string Reconsideracion { get; set; }
        public string Respuesta { get; set; }
        public string PartnerType { get; set; }
        public string ProductSimple { get; set; }
        public string SbsCode { get; set; }
        public string CreditRequire { get; set; }
        public string ContractFirm { get; set; }
    }
}