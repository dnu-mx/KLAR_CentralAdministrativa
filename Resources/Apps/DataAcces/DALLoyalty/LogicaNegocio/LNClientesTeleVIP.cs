using DALAutorizador.Utilidades;
using DALCentroContacto.BaseDatos;
using DALCentroContacto.Entidades;
using Interfases;
using Interfases.Exceptiones;
using System;
using System.Configuration;
using System.Data;
using System.Text.RegularExpressions;

namespace DALCentroContacto.LogicaNegocio
{
    /// <summary>
    /// Establece la lógica de negocio para los Clientes TeleVIP
    /// </summary>
    public class LNClientesTeleVIP
    {
        #region Variables privadas

        /// <summary>
        /// Expresión regular de validación para email
        /// </summary>
        private const string regexEmail = @"^([\w-]+\.)*?[\w-]+@[\w-]+\.([\w-]+\.)*?[\w]+${7,100}";

        #endregion

        /// <summary>
        /// Establece las condiciones de validación para la consulta de clientes
        /// </summary>
        /// <param name="cliente">Datos de la entidad Cliente por actualizar</param>
        /// <param name="usuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DatSet con los registros resultados de la búsqueda en BD</returns>
        public static DataSet ConsultaClientes(Cliente cliente, IUsuario usuario, Guid AppID)
        {
            //Se verifica que se haya capturado al menos un criterio de búsqueda
            if (cliente.ID_Cliente == 0 && cliente.ID_Cuenta == 0 &&
                String.IsNullOrEmpty(cliente.MedioAcceso) && String.IsNullOrEmpty(cliente.ApellidoPaterno) &&
                String.IsNullOrEmpty(cliente.ApellidoMaterno) && String.IsNullOrEmpty(cliente.Nombre) &&
                String.IsNullOrEmpty(cliente.Email))
            {
                throw new CAppException(8006, "Selecciona al menos un criterio de búsqueda");
            }

            //Si se ingresó el correo electrónico, se verifica que sea una cadena válida
            if (!String.IsNullOrEmpty(cliente.Email))
            {
                Match matchExpression;
                Regex matchEmail = new Regex(regexEmail);

                matchExpression = matchEmail.Match(cliente.Email);

                if (!matchExpression.Success)
                {
                    throw new CAppException(8006, "El Correo Electrónico que ingresaste no es una dirección válida.");
                }
            }

            try
            {
                return DAOConsultaClientesTeleVIP.ObtieneClientes(cliente, usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
            }

            catch (CAppException err)
            {
                Loguear.Error(err, usuario.ClaveUsuario);
                throw err;
            }

            catch (Exception err)
            {
                Loguear.Error(err, usuario.ClaveUsuario);
                throw err;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación de la consulta del resumen de los últimos meses del tag
        /// </summary>
        /// <param name="tag">Número de tag</param>
        /// <param name="cuenta">Cuenta televia</param>
        /// <param name="usuario">Usuario en sesión</param>
        /// <returns>Dataset con los registros obtenidos</returns>
        public static DataSet ConsultaResumenMeses(string tag, string cuenta, IUsuario usuario)
        {
            //Se verifica que se haya ingresado el tipo de cuenta
            if (String.IsNullOrEmpty(tag))
            {
                throw new CAppException(8006, "Por favor selecciona el Tag");
            }


            try
            {
                return DAOConsultaClientesTeleVIP.ObtieneResumenMeses(tag, cuenta, usuario);
            }

            catch (CAppException err)
            {
                Loguear.Error(err, usuario.ClaveUsuario);
                throw err;
            }

            catch (Exception err)
            {
                Loguear.Error(err, usuario.ClaveUsuario);
                throw err;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación de la consulta de movimientos del tag
        /// </summary>
        /// <param name="fechaInicial">Fecha inicial del periodo de consulta</param>
        /// <param name="fechaFinal">Fecha final del periodo de consulta</param>
        /// <param name="tag">Número de tag</param>
        /// <param name="cuenta">Cuenta televia</param>
        /// <param name="usuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>Dataset con los registros obtenidos</returns>
        public static DataSet ConsultaMovimientos(DateTime fechaInicial, DateTime fechaFinal, string tag, string cuenta,
            IUsuario usuario)
        {
            //Se verifica que se hayan capturado las dos fechas
            if (DateTime.Compare(fechaInicial, DateTime.MinValue) == 0 ||
                DateTime.Compare(fechaFinal, DateTime.MinValue) == 0)
            {
                throw new CAppException(8006, "El periodo de tiempo de los movimientos es obligatorio");
            }

            //Se verifica que se haya ingresado el tag
            if (String.IsNullOrEmpty(tag))
            {
                throw new CAppException(8006, "Por favor selecciona el Tag");
            }

            //Se validan fechas
            if (DateTime.Compare(fechaInicial, fechaFinal) > 0)
            {
                throw new CAppException(8006, "La fecha inicial debe ser menor o igual a la fecha final");
            }


            try
            {
                return DAOConsultaClientesTeleVIP.ObtieneMovimientosTag(fechaInicial, fechaFinal,
                            tag, cuenta, usuario);
            }

            catch (CAppException err)
            {
                Loguear.Error(err, usuario.ClaveUsuario);
                throw err;
            }

            catch (Exception err)
            {
                Loguear.Error(err, usuario.ClaveUsuario);
                throw err;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación de la consulta de movimientos del tag
        /// </summary>
        /// <param name="tag">Número de tag</param>
        /// <param name="cuenta">Cuenta televia</param>
        /// <param name="usuario">Usuario en sesión</param>
        /// <returns>Dataset con los registros obtenidos</returns>
        public static DataSet ConsultaRecompensas(string tag, string cuenta, IUsuario usuario)
        {
            //Se verifica que se haya ingresado el tag
            if (String.IsNullOrEmpty(tag))
            {
                throw new CAppException(8006, "Por favor selecciona el Tag");
            }


            try
            {
                return DAOConsultaClientesTeleVIP.ObtieneRecompensas(tag, cuenta, usuario);
            }

            catch (CAppException err)
            {
                Loguear.Error(err, usuario.ClaveUsuario);
                throw err;
            }

            catch (Exception err)
            {
                Loguear.Error(err, usuario.ClaveUsuario);
                throw err;
            }
        }
    }
}