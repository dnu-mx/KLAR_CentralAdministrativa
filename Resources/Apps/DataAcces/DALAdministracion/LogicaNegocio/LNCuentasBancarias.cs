using DALAdministracion.BaseDatos;
using DALAdministracion.Entidades;
using DALCentralAplicaciones.Utilidades;
using Executer.Entidades;
using Interfases;
using Interfases.Entidades;
using Interfases.Exceptiones;
using Log_PCI.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace DALAdministracion.LogicaNegocio
{
    /// <summary>
    /// Establece la lógica de negocio para las Cuentas Bancarias
    /// </summary>
    public class LNCuentasBancarias
    {
        #region Variables privadas

        /// <summary>
        /// Expresión regular de validación para email
        /// </summary>
        private const string regexEmail = @"^([\w-]+\.)*?[\w-]+@[\w-]+\.([\w-]+\.)*?[\w]+${7,100}";

        #endregion

        /// <summary>
        /// Establece las condiciones de validación para la consulta de cuentas
        /// </summary>
        /// <param name="losDatos">Datos personales de la cuenta por consultar</param>
        /// <param name="usuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DatSet con los registros resultados de la búsqueda en BD</returns>
        public static DataSet ConsultaCuentas(DatosPersonalesCuenta losDatos, IUsuario usuario, Guid AppID)
        {
            //Se verifica que se haya capturado al menos un criterio de búsqueda
            if (losDatos.ID_Cuenta == 0 && String.IsNullOrEmpty(losDatos.TarjetaTitular) &&
                String.IsNullOrEmpty(losDatos.TarjetaAdicional) && String.IsNullOrEmpty(losDatos.ApellidoPaterno) &&
                String.IsNullOrEmpty(losDatos.ApellidoMaterno) && String.IsNullOrEmpty(losDatos.Nombre))
            {
                throw new CAppException(8006, "Selecciona al menos un criterio de búsqueda");
            }

            try
            {
                return DAOCuentasBancarias.ObtieneCuentas(losDatos, usuario, AppID);
            }

            catch (CAppException err)
            {
                DALAdministracion.Utilidades.Loguear.Error(err, usuario.ClaveUsuario);
                throw err;
            }

            catch (Exception err)
            {
                DALAdministracion.Utilidades.Loguear.Error(err, usuario.ClaveUsuario);
                throw err;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la actualización de los datos
        /// personales de la cuenta
        /// </summary>
        /// <param name="losDatos">Datos de la entidad DatosPersonalesCuentaTDC por actualizar</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void ActualizaDatosPersonalesCuenta(DatosPersonalesCuenta losDatos, IUsuario usuario)
        {
            try
            {
                //Se valida que se tenga ingresado por lo menos el nombre del cliente
                if (String.IsNullOrEmpty(losDatos.Nombre))
                {
                    throw new CAppException(8006, "Ingresa el nombre del titular de la cuenta.");
                }

                //Si se ingresó el correo electrónico, se verifica que sea una cadena válida
                if (!String.IsNullOrEmpty(losDatos.Email))
                {
                    Match matchExpression;
                    Regex matchEmail = new Regex(regexEmail);

                    matchExpression = matchEmail.Match(losDatos.Email);

                    if (!matchExpression.Success)
                    {
                        throw new CAppException(8006, "El Correo Electrónico que ingresaste no es una dirección válida.");
                    }
                }

                using (SqlConnection conn = BDAutorizadorMC.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DAOCuentasBancarias.ActualizaDatosPersonalesCuenta(losDatos, usuario);
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
                            throw new CAppException(8006, "Falla al Actualizar los Datos Personales de la Cuenta en Base de Datos ", err);
                        }
                    }
                }
            }
            catch (CAppException err)
            {
                DALAdministracion.Utilidades.Loguear.Error(err, usuario.ClaveUsuario);
                throw err;
            }
            catch (Exception err)
            {
                DALAdministracion.Utilidades.Loguear.Error(err, usuario.ClaveUsuario);
                throw err;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la actualización de los datos de la cuenta
        /// </summary>
        /// <param name="laCuentaAntes">Datos de la entidad CuentaTDC como se recibieron en la consulta</param>
        /// <param name="laCuentaDespues">Datos de la entidad CuentaTDC por actualizar</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static Dictionary<String, String> ValidaDatosCuenta(CuentaTDC laCuentaAntes, CuentaTDC laCuentaDespues, IUsuario usuario)
        {
            Dictionary<String, String> losCambios = new Dictionary<String, String>();

            //Se valida si cambió de los datos para evitar llamadas innecesarias a BD
            if (laCuentaAntes.FechaCorte != laCuentaDespues.FechaCorte)
            {
                losCambios.Add("Corte", "1");
            }

            if (laCuentaAntes.FechaLimitePago != laCuentaDespues.FechaLimitePago)
            {
                losCambios.Add("LimitePago", "1");
            }

            if (laCuentaAntes.LimiteCredito != laCuentaDespues.LimiteCredito)
            {
                losCambios.Add("LimiteCredito", "1");
            }

            if (laCuentaAntes.NombreEmbozado != laCuentaDespues.NombreEmbozado)
            {
                losCambios.Add("NombreEmbozo", "1");
            }

            if (laCuentaAntes.VigenciaTarjeta != laCuentaDespues.VigenciaTarjeta)
            {
                losCambios.Add("VigenciaTarjeta", "1");
            }

            return losCambios;
        }

        /// <summary>
        /// Establece las condiciones de validación para la actualización de parámetros de la cuenta
        /// </summary>
        /// <param name="IdCuenta">Identificador de la cuenta</param>
        /// <param name="IdProducto">Identificador del producto</param>
        /// <param name="NombreParametro">Nombre del parámetro por actualizar</param>
        /// <param name="Valor">Nuevo valor del parámetro por actualizar</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void ActualizaParametrosCuenta(Int64 IdCuenta, Int64 IdProducto, String NombreParametro,
            String Valor, IUsuario usuario)
        {
            try
            {
                using (SqlConnection conn = BDAutorizadorMC.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DAOCuentasBancarias.ActualizaParametroProductoCuenta(IdCuenta, IdProducto, NombreParametro, Valor, usuario);
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
                            throw new CAppException(8006, "Falla al Actualizar los Parámetros Cuenta del Producto en Base de Datos ", err);
                        }
                    }
                }
            }
            catch (CAppException err)
            {
                DALAdministracion.Utilidades.Loguear.Error(err, usuario.ClaveUsuario);
                throw err;
            }
            catch (Exception err)
            {
                DALAdministracion.Utilidades.Loguear.Error(err, usuario.ClaveUsuario);
                throw err;
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la actualización de parámetros del producto
        /// </summary>
        /// <param name="IdProducto">Identificador del producto por actualizar</param>
        /// <param name="NombreParametro">Nombre del parámetro por actualizar</param>
        /// <param name="Valor">Nuevo valor del parámetro por actualizar</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void ActualizaParametrosProducto(Int64 IdProducto, String NombreParametro, String Valor, IUsuario usuario)
        {
            try
            {
                using (SqlConnection conn = BDAutorizadorMC.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DAOCuentasBancarias.ActualizaParametroProducto(IdProducto, NombreParametro, Valor, usuario);
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
                            throw new CAppException(8006, "Falla al Actualizar los Parámetros del Producto en Base de Datos ", err);
                        }
                    }
                }
            }
            catch (CAppException err)
            {
                DALAdministracion.Utilidades.Loguear.Error(err, usuario.ClaveUsuario);
                throw err;
            }
            catch (Exception err)
            {
                DALAdministracion.Utilidades.Loguear.Error(err, usuario.ClaveUsuario);
                throw err;
            }
        }

        /// <summary>
        /// Controla las validaciones y solicita el evento manual de Ajuste de Límte de Crédito
        /// </summary>
        /// <param name="IdColectiva">Identificador de la colectiva seleccionada</param>
        /// <param name="LDCActual">Límite de crédito actual</param>
        /// <param name="LDCNuevo">Límite de crédito solicitado</param>
        /// <param name="usuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        public static void RegistraEvManual_AjustaLimiteCredito(int IdColectiva, string medioAcceso, string LDCActual, string LDCNuevo, 
            IUsuario usuario, Guid AppID)
        {
            using (SqlConnection conn = BDAutorizadorMC.BDEscritura)
            {
                conn.Open();

                using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                {

                    try
                    {
                        Poliza laPoliza = null;

                        Dictionary<String, Parametro> TodosLosParametros = new Dictionary<string, Parametro>();

                        //Se consulta la clave de la colectiva y la clave del evento
                        DataSet dsDatosEvento = DAOCuentasBancarias.ConsultaEventoAjusteManualLDC(IdColectiva, usuario, AppID);

                        string idEmisor = dsDatosEvento.Tables[0].Rows[0]["IdEmisor"].ToString();
                        string idCadena = dsDatosEvento.Tables[0].Rows[0]["IdCadena"].ToString();
                        string claveCadena = dsDatosEvento.Tables[0].Rows[0]["ClaveCadena"].ToString();
                        string claveEvento = dsDatosEvento.Tables[0].Rows[0]["ClaveEvento"].ToString();

                        //se consultan los parámetros del contrato
                        TodosLosParametros = Executer.BaseDatos.DAOEvento.ListaDeParamentrosContrato
                            (claveCadena, medioAcceso, claveEvento, usuario.ClaveUsuario);

                        //Se sustituyen los valores de los parámetros obtenidos del contrato por los requeridos para
                        //el evento manual, y se añaden los que se necesitan adicionalmente
                        TodosLosParametros["@ID_Emisor"].Valor = idEmisor;
                        TodosLosParametros["@ID_CadenaComercial"].Valor = idCadena;                        
                        TodosLosParametros["@SaldoActual"] = new Parametro() { Nombre = "@SaldoActual ", Valor = LDCActual, Descripcion = "SaldoActual" };
                        TodosLosParametros["@Importe"] = new Parametro() { Nombre = "@Importe", Valor = LDCNuevo, Descripcion = "Importe" };

                        //Se consulta la cadena default para las observaciones del evento
                        string observaciones = Configuracion.Get(AppID, "ObsAjusteManualLDC").Valor;

                        LogHeader logTEMP = new LogHeader();

                        //Se genera y aplica la póliza
                        Executer.EventoManual aplicador = new Executer.EventoManual(
                            Convert.ToInt32(TodosLosParametros["@ID_Evento"].Valor), TodosLosParametros["@DescEvento"].Valor,
                            false, 0, TodosLosParametros, observaciones, conn, transaccionSQL, logTEMP);
                        laPoliza = aplicador.AplicaContablilidad(logTEMP);

                        if (laPoliza.CodigoRespuesta != 0)
                        {
                            transaccionSQL.Rollback();
                            throw new Exception("No se generó la Póliza: " + laPoliza.DescripcionRespuesta);
                        }
                        else
                        {
                            transaccionSQL.Commit();
                        }
                    }

                    catch (Exception err)
                    {
                        transaccionSQL.Rollback();
                        DALAdministracion.Utilidades.Loguear.Error(err, "");
                        throw err;
                    }
                }
            }
        }
    }
}