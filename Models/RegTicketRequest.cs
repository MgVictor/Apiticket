using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiTicket.Models
{
    public class RegTicketRequest
    {
        public string Canal { get; set; }
        public int Tipo { get; set; }
        public int Estado { get; set; }
        public string Usuario { get; set; }
        public string FechaRecepcion { get; set; }
        public string Ramo { get; set; }
        public string Producto { get; set; }
        public string Poliza { get; set; }
        public Via ViaRespuesta { get; set; }
        public Via ViaRecepcion { get; set; }
        public SubMotivo SubMotivo { get; set; }
        public Client Contacto { get; set; }
        public string EmailTitular { get; set; }
        public string DireccionTitular { get; set; }
        public string UbigeoTitular { get; set; }
        public string Monto { get; set; }
        public string Reconsideracion { get; set; }
        public string Descripcion { get; set; }
        public string docClient { get; set; }
        public string tipoDocClient { get; set; }
        public string Respuesta { get; set; }
        public List<Archivo> Adjuntos { get; set; }
        public string NID_TICK_PREST { get; set; }
        public string NID_TBL_TICK_TYPE_JIRA { get; set; }//Trámite de Rentas //56
        public string PartnerType { get; set; }
        public string ProductSimple { get; set; }
        public string SbsCode { get; set; }
        public string CreditRequire { get; set; }
        public string ContractFirm { get; set; }
    }
}
