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
    public class SolSinService : ISolSinService
    {
        private readonly ISolSinRepository _SolSinRepository;
        public SolSinService(ISolSinRepository SolSinRepository)
        {
            this._SolSinRepository = SolSinRepository;
        }
        public string GenerateSolicitud(Contract contract)
        {
            return _SolSinRepository.GenerateSolicitudNew(contract);
        }
        public List<Adjunto> GetAdjuntos(string codigo, string tipo)
        {
            return this._SolSinRepository.GetAdjuntosNew(codigo, int.Parse(tipo));
        }
        public List<Adjunto> GetEnviados(string codigo)
        {
            return this._SolSinRepository.GetAdjuntosNew(codigo, 2);
        }
        public async Task<string> GetS3Adjunto(string codigo, string type)
        {
            return await this._SolSinRepository.GetS3AdjuntoNew(codigo, type);
        }
        public DatosPagoResponse RegistrarDatosPago(DatosPagoRequest request)
        {
            DatosPagoResponse response = this._SolSinRepository.RegistrarDatosPagoNew(request);
            return response;
        }
        public RegiTicketResponse RegistraTicketJIRA(Contract request, string type)
        {
            string tipo = request.Codigo?.Substring(0, 3);
            var tip = string.Empty;
            if (tipo == "SIN")
            {
                request.Motivo = request.MotivoREC;
                request.SubMotivo = request.SubMotivoREC;
            }
            
            List<EstructuraTicket> estructuras = this._SolSinRepository.GetEstructuraNew(tip);
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
                    if (request.adjunto != null)
                    {
                        foreach (Adjunto adjunto in request.adjunto)
                        {
                            att.value.Add(adjunto.path_gd);
                        }
                        dinamico.attachments.Add(att);
                    }
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
                }
            }
            string jsonString = string.Empty;
            jsonString = JsonSerializer.Serialize(dinamico);

            RegiTicketResponse response = this._SolSinRepository.RegistraTicketJIRANew(dinamico, type);
            RegiTicketResponse final = this._SolSinRepository.SetJIRANew(new Contract { Codigo = request.Codigo, CodigoJIRA = response.Codigo, Aplicacion = "360" });
            return final;
        }
        public List<Adjunto> S3Adjuntar(List<Adjunto> adjuntos, string type)
        {
            List<Adjunto> grabados = new List<Adjunto>();
            foreach (Adjunto ar in adjuntos)
            {
                Adjunto arch = this._SolSinRepository.S3AdjuntarNew(ar, type);
            }
            return adjuntos;
        }
        public string SetArchivosAdjunto(List<Adjunto> adjuntos)
        {
            string respuesta = string.Empty;
            bool control = true;
            foreach (Adjunto arch in adjuntos)
            {
                var r = this._SolSinRepository.SetArchivoAdjuntoNew(arch);
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
        public Field AgregarCampo(EstructuraTicket est, Contract contract)
        {
            object value = null;

            if (est.STYPE == "summary" || est.STYPE == "string" || est.STYPE == "description")
            {
                value = contract.GetType().GetProperty(est.SDESCRIPT).GetValue(contract).ToString();
            }
            if (est.STYPE == "number")
            {
                value = contract.GetType().GetProperty(est.SDESCRIPT).GetValue(contract);
            }
            if (est.STYPE == "datetime")
            {
                string conver = "";
                var fecha = contract.GetType().GetProperty(est.SDESCRIPT).GetValue(contract);
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
                value = conver + "T05:00:00.000+0000";
            }
            if (est.STYPE == "issuetype" || est.STYPE == "option")
            {
                var valor = contract.GetType().GetProperty(est.SDESCRIPT).GetValue(contract).ToString();
                value = new ID { id = valor };
            }
            if (est.STYPE == "project")
            {
                var valor = contract.GetType().GetProperty(est.SDESCRIPT).GetValue(contract).ToString();
                value = new Key { key = valor };
            }
            if (est.STYPE == "user" || est.STYPE == "reporter")
            {
                var valor = contract.GetType().GetProperty(est.SDESCRIPT).GetValue(contract).ToString();
                value = new Name { name = valor };
            }
            if (est.STYPE == "array")
            {
                var valor = contract.GetType().GetProperty(est.SDESCRIPT).GetValue(contract).ToString();
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
                var valor = contract.GetType().GetProperty(est.SDESCRIPT).GetValue(contract).ToString();
                value = new Child { id = contract.Motivo, child = new ID { id = contract.SubMotivo } };
            }
            if (est.STYPE == "option-with-child2")
            {
                var valor = contract.GetType().GetProperty(est.SDESCRIPT).GetValue(contract).ToString();
                value = new Child { id = contract.Ramo, child = new ID { id = contract.Producto } };
            }

            Field field = new Field { id = est.SCODE_JIRA, value = value };
            return field;
        }
        public CommonResponse ValidateTicketNew(RegContractRequest request)
        {
            return this._SolSinRepository.ValidateTicketNew(request);
        }
        public RegiTicketResponse SetTicketNew(RegContractRequest request)
        {
            RegiTicketResponse response = this._SolSinRepository.SetTicketNew(request);
            return response;
        }
        public async Task<Contract> GetTicketNew(string codigo, string type)
        {
            Contract contract = new Contract();
            string token = new TokenService().getTokenAWS(type);
            contract = this._SolSinRepository.GetTicketNew(codigo);
            if (contract.CodigoJIRA != "" && contract.CodigoJIRA != null)
            {
                Ticket jira = await this._SolSinRepository.ConsultaTicketJIRANew(contract.CodigoJIRA, token, type);
            }
            return contract;
        }
        public RegiTicketResponse GestionaRegistroJIRANew(string codigo, string type)
        {
            RegiTicketResponse response = new RegiTicketResponse();
            //DEV CY -- INI
            Contract request = new Contract();
            Contract ticket = GetTicketType(codigo);
            if (ticket.Codigo == "SIN") 
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
        public Contract GetTicketType(string codigo)
        {
            Contract ticket = new Contract();
            ticket = this._SolSinRepository.GetTicketType(codigo);
            return ticket;
        }
        public Contract GetSTC_Ticket(string codigo)
        {
            Contract ticket = new Contract();
            ticket = this._SolSinRepository.GetSTC_Ticket(codigo); 
            ticket.adjunto = new List<Adjunto>();            
            return ticket;
        }
        public Contract GetJIRA(string codigo)
        {
            Contract ticket = new Contract();
            ticket = this._SolSinRepository.GetJIRA(codigo);
            ticket.adjunto = new List<Adjunto>();
            return ticket;
        }
    }
}
