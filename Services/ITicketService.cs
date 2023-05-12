using apiTicket.Models;
using apiTicket.Models.Reportes;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace apiTicket.Services
{
    public interface ITicketService
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
        List<Policy> GetClientPolicies(string product, string tipoDocumento, string documento);
        Task<List<Ticket>> GetClientTickets(GetTicketRequest request, string type);
        Task<Ticket> GetTicket(string codigo, string type);
        Ticket GetCodigo(string codigo);
        string CerrarTicket(Ticket ticket);
        string SetArchivosAdjunto(List<Archivo> archivos);
        List<Archivo> GetAdjuntos(string codigo, string tipo);
        List<Archivo> GetEnviados(string codigo);
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
        List<Ticket> ListaJIRA(GetTicketRequest request);
        RegiTicketResponse RegistraTicketJIRA(Ticket request, string type);
        RegiTicketResponse GestionaRegistroJIRA(string codigo, string type);
        List<Archivo> S3Adjuntar(List<Archivo> adjuntos, string type);
        Task<Ticket> ConsultaTicketJIRA(string codigo, string token, string type);
        Task<string> GetS3Adjunto(string codigo, string type);
        string GenerateReclamo(Ticket ticket);
        string GenerateSolicitud(Ticket ticket);
        TicketSGC SetTicketSGC(TicketSGC ticket);
        //DEV EC - INI
        TicketSGC SetTicketSolSGC(TicketSGC ticket);
        //DEV EC - FIN
        Task<List<TicketSGC>> ListTicketSGC(SGCRequest request);
        Task<ExcelPackage> ExportGetTicketList(SGCRequest request);
        List<ListBranch> GetBranchList();
        List<Product> GetProducts(string ramo);
        string GetCommGruoupUusuario(string usuario);
        //add 20220218
        CommonResponse ValidateTicket(RegTicketRequest request);

        List<SimpleProduct> GetSimpleProduct();
        List<PartnerType> GetPartnerType();
        List<TipoPrestacion> GetTipoPrestaciones();

        string ValidarTramiteRentas(TramiteRentas request);
        Task<ResponseViewModel> updateStateJira(TicketUpdateJiraPutModel model);
        Task<ResponseViewModel> SetObservation(ResponseObservationModel model);

        //DEV DS --INI
        List<TipoDocumento> GetTipoDocumento();
        List<Banco> GetBanco();
        List<TipoMoneda> GetTipoMoneda();
        List<TipoCuenta> GetTipoCuenta();
        DatosPagoResponse RegistrarDatosPago(DatosPagoRequest request);
        //DEV DS --FIN
        //DEV RM --INI
        TicketSTC GetTicketSTC(string scode);
        //DEV RM --FIN
        //DEV CY --INI
        Task<ResponseViewModel> UpdateInsertFilesSTC(List<Archivo> adjuntos);

        ResponseViewModel UpdateFieldsSTC(DatosPagoRequest model);

        Task<ResponseViewModel> SetSTCObservation(DatosPagoRequest model);

        
        //DEV CY --FIN


        //DEV DS INICIO
        DatosPagoRequest GetInfoToUPDSTC(string scode);
        //DEV DS FINAL
        int AfterFecCorte(string scode);
    }
}