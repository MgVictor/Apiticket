using apiTicket.Models;
using apiTicket.Models.Reportes;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace apiTicket.Repository.Interfaces
{
    public interface ITicketRepository
    {
        TicketResponse SetTicketS(TicketSiniestro ticket);
        string UpdPlazoTicket(UpdPlazo updPlazo);
        TicketResponse UpdTicket(TicketSiniestro ticket);
        List<Motivo> GetMotivos(int TipoTicket);
        List<SubMotivo> GetSubMotivos(int TipoTicket, int Motivo);
        //DEV EC - INICIO
        List<SubMotivo> GetSubMotivosSol(int TipoTicket, int Motivo);
        //DEV EC - FIN
        List<Client> GetContactos(string tipo, string documento);
        List<Canal> GetCanales();
        List<Via> GetViasRecepcion();
        List<Via> GetViasRespuesta();
        List<Estado> GetEstadosTicket();
        RegiTicketResponse SetTicket(RegTicketRequest request);
        List<Ticket> GetClientTickets(GetTicketRequest request);
        Ticket GetCodigo(string codigo);
        Ticket GetTicket(string codigo);
        string CerrarTicket(Ticket ticket);
        string SetArchivoAdjunto(Archivo archivo);
        Task<string> SetArchivoAdjuntoAsync(Archivo archivo);
        List<Archivo> GetAdjuntos(string codigo, int tipo);
        (string, List<Archivo>) GetAdjuntos2(string codigo, int tipo);
        List<Archivo> GetEnviados(string codigo, int tipo);
        List<Ticket> GetTicketsNoAtendidos();
        Histograma GetHistograma();
        ReporteDona GetDonas();
        Alerta GetAlerta();
        Ticket GetJIRA(string codigo);
        RegiTicketResponse SetReporte(ReporteRequest request);
        List<ReporteRequest> GetReportes();
        List<ReporteRequest> GetReportesTipoBusqueda(string tipoReporte, string datoBuscar, string tipoBusqueda);
        List<RR1> GetRR1(string codigo);
        List<RR3> GetRR3(string codigo);
        List<RR1_ReporteDetalleReclamos360> GetRR1_ReporteDetalleReclamos360(string codigo);
        RegiTicketResponse SetJIRA(Ticket ticket);
        string CreaReporte(ParamReporte request);

        Task<List<TicketSGC>> ListaJIRAAWS(String proy, String codigo, string token);
        RegiTicketResponse RegistraTicketJIRA(TicketDinamico request, string type);
        List<EstructuraTicket> GetEstructura(string Tipo);
        Archivo S3Adjuntar(Archivo adjunto, string type, string customfield = "customfield_12801");
        Task<Archivo> S3AdjuntarAsync(Archivo ar, string type, string customfield = "customfield_12801");
        Task<Ticket> ConsultaTicketJIRA(string codigo, string token, string type);
        Task<string> GetS3Adjunto(string codigo, string type);
        string GenerateReclamo(Ticket ticket);
        string GenerateSolicitud(Ticket ticket);
        TicketSGC SetTicketSGC(TicketSGC request);
        //DEV EC - INICIO
        TicketSGC SetTicketSolSGC(TicketSGC request);
        ResponseViewModel SetPayDataSolSGC(TicketSGC request);
        //DEV EC - FIN
        TicketSGC GetTicketSGC(string codigo);
        List<TicketSGC> ListTicketSGC(SGCRequest request);
        Task<List<TicketSGC>> ListTicketOVPDGSGC(SGCRequest request);
        List<ListBranch> GetBranchList();
        List<Product> GetProducts(string ramo);
        String GetUusuarioJIRA(string usuario);
        String GetCommGruoupUusuario(string usuario);

        int ReaLimitJira();

        //add 20220218
        CommonResponse ValidateTicket(RegTicketRequest request);
        Task<ExcelPackage> ExportGetTicketList(SGCRequest request);
        List<SimpleProduct> GetSimpleProduct();
        List<PartnerType> GetPartnerType();
        List<TipoPrestacion> GetTipoPrestaciones();
        string ValidarTramiteRentas(TramiteRentas request);
        Task<HttpResponseMessage> PutRequest(string baseUrl, string url, object postObject, string token = null);
        Task<GetResponse> GetRequest2(string baseUrl, string url, string token = null, int tipo = 1);
        Task<GetResponse> GetRequest3(string baseUrl, string url, string token = null, int tipo = 1);
        //DEV RM -- INI
        TicketSTC GetTicketSTC(string scode);
        //DEV RM -- FIN
        //DEV DS -- INI
        List<TipoDocumento> GetTipoDocumento();
        List<Banco> GetBanco();
        List<TipoMoneda> GetTipoMoneda();
        List<TipoCuenta> GetTipoCuenta();
        DatosPagoResponse RegistrarDatosPago(DatosPagoRequest request);
        //DEV DS -- FIN
        //DEV CY -- INI
        Ticket GetTicketType(string codigo);
        Ticket GetSTC_Ticket(string codigo);
        ResponseViewModel SetSTCObservation(string codigo); 
        ResponseViewModel UpdateFieldsSTC(DatosPagoRequest model);
        string GetFieldsSTC(DatosPagoRequest model); 
        ResponseViewModel UpdateTicketFile(string codigo);
        //DEV CY -- FIN

        //DEV DS INICIO
        DatosPagoRequest GetInfoToUPDSTC(string scode);
        //DEV DS FIN
        int AfterFecCorte(string scode);
    }
}