using apiTicket.Models;
using apiTicket.Models.Reportes;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace apiTicket.Services
{
    public interface ISolSinService
    {
        string SetArchivosAdjunto(List<Adjunto> adjuntos);
        List<Adjunto> GetAdjuntos(string codigo, string tipo);
        List<Adjunto> GetEnviados(string codigo);
        RegiTicketResponse RegistraTicketJIRA(Contract request, string type);
        List<Adjunto> S3Adjuntar(List<Adjunto> adjuntos, string type);
        Task<string> GetS3Adjunto(string codigo, string type);
        string GenerateSolicitud(Contract contract);
        DatosPagoResponse RegistrarDatosPago(DatosPagoRequest request);
        CommonResponse ValidateTicketNew(RegContractRequest request); 
        RegiTicketResponse SetTicketNew(RegContractRequest request);
        Task<Contract> GetTicketNew(string codigo, string type);
        RegiTicketResponse GestionaRegistroJIRANew(string codigo, string type);
    }
}