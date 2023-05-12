using System.Collections.Generic;
namespace apiTicket.Models
{
public class ListTicketResponse
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
        public int Productname { get; set; }
        public string Branch { get; set; }
        public string Product { get; set; }
        public string monto { get; set; }
        public string tipoDocClient { get; set; }
        public string docClient { get; set; }
        public string ContracterType { get; set; }
        public string currencyType { get; set; }
        public string Currency { get; set; }
        public string BillType { get; set; }
        public string intermediary { get; set; }
        public string PolicyType { get; set; }
        public int creditRequest { get; set; }
        public int assistanceRequest { get; set; }
        public string marketer { get; set; }
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
    }
}
