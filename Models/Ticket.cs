using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiTicket.Models
{
    public class Ticket
    {
        public string Codigo { get; set; }
        public string CodigoJIRA { get; set; }
        public string Nombre { get; set; }
        public string Contacto { get; set; }
        public string TipoDocumento { get; set; }
        public string Documento { get; set; }
        public string TipoDocumentoCli { get; set; }
        public string DocumentoCli { get; set; }
        public string Dias { get; set; }
        public string Canal { get; set; }
        public string FecRecepcion { get; set; }
        public string FecRegistro { get; set; }
        public string ViaRecepcion { get; set; }
        public string ViaRespuesta { get; set; }
        public string Ramo { get; set; }
        public string Producto { get; set; }
        public string Poliza { get; set; }
        public string Estado { get; set; }
        public string Direccion { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Motivo { get; set; }
        public string MotivoREC { get; set; }
        public string MotivoSOL { get; set; }
        public string SubMotivo { get; set; }
        public string SubMotivoREC { get; set; }
        public string SubMotivoSOL { get; set; }
        public string Monto { get; set; }
        public string Reconsideracion { get; set; }
        public string Descripcion { get; set; }
        public string Vinculo { get; set; }
        public string TipoCaso { get; set; }
        public string EmailCli { get; set; }
        public string Aplicacion { get; set; }
        public string Ejecutivo { get; set; }
        public string Tipo { get; set; }
        public string Proyecto { get; set; }
        public string Summary { get; set; }
        public string Reporter { get; set; }
        public List<Archivo> Adjuntos { get; set; }
        public List<string> sustentatorios { get; set; }
        public List<string> respuestasoluciones { get; set; }
        public List<string> respuestaderivacion { get; set; }
        public List<string> comprobantes { get; set; }
        public string Carta { get; set; }
        public List<Archivo> Enviados { get; set; }
        public string DiasAtencion { get; set; }
        public string UsuarioEnvio { get; set; }
        public string FechaEnvio { get; set; }
        public string Absolucion { get; set; }
        public string tipocierre { get; set; }
        public string referencia { get; set; }
        public string direccioncli { get; set; }
        public string FechaCierre { get; set; }
        public string Usuario { get; set; }
        public decimal TotalPage { get; set; }
        public List<ListTicketResponse> ListTicketResponse { get; set; }
        /*hcama@mg 20210903 ini*/
        public string DiasTiempoAtencion_SLA { get; set; }
        public string DescripcionTipoDiaPlural_SLA { get; set; }
        public string NombreUsuarioRegistraTicket { get; set; }
        /*hcama@mg 20210903 fin*/
        public string Segmento { get; set; }

        //add  20220211
        public string Respuesta { get; set; }
        public string ChannelProcess { get; set; }
        public string ProductoRentas { get; set; }
        public string TramiteRentas { get; set; }
        public string TipoPension { get; set; }
        public string TipoPrestacion { get; set; }
        public string Cussp { get; set; }
        public string NtypeJira { get; set; }

        public string Tramite_Rentas { get; set; }
        public string Tipo_Pension { get; set; }
        public string Tipo_Prestacion { get; set; }
        public string Canal_Ingreso { get; set; }

        //DEV CY - INI
        public string CobranTramite { get; set; }
        public string TramInCant { get; set; }
        public string PayType { get; set; }
        public string Factura { get; set; }
        public string CreditNote { get; set; }
        public string Glosa { get; set; }
        public string TitularCuenta { get; set; }
        public string DocTypeTitularCuenta { get; set; }
        public string NumDocTitularCuenta { get; set; }
        public string Bank { get; set; }
        public string Currency { get; set; }
        public float? Ammount { get; set; }
        public string AccountType { get; set; }
        public string DestAccount { get; set; }
        public string PayCommentary { get; set; }

        public string CommercialGroup { get; set; }
        //DEV CY - FIN

        /* Cantidad de ampliacion de plazo */
        public string SsimpleProduct { get; set; }

        public static implicit operator Ticket(Contract v)
        {
            throw new NotImplementedException();
        }
    }
    public class TicketSGC
    {

        public string Codigo { get; set; }
        public string CodigoJIRA { get; set; }
        public string TramiteType { get; set; }
        public int canal { get; set; }
        public string Fecha { get; set; }
        public string salesChannel { get; set; }
        public int? Tipo { get; set; }
        public string stipo { get; set; }
        public string estado { get; set; }
        public int usuario { get; set; }
        public string Ramo { get; set; }
        public int Producto { get; set; }
        public string Poliza { get; set; }
        public string Productname { get; set; }
        public string Branch { get; set; }
        public string Product { get; set; }
        public string monto { get; set; }
        public string tipoDocClient { get; set; }
        //DEV CY MG 18/05/2022 - INI
        public string Email { get; set; }
        //DEV CY MG 18/05/2022 - FIN
        public string docClient { get; set; }
        public string ContracterType { get; set; }
        public string currencyType { get; set; }
        public string BillType { get; set; }
        public string intermediary { get; set; }
        public string PolicyType { get; set; }
        public int creditRequest { get; set; }
        public int assistanceRequest { get; set; }
        public string marketer { get; set; }
        public string promoter { get; set; }
        public string insurancebank { get; set; }
        public string other { get; set; }
        public string Descripcion { get; set; }
        public string Proyecto { get; set; }
        public string Summary { get; set; }
        public string Reporter { get; set; }
        public string NameClient { get; set; }
        public string documentType { get; set; }
        public string brokerName { get; set; }
        public string intermediaryName { get; set; }
        public string turnover { get; set; }
        public string monthlyPayroll { get; set; }
        public string sumOfInsured { get; set; }
        public string insuredCant { get; set; }
        public List<Archivo> Adjuntos { get; set; }
        public string ContractorDocument { get; set; }
        public string ContractorName { get; set; }
        public string RequestContractor { get; set; }
        public string AssistanceContractor { get; set; }
        public string CommercialGroup { get; set; }
        public string url { get; set; }
        public decimal TotalPage { get; set; }
        public List<ListTicketResponse> ListTicketResponse { get; set; }
        public string Segmento { get; set; }
        public string TotalRows { get; set; }
        public string TotalTra { get; set; }
        public string TotalCot { get; set; }
        public string TotalPD { get; set; }
        //DEV EC - INICIO
        public string TotalSol { get; set; }
        public string processEntryChannel { get; set; }
        public string CobranTramite { get; set; }
        public string TramInCant { get; set; }
        public string Nombre { get; set; }
        public string TipoDocumentoCli { get; set; }
        public string DocumentoCli { get; set; }
        public string Factura { get; set; }
        public string CreditNote { get; set; }
        public string Glosa { get; set; }
        public string TitularCuenta { get; set; }
        public string DocTypeTitularCuenta { get; set; }
        public string NumDocTitularCuenta { get; set; }
        public string Bank { get; set; }
        public string Ammount { get; set; }
        public string AccountType { get; set; }
        public string DestAccount { get; set; }
        public string PayCommentary { get; set; }
        public string Currency { get; set; }
        public string TramiteTecnica { get; set; }
        public int NCODE { get; set; }
        public string SMESSAGE { get; set; }
        public string FacturaOrigen { get; set; }
        //DEV EC - FIN
    }

    //DEV RM - INI
    public class TicketSTC
    {
        //
        public string codigo { get; set; }
        public string code360 { get; set; }
        public string codeJira { get; set; }
        public string codTramiteCobran { get; set; }
        public string desTramiteCobran { get; set; }
        public string recepDate { get; set; }
        public string regDate { get; set; }
        public string codBranch { get; set; }
        public string desBranch { get; set; }
        public string codProduct { get; set; }
        public string desProduct { get; set; }
        public string codTickCanal { get; set; }
        public string desTickCanal { get; set; }
        public string codViaRecep { get; set; }
        public string desViaRecep { get; set; }
        public int numPolicy { get; set; }
        public int typDocument { get; set; }
        public string numDocumento { get; set; }
        public string clientName { get; set; }
        public string clientDir { get; set; }


        //
        public string numTramite { get; set; }
        public string desTramite { get; set; }
        public string codMetPago { get; set; }
        public string desMetPago { get; set; }
        public string invoice { get; set; }
        public string notCredit { get; set; }
        public string glosa { get; set; }
        public string nomTitular { get; set; }
        public string docTitular { get; set; }
        public string desDocTitular { get; set; }
        public string codBanco { get; set; }
        public string desBanco { get; set; }
        public string codMoneda { get; set; }
        public string desMoneda { get; set; }
        public string monto { get; set; }
        public string codCuenta { get; set; }
        public string desCuenta { get; set; }
        public string cuentDestino { get; set; }
        public string comentPago { get; set; }
        public string codCanalIngreso { get; set; }
        public string desCanarIngreso { get; set; } 
        
    }
    //DEV RM - FIN
}
