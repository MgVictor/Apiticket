using apiTicket.Helper;
using apiTicket.Models;
using apiTicket.Services;
using apiTicket.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace apiTicket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly IHostingEnvironment _HostEnvironment;
        private readonly IOptions<AppSettings> appSettings;
        public TicketController(ITicketService TicketService, IHostingEnvironment HostEnvironment, IOptions<AppSettings> appSettings)
        {
            this._ticketService = TicketService;
            _HostEnvironment = HostEnvironment;
            this.appSettings = appSettings;
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "ticket1", "ticket2" };
        }

        [HttpPost("GestionarTicket")]
        public IActionResult GestionarTicket([FromBody] TicketSiniestro request)
        {
            ValidateTicket val = new ValidateTicket();
            TicketResponse response = val.ValidarTicket(request);
            if (!response.respuesta)
            {
                return Ok(response);
            }
            else
            {
                if (request.codTipoOperacion == 1)
                {
                    var _objReturn = this._ticketService.SetTicketS(request);
                    if (_objReturn == null)
                    {
                        return NotFound();
                    }
                    return Ok(_objReturn);
                }
                else
                {
                    var _objReturn = this._ticketService.UpdTicket(request);
                    if (_objReturn == null)
                    {
                        return NotFound();
                    }
                    return Ok(_objReturn);
                }
            }
        }


        [HttpGet("Search/GetMotives/{tipo}")]
        public IActionResult GetMotives(string tipo)
        {
            var tip = 0;
            try
            {
                tip = Int32.Parse(tipo);
            }
            catch (Exception)
            {
                return NotFound();
            }
            var _objReturn = this._ticketService.GetMotivos(tip);
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }

        [HttpGet("Search/GetSubMotives")]
        public IActionResult GeSubtMotives(string tipo, string motivo)
        {
            var tip = 0;
            var mot = 0;
            try
            {
                tip = Int32.Parse(tipo);
                mot = Int32.Parse(motivo);
            }
            catch (Exception)
            {
                return NotFound();
            }
            var _objReturn = this._ticketService.GetSubMotivos(tip, mot);
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }
        
        //DEV EC - INICIO
        [HttpGet("Search/GetSubMotivesSol")]
        public IActionResult GeSubtMotivesSol(string tipo, string motivo)
        {
            var tip = 0;
            var mot = 0;
            try
            {
                tip = Int32.Parse(tipo);
                mot = Int32.Parse(motivo);
            }
            catch (Exception)
            {
                return NotFound();
            }
            var _objReturn = this._ticketService.GetSubMotivosSol(tip, mot);
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }
        //DEV EC - FIN
        
        [HttpGet("Search/GetContactos")]
        public IActionResult GetContactos(string tipo, string documento)
        {
            var _objReturn = this._ticketService.GetContactos(tipo, documento);
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }

        [HttpGet("Search/GetViaRecepcion")]
        public IActionResult GetViaRecepcion()
        {
            var _objReturn = this._ticketService.GetViasRecepcion();
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }

        [HttpGet("Search/GetViaRespuesta")]
        public IActionResult GetViaRespuesta()
        {
            var _objReturn = this._ticketService.GetViasRespuesta();
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }

        [HttpGet("Search/GetEstadosTicket")]
        public IActionResult GetEstadosTicket()
        {
            var _objReturn = this._ticketService.GetEstadosTicket();
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }

        [HttpGet("Search/GetCanalTicket")]
        public IActionResult GetCanalTicket()
        {
            var _objReturn = this._ticketService.GetCanales();
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }

        [HttpPost("Manage/UpdPlazo")]
        public IActionResult UpdPlazo([FromBody] UpdPlazo request)
        {
            //calidad
            var _objReturn = this._ticketService.UpdPlazoTicket(request);
            //     var _objReturn = new RegiTicketResponse();
            if (_objReturn == null)
            {
                return Ok("Se ha actualizado con exito");
            }
            return StatusCode(500, _objReturn);
        }

        [HttpPost("Manage/SetTicket")]
        public IActionResult SetTicket([FromBody] RegTicketRequest request)
        {
            //calidad
            var _objReturn = this._ticketService.SetTicket(request);
            //     var _objReturn = new RegiTicketResponse();
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }

        [HttpPost("Manage/SetTicketSGC")]
        public IActionResult SetTicketSGC([FromBody] TicketSGC request)
        {
            var _objReturn = this._ticketService.SetTicketSGC(request);
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }

        //DEV EC - INICIO
        [HttpPost("Manage/SetTicketSolSGC")]
        public IActionResult SetTicketSolSGC([FromBody] TicketSGC request)
        {
            var _objReturn = this._ticketService.SetTicketSolSGC(request);
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }
        //DEV EC - FIN

        [HttpPost("Search/ListarTicketsSGC")]
        public async Task<ActionResult<List<TicketSGC>>> ListarTicketsSGC(SGCRequest request)//here
        {
            var _objReturn = await this._ticketService.ListTicketSGC(request);
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }

        [HttpPost("Export/ExportTtiketsSGC")]
        public async Task<FileContentResult> ExportTicketList(SGCRequest request)
        {
            try
            {
                ExcelPackage exc = await _ticketService.ExportGetTicketList(request);
                byte[] file = exc.GetAsByteArray();
                return File(file, "application/ms-excel", "lista_tickets.xlsx");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        [HttpPost("Manage/RegistrarTicket")]
        public async Task<IActionResult> RegistrarTicket([FromBody] RegTicketRequest request)
        {

            List<Archivo> archivosAdjuntosTicket = request.Adjuntos;
            RegiTicketResponse _objReturn = new RegiTicketResponse();
            CommonResponse oValidarTicket360 = new CommonResponse();
            ValidateTicket oValidateTicket = new ValidateTicket();
            string codeTicket;
            Ticket ticket;
            string data;
            int tipoTicket = request.Tipo;
            int estadoTicket = request.Estado;
            const int tipoTicket_solicitud = 1;
            const int tipoTicket_tramite = 2;
            const int tipoTicket_consulta = 3;
            const int tipoTicket_reclamo = 4;
            //const int estadoTicket_registrado = 1;

            oValidarTicket360 = oValidateTicket.ValidarTicket360(request);
            if (!oValidarTicket360.respuesta)
            {
                _objReturn.respuesta = oValidarTicket360.respuesta;
                _objReturn.mensaje = "Ocurrio un error al validar los datos del ticket.";
                _objReturn.mensajes = oValidarTicket360.mensajes;
                return Ok(_objReturn);
            }
            oValidarTicket360 = this._ticketService.ValidateTicket(request);
            if (!oValidarTicket360.respuesta)
            {
                _objReturn.respuesta = oValidarTicket360.respuesta;
                _objReturn.mensaje = "Ocurrio un error al validar los datos del ticket.";
                _objReturn.mensajes = oValidarTicket360.mensajes;
                return Ok(_objReturn);
            }

            _objReturn = this._ticketService.SetTicket(request);
            codeTicket = _objReturn.Codigo;

            if (_objReturn.respuesta)
            {

                ticket = await this._ticketService.GetTicket(codeTicket, "SGC");
                if (String.IsNullOrEmpty(ticket.Codigo))
                {
                    _objReturn.respuesta = false;
                    _objReturn.mensaje = "Ocurrio un error al consultar los datos del ticket.";
                    return Ok(_objReturn);
                }

                if (object.ReferenceEquals(null, archivosAdjuntosTicket))
                {
                    archivosAdjuntosTicket = new List<Archivo>();
                }

                List<Archivo> archivosAdjuntosAnt = this._ticketService.GetAdjuntos(codeTicket, "1");

                foreach (Archivo archivo in archivosAdjuntosAnt)
                {
                    archivosAdjuntosTicket.Add(archivo);
                }

                if (tipoTicket == tipoTicket_solicitud || tipoTicket == tipoTicket_reclamo)
                {
                    try
                    {
                        switch (tipoTicket)
                        {
                            case 1:
                                data = this._ticketService.GenerateSolicitud(ticket);
                                archivosAdjuntosTicket.Add(
                                    new Archivo { name = "Solicitud-" + ticket.Codigo + ".pdf", nuser = "1", mime = "application/pdf", scode = ticket.Codigo, size = "2MB", tipo = "1", content = data }
                                );
                                break;
                            case 4:
                                data = this._ticketService.GenerateReclamo(ticket);
                                archivosAdjuntosTicket.Add(
                                    new Archivo { name = "Reclamo-" + ticket.Codigo + ".pdf", nuser = "1", mime = "application/pdf", scode = ticket.Codigo, size = "2MB", tipo = "1", content = data }
                                );
                                break;
                        }
                    }
                    catch (Exception)
                    {
                        _objReturn.respuesta = false;
                        _objReturn.mensaje = "Ocurrio un error generar la  solicitud del ticket";
                        return Ok(_objReturn);

                    }

                }
                foreach (Archivo archivo in archivosAdjuntosTicket)
                {
                    archivo.scode = codeTicket;
                }

                if (archivosAdjuntosTicket.Count != 0)
                {
                    List<Archivo> archivos_s3;
                    try
                    {
                        archivos_s3 = this._ticketService.S3Adjuntar(archivosAdjuntosTicket, "SGC");
                    }
                    catch (Exception)
                    {
                        _objReturn.respuesta = false;
                        _objReturn.mensaje = "Ocurrio un error al adjuntar los archivos  en la nube.";
                        return Ok(_objReturn);
                    }

                    foreach (Archivo archivo in archivos_s3)
                    {
                        archivo.path = archivo.path_gd;
                    }
                    try
                    {
                        this._ticketService.SetArchivosAdjunto(archivos_s3);
                    }
                    catch (Exception)
                    {
                        _objReturn.respuesta = false;
                        _objReturn.mensaje = "Ocurrio un error al guardar los archivos.";
                        return Ok(_objReturn);
                    }
                }
                if (tipoTicket == tipoTicket_solicitud || tipoTicket == tipoTicket_reclamo || (tipoTicket == tipoTicket_tramite && ticket.Estado == "Registrado"))
                {
                    try
                    {
                        this._ticketService.GestionaRegistroJIRA(codeTicket, "SGC");
                    }
                    catch (Exception)
                    {
                        _objReturn.respuesta = false;
                        _objReturn.mensaje = "Ocurrio un error al registrar  en Jira.";
                        return Ok(_objReturn);
                    }
                }
                //if (tipoTicket != tipoTicket_consulta)
                if (tipoTicket == tipoTicket_tramite && ticket.ViaRecepcion != "Net Privada")
                {
                    string contentRootPath = _HostEnvironment.ContentRootPath;
                    string htmlCorreo;
                    string canalTicket = ticket.Canal;
                    string addressFrom;
                    string pwdFrom;
                    string addressTo = ticket.Email;
                    string subject;

                    if (canalTicket == "Clientes")
                    {
                        addressFrom = AppSettings.UsuarioCorreo_CanalTicket_Clientes;
                        pwdFrom = AppSettings.PwdCorreo_CanalTicket_Clientes;
                    }
                    else
                    {
                        addressFrom = AppSettings.UsuarioCorreo_CanalTicket_Corporativo;
                        pwdFrom = AppSettings.PwdCorreo_CanalTicket_Corporativo;
                    }

                    List<Archivo> archivosAdjuntosTicketEC = this._ticketService.GetAdjuntos(codeTicket, "1");
                    List<Archivo> archivosRespuestaAdjuntosTicketEC = this._ticketService.GetAdjuntos(codeTicket, "2");

                    NotifyHelper objNotifyHelper = new NotifyHelper();
                    htmlCorreo = objNotifyHelper.ComposeBodyEmailTicket(contentRootPath, ticket, archivosRespuestaAdjuntosTicketEC.Count, archivosAdjuntosTicketEC.Count);
                    if (string.IsNullOrWhiteSpace(htmlCorreo))
                    {
                        _objReturn.respuesta = false;
                        _objReturn.mensaje = "Ocurrio un error al generar el cuerpo del correo.";
                        return Ok(_objReturn);
                    }
                    subject = objNotifyHelper.ComposeSubjectEmailTicket(ticket);

                    //desarrollo
                    // addressTo = "hernan.cama@materiagris.pe";

                    var envioCorreo = objNotifyHelper.SendMail(addressFrom, pwdFrom, addressTo, subject, htmlCorreo, archivosAdjuntosTicket);
                    if (!envioCorreo)
                    {
                        _objReturn.respuesta = false;
                        _objReturn.mensaje = "Ocurrio un error al enviar el correo.";
                        return Ok(_objReturn);
                    }
                }
            }

            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }
        //DEV CY 11-04-22 INI
        
        [HttpPost("Manage/EmailState")]
        public async Task<IActionResult> EmailState([FromBody] Ticket tickets)//codigo de ticket
        {
            RegiTicketResponse _objReturn = new RegiTicketResponse();
            List<Archivo> archivosAdjuntosTicket = null;
            string contentRootPath = _HostEnvironment.ContentRootPath;
            string htmlCorreo;
            Ticket ticket;
            string canalTicket;
            string addressFrom;
            string pwdFrom;
            //string addressTo = ticket.Email;
            string addressTo;
            string subject;
            string codeTicket = tickets.Codigo;



            List<Archivo> archivosAdjuntosTicketEC = this._ticketService.GetAdjuntos(codeTicket, "1");
            List<Archivo> archivosRespuestaAdjuntosTicketEC = this._ticketService.GetAdjuntos(codeTicket, "2");
            if (archivosAdjuntosTicketEC.Count > 0)
            {
                codeTicket = archivosAdjuntosTicketEC[0].scode;

            }
            else
            {
                codeTicket = this._ticketService.GetCodigo(codeTicket).Codigo;
            }

            ticket = await this._ticketService.GetTicket(codeTicket, "360");
            addressTo = ticket.Email;
            canalTicket = ticket.Canal;
            if (canalTicket == "Clientes")
            {
                addressFrom = AppSettings.UsuarioCorreo_CanalTicket_Clientes;
                pwdFrom = AppSettings.PwdCorreo_CanalTicket_Clientes;
            }
            else
            {
                addressFrom = AppSettings.UsuarioCorreo_CanalTicket_Corporativo;
                pwdFrom = AppSettings.PwdCorreo_CanalTicket_Corporativo;
            }
            /*addressFrom = "diego.salazar@materiagris.pe";
            pwdFrom = "Diego123456";*/
            /*
                        List<Archivo> archivosAdjuntosTicketEC = this._ticketService.GetAdjuntos(codeTicket, "1");
                        List<Archivo> archivosRespuestaAdjuntosTicketEC = this._ticketService.GetAdjuntos(codeTicket, "2");
            */
            if (archivosAdjuntosTicketEC.Count > 0)
            {
                archivosAdjuntosTicket = archivosAdjuntosTicketEC;

                if (archivosRespuestaAdjuntosTicketEC.Count > 0)
                {
                    for (int a = 0; a < archivosRespuestaAdjuntosTicketEC.Count; a++)
                    {
                        archivosAdjuntosTicket.Add(archivosRespuestaAdjuntosTicketEC[a]);
                    }
                }
            }
            else
            {

                if (archivosRespuestaAdjuntosTicketEC.Count > 0)
                {
                    archivosAdjuntosTicket = new List<Archivo>();
                    for (int a = 0; a < archivosRespuestaAdjuntosTicketEC.Count; a++)
                    {
                        archivosAdjuntosTicket.Add(archivosRespuestaAdjuntosTicketEC[a]);
                    }
                }
            }

            int FecCorte = 0;
            try
            {
                FecCorte = _ticketService.AfterFecCorte(codeTicket);
            }
            catch (Exception ex) {
                FecCorte = 0;
            }
            NotifyHelper objNotifyHelper = new NotifyHelper();
            htmlCorreo = objNotifyHelper.ComposeBodyEmailTicket(contentRootPath, ticket, archivosRespuestaAdjuntosTicketEC.Count, archivosAdjuntosTicketEC.Count, FecCorte);
            if (string.IsNullOrWhiteSpace(htmlCorreo))
            {
                _objReturn.respuesta = false;
                _objReturn.mensaje = "Ocurrio un error al generar el cuerpo del correo.";
                return Ok(_objReturn);
            }
            subject = objNotifyHelper.ComposeSubjectEmailTicket(ticket);

            //desarrollo
            //addressTo = "cristopher.yanque@materiagris.pe"; 
            for (int i = 0; i < archivosAdjuntosTicket?.Count; i++)
            {
                archivosAdjuntosTicket[i].content = await this._ticketService.GetS3Adjunto(archivosAdjuntosTicket[i].path_gd, "360");
                //archivosAdjuntosTicket[i].mime = "application/pdf";
            }
            //archivosAdjuntosTicket[0].content = this._ticketService.GetS3Adjunto(codeTicket);
            //SOLO MANDAR CORREO CON PRUEBAS EN WEBHOOK
            //if(addressTo == "DG0202A@GMAIL.COM" || addressTo == "dg0202a@gmail.com")
            //{
                var envioCorreo = objNotifyHelper.SendMail(addressFrom, pwdFrom, addressTo, subject, htmlCorreo, archivosAdjuntosTicket);
                if (!envioCorreo)
                {
                    _objReturn.respuesta = false;
                    _objReturn.mensaje = "Ocurrio un error al enviar el correo.";
                    return Ok(_objReturn);
                }
                if (_objReturn == null)
                {
                    return NotFound();
                }
                return Ok(_objReturn);
            }
        //else
        //{
        //    return Ok(_objReturn);
        //}

        //}
        //DEV CY 11-04-22 FIN

        //DEV DS 01-08-22 INI 
        [HttpPost("Manage/EmailObsTRE")]
        public async Task<IActionResult> EmailObsTRE([FromBody] Ticket tickets)//codigo de ticket
        {
            RegiTicketResponse _objReturn = new RegiTicketResponse();
            string contentRootPath = _HostEnvironment.ContentRootPath;
            string htmlCorreo;
            Ticket ticket;
            string canalTicket;
            string addressFrom;
            string pwdFrom;
            //string addressTo = ticket.Email;
            string addressTo;
            string subject;
            string codeTicket = tickets.Codigo;


            //codeTicket = this._ticketService.GetCodigo(codeTicket).Codigo;

            ticket = await this._ticketService.GetTicket(codeTicket, "360");
            addressTo = ticket.Email;
            canalTicket = ticket.Canal;

            if (canalTicket == "Clientes")
            {
                addressFrom = AppSettings.UsuarioCorreo_CanalTicket_Clientes;
                pwdFrom = AppSettings.PwdCorreo_CanalTicket_Clientes;
            }
            else
            {
                addressFrom = AppSettings.UsuarioCorreo_CanalTicket_Corporativo;
                pwdFrom = AppSettings.PwdCorreo_CanalTicket_Corporativo;
            }
            //pruebas para desarrollo y calidad descomenterar luego

            //addressFrom = "diego.salazar@materiagris.pe";
            //pwdFrom = "Diego123456";

            NotifyHelper objNotifyHelper = new NotifyHelper();
            htmlCorreo = objNotifyHelper.ComposeBodyEmailTicket(contentRootPath, ticket, 0, 0);
            if (string.IsNullOrWhiteSpace(htmlCorreo))
            {
                _objReturn.respuesta = false;
                _objReturn.mensaje = "Ocurrio un error al generar el cuerpo del correo.";
                return Ok(_objReturn);
            }
            subject = objNotifyHelper.ComposeSubjectEmailTicket(ticket);

            //desarrollo
            //addressTo = "cristopher.yanque@materiagris.pe"; 


            //archivosAdjuntosTicket[0].content = this._ticketService.GetS3Adjunto(codeTicket);
            //SOLO MANDAR CORREO CON PRUEBAS EN WEBHOOK
            //if(addressTo == "DG0202A@GMAIL.COM" || addressTo == "dg0202a@gmail.com")
            //{

            List<Archivo> archivosAdjuntosTicket = null;


            var envioCorreo = objNotifyHelper.SendMail(addressFrom, pwdFrom, addressTo, subject, htmlCorreo, archivosAdjuntosTicket);
            if (!envioCorreo)
            {
                _objReturn.respuesta = false;
                _objReturn.mensaje = "Ocurrio un error al enviar el correo.";
                return Ok(_objReturn);
            }
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }
        //DEV DS 01-08-22 FIN

        [HttpPost("Search/GetTickets")]//360
        public async Task<IActionResult> GetTickets([FromBody] GetTicketRequest request)
        {
            try
            {
                var _objReturn = await this._ticketService.GetClientTickets(request, "360");
                if (_objReturn == null)
                {
                    return NotFound();
                }
                return Ok(_objReturn);

            }
            catch (Exception ex)
            {
                ex.HelpLink = ex.HelpLink + "error en GetTickets controller" + ex.Message;
                return Ok(ex);

            }

        }

        [HttpGet("Search/GetTicket/{codigo}")]//360
        public async Task<IActionResult> GetTicket(string codigo)
        {
            var _objReturn = await this._ticketService.GetTicket(codigo, "360");
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }
        
        [HttpGet("Search/GetAdjuntos")]
        public IActionResult GetAdjuntos(string ticket, string tipo)
        {

            var _objReturn = this._ticketService.GetAdjuntos(ticket, tipo);
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }
        //[HttpGet("Manage/CerrarTicket/{codigo}")]
        //public IActionResult CerrarTicket(string codigo)
        //{
        //    var _objReturn = this._ticketService.CerrarTicket(codigo);
        //    if (_objReturn == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(_objReturn);
        //}

        [HttpPost("Manage/CerrarTicket")]
        public IActionResult CerrarTicket([FromBody] Ticket ticket)
        {
            var _objReturn = this._ticketService.CerrarTicket(ticket);
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }

        [HttpPost("Manage/SetArchivosAdjunto")]
        public IActionResult SetArchivosjuntos([FromBody] List<Models.Archivo> request)
        {

            var _objReturn = this._ticketService.SetArchivosAdjunto(request);
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }

        [HttpPost("Manage/RegistrarArchivosAdjunto")]
        public IActionResult RegArchivosjuntos([FromBody] List<Models.Archivo> request)
        {
            foreach (Archivo a in request)
            {
                a.path_gd = a.path;
            }
            var _objReturn = this._ticketService.SetArchivosAdjunto(request);
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }

        [HttpGet("Search/TicketsNoAten")]
        public IActionResult GetNoAtend()
        {
            var _objReturn = this._ticketService.GetTicketsNoAtendidos();
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }

        [HttpGet("Search/GetHistograma")]
        public IActionResult GetHistograma()
        {
            var _objReturn = this._ticketService.GetHistograma();
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }

        [HttpGet("Search/GetAlert")]
        public IActionResult GetAlert()
        {
            var _objReturn = this._ticketService.GetAlerta();
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);

        }

        [HttpGet("Search/GetJIRA/{codigo}")]
        public IActionResult GetJIRA(string codigo)
        {
            var _objReturn = this._ticketService.GetJIRA(codigo);
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }

        [HttpPost("Manage/SetJIRA")]
        public IActionResult SetJIRA([FromBody] Ticket ticket)
        {
            var _objReturn = this._ticketService.SetJIRA(ticket);
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }

        [HttpPost("Manage/SetNormativo")]
        public IActionResult SetNormativo([FromBody] ReporteRequest request)
        {
            var _objReturn = this._ticketService.SetReporte(request);
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }

        [HttpGet("Search/GetNormativos")]
        public IActionResult GetNormativos()
        {
            var _objReturn = this._ticketService.GetReportes();
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }

        [HttpGet("Search/GetNormativos/{tipoReporte}/{tipoBusqueda}/{datoBuscar?}")]
        public IActionResult GetReportesTipoBusqueda(string tipoReporte, string tipoBusqueda, string datoBuscar)
        {
            var _objReturn = this._ticketService.GetReportesTipoBusqueda(tipoReporte, datoBuscar, tipoBusqueda);
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }

        [HttpGet("Search/GetRR1/{codigo}")]
        public IActionResult GetRR1(string codigo)
        {
            var _objReturn = this._ticketService.GetRR1(codigo);
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }
        
        [HttpGet("Search/GetRR3/{codigo}")]
        public IActionResult GetRR3(string codigo)
        {
            var _objReturn = this._ticketService.GetRR3(codigo);
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }

        [HttpGet("Search/GetRR1_ReporteDetalleReclamos360/{codigo}")]
        public IActionResult GetRR1_ReporteDetalleReclamos360(string codigo)
        {
            var _objReturn = this._ticketService.GetRR1_ReporteDetalleReclamos360(codigo);
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }

        [HttpGet("Search/GetDona")]
        public IActionResult GetDona()
        {
            var _objReturn = this._ticketService.GetDonas();
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }

        [HttpPost("Manage/CreaReporte")]
        public IActionResult CreaReporte([FromBody] Models.ParamReporte request)
        {
            var _objReturn = this._ticketService.CreaReporte(request);
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }

        //hcama@mg 30.08.21 ini
        [HttpPost("Search/CreaReporte")]
        public IActionResult Search_CreaReporte([FromBody] Models.ParamReporte request)
        {
            var _objReturn = this._ticketService.CreaReporte(request);
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }
        //hcama@mg 30.08.21 fin
        
        [HttpPost("Search/ListaJIRA")]
        public IActionResult ListaJIRA([FromBody] GetTicketRequest request)
        {
            var _objReturn = this._ticketService.ListaJIRA(request);
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }
        
        [HttpPost("Search/RegistraJIRA")]
        public IActionResult RegistraJIRA([FromBody] Ticket request)
        {
            var _objReturn = this._ticketService.RegistraTicketJIRA(request, "360");
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }
        
        [HttpGet("Manage/RegistraJIRA/{codigo}")] //360
        public IActionResult RegistrarJIRA(string codigo)
        {
            var _objReturn = this._ticketService.GestionaRegistroJIRA(codigo, "360");
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }
        
        [HttpPost("Manage/S3Adjunto")]
        public IActionResult S3Adjunto([FromBody] List<Archivo> Adjuntos)
        {

            var json = this._ticketService.S3Adjuntar(Adjuntos, "360");
            if (json == null)
            {
                return NotFound();
            }
            return Ok(json);
        }
        
        [HttpGet("Manage/ListaJIRA/{codigo}")]
        public async Task<IActionResult> ListaJIRA(string codigo)
        {
            var _objReturn = await this._ticketService.ConsultaTicketJIRA(codigo, "", "360");
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }
        
        [HttpGet("Manage/GetS3Adjunto/{codigo}")] //360
        public async Task<IActionResult> GetS3Adjunto(string codigo)
        {
            var json = await this._ticketService.GetS3Adjunto(codigo, "360");
            if (json == null)
            {
                return NotFound();
            }
            return Ok(json);
        }

        public static void WriteErrorLog(string Message)
        {


            StreamWriter sw = null;


            try
            {
                sw = new StreamWriter(@"D:\LogFile.txt", true);
                sw.WriteLine(DateTime.Now.ToString() + ": " + Message);
                sw.Flush();
                sw.Close();

            }
            catch (Exception e)
            {
                WriteErrorLog(e, "writeerror mess");
            }

        }

        public static void WriteErrorLog(Exception ex, String datos)
        {
            StreamWriter sw = null;

            try
            {
                sw = new StreamWriter(@"D:\LogFile.txt", true);
                sw.WriteLine(DateTime.Now.ToString() + ": " + ex.Source.ToString().Trim() + "; " + ex.Message.ToString().Trim());
                sw.WriteLine("en " + datos);
                sw.Flush();
                sw.Close();
            }
            catch (Exception)
            {
                var es = ex;
            }
        }

        [HttpPost("GetBranchList")]
        public IActionResult GetBranchList()
        {
            var json = this._ticketService.GetBranchList();
            if (json == null)
            {
                return NotFound();
            }
            return Ok(json);
        }
        [HttpGet("GetProduct/{codigo}")]
        public IActionResult GetProduct(string codigo)
        {
            List<Product> prods = new List<Product>();
            ListaProds resp = new ListaProds { status = "", data = prods };

            var js = this._ticketService.GetProducts(codigo);
            resp.data = js;
            var json = resp;
            if (json == null)
            {
                return NotFound();
            }
            return Ok(json);
        }
        [HttpGet("GetCommGroup/{codigo}")]
        public IActionResult GetCommGroup(string codigo)
        {
            var val = this._ticketService.GetCommGruoupUusuario(codigo);
            Name json = new Name();
            json.name = val;
            if (json == null)
            {
                return NotFound();
            }
            return Ok(json);
        }
        [HttpGet("Search/GetPartnerTypeTicket")]
        public IActionResult GetPartnerTypeTicket()
        {
            var _objReturn = this._ticketService.GetPartnerType();
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }

        [HttpGet("Search/GetSimpleProductTicket")]
        public IActionResult GetSimpleProductTicket()
        {
            var _objReturn = this._ticketService.GetSimpleProduct();
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }
        [HttpPost("Manage/ValidarTramiteRentas")]
        public IActionResult ValidarTramiteRentas([FromBody] TramiteRentas request)
        {
            //calidad
            var _objReturn = this._ticketService.ValidarTramiteRentas(request);
            //     var _objReturn = new RegiTicketResponse();
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }
        [HttpGet("Search/GetTipoPrestaciones")]
        public IActionResult GetTipoPrestaciones()
        {
            var _objReturn = this._ticketService.GetTipoPrestaciones();
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }

        [HttpPost("Ticket/updateStatus")]
        public async Task<IActionResult> UpdateStatus(TicketUpdateJiraPutModel model)
        {
            ResponseViewModel res = new ResponseViewModel();

            try
            {
                res = await _ticketService.updateStateJira(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                res.message = ex.Message;
                res.customMessage = ex;
                return Ok(res);
            }
        }

        [HttpPost("Manage/ResponseObs")]
        public async Task<IActionResult> responseObs(ResponseObservationModel model)
        {

            ResponseViewModel res = new ResponseViewModel();
            try
            {
                res = await _ticketService.SetObservation(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                res.message = ex.Message;
                res.code = 2;
                res.customMessage = ex.InnerException;
                return Ok(res);
            }
        }
        //DEV DS -- INI
        [HttpGet("Search/GetTipoDocumento")]
        public IActionResult GetTipoDocumento()
        {
            var _objReturn = this._ticketService.GetTipoDocumento();
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }

        [HttpGet("Search/GetBanco")]
        public IActionResult GetBanco()
        {
            var _objReturn = this._ticketService.GetBanco();
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }

        [HttpGet("Search/GetTipoMoneda")]
        public IActionResult GetTipoMoneda()
        {
            var _objReturn = this._ticketService.GetTipoMoneda();
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }

        [HttpGet("Search/GetTipoCuenta")]
        public IActionResult GetTipoCuenta()
        {
            var _objReturn = this._ticketService.GetTipoCuenta();
            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }

        [HttpPost("Search/RegistrarDatosPago")]
        public IActionResult RegistrarDatosPago([FromBody] DatosPagoRequest request)
        {
            //calidad
            var _objReturn = this._ticketService.RegistrarDatosPago(request);

            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }
        //DEV DS -- FIN

        //DEV RM -- INI
        [HttpGet("Search/GetTicketsStc/{scode}")]
        public IActionResult GetTicketsStc(string scode)
        {
            try
            {
                var _objReturn = this._ticketService.GetTicketSTC(scode);
                if (_objReturn == null)
                {
                    return NotFound();
                }
                return Ok(_objReturn);
            }
            catch (Exception)
            {

                throw;
            }
        }
        //DEV RM -- FIN

        //DEV CY -- INI
        [HttpPost("Manage/UpdateSTCFields")]
        public IActionResult UpdateSTCFields(DatosPagoRequest model)
        {

            ResponseViewModel res = new ResponseViewModel();
            try
            {
                res = _ticketService.UpdateFieldsSTC(model);
                
            }
            catch (Exception ex)
            {
                throw ex;
                //res.customMessage = ex.InnerException;
                //return Ok(res);
            }
            return Ok(res);
        }

        [HttpPost("Manage/UpdateInsertFilesSTC")]
        public async Task<IActionResult> UpdateInsertFilesSTC(List<Archivo> adjuntos)
        {
            ResponseViewModel res = new ResponseViewModel();
            try
            {
                res = await _ticketService.UpdateInsertFilesSTC(adjuntos);

            }
            catch (Exception ex)
            {
                res.customMessage = ex.InnerException;
                return Ok(res);
            }
            return Ok(res);
        }

        [HttpPost("Manage/responseSTCObs")]
        public async Task<IActionResult> responseSTCObs(DatosPagoRequest model)
        {

            ResponseViewModel res = new ResponseViewModel();
            try
            {
                res = await _ticketService.SetSTCObservation(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                res.message = ex.Message;
                res.code = 2;
                res.customMessage = ex.InnerException;
                return Ok(res);
            }
        }  
        //DEV CY -- FIN

        //DEV DS -- INICIO
        [HttpGet("Search/GetInfoToUPDSTC/{scode}")]
        public IActionResult GetInfoToUPDSTC(string scode)
        {
            var _objReturn = this._ticketService.GetInfoToUPDSTC(scode);

            if (_objReturn == null)
            {
                return NotFound();
            }
            return Ok(_objReturn);
        }
        //DEV DS -- FIN
    }
}
