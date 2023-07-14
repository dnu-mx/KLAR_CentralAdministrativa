using DALCentralAplicaciones.Utilidades;
using Interfases;
using Interfases.Exceptiones;
using Log_PCI;
using System;
using System.Configuration;
using WebServices;
using WebServices.Entidades;
using WebServices.Utilerias;

namespace DALCentroContacto.LogicaNegocio
{
    public class LNAppConnect
    {
        /// <summary>
        /// Establece las validaciones de negocio para ejecutar el método de Login del web service AppConnect
        /// </summary>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Respuesta del web service a la petición</returns>
        public static Parametros.Headers LoginWebService(ILogHeader logHeader)
        {
            LogPCI log = new LogPCI(logHeader);

            try
            {
                string wsLoginResp = null;
                Parametros.Headers _headers = new Parametros.Headers();
                Parametros.LoginBody _body = new Parametros.LoginBody();

                _headers.URL = Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString().Trim()),
                    "WsAppConnect_URL", logHeader).Valor;

                _body.NombreUsuario = Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString().Trim()),
                    "WsAppConnect_Usr", logHeader).Valor;
                _body.Password = Configuracion.Get(Guid.Parse(
                    ConfigurationManager.AppSettings["IDApplication"].ToString().Trim()),
                    "WsAppConnect_Pwd", logHeader).Valor;

                log.Info("INICIA AppConnect.Login()");
                wsLoginResp = AppConnect.Login(_headers, _body, logHeader);
                log.Info("TERMINA AppConnect.Login()");

                if (wsLoginResp.ToUpper().Contains("ERROR"))
                {
                    throw new CAppException(8006, wsLoginResp);
                }

                _headers.Token = wsLoginResp;
                _headers.Credenciales = Cifrado.Base64Encode(_body.NombreUsuario + ":" + _body.Password);

                return _headers;
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception err)
            {
                log.ErrorException(err);
                throw new CAppException(8011, "Falla al establecer establecer el login con el Servicio Web");
            }
        }

        /// <summary>
        /// Establece las validaciones de negocio para ejecutar el método de ConsultarUsuariosEmpresa del
        /// web service AppConnect
        /// </summary>
        /// <param name="losHeaders">Parámetros del header del método</param>
        /// <param name="elBody">Parámetros del cuerpo de la petición</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Respuesta del web service a la petición</returns>
        public static string ConsultaUsuarioEmpresa(Parametros.Headers losHeaders, Parametros.ConsultarUsuariosEmpresaBody elBody,
            ILogHeader logHeader)
        {
            LogPCI log = new LogPCI(logHeader);

            try
            {
                string wsConsUEResp = null;

                log.Info("INICIA AppConnect.ConsultaUsuarioEmpresa()");
                wsConsUEResp = AppConnect.ConsultaUsuarioEmpresa(losHeaders, elBody, logHeader);
                log.Info("TERMINA AppConnect.ConsultaUsuarioEmpresa()");

                if (wsConsUEResp.ToUpper().Contains("ERROR"))
                {
                    throw new CAppException(8006, wsConsUEResp);
                }

                return wsConsUEResp;
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception err)
            {
                log.ErrorException(err);
                throw new CAppException(8011, "Falla en la consulta de Usuario/Empresa");
            }
        }

        /// <summary>
        /// Establece las validaciones de negocio para ejecutar el método de UsuariosEmpresa del web service AppConnect
        /// </summary>
        /// <param name="losHeaders">Parámetros del header del método</param>
        /// <param name="elBody">Parámetros del cuerpo de la petición</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Respuesta del web service a la petición</returns>
        public static void ActualizaUsuarioEmpresa(Parametros.Headers losHeaders, Parametros.UsuariosEmpresaBody elBody,
            ILogHeader logHeader)
        {
            LogPCI log = new LogPCI(logHeader);

            try
            {
                string wsUEResp = null;

                log.Info("INICIA AppConnect.UsuariosEmpresa()");
                wsUEResp = AppConnect.UsuariosEmpresa(losHeaders, elBody, logHeader);
                log.Info("TERMINA AppConnect.UsuariosEmpresa()");

                if (wsUEResp.ToUpper() != "OK")
                {
                    throw new CAppException(8006, wsUEResp);
                }
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception err)
            {
                log.ErrorException(err);
                throw new CAppException(8011, "Falla en la actualización de Usuario/Empresa");
            }
        }
    }
}
