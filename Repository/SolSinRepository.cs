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
    public class SolSinRepository : ISolSinRepository
    {
        private readonly IOptions<AppSettings> appSettings;
        private readonly IConnectionBase _connectionBase;

//Métodos Adicionales
#region
        public SolSinRepository(IOptions<AppSettings> appSettings, IConnectionBase ConnectionBase)
        {
            this.appSettings = appSettings;
            _connectionBase = ConnectionBase;
        }

        public SolSinRepository()
        {
        }

        private string Package3 = "PKG_BDU_CLIENT_360";
        private string Package4 = "PKG_BDU_TICKET";
        public async Task<GetResponse> GetRequest(string baseUrl, string url, string token = null, int tipo = 1)
        {
            GetResponse result = new GetResponse();
            try
            {
                UriBuilder builder = new UriBuilder(baseUrl + url);
                string cadena = builder.Uri.ToString();
                HttpClientHandler handler = new HttpClientHandler();


                using (var client = new HttpClient(handler))
                {
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
                throw ex;
            }
            return result;
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
        List<EstructuraTicket> ISolSinRepository.GetEstructuraNew(string Tipo)
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
#endregion

//Tareas Principales S3
#region 
        public string SetArchivoAdjuntoNew(Adjunto adjunto)
        {
            string ticket = string.Empty;
            try
            { 
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("SNAME", OracleDbType.Varchar2, adjunto.name, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SSIZE", OracleDbType.Varchar2, adjunto.size, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SPATH", OracleDbType.Varchar2, adjunto.path, ParameterDirection.Input));
                parameters.Add(new OracleParameter("STYPE", OracleDbType.Varchar2, adjunto.tipo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("STYPE", OracleDbType.Varchar2, adjunto.path_gd, ParameterDirection.Input));
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
        public async Task<string> SetArchivoAdjuntoAsyncNew(Adjunto adjunto)
        {
            string ticket = string.Empty;
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("SNAME", OracleDbType.Varchar2, adjunto.name, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SSIZE", OracleDbType.Varchar2, adjunto.size, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SPATH", OracleDbType.Varchar2, adjunto.path, ParameterDirection.Input));
                parameters.Add(new OracleParameter("STYPE", OracleDbType.Varchar2, adjunto.tipo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("STYPE", OracleDbType.Varchar2, adjunto.path_gd, ParameterDirection.Input));
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
        public List<Adjunto> GetAdjuntosNew(string codigo, int tipo)
        {
            List<Adjunto> Adjunto = new List<Adjunto>();
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
                        Adjunto adjunto = new Adjunto();
                        adjunto.name = dr["SNAME"].ToString();
                        adjunto.path = dr["SPATH"].ToString();
                        adjunto.size = dr["SSIZE"].ToString();
                        adjunto.path_gd = dr["SPATH_GD"].ToString();                        
                        Adjunto.Add(adjunto);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Adjunto;
        }
        public (string, List<Adjunto>) GetAdjuntos2New(string codigo, int tipo)
        {
            List<Adjunto> Adjunto = new List<Adjunto>();
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
                        Adjunto adjunto = new Adjunto();
                        adjunto.name = dr["SNAME"].ToString();
                        adjunto.path = dr["SPATH"].ToString();
                        adjunto.size = dr["SSIZE"].ToString();
                        adjunto.path_gd = dr["SPATH_GD"].ToString();                        
                        Adjunto.Add(adjunto);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return (scodejira, Adjunto);
        }
        public List<Adjunto> GetEnviadosNew(string codigo, int tipo)
        {
            List<Adjunto> Adjunto = new List<Adjunto>();
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
                        Adjunto adjunto = new Adjunto();
                        adjunto.name = dr["SNAME"].ToString();
                        adjunto.path = dr["SPATH"].ToString();
                        adjunto.size = dr["SSIZE"].ToString();
                        adjunto.path_gd = dr["SPATH_GD"].ToString();
                        Adjunto.Add(adjunto);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Adjunto;
        }
        public RegiTicketResponse RegistraTicketJIRANew(TicketDinamico request, string type)
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
        public Adjunto S3AdjuntarNew(Adjunto adjunto, string type, string customfield = "customfield_12801")
        {
            //string base6 = "";
            var client = new RestClient(AppSettings.AWSAdjuntar + adjunto.name + "&time=" + Codificatiempo() + "&field=" + customfield);
            string responsed = "";
            string token = new TokenService().getTokenAWS(type);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer " + token);
            request.AddHeader("Content-Type", adjunto.mime);
            request.AddParameter(adjunto.name, Convert.FromBase64String(adjunto.content), ParameterType.RequestBody);
            //request.AddFile(adjunto.name, Convert.FromBase64String(adjunto.content), adjunto.name, adjunto.mime);
            // request.AddFile("btn_flecha_der_hover-SOL-157", @"D:\Descargas\Archivos\btn_flecha_der_hover.jpg");
            try
            {
                IRestResponse response = client.Execute(request);
                ID respuesta = JsonConvert.DeserializeObject<ID>(response.Content);

                //PRODUCCION
                adjunto.path_gd = respuesta.id;

            }
            catch (Exception ex)
            {
                responsed = ex.Message;
            }
            //CALIDAD
            // adjunto.path_gd = "PRUEBA DOCUMENTO";
            return adjunto;
        }
        public async Task<Adjunto> S3AdjuntarAsyncNew(Adjunto adjunto, string type, string customfield = "customfield_12801")
        {
            var client = new RestClient(AppSettings.AWSAdjuntar + adjunto.name + "&time=" + Codificatiempo() + "&field=" + customfield);
            string token = new TokenService().getTokenAWS(type);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Bearer " + token);
            request.AddHeader("Content-Type", adjunto.mime);
            request.AddParameter(adjunto.name, Convert.FromBase64String(adjunto.content), ParameterType.RequestBody);
            try
            {
                IRestResponse response = await client.ExecuteAsync(request);
                ID respuesta = JsonConvert.DeserializeObject<ID>(response.Content);
                adjunto.path_gd = respuesta.id;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return adjunto;
        }
        public async Task<string> GetS3AdjuntoNew(string codigo, string type)
        {
            List<Adjunto> lista = new List<Adjunto>();
            string token = new TokenService().getTokenAWS(type);
            string bass = AppSettings.AWSGetAdjunto;

            var result = await GetRequestRedirect(bass, codigo, token, 2);

            byte[] bytes = result.bytes;
            string enconded = Convert.ToBase64String(bytes);
            
            return enconded;
        }
        public string GenerateSolicitudNew(Contract contract)
        {
            GenerateResponse response = new GenerateResponse();
            string bass = AppSettings.ServicioWebGCliente + "api/Report/";
            var result = PostRequest(bass, "CreateSolicitud", contract);
            response = JsonConvert.DeserializeObject<GenerateResponse>(result);

            return response.data.ToString();
        }
        public DatosPagoResponse RegistrarDatosPagoNew(DatosPagoRequest request)
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
        public RegiTicketResponse SetJIRANew(Contract contract)
        {
            RegiTicketResponse response = new RegiTicketResponse();
            string pak = "PKG_BDU_CLIENT_360";
            if (contract.Aplicacion == "SGC")
            {
                pak = "PKG_BDU_CLIENT_360_2";
            }
            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("SCODE", OracleDbType.Varchar2, contract.Codigo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SCODE_JIRA", OracleDbType.Varchar2, contract.CodigoJIRA, ParameterDirection.Input));
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
        public CommonResponse ValidateTicketNew(RegContractRequest request)
        {
            CommonResponse oValidateTicket = new CommonResponse();
            String mensaje;
            List<String> listMensajes = new List<string>();
            oValidateTicket.respuesta = true;


            if (string.IsNullOrEmpty(request.correoAseg)) { request.correoAseg = null; }
            if (string.IsNullOrEmpty(request.dirAseg)) { request.dirAseg = null; }

            try
            {
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("NTYPETICK", OracleDbType.Varchar2, request.Tipo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NTYPEDOCCLI", OracleDbType.Varchar2, request.tipoDocAseg, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SDOCNUMCLI", OracleDbType.Varchar2, request.nroDocAseg, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NTYPEDOCCON", OracleDbType.Varchar2, request.tipoDocAseg, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SDOCNUMCON", OracleDbType.Varchar2, request.nroDocAseg, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SEMAILTITULAR", OracleDbType.Varchar2, request.correoAseg, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SDIRECCIONTTITULAR", OracleDbType.Varchar2, request.dirAseg, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NBRANCH", OracleDbType.Varchar2, request.ramo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NPOLICY", OracleDbType.Varchar2, request.Poliza, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NMONTO", OracleDbType.Varchar2, request.Monto, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SDESCRIPCION", OracleDbType.Varchar2, request.summary, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NREP", OracleDbType.Varchar2, request.Reconsideracion, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NVIARECP", OracleDbType.Varchar2, request.ViaRecepcion.Id, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NVIARESP", OracleDbType.Varchar2, request.ViaRespuesta.Id, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NMOTIVO", OracleDbType.Varchar2, request.SubMotivo.Motivo.Id, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NSUBMOTIVO", OracleDbType.Varchar2, request.SubMotivo.Id, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NUSER", OracleDbType.Varchar2, request.Usuario, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NCANAL", OracleDbType.Varchar2, request.Canal, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NRECON", OracleDbType.Varchar2, request.Reconsideracion, ParameterDirection.Input));
                parameters.Add(new OracleParameter("DFECRECEP", OracleDbType.Varchar2, request.fechaRecepcion, ParameterDirection.Input));
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
        public RegiTicketResponse SetTicketNew(RegContractRequest request)
        {
            RegiTicketResponse ticket = new RegiTicketResponse(); 
            if (string.IsNullOrEmpty(request.correoAseg)) { request.correoAseg = null; }
            if (string.IsNullOrEmpty(request.dirAseg)) { request.correoAseg = null; }
            // if (string.IsNullOrEmpty(request.UbigeoTitular)) { request.EmailTitular = null; }
            if (string.IsNullOrEmpty(request.tipoDocAseg.ToString())) { request.tipoDocAseg = 2; }
            if (request.NID_TICK_PREST == "0") { request.NID_TICK_PREST = null; }
              
            try
            {
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;

                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("NTYPETICK", OracleDbType.Varchar2, request.Tipo, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NTYPEDOCCLI", OracleDbType.Varchar2, request.tipoDocAseg, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SDOCNUMCLI", OracleDbType.Varchar2, request.nroDocAseg, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NTYPEDOCCON", OracleDbType.Varchar2, request.tipoDocAseg, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SDOCNUMCON", OracleDbType.Varchar2, request.nroDocAseg, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SEMAILTITULAR", OracleDbType.Varchar2, request.correoAseg, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SDIRECCIONTTITULAR", OracleDbType.Varchar2, request.dirAseg, ParameterDirection.Input));
                //  parameters.Add(new OracleParameter("SUBIGEOTITULAR", OracleDbType.Varchar2, request.Contacto.documento, ParameterDirection.Input));
                // parameters.Add(new OracleParameter("SUBIGEOTITULAR", OracleDbType.Varchar2, request.UbigeoTitular, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NBRANCH", OracleDbType.Varchar2, request.ramo, ParameterDirection.Input));
                // parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Varchar2, request.Producto, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NPOLICY", OracleDbType.Varchar2, request.Poliza, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NMONTO", OracleDbType.Varchar2, request.Monto, ParameterDirection.Input));
                parameters.Add(new OracleParameter("SDESCRIPCION", OracleDbType.Varchar2, request.summary, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NREP", OracleDbType.Varchar2, request.Reconsideracion, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NVIARECP", OracleDbType.Varchar2, request.ViaRecepcion.Id, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NVIARESP", OracleDbType.Varchar2, request.ViaRespuesta.Id, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NMOTIVO", OracleDbType.Varchar2, request.SubMotivo.Motivo.Id, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NSUBMOTIVO", OracleDbType.Varchar2, request.SubMotivo.Id, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NUSER", OracleDbType.Varchar2, request.Usuario, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NCANAL", OracleDbType.Varchar2, request.Canal, ParameterDirection.Input));
                parameters.Add(new OracleParameter("NRECON", OracleDbType.Varchar2, request.Reconsideracion, ParameterDirection.Input));
                parameters.Add(new OracleParameter("DFECRECEP", OracleDbType.Varchar2, request.fechaRecepcion, ParameterDirection.Input));
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
        public Contract GetTicketNew(string codigo)
        {
            Contract ticket = new Contract();
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
                        ticket.fechaRecepcion = DateTime.ParseExact(dr["DRECEPDATE"].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        ticket.Ramo = dr["RAMO"].ToString();
                        ticket.Producto = dr["PRODUCTO"].ToString();
                        ticket.Monto = dr["MONTO"].ToString();
                        ticket.SubMotivo = dr["SUBMOTIVO"].ToString();
                        ticket.UsuarioEnvio = dr["USRAT"].ToString();
                        ticket.FechaEnvio = dr["DSENT"].ToString();
                        if (!string.IsNullOrEmpty(ticket.FechaEnvio))
                        {
                            ticket.FechaEnvio = ticket.FechaEnvio.Substring(0, 10);
                        }
                        ticket.nombreAseg = dr["CLIENTENOM"].ToString();
                        ticket.tipoDocAseg = int.Parse(dr["TIPODOCCLI"].ToString());
                        ticket.nroDocAseg = dr["DOCUMENTOCLI"].ToString();
                        ticket.correoAseg = dr["EMAILCLI"].ToString();
                        ticket.dirAseg = dr["DIRCLI"].ToString();
                        ticket.nombreAseg = dr["USER_NAME_REG_TICK"].ToString();
                        ticket.NtypeJira = dr["NTYPE_JIRA"].ToString();
                    }  
                }
            }
            catch (Exception ex)
            {
                ticket = new Contract();
            }
            return ticket;
        }
        public async Task<Ticket> ConsultaTicketJIRANew(string codigo, string token, string type)
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
        public Contract GetTicketType(string codigo)
        {
            Contract ticket = new Contract();
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
                ticket = new Contract();
            }
            return ticket;
        }
        public Contract GetSTC_Ticket(string codigo)
        {
            Contract ticket = new Contract();
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
                        ticket.fechaRecepcion = DateTime.ParseExact(dr["DREGDATE"].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);;
                        ticket.Ramo = dr["RAMO_JIRA"].ToString();
                        ticket.Producto = dr["PRODUCTO_JIRA"].ToString();
                        ticket.nombreAseg = dr["CLIENTE"].ToString();
                        ticket.nroDocAseg = dr["NRODOC"].ToString();
                        ticket.tipoDocAseg = int.Parse(dr["DOCUMENTOCLI"].ToString());
                        //PRODUCCION                        
                    }
                }
            }
            catch (Exception ex)
            {
                ticket = new Contract();
            }
            return ticket;
        }
        public Contract GetJIRA(string codigo)
        {
            Contract ticket = new Contract();
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
                        ticket.fechaRecepcion = DateTime.ParseExact(dr["DREGDATE"].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);;
                        ticket.Ramo = dr["RAMO_JIRA"].ToString();
                        ticket.Producto = dr["PRODUCTO_JIRA"].ToString();
                        ticket.nombreAseg = dr["CLIENTE"].ToString();
                        ticket.nroDocAseg = dr["NRODOC"].ToString();
                        ticket.tipoDocAseg = int.Parse(dr["DOCUMENTOCLI"].ToString());                      
                    }
                }
            }
            catch (Exception ex)
            {
                ticket = new Contract();
            }
            return ticket;
        }
        #endregion
    }
}