using DALAutorizador.BaseDatos;
using DALAutorizador.Utilidades;
using DALCentroContacto.BaseDatos;
using DALCentroContacto.Entidades;
using Interfases;
using Interfases.Exceptiones;
using Log_PCI;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace DALCentroContacto.LogicaNegocio
{
    /// <summary>
    /// Establece la lógica de negocio para los Clientes
    /// </summary>
    public class LNClientes
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
            if (cliente.ID_Cadena == 0 && cliente.ID_Cliente == 0 &&
                String.IsNullOrEmpty(cliente.MedioAcceso) && String.IsNullOrEmpty(cliente.ApellidoPaterno) &&
                String.IsNullOrEmpty(cliente.ApellidoMaterno) && String.IsNullOrEmpty(cliente.Nombre) &&
                String.IsNullOrEmpty(cliente.Email) && DateTime.Compare(cliente.FechaNacimiento, DateTime.MinValue) == 0)

            {
                throw new CAppException(8006, "Selecciona al menos un criterio de búsqueda");
            }

            //Si se ingresó la cadena, se debe complementar la búsqueda con al menos un parámetro más
            if (cliente.ID_Cadena != 0)
            {
                if (cliente.ID_Cliente == 0 && DateTime.Compare(cliente.FechaNacimiento, DateTime.MinValue) == 0 &&
                String.IsNullOrEmpty(cliente.MedioAcceso) && String.IsNullOrEmpty(cliente.ApellidoPaterno) &&
                String.IsNullOrEmpty(cliente.ApellidoMaterno) && String.IsNullOrEmpty(cliente.Nombre) &&
                String.IsNullOrEmpty(cliente.Email))
                {
                    throw new CAppException(8006, "Selecciona un criterio de búsqueda adicional a la Cadena");
                }
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
                return DAOConsultaClientes.ObtieneClientes(cliente, usuario,
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
        /// Establece las condiciones de validación para la actualización de los datos del cliente
        /// </summary>
        /// <param name="elCliente">Datos de la entidad Cliente por actualizar</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void ActualizaDatosCliente(Cliente elCliente, IUsuario usuario)
        {
            try
            {
                //Se valida que se tenga ingresado por lo menos el nombre del cliente
                if (String.IsNullOrEmpty(elCliente.Nombre))
                {
                    throw new CAppException(8006, "Ingresa el nombre del cliente.");
                }

                using (SqlConnection conn = BDAutorizador.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DAOConsultaClientes.actualizaCliente(elCliente, usuario);
                            transaccionSQL.Commit();
                        }
                        catch (CAppException err)
                        {
                            transaccionSQL.Rollback();
                            throw err;
                        }
                        catch (Exception err)
                        {
                            transaccionSQL.Rollback();
                            throw new CAppException(8006, "Falla al Actualizar los datos del Cliente en Base de Datos ", err);
                        }
                    }
                }
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
        /// Establece las condiciones de validación de la consulta de movimientos de un medio de acceso
        /// </summary>
        /// <param name="fechaInicial">Fecha inicial del periodo de consulta</param>
        /// <param name="fechaFinal">Fecha final del periodo de consulta</param>
        /// <param name="idTipoCuenta">Identificador del tipo de cuenta</param>
        /// <param name="idTipoOper">Identificador del tipo de operación</param>
        /// <param name="medioAcceso">Clave del medio de acceso</param>
        /// <param name="usuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>Dataset con los registros obtenidos</returns>
        public static DataSet ConsultaMovimientos(DateTime fechaInicial, DateTime fechaFinal, int idTipoCuenta, int idTipoOper, 
            int idMedioAcceso, IUsuario usuario, Guid AppID)
        {
            //Se verifica que se hayan capturado las dos fechas
            if (DateTime.Compare(fechaInicial, DateTime.MinValue) == 0 ||
                DateTime.Compare(fechaFinal, DateTime.MinValue) == 0)
            {
                throw new CAppException(8006, "El periodo de tiempo de los movimientos es obligatorio");
            }

            //Se verifica que se haya ingresado el tipo de cuenta
            if (idTipoCuenta == 0)
            {
                throw new CAppException(8006, "Por favor ingresa el tipo de cuenta");
            }

            //Se validan fechas
            if (DateTime.Compare(fechaInicial, fechaFinal) > 0)
            {
                throw new CAppException(8006, "La fecha inicial debe ser menor o igual a la fecha final");
            }


            try
            {
                return DAOConsultaClientes.ObtieneMovimientosTX(fechaInicial, fechaFinal, idTipoCuenta,
                            idTipoOper, idMedioAcceso, usuario,
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
        /// Establece las condiciones de validación para la actualización del estatus de una tarjeta
        /// </summary>
        /// <param name="idMA">Identificador del medio de acceso</param>
        /// <param name="idEstatusMA">Identificador del estatus del medio de acceso</param>
        /// <param name="razones">Comentarios descritos como razones para la cancelación de la tarjeta</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void ActualizaEstatusMA(int idMA, int idEstatusMA, string razones, IUsuario usuario)
        {
            try
            {
                //Se valida que se hayan capturado las razones de la cancelación
                if (String.IsNullOrEmpty(razones))
                {
                    throw new CAppException(8006, "Ingresa la razón de la cancelación.");
                }

                using (SqlConnection conn = BDAutorizador.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            //Se modifica el estatus del medio de acceso
                            DAOConsultaClientes.ModificaEstatusMA(idMA, idEstatusMA, usuario);

                            //Se inserta el registro en la bitázora de detalle
                            DAOConsultaClientes.InsertaRazonBitacoraDetalle(idMA, razones, usuario);

                            transaccionSQL.Commit();
                        }
                        catch (CAppException err)
                        {
                            transaccionSQL.Rollback();
                            throw err;
                        }
                        catch (Exception err)
                        {
                            transaccionSQL.Rollback();
                            throw new CAppException(8006, "Falla al Actualizar el Estatus del MDA en Base de Datos ", err);
                        }
                    }
                }
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
        /// Establece las condiciones de validación para el registro de la llamada del cliente
        /// </summary>
        /// <param name="ID_Cliente">Identificador del cliente</param>
        /// <param name="ID_Actividad">Identificador de la actividad</param>
        /// <param name="Comentarios">Comentarios</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void RegistraLlamadaCliente(int ID_Cliente, int ID_Actividad, string Comentarios, IUsuario usuario)
        {
            try
            {
                //Se valida que se capturen todos los datos del formulario
                if (ID_Actividad == 0 || String.IsNullOrEmpty(Comentarios))
                {
                    throw new CAppException(8006, "Captura todos los datos para registrar la llamada.");
                }
                if (ID_Cliente == 0)
                {
                    throw new CAppException(8006, "Debes seleccionar un cliente para registrar su llamada.");
                }


                using (SqlConnection conn = BDAutorizador.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DAOConsultaClientes.InsertaLlamadaCliente(ID_Cliente, ID_Actividad, Comentarios, usuario);
                            transaccionSQL.Commit();
                        }
                        catch (CAppException err)
                        {
                            transaccionSQL.Rollback();
                            throw err;
                        }
                        catch (Exception err)
                        {
                            transaccionSQL.Rollback();
                            throw new CAppException(8006, "Falla al Actualizar los datos del Cliente en Base de Datos ", err);
                        }
                    }
                }
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
        /// Establece las condiciones de validación para el registro de la llamadas
        /// </summary>
        /// <param name="elRegistro">Objeto con los detalles de la llamada a registrar</param>
        /// <param name="usuario">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
            public static void RegistrarLlamada(RegistroLlamada elRegistro, IUsuario usuario, ILogHeader logHeader)
        {
            try
            {
                string usuarioLlamada = string.IsNullOrEmpty(elRegistro.UsuarioLlama) ? "" :
                    "UsuarioLlamada=" + elRegistro.UsuarioLlama + "|";

                elRegistro.ParametrosLlamada = usuarioLlamada + "ID_MA=" + elRegistro.ID_MedioAcceso.ToString() + 
                    "|ID_Colectiva=" + elRegistro.ID_Colectiva.ToString() + ";";

                DAOConsultaClientes.InsertaLlamada(elRegistro, usuario, logHeader);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(logHeader);
                pCI.ErrorException(ex);
                throw new CAppException(8011, "Falla al registrar la llamada");
            }
        }
    }
}