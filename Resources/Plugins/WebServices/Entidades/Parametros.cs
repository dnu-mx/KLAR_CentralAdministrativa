 using System;

namespace WebServices.Entidades
{
    public class Parametros
    {
        public class Headers
        {
            public string   URL                 { get; set; }
            public string   Metodo              { get; set; }
            public string   Token               { get; set; }
            public string   Credenciales        { get; set; }
        }

        public class LoginBody
        {
            public string NombreUsuario         { get; set; }
            public string Password              { get; set; }
        }

        #region Parabilium

        public class AltaPersonaMoralBody
        {
            public string Folio                 { get; set; }
            public string ClaveEmpresa          { get; set; }
            public string UsuarioAutorizador    { get; set; }
        }

        public class ActivarTarjetaBody
        {
            public string IDSolicitud           { get; set; }
            public string Tarjeta               { get; set; }
            public string MedioAcceso           { get; set; }
            public string TipoMedioAcceso       { get; set; }
        }

        public class BloquearTarjetaBody
        {
            public string Tarjeta               { get; set; }
            public string MedioAcceso           { get; set; }
            public string TipoMedioAcceso       { get; set; }
            public string MotivoBloqueo         { get; set; }
        }

        public class ConsultarTarjetaCreditoBody
        {
            public string IDSolicitud { get; set; }
            public string Tarjeta { get; set; }
            public string MedioAcceso { get; set; }
            public string TipoMedioAcceso { get; set; }
        }

        public class LoginBodyENG
        {
            public string user                  { get; set; }
            public string password              { get; set; }
        }

        public class BlackListBody
        {
            public string full_name             { get; set; }
        }

        public class BodyValidarMembresiaSams
        {
            public String   WsUsuario       { get; set; }
            public String   WsPassword      { get; set; }
            public String   NumeroMembresia { get; set; }
        }

        public class BodyCertificadosAmazon
        {
            public String WsUsuario { get; set; }
            public String WsPassword { get; set; }
            public int IdPedido { get; set; }
            public int IdProveedor { get; set; }
        }

        public class BodyCertificadosUber
        {
            public String WsUsuario { get; set; }
            public String WsPassword { get; set; }
            public int IdPedido { get; set; }
            public string IdProducto { get; set; }
        }

        public class EliminarTarjetaBody
        {
            public string IDSolicitud { get; set; }
            public string Tarjeta { get; set; }
            public string MedioAcceso { get; set; }
            public string TipoMedioAcceso { get; set; }
            public string MotivoCancelacion { get; set; }
        }

        public class CancelarCuentasBody
        {
            public string Empresa { get; set; }
            public string RegistroCliente { get; set; }
            public string Tarjeta { get; set; }
            public string MedioAcceso { get; set; }
            public string TipoMedioAcceso { get; set; }
        }

        public class DiferirOperacionBody
        {
            public string IdOperacion   { get; set; }
            public string Tarjeta       { get; set; }
            public string IdPlan        { get; set; }
        }

        #endregion

        #region Boveda Digital

        public class Headers_BD
        {
            public string URL           { get; set; }
            public string Metodo        { get; set; }
            public string Token         { get; set; }
            public string Credentials   { get; set; }
        }

        public class LoginBody_BD
        {
            public string user_name { get; set; }
            public string password  { get; set; }
        }

        public class Productos_BD
        {
            public string issuer_key            { get; set; }
            public string sub_bins_group_key    { get; set; }
        }

        public class Lots_BD
        {
            public string lot_key                   { get; set; }
            public string sub_bins_group_key        { get; set; }
            public string lot_type_key              { get; set; }
            public string manufacturing_type_key    { get; set; }
            public string items                     { get; set; }
            public string user                      { get; set; }
        }

        #endregion

        #region DNU Usuarios

        public class LoginDnuBody
        {
            public string   NombreUsuario   { get; set; }
            public string   Password        { get; set; }
            public Guid     Aplicacion      { get; set; }
            public string   Cifrado         { get; set; }

        }

        public class SmsBody
        {
            public Guid     UserID          { get; set; }
            public string   NombreUsuario   { get; set; }
            public string   Telefono        { get; set; }
            public string   TokenSMS        { get; set; }
        }

        #endregion

        #region AppConnect

        public class ConsultarUsuariosEmpresaBody
        {
            public string   idUsuario     { get; set; }
            public string   nombreUsuario { get; set; }
        }

        public class UsuariosEmpresaBody
        {
            public string   idUsuario       { get; set; }
            public string   nombreUsuario   { get; set; }
            public string   ClaveEmpresa    { get; set; }
            public string   NombreEmpresa   { get; set; }
            public string   CuentaEmpresa   { get; set; }
        }

        #endregion
    }
}
