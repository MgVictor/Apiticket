using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiTicket.Models
{
    public class RR1_ReporteDetalleReclamos360
    {
        public string PeriodoReporteSucave { get; set; }
        public string NumeroTicket360 { get; set; }
        public string NumeroTicketJira { get; set; }
        public string DNICliente { get; set; }
        public string NombreCompletoCliente { get; set; }
        public string DNIContacto { get; set; }
        public string NombreCompletoContacto { get; set; }
        public string Reconsideracion { get; set; }
        public string DescripcionUbigeoProvincia { get; set; }
        public string DireccionTicket { get; set; }
        public string DescripcionTicket { get; set; }
        public string CodigoProductoSBS { get; set; }
        public string DescripcionProductoSBS { get; set; }
        public string CodigoMotivoSBS { get; set; }
        public string DescripcionMotivoSBS { get; set; }
        public string DescripcionMotivo360 { get; set; }
        public string DescripcionSubMotivo360 { get; set; }
        public string DescripcionCanalSBS { get; set; }
        public string DescripcionMesRecepcion { get; set; }
        public string FechaRecepcionTicket { get; set; }
        public string FechaRegistroTicket { get; set; }
        public string DescripcionEstadoPlazo { get; set; }
        public string NombreUsuarioCierraReclamo { get; set; }
        public string FechaEnvio { get; set; }
        public string DescripcionMesFechaEnvio { get; set; }
        public string NumeroDiasReclamo { get; set; }
        public string DescipcionEstadoAbsolucion { get; set; }
        public string DescipcionViaRespuesta { get; set; }
    }
}
