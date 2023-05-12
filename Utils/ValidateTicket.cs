using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using apiTicket.Models;


namespace apiTicket.Utils
{

    public class ValidateTicket
    {
        private readonly Util util = new Util();
        public TicketResponse ValidarTicket(TicketSiniestro request)
        {

            TicketResponse response = new TicketResponse();
            string operacion = string.Empty;

            if (request.codTipoOperacion == null)
            {
                Error error = new Error();
                string[] err = AppSettings.OperacionNoEnviada;
                error.codError = err[0];
                error.mensaje = err[1];
                response.errorList.Add(error);
            }
            else
            {

                if (util.CheckOperacion(request.codTipoOperacion))
                {
                    if (request.codTipoOperacion == 1)
                    {
                        operacion = "registro";
                        if (!util.CheckString(request.fecRegistro))
                        {
                            Error error = new Error();
                            string[] err = AppSettings.CampoObligatorio;
                            error.codError = err[0];
                            error.mensaje = err[1].Replace("[campo]", "fecRegistro").Replace("[codigo]", operacion);
                            response.errorList.Add(error);
                        }
                        if (request.codTipoTramite == null)
                        {
                            Error error = new Error();
                            string[] err = AppSettings.CampoObligatorio;
                            error.codError = err[0];
                            error.mensaje = err[1].Replace("[campo]", "codTipoTramite").Replace("[codigo]", operacion);
                            response.errorList.Add(error);
                        }
                        if (request.Poliza == null)
                        {
                            Error error = new Error();
                            string[] err = AppSettings.EstructuraSinDatos;
                            error.codError = err[0];
                            error.mensaje = err[1].Replace("[campo]", "Poliza");
                            response.errorList.Add(error);
                        }
                        else
                        {
                            if (request.Poliza.codEstado == null)
                            {
                                Error error = new Error();
                                string[] err = AppSettings.CampoObligatorioSub;
                                error.codError = err[0];
                                error.mensaje = err[1].Replace("[campo]", "codEstado").Replace("[codigo]", operacion).Replace("[estructura]", "Poliza");
                                response.errorList.Add(error);
                            }
                            if (request.Poliza.codProducto == null)
                            {
                                Error error = new Error();
                                string[] err = AppSettings.CampoObligatorio;
                                error.codError = err[0];
                                error.mensaje = err[1].Replace("[campo]", "codProducto").Replace("[codigo]", operacion);
                                response.errorList.Add(error);
                            }
                            if (request.Poliza.codRamo == null)
                            {
                                Error error = new Error();
                                string[] err = AppSettings.CampoObligatorio;
                                error.codError = err[0];
                                error.mensaje = err[1].Replace("[campo]", "codRamo").Replace("[codigo]", operacion);
                                response.errorList.Add(error);
                            }
                            if (request.Poliza.nroCertificado == null)
                            {
                                Error error = new Error();
                                string[] err = AppSettings.CampoObligatorio;
                                error.codError = err[0];
                                error.mensaje = err[1].Replace("[campo]", "nroCertificado").Replace("[codigo]", operacion);
                                response.errorList.Add(error);
                            }
                            if (request.Poliza.nroPoliza == null)
                            {
                                Error error = new Error();
                                string[] err = AppSettings.CampoObligatorio;
                                error.codError = err[0];
                                error.mensaje = err[1].Replace("[campo]", "nroPoliza").Replace("[codigo]", operacion);
                                response.errorList.Add(error);
                            }
                        }
                        if (request.Titular == null)
                        {
                            Error error = new Error();
                            string[] err = AppSettings.EstructuraSinDatos;
                            error.codError = err[0];
                            error.mensaje = err[1].Replace("[estructura]", "Titular");
                            response.errorList.Add(error);
                        }
                        else
                        {
                            if (request.Titular.codTipoDoc == null)
                            {
                                Error error = new Error();
                                string[] err = AppSettings.CampoObligatorioSub;
                                error.codError = err[0];
                                error.mensaje = err[1].Replace("[campo]", "codTipoDoc").Replace("[codigo]", operacion).Replace("[estructura]", "Titular");
                                response.errorList.Add(error);
                            }
                            if (!util.CheckString(request.Titular.nroDocumento))
                            {
                                Error error = new Error();
                                string[] err = AppSettings.CampoObligatorioSub;
                                error.codError = err[0];
                                error.mensaje = err[1].Replace("[campo]", "nroDocumento").Replace("[codigo]", operacion).Replace("[estructura]", "Titular");
                                response.errorList.Add(error);
                            }
                        }

                        if (request.Asegurado == null)
                        {
                            Error error = new Error();
                            string[] err = AppSettings.EstructuraSinDatos;
                            error.codError = err[0];
                            error.mensaje = err[1].Replace("[estructura]", "Asegurado");
                            response.errorList.Add(error);
                        }
                        else
                        {
                            if (request.Asegurado.codTipoDoc == null)
                            {
                                Error error = new Error();
                                string[] err = AppSettings.CampoObligatorioSub;
                                error.codError = err[0];
                                error.mensaje = err[1].Replace("[campo]", "codTipoDoc").Replace("[codigo]", operacion).Replace("[estructura]", "Asegurado");
                                response.errorList.Add(error);
                            }
                            if (!util.CheckString(request.Asegurado.nroDocumento))
                            {
                                Error error = new Error();
                                string[] err = AppSettings.CampoObligatorioSub;
                                error.codError = err[0];
                                error.mensaje = err[1].Replace("[campo]", "nroDocumento").Replace("[codigo]", operacion).Replace("[estructura]", "Asegurado");
                                response.errorList.Add(error);
                            }
                        }

                        if (request.Preapertura == null)
                        {
                            Error error = new Error();
                            string[] err = AppSettings.CampoObligatorioSub;
                            error.codError = err[0];
                            error.mensaje = err[1].Replace("[campo]", "Preapertura");
                            response.errorList.Add(error);
                        }
                        else
                        {
                            if (request.Preapertura.codDepartamento == null)
                            {
                                Error error = new Error();
                                string[] err = AppSettings.CampoObligatorio;
                                error.codError = err[0];
                                error.mensaje = err[1].Replace("[campo]", "codDepartamento").Replace("[codigo]", operacion);
                                response.errorList.Add(error);
                            }
                            if (request.Preapertura.codDistrito == null)
                            {
                                Error error = new Error();
                                string[] err = AppSettings.CampoObligatorio;
                                error.codError = err[0];
                                error.mensaje = err[1].Replace("[campo]", "codDistrito").Replace("[codigo]", operacion);
                                response.errorList.Add(error);
                            }
                            if (request.Preapertura.codProvincia == null)
                            {
                                Error error = new Error();
                                string[] err = AppSettings.CampoObligatorio;
                                error.codError = err[0];
                                error.mensaje = err[1].Replace("[campo]", "codProvincia").Replace("[codigo]", operacion);
                                response.errorList.Add(error);
                            }
                            if (!util.CheckString(request.Preapertura.fecOcurrencia))
                            {
                                Error error = new Error();
                                string[] err = AppSettings.CampoObligatorio;
                                error.codError = err[0];
                                error.mensaje = err[1].Replace("[campo]", "fecOcurrencia").Replace("[codigo]", operacion);
                                response.errorList.Add(error);
                            }
                        }
                    }
                    else
                    {
                        operacion = "actualización";

                    }

                    if (!util.CheckString(request.usuario))
                    {
                        Error error = new Error();
                        string[] err = AppSettings.CampoObligatorio;
                        error.codError = err[0];
                        error.mensaje = err[1].Replace("[campo]", "usuario").Replace("[codigo]", operacion);
                        response.errorList.Add(error);
                    }
                    if (request.nroCaso == null)
                    {
                        Error error = new Error();
                        string[] err = AppSettings.CampoObligatorio;
                        error.codError = err[0];
                        error.mensaje = err[1].Replace("[campo]", "nroCaso").Replace("[codigo]", operacion);
                        response.errorList.Add(error);
                    }
                    if (request.codEstado == null)
                    {
                        Error error = new Error();
                        string[] err = AppSettings.CampoObligatorio;
                        error.codError = err[0];
                        error.mensaje = err[1].Replace("[campo]", "codEstado").Replace("[codigo]", operacion);
                        response.errorList.Add(error);
                    }
                }
                else
                {
                    Error error = new Error();
                    string[] err = AppSettings.OperacionNoEnviada;
                    error.codError = err[0];
                    error.mensaje = err[1];
                    response.errorList.Add(error);
                }

            }

            if (response.errorList.Count > 0)
            {
                response.respuesta = false;
                response.mensaje = "La operación no se pudo completar";
            }
            else
            {
                response.respuesta = true;
            }
            return response;
        }
        public CommonResponse ValidarTicket360(RegTicketRequest ticket)
        {CommonResponse response = new CommonResponse();
            List<string> mensajes = new List<string>();
            Boolean isValidate = true;
            String mensaje;
            int tipoTicket = ticket.Tipo;
            string docClient = ticket.docClient;
            string tipoDocClient = ticket.tipoDocClient;
            string UbigeoTitular = ticket.UbigeoTitular;
            String EmailTitular = ticket.EmailTitular;
            int ViaRespuesta_Id = 0;
            String Canal = ticket.Canal;
            String Producto = ticket.Producto;
            String Ramo = ticket.Ramo;
            String Descripcion = ticket.Descripcion;
            String FechaRecepcion = ticket.FechaRecepcion;
            String Usuario = ticket.Usuario;
            String Poliza = ticket.Poliza;
            String Reconsideracion = ticket.Reconsideracion;
            String Monto = ticket.Monto;
            int ViaRecepcion_Id;
            int SubMotivo_Id;
            int Motivo_Id;
            List<Archivo> archivosAdjuntosTicket = ticket.Adjuntos;
            int estadoTicket = ticket.Estado;

            const int tipoTicket_solicitud = 1;
            const int tipoTicket_tramite = 2;
            const int tipoTicket_consulta = 3;
            const int tipoTicket_reclamo = 4;
            const int estadoTicket_sinregistrar = 0;
            const int estadoTicket_cerrado = 8;


            Regex oValidarEnteros = new Regex("[^0-9]");
            Regex oValidarreales = new Regex("^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$");
            Regex oValiFechas = new Regex(@"^(0?[1-9]|[12][0-9]|3[01])[\/\-](0?[1-9]|1[012])[\/\-]\d{4}$");
            Regex oValiEmail = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");

            if (object.ReferenceEquals(null, ticket))
            {
                isValidate = false;
                mensaje = "Ticket  inválido.";
                mensajes.Add(mensaje);
            }

            if (String.IsNullOrEmpty(Canal) || String.Equals(Canal, "0"))
            {
                isValidate = false;
                mensaje = "Canal  inválido.";
                mensajes.Add(mensaje);
            }
            else
            {
                if (oValidarEnteros.IsMatch(Canal))
                {
                    isValidate = false;
                    mensaje = "El canal solo puede  tener  valores   enteros.";
                    mensajes.Add(mensaje);
                }
            }

            if (String.IsNullOrEmpty(Producto) || String.Equals(Producto, "0"))
            {
                isValidate = false;
                mensaje = "Producto  inválido.";
                mensajes.Add(mensaje);
            }
            else
            {
                if (oValidarEnteros.IsMatch(Producto))
                {
                    isValidate = false;
                    mensaje = "El código del producto solo puede  tener  valores   enteros.";
                    mensajes.Add(mensaje);
                }
            }
            if (String.IsNullOrEmpty(Ramo) || String.Equals(Ramo, "0"))
            {
                isValidate = false;
                mensaje = "Ramo  inválido.";
                mensajes.Add(mensaje);
            }
            else
            {
                if (oValidarEnteros.IsMatch(Ramo))
                {
                    isValidate = false;
                    mensaje = "El código del ramo solo puede  tener  valores   enteros.";
                    mensajes.Add(mensaje);
                }
            }
            if (String.IsNullOrEmpty(Descripcion))
            {
                isValidate = false;
                mensaje = "Descripción  inválida.";
                mensajes.Add(mensaje);
            }
            if (String.IsNullOrEmpty(FechaRecepcion))
            {
                isValidate = false;
                mensaje = "Fecha de  recepción inválida.";
                mensajes.Add(mensaje);
            }
            else
            {
                if (!oValiFechas.IsMatch(FechaRecepcion))
                {
                    isValidate = false;
                    mensaje = "El formato de la fecha de recepción es inválido.";
                    mensajes.Add(mensaje);
                }
                else
                {
                    if (!DateTime.TryParseExact(FechaRecepcion, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dt))
                    {
                        isValidate = false;
                        mensaje = "Fecha de recepción no existe.";
                        mensajes.Add(mensaje);
                    }
                }
            }
            if (String.IsNullOrEmpty(Usuario))
            {
                isValidate = false;
                mensaje = "Usuario inválido.";
                mensajes.Add(mensaje);
            }
            else
            {
                if (oValidarEnteros.IsMatch(Usuario))
                {
                    isValidate = false;
                    mensaje = "El código del usuario solo puede  tener  valores   enteros.";
                    mensajes.Add(mensaje);
                }
            }
            if (String.IsNullOrEmpty(Poliza))
            {
                isValidate = false;
                mensaje = "Poliza inválida.";
                mensajes.Add(mensaje);
            }
            else
            {
                if (oValidarEnteros.IsMatch(Poliza))
                {
                    isValidate = false;
                    mensaje = "El código de  la póliza solo puede  tener  valores   enteros.";
                    mensajes.Add(mensaje);
                }
            }
            if (String.IsNullOrEmpty(docClient))
            {
                isValidate = false;
                mensaje = "Documento del titular inválida.";
                mensajes.Add(mensaje);
            }
            else
            {
                if (oValidarEnteros.IsMatch(docClient))
                {
                    isValidate = false;
                    mensaje = "El código del titular solo puede  tener  valores   enteros.";
                    mensajes.Add(mensaje);
                }

            }
            if (String.IsNullOrEmpty(tipoDocClient))
            {
                isValidate = false;
                mensaje = "Tipo del documento del titular inválido.";
                mensajes.Add(mensaje);
            }
            else
            {
                if (oValidarEnteros.IsMatch(tipoDocClient))
                {
                    isValidate = false;
                    mensaje = "El código del tipo del cliente solo puede  tener  valores   enteros.";
                    mensajes.Add(mensaje);
                }
            }

            if (!String.IsNullOrEmpty(UbigeoTitular))
            {
                if (oValidarEnteros.IsMatch(UbigeoTitular))
                {
                    isValidate = false;
                    mensaje = "El código del ubigeo solo puede  tener  valores   enteros.";
                    mensajes.Add(mensaje);
                }
            }
            if (!String.IsNullOrEmpty(Reconsideracion))
            {
                if (oValidarEnteros.IsMatch(Reconsideracion))
                {
                    isValidate = false;
                    mensaje = "El código de la reconsideración solo puede  tener  valores   enteros";
                    mensajes.Add(mensaje);
                }
            }
            if (!String.IsNullOrEmpty(Monto))
            {
                if (!oValidarreales.IsMatch(Monto))
                {
                    isValidate = false;
                    mensaje = "El formato del monto es inválido.";
                    mensajes.Add(mensaje);
                }
            }


            if (!(estadoTicket == estadoTicket_sinregistrar || estadoTicket == estadoTicket_cerrado))
            {
                isValidate = false;
                mensaje = "Estado del ticket inválido.";
                mensajes.Add(mensaje);
            }
            else
            {
                if (!(tipoTicket == tipoTicket_tramite || tipoTicket == tipoTicket_consulta) && estadoTicket == estadoTicket_cerrado)
                {
                    isValidate = false;
                    mensaje = "Estado del ticket inválido.";
                    mensajes.Add(mensaje);
                }
            }

            if (object.ReferenceEquals(null, ticket.ViaRecepcion))
            {
                isValidate = false;
                mensaje = "Via recepción  inválido.";
                mensajes.Add(mensaje);
            }
            else
            {
                ViaRecepcion_Id = ticket.ViaRecepcion.Id;
                if (ViaRecepcion_Id == 0)
                {
                    isValidate = false;
                    mensaje = "Código Via recepción inválido.";
                    mensajes.Add(mensaje);
                }
            }

            if (object.ReferenceEquals(null, ticket.SubMotivo))
            {
                isValidate = false;
                mensaje = "Submotivo  inválido.";
                mensajes.Add(mensaje);
                mensaje = "Mótivo  inválido.";
                mensajes.Add(mensaje);
            }
            else
            {
                SubMotivo_Id = ticket.SubMotivo.Id;
                if (SubMotivo_Id == 0)
                {
                    isValidate = false;
                    mensaje = "Código Submotivo inválido.";
                    mensajes.Add(mensaje);
                }

                if (object.ReferenceEquals(null, ticket.SubMotivo.Motivo))
                {
                    isValidate = false;
                    mensaje = "Motivo  inválido.";
                    mensajes.Add(mensaje);
                }
                else
                {
                    Motivo_Id = ticket.SubMotivo.Motivo.Id;
                    if (Motivo_Id == 0)
                    {
                        isValidate = false;
                        mensaje = "Código motivo inválido.";
                        mensajes.Add(mensaje);
                    }
                }
            }
            if (ticket.Tipo == 0)
            {
                isValidate = false;
                mensaje = "Tipo  ticket  inválido.";
                mensajes.Add(mensaje);
            }
            else
            {
                if (tipoTicket == tipoTicket_reclamo || tipoTicket == tipoTicket_solicitud)
                {
                    //  solo pra reclamos  y  solicitudes
                    if (object.ReferenceEquals(null, ticket.ViaRespuesta))
                    {
                        isValidate = false;
                        mensaje = "Via respuestas inválida.";
                        mensajes.Add(mensaje);
                    }
                    else
                    {
                        ViaRespuesta_Id = ticket.ViaRespuesta.Id;
                        if (ViaRespuesta_Id == 0)
                        {
                            isValidate = false;
                            mensaje = "Código Via respuesta inválido.";
                            mensajes.Add(mensaje);
                        }
                    }
                }
            }

            if (object.ReferenceEquals(null, ticket.Contacto))
            {
                isValidate = false;
                mensaje = "Datos del contacto / titular inválidos.";
                mensajes.Add(mensaje);
            }
            else
            {
                if (!String.IsNullOrEmpty(ticket.Contacto.tipodoc))
                {
                    if (oValidarEnteros.IsMatch(ticket.Contacto.tipodoc))
                    {
                        isValidate = false;
                        mensaje = "El código del tipo documento del contacto / titular solo puede  tener  valores   enteros.";
                        mensajes.Add(mensaje);
                    }
                }
                if (String.IsNullOrEmpty(ticket.Contacto.documento))
                {
                    isValidate = false;
                    mensaje = "Documento del contacto / titular inválido.";
                    mensajes.Add(mensaje);
                }
                else
                {
                    //es titular
                    if (String.Equals(docClient, ticket.Contacto.documento))
                    {
                        if (oValidarEnteros.IsMatch(ticket.Contacto.documento))
                        {
                            isValidate = false;
                            mensaje = "El número del documento del titular solo puede  tener  valores   enteros.";
                            mensajes.Add(mensaje);
                        }
                        if (tipoTicket != tipoTicket_reclamo && tipoTicket != tipoTicket_solicitud)
                        {
                            if (tipoTicket != tipoTicket_consulta)
                            {
                                if (EmailTitular == "0" || String.IsNullOrEmpty(EmailTitular))
                                {
                                    isValidate = false;
                                    mensaje = "Email inválido.";
                                    mensajes.Add(mensaje);
                                }
                            }
                        }
                        else
                        {
                            if ((EmailTitular == "0" || String.IsNullOrEmpty(EmailTitular)) && ViaRespuesta_Id == 2)
                            {
                                isValidate = false;
                                mensaje = "Titular no tiene un e-mail relacionado.";
                                mensajes.Add(mensaje);
                            }
                            if ((UbigeoTitular == "0" || String.IsNullOrEmpty(UbigeoTitular)) && ViaRespuesta_Id == 1)
                            {
                                isValidate = false;
                                mensaje = "Titular no tiene una dirección relacionado.";
                                mensajes.Add(mensaje);
                            }
                        }

                        if (!String.IsNullOrEmpty(EmailTitular))
                        {
                            if (!oValiEmail.IsMatch(EmailTitular))
                            {
                                isValidate = false;
                                mensaje = "El formato del e-mail del titular is inválido.";
                                mensajes.Add(mensaje);
                            }
                        }
                    }
                    else
                    {
                        if (oValidarEnteros.IsMatch(ticket.Contacto.documento))
                        {
                            isValidate = false;
                            mensaje = "El número del documento del contacto solo puede  tener  valores   enteros.";
                            mensajes.Add(mensaje);
                        }
                    }
                }
            }

            if (!object.ReferenceEquals(null, archivosAdjuntosTicket))
            {
                int cantMime = archivosAdjuntosTicket.Count(x => x.mime == null);
                int cantName = archivosAdjuntosTicket.Count(x => x.name == null);
                int cantContent = archivosAdjuntosTicket.Count(x => x.content == null);

                if (cantMime != 0 || cantName != 0 || cantContent != 0)
                {
                    isValidate = false;
                    mensaje = "Los archivos adjuntos debene contener nombre, tipo mime y contenido.";
                    mensajes.Add(mensaje);
                }

            };

            response.respuesta = isValidate;
            response.mensajes = mensajes;
            return response;
        }
        public CommonResponse ValidarTicket360New(RegContractRequest contract)
        {
            CommonResponse response = new CommonResponse();
            List<string> mensajes = new List<string>();
            Boolean isValidate = true;
            String mensaje;
            int tipoTicket = contract.Tipo;
            string docClient = contract.nroDocAseg;
            int tipoDocClient = contract.tipoDocAseg;
            String correoAseg = contract.correoAseg;
            int ViaRespuesta_Id = 0;
            String Canal = contract.Canal;
            int ramo = contract.ramo;
            String summary = contract.summary;
            DateTime FechaRecepcion = contract.fechaRecepcion;
            String Usuario = contract.Usuario;
            String Poliza = contract.Poliza;
            String Reconsideracion = contract.Reconsideracion;
            String Monto = contract.Monto;
            int ViaRecepcion_Id;
            int SubMotivo_Id;
            int Motivo_Id;
            List<Adjunto> archivosAdjuntosTicket = contract.adjunto;
            int estadoTicket = contract.Estado;

            const int tipoTicket_solicitud = 1;
            const int tipoTicket_tramite = 2;
            const int tipoTicket_consulta = 3;
            const int tipoTicket_reclamo = 4;
            const int estadoTicket_sinregistrar = 0;
            const int estadoTicket_cerrado = 8;


            Regex oValidarEnteros = new Regex("[^0-9]");
            Regex oValidarreales = new Regex("^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$");
            Regex oValiFechas = new Regex(@"^(0?[1-9]|[12][0-9]|3[01])[\/\-](0?[1-9]|1[012])[\/\-]\d{4}$");
            Regex oValiEmail = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");

            if (object.ReferenceEquals(null, contract))
            {
                isValidate = false;
                mensaje = "Ticket  inválido.";
                mensajes.Add(mensaje);
            }
            else
            {
                if (oValidarEnteros.IsMatch(Canal))
                {
                    isValidate = false;
                    mensaje = "El canal solo puede  tener  valores   enteros.";
                    mensajes.Add(mensaje);
                }
            }
            if (ramo == null || ramo == 0)
            {
                isValidate = false;
                mensaje = "Ramo inválido.";
                mensajes.Add(mensaje);
            }
            else
            {
                if (oValidarEnteros.IsMatch(ramo.ToString()))
                {
                    isValidate = false;
                    mensaje = "El código del ramo solo puede  tener  valores   enteros.";
                    mensajes.Add(mensaje);
                }
            }
            if (String.IsNullOrEmpty(summary))
            {
                isValidate = false;
                mensaje = "Descripción  inválida.";
                mensajes.Add(mensaje);
            }
            if (FechaRecepcion == DateTime.MinValue)
            {
                isValidate = false;
                mensaje = "Fecha de recepción inválida.";
                mensajes.Add(mensaje);
            }
            else
            {
                if (!oValiFechas.IsMatch(FechaRecepcion.ToString()))
                {
                    isValidate = false;
                    mensaje = "El formato de la fecha de recepción es inválido.";
                    mensajes.Add(mensaje);
                }
                else
                {
                    if (!DateTime.TryParseExact(FechaRecepcion.ToString(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dt))
                    {
                        isValidate = false;
                        mensaje = "Fecha de recepción no existe.";
                        mensajes.Add(mensaje);
                    }
                }
            }
            if (String.IsNullOrEmpty(Usuario))
            {
                isValidate = false;
                mensaje = "Usuario inválido.";
                mensajes.Add(mensaje);
            }
            else
            {
                if (oValidarEnteros.IsMatch(Usuario))
                {
                    isValidate = false;
                    mensaje = "El código del usuario solo puede  tener  valores   enteros.";
                    mensajes.Add(mensaje);
                }
            }
            if (String.IsNullOrEmpty(Poliza))
            {
                isValidate = false;
                mensaje = "Poliza inválida.";
                mensajes.Add(mensaje);
            }
            else
            {
                if (oValidarEnteros.IsMatch(Poliza))
                {
                    isValidate = false;
                    mensaje = "El código de  la póliza solo puede  tener  valores   enteros.";
                    mensajes.Add(mensaje);
                }
            }
            if (String.IsNullOrEmpty(docClient))
            {
                isValidate = false;
                mensaje = "Documento del titular inválida.";
                mensajes.Add(mensaje);
            }
            else
            {
                if (oValidarEnteros.IsMatch(docClient))
                {
                    isValidate = false;
                    mensaje = "El código del titular solo puede  tener  valores   enteros.";
                    mensajes.Add(mensaje);
                }

            }
            if (tipoDocClient == 0)
            {
                isValidate = false;
                mensaje = "Tipo del documento del titular inválido.";
                mensajes.Add(mensaje);
            }
            else
            {
                if (oValidarEnteros.IsMatch(tipoDocClient.ToString()))
                {
                    isValidate = false;
                    mensaje = "El código del tipo del cliente solo puede  tener  valores   enteros.";
                    mensajes.Add(mensaje);
                }
            }
            if (!String.IsNullOrEmpty(Reconsideracion))
            {
                if (oValidarEnteros.IsMatch(Reconsideracion))
                {
                    isValidate = false;
                    mensaje = "El código de la reconsideración solo puede  tener  valores   enteros";
                    mensajes.Add(mensaje);
                }
            }
            if (!String.IsNullOrEmpty(Monto))
            {
                if (!oValidarreales.IsMatch(Monto))
                {
                    isValidate = false;
                    mensaje = "El formato del monto es inválido.";
                    mensajes.Add(mensaje);
                }
            }
            if (!(estadoTicket == estadoTicket_sinregistrar || estadoTicket == estadoTicket_cerrado))
            {
                isValidate = false;
                mensaje = "Estado del ticket inválido.";
                mensajes.Add(mensaje);
            }
            else
            {
                if (!(tipoTicket == tipoTicket_tramite || tipoTicket == tipoTicket_consulta) && estadoTicket == estadoTicket_cerrado)
                {
                    isValidate = false;
                    mensaje = "Estado del ticket inválido.";
                    mensajes.Add(mensaje);
                }
            }

            if (object.ReferenceEquals(null, contract.ViaRecepcion))
            {
                isValidate = false;
                mensaje = "Via recepción  inválido.";
                mensajes.Add(mensaje);
            }
            else
            {
                ViaRecepcion_Id = contract.ViaRecepcion.Id;
                if (ViaRecepcion_Id == 0)
                {
                    isValidate = false;
                    mensaje = "Código Via recepción inválido.";
                    mensajes.Add(mensaje);
                }
            }

            if (object.ReferenceEquals(null, contract.SubMotivo))
            {
                isValidate = false;
                mensaje = "Submotivo  inválido.";
                mensajes.Add(mensaje);
                mensaje = "Mótivo  inválido.";
                mensajes.Add(mensaje);
            }
            else
            {
                SubMotivo_Id = contract.SubMotivo.Id;
                if (SubMotivo_Id == 0)
                {
                    isValidate = false;
                    mensaje = "Código Submotivo inválido.";
                    mensajes.Add(mensaje);
                }

                if (object.ReferenceEquals(null, contract.SubMotivo.Motivo))
                {
                    isValidate = false;
                    mensaje = "Motivo  inválido.";
                    mensajes.Add(mensaje);
                }
                else
                {
                    Motivo_Id = contract.SubMotivo.Motivo.Id;
                    if (Motivo_Id == 0)
                    {
                        isValidate = false;
                        mensaje = "Código motivo inválido.";
                        mensajes.Add(mensaje);
                    }
                }
            }
            if (contract.Tipo == 0)
            {
                isValidate = false;
                mensaje = "Tipo  ticket  inválido.";
                mensajes.Add(mensaje);
            }
            else
            {
                if (tipoTicket == tipoTicket_reclamo || tipoTicket == tipoTicket_solicitud)
                {
                    //  solo pra reclamos  y  solicitudes
                    if (object.ReferenceEquals(null, contract.ViaRespuesta))
                    {
                        isValidate = false;
                        mensaje = "Via respuestas inválida.";
                        mensajes.Add(mensaje);
                    }
                    else
                    {
                        ViaRespuesta_Id = contract.ViaRespuesta.Id;
                        if (ViaRespuesta_Id == 0)
                        {
                            isValidate = false;
                            mensaje = "Código Via respuesta inválido.";
                            mensajes.Add(mensaje);
                        }
                    }
                }
            }

            if (object.ReferenceEquals(null, contract))
            {
                isValidate = false;
                mensaje = "Datos del contacto / titular inválidos.";
                mensajes.Add(mensaje);
            }
            else
            {
                if (!String.IsNullOrEmpty(contract.tipoDocAseg.ToString()))
                {
                    if (oValidarEnteros.IsMatch(contract.tipoDocAseg.ToString()))
                    {
                        isValidate = false;
                        mensaje = "El código del tipo documento del contacto / titular solo puede  tener  valores   enteros.";
                        mensajes.Add(mensaje);
                    }
                }
                if (String.IsNullOrEmpty(contract.nroDocAseg))
                {
                    isValidate = false;
                    mensaje = "Documento del contacto / titular inválido.";
                    mensajes.Add(mensaje);
                }
                else
                {
                    //es titular
                    if (String.Equals(docClient, contract.nroDocAseg))
                    {
                        if (oValidarEnteros.IsMatch(contract.nroDocAseg))
                        {
                            isValidate = false;
                            mensaje = "El número del documento del titular solo puede  tener  valores   enteros.";
                            mensajes.Add(mensaje);
                        }
                        if (tipoTicket != tipoTicket_reclamo && tipoTicket != tipoTicket_solicitud)
                        {
                            if (tipoTicket != tipoTicket_consulta)
                            {
                                if (correoAseg == "0" || String.IsNullOrEmpty(correoAseg))
                                {
                                    isValidate = false;
                                    mensaje = "Email inválido.";
                                    mensajes.Add(mensaje);
                                }
                            }
                        }
                        else
                        {
                            if ((correoAseg == "0" || String.IsNullOrEmpty(correoAseg)) && ViaRespuesta_Id == 2)
                            {
                                isValidate = false;
                                mensaje = "Titular no tiene un e-mail relacionado.";
                                mensajes.Add(mensaje);
                            }
                        }

                        if (!String.IsNullOrEmpty(correoAseg))
                        {
                            if (!oValiEmail.IsMatch(correoAseg))
                            {
                                isValidate = false;
                                mensaje = "El formato del e-mail del titular is inválido.";
                                mensajes.Add(mensaje);
                            }
                        }
                    }
                    else
                    {
                        if (oValidarEnteros.IsMatch(contract.nroDocAseg))
                        {
                            isValidate = false;
                            mensaje = "El número del documento del contacto solo puede  tener  valores   enteros.";
                            mensajes.Add(mensaje);
                        }
                    }
                }
            }

            if (!object.ReferenceEquals(null, archivosAdjuntosTicket))
            {
                int cantMime = archivosAdjuntosTicket.Count(x => x.mime == null);
                int cantName = archivosAdjuntosTicket.Count(x => x.name == null);
                int cantContent = archivosAdjuntosTicket.Count(x => x.content == null);

                if (cantMime != 0 || cantName != 0 || cantContent != 0)
                {
                    isValidate = false;
                    mensaje = "Los archivos adjuntos debene contener nombre, tipo mime y contenido.";
                    mensajes.Add(mensaje);
                }

            };

            response.respuesta = isValidate;
            response.mensajes = mensajes;
            return response;
        }
    }

}
