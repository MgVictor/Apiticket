using apiTicket.Models;
using apiTicket.Models.Reportes;
using apiTicket.Repository;
using apiTicket.Repository.Interfaces;
using apiTicket.Utils;
using apiTicket.Utils.Authentication;

using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Globalization;

namespace apiTicket.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        public TicketService(ITicketRepository ticketRepository)
        {
            this._ticketRepository = ticketRepository;
        }
        public string UpdPlazoTicket(UpdPlazo ticket)
        {
            return _ticketRepository.UpdPlazoTicket(ticket);
        }
        public TicketResponse SetTicketS(TicketSiniestro ticket)
        {
            return _ticketRepository.SetTicketS(ticket);
        }
        public TicketResponse UpdTicket(TicketSiniestro ticket)
        {
            return _ticketRepository.UpdTicket(ticket);
        }
        public List<Motivo> GetMotivos(int TipoTicket)
        {
            return this._ticketRepository.GetMotivos(TipoTicket);
        }
        public List<SubMotivo> GetSubMotivos(int TipoTicket, int Motivo)
        {
            return this._ticketRepository.GetSubMotivos(TipoTicket, Motivo);
        }
        //DEV EC - INICIO
        public List<SubMotivo> GetSubMotivosSol(int TipoTicket, int Motivo)
        {
            return this._ticketRepository.GetSubMotivosSol(TipoTicket, Motivo);
        }
        //DEV EC - FIN
        public List<Canal> GetCanales()
        {
            return this._ticketRepository.GetCanales();
        }
        public List<Via> GetViasRecepcion()
        {
            return this._ticketRepository.GetViasRecepcion();
        }
        public List<Via> GetViasRespuesta()
        {
            return this._ticketRepository.GetViasRespuesta();
        }
        public List<Estado> GetEstadosTicket()
        {
            return this._ticketRepository.GetEstadosTicket();
        }

        public List<Client> GetContactos(string tipo, string documento)
        {
            return this._ticketRepository.GetContactos(tipo, documento);
        }


        public RegiTicketResponse SetTicket(RegTicketRequest request)
        {
            RegiTicketResponse response = this._ticketRepository.SetTicket(request);
            return response;
        }
        public List<Policy> GetClientPolicies(string product, string tipoDocumento, string documento)
        {
            throw new NotImplementedException();
        }
        public async Task<List<Ticket>> GetClientTickets(GetTicketRequest request, string type)
        {
            List<Ticket> tickets = this._ticketRepository.GetClientTickets(request);
            try
            {
                string token = new TokenService().getTokenAWS(type);
                foreach (Ticket t in tickets)
                {
                    if (t.CodigoJIRA != null && t.CodigoJIRA != "" && t.Estado != "Cerrado 360")
                    {
                        Ticket jira = await this._ticketRepository.ConsultaTicketJIRA(t.CodigoJIRA, token, type);
                        t.Estado = jira.Estado;
                        t.Absolucion = jira.Absolucion;
                        t.sustentatorios = jira.sustentatorios;
                        t.respuestaderivacion = jira.respuestaderivacion;
                        t.respuestasoluciones = jira.respuestasoluciones;
                        t.comprobantes = jira.comprobantes;
                        t.FechaCierre = jira.FechaCierre;
                    }
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return tickets;

        }

        public Ticket GetCodigo(string codigo)
        {
            Ticket ticket = new Ticket();
            ticket = this._ticketRepository.GetCodigo(codigo);
            return ticket;
        }
        public async Task<Ticket> GetTicket(string codigo, string type)
        {
            Ticket ticket = new Ticket();
            string token = new TokenService().getTokenAWS(type);
            ticket = this._ticketRepository.GetTicket(codigo);
            if (ticket.CodigoJIRA != "" && ticket.CodigoJIRA != null)
            {
                Ticket jira = await this._ticketRepository.ConsultaTicketJIRA(ticket.CodigoJIRA, token, type);
                if (jira.comprobantes != null) { ticket.comprobantes = jira.comprobantes; }
                if (jira.sustentatorios != null) { ticket.sustentatorios = jira.sustentatorios; }
                if (jira.respuestaderivacion != null) { ticket.respuestaderivacion = jira.respuestaderivacion; }
                if (jira.respuestasoluciones != null) { ticket.respuestasoluciones = jira.respuestasoluciones; }
                if (jira.FechaCierre != null) { ticket.FechaCierre = jira.FechaCierre; }
                if (ticket.Estado != "Cerrado 360")
                {
                    ticket.Estado = jira.Estado;
                    ticket.Absolucion = jira.Absolucion;
                }
                else
                {
                    ticket.Enviados = this._ticketRepository.GetEnviados(codigo, 2);
                    ticket.Absolucion = jira.Absolucion;
                }
                ticket.Carta = jira.Carta;
            }
            else
            {
                if (ticket.Estado == "Cerrado 360")
                {
                    ticket.Enviados = this._ticketRepository.GetEnviados(codigo, 2);
                }
            }
            return ticket;
        }
        public string CerrarTicket(Ticket ticket)
        {
            foreach (Archivo arch in ticket.Enviados)
            {
                arch.scode = ticket.Codigo;
                arch.nuser = "1";
                arch.size = "1 MB";
            }
            SetArchivosAdjunto(ticket.Enviados);
            return this._ticketRepository.CerrarTicket(ticket);
        }
        public string SetArchivosAdjunto(List<Archivo> archivos)
        {
            string respuesta = string.Empty;
            bool control = true;
            foreach (Archivo arch in archivos)
            {
                var r = this._ticketRepository.SetArchivoAdjunto(arch);
                if (string.IsNullOrEmpty(r))
                {
                    control = false;
                }
                respuesta = r;
            }
            if (!control)
            {
                return string.Empty;
            }
            return respuesta;
        }
        public List<Archivo> GetAdjuntos(string codigo, string tipo)
        {
            return this._ticketRepository.GetAdjuntos(codigo, int.Parse(tipo));
        }
        public List<Archivo> GetEnviados(string codigo)
        {
            return this._ticketRepository.GetAdjuntos(codigo, 2);
        }
        public List<Ticket> GetTicketsNoAtendidos()
        {
            return this._ticketRepository.GetTicketsNoAtendidos();
        }
        public Histograma GetHistograma()
        {
            return this._ticketRepository.GetHistograma();
        }
        public Alerta GetAlerta()
        {
            return this._ticketRepository.GetAlerta();
        }
        public Ticket GetJIRA(string codigo)
        {
            Ticket ticket = new Ticket();
            ticket = this._ticketRepository.GetJIRA(codigo);
            ticket.Adjuntos = new List<Archivo>();
            ticket.Adjuntos = this._ticketRepository.GetAdjuntos(codigo, 1);
            ticket.FecRecepcion = ticket.FecRecepcion?.Substring(0, 10);
            ticket.FecRegistro = ticket.FecRegistro?.Substring(0, 10);
            return ticket;
        }
        public RegiTicketResponse SetReporte(ReporteRequest request)
        {
            RegiTicketResponse response = new RegiTicketResponse();
            response = this._ticketRepository.SetReporte(request);
            if (response.Codigo != "1")
            {
                string ini = "";
                string fin = "";
                string year_ant = "";
                string mesini_ant = "";
                string mesfin_ant = "";
                string tipoReporte = request.tipo;
                year_ant = request.anio;

                switch (request.trimestre)
                {
                    case "1":
                        ini = "1";
                        fin = "3";
                        year_ant = (Convert.ToInt32(request.anio) - 1).ToString();
                        mesini_ant = "10";
                        mesfin_ant = "12";
                        break;
                    case "2":
                        ini = "4";
                        fin = "6";
                        mesini_ant = "1";
                        mesfin_ant = "3";
                        break;
                    case "3":
                        ini = "7";
                        fin = "9";
                        mesini_ant = "4";
                        mesfin_ant = "6";
                        break;
                    case "4":
                        ini = "10";
                        fin = "12";
                        mesini_ant = "7";
                        mesfin_ant = "9";
                        break;
                }


                string rp = this._ticketRepository.CreaReporte(new ParamReporte
                {
                    year = request.anio,
                    mesini = ini,
                    mesfin = fin,
                    year_ant = year_ant,
                    mesini_ant = mesini_ant,
                    mesfin_ant = mesfin_ant,
                    scode = response.Codigo,
                    tipoReporte = tipoReporte
                });
                //se envio dos veces la ceracion del detalle del reporte
                //hcama@mg 30.08.21 ini 
                response.respuesta = true;
                // hcama@mg 30.08.21  fin


            }
            return response;
        }
        public List<ReporteRequest> GetReportes()
        {
            List<ReporteRequest> response = new List<ReporteRequest>();
            response = this._ticketRepository.GetReportes();
            return response;
        }
        public List<ReporteRequest> GetReportesTipoBusqueda(string tipoReporte, string datoBuscar, string tipoBusqueda)
        {
            List<ReporteRequest> response = new List<ReporteRequest>();
            response = this._ticketRepository.GetReportesTipoBusqueda(tipoReporte, datoBuscar, tipoBusqueda);
            return response;
        }
        public List<RR1> GetRR1(string codigo)
        {
            List<RR1> response = new List<RR1>();
            response = this._ticketRepository.GetRR1(codigo);
            return response;
        }
        public List<RR3> GetRR3(string codigo)
        {
            List<RR3> response = new List<RR3>();
            response = this._ticketRepository.GetRR3(codigo);
            return response;
        }
        public List<RR1_ReporteDetalleReclamos360> GetRR1_ReporteDetalleReclamos360(string codigo)
        {
            List<RR1_ReporteDetalleReclamos360> response = new List<RR1_ReporteDetalleReclamos360>();
            response = this._ticketRepository.GetRR1_ReporteDetalleReclamos360(codigo);
            return response;
        }
        public RegiTicketResponse SetJIRA(Ticket ticket)
        {
            RegiTicketResponse response = new RegiTicketResponse();
            response = this._ticketRepository.SetJIRA(ticket);
            return response;
        }

        string ITicketService.CreaReporte(ParamReporte request)
        {
            string response = this._ticketRepository.CreaReporte(request);
            return response;
        }

        ReporteDona ITicketService.GetDonas()
        {
            ReporteDona response = new ReporteDona();
            response = this._ticketRepository.GetDonas();
            return response;
        }

        public List<Ticket> ListaJIRA(GetTicketRequest request)
        {
            return new List<Ticket>();
        }
        public RegiTicketResponse GestionaRegistroJIRA(string codigo, string type)
        {
            RegiTicketResponse response = new RegiTicketResponse();
            //DEV CY -- INI
            Ticket request = new Ticket();
            Ticket ticket = GetTicketType(codigo);
            if (ticket.Codigo == "TRA" && ticket.NtypeJira == "55") 
            {
                request = GetSTC_Ticket(codigo);
            }
            else
            {
                //Ticket request = GetJIRA(codigo);
                request = GetJIRA(codigo);
            }
            //DEV CY -- FIN
            RegistraTicketJIRA(request, type);

            return response;
        }
        public RegiTicketResponse RegistraTicketJIRA(Ticket request, string type)
        {
            string tipo = request.Codigo?.Substring(0, 3);
            var tip = string.Empty;
            if (tipo == "REC")
            {
                tip = "4";
                request.Motivo = request.MotivoREC;
                request.SubMotivo = request.SubMotivoREC;
            }
            else if (tipo == "SOL")
            {
                tip = "1";
                request.Motivo = request.MotivoSOL;
                request.SubMotivo = request.SubMotivoSOL;
            }
            else if (tipo == "TRA")
            {
                if (request.NtypeJira == "56") 
                {
                    tip = "81";
                }
                //DEV CY -- INI
                else if (request.NtypeJira == "55") 
                {

                    tip = "102";
                }
                //DEV CY -- FIN
                else
                {
                    tip = "2";
                }
            }

            List<EstructuraTicket> estructuras = this._ticketRepository.GetEstructura(tip);
            TicketDinamico dinamico = new TicketDinamico();
            dinamico.system = "cliente360";
            dinamico.fields = new List<Field>();
            dinamico.attachments = new List<Attachment>();
            foreach (EstructuraTicket es in estructuras)
            {
                if (es.STYPE == "adjuntos")
                {
                    Attachment att = new Attachment();
                    att.id = es.SCODE_JIRA;
                    att.value = new List<string>();
                    if (request.Adjuntos != null)//SOLO CUANDO TENGA ADJUNTOS //DEV CY -- INI
                    {
                        foreach (Archivo arch in request.Adjuntos)
                        {
                            att.value.Add(arch.path_gd);
                        }
                        dinamico.attachments.Add(att);
                    } //SOLO CUANDO TENGA ADJUNTOS //DEV CY -- FIN
                }
                else
                {
                    Field campo = AgregarCampo(es, request); 
                    if(request.NtypeJira == "55" && request.Monto== null && es.SCODE_JIRA == "customfield_11314")
                    {
                        campo.value = 0;
                    }
                    else
                    {
                        dinamico.fields.Add(campo);
                    }
                    //dinamico.fields.Add(campo);
                }
            }
            string jsonString = string.Empty;
            jsonString = JsonSerializer.Serialize(dinamico);

            RegiTicketResponse response = this._ticketRepository.RegistraTicketJIRA(dinamico, type);
            RegiTicketResponse final = this._ticketRepository.SetJIRA(new Ticket { Codigo = request.Codigo, CodigoJIRA = response.Codigo, Aplicacion = "360" });
            return final;

        }
        public Field AgregarCampo(EstructuraTicket est, Ticket ticket)
        {
            object value = null;

            if (est.STYPE == "summary" || est.STYPE == "string" || est.STYPE == "description")
            {
                value = ticket.GetType().GetProperty(est.SDESCRIPT).GetValue(ticket).ToString();
            }
            if (est.STYPE == "number")
            {
                value = ticket.GetType().GetProperty(est.SDESCRIPT).GetValue(ticket);
            }
            if (est.STYPE == "datetime")
            {
                string conver = "";
                var fecha = ticket.GetType().GetProperty(est.SDESCRIPT).GetValue(ticket);
                if (fecha != null)
                {
                    string[] allowedFormats = { "d/MM/yyyy", "dd/MM/yyyy", "d/M/yyyy", "d/M/yyyy hh:mm:ss tt", "dd/MM/yyyy hh:mm:ss tt", "MM/dd/yyyy hh:mm:ss tt", "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm:ss.fffK", "yyyy-MM-ddTHH:mm:ssK" };
                    if (System.DateTime.TryParseExact(fecha.ToString().Trim(), allowedFormats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime date))
                    {
                        //conver = date.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz");// 2022-04-21T05:00:00.000+0000
                        conver = date.ToString("yyyy-MM-dd");
                        //value = conver; //+ "T05:00:00.000+0000";
                    }
                    else
                    { 
                        try
                        {
                            conver = DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(fecha.ToString())).DateTime.ToString("MM/dd/yyyy HH:mm:ss");
                        }
                        catch (Exception ex) {
                            conver = Convert.ToDateTime(fecha).ToString("MM/dd/yyyy HH:mm:ss");
                        } 
                        
                    }

                }
                //string conver = DateTime.Parse(fecha.ToString()).ToString("yyyy-MM-dd");
                value = conver + "T05:00:00.000+0000";
            }
            if (est.STYPE == "issuetype" || est.STYPE == "option")
            {
                var valor = ticket.GetType().GetProperty(est.SDESCRIPT).GetValue(ticket).ToString();
                value = new ID { id = valor };
            }
            if (est.STYPE == "project")
            {
                var valor = ticket.GetType().GetProperty(est.SDESCRIPT).GetValue(ticket).ToString();
                value = new Key { key = valor };
            }
            if (est.STYPE == "user" || est.STYPE == "reporter")
            {
                var valor = ticket.GetType().GetProperty(est.SDESCRIPT).GetValue(ticket).ToString();
                value = new Name { name = valor };
            }
            if (est.STYPE == "array")
            {
                var valor = ticket.GetType().GetProperty(est.SDESCRIPT).GetValue(ticket).ToString();
                //  value = new List<ID> { new ID { id = valor } }; 
                List<ID> lista = new List<ID>();
                string[] valores = valor.Split(',');
                foreach (string v in valores)
                {
                    ID i = new ID();
                    i.id = v;
                    lista.Add(i);
                }
                value = lista;
            }
            if (est.STYPE == "option-with-child")
            {
                var valor = ticket.GetType().GetProperty(est.SDESCRIPT).GetValue(ticket).ToString();
                value = new Child { id = ticket.Motivo, child = new ID { id = ticket.SubMotivo } };
            }
            if (est.STYPE == "option-with-child2")
            {
                var valor = ticket.GetType().GetProperty(est.SDESCRIPT).GetValue(ticket).ToString();
                value = new Child { id = ticket.Ramo, child = new ID { id = ticket.Producto } };
            }

            Field field = new Field { id = est.SCODE_JIRA, value = value };
            return field;
        }
        public Field AgregarCampo(EstructuraTicket est, TicketSGC ticket)
        {
            object value = null;

            if (est.STYPE == "summary" || est.STYPE == "string" || est.STYPE == "description")
            {
                value = ticket.GetType().GetProperty(est.SDESCRIPT).GetValue(ticket).ToString();
            }
            if (est.STYPE == "datetime")
            {
                var fecha = ticket.GetType().GetProperty(est.SDESCRIPT).GetValue(ticket);
                string conver = DateTime.Parse(fecha.ToString()).ToString("yyyy-MM-dd");
                value = conver + "T05:00:00.000+0000";
            }
            if (est.STYPE == "issuetype" || est.STYPE == "option")
            {
                var valor = ticket.GetType().GetProperty(est.SDESCRIPT).GetValue(ticket).ToString();
                value = new ID { id = valor };
            }
            if (est.STYPE == "project")
            {
                var valor = ticket.GetType().GetProperty(est.SDESCRIPT).GetValue(ticket).ToString();
                value = new Key { key = valor };
            }
            if (est.STYPE == "user" || est.STYPE == "reporter")
            {
                var valor = ticket.GetType().GetProperty(est.SDESCRIPT).GetValue(ticket).ToString();
                value = new Name { name = valor };
            }
            if (est.STYPE == "array")
            {
                var valor = ticket.GetType().GetProperty(est.SDESCRIPT).GetValue(ticket).ToString();
                value = new List<ID> { new ID { id = valor } };
            }

            if (est.STYPE == "option-with-child2")
            {
                var valor = ticket.GetType().GetProperty(est.SDESCRIPT).GetValue(ticket).ToString();
                value = new Child { id = ticket.Branch, child = new ID { id = ticket.Product } };
            }

            Field field = new Field { id = est.SCODE_JIRA, value = value };
            return field;
        }

        public List<Archivo> S3Adjuntar(List<Archivo> adjuntos, string type)
        {
            List<Archivo> grabados = new List<Archivo>();
            foreach (Archivo ar in adjuntos)
            {
                Archivo arch = this._ticketRepository.S3Adjuntar(ar, type);
            }
            return adjuntos;
        }
        public async Task<string> GetS3Adjunto(string codigo, string type)
        {
            return await this._ticketRepository.GetS3Adjunto(codigo, type);
        }
        public async Task<Ticket> ConsultaTicketJIRA(string codigo, string token, string type)
        {
            Ticket ticket = await _ticketRepository.ConsultaTicketJIRA(codigo, token, type);
            return ticket;
        }
        public string GenerateReclamo(Ticket ticket)
        {
            return _ticketRepository.GenerateReclamo(ticket);
        }
        public string GenerateSolicitud(Ticket ticket)
        {
            return _ticketRepository.GenerateSolicitud(ticket);
        }
        public TicketSGC SetTicketSGC(TicketSGC ticket)
        {
            List<EstructuraTicket> estructuras = this._ticketRepository.GetEstructura(ticket.Tipo.ToString());
            List<Archivo> archs = new List<Archivo>();
            foreach (Archivo arch in ticket.Adjuntos)
            {
                Archivo file = new Archivo();
                string cadena = arch.content;
                string[] arr = cadena.Split("base64,");
                if (arr.Length > 1)
                {
                    arch.content = arr[1];
                }
                file = this._ticketRepository.S3Adjuntar(arch, "SGC");
                archs.Add(file);
            }
            ticket.Adjuntos = archs;
            TicketSGC tkt = _ticketRepository.SetTicketSGC(ticket);
            var comm = ticket.CommercialGroup.Split(";");
            ticket.CommercialGroup = comm[1];
            ticket.Codigo = tkt.Codigo;
            ticket.Branch = ticket.Ramo;
            ticket.Product = ticket.Producto.ToString();
            if (ticket.Tipo == 101)
            {
                ticket.Tipo = 10806;
                ticket.Proyecto = "GCS";
            }
            else
            {

                ticket.Tipo = 11000;
                ticket.Proyecto = "OV";
            }
            if (ticket.creditRequest == 1)
            {
                ticket.RequestContractor = "12527";
            }
            else
            {
                ticket.RequestContractor = "12528";
            }
            if (ticket.assistanceRequest == 1)
            {
                ticket.AssistanceContractor = "12549";
            }
            else
            {
                ticket.AssistanceContractor = "12550";
            }
            ticket.ContractorDocument = ticket.docClient;
            ticket.documentType = ticket.docClient.Length > 8 ? "12555" : "12554";
            ticket.Currency = ticket.currencyType;
            ticket.ContractorName = ticket.NameClient == null ? "" : ticket.NameClient;
            ticket.Ramo = "12682";
            ticket.Producto = 12686;
            ticket.Reporter = this._ticketRepository.GetUusuarioJIRA(ticket.usuario.ToString());
            TicketDinamico dinamico = new TicketDinamico();
            dinamico.system = "sgc";
            ticket.Summary = ticket.Productname + "/" + ticket.ContractorName + "-" + ticket.marketer + "-" + ticket.intermediary;
            dinamico.fields = new List<Field>();
            dinamico.attachments = new List<Attachment>();

            foreach (EstructuraTicket es in estructuras)
            {
                if (es.STYPE == "adjuntos")
                {
                    Attachment att = new Attachment();
                    att.id = es.SCODE_JIRA;
                    att.value = new List<string>();
                    foreach (Archivo arch in ticket.Adjuntos)
                    {
                        att.value.Add(arch.path_gd);
                    }
                    dinamico.attachments.Add(att);
                }
                else
                {
                    Field campo = AgregarCampo(es, ticket);
                    dinamico.fields.Add(campo);
                }
            }
            string jsonString = string.Empty;
            jsonString = JsonSerializer.Serialize(dinamico);
            //string sdf = "";

            // Descomentar GCS
            // RegiTicketResponse response = this._ticketRepository.RegistraTicketJIRA(dinamico, "SGC");
            // RegiTicketResponse final = this._ticketRepository.SetJIRA(new Ticket { Codigo = ticket.Codigo, CodigoJIRA = response.Codigo, Aplicacion = "SGC" });
            // ticket.CodigoJIRA = response.Codigo;
            return ticket;
        }
        //DEV EC - INICIO
        public TicketSGC SetTicketSolSGC(TicketSGC ticket)
        {
            ResponseViewModel resPayData = new ResponseViewModel();
            List<EstructuraTicket> estructuras = this._ticketRepository.GetEstructura(ticket.Tipo.ToString());
            List<Archivo> archs = new List<Archivo>();
            foreach (Archivo arch in ticket.Adjuntos)
            {
                Archivo file = new Archivo();
                string cadena = arch.content;
                string[] arr = cadena.Split("base64,");
                if (arr.Length > 1)
                {
                    arch.content = arr[1];
                }
                file = this._ticketRepository.S3Adjuntar(arch, "SGC");
                archs.Add(file);
            }
            ticket.Adjuntos = archs;
            ticket.Reporter = this._ticketRepository.GetUusuarioJIRA(ticket.usuario.ToString());
            TicketSGC tkt = _ticketRepository.SetTicketSolSGC(ticket);
            var comm = ticket.CommercialGroup.Split(";");
            ticket.CommercialGroup = comm[0];
            ticket.Codigo = tkt.Codigo;
            if (ticket.CobranTramite == "15643" || ticket.CobranTramite == "15835")
            {
                resPayData = _ticketRepository.SetPayDataSolSGC(ticket);

            }

            ticket.NCODE = resPayData.code;
            ticket.SMESSAGE = resPayData.message;
            ticket.Branch = ticket.Ramo;
            ticket.Product = ticket.Producto.ToString();
            if (ticket.Tipo == 101)
            {
                ticket.Tipo = 10806;
                ticket.Proyecto = "GCS";
            }
            else if (ticket.Tipo == 102)
            {

                ticket.Tipo = 11002;
                ticket.Proyecto = "STC";
            }
            else if (ticket.Tipo == 103)
            {

                ticket.Tipo = 11002;
                ticket.Proyecto = "SGT";
            }
            else
            {
                ticket.Tipo = 11000;
                ticket.Proyecto = "OV";
            }
            if (ticket.CommercialGroup == "1")
            {
                ticket.CommercialGroup = "12543";
            }
            else if (ticket.CommercialGroup == "2")
            {
                ticket.CommercialGroup = "12553";
            }
            else if (ticket.CommercialGroup == "3")
            {
                ticket.CommercialGroup = "12542";
            }
            else if (ticket.CommercialGroup == "4")
            {
                ticket.CommercialGroup = "12541";
            }
            else
            { }
            //if (ticket.creditRequest == 1)
            //{
            //    ticket.RequestContractor = "12527";
            //}
            //else
            //{
            //    ticket.RequestContractor = "12528";
            //}
            //if (ticket.assistanceRequest == 1)
            //{
            //    ticket.AssistanceContractor = "12549";
            //}
            //else
            //{
            //    ticket.AssistanceContractor = "12550";
            //}
            //ticket.ContractorDocument = ticket.DocumentoCli;
            ticket.documentType = ticket.DocumentoCli.Length > 8 ? "12555" : "12554";
            //ticket.Currency = ticket.currencyType;
            //ticket.ContractorName = ticket.NameClient == null ? "" : ticket.NameClient;
            ticket.Ramo = "12682";
            ticket.Producto = 12686;
            //ticket.Reporter = this._ticketRepository.GetUusuarioJIRA(ticket.usuario.ToString());
            TicketDinamico dinamico = new TicketDinamico();
            dinamico.system = "sgc";
            //ticket.Summary = ticket.Productname + "/" + ticket.ContractorName + "-" + ticket.marketer + "-" + ticket.intermediary;
            dinamico.fields = new List<Field>();
            dinamico.attachments = new List<Attachment>();

            foreach (EstructuraTicket es in estructuras)
            {
                if (es.STYPE == "adjuntos")
                {
                    Attachment att = new Attachment();
                    att.id = es.SCODE_JIRA;
                    att.value = new List<string>();
                    foreach (Archivo arch in ticket.Adjuntos)
                    {
                        att.value.Add(arch.path_gd);
                    }
                    dinamico.attachments.Add(att);
                }
                else
                {
                    Field campo = AgregarCampo(es, ticket);
                    dinamico.fields.Add(campo);
                }
            }
            string jsonString = string.Empty;
            jsonString = JsonSerializer.Serialize(dinamico);
            //string sdf = "";

            // DESCOMENTAR AL FINAL DE LA PRUEBAS
            // RegiTicketResponse response = this._ticketRepository.RegistraTicketJIRA(dinamico, "SGC");
            //RegiTicketResponse final = this._ticketRepository.SetJIRA(new Ticket { Codigo = ticket.Codigo, CodigoJIRA = response.Codigo, Aplicacion = "SGC" });
           // ticket.CodigoJIRA = response.Codigo;
            return ticket;
        }
        //DEV EC - FIN
        public async Task<List<TicketSGC>> ListTicketSGC(SGCRequest request)
        {
            List<TicketSGC> tickets = new List<TicketSGC>();
            //List<TicketSGC>  tickets = _ticketRepository.ListTicketSGC(request);
            //int limite = 0;
            try
            {
                //limite = _ticketRepository.ReaLimitJira();
                tickets = await _ticketRepository.ListTicketOVPDGSGC(request);
                //Util util = new Util();
                // tickets.GroupBy(x => x.stipo);
                //Consulta OV
                //var query = tickets.Where(a => a.stipo == "Cotizacion");

                //if (tickets.Count > 0)
                //{
                //    TicketSGC ticketCot = new TicketSGC();
                //    TicketSGC ticketTot = new TicketSGC();
                //    TicketSGC ticketTra = new TicketSGC();
                //    TicketSGC ticketPD = new TicketSGC();
                //    ticketCot = tickets.Where(a => Convert.ToInt32(a.TotalCot) > 0).FirstOrDefault();
                //    ticketTot = tickets.Where(a => Convert.ToInt32(a.TotalRows) > 0).FirstOrDefault();
                //    ticketTra = tickets.Where(a => Convert.ToInt32(a.TotalTra) > 0).FirstOrDefault();
                //    ticketPD = tickets.Where(a => Convert.ToInt32(a.TotalPD) > 0).FirstOrDefault();
                //    tickets[0].TotalRows = ticketTot == null ? "0" : ticketTot.TotalRows;
                //    tickets[0].TotalTra = ticketTra == null ? "0" : ticketTra.TotalTra;
                //    tickets[0].TotalCot = ticketCot == null ? "0" : ticketCot.TotalCot;
                //    tickets[0].TotalPD = ticketPD == null ? "0" : ticketPD.TotalPD;

                //}

                // List<TicketSGC> tickOV = query.ToList();
                //List<TicketSGC> tiks = new List<TicketSGC>();
                //string token = new TokenService().getTokenAWS("SGC");
                //if (tickOV.Count > 0)
                //{
                //    //List<List<TicketSGC>> listaOV = util.SplitList(tickOV, 5);
                //    List<List<TicketSGC>> listaOV = util.SplitList(tickOV, limite);
                //    foreach (List<TicketSGC> s in listaOV)
                //    {
                //        string codigo = string.Join(", ", s.Select(z => z.CodigoJIRA));
                //        tiks.AddRange(await this._ticketRepository.ListaJIRAAWS("OV", codigo, token));
                //    }
                //}
                //Consulta GCS
                //var queryq = tickets.Where(a => a.stipo == "Tramite");
                //List<TicketSGC> tickGCS = queryq.ToList();
                //List<TicketSGC> tiks2 = new List<TicketSGC>();
                //if (tickGCS.Count > 0)
                //{

                //    //List<List<TicketSGC>> listaG = util.SplitList(tickGCS, 5);
                //    List<List<TicketSGC>> listaG = util.SplitList(tickGCS, limite);
                //    foreach (List<TicketSGC> s in listaG)
                //    {
                //        string codigo = string.Join(", ", s.Select(z => z.CodigoJIRA));
                //        tiks2.AddRange(await this._ticketRepository.ListaJIRAAWS("GCS", codigo, token));
                //    }
                //}

                // List<TicketSGC> total = new List<TicketSGC>();
            }
            catch (Exception ex)
            {
                throw ex;

            }

            //if (tiks.Count > 0) { total.AddRange(tiks); }
            //if (tiks2.Count > 0) { total.AddRange(tiks2); }
            //foreach (TicketSGC x in total)
            //{
            //    tickets.First(d => d.CodigoJIRA == x.CodigoJIRA).estado = x.estado;
            //}

            return tickets;
        }
        public List<ListBranch> GetBranchList()
        {
            return this._ticketRepository.GetBranchList();
        }
        public List<Product> GetProducts(string ramo)
        {

            return this._ticketRepository.GetProducts(ramo);
        }
        public string GetCommGruoupUusuario(string usuario)
        {
            return this._ticketRepository.GetCommGruoupUusuario(usuario);
        }
        //add 20220218
        public CommonResponse ValidateTicket(RegTicketRequest request)
        {
            return this._ticketRepository.ValidateTicket(request);
        }

        public async Task<ExcelPackage> ExportGetTicketList(SGCRequest request)
        {

            return await this._ticketRepository.ExportGetTicketList(request);
        }
        public List<SimpleProduct> GetSimpleProduct()
        {
            List<SimpleProduct> response = new List<SimpleProduct>();
            response = this._ticketRepository.GetSimpleProduct();
            return response;
        }


        public List<PartnerType> GetPartnerType()
        {
            List<PartnerType> response = new List<PartnerType>();
            response = this._ticketRepository.GetPartnerType();
            return response;
        }
        public List<TipoPrestacion> GetTipoPrestaciones()
        {
            return this._ticketRepository.GetTipoPrestaciones();
        }

        public string ValidarTramiteRentas(TramiteRentas request)
        {
            string response = this._ticketRepository.ValidarTramiteRentas(request);

            return response;
        }
        public async Task<ResponseViewModel> updateStateJira(TicketUpdateJiraPutModel model)
        {
            ResponseViewModel res = new ResponseViewModel();

            try
            {
                string baseUrl = AppSettings.AWSRegistrar;
                string url = AppSettings.AWSRegistrar2 + "/" + model.code;
                string token = new TokenService().getTokenAWS("360");
                HttpResponseMessage response = await _ticketRepository.PutRequest(baseUrl, url, model, token);
                //var parsed = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(response);
                if (!response.IsSuccessStatusCode)
                {
                    res.message = "Ocurrió un error";
                    res.code = 1;
                    var ResponseBodyss = response.Content.ReadAsAsync(typeof(object)).Result;
                    res.customMessage = ResponseBodyss;
                }
                else
                {
                    res.message = "Modificación exitosa";
                    res.code = 0;
                }


                return res;

            }
            catch (Exception e)
            {
                res.message = e.Message;
                res.customMessage = e;
                return res;

            }

        }

        public async Task<ResponseViewModel> SetObservation(ResponseObservationModel model)
        {
            try
            {
                List<Archivo> adjunt = new List<Archivo>();
                ResponseViewModel res = new ResponseViewModel();
                string scodejira = "";
                (scodejira, adjunt) = _ticketRepository.GetAdjuntos2(model.code, 3);//consulta adjunto
                if (scodejira == "")
                {
                    ResponseViewModel re = new ResponseViewModel();
                    re.message = "No existe el código Jira";
                    re.code = 1;
                    re.customMessage = null;
                    return re;
                }

                if (model.files.Count > 0)
                {
                    foreach (Archivo file in model.files)
                    {
                        await _ticketRepository.S3AdjuntarAsync(file, "360", "customfield_13414");//guarda adjunto en S3
                        if (file.path_gd != null)
                        {
                            file.scode = model.code;
                            file.path = file.path_gd;
                            file.nuser = "99999";
                            file.tipo = "3";
                            await _ticketRepository.SetArchivoAdjuntoAsync(file);
                        }
                    }
                }

                (scodejira, adjunt) = _ticketRepository.GetAdjuntos2(model.code, 3);

                TicketUpdateJiraPutModel ticket = new TicketUpdateJiraPutModel();
                ticket.system = "360";

                CustomField field1 = new CustomField();
                field1.id = "customfield_13413";
                field1.value = model.response;
                ticket.fields = new List<CustomField>();
                ticket.fields.Add(field1);
                CustomField field2 = new CustomField();
                field2.id = "customfield_13414";
                ticket.code = scodejira;
                if (adjunt.Count > 0)
                {
                    field2.value = String.Join(',', adjunt.Where(x => x.path_gd != "").Select(p => p.path_gd.ToString()));
                }
                ticket.fields.Add(field2);
                res = await updateStateJira(ticket);

                return res;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        //DEV RM -- INI
        public TicketSTC GetTicketSTC(string scode)
        {
            return _ticketRepository.GetTicketSTC(scode);
        }
        //DEV RM -- FIN
        //DEV CY -- INI
        public Ticket GetTicketType(string codigo)
        {
            Ticket ticket = new Ticket();
            ticket = this._ticketRepository.GetTicketType(codigo);
            return ticket;
        }

        public Ticket GetSTC_Ticket(string codigo)
        {
            Ticket ticket = new Ticket();
            ticket = this._ticketRepository.GetSTC_Ticket(codigo); 
            ticket.Adjuntos = new List<Archivo>();
            ticket.Adjuntos = this._ticketRepository.GetAdjuntos(codigo, 1);
            ticket.FecRecepcion = ticket.FecRecepcion?.Substring(0, 10);
            ticket.FecRegistro = ticket.FecRegistro?.Substring(0, 10);
            return ticket;
        }
        //DEV CY -- FIN
        //DEV DS -- INI
        public List<TipoDocumento> GetTipoDocumento()
        {
            return this._ticketRepository.GetTipoDocumento();
        }
        public List<Banco> GetBanco()
        {
            return this._ticketRepository.GetBanco();
        }
        public List<TipoMoneda> GetTipoMoneda()
        {
            return this._ticketRepository.GetTipoMoneda();
        }
        public List<TipoCuenta> GetTipoCuenta()
        {
            return this._ticketRepository.GetTipoCuenta();
        }
        public DatosPagoResponse RegistrarDatosPago(DatosPagoRequest request)
        {
            DatosPagoResponse response = this._ticketRepository.RegistrarDatosPago(request);
            return response;
        }
        //DEV DS -- FIN
        //DEV CY -- INI
        public ResponseViewModel UpdateFieldsSTC(DatosPagoRequest model)
        { 
            ResponseViewModel response = this._ticketRepository.UpdateFieldsSTC(model);
            return response;
        }

       public async Task<ResponseViewModel> UpdateInsertFilesSTC(List<Archivo> adjuntos)
        {
            ResponseViewModel response = new ResponseViewModel();
            try
            {
                for (int i = 0; i < adjuntos.Count; i++)
                {
                    //primero modificar estado si elimino un adjunto
                    /*if (adjuntos[i].sstate == "0")
                    {
                        ResponseViewModel responseUpdate = this._ticketRepository.UpdateTicketFile(adjuntos[i].nid);
                    }*/
                    //luego insertar nuevos adjuntos
                    //
                    if (adjuntos[i].sstate == "1")  
                    {
                        await _ticketRepository.S3AdjuntarAsync(adjuntos[i], "360", "customfield_13607");//guarda adjunto en S3--56
                        if (adjuntos[i].path_gd != null)
                        {
                            adjuntos[i].scode = adjuntos[i].scode;
                            adjuntos[i].path = adjuntos[i].path_gd;
                            adjuntos[i].nuser = "99999";
                            adjuntos[i].tipo = "3";
                            await _ticketRepository.SetArchivoAdjuntoAsync(adjuntos[i]);//guarda en bd
                        }
                    }
                }
                response.code = 0;
                response.message = "Modificación exitosa";
            }
            catch (Exception ex)
            {
                response.code = 1;
                response.message = ex.Message;
            }
            return response;
        }

        public async Task<ResponseViewModel> SetSTCObservation(DatosPagoRequest model)
        {
            try
            {
                List<Archivo> adjunt = new List<Archivo>();
                ResponseViewModel res = new ResponseViewModel();
                ResponseViewModel resUpdateTicket = new ResponseViewModel();
                string scodejira = "";
                resUpdateTicket = _ticketRepository.SetSTCObservation(model.scode);
                /*if(resUpdateTicket.code == 0)//if (scodejira == "")
                {
                    ResponseViewModel re = new ResponseViewModel();
                    re.message = "No existe el código Jira";
                    re.code = 1;
                    re.customMessage = null;
                    return re;
                }*/

                TicketUpdateJiraPutModel ticket = new TicketUpdateJiraPutModel();
                ticket.system = "360";

                CustomField field1 = new CustomField();
                field1.id = "customfield_12404";//customfield_13413 para fecha de modificacion
                field1.value = String.Format("{0:s}", DateTime.Now) + ".000+0000";//model.response;
                ticket.fields = new List<CustomField>();
                ticket.fields.Add(field1); 
                //campos modificados
                string fields = "";
                (fields) = _ticketRepository.GetFieldsSTC(model);

                if (fields != "")
                {
                    var arrCampos = fields.Split(";");
                    for (int i = 0; i < arrCampos.Length - 1; i++)
                    {
                        CustomField fieldsSTC = new CustomField();
                        var arrIdValue = arrCampos[i].Split(",");
                        fieldsSTC.id = arrIdValue[0];
                        fieldsSTC.value = arrIdValue[1];
                        if (fieldsSTC.id == "customfield_11314")
                        {
                            if (arrIdValue.Length > 2)
                            {
                                fieldsSTC.value = string.Join(",", arrIdValue[1], arrIdValue[2]);
                            }
                            fieldsSTC.value = Convert.ToSingle(fieldsSTC.value, CultureInfo.CreateSpecificCulture("es-ES"));
                        }//valida formato de monto
                        if (fieldsSTC.id == "customfield_13321" || fieldsSTC.id == "customfield_13447" || fieldsSTC.id == "customfield_11709" || fieldsSTC.id == "customfield_13449")
                        {
                            fieldsSTC.value = new ID { id = fieldsSTC.value.ToString() };
                        }
                        ticket.fields.Add(fieldsSTC);//agrega cada campo con cambio
                    }
                }
                //adjuntos modificados COMPARAR LISTA DE ANTES Y AHORA
                /*if (model.files.Count > 0)//si se realizo cambio en adjuntos
                {
                    //primero modificar estado si elimino un adjunto
                    for (int i = 0; i<model.files.Count; i++)
                    {
                        if(model.files[i].sstate == "0")
                        {
                            ResponseViewModel resFile = new ResponseViewModel();
                            resFile = _ticketRepository.UpdateTicketFile(model.files[i].nid);
                        }
                        //luego revisar si hay nuevo adjunto con spath_gd o sstate 1 
                        if(model.files[i].sstate == "1")
                        {
                            await _ticketRepository.S3AdjuntarAsync(model.files[i], "360", "customfield_12801");//guarda adjunto en S3
                            if (model.files[i].path_gd != null)
                            {
                                model.files[i].scode = model.scode;
                                model.files[i].path = model.files[i].path_gd;
                                model.files[i].nuser = "99999";
                                model.files[i].tipo = "1";
                                await _ticketRepository.SetArchivoAdjuntoAsync(model.files[i]);//guarda en bd
                            }
                        }
                    }
                    //finalmente junta todos los adjuntos y se manda en json
                    (scodejira, adjunt) = _ticketRepository.GetAdjuntos2(model.scode, 1);
                    CustomField field2 = new CustomField();

                    field2.id = "customfield_12801";//customfield_13414 campo para adjunto en tre
                    
                    if (adjunt.Count > 0)
                    {
                        field2.value = String.Join(',', adjunt.Where(x => x.path_gd != "").Select(p => p.path_gd.ToString()));
                    }
                    ticket.fields.Add(field2);
                }*/
                //validar adjunto como 0 o 1
                 
                 if (model.flag_file == "1")
                 {
                    (scodejira, adjunt) = _ticketRepository.GetAdjuntos2(model.scode, 3);//consigue adjunto del ticket
                    CustomField field2 = new CustomField(); 

                    field2.id = "customfield_13607";//customfield_13414 campo para adjunto en tre 
                    if (adjunt.Count > 0)
                    {
                        field2.value = String.Join(',', adjunt.Where(x => x.path_gd != "").Select(p => p.path_gd.ToString())); 
                    }   
                    ticket.fields.Add(field2); 
                 }

                 if(model.P_RESPUESTA != null)
                {
                    CustomField fieldr = new CustomField(); 
                    fieldr.id = "customfield_13609";//customfield_13414 campo para adjunto en tre 
                    fieldr.value = model.P_RESPUESTA;
                     
                    ticket.fields.Add(fieldr);
                }

                ticket.code = resUpdateTicket.message;
                string jsonString = string.Empty;//OBTENER JSON
                jsonString = JsonSerializer.Serialize(ticket);
                res = await updateStateJira(ticket);

                return res;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        //DEV CY -- FIN
        //DEV DS INICIO
        public DatosPagoRequest GetInfoToUPDSTC(string scode) 
        {
            return _ticketRepository.GetInfoToUPDSTC(scode);
        }

        //DEV DS FINAL
        public int AfterFecCorte(string scode) {
            return _ticketRepository.AfterFecCorte(scode);
        }
    }
}