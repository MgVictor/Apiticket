using apiTicket.Models;
using apiTicket.Models.Reportes;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace apiTicket.Repository.Interfaces
{
    public interface ISolSinRepository
    {
        string SetArchivoAdjuntoNew(Adjunto adjunto);
        Task<string> SetArchivoAdjuntoAsyncNew(Adjunto adjunto);
        List<Adjunto> GetAdjuntosNew(string codigo, int tipo);
        (string, List<Adjunto>) GetAdjuntos2New(string codigo, int tipo);
        List<Adjunto> GetEnviadosNew(string codigo, int tipo);
        RegiTicketResponse RegistraTicketJIRANew(TicketDinamico request, string type);
        Adjunto S3AdjuntarNew(Adjunto adjunto, string type, string customfield = "customfield_12801");
        Task<Adjunto> S3AdjuntarAsyncNew(Adjunto ar, string type, string customfield = "customfield_12801");
        Task<string> GetS3AdjuntoNew(string codigo, string type);
        string GenerateSolicitudNew(Contract contract);
        DatosPagoResponse RegistrarDatosPagoNew(DatosPagoRequest request);
        List<EstructuraTicket> GetEstructuraNew(string Tipo);
        RegiTicketResponse SetJIRANew(Contract contract);
        CommonResponse ValidateTicketNew(RegContractRequest request);
        RegiTicketResponse SetTicketNew(RegContractRequest request);
        Contract GetTicketNew(string codigo);
        Task<Ticket> ConsultaTicketJIRANew(string codigo, string token, string type);
        Contract GetTicketType(string codigo);
        Contract GetSTC_Ticket(string codigo);
        Contract GetJIRA(string codigo);
    }
}