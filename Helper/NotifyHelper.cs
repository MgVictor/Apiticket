using apiTicket.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace apiTicket.Helper
{
    public class NotifyHelper
    {
        public string ComposeBodyEmailTicket(string contentRootPath, Ticket ticketC, int cantRespuestaAdjuntosTicket, int cantAdjuntosTicket, int corte = 0)
        {

            string CodigoTicket = ticketC.Codigo;
            string Nro_dias = ticketC.DiasTiempoAtencion_SLA;
            string Tipo_Dias = ticketC.DescripcionTipoDiaPlural_SLA.ToLower();
            string tipoTicket = CodigoTicket.Substring(0, 3);
            string estadoTicket = ticketC.Estado;
            string canalTicket = ticketC.Canal;

            string code_jira = ticketC.CodigoJIRA.Substring(0, 3);

            string FecRecepcion = ticketC.FecRecepcion.Substring(0, 10);
            string ViaRecepcion = ticketC.ViaRecepcion;
            string SubMotivo = ticketC.SubMotivo;

            string htmlBody = string.Empty;

            string path_trama;

            try
            {

                switch (tipoTicket)
                {
                    case "REC":
                        if (corte == 1)
                        {
                            path_trama = Path.Combine(contentRootPath, @"Mail\Reclamo_Nuevo.html");
                        }
                        else
                        {
                            path_trama = Path.Combine(contentRootPath, @"Mail\Reclamo.html");
                        }
                        htmlBody = System.IO.File.ReadAllText(path_trama);
                        htmlBody = htmlBody.Replace("[CodigoTicket]", CodigoTicket);
                        htmlBody = htmlBody.Replace("[Nro_dias]", Nro_dias);
                        htmlBody = htmlBody.Replace("[Tipo_Dias]", Tipo_Dias);
                        break;
                    case "SOL":
                        if (corte == 1)
                        {
                            path_trama = Path.Combine(contentRootPath, @"Mail\Solicitud_Nuevo.html");
                        }
                        else
                        {
                            path_trama = Path.Combine(contentRootPath, @"Mail\Solicitud.html");
                        }
                        htmlBody = System.IO.File.ReadAllText(path_trama);
                        htmlBody = htmlBody.Replace("[CodigoTicket]", CodigoTicket);
                        htmlBody = htmlBody.Replace("[Nro_dias]", Nro_dias);
                        htmlBody = htmlBody.Replace("[Tipo_Dias]", Tipo_Dias);
                        break;
                    case "TRA":
                        //path_trama = Path.Combine(contentRootPath, @"Mail\Tramite.html");
                        //htmlBody = System.IO.File.ReadAllText(path_trama);
                        //htmlBody = htmlBody.Replace("[CodigoTicket]", CodigoTicket);
                        //htmlBody = htmlBody.Replace("[Nro_dias]", Nro_dias);
                        //htmlBody = htmlBody.Replace("[Tipo_Dias]", Tipo_Dias);
                        if (estadoTicket == "Cerrado 360")
                        {
                            if (canalTicket == "Clientes")
                            {
                                path_trama = Path.Combine(contentRootPath, @"Mail\AtencionRQ.html");
                                htmlBody = System.IO.File.ReadAllText(path_trama);
                                htmlBody = htmlBody.Replace("[Fecha]", FecRecepcion);
                                htmlBody = htmlBody.Replace("[Via]", ViaRecepcion);
                                htmlBody = htmlBody.Replace("[Motivo]", SubMotivo);
                                htmlBody = htmlBody.Replace("[Codigo]", CodigoTicket);
                            }
                            else
                            {
                                path_trama = Path.Combine(contentRootPath, @"Mail\AtencionCanal.html");
                                htmlBody = System.IO.File.ReadAllText(path_trama);
                                htmlBody = htmlBody.Replace("[Fecha]", FecRecepcion);
                                htmlBody = htmlBody.Replace("[Canal]", ViaRecepcion);
                                htmlBody = htmlBody.Replace("[Motivo]", SubMotivo);
                                htmlBody = htmlBody.Replace("[Codigo]", CodigoTicket);
                            }
                            htmlBody = (cantRespuestaAdjuntosTicket > 0) ? htmlBody.Replace("[Adjunto]", "SI") : htmlBody = htmlBody.Replace("[Adjunto]", "NO");
                        }
                        else
                        {
                            //DEV DS INI
                            //if (canalTicket == "Clientes")
                            if (code_jira == "TRE")
                            {
                                path_trama = Path.Combine(contentRootPath, @"Mail\Recepcióndocumentosadicionales_Jira.html");
                                htmlBody = System.IO.File.ReadAllText(path_trama);
                                htmlBody = htmlBody.Replace("[Codigo]", CodigoTicket);
                                htmlBody = htmlBody.Replace("[Fecha]", FecRecepcion);
                                htmlBody = htmlBody.Replace("[ViaRecep]", ViaRecepcion);
                                htmlBody = htmlBody.Replace("[SubMotivo]", SubMotivo);
                                htmlBody = htmlBody.Replace("XX", "15");
                            }

                            
                            else if (canalTicket == "Clientes")

                            // if (canalTicket == "Clientes")
                            //DEV DS FIN
                            {
                                path_trama = Path.Combine(contentRootPath, @"Mail\Requerimiento.html");
                                htmlBody = System.IO.File.ReadAllText(path_trama);
                                htmlBody = htmlBody.Replace("~", CodigoTicket);
                                htmlBody = htmlBody.Replace("[Fecha]", FecRecepcion);
                                htmlBody = htmlBody.Replace("[ViaRecep]", ViaRecepcion);
                                htmlBody = htmlBody.Replace("[SubMotivo]", SubMotivo);
                                htmlBody = htmlBody.Replace("XX", "15");
                            }
                            else
                            {
                                path_trama = Path.Combine(contentRootPath, @"Mail\RecepcionCanal.html");
                                htmlBody = System.IO.File.ReadAllText(path_trama);
                                htmlBody = htmlBody.Replace("[Codigo]", CodigoTicket);
                                htmlBody = htmlBody.Replace("[Fecha]", FecRecepcion);
                                htmlBody = htmlBody.Replace("[Canal]", ViaRecepcion);
                                htmlBody = htmlBody.Replace("[Motivo]", SubMotivo);
                                htmlBody = htmlBody.Replace("XX", "15");
                            }
                            htmlBody = (cantAdjuntosTicket > 0) ? htmlBody.Replace("[Adjunto]", "SI") : htmlBody = htmlBody.Replace("[Adjunto]", "NO");
                        }
                        break;
                    default:
                        path_trama = Path.Combine(contentRootPath, @"Mail\Consulta.html");
                        htmlBody = System.IO.File.ReadAllText(path_trama);
                        htmlBody = htmlBody.Replace("[CodigoTicket]", CodigoTicket);
                        break;
                }
            }
            catch (Exception ex)
            {
                htmlBody = "";
            }

            return htmlBody;
        }
        public string ComposeSubjectEmailTicket(Ticket ticketC)
        {

            string CodigoTicket = ticketC.Codigo;
            string tipoTicket = CodigoTicket.Substring(0, 3);
            string estadoTicket = ticketC.Estado;
            string canalTicket = ticketC.Canal;
            string AsuntoEmail = string.Empty;
            string code_jira = ticketC.CodigoJIRA.Substring(0, 3);

            switch (tipoTicket)
            {
                case "REC":
                    AsuntoEmail = "Tu reclamo " + CodigoTicket + " ha sido recibido";
                    break;
                case "SOL":
                    AsuntoEmail = "Tu solicitud de información " + CodigoTicket + " ha sido recibida ";
                    break;
                case "TRA":
                    AsuntoEmail = "Tu trámite " + CodigoTicket + " ha sido recibido ";
                    if (estadoTicket == "Cerrado 360")
                    {
                        if (canalTicket == "Clientes")
                        {
                            AsuntoEmail = "Tu trámite N° " + CodigoTicket + " ha sido atendido";
                        }
                        else
                        {
                            AsuntoEmail = "Tu trámite N° " + CodigoTicket + " ha sido atendido";
                        }
                    }
                    else
                    {
                        AsuntoEmail = "Tu trámite " + CodigoTicket + " ha sido recibido ";
                    }
                    break;
                default:
                    AsuntoEmail = "Tu consulta " + CodigoTicket + " ha sido recibido";
                    break;
            }
            if(code_jira == "TRE")
            {
                AsuntoEmail = "Hemos recibido su respuesta para el trámite N° " + CodigoTicket ;
            }
            return AsuntoEmail;
        }


        public Boolean SendMail(string addressFrom, string pwdFrom, string addressTo, string subject, string body, List<Archivo> archivos)
        {
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            Boolean seEnvioCorreo = true;
            try
            {
                using (var mail = new MailMessage())
                {
                    mail.From = new MailAddress(addressFrom);
                    mail.To.Add(addressTo);
                    mail.IsBodyHtml = true;
                    mail.Subject = subject;
                    mail.Body = body;
                    System.Net.Mail.Attachment attachment;

                    if (archivos != null)
                    {
                        foreach (Archivo archivo in archivos)
                        {
                            byte[] myByteArray = Convert.FromBase64String(archivo.content);
                            MemoryStream stream1 = new MemoryStream(myByteArray);
                            attachment = new System.Net.Mail.Attachment(stream1, archivo.name, archivo.mime);
                            //  attachment = new System.Net.Mail.Attachment(archivo);
                            mail.Attachments.Add(attachment);
                        }
                    }

                    SmtpServer.Port = 587;
                    SmtpServer.UseDefaultCredentials = false;
                    SmtpServer.Credentials = new System.Net.NetworkCredential(addressFrom, pwdFrom);
                    SmtpServer.EnableSsl = true;
                    SmtpServer.Send(mail);
                }

            }
            catch (Exception ex)
            {
                seEnvioCorreo = false;
            }
            return seEnvioCorreo;

        }
    }
}
