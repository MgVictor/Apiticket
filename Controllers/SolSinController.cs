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
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using System.Threading.Tasks;

namespace apiTicket.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolSinController : Controller
    {
        private readonly ISolSinService _SolSinService;
        private readonly IHostingEnvironment _HostEnvironment;
        private readonly IOptions<AppSettings> appSettings;

        private readonly IConfiguration _config;
        public SolSinController(IHostingEnvironment HostEnvironment, IOptions<AppSettings> appSettings, IConfiguration configuration)
        {
            // this._SolSinService = solSinService;
            _HostEnvironment = HostEnvironment;
            this.appSettings = appSettings;
            this._config = configuration;
        }

        [HttpGet]
        [Authorize]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "ticket1", "ticket2" };
        }

        [HttpPost]
        [Route("GetTokenSin")]
        public IActionResult GetToken(SessionUser session)
        {
            try
            {
                var secretKey = _config.GetValue<string>("Jwt:SecretKey");
                var issuer = _config.GetValue<string>("Jwt:Issuer");
                var audience = _config.GetValue<string>("Jwt:Audience");
                var expirationMinutes = _config.GetValue<int>("Jwt:ExpirationMinutes");

                var token = GenerateJwtToken2(secretKey, issuer, audience, session.username, expirationMinutes);
                return Ok(new { token });
            }
            catch
            {
                return Unauthorized();
            }
        }

        [HttpGet]
        [Route("GetTokenSinietro")]
        public string GenerateJwtToken2(string secretKey, string issuer, string audience, string username, int expirationMinutes)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,

                expires: DateTime.Now.AddMinutes(expirationMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("Manage/RegContract")]
        [Authorize]
        public async Task<IActionResult> RegistrarTicketNew([FromBody] RegContractRequest request)
        {

            List<Adjunto> archivosAdjuntosTicket = request.adjunto;
            RegiTicketResponse _objReturn = new RegiTicketResponse();
            CommonResponse oValidarTicket360 = new CommonResponse();
            ValidateTicket oValidateTicket = new ValidateTicket();
            string codeTicket;
            Contract ticket;
            string data;
            int tipoTicket = request.Tipo;
            int estadoTicket = request.Estado;
            const int tipoTicket_solicitud = 1;

            oValidarTicket360 = oValidateTicket.ValidarTicket360New(request);
            if (!oValidarTicket360.respuesta)
            {
                _objReturn.respuesta = oValidarTicket360.respuesta;
                _objReturn.mensaje = "Ocurrio un error al validar los datos del ticket.";
                _objReturn.mensajes = oValidarTicket360.mensajes;
                return Ok(_objReturn);
            }
            oValidarTicket360 = this._SolSinService.ValidateTicketNew(request);
            if (!oValidarTicket360.respuesta)
            {
                _objReturn.respuesta = oValidarTicket360.respuesta;
                _objReturn.mensaje = "Ocurrio un error al validar los datos del ticket.";
                _objReturn.mensajes = oValidarTicket360.mensajes;
                return Ok(_objReturn);
            }

            _objReturn = this._SolSinService.SetTicketNew(request);
            codeTicket = _objReturn.Codigo;

            if (_objReturn.respuesta)
            {

                ticket = await this._SolSinService.GetTicketNew(codeTicket, "SGC");
                if (String.IsNullOrEmpty(ticket.Codigo))
                {
                    _objReturn.respuesta = false;
                    _objReturn.mensaje = "Ocurrio un error al consultar los datos del ticket.";
                    return Ok(_objReturn);
                }

                if (object.ReferenceEquals(null, archivosAdjuntosTicket))
                {
                    archivosAdjuntosTicket = new List<Adjunto>();
                }

                List<Adjunto> archivosAdjuntosAnt = this._SolSinService.GetAdjuntos(codeTicket, "1");

                foreach (Adjunto archivo in archivosAdjuntosAnt)
                {
                    archivosAdjuntosTicket.Add(archivo);
                }

                if (tipoTicket == tipoTicket_solicitud)
                {
                    try
                    {
                        switch (tipoTicket)
                        {
                            case 1:
                                data = this._SolSinService.GenerateSolicitud(ticket);
                                archivosAdjuntosTicket.Add(
                                    new Adjunto { name = "Solicitud-" + ticket.Codigo + ".pdf", mime = "application/pdf", scode = ticket.Codigo, size = "2MB", tipo = "1", content = data }
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
                foreach (Adjunto archivo in archivosAdjuntosTicket)
                {
                    archivo.scode = codeTicket;
                }

                if (archivosAdjuntosTicket.Count != 0)
                {
                    List<Adjunto> archivos_s3;
                    try
                    {
                        archivos_s3 = this._SolSinService.S3Adjuntar(archivosAdjuntosTicket, "SGC");
                    }
                    catch (Exception)
                    {
                        _objReturn.respuesta = false;
                        _objReturn.mensaje = "Ocurrio un error al adjuntar los archivos  en la nube.";
                        return Ok(_objReturn);
                    }

                    foreach (Adjunto archivo in archivos_s3)
                    {
                        archivo.path = archivo.path_gd;
                    }
                    try
                    {
                        this._SolSinService.SetArchivosAdjunto(archivos_s3);
                    }
                    catch (Exception)
                    {
                        _objReturn.respuesta = false;
                        _objReturn.mensaje = "Ocurrio un error al guardar los archivos.";
                        return Ok(_objReturn);
                    }
                }
                if (tipoTicket == tipoTicket_solicitud)
                {
                    try
                    {
                        this._SolSinService.GestionaRegistroJIRANew(codeTicket, "SGC");
                    }
                    catch (Exception)
                    {
                        _objReturn.respuesta = false;
                        _objReturn.mensaje = "Ocurrio un error al registrar  en Jira.";
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
    
    }
}