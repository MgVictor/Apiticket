using apiTicket.Models;
using apiTicket.Models.Reportes;
using apiTicket.Repository.DB;
using apiTicket.Repository.Interfaces;
using apiTicket.Utils;
using apiTicket.Utils.Authentication;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using Oracle.ManagedDataAccess.Client;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace apiTicket.Repository
{
    public class TicketRepository : ITicketRepository
    {
        private readonly IOptions<AppSettings> appSettings;
        private readonly IConnectionBase _connectionBase;


        public TicketRepository(IOptions<AppSettings> appSettings, IConnectionBase ConnectionBase)
        {
            this.appSettings = appSettings;
            _connectionBase = ConnectionBase;

        }
        private string Package3 = "PKG_BDU_CLIENT_360";
        private string Package4 = "PKG_BDU_TICKET";
        public TicketResponse SetTicketS(TicketSiniestro ticket)
        {
            TicketResponse response = new TicketResponse();
            response.codTicket = "SIN-001";
            response.respuesta = true;
            response.mensaje = "Registro exitoso";
            return response;
        }
        public string UpdPlazoTicket(UpdPlazo updPlazo)
        {
            string response;
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>
                {
                    new OracleParameter("P_SCODE", OracleDbType.Varchar2, updPlazo.Scode, ParameterDirection.Input),
                    new OracleParameter("P_NID_TIEMPO_ATENCION", OracleDbType.Int32, updPlazo.NidTiempoAtencion, ParameterDirection.Input),
                    new OracleParameter("P_SEMAIL", OracleDbType.Varchar2, updPlazo.Semail, ParameterDirection.Input)
                };
                _connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package4, "PRC_UPD_CLIENT_TICKET_PLAZO"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime, ConnectionBase.enuTypeExecute.ExecuteNonQuery);
                response = null;
            } catch (Exception ex)
            {
                response = ex.Message;
            }
            return response;
        }
        public TicketResponse UpdTicket(TicketSiniestro ticket)
        {
            TicketResponse response = new TicketResponse();
            response.codTicket = "SIN-001";
            response.respuesta = true;
            response.mensaje = "Actualización exitosa";
            return response;
        }
        public List<Motivo> GetMotivos(int TipoTicket)
        {
            List<Motivo> motivos = new List<Motivo>();
            try
            { 
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("NTYPEID", OracleDbType.Int32, TipoTicket, ParameterDirection.Input));
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package3, "PRC_REA_TICK_MOTIVOS"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        Motivo motivo = new Motivo();
                        motivo.Id = int.Parse(dr["NID"].ToString());
                        motivo.Descripcion = dr["SDESCRIPT"].ToString();
                        motivos.Add(motivo);
                    }
                }
            }
            catch (Exception)
            {
                motivos = new List<Motivo>();
            }

            return motivos;
        }
        public List<SubMotivo> GetSubMotivos(int TipoTicket, int Motivo)
        {
            List<SubMotivo> motivos = new List<SubMotivo>();
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("NTYPEID", OracleDbType.Int32, TipoTicket, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NMOTIVID", OracleDbType.Int32, Motivo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package3, "PRC_REA_TICK_SUBMOTIVOS"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        SubMotivo motivo = new SubMotivo();
                        motivo.Id = int.Parse(dr["NID"].ToString());
                        motivo.Codigo = dr["CODIGO"].ToString();
                        motivo.Descripcion = dr["SDESCRIPT"].ToString();
                        motivos.Add(motivo);
                    }
                }
            }
            catch (Exception ex)
            {
                motivos = new List<SubMotivo>();
            }
            return motivos;
        }
        //DEV EC - INICIO
        public List<SubMotivo> GetSubMotivosSol(int TipoTicket, int Motivo)
        {
            List<SubMotivo> motivos = new List<SubMotivo>();
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("NTYPEID", OracleDbType.Int32, TipoTicket, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NMOTIVID", OracleDbType.Int32, Motivo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package3, "PRC_REA_TICK_SUBMOTIVOS_SOL"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        SubMotivo motivo = new SubMotivo();
                        motivo.Id = int.Parse(dr["NID"].ToString());
                        motivo.Codigo = dr["CODIGO"].ToString();
                        motivo.Descripcion = dr["SDESCRIPT"].ToString();
                        motivo.CobranTram = dr["NCOBRANTRAM"].ToString();
                        motivos.Add(motivo);
                    }
                }
            }
            catch (Exception ex)
            {
                motivos = new List<SubMotivo>();
            }
            return motivos;
        }
        //DEV EC - FIN
        public List<Client> GetContactos(string tipo, string documento)
        {
            List<Client> Clients = new List<Client>();
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("NTYPEDOC", OracleDbType.Int32, tipo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SDOCNUM", OracleDbType.Int32, documento, ParameterDirection.Input));
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package3, "PRC_REA_CLICONTACT_AUTO"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        Client cliente = new Client();
                        cliente.name = dr["Nombres"].ToString();
                        cliente.tipodoc = dr["NIDDOC_TYPE"].ToString();
                        cliente.documento = dr["SIDDOC"].ToString();
                        cliente.correo = dr["SE_MAIL"].ToString();
                        cliente.direccion = dr["ADDRESS"].ToString();
                        cliente.ubigeo = dr["SDISTRICT"].ToString();
                        Clients.Add(cliente);
                    }
                }
            }
            catch (Exception)
            {
                Clients = new List<Client>();
            }
            return Clients;
        }
        public List<Canal> GetCanales()
        {
            List<Canal> canales = new List<Canal>();
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package3, "PRC_REA_TICK_CANAL"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        Canal canal = new Canal();
                        canal.Id = int.Parse(dr["NID"].ToString());
                        //canal.Codigo = int.Parse(dr["CODIGO"].ToString());
                        canal.Descripcion = dr["SDESCRIPT"].ToString();
                        canales.Add(canal);
                    }
                }
            }
            catch (Exception)
            {
                canales = new List<Canal>();
            }
            return canales;
        }
        public List<Via> GetViasRecepcion()
        {
            List<Via> vias = new List<Via>();
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package3, "PRC_REA_TICK_VIARECEPCION"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        Via via = new Via();
                        via.Id = int.Parse(dr["NID"].ToString());
                        //via.Codigo = int.Parse(dr["CODIGO"].ToString());
                        via.Descripcion = dr["SDESCRIPT"].ToString();
                        vias.Add(via);
                    }
                }
            }
            catch (Exception)
            {
                vias = new List<Via>();
            }
            return vias;
        }
        public List<Via> GetViasRespuesta()
        {
            List<Via> vias = new List<Via>();
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package3, "PRC_REA_TICK_VIARESPUESTA"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        Via via = new Via();
                        via.Id = int.Parse(dr["NID"].ToString());
                        //via.Codigo = int.Parse(dr["CODIGO"].ToString());
                        via.Descripcion = dr["SDESCRIPT"].ToString();
                        vias.Add(via);
                    }
                }
            }
            catch (Exception)
            {
                vias = new List<Via>();
            }
            return vias;
        }
        public List<Estado> GetEstadosTicket()
        {
            List<Estado> estados = new List<Estado>();
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package3, "PRC_REA_TICK_ESTADOS"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        Estado estado = new Estado();
                        estado.IdEstado = int.Parse(dr["NID"].ToString());
                        //via.Codigo = int.Parse(dr["CODIGO"].ToString());
                        estado.DescEstado = dr["SDESCRIPT"].ToString();
                        estados.Add(estado);
                    }
                }
            }
            catch (Exception ex)
            {
                estados = new List<Estado>();
            }
            return estados;
        }
        public RegiTicketResponse SetTicket(RegTicketRequest request)
        {
            RegiTicketResponse ticket = new RegiTicketResponse(); 
            if (string.IsNullOrEmpty(request.EmailTitular)) { request.EmailTitular = null; }
            if (string.IsNullOrEmpty(request.DireccionTitular)) { request.EmailTitular = null; }
            if (string.IsNullOrEmpty(request.UbigeoTitular)) { request.EmailTitular = null; }
            if (string.IsNullOrEmpty(request.Contacto.tipodoc)) { request.Contacto.tipodoc = "2"; }
            if (request.NID_TICK_PREST == "0") { request.NID_TICK_PREST = null; }
              
            try
            {
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;

                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("NTYPETICK", OracleDbType.Varchar2, request.Tipo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NTYPEDOCCLI", OracleDbType.Varchar2, request.tipoDocClient, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SDOCNUMCLI", OracleDbType.Varchar2, request.docClient, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NTYPEDOCCON", OracleDbType.Varchar2, request.Contacto.tipodoc, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SDOCNUMCON", OracleDbType.Varchar2, request.Contacto.documento, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SEMAILTITULAR", OracleDbType.Varchar2, request.EmailTitular, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SDIRECCIONTTITULAR", OracleDbType.Varchar2, request.DireccionTitular, ParameterDirection.Input));
                //  parameters.Add(new OracleParameter("SUBIGEOTITULAR", OracleDbType.Varchar2, request.Contacto.documento, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SUBIGEOTITULAR", OracleDbType.Varchar2, request.UbigeoTitular, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NBRANCH", OracleDbType.Varchar2, request.Ramo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Varchar2, request.Producto, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NPOLICY", OracleDbType.Varchar2, request.Poliza, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NMONTO", OracleDbType.Varchar2, request.Monto, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SDESCRIPCION", OracleDbType.Varchar2, request.Descripcion, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NREP", OracleDbType.Varchar2, request.Reconsideracion, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NVIARECP", OracleDbType.Varchar2, request.ViaRecepcion.Id, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NVIARESP", OracleDbType.Varchar2, request.ViaRespuesta.Id, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NMOTIVO", OracleDbType.Varchar2, request.SubMotivo.Motivo.Id, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NSUBMOTIVO", OracleDbType.Varchar2, request.SubMotivo.Id, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NUSER", OracleDbType.Varchar2, request.Usuario, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NCANAL", OracleDbType.Varchar2, request.Canal, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NRECON", OracleDbType.Varchar2, request.Reconsideracion, ParameterDirection.Input));
                parameters.Add(new OracleParameter("DFECRECEP", OracleDbType.Varchar2, request.FechaRecepcion, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NESTADO", OracleDbType.Varchar2, request.Estado, ParameterDirection.Input));
                parameters.Add(new OracleParameter("N_TICK_PREST", OracleDbType.Varchar2, request.NID_TICK_PREST, ParameterDirection.Input));
                parameters.Add(new OracleParameter("N_TICK_TYPE_JIRA", OracleDbType.Varchar2, request.NID_TBL_TICK_TYPE_JIRA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SRESPUESTA", OracleDbType.Varchar2, request.Respuesta, ParameterDirection.Input));

                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package3, "PRC_SET_TICKET"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    var v_P_NCODE = P_NCODE.Value.ToString();
                    List<String> lst_mensaje = new List<String>();
                    lst_mensaje.Add(P_SMESSAGE.Value.ToString());
                    var v_respuesta = true;
                    if (v_P_NCODE != "0")
                    {
                        v_respuesta = false;
                    }
                    else
                    {
                        while (dr.Read())
                        {
                            ticket.Codigo = dr["STICKET"].ToString();

                        }
                    }


                    //     response.mensajeRespuesta = 

                    ticket.respuesta = v_respuesta;
                    ticket.mensajes = lst_mensaje;
                    ticket.codigoRespuestaError = v_P_NCODE; 
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex, "Catch");
                ticket = new RegiTicketResponse();
                ticket.respuesta = false;
                ticket.mensaje = ex.Message;
            }
            return ticket;
        }

        //add 20220218
        public CommonResponse ValidateTicket(RegTicketRequest request)
        {
            CommonResponse oValidateTicket = new CommonResponse();
            String mensaje;
            List<String> listMensajes = new List<string>();
            oValidateTicket.respuesta = true;


            if (string.IsNullOrEmpty(request.EmailTitular)) { request.EmailTitular = null; }
            if (string.IsNullOrEmpty(request.DireccionTitular)) { request.EmailTitular = null; }
            if (string.IsNullOrEmpty(request.UbigeoTitular)) { request.EmailTitular = null; }
            if (string.IsNullOrEmpty(request.Contacto.tipodoc)) { request.Contacto.tipodoc = "2"; }

            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("NTYPETICK", OracleDbType.Varchar2, request.Tipo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NTYPEDOCCLI", OracleDbType.Varchar2, request.tipoDocClient, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SDOCNUMCLI", OracleDbType.Varchar2, request.docClient, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NTYPEDOCCON", OracleDbType.Varchar2, request.Contacto.tipodoc, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SDOCNUMCON", OracleDbType.Varchar2, request.Contacto.documento, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SEMAILTITULAR", OracleDbType.Varchar2, request.EmailTitular, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SDIRECCIONTTITULAR", OracleDbType.Varchar2, request.DireccionTitular, ParameterDirection.Input));
                //  parameters.Add(new OracleParameter("SUBIGEOTITULAR", OracleDbType.Varchar2, request.Contacto.documento, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SUBIGEOTITULAR", OracleDbType.Varchar2, request.UbigeoTitular, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NBRANCH", OracleDbType.Varchar2, request.Ramo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Varchar2, request.Producto, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NPOLICY", OracleDbType.Varchar2, request.Poliza, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NMONTO", OracleDbType.Varchar2, request.Monto, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SDESCRIPCION", OracleDbType.Varchar2, request.Descripcion, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NREP", OracleDbType.Varchar2, request.Reconsideracion, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NVIARECP", OracleDbType.Varchar2, request.ViaRecepcion.Id, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NVIARESP", OracleDbType.Varchar2, request.ViaRespuesta.Id, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NMOTIVO", OracleDbType.Varchar2, request.SubMotivo.Motivo.Id, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NSUBMOTIVO", OracleDbType.Varchar2, request.SubMotivo.Id, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NUSER", OracleDbType.Varchar2, request.Usuario, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NCANAL", OracleDbType.Varchar2, request.Canal, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NRECON", OracleDbType.Varchar2, request.Reconsideracion, ParameterDirection.Input));
                parameters.Add(new OracleParameter("DFECRECEP", OracleDbType.Varchar2, request.FechaRecepcion, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NESTADO", OracleDbType.Varchar2, request.Estado, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SRESPUESTA", OracleDbType.Varchar2, request.Respuesta, ParameterDirection.Input));

                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package3, "PRC_VALIDATE_TICKET"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        mensaje = dr["SMENSAJE"].ToString();
                        listMensajes.Add(mensaje);

                    }
                }
                oValidateTicket.mensajes = listMensajes;
                if (oValidateTicket.mensajes.Count > 0)
                {
                    oValidateTicket.respuesta = false;
                }
            }
            catch (Exception)
            {
                oValidateTicket.respuesta = false;
            }
            return oValidateTicket;
        }
        //add 20220218

        public List<Policy> GetClientPolicies(string product, string tipoDocumento, string documento)
        {
            throw new NotImplementedException();
        }
        public List<Ticket> GetClientTickets(GetTicketRequest request)
        {
            List<Ticket> Tickets = new List<Ticket>();
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("NTYPEDOC", OracleDbType.Varchar2, request.tipoDocumento, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SDOCNUM", OracleDbType.Varchar2, request.documento, ParameterDirection.Input));
                parameters.Add(new OracleParameter("DINIFECHA", OracleDbType.Varchar2, request.fechaInicio, ParameterDirection.Input));
                parameters.Add(new OracleParameter("DDFINFECHA", OracleDbType.Varchar2, request.fechaFin, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NSTATE", OracleDbType.Varchar2, "0"/*request.Estado.ToString()*/, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NVIARECEP", OracleDbType.Varchar2, request.viaRecep.ToString(), ParameterDirection.Input));
                //parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Varchar2, request.Ramo.ToString(), ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Varchar2, request.Producto.ToString(), ParameterDirection.Input));
                parameters.Add(new OracleParameter("USUARIO", OracleDbType.Varchar2, request.Usuario, ParameterDirection.Input));
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package3, "PRC_REA_CLIENT_TICKETS"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        Ticket ticket = new Ticket();
                        ticket.Codigo = dr["SCODE"].ToString();
                        ticket.CodigoJIRA = dr["SCODE_JIRA"].ToString();
                        ticket.FecRecepcion = dr["DRECEPDATE"].ToString();
                        ticket.FecRegistro = dr["DREGDATE"].ToString();
                        ticket.ViaRecepcion = dr["Via"].ToString();
                        ticket.Ramo = dr["Ramo"].ToString();
                        ticket.Producto = dr["Producto"].ToString();
                        ticket.Poliza = dr["Poliza"].ToString();
                        if (ticket.Poliza == "-1")
                        {
                            ticket.Poliza = "";
                        }
                        ticket.Estado = dr["Estado"].ToString();
                        //add 20220211
                        ticket.NombreUsuarioRegistraTicket = dr["USER_NAME_REG_TICK"].ToString();
                        ticket.Motivo = dr["MOTIVO"].ToString();
                        ticket.SubMotivo = dr["SUBMOTIVO"].ToString();
                        //add 20220211
                        Tickets.Add(ticket);
                    }
                }
            }
            catch (Exception)
            {
                Tickets = new List<Ticket>();
            }
            return Tickets;
        }
        public Ticket GetCodigo(string codigo)
        {
            Ticket ticket = new Ticket();
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("SCODE", OracleDbType.Varchar2, codigo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package3, "PRC_REA_CLIENT_CODE"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {

                        ticket.Codigo = dr["SCODE"].ToString();
                    }
                }
            }
            catch (Exception)
            {
                ticket = new Ticket();
            }
            return ticket;
        }
        public Ticket GetTicket(string codigo)
        {
            Ticket ticket = new Ticket();
            try
            { 
                List <OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("SCODE", OracleDbType.Varchar2, codigo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package3, "PRC_REA_CLIENT_TICKET"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        ticket.Codigo = dr["SCODE"].ToString();
                        ticket.CodigoJIRA = dr["SCODE_JIRA"].ToString();
                        ticket.FecRecepcion = dr["DRECEPDATE"].ToString();
                        ticket.FecRegistro = dr["DREGDATE"].ToString();
                        ticket.ViaRecepcion = dr["VIA"].ToString();
                        ticket.Ramo = dr["RAMO"].ToString();
                        ticket.Producto = dr["PRODUCTO"].ToString();
                        ticket.Poliza = dr["POLIZA"].ToString();
                        ticket.Estado = dr["ESTADO"].ToString();
                        ticket.Contacto = dr["CONTACTO"].ToString();
                        ticket.Documento = dr["DOCUMENTO"].ToString();
                        //ticket.TipoDocumento = dr["TIPDOC_CONTAC"].ToString();
                        ticket.ViaRespuesta = dr["VIARESPUESTA"].ToString();
                        ticket.Monto = dr["MONTO"].ToString();
                        ticket.SubMotivo = dr["SUBMOTIVO"].ToString();
                        ticket.Motivo = dr["MOTIVO"].ToString();
                        ticket.Direccion = dr["DIRECCIO"].ToString();
                        ticket.Canal = dr["CANAL"].ToString();
                        ticket.Descripcion = dr["DESCRIPCION"].ToString();
                        ticket.Reconsideracion = dr["NRECON"].ToString();
                        ticket.Telefono = dr["PHONE"].ToString();
                        ticket.Email = dr["MAIL"].ToString();
                        ticket.DiasAtencion = dr["DAYSAT"].ToString();
                        if (!string.IsNullOrEmpty(ticket.DiasAtencion))
                        {
                            if (ticket.DiasAtencion.Substring(0, 1) == ",")
                            {
                                ticket.DiasAtencion = "0" + ticket.DiasAtencion;
                            }
                        }
                        ticket.UsuarioEnvio = dr["USRAT"].ToString();
                        ticket.FechaEnvio = dr["DSENT"].ToString();
                        if (!string.IsNullOrEmpty(ticket.FechaEnvio))
                        {
                            ticket.FechaEnvio = ticket.FechaEnvio.Substring(0, 10);
                        }
                        ticket.referencia = dr["REFERENCIA"].ToString();
                        ticket.Nombre = dr["CLIENTENOM"].ToString();
                        ticket.TipoDocumentoCli = dr["TIPODOCCLI"].ToString();
                        ticket.DocumentoCli = dr["DOCUMENTOCLI"].ToString();
                        ticket.EmailCli = dr["EMAILCLI"].ToString();
                        ticket.direccioncli = dr["DIRCLI"].ToString();
                        ticket.Vinculo = dr["VINCULO"].ToString();
                        ticket.Reporter = dr["EXECU"].ToString();
                        ticket.DiasTiempoAtencion_SLA = dr["NID_TIEMPO_ATENCION"].ToString();
                        ticket.DescripcionTipoDiaPlural_SLA = dr["SDESCRIPT_PLURAL"].ToString();
                        ticket.NombreUsuarioRegistraTicket = dr["USER_NAME_REG_TICK"].ToString();
                        ticket.Respuesta = dr["SRESPUESTA"].ToString();
                        ticket.NtypeJira = dr["NTYPE_JIRA"].ToString();
                        ticket.Cussp = dr["CUSSP"].ToString();
                        ticket.Tramite_Rentas = dr["STRA_RENTAS"].ToString();
                        ticket.Tipo_Pension = dr["STRA_PENSION"].ToString();
                        ticket.Tipo_Prestacion = dr["STIP_PRES"].ToString();
                        ticket.Canal_Ingreso = dr["CANAL_INGRESO"].ToString();
                        ticket.SsimpleProduct = dr["SSIMPLEPRODUCT"].ToString();
                    }  
                }
            }
            catch (Exception ex)
            {
                ticket = new Ticket();
            }
            return ticket;
        }

        public string CerrarTicket(Ticket ticket)
        {
            string response = string.Empty;
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("SCODE", OracleDbType.Varchar2, ticket.Codigo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("USUARIO", OracleDbType.Varchar2, ticket.UsuarioEnvio, ParameterDirection.Input));
                parameters.Add(new OracleParameter("DSENT", OracleDbType.Varchar2, ticket.FechaEnvio, ParameterDirection.Input));
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package3, "PRC_REA_CERRAR_TICKET"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {

                        response = dr["ND"].ToString();
                    }
                }
            }
            catch (Exception)
            {
                response = "";
            }
            return response;
        }

        public string SetArchivoAdjunto(Archivo archivo)
        {
            string ticket = string.Empty;
            try
            { 
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("SCODE", OracleDbType.Varchar2, archivo.scode, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SNAME", OracleDbType.Varchar2, archivo.name, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SSIZE", OracleDbType.Varchar2, archivo.size, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SPATH", OracleDbType.Varchar2, archivo.path, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SPATH_GD", OracleDbType.Varchar2, archivo.path_gd, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SUSER", OracleDbType.Varchar2, archivo.nuser, ParameterDirection.Input));
                parameters.Add(new OracleParameter("STYPE", OracleDbType.Varchar2, archivo.tipo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package3, "PRC_SET_TICKET_ADJ"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        ticket = dr["TICKET"].ToString(); 
                    }
                }
            }
            catch (Exception)
            {
                ticket = string.Empty;
            }
            return ticket;
        }
        public async Task<string> SetArchivoAdjuntoAsync(Archivo archivo)
        {
            string ticket = string.Empty;
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("SCODE", OracleDbType.Varchar2, archivo.scode, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SNAME", OracleDbType.Varchar2, archivo.name, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SSIZE", OracleDbType.Varchar2, archivo.size, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SPATH", OracleDbType.Varchar2, archivo.path, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SPATH_GD", OracleDbType.Varchar2, archivo.path_gd, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SUSER", OracleDbType.Varchar2, archivo.nuser, ParameterDirection.Input));
                parameters.Add(new OracleParameter("STYPE", OracleDbType.Varchar2, archivo.tipo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)await _connectionBase.ExecuteByStoredProcedureVTAsync(string.Format("{0}.{1}", Package3, "PRC_SET_TICKET_ADJ"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (await dr.ReadAsync())
                    {
                        ticket = dr["TICKET"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ticket; 
        }
        public (string, List<Archivo>) GetAdjuntos2(string codigo, int tipo)
        {
            List<Archivo> Adjuntos = new List<Archivo>();
            string scodejira = "";
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("SCODE", OracleDbType.Varchar2, codigo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("STYPE", OracleDbType.Varchar2, tipo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SCODEJIRA", OracleDbType.Varchar2, ParameterDirection.Output));
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                parameters[2].Size = 4000;
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package3, "PRC_LIST_TICKET_ADJ2"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    scodejira = parameters[2].Value.ToString();
                    while (dr.Read())
                    {
                        Archivo arch = new Archivo();
                        //DEV CY 11-04-22 INI
                        arch.scode = dr["SCODE"].ToString();
                        //DEV CY 11-04-22 FIN
                        arch.name = dr["SNAME"].ToString();
                        arch.path = dr["SPATH"].ToString();
                        arch.size = dr["SSIZE"].ToString();
                        arch.path_gd = dr["SPATH_GD"].ToString();
                        Adjuntos.Add(arch);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return (scodejira, Adjuntos);
        }
        public List<Archivo> GetAdjuntos(string codigo, int tipo)
        {
            List<Archivo> Adjuntos = new List<Archivo>();
            string scodejira = "";
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("SCODE", OracleDbType.Varchar2, codigo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("STYPE", OracleDbType.Varchar2, tipo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                 
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package3, "PRC_LIST_TICKET_ADJ"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    scodejira = parameters[2].Value.ToString();
                    while (dr.Read())
                    {
                        Archivo arch = new Archivo();
                        //DEV DS 06/07/22 INI
                        arch.nid = dr["NID"].ToString(); 
                        //DEV DS 06/07/22 INI 
                        //DEV CY 11-04-22 INI
                        arch.scode = dr["SCODE"].ToString();
                        //DEV CY 11-04-22 FIN
                        arch.name = dr["SNAME"].ToString();
                        arch.path = dr["SPATH"].ToString();
                        arch.size = dr["SSIZE"].ToString();
                        arch.path_gd = dr["SPATH_GD"].ToString();
                        //DEV DS 06/07/22 INI
                        arch.sstate = dr["NTYPE"].ToString();
                        //DEV DS 06/07/22 INI
                        Adjuntos.Add(arch);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Adjuntos;
        }
        public List<Archivo> GetEnviados(string codigo, int tipo)
        {
            List<Archivo> Adjuntos = new List<Archivo>();
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("SCODE", OracleDbType.Varchar2, codigo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("STYPE", OracleDbType.Varchar2, tipo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package3, "PRC_LIST_TICKET_ADJ"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        Archivo arch = new Archivo();
                        arch.name = dr["SNAME"].ToString();
                        arch.path = dr["SPATH"].ToString();
                        arch.size = dr["SSIZE"].ToString();
                        arch.path_gd = dr["SPATH_GD"].ToString();
                        Adjuntos.Add(arch);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Adjuntos;
        }
        public List<Ticket> GetTicketsNoAtendidos()
        {
            List<Ticket> Tickets = new List<Ticket>();
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package3, "PRC_LIST_TICKET_NOATEND"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        Ticket ticket = new Ticket();
                        ticket.Codigo = dr["SCODE"].ToString();
                        ticket.FecRecepcion = dr["DRECEPDATE"].ToString();
                        ticket.FecRegistro = dr["DREGDATE"].ToString();
                        ticket.FecRecepcion = ticket.FecRecepcion.Substring(0, 10);
                        // ticket.FecRegistro = ticket.FecRegistro.Substring(0, 10);
                        ticket.Nombre = dr["SCLIENAME"].ToString();
                        ticket.Documento = dr["SDOCUME"].ToString();
                        ticket.Estado = dr["SDESCRIPT"].ToString();
                        ticket.TipoDocumento = dr["NTIPDOC"].ToString();
                        ticket.TipoDocumentoCli = dr["TipoDoc"].ToString();
                        ticket.Dias = dr["Espera"].ToString();
                        Tickets.Add(ticket);
                    }
                }
            }
            catch (Exception)
            {
                Tickets = new List<Ticket>();
            }
            return Tickets;
        }
        public Histograma GetHistograma()
        {
            Histograma hi = new Histograma();
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package3, "PRC_REPO_HIST"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        hi.REGEN = dr["REGEN"].ToString();
                        hi.CERREN = dr["CERREN"].ToString();
                        hi.REGFEB = dr["REGFEB"].ToString();
                        hi.CERRFEB = dr["CERRFEB"].ToString();
                        hi.REGMAR = dr["REGMAR"].ToString();
                        hi.CERRMAR = dr["CERRMAR"].ToString();
                        hi.REGABR = dr["REGABR"].ToString();
                        hi.CERRABR = dr["CERRABR"].ToString();
                        hi.REGMAY = dr["REGMAY"].ToString();
                        hi.CERRMAY = dr["CERRMAY"].ToString();
                        hi.REGJUN = dr["REGJUN"].ToString();
                        hi.CERRJUN = dr["CERRJUN"].ToString();
                        hi.REGJUL = dr["REGJUL"].ToString();
                        hi.CERRJUL = dr["CERRJUL"].ToString();
                        hi.REGAG = dr["REGAG"].ToString();
                        hi.CERRAG = dr["CERRAG"].ToString();
                        hi.REGSEP = dr["REGSEP"].ToString();
                        hi.CERRSEP = dr["CERRSEP"].ToString();
                        hi.REGOCT = dr["REGOCT"].ToString();
                        hi.CERROCT = dr["CERROCT"].ToString();
                        hi.GetType().GetProperty("REGNOV").SetValue(hi, dr["REGNOV"].ToString());
                        hi.CERRNOV = dr["CERRNOV"].ToString();
                        hi.GetType().GetProperty("REGDIC").SetValue(hi, dr["REGDIC"].ToString());
                        hi.CERRDIC = dr["CERRDIC"].ToString();
                    }
                }
            }
            catch (Exception)
            {
                hi = new Histograma();
            }
            return hi;

        }
        public Alerta GetAlerta()
        {
            Alerta alerta = new Alerta();
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package3, "PRC_LIST_LAST_ALERT"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        alerta.NID = long.Parse(dr["NDI"].ToString());
                        alerta.NTYPE = int.Parse(dr["NTYPE"].ToString());
                        alerta.SMESSAGE = dr["SMESSAGE"].ToString();
                        alerta.SLINK = dr["SLINK"].ToString();
                    }
                }
            }
            catch (Exception)
            {
                alerta = new Alerta();
            }
            return alerta;
        }
        public Ticket GetJIRA(string codigo)
        {
            Ticket ticket = new Ticket();
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("SCODE", OracleDbType.Varchar2, codigo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package3, "PRC_REA_CLIENT_TICKET"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {

                        ticket.Codigo = dr["SCODE"].ToString();
                        ticket.FecRecepcion = dr["DRECEPDATE"].ToString();
                        ticket.FecRegistro = dr["DREGDATE"].ToString();
                        ticket.ViaRecepcion = dr["VJIRA"].ToString();
                        ticket.Ramo = dr["RAMO_JIRA"].ToString();
                        ticket.Producto = dr["PRODUCTO_JIRA"].ToString();
                        ticket.Poliza = dr["POLIZA"].ToString();
                        ticket.Estado = dr["ESTADO"].ToString();
                        ticket.Contacto = dr["CONTACTO"].ToString();
                        ticket.Nombre = dr["CLIENTE"].ToString();
                        ticket.Documento = dr["NRODOC"].ToString();
                        ticket.DocumentoCli = dr["DOCUMENTOCLI"].ToString();
                        ticket.TipoDocumento = dr["NCODE_JIRA_CON"].ToString();
                        ticket.TipoDocumentoCli = dr["NCODE_JIRA_CLI"].ToString();
                        ticket.Tipo = dr["ISSUE"].ToString();
                        ticket.Ejecutivo = dr["EXECU"].ToString();
                        ticket.TipoCaso = dr["TIPCASO"].ToString();
                        ticket.Proyecto = dr["PROY"].ToString();
                        //PRODUCCION
                        ticket.Summary = codigo;
                        //CALIDAD
                        //ticket.Summary = "PRUEBA " + codigo;
                        ticket.Reporter = dr["EXECU"].ToString();
                        ticket.EmailCli = dr["MAIL"].ToString();
                        ticket.ViaRespuesta = dr["VRJIRA"].ToString();
                        ticket.Monto = dr["MONTO"].ToString();
                        ticket.SubMotivo = dr["SubTra"].ToString();
                        ticket.SubMotivoREC = dr["SubREC"].ToString();
                        ticket.SubMotivoSOL = dr["SubSOL"].ToString();
                        ticket.Motivo = dr["SCODE_JIRA_TRA"].ToString();
                        ticket.MotivoREC = dr["SCODE_JIRA_REC"].ToString();
                        ticket.MotivoSOL = dr["SCODE_JIRA_SOL"].ToString();
                        ticket.Direccion = dr["DIRECCIO"].ToString();
                        ticket.Canal = dr["CANAL_JIRA"].ToString();
                        ticket.Descripcion = dr["DESCRIPCION"].ToString();
                        ticket.Vinculo = dr["VINC_JIRA"].ToString();
                        ticket.Reconsideracion = dr["NRECON"].ToString();
                        ticket.Telefono = dr["PHONE"].ToString();
                        ticket.Email = dr["MAIL"].ToString();
                        ticket.Aplicacion = "14113";
                        ticket.Segmento = dr["SEGMENTO"].ToString();
                        ticket.ChannelProcess = "15712";
                        ticket.ProductoRentas = dr["PROD_RENTAS"].ToString();
                        ticket.TramiteRentas = dr["TRA_RENTAS"].ToString();
                        ticket.TipoPension = dr["TRA_PENSION"].ToString();//
                        ticket.TipoPrestacion = dr["TIP_PRES"].ToString();//
                        ticket.NtypeJira = dr["NTYPE_JIRA"].ToString();//
                        ticket.Cussp = dr["CUSSP"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                ticket = new Ticket();
            }
            return ticket;
        }

        public RegiTicketResponse SetReporte(ReporteRequest request)
        {
            RegiTicketResponse response = new RegiTicketResponse();
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("NYEAR", OracleDbType.Int32, request.anio, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NTRIMESTRE", OracleDbType.Int32, request.trimestre, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SUSUARIO", OracleDbType.Varchar2, request.usario, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NTIPO", OracleDbType.Int32, request.tipo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NDEFINITUVO", OracleDbType.Int32, request.definitivo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NOPERACIONES", OracleDbType.Int32, request.operaciones, ParameterDirection.Input));
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package3, "PRC_SET_NORMATIVO"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        response.Codigo = dr["SCODE"].ToString();
                    }
                }
            }
            catch (Exception)
            {
                response = new RegiTicketResponse();
            }
            if (response.Codigo == "1")
            {
                response.mensaje = "El trimestre ya tiene un reporte definitivo de este tipo.";
            }
            return response;
        }

        public RegiTicketResponse SetJIRA(Ticket ticket)
        {
            RegiTicketResponse response = new RegiTicketResponse();
            string pak = "PKG_BDU_CLIENT_360";
            if (ticket.Aplicacion == "SGC")
            {
                pak = "PKG_BDU_CLIENT_360_2";
            }
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("SCODE", OracleDbType.Varchar2, ticket.Codigo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SCODE_JIRA", OracleDbType.Varchar2, ticket.CodigoJIRA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", pak, "PRC_SET_JIRA"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        response.Codigo = dr["SCODE"].ToString();
                    }
                }
            }
            catch (Exception)
            {
                response = new RegiTicketResponse();
            }
            if (response.Codigo != "1")
            {
                response.mensaje = "Hubo error en la actualizacion.";
            }
            else
            {
                response.mensaje = "Actualizacion exitosa";
            }
            return response;
        }

        public List<ReporteRequest> GetReportes()
        {
            List<ReporteRequest> response = new List<ReporteRequest>();
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package3, "PRC_LIST_REPO"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        ReporteRequest reporte = new ReporteRequest();
                        reporte.codigo = dr["SCODE"].ToString();
                        reporte.tipo = dr["NTYPE"].ToString();
                        reporte.anio = dr["NYEAR"].ToString();
                        reporte.trimestre = dr["NTRIMESTRE"].ToString();
                        reporte.estado = dr["SSTATE"].ToString();
                        reporte.fecha = dr["DREGDATE"].ToString();
                        reporte.usario = dr["NUSER"].ToString();
                        reporte.definitivo = Int32.Parse(dr["NDEFI"].ToString());
                        response.Add(reporte);
                    }
                }
            }
            catch (Exception)
            {
                response = new List<ReporteRequest>();
            }
            return response;
        }
        public List<ReporteRequest> GetReportesTipoBusqueda(string tipoReporte, string datoBuscar, string tipoBusqueda)
        {
            List<ReporteRequest> response = new List<ReporteRequest>();
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("P_STIPO_REPORTE", OracleDbType.Varchar2, tipoReporte, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DATOBUSCAR", OracleDbType.Varchar2, datoBuscar, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_TIPOBUSQUEDA", OracleDbType.Varchar2, tipoBusqueda, ParameterDirection.Input));
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package3, "PRC_LIST_REPO_BUSQUEDA"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        ReporteRequest reporte = new ReporteRequest();
                        reporte.codigo = dr["SCODE"].ToString();
                        reporte.tipo = dr["NTYPE"].ToString();
                        reporte.anio = dr["NYEAR"].ToString();
                        reporte.trimestre = dr["NTRIMESTRE"].ToString();
                        reporte.estado = dr["SSTATE"].ToString();
                        reporte.fecha = dr["DREGDATE"].ToString();
                        reporte.usario = dr["NUSER"].ToString();
                        reporte.definitivo = Int32.Parse(dr["NDEFI"].ToString());
                        response.Add(reporte);
                    }
                }
            }
            catch (Exception)
            {
                response = new List<ReporteRequest>();
            }
            return response;
        }
        public List<RR1> GetRR1(string codigo)
        {
            List<RR1> response = new List<RR1>();

            RR1 Reporte = new RR1();
            List<RR1_Cabecera> Cabecera_Reporte = new List<RR1_Cabecera>();
            List<RR1_Detalle> Detalle_Reporte = new List<RR1_Detalle>();
            List<RR1_Detalle> Totales_Reporte = new List<RR1_Detalle>();
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("SCODE", OracleDbType.Varchar2, codigo, ParameterDirection.Input));

                parameters.Add(new OracleParameter("RC_CABECERA", OracleDbType.RefCursor, ParameterDirection.Output));
                parameters.Add(new OracleParameter("RC_DETALLE", OracleDbType.RefCursor, ParameterDirection.Output));
                parameters.Add(new OracleParameter("RC_TOTALES", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package3, "PRC_GET_DETALLE_NORMATIVO"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {

                    while (dr.Read())
                    {
                        RR1_Cabecera reporte = new RR1_Cabecera();
                        reporte.SCODE = dr["SCODE"].ToString();
                        reporte.NYEAR = dr["NYEAR"].ToString();
                        reporte.NTRIMESTRE = dr["NTRIMESTRE"].ToString();
                        reporte.TOTREC = dr["NTOTREC"].ToString();
                        reporte.TOTPROM = dr["NTOTPROM"].ToString();
                        reporte.TOTOPER = dr["NTOTOPER"].ToString();
                        //SUCAVE
                        reporte.CodigoSBSEmpresaVigilada = dr["CODSBSEMPRESA"].ToString();
                        reporte.IdFormato = dr["IDFORMATO"].ToString();
                        reporte.IdAnexo = dr["IDANEXO"].ToString();
                        reporte.CodigoExpresionMonto = dr["CODEXPRESIONMONTO"].ToString();
                        reporte.DatoControl = dr["DATOCONTROL"].ToString();
                        Cabecera_Reporte.Add(reporte);
                    }
                    dr.NextResult();
                    while (dr.Read())
                    {
                        RR1_Detalle reporte = new RR1_Detalle();
                        reporte.NVIARECEP = dr["NVIARECEP"].ToString();
                        reporte.NSUBMOTIV = dr["NSUBMOTIV"].ToString();
                        reporte.NPRODUCT = dr["NPRODUCT"].ToString();
                        reporte.SUBIGEO = dr["SUBIGEO"].ToString();
                        reporte.NOMBRE_UBIGEO = dr["SNOMBRE_UBIGEO"].ToString();
                        reporte.rev = dr["rev"].ToString();
                        reporte.NENTRAM1 = dr["NENTRAM1"].ToString();
                        reporte.NETRAM16 = dr["NETRAM16"].ToString();
                        reporte.NETRAM31 = dr["NETRAM31"].ToString();
                        reporte.NENTRAM60 = dr["NENTRAM60"].ToString();
                        reporte.NEMTRAMTOTAL = dr["NEMTRAMTOTAL"].ToString();
                        reporte.NEMRP1 = dr["NEMRP1"].ToString();
                        reporte.NEMP16 = dr["NEMP16"].ToString();
                        reporte.NEMP31 = dr["NEMP31"].ToString();
                        reporte.NEMP60 = dr["NEMP60"].ToString();
                        reporte.NEMPTOTAL = dr["NEMPTOTAL"].ToString();
                        reporte.NUSU1 = dr["NUSU1"].ToString();
                        reporte.NUSU16 = dr["NUSU16"].ToString();
                        reporte.NUSU31 = dr["NUSU31"].ToString();
                        reporte.NUSU60 = dr["NUSU60"].ToString();
                        reporte.NUSUTOTAL = dr["NUSUTOTAL"].ToString();
                        reporte.NABSTOTAL = dr["NABSTOTAL"].ToString();
                        reporte.NBSTIEMPPROM_TOTAL = dr["NBSTIEMPPROM_TOTAL"].ToString();
                        Detalle_Reporte.Add(reporte);

                        /*t.NVIARECEP,t.NSUBMOTIV, t.NPRODUCT, t.SUBIGEO, SUM(t.NREVISION) rev,
                       SUM(t.NENTRAM1) NENTRAM1, SUM(t.NETRAM16) NETRAM16, SUM(NETRAM31) NETRAM31,
                        SUM(NENTRAM60) NENTRAM60, SUM(NEMTRAMTOTAL) NEMTRAMTOTAL, SUM(NEMRP1) NEMRP1,
                        SUM(NEMP16) NEMP16, 0 NEMP31, SUM(NEMP60) NEMP60 , SUM(NEMPTOTAL) NEMPTOTAL,
                        SUM(NUSU1) NUSU1, SUM(NUSU16) NUSU16, 0 NUSU31 , SUM(NUSU60) NUSU60,
                        SUM(NUSUTOTAL) NUSUTOTAL, SUM(NABSTOTAL) NABSTOTAL*/
                    }
                    dr.NextResult();
                    while (dr.Read())
                    {
                        RR1_Detalle reporte = new RR1_Detalle();

                        reporte.NENTRAM1 = dr["NENTRAM1"].ToString();
                        reporte.NETRAM16 = dr["NETRAM16"].ToString();
                        reporte.NETRAM31 = dr["NETRAM31"].ToString();
                        reporte.NENTRAM60 = dr["NENTRAM60"].ToString();
                        reporte.NEMTRAMTOTAL = dr["NEMTRAMTOTAL"].ToString();
                        reporte.NEMRP1 = dr["NEMRP1"].ToString();
                        reporte.NEMP16 = dr["NEMP16"].ToString();
                        reporte.NEMP31 = dr["NEMP31"].ToString();
                        reporte.NEMP60 = dr["NEMP60"].ToString();
                        reporte.NEMPTOTAL = dr["NEMPTOTAL"].ToString();
                        reporte.NUSU1 = dr["NUSU1"].ToString();
                        reporte.NUSU16 = dr["NUSU16"].ToString();
                        reporte.NUSU31 = dr["NUSU31"].ToString();
                        reporte.NUSU60 = dr["NUSU60"].ToString();
                        reporte.NUSUTOTAL = dr["NUSUTOTAL"].ToString();
                        reporte.NABSTOTAL = dr["NABSTOTAL"].ToString();

                        Totales_Reporte.Add(reporte);
                    }

                }
                Reporte.Cabecera_Reporte = Cabecera_Reporte;
                Reporte.Detalle_Reporte = Detalle_Reporte;
                Reporte.Totales_Reporte = Totales_Reporte;
                response.Add(Reporte);
            }
            catch (Exception)
            {
                response = new List<RR1>();
            }
            return response;
        }
        public List<RR1_ReporteDetalleReclamos360> GetRR1_ReporteDetalleReclamos360(string codigo)
        {
            List<RR1_ReporteDetalleReclamos360> response = new List<RR1_ReporteDetalleReclamos360>();

            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("P_SREPORTE", OracleDbType.Varchar2, codigo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("RC_DETALLE", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package3, "PRC_GET_REPORTE_DETALLE_RECLAMOS_360"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        RR1_ReporteDetalleReclamos360 reporte = new RR1_ReporteDetalleReclamos360();
                        reporte.PeriodoReporteSucave = dr["ntrimestre"].ToString();
                        reporte.NumeroTicket360 = dr["scode"].ToString();
                        reporte.NumeroTicketJira = dr["scode_jira"].ToString();
                        reporte.DNICliente = dr["clientdni"].ToString();
                        reporte.NombreCompletoCliente = dr["clientname"].ToString();
                        reporte.DNIContacto = dr["contactdni"].ToString();
                        reporte.NombreCompletoContacto = dr["contactname"].ToString();
                        reporte.Reconsideracion = dr["nrecon"].ToString();
                        reporte.DescripcionUbigeoProvincia = dr["desUbigeo"].ToString();
                        reporte.DireccionTicket = dr["sdireccion"].ToString();
                        reporte.DescripcionTicket = dr["sdescriptTick"].ToString();
                        reporte.CodigoProductoSBS = dr["productSBScode"].ToString();
                        reporte.DescripcionProductoSBS = dr["productSBSsdescript"].ToString();

                        reporte.CodigoMotivoSBS = dr["motivSBScode"].ToString();
                        reporte.DescripcionMotivoSBS = dr["motivSBSsdescript"].ToString();
                        reporte.DescripcionMotivo360 = dr["desMotiv360"].ToString();
                        reporte.DescripcionSubMotivo360 = dr["desSubmotiv360"].ToString();
                        reporte.DescripcionCanalSBS = dr["desCanalSBS"].ToString();

                        reporte.DescripcionMesRecepcion = dr["recepMonth"].ToString();
                        reporte.FechaRecepcionTicket = dr["drecepdate"].ToString();
                        reporte.FechaRegistroTicket = dr["dregdateTick"].ToString();
                        reporte.DescripcionEstadoPlazo = dr["desEstadoPlazo"].ToString();
                        reporte.NombreUsuarioCierraReclamo = dr["usuarioenvio"].ToString();
                        reporte.FechaEnvio = dr["denvio"].ToString();

                        reporte.DescripcionMesFechaEnvio = dr["desenvioMonth"].ToString();
                        reporte.NumeroDiasReclamo = dr["ndias"].ToString();
                        reporte.DescipcionEstadoAbsolucion = dr["desAbsol"].ToString();
                        reporte.DescipcionViaRespuesta = dr["desViaresp"].ToString();
                        response.Add(reporte);
                    }

                }

            }
            catch (Exception)
            {
                response = new List<RR1_ReporteDetalleReclamos360>();
            }
            return response;
        }

        public List<RR3> GetRR3(string codigo)
        {
            List<RR3> response = new List<RR3>();

            RR3 Reporte = new RR3();
            List<RR3_Cabecera> Cabecera_Reporte = new List<RR3_Cabecera>();
            List<RR3_Detalle> Detalle_Reporte = new List<RR3_Detalle>();
            List<RR3_Detalle> Totales_Reporte = new List<RR3_Detalle>();
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("SCODE", OracleDbType.Varchar2, codigo, ParameterDirection.Input));

                parameters.Add(new OracleParameter("RC_CABECERA", OracleDbType.RefCursor, ParameterDirection.Output));
                parameters.Add(new OracleParameter("RC_DETALLE", OracleDbType.RefCursor, ParameterDirection.Output));
                parameters.Add(new OracleParameter("RC_TOTALES", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package3, "PRC_GET_DETALLE_NORMATIVO_RR3"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {

                    while (dr.Read())
                    {
                        RR3_Cabecera reporte = new RR3_Cabecera();
                        reporte.SCODE = dr["SCODE"].ToString();
                        reporte.NYEAR = dr["NYEAR"].ToString();
                        reporte.NTRIMESTRE = dr["NTRIMESTRE"].ToString();
                        //SUCAVE
                        reporte.CodigoSBSEmpresaVigilada = dr["CODSBSEMPRESA"].ToString();
                        reporte.IdFormato = dr["IDFORMATO"].ToString();
                        reporte.IdAnexo = dr["IDANEXO"].ToString();
                        reporte.CodigoExpresionMonto = dr["CODEXPRESIONMONTO"].ToString();
                        reporte.DatoControl = dr["DATOCONTROL"].ToString();
                        Cabecera_Reporte.Add(reporte);
                    }
                    dr.NextResult();
                    while (dr.Read())
                    {
                        RR3_Detalle reporte = new RR3_Detalle();
                        reporte.NCANAL_SBS = dr["NCANAL_SBS"].ToString();
                        reporte.NPRODUCT_SBS = dr["NPRODUCT_SBS"].ToString();
                        reporte.SDESCRIPT_PRODUCT_NOTIP = dr["SDESCRIPT_PRODUCT_NOTIP"].ToString();
                        reporte.NMOTIVO_SBS = dr["NMOTIVO_SBS"].ToString();
                        reporte.SDESCRIPT_MOTIV_NOTIP = dr["SDESCRIPT_MOTIV_NOTIP"].ToString();
                        reporte.NCANTIDAD = dr["NCANTIDAD"].ToString();
                        Detalle_Reporte.Add(reporte);
                    }
                    dr.NextResult();
                    while (dr.Read())
                    {
                        RR3_Detalle reporte = new RR3_Detalle();

                        reporte.NCANTIDAD = dr["NCANTIDAD"].ToString();
                        Totales_Reporte.Add(reporte);
                    }

                }
                Reporte.Cabecera_Reporte = Cabecera_Reporte;
                Reporte.Detalle_Reporte = Detalle_Reporte;
                Reporte.Totales_Reporte = Totales_Reporte;
                response.Add(Reporte);
            }
            catch (Exception)
            {
                response = new List<RR3>();
            }
            return response;
        }

        string ITicketRepository.CreaReporte(ParamReporte request)
        {
            String response = string.Empty;
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("NYEAR", OracleDbType.Int32, request.year, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NNMESINI", OracleDbType.Int32, request.mesini, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NMESFIN", OracleDbType.Int32, request.mesfin, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NYEAR_ANT", OracleDbType.Int32, request.year_ant, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NMESINI_ANT", OracleDbType.Int32, request.mesini_ant, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NMESFIN_ANT", OracleDbType.Int32, request.mesfin_ant, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SREPORTE", OracleDbType.Varchar2, request.scode, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NTIPOREPORTE", OracleDbType.Int32, request.tipoReporte, ParameterDirection.Input));
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package3, "PRC_CRE_NORMATIVO"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        response = dr["RESP"].ToString();
                    }
                }
            }
            catch (Exception)
            {
                response = string.Empty;
            }
            return response;
        }
        ReporteDona ITicketRepository.GetDonas()
        {
            ReporteDona hi = new ReporteDona();
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package3, "PRC_REPO_DONUT"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        hi.MES = dr["MES"].ToString();
                        hi.REGREC = Int32.Parse(dr["REGREC"].ToString());
                        hi.CERRREC = Int32.Parse(dr["CERRREC"].ToString());
                        hi.REGSOL = Int32.Parse(dr["REGSOL"].ToString());
                        hi.CERRSOL = Int32.Parse(dr["CERRSOL"].ToString());
                        hi.REGCON = Int32.Parse(dr["REGCON"].ToString());
                        hi.CERRCON = Int32.Parse(dr["CERRCON"].ToString());
                        hi.REGTRA = Int32.Parse(dr["REGTRA"].ToString());
                        hi.CERRTRA = Int32.Parse(dr["CERRTRA"].ToString());
                    }
                }
            }
            catch (Exception)
            {
                hi = new ReporteDona();
            }
            return hi;
        }

        public RegiTicketResponse RegistraTicketJIRA(TicketDinamico request, string type)
        {
            string bass = AppSettings.AWSRegistrar;
            string token = new TokenService().getTokenAWS(type);
            var result = PostRequest(bass, AppSettings.AWSRegistrar2, request, token);
            ID res = JsonConvert.DeserializeObject<ID>(result);
            //PRODUCCION
            RegiTicketResponse response = new RegiTicketResponse { Codigo = res.id, respuesta = true, mensaje = "Registro exitoso" };
            //CALIDAD
            // RegiTicketResponse response = new RegiTicketResponse { Codigo = "RYS-440", respuesta = true, mensaje = "Registro exitoso" };

            return response;
        }
        public async Task<List<TicketSGC>> ListaJIRAAWS(String proy, String codigo, string token = null)
        {

            string bass = AppSettings.AWSConsultar;
            if (token == null)
            {
                return new List<TicketSGC>();
            }
            var result = await GetRequest(bass, AppSettings.AWSConsultar2 + proy + ") AND key in (" + codigo + ")", token);
            string json = result.result.ToString();
            string value = "\"id\" : \"" + proy;
            List<int> indexes = AllIndexesOf(json, value);
            if (indexes.Count > 0)
            {
                indexes.RemoveAt(0);
            }

            StringBuilder sb = new StringBuilder(json.Length + indexes.Count);
            sb.Append(json);
            int pos = 0;
            foreach (int x in indexes)
            {
                sb = sb.Insert(x - 4 + pos, ",");
                pos++;
            }
            String resultado = sb.ToString();
            List<TicketSGC> lista = new List<TicketSGC>();
            if (sb.Length > 20)
            {
                List<ResponseListaJIRA> response = JsonConvert.DeserializeObject<List<ResponseListaJIRA>>(resultado);
                foreach (ResponseListaJIRA jira in response)
                {
                    TicketSGC ticket = new TicketSGC();
                    ticket.CodigoJIRA = jira.id;
                    foreach (Campo c in jira.fields)
                    {
                        if (c.id == "status")
                        {
                            var valor = JObject.Parse(c.value.ToString());
                            ticket.estado = GetValue<string>(valor, "name");
                        }
                    }
                    lista.Add(ticket);
                }
            }
            return lista;
        }
        public async Task<string> GetS3Adjunto(string codigo, string type)
        {
            List<Ticket> lista = new List<Ticket>();
            string token = new TokenService().getTokenAWS(type);
            string bass = AppSettings.AWSGetAdjunto;

            var result = await GetRequestRedirect(bass, codigo, token, 2);

            byte[] bytes = result.bytes;
            string enconded = Convert.ToBase64String(bytes);



            return enconded;
        }


        public Archivo S3Adjuntar(Archivo ar, string type, string customfield)
        {
            //string base6 = "";
            var client = new RestClient(AppSettings.AWSAdjuntar + ar.name + "&time=" + Codificatiempo() + "&field=" + customfield);
            string responsed = "";
            string token = new TokenService().getTokenAWS(type);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer " + token);
            request.AddHeader("Content-Type", ar.mime);
            request.AddParameter(ar.name, Convert.FromBase64String(ar.content), ParameterType.RequestBody);
            //request.AddFile(ar.name, Convert.FromBase64String(ar.content), ar.name, ar.mime);
            // request.AddFile("btn_flecha_der_hover-SOL-157", @"D:\Descargas\Archivos\btn_flecha_der_hover.jpg");
            try
            {
                IRestResponse response = client.Execute(request);
                ID respuesta = JsonConvert.DeserializeObject<ID>(response.Content);

                //PRODUCCION
                ar.path_gd = respuesta.id;

            }
            catch (Exception ex)
            {
                responsed = ex.Message;
            }
            //CALIDAD
            // ar.path_gd = "PRUEBA DOCUMENTO";
            return ar;
        }
        public async Task<Archivo> S3AdjuntarAsync(Archivo ar, string type, string customfield)
        {
            var client = new RestClient(AppSettings.AWSAdjuntar + ar.name + "&time=" + Codificatiempo() + "&field=" + customfield);
            string token = new TokenService().getTokenAWS(type);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer " + token);
            request.AddHeader("Content-Type", ar.mime);
            request.AddParameter(ar.name, Convert.FromBase64String(ar.content), ParameterType.RequestBody);
            try
            {
                IRestResponse response = await client.ExecuteAsync(request);
                ID respuesta = JsonConvert.DeserializeObject<ID>(response.Content);
                ar.path_gd = respuesta.id;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ar;
        }
        List<EstructuraTicket> ITicketRepository.GetEstructura(string Tipo)
        {

            List<EstructuraTicket> response = new List<EstructuraTicket>();
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("TIPO", OracleDbType.Varchar2, Tipo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package3, "PRC_GET_ESTRUCTURAJIRA"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        EstructuraTicket est = new EstructuraTicket();
                        est.SCODE_JIRA = dr["SCODE_JIRA"].ToString();
                        est.SDESCRIPT = dr["SDESCRIPT"].ToString();
                        est.STYPE = dr["STYPE"].ToString();
                        est.NREQUIRED = dr["NREQUIRED"].ToString();
                        response.Add(est);

                    }
                }
            }
            catch (Exception)
            {
                response = new List<EstructuraTicket>();
            }
            return response;
        }
        public async Task<Ticket> ConsultaTicketJIRA(string codigo, string token, string type)
        {
            string proy = codigo.Substring(0, 2);
            if (proy != "OV")
            {
                proy = codigo.Substring(0, 3);
            }
            Ticket lista = new Ticket();
            string bass = AppSettings.AWSConsultar;

            try
            {
                var result = await GetRequest(bass, AppSettings.AWSConsultar2 + proy + ") AND key=" + codigo, token);

                List<ResponseListaJIRA> response = JsonConvert.DeserializeObject<List<ResponseListaJIRA>>(result.result);
                ResponseListaJIRA ticket = response?[0];
                foreach (Campo c in ticket.fields)
                {
                    if (c.id == "status")
                    {
                        var valor = JObject.Parse(c.value.ToString());
                        lista.Estado = GetValue<string>(valor, "name");
                    }
                    if (c.id == "customfield_12900")
                    {
                        if (c.value != null)
                        {
                            var valor = JObject.Parse(c.value.ToString());
                            lista.Absolucion = GetValue<string>(valor, "value");
                        }
                    }
                    if (c.id == "customfield_12813")
                    {
                        if (c.value != null)
                        {
                            var valor = c.value.ToString();
                            string[] d = valor.Split(",");
                            List<string> Docs = new List<string>();
                            Docs.AddRange(d);
                            lista.sustentatorios = Docs;
                        }
                    }
                    if (c.id == "customfield_12816")
                    {
                        if (c.value != null)
                        {
                            var valor = c.value.ToString();
                            string[] d = valor.Split(",");
                            List<string> Docs = new List<string>();
                            Docs.AddRange(d);
                            lista.respuestasoluciones = Docs;
                        }
                    }
                    if (c.id == "customfield_12817")
                    {
                        if (c.value != null)
                        {
                            var valor = c.value.ToString();
                            string[] d = valor.Split(",");
                            List<string> Docs = new List<string>();
                            Docs.AddRange(d);
                            lista.comprobantes = Docs;
                        }
                    }
                    if (c.id == "customfield_12815")
                    {
                        if (c.value != null)
                        {
                            try
                            {
                                //    c.value = "Respuesta1609966604811customfield_12815_mrobles.docx";
                                var valor = c.value.ToString();
                                string doc = valor;
                                string[] d = doc.Split(",");
                                List<string> Docs = new List<string>();
                                Docs.AddRange(d);

                                lista.respuestaderivacion = Docs;
                            }
                            catch (Exception ex)
                            {
                                WriteErrorLog(ex, "En customfield_12815");
                            }
                        }
                    }
                    if (c.id == "customfield_12814")
                    {
                        if (c.value != null)
                        {
                            lista.Carta = c.value.ToString();
                        }
                    }
                    if (c.id == "customfield_12319")
                    {
                        if (c.value != null)
                        {
                            var sd = c.value.ToString().Split('T');
                            lista.FechaCierre = sd[0];
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ex.HelpLink = ex.HelpLink + "error en ConsultaTicketJIRA" + ex.Message;
                throw ex;
            }


            return lista;
        }

        public string Codificatiempo()
        {
            DateTime tiempo = DateTime.Now;
            return tiempo.Year.ToString() + tiempo.Month.ToString() + tiempo.Day.ToString() + tiempo.Hour.ToString() + tiempo.Minute.ToString() + tiempo.Millisecond.ToString();
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
        public string PostRequest(string baseUrl, string url, object postObject, string token = null)
        {
            string result = null;
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                UriBuilder builder = new UriBuilder(baseUrl + url);
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();
                    if (token != null)
                    {
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    }
                    string json = JsonConvert.SerializeObject(postObject);
                    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                    var response = client.PostAsync(builder.Uri, stringContent).Result;

                    result = response.Content.ReadAsStringAsync().Result; //JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
                }
            }
            catch (Exception)
            {
                result = "";
            }

            return result;
        }
        public async Task<HttpResponseMessage> PutRequest(string baseUrl, string url, object postObject, string token = null)
        {
            //string result = null;
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                UriBuilder builder = new UriBuilder(baseUrl + url);
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();
                    if (token != null)
                    {
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    }
                    string json = JsonConvert.SerializeObject(postObject);
                    var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                    response = await client.PutAsync(builder.Uri, stringContent);

                    //result = await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return response;
        }
        //
        public async Task<GetResponse> GetRequestRedirect(string baseUrl, string url, string token = null, int tipo = 1)
        {
            GetResponse result = new GetResponse();

            try
            {

                UriBuilder builder = new UriBuilder(baseUrl + url);
                string cadena = builder.Uri.ToString();
                HttpClientHandler handler = new HttpClientHandler()
                {
                    AllowAutoRedirect = true
                };
                using (var client = new HttpClient(handler))
                {

                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    client.DefaultRequestHeaders.Add("Accept", "*/*");

                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 |
                                     SecurityProtocolType.Tls11 |
                                      SecurityProtocolType.Tls;

                    HttpResponseMessage response = await client.GetAsync(builder.Uri);

                    int stCode = (int)response.StatusCode;

                    if (stCode == 403)
                    {

                        HttpResponseMessage responseRedirect = await client.GetAsync(response.RequestMessage.RequestUri);

                        responseRedirect.EnsureSuccessStatusCode();

                        result.result = responseRedirect.ToString();

                        if (tipo == 1)
                        {
                            result.result = responseRedirect.Content.ReadAsStringAsync().Result;
                        }
                        else
                        {
                            result.bytes = responseRedirect.Content.ReadAsByteArrayAsync().Result;
                        }
                        client.Dispose();
                    };

                }
                handler.Dispose();

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return result;
        }
        //

        public async Task<GetResponse> GetRequest(string baseUrl, string url, string token = null, int tipo = 1)
        {
            GetResponse result = new GetResponse();
            //string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt";
            //StreamWriter sw = null;
            //sw = new StreamWriter(filepath, true);
            try
            {
                //                ServicePointManager.ServerCertificateValidationCallback =
                //delegate (object s, X509Certificate certificate,
                //X509Chain chain, SslPolicyErrors sslPolicyErrors)
                //{ return true; };
                UriBuilder builder = new UriBuilder(baseUrl + url);
                string cadena = builder.Uri.ToString();
                HttpClientHandler handler = new HttpClientHandler();


                using (var client = new HttpClient(handler))
                {
                    //HttpClientHandler clientHandler = new HttpClientHandler();
                    //clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    client.DefaultRequestHeaders.Add("Accept", "*/*");

                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 |
                                     SecurityProtocolType.Tls11 |
                                      SecurityProtocolType.Tls;

                    HttpResponseMessage response = await client.GetAsync(builder.Uri);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    result.result = response.ToString();
                    if (tipo == 1)
                    {
                        result.result = response.Content.ReadAsStringAsync().Result;//JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
                    }
                    else
                    {
                        result.bytes = response.Content.ReadAsByteArrayAsync().Result;
                    }
                    client.Dispose();
                }
                handler.Dispose();

            }
            catch (Exception ex)
            {
                //sw.WriteLine(DateTime.Now.ToString() + "- GetRequest: " + ex.ToString());
                throw ex;
            }
            //finally
            //{
            //sw.Flush();
            //sw.Close();
            //}

            return result;
        }
        public async Task<GetResponse> GetRequest2(string baseUrl, string url, string token = null, int tipo = 1)
        {
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\LogFile.txt";
            StreamWriter sw = null;
            sw = new StreamWriter(filepath, true);
            GetResponse result = new GetResponse();
            try
            {
                if (token != null)
                {
                    string response = string.Empty;
                    UriBuilder builder = new UriBuilder(baseUrl + url);
                    HttpWebRequest request;
                    request = WebRequest.Create(builder.Uri) as HttpWebRequest;
                    request.Method = "GET";
                    request.Headers = new WebHeaderCollection();
                    request.ContentType = "application/json; charset=utf-8";
                    request.Proxy = new WebProxy() { UseDefaultCredentials = false };
                    request.PreAuthenticate = true;
                    request.Headers.Add("Authorization", "Bearer " + token);
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateServerCertificate);

                    HttpWebResponse respuesta = request.GetResponse() as HttpWebResponse;
                    StreamReader read = new StreamReader(respuesta.GetResponseStream(), Encoding.UTF8);
                    response = read.ReadToEnd();
                    result.result = response;
                }
                result.result = "";

            }
            catch (Exception ex)
            {
                sw.WriteLine(DateTime.Now.ToString() + "- GetRequest: " + ex.ToString());
                throw ex;
            }
            finally
            {

                sw.Flush();
                sw.Close();
            }
            return result;

        }
        public async Task<GetResponse> GetRequest3(string baseUrl, string url, string token = null, int tipo = 1)
        {
            GetResponse result = new GetResponse();
            try
            {
                UriBuilder builder = new UriBuilder(baseUrl + url);
                var client = new RestClient(builder.Uri);
                var request = new RestRequest(Method.GET);
                //request.AddHeader("postman-token", "5f93c79b-b055-db5d-ee86-b23b7799f77a");
                //request.AddHeader("cache-control", "no-cache");
                request.AddHeader("authorization", "Bearer " + token);
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json", "", ParameterType.RequestBody);
                IRestResponse response = await client.ExecuteAsync(request);
                result.result = response.Content;
            }
            catch (Exception ex)
            {
                ex.HelpLink = ex.HelpLink + "error en GetRequest3" + ex.Message;
                throw ex;
            }
            return result;
        }


        public List<EstructuraTicket> GetEstructura(string Tipo)
        {
            List<EstructuraTicket> lista = new List<EstructuraTicket>();
            return lista;
        }
        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }


        public static T GetValue<T>(JToken jToken, string key, T defaultValue = default(T))
        {
            dynamic ret = jToken[key];
            if (ret == null)
            {
                return defaultValue;
            }

            if (ret is JObject)
            {
                return JsonConvert.DeserializeObject<T>(ret.ToString());
            }

            return (T)ret;
        }
        public string EnviarMail(Ticket ticketC, List<string> archivos)
        {

            try
            {
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                using (var mail = new MailMessage())
                {
                    mail.From = new MailAddress("daniel.granados@materiagris.pe");
                    //mail.To.Add("lloza@protectasecurity.pe");
                    //mail.To.Add("mrobles@protectasecurity.pe");
                    //mail.To.Add("dmejia@protectasecurity.pe");
                    //mail.To.Add("miguel.fernandez@materiagris.pe");
                    //mail.To.Add("luz.gonzales@materiagris.pe");

                    mail.To.Add("academicsaucer17@gmail.com");

                    string tipo = ticketC.Codigo.Substring(0, 3);
                    string ticket = string.Empty;
                    string htmlBody = string.Empty;
                    string clave = "Bomba2013";
                    if (tipo == "REC")
                    {
                        ticket = "Reclamo";
                        htmlBody = System.IO.File.ReadAllText("/Mail/Reclamo.html");
                        var arr = htmlBody.Split('~');
                        htmlBody = arr[0] + ticketC.Codigo + arr[1];
                    }
                    else if (tipo == "SOL")
                    {
                        ticket = "Solicitud";
                        htmlBody = System.IO.File.ReadAllText("/Mail/Solicitud.html");
                        htmlBody = htmlBody.Replace("[Numero]", ticketC.Codigo);
                    }
                    else if (tipo == "TRA")
                    {

                        if (ticketC.Estado == "Cerrado360")
                        {
                            ticket = "Trámite y cierre ";
                            htmlBody = System.IO.File.ReadAllText("/Mail/AtencionS.html");
                            htmlBody = htmlBody.Replace("XXXX", ticketC.Codigo);
                        }
                        else
                        {
                            ticket = "Trámite";
                            htmlBody = System.IO.File.ReadAllText("/Mail/Requerimiento.html");
                            var arr = htmlBody.Split('~');
                            htmlBody = arr[0] + ticketC.Codigo + arr[1];
                            htmlBody = htmlBody.Replace("[Fecha]", ticketC.FecRecepcion.Substring(0, 10));
                            htmlBody = htmlBody.Replace("[ViaRecep]", ticketC.ViaRecepcion);
                            htmlBody = htmlBody.Replace("[SubMotivo]", ticketC.SubMotivo);
                            htmlBody = htmlBody.Replace("XX", "15");
                        }
                    }
                    else
                    {
                        ticket = "Consulta";
                        htmlBody = System.IO.File.ReadAllText("/Mail/Solicitud.html");
                        htmlBody.Replace("[Numero]", ticketC.Codigo);
                    }
                    mail.Subject = "Recepción de " + ticket + " " + ticketC.Codigo;
                    mail.IsBodyHtml = true;



                    mail.Body = htmlBody;
                    System.Net.Mail.Attachment attachment;
                    foreach (string arch in archivos)
                    {
                        attachment = new System.Net.Mail.Attachment(arch);
                        mail.Attachments.Add(attachment);

                    }

                    SmtpServer.Port = 587;
                    SmtpServer.Credentials = new System.Net.NetworkCredential("daniel.granados@materiagris.pe", clave);
                    SmtpServer.EnableSsl = true;

                    SmtpServer.Send(mail);
                }

            }
            catch (Exception ex)
            {
                var inn = ex.InnerException;
                string mess = "";
                if (inn != null)
                {
                    mess = ex.InnerException.ToString();
                }
                WriteErrorLog(ex, mess + "Loc : mail2");
                string respuesta = ex.Message;
            }
            return string.Empty;
        }
        public string GenerateReclamo(Ticket ticket)
        {
            GenerateResponse response = new GenerateResponse();

            string bass = AppSettings.ServicioWebGCliente + "api/Report/";
            // string bass = @"http://10.10.1.56/WSGClientesDesarrollo/api/Report/";
            var result = PostRequest(bass, "CreateReclamo", ticket);
            response = JsonConvert.DeserializeObject<GenerateResponse>(result);

            return response.data.ToString();
        }
        public string GenerateSolicitud(Ticket ticket)
        {
            GenerateResponse response = new GenerateResponse();
            string bass = AppSettings.ServicioWebGCliente + "api/Report/";
            var result = PostRequest(bass, "CreateSolicitud", ticket);
            response = JsonConvert.DeserializeObject<GenerateResponse>(result);

            return response.data.ToString();
        }


        public TicketSGC SetTicketSGC(TicketSGC request)
        {
            TicketSGC ticket = new TicketSGC();
            var comm = request.CommercialGroup.Split(";");
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("TIPO", OracleDbType.Varchar2, request.Tipo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NTIPDOC", OracleDbType.Varchar2, request.tipoDocClient, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SDOCUME", OracleDbType.Varchar2, request.docClient, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NBRANCH", OracleDbType.Varchar2, request.Ramo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NPRODUCT", OracleDbType.Varchar2, request.Producto, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NCONTRACTORTYPE", OracleDbType.Varchar2, request.ContracterType, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NCURRENCYTYPE", OracleDbType.Varchar2, request.currencyType, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NPOLICYTYPE", OracleDbType.Varchar2, request.PolicyType, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NSALESCHANNEL", OracleDbType.Varchar2, request.salesChannel, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SINTERMEDIARY", OracleDbType.Varchar2, request.intermediary, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NTIPOFACT", OracleDbType.Varchar2, request.BillType, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NSOLCREDIT", OracleDbType.Varchar2, request.creditRequest, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NSOLASSIST", OracleDbType.Varchar2, request.assistanceRequest, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SMARKETER", OracleDbType.Varchar2, request.marketer, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SDESCRIPT", OracleDbType.Varchar2, request.Descripcion, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NGROUP", OracleDbType.Varchar2, comm[0], ParameterDirection.Input));
                parameters.Add(new OracleParameter("NUSERCODE", OracleDbType.Int32, request.usuario, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SNAME", OracleDbType.Varchar2, request.NameClient, ParameterDirection.Input));
                //DEV CY MG 18/05/2022 - INI
                parameters.Add(new OracleParameter("SCONTACT", OracleDbType.Varchar2, request.other, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SCONTACTEMAIL", OracleDbType.Varchar2, request.Email, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SFACTORIGEN", OracleDbType.Varchar2, request.FacturaOrigen, ParameterDirection.Input));
                //DEV CY MG 18/05/2022 - FIN
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", "PKG_BDU_CLIENT_360_2", "PRC_SET_TICKETSSGC"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {

                        ticket.Codigo = dr["STICKET"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex, "Catch");
            }
            return ticket;
        }
        //DEV EC - INICIO
        public TicketSGC SetTicketSolSGC(TicketSGC request)
        {

            TicketSGC ticket = new TicketSGC();
            var comm = request.CommercialGroup.Split(";");
            try
            {

                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("TIPO", OracleDbType.Varchar2, request.Tipo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NTIPDOC", OracleDbType.Varchar2, request.TipoDocumentoCli, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SDOCUME", OracleDbType.Varchar2, request.DocumentoCli, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NBRANCH", OracleDbType.Varchar2, request.Ramo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NPRODUCT", OracleDbType.Varchar2, request.Producto, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NCONTRACTORTYPE", OracleDbType.Varchar2, request.ContracterType, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NCURRENCYTYPE", OracleDbType.Varchar2, request.currencyType, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NPOLICYTYPE", OracleDbType.Varchar2, request.PolicyType, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NSALESCHANNEL", OracleDbType.Varchar2, request.salesChannel, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SINTERMEDIARY", OracleDbType.Varchar2, request.intermediary, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NTIPOFACT", OracleDbType.Varchar2, request.BillType, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NSOLCREDIT", OracleDbType.Varchar2, request.creditRequest, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NSOLASSIST", OracleDbType.Varchar2, request.assistanceRequest, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SMARKETER", OracleDbType.Varchar2, request.marketer, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SDESCRIPT", OracleDbType.Varchar2, request.Descripcion, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NGROUP", OracleDbType.Varchar2, comm[0], ParameterDirection.Input));
                parameters.Add(new OracleParameter("NUSERCODE", OracleDbType.Int32, request.usuario, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SNAME", OracleDbType.Varchar2, request.Nombre, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NTRAMITECHANNEL", OracleDbType.Varchar2, request.TramInCant, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SPOLICYNUMBER", OracleDbType.Varchar2, request.Poliza, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NSEGMENT", OracleDbType.Varchar2, request.Segmento, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NIDSTATE", OracleDbType.Varchar2, request.estado, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SFACTORIGEN", OracleDbType.Varchar2, request.FacturaOrigen, ParameterDirection.Input));
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", "PKG_BDU_CLIENT_360_2", "PRC_SET_TICKETSSOLSGC"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        
                        ticket.Codigo = dr["STICKET"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex, "Catch");
            }
            return ticket;
        }

        public ResponseViewModel SetPayDataSolSGC(TicketSGC request)
        {
            ResponseViewModel response = new ResponseViewModel();

            var comm = request.CommercialGroup.Split(";");
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("SCODE", OracleDbType.Varchar2, request.Codigo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NTRAMITE_COBRAN", OracleDbType.Varchar2, request.CobranTramite, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SNOTA_CRED", OracleDbType.Varchar2, request.CreditNote, ParameterDirection.Input));
                parameters.Add(new OracleParameter("STITULAR", OracleDbType.Varchar2, request.TitularCuenta, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NTIPDOC_TITULAR", OracleDbType.Varchar2, request.DocTypeTitularCuenta, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SDOC_TITULAR", OracleDbType.Varchar2, request.NumDocTitularCuenta, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NBANCO", OracleDbType.Varchar2, request.Bank, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NMONEDA", OracleDbType.Varchar2, request.Currency, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SMONTO", OracleDbType.Varchar2, request.Ammount, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NTIPO_CUENTA", OracleDbType.Varchar2, request.AccountType, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SCUENTA_DEST", OracleDbType.Varchar2, request.DestAccount, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SCOMENTA_PAGO", OracleDbType.Varchar2, request.PayCommentary, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NID_GROUP", OracleDbType.Varchar2, comm[0], ParameterDirection.Input));
                parameters.Add(new OracleParameter("NCANAL_INGRESO", OracleDbType.Varchar2, request.TramInCant, ParameterDirection.Input));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;

                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                using (OracleDataReader dr2 = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", "PKG_BDU_CLIENT_360_2", "PRC_SET_PAYDATASOLSGC"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    response.code = int.Parse(P_NCODE.Value.ToString());
                    response.message = P_SMESSAGE.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
                response.code = 1;
                response.message = ex.Message;
            }
            return response;

        }

        //DEV EC - FIN

        public TicketSGC GetTicketSGC(string codigo)
        {
            TicketSGC ticket = new TicketSGC();
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("SCODE", OracleDbType.Varchar2, codigo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", "PKG_BDU_CLIENT_360_2", "PRC_GET_TICKETSSGC"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {

                        ticket.Codigo = dr["SCODE"].ToString();
                        ticket.Tipo = Int32.Parse(dr["NTIPO"].ToString());
                        if (ticket.Tipo == 100)
                        {
                            ticket.Tipo = 10806;
                            ticket.Proyecto = "GCS";
                        }
                        else
                        {

                            ticket.Tipo = 11000;
                            ticket.Proyecto = "OV";
                        }
                        ticket.tipoDocClient = dr["NTIPDOC"].ToString();
                        ticket.docClient = dr["SDOCUME"].ToString();
                        ticket.Ramo = dr["NBRANCH"].ToString();
                        ticket.Producto = Int32.Parse(dr["NPRODUCT"].ToString());
                        ticket.ContracterType = dr["NCONTRACTORTYPE"].ToString();
                        ticket.currencyType = dr["NCURRENCYTYPE"].ToString();
                        ticket.PolicyType = dr["NPOLICYTYPE"].ToString();
                        ticket.salesChannel = dr["NSALESCHANNEL"].ToString();
                        ticket.BillType = dr["NTIPOFACT"].ToString();
                        ticket.creditRequest = Int32.Parse(dr["NSOLCREDIT"].ToString());
                        ticket.assistanceRequest = Int32.Parse(dr["NSOLASSIST"].ToString());
                        ticket.marketer = dr["SMARKETER"].ToString();
                        ticket.Descripcion = dr["SDESCRIPT"].ToString();
                        //DEV CY MG 18/05/2022 - INI
                        ticket.other = dr["CONTACTO"].ToString();
                        ticket.Email = dr["CONTACTOEMAIL"].ToString();
                        //DEV CY MG 18/05/2022 - FIN
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex, "Catch");
            }
            return ticket;

        }

        public List<TicketSGC> ListTicketSGC(SGCRequest request)
        {
            List<TicketSGC> lista = new List<TicketSGC>();

            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("P_USERCODE", OracleDbType.Int32, request.usuario, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_COMMGROUP", OracleDbType.Int32, request.grupoComercial, ParameterDirection.Input));
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", "PKG_BDU_CLIENT_360_2", "PRC_LIST_TICKETSSGC"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        TicketSGC ticket = new TicketSGC();
                        ticket.Codigo = dr["SCODE"].ToString();
                        ticket.CodigoJIRA = dr["SCODE_JIRA"].ToString();
                        ticket.stipo = dr["TIPO"].ToString();
                        ticket.marketer = dr["SNAME"].ToString();
                        ticket.NameClient = dr["NOMBRE"].ToString();
                        ticket.Fecha = dr["Fecha"].ToString();
                        string proy = "OV";
                        if (proy != ticket.CodigoJIRA.Substring(0, 2))
                        {
                            proy = "GCS";
                        }
                        ticket.url = @"https://soporte.protectasecurity.pe/projects/" + proy + @"/issues/" + ticket.CodigoJIRA;
                        lista.Add(ticket);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex, "Catch");
            }

            return lista;
        }

        //Listado con PlataformaDigital
        public async Task<List<TicketSGC>> ListTicketOVPDGSGC(SGCRequest request)
        {
            List<TicketSGC> lista = new List<TicketSGC>();

            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("P_USERCODE", OracleDbType.Int32, request.usuario, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_COMMGROUP", OracleDbType.Int32, request.grupoComercial, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SEARCH", OracleDbType.Varchar2, request.search, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_TYPE", OracleDbType.Varchar2, request.type, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NPAGE_INDEX", OracleDbType.Int32, request.PageIndex, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NPAGE_SIZE", OracleDbType.Int32, request.PageSize, ParameterDirection.Input));
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)await _connectionBase.ExecuteByStoredProcedureVTAsync(string.Format("{0}.{1}", "PKG_BDU_CLIENT_360_2", "PRC_LIST_TICKETSOVPDGSGC2"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    // lista.ListTicketResponse = new List<ListTicketResponse>();
                    // foreach (var item in dr.ReadRowsList<ListTicketResponse>()){
                    //     TicketSGC ticket = new TicketSGC();
                    //     ticket.Codigo = dr["SCODE"].ToString();
                    //     ticket.CodigoJIRA = dr["SCODE_JIRA"].ToString();
                    //     ticket.stipo = dr["TIPO"].ToString();
                    //     ticket.marketer = dr["SNAME"].ToString();
                    //     ticket.NameClient = dr["NOMBRE"].ToString();
                    //     ticket.Fecha = dr["Fecha"].ToString();
                    //     //string proy = "OV";
                    //     ticket.url = dr["URL"].ToString();
                    //     /*if (proy != ticket.CodigoJIRA.Substring(0, 2))
                    //     {
                    //         if(ticket.CodigoJIRA.Substring(0, 2) = "GC"){
                    //             proy = "GCS";
                    //         }
                    //         else{
                    //             proy = "PDG";
                    //         }
                    //     }
                    //     ticket.url = url+ proy + @"/issues/" + ticket.CodigoJIRA;*/

                    //     lista.Add(ticket);
                    // }

                    while (dr.Read())
                    {
                        TicketSGC ticket = new TicketSGC();
                        ticket.Codigo = dr["SCODE"].ToString();
                        ticket.CodigoJIRA = dr["SCODE_JIRA"].ToString();
                        ticket.stipo = dr["TIPO"].ToString();
                        ticket.marketer = dr["SNAME"].ToString();
                        ticket.NameClient = dr["NOMBRE"].ToString();
                        ticket.Fecha = dr["Fecha"].ToString();
                        ticket.url = dr["URL"].ToString();
                        ticket.estado = dr["ESTADO"].ToString();
                        ticket.TotalRows = dr["TOTALROWS"].ToString();
                        ticket.TotalTra = dr["TOTALTRAMITE"].ToString();
                        ticket.TotalCot = dr["TOTALCOT"].ToString();
                        ticket.TotalPD = dr["TOTALPD"].ToString();
                        //DEV EC - INICIO
                        ticket.TotalSol = dr["TOTALSOL"].ToString();
                        //DEV EC - FIN
                        /*if (proy != ticket.CodigoJIRA.Substring(0, 2))
                        {
                            if(ticket.CodigoJIRA.Substring(0, 2) = "GC"){
                                proy = "GCS";
                            }
                            else{
                                proy = "PDG";
                            }
                        }
                        ticket.url = url+ proy + @"/issues/" + ticket.CodigoJIRA;*/

                        lista.Add(ticket);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex, "Catch");
            }

            return lista;
        }
        //public async Task<ResponseViewModel> setResponseObs()
        //{


        //}
        public List<ListBranch> GetBranchList()
        {

            List<ListBranch> ramos = new List<ListBranch>();
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", "PKG_BDU_CLIENT_360_2", "PRC_LIST_BRANCHSGC"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        ListBranch ramo = new ListBranch();
                        ramo.BranchId = Decimal.Parse(dr["NBRANCH_JIRA"].ToString());
                        ramo.BranchName = dr["SDESCRIPT"].ToString();
                        ramos.Add(ramo);
                    }
                }
            }
            catch (Exception)
            {
                ramos = new List<ListBranch>();
            }
            return ramos;
        }
        public List<Product> GetProducts(string ramo)
        {
            List<Product> products = new List<Product>();
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("P_NBRANCH", OracleDbType.Int32, ramo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", "PKG_BDU_CLIENT_360_2", "PRC_LIST_PRODMSGC"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        Product prod = new Product();
                        prod.NCODE = long.Parse(dr["NPROD_JIRA"].ToString());
                        prod.SNAME = dr["SDESCRIPT"].ToString();
                        products.Add(prod);
                    }
                }
            }
            catch (Exception)
            {
                products = new List<Product>();
            }
            return products;
        }
        public String GetUusuarioJIRA(string usuario)
        {
            string user = "";
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("SCODE", OracleDbType.Varchar2, usuario, ParameterDirection.Input));
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", "PKG_BDU_CLIENT_360_2", "PRC_GET_CLIENTJIRA"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {

                        user = dr["SCODE_JIRA"].ToString();

                    }
                }
            }
            catch (Exception)
            {
                user = "";
            }
            return user;
        }
        public String GetCommGruoupUusuario(string usuario)
        {
            string group = "";
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("SCODE", OracleDbType.Varchar2, usuario.ToString(), ParameterDirection.Input));
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", "PKG_BDU_CLIENT_360_2", "PRC_GET_CLIENTCOMMGROUP"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        group = dr["NID_GROUP"].ToString();
                        group = group + ";" + dr["SCODE_JIRA"].ToString();
                    }
                }
            }
            catch (Exception)
            {
                group = "";
            }
            return group;

        }

        public int ReaLimitJira()
        {
            int score = 0;
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", "PKG_BDU_CLIENT_360_2", "REA_LIMIT_JIRA"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        score = int.Parse(dr["NLIMIT"].ToString());
                    }
                }
            }
            catch (Exception)
            {
                score = -1;
            }
            return score;

        }
        public static List<int> AllIndexesOf(string str, string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                throw new ArgumentException("the string to find may not be empty", "value");
            }

            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                {
                    return indexes;
                }

                indexes.Add(index);
            }
        }
        public async Task<ExcelPackage> ExportGetTicketList(SGCRequest request)
        {
            // UserListExcel result = new UserListExcel();
            var excelPackage = new ExcelPackage();
            var workSheet = excelPackage.Workbook.Worksheets.Add("Tickets");
            int i = 4;

            workSheet.Cells["A1:G1"].Style.Font.Bold = true;
            workSheet.Cells["A1:G1"].Merge = true;
            workSheet.Cells["A1:G1"].Style.Font.Size = 20;

            workSheet.Cells["A3:G3"].Style.Font.Bold = true;
            workSheet.Cells["A1"].Value = "Lista de tickets";


            workSheet.Cells["A3"].Value = "Código";
            workSheet.Cells["B3"].Value = "Tipo";
            workSheet.Cells["C3"].Value = "Fecha";
            workSheet.Cells["D3"].Value = "Encargado";
            workSheet.Cells["E3"].Value = "Cliente";
            workSheet.Cells["F3"].Value = "Estado";

            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("P_USERCODE", OracleDbType.Int32, request.usuario, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_COMMGROUP", OracleDbType.Int32, request.grupoComercial, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SEARCH", OracleDbType.Varchar2, request.search, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_TYPE", OracleDbType.Varchar2, request.type, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NPAGE_INDEX", OracleDbType.Int32, request.PageIndex, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NPAGE_SIZE", OracleDbType.Int32, request.PageSize, ParameterDirection.Input));
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)await _connectionBase.ExecuteByStoredProcedureVTAsync(string.Format("{0}.{1}", "PKG_BDU_CLIENT_360_2", "PRC_LIST_TICKETSOVPDGSGC2"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))


                {


                    while (dr.Read())
                    {

                        workSheet.Cells["A" + i].Value = dr["SCODE"].ToString();
                        workSheet.Cells["B" + i].Value = dr["TIPO"].ToString();
                        workSheet.Cells["C" + i].Value = dr["Fecha"].ToString();
                        workSheet.Cells["D" + i].Value = dr["SNAME"].ToString();
                        workSheet.Cells["E" + i].Value = dr["NOMBRE"].ToString();
                        workSheet.Cells["F" + i].Value = dr["ESTADO"].ToString();
                        //workSheet.Cells["G" + i].Value = dr["URL"].ToString();
                        //workSheet.Cells["H" + i].Value = dr["ESTADO"].ToString();
                        //workSheet.Cells["I" + i].Value = dr["TOTALROWS"].ToString();
                        //workSheet.Cells["J" + i].Value = dr["TOTALTRAMITE"].ToString();
                        //workSheet.Cells["K" + i].Value = dr["TOTALCOT"].ToString();
                        //workSheet.Cells["L" + i].Value = dr["TOTALPD"].ToString();
                        i++;


                    }






                    //for (var c = 1; c <= 7; c++)
                    //{
                    //    workSheet.Column(c).AutoFit();
                    //}

                }
            }
            catch (System.Exception ex)
            {

                throw ex;
            }

            workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();
            return excelPackage;
        }
        public List<SimpleProduct> GetSimpleProduct()
        {
            List<SimpleProduct> canales = new List<SimpleProduct>();
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package3, "PRC_REA_TICK_SIMPLEPRODUCT"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        SimpleProduct product = new SimpleProduct();
                        product.NIDCM = int.Parse(dr["NIDCM"].ToString());
                        product.SDESCRIPTION = dr["SDESCRIPTION"].ToString();
                        canales.Add(product);
                    }
                }
            }
            catch (Exception)
            {
                canales = new List<SimpleProduct>();
            }
            return canales;
        }
        public List<PartnerType> GetPartnerType()
        {

            List<PartnerType> canales = new List<PartnerType>();
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package3, "PRC_REA_TICK_PARTNERTYPE"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        PartnerType partner = new PartnerType();
                        partner.NIDCM = int.Parse(dr["NIDCM"].ToString());
                        //canal.Codigo = int.Parse(dr["CODIGO"].ToString());
                        partner.SDESCRIPTION = dr["SDESCRIPTION"].ToString();
                        canales.Add(partner);
                    }
                }
            }
            catch (Exception)
            {
                canales = new List<PartnerType>();
            }
            return canales;
        }
        public List<TipoPrestacion> GetTipoPrestaciones()
        {
            List<TipoPrestacion> tipoprestaciones = new List<TipoPrestacion>();
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package3, "PRC_REA_TICK_PREST"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        TipoPrestacion tipoprestacion = new TipoPrestacion();
                        tipoprestacion.Id = int.Parse(dr["NID"].ToString());
                        tipoprestacion.Descripcion = dr["SDESCRIPT"].ToString();
                        tipoprestaciones.Add(tipoprestacion);
                    }
                }
            }
            catch (Exception e)
            {
                tipoprestaciones = new List<TipoPrestacion>();
            }
            return tipoprestaciones;
        }

        public string ValidarTramiteRentas(TramiteRentas request)
        {

            WriteErrorLog("METODO DE VALIDAR TRAMITE DE RENTAS INICIA");
            string mensaje = "";

            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("NBRANCH", OracleDbType.Int32, request.valorRamo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NPRODUCT", OracleDbType.Int32, request.valorProducto, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NMOTIV", OracleDbType.Int32, request.valorMotivo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NSUBMOTIV", OracleDbType.Int32, request.valorSubmotivo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package3, "PRC_VALIDACION_TRAMITE_RENTAS"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        string respuesta = "";

                        respuesta = dr["RESPUESTA"].ToString();
                        mensaje = respuesta;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex, "Catch");
                throw ex;
            }
            return mensaje;


            /* 
             * public List<TipoPrestacion> GetTipoPrestaciones()
        {
            List<TipoPrestacion> tipoprestaciones = new List<TipoPrestacion>();
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package3, "PRC_REA_TICK_PREST"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        TipoPrestacion tipoprestacion = new TipoPrestacion();
                        tipoprestacion.Id = int.Parse(dr["NID"].ToString());
                        tipoprestacion.Descripcion = dr["SDESCRIPT"].ToString();
                        tipoprestaciones.Add(tipoprestacion);
                    }
                }
            }
            catch (Exception ex)
            {
                tipoprestaciones = new List<TipoPrestacion>();
            }
            return tipoprestaciones;
        }
             
             */

        }

        //DEV RENE --INI
        public TicketSTC GetTicketSTC(String scode)
        {
            TicketSTC ticket = new TicketSTC();

            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("SCODE", OracleDbType.Varchar2, scode, ParameterDirection.Input));
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));//se cambio de paquete ahora este PKG_BDU_CLIENT_360.PRC_LIST_TICKETSTC
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", "PKG_BDU_CLIENT_360", "PRC_LIST_TICKETSTC"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        ticket.code360 = dr["SCODE"].ToString();
                        ticket.codeJira = dr["SCODE_JIRA"].ToString();
                        ticket.numTramite = dr["NTRAMITE_COBRAN"].ToString();
                        ticket.desTramite = dr["SDESC_COBRA"].ToString();
                        //ticket.codMetPago = dr["NMETODO_PAGO"].ToString();
                        //ticket.desMetPago = dr["SDESC_PAGO"].ToString();
                        //ticket.invoice = dr["SFACTURA"].ToString();
                        ticket.notCredit = dr["SNOTA_CRED"].ToString();
                        //ticket.glosa = dr["SGLOSA"].ToString();
                        ticket.nomTitular = dr["STITULAR"].ToString();
                        ticket.docTitular = dr["SDOC_TITULAR"].ToString();
                        ticket.desDocTitular = dr["SDESC_DOC"].ToString();
                        ticket.codBanco = dr["NBANCO"].ToString();
                        ticket.desBanco = dr["SDESC_DESC_BANCO"].ToString();
                        ticket.codMoneda = dr["NMONEDA"].ToString();
                        ticket.desMoneda = dr["DESC_MONEDA"].ToString();
                        ticket.monto = dr["SMONTO"].ToString();
                        ticket.codCuenta = dr["NTIPO_CUENTA"].ToString();
                        ticket.desCuenta = dr["SDESC_CUENTA"].ToString();
                        ticket.comentPago = dr["SCOMENTA_PAGO"].ToString();
                        ticket.cuentDestino = dr["SCUENTA_DEST"].ToString();
                        ticket.codCanalIngreso = dr["NCANAL_INGRESO"].ToString();
                        ticket.desCanarIngreso = dr["SCANAL"].ToString();

                    }
                }

            }
            catch (Exception ex)
            {

                WriteErrorLog(ex, "Catch");
            }
            return ticket;
        }
        //DEV RENE --FIN

        //DEV DS --INI
        public List<TipoDocumento> GetTipoDocumento()
        {
            List<TipoDocumento> tipodocumentos = new List<TipoDocumento>();
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package4, "SEL_TICKET_TYPE_DOCUMENT"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        TipoDocumento tipdoc = new TipoDocumento();
                        tipdoc.NCODE_JIRA = int.Parse(dr["NCODE_JIRA"].ToString());
                        tipdoc.Descripcion = dr["SDESCRIPT"].ToString();
                        tipodocumentos.Add(tipdoc);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex, "Catch");
                throw ex;
            }
            return tipodocumentos;
        }

        public List<Banco> GetBanco()
        {
            List<Banco> bancos = new List<Banco>();
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package4, "SEL_TICKET_BANK"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        Banco banco = new Banco();
                        banco.NCODE_JIRA = int.Parse(dr["NCODE_JIRA"].ToString());
                        banco.Descripcion = dr["SDESCRIPT"].ToString();
                        bancos.Add(banco);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex, "Catch");
                throw ex;
            }
            return bancos;
        }


        public List<TipoMoneda> GetTipoMoneda()
        {
            List<TipoMoneda> tipomonedas = new List<TipoMoneda>();
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package4, "SEL_TICKET_TYPE_CURRENCY"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        TipoMoneda tipmoneda = new TipoMoneda();
                        tipmoneda.Sdata = int.Parse(dr["SDATA"].ToString());
                        tipmoneda.Sdescript = dr["SDESCRIPT"].ToString();
                        tipomonedas.Add(tipmoneda);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex, "Catch");
                throw ex;
            }
            return tipomonedas;
        }

        public List<TipoCuenta> GetTipoCuenta()
        {
            List<TipoCuenta> tipodecuentas = new List<TipoCuenta>();
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package4, "SEL_TICKET_TYPE_BILL"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        TipoCuenta tipocuenta = new TipoCuenta();
                        tipocuenta.NCODE_JIRA = int.Parse(dr["NCODE_JIRA"].ToString());
                        tipocuenta.Descripcion = dr["SDESCRIPT"].ToString();
                        tipodecuentas.Add(tipocuenta);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex, "Catch");
                throw ex;
            }
            return tipodecuentas;
        }

        public DatosPagoResponse RegistrarDatosPago(DatosPagoRequest request)
        {
            DatosPagoResponse DatosPagoR = new DatosPagoResponse();

            if (string.IsNullOrEmpty(request.P_NTIPDOC_TITULAR) || request.P_NTIPDOC_TITULAR == "0") { request.P_NTIPDOC_TITULAR = null; }
            if (string.IsNullOrEmpty(request.P_NBANCO) || request.P_NBANCO == "0") { request.P_NBANCO = null; }
            if (string.IsNullOrEmpty(request.P_NMONEDA) || request.P_NMONEDA == "0") { request.P_NMONEDA = null; }
            if (string.IsNullOrEmpty(request.P_NTIPO_CUENTA) || request.P_NTIPO_CUENTA == "0") { request.P_NTIPO_CUENTA = null; }
             
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("P_SCODE", OracleDbType.Varchar2, request.P_SCODE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_IDSUBMOTIVO", OracleDbType.Varchar2, request.P_IDSUBMOTIVO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NAMESUBMOTIVO", OracleDbType.Varchar2, request.P_NAMESUBMOTIVO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SNOTA_CRED", OracleDbType.Varchar2, request.P_SNOTA_CRED, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_STITULAR", OracleDbType.Varchar2, request.P_STITULAR, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTIPDOC_TITULAR", OracleDbType.Varchar2, request.P_NTIPDOC_TITULAR, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SDOC_TITULAR", OracleDbType.Varchar2, request.P_SDOC_TITULAR, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NBANCO", OracleDbType.Varchar2, request.P_NBANCO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NMONEDA", OracleDbType.Varchar2, request.P_NMONEDA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SMONTO", OracleDbType.Varchar2, request.P_SMONTO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTIPO_CUENTA", OracleDbType.Varchar2, request.P_NTIPO_CUENTA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCUENTA_DEST", OracleDbType.Varchar2, request.P_SCUENTA_DEST, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCOMENTA_PAGO", OracleDbType.Varchar2, request.P_SCOMENTA_PAGO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NID_GROUP", OracleDbType.Varchar2, request.P_NID_GROUP, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCANAL_INGRESO", OracleDbType.Varchar2, request.P_NCANAL_INGRESO, ParameterDirection.Input));

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;

                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);

                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package4, "INS_INSERTDATOSPAGO"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    var v_P_NCODE = P_NCODE.Value.ToString();
                    string mensajito = P_SMESSAGE.Value.ToString();

                    DatosPagoR.Codigo = v_P_NCODE;
                    DatosPagoR.Mensaje = mensajito; 
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex, "Catch");
                DatosPagoR = new DatosPagoResponse();
                DatosPagoR.Mensaje = ex.Message;
            }
            return DatosPagoR;
        }
        //DEV DS --FIN

        //DEV CY -- INI

        public Ticket GetTicketType(string codigo)
        {
            Ticket ticket = new Ticket();
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("SCODE", OracleDbType.Varchar2, codigo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package3, "PRC_GET_TICKET_TYPE"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {

                        ticket.Codigo = dr["SCODE"].ToString();
                        ticket.NtypeJira = dr["NTYPE_JIRA"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                ticket = new Ticket();
            }
            return ticket;
        }

        public Ticket GetSTC_Ticket(string codigo)
        {
            Ticket ticket = new Ticket();
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("SCODE", OracleDbType.Varchar2, codigo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package3, "PRC_REA_CLIENT_TICKET_STC"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {

                        ticket.Codigo = dr["SCODE"].ToString();
                        ticket.FecRegistro = dr["DREGDATE"].ToString();
                        ticket.Ramo = dr["RAMO_JIRA"].ToString();
                        ticket.Producto = dr["PRODUCTO_JIRA"].ToString();
                        ticket.Poliza = dr["POLIZA"].ToString();
                        ticket.Estado = dr["ESTADO"].ToString();
                        ticket.Contacto = dr["CONTACTO"].ToString();
                        ticket.Nombre = dr["CLIENTE"].ToString();
                        ticket.Documento = dr["NRODOC"].ToString();
                        ticket.DocumentoCli = dr["DOCUMENTOCLI"].ToString();
                        ticket.Ejecutivo = dr["EXECU"].ToString();
                        ticket.TipoCaso = dr["TIPCASO"].ToString();
                        ticket.Proyecto = dr["PROY"].ToString();
                        //PRODUCCION
                        ticket.Summary =  codigo;
                        //CALIDAD
                        //ticket.Summary = "PRUEBA " + codigo;
                        ticket.Reporter = dr["EXECU"].ToString();
                        ticket.EmailCli = dr["MAIL"].ToString();
                        ticket.SubMotivo = dr["SubTra"].ToString();
                        ticket.Motivo = dr["SCODE_JIRA_TRA"].ToString();
                        ticket.Direccion = dr["DIRECCIO"].ToString();
                        ticket.Descripcion = dr["DESCRIPCION"].ToString();
                        ticket.Reconsideracion = dr["NRECON"].ToString();
                        ticket.Telefono = dr["PHONE"].ToString();
                        ticket.Email = dr["MAIL"].ToString();
                        ticket.Aplicacion = "14113";
                        ticket.Segmento = dr["SEGMENTO"].ToString();
                        ticket.ChannelProcess = "15712";
                        ticket.NtypeJira = dr["NTYPE_JIRA"].ToString();
                        ticket.Tipo = dr["ISSUE"].ToString();
                        ticket.Proyecto = dr["PROY"].ToString();
                        ticket.CobranTramite = dr["TRAMCOBR"].ToString();
                        ticket.CommercialGroup = "-1";
                        ticket.TramInCant = dr["CANALING"].ToString();
                        //ticket.PayType = dr["METODOPAGO"].ToString();
                        ticket.Factura = dr["FACTURA"].ToString();
                        ticket.CreditNote = dr["NOTA_CRED"].ToString();
                        //ticket.Glosa = dr["GLOSA"].ToString();
                        ticket.TitularCuenta = dr["NOMTITULAR"].ToString();
                        ticket.DocTypeTitularCuenta = dr["TIPDOCTIT"].ToString();
                        ticket.NumDocTitularCuenta = dr["DOCTITULAR"].ToString();
                        ticket.Bank = dr["BANCO"].ToString();
                        ticket.Currency = dr["MONEDA"].ToString();
                        float val = Convert.ToSingle(dr["MONTO"], CultureInfo.CreateSpecificCulture("es-ES"));
                        ticket.Ammount = val;
                        ticket.AccountType = dr["TIPOCUENTA"].ToString();
                        ticket.DestAccount = dr["CUENTA_DEST"].ToString();
                        ticket.PayCommentary = dr["COMENTARIOS"].ToString();
                        ticket.TipoDocumentoCli = dr["TIPODOCCLI"].ToString(); 
                    }
                }
            }
            catch (Exception ex)
            {
                ticket = new Ticket();
            }
            return ticket;
        }
        
        public ResponseViewModel SetSTCObservation(string codigo)
        {
            ResponseViewModel response = new ResponseViewModel();
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("SCODE", OracleDbType.Varchar2, codigo, ParameterDirection.Input));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;

                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                using (OracleDataReader dr = (OracleDataReader) _connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package4, "UPD_TICKET_STC_OBSERVATION"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    response.code = int.Parse(P_NCODE.Value.ToString());
                    response.message = P_SMESSAGE.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
                response.code = 1;
                response.message = ex.Message;
            }
            return response;
        } 

        public ResponseViewModel UpdateFieldsSTC(DatosPagoRequest model)
        {
            ResponseViewModel response = new ResponseViewModel();
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("P_SCODE", OracleDbType.Varchar2, model.scode, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SNOTA_CRED", OracleDbType.Varchar2, model.P_SNOTA_CRED, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_STITULAR", OracleDbType.Varchar2, model.P_STITULAR, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTIPDOC_TITULAR", OracleDbType.Varchar2, model.P_NTIPDOC_TITULAR, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SDOC_TITULAR", OracleDbType.Varchar2, model.P_SDOC_TITULAR, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NBANCO", OracleDbType.Varchar2, model.P_NBANCO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NMONEDA", OracleDbType.Varchar2, model.P_NMONEDA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SMONTO", OracleDbType.Varchar2, model.P_SMONTO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTIPO_CUENTA", OracleDbType.Varchar2, model.P_NTIPO_CUENTA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCUENTA_DEST", OracleDbType.Varchar2, model.P_SCUENTA_DEST, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCOMENTA_PAGO", OracleDbType.Varchar2, model.P_SCOMENTA_PAGO, ParameterDirection.Input));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Int16, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                
                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;

                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);

                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package4, "UPD_FIELDS_STC"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    response.code = int.Parse(P_NCODE.Value.ToString());
                    response.message = P_SMESSAGE.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                //throw ex;
                response.code = 1;
                response.message = ex.Message;
            }
            return response;
        } 

        public string GetFieldsSTC(DatosPagoRequest model)
        {
            string Fields = "";
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("P_SCODE", OracleDbType.Varchar2, model.scode, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SNOTA_CRED", OracleDbType.Varchar2, model.P_SNOTA_CRED, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_STITULAR", OracleDbType.Varchar2, model.P_STITULAR, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTIPDOC_TITULAR", OracleDbType.Varchar2, model.P_NTIPDOC_TITULAR, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SDOC_TITULAR", OracleDbType.Varchar2, model.P_SDOC_TITULAR, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NBANCO", OracleDbType.Varchar2, model.P_NBANCO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NMONEDA", OracleDbType.Varchar2, model.P_NMONEDA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SMONTO", OracleDbType.Varchar2, model.P_SMONTO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NTIPO_CUENTA", OracleDbType.Varchar2, model.P_NTIPO_CUENTA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCUENTA_DEST", OracleDbType.Varchar2, model.P_SCUENTA_DEST, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCOMENTA_PAGO", OracleDbType.Varchar2, model.P_SCOMENTA_PAGO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));

                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package4, "PRC_VALIDACION_FIELDS_STC"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        string respuesta = "";

                        respuesta = dr["RESPUESTA"].ToString();
                        Fields = respuesta;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Fields;
        }

        public ResponseViewModel UpdateTicketFile(string codigo)
        {
            ResponseViewModel response = new ResponseViewModel();
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("P_NID", OracleDbType.Varchar2, codigo, ParameterDirection.Input));
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;

                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package4, "PRC_UPD_TICKET_ADJ"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    response.code = int.Parse(P_NCODE.Value.ToString());
                    response.message = P_SMESSAGE.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //response.code = 1;
                //response.message = ex.Message;
            }
            return response;
        }
        //DEV CY -- FIN

        //DEV DS INICIO
        public DatosPagoRequest GetInfoToUPDSTC(string scode)
        {
            DatosPagoRequest ticket = new DatosPagoRequest();

            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("SCODE_TRA", OracleDbType.Varchar2, scode, ParameterDirection.Input));
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package4, "GetInfoToUPDATE_STC"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    while (dr.Read())
                    {
                        ticket.P_SUB_MOTIVO = dr["SUBMOTIVO"].ToString();
                        ticket.P_SNOTA_CRED = dr["NOTA_CRED"].ToString();
                        ticket.P_STITULAR = dr["TIT_CUENTA"].ToString();
                        ticket.P_NTIPDOC_TITULAR = dr["TIPO_DOCUMENTO"].ToString();
                        ticket.P_SDOC_TITULAR = dr["NUMERO_DOCUMENTO"].ToString();
                        ticket.P_NBANCO = dr["BANCO"].ToString();
                        ticket.P_NMONEDA = dr["MONEDA"].ToString();
                        ticket.P_SMONTO = dr["MONTO"].ToString();
                        ticket.P_NTIPO_CUENTA = dr["TIPO_CUENTA"].ToString();
                        ticket.P_SCUENTA_DEST = dr["CUENTA_DEST"].ToString();
                        ticket.P_SCOMENTA_PAGO = dr["COMENTARIO"].ToString();
                        ticket.P_CANT_ADJUNT = dr["SCODE_TRA"].ToString();
                    } 
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog(ex, "Error");
            }
            return ticket;
        }
        //DEV DS FIN
        //Cambios Normativos SLA
        public int AfterFecCorte(string scode)
        {
            int result = 0;
            try
            {
                List<OracleParameter> parameter = new List<OracleParameter>();
                parameter.Add(new OracleParameter("P_SCODE", OracleDbType.Varchar2, scode, ParameterDirection.Input));
                parameter.Add(new OracleParameter("P_AFTER_FEC_CORTE", OracleDbType.Int64, result, ParameterDirection.Output));

                result = _connectionBase.ExecuteByStoredProcedureInt("PKG_BDU_TICKET.PRC_GET_AFTER_FEC_CORTE", parameter);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

    }
}





