using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apiTicket.Models
{
    public class Contract
    {
        public string Codigo { get; set; }
        public string CodigoJIRA { get; set; }
        public string UsuarioEnvio { get; set; }
        public string FechaEnvio { get; set; }
        //----------------------------------------------
        public string summary = ".";
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
        //----------------------------------------------
        public string Motivo { get; set; }
        public string MotivoREC { get; set; }
        public string MotivoSOL { get; set; }
        public string SubMotivo { get; set; }
        public string SubMotivoREC { get; set; }
        public string SubMotivoSOL { get; set; }
        public string Aplicacion { get; set; }
        public string Ramo { get; set; }
        public string Producto { get; set; }
        public string NtypeJira { get; set; }
        public string Monto { get; set; }        
    }
}