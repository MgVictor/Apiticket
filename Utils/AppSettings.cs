using System;

namespace apiTicket.Utils
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public string ConnectionStringORA { get; set; }
        public string ConnectionStringTimeP { get; set; }
        public string ConnectionStringConciliacion { get; set; }
        public static String[] CampoObligatorio => Util.ObtainConfig("CAMPO_OBLIGATORIO");
        public static String[] TipoDatoNumerico => Util.ObtainConfig("TIPO_DATO_NO_NUMERICO");
        public static String[] EstructuraSinDatos => Util.ObtainConfig("ESTRUCTURA_SIN_DATOS");
        public static String[] OperacionNoEnviada => Util.ObtainConfig("OPERACION_NO_ENVIADA");
        public static String[] CampoObligatorioSub => Util.ObtainConfig("CAMPO_OBLIGATORIO_SUB");
        public static string AWSRegistrar => Util.ObtainConfigAWS("SERVICIOAWS_REGISTRAR");
        public static string AWSRegistrar2 => Util.ObtainConfigAWS("SERVICIOAWS_REGISTRAR2");
        public static string AWSKey => Util.ObtainConfigAWS("SERVICIOAWS_KEY");
        public static string AWSConsultar => Util.ObtainConfigAWS("SERVICIOAWS_CONSULTA");
        public static string AWSConsultar2 => Util.ObtainConfigAWS("SERVICIOAWS_CONSULTA2");
        public static string AWSAdjuntar => Util.ObtainConfigAWS("SERVICIOAWS_ADJUNTAR");
        public static string AWSGetAdjunto => Util.ObtainConfigAWS("SERVICIOAWS_GETADJUNTO");
        public static string GetTokenAwsSGS => Util.ObtainConfigAWS("URL_GET_TOKEN_SGC");
        public static string GetTokenAws360 => Util.ObtainConfigAWS("URL_GET_TOKEN_360");
        public static string GetUserName_SGC => Util.ObtainConfigAWS("USERNAME_SGC");
        public static string GetUserName_360 => Util.ObtainConfigAWS("USERNAME_360");
        public static string GetPassword_SGC => Util.ObtainConfigAWS("PASSWORD_SGC");
        public static string GetPassword_360 => Util.ObtainConfigAWS("PASSWORD_360");
        public static string GetScope => Util.ObtainConfigAWS("SCOPE");

        //hcama @mg  20220216  ini
        public static string ServicioWebGCliente => Util.ObtainConfigAWS("SERVICIOWEB_GCLIENTE");
        public static string UsuarioCorreo_CanalTicket_Clientes => Util.ObtainConfigAWS("USUARIOCORREO_CANALTICKET_CLIENTES");
        public static string PwdCorreo_CanalTicket_Clientes => Util.ObtainConfigAWS("PWDCORREO_CANALTICKET_CLIENTES");
        public static string UsuarioCorreo_CanalTicket_Corporativo => Util.ObtainConfigAWS("USUARIOCORREO_CANALTICKET_CORPORATIVO");
        public static string PwdCorreo_CanalTicket_Corporativo => Util.ObtainConfigAWS("PWDUARIOCORREO_CANALTICKET_CORPORATIVO");
        //hcama @mg  20220216  fin

    }
}