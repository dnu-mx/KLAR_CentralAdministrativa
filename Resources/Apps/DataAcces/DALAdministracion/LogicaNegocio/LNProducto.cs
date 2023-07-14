using DALAdministracion.BaseDatos;
using DALAdministracion.Entidades;
using DALAdministracion.Utilidades;
using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALCentralAplicaciones.Entidades;
using DALCortador.Entidades;
using DALEventos.LogicaNegocio;
using Executer.Entidades;
using Interfases;
using Interfases.Entidades;
using Interfases.Exceptiones;
using Log_PCI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Producto = DALAutorizador.Entidades.Producto;

namespace DALAdministracion.LogicaNegocio
{
    /// <summary>
    /// Establece la lógica del negocio para el Producto
    /// </summary>
    public class LNProducto
    {
        /// <summary>
        /// Establece las condiciones de validación para crear un nuevo producto en el Autorizador,
        /// controlando la transacción (commit o rollback)
        /// </summary>
        /// <param name="elProducto">Datos del nuevo producto</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con las respuestas del SP</returns>
        public static DataTable CreaNuevoProductoEnAutorizador(Producto elProducto, Usuario elUsuario, ILogHeader logHeader)
        {
            try
            {
                return DAOProducto.InsertaProducto(elProducto, elUsuario, logHeader);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(logHeader);
                pCI.ErrorException(ex);
                throw new CAppException(8011, "Falla al crear el producto");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la modificación de los valores de los parámetros
        /// de contrtato CLABE, para la colectiva indicada
        /// </summary>
        /// <param name="idColectiva">Identificador de la colectiva dueña del contrato</param>
        /// <param name="elParametro">Datos del parámetro por actualizar</param>
        /// <param name="elUser">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void ModificaValoresParametrosCLABE(Int64 IdColectiva, List<ParametroValor> losParametrosValor,
            Usuario elUser, ILogHeader logHeader)
        {
            try
            {
                foreach (ParametroValor param in losParametrosValor)
                {
                    DAOProducto.InsertaOActualizaValorParametroContrato(IdColectiva, param, elUser, logHeader);
                }
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(logHeader);
                pCI.ErrorException(ex);
                throw new CAppException(8011, "Falla al actualizar los valores de los parámetros CLABE");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para modificar los datos de un producto en el Autorizador
        /// </summary>
        /// <param name="elProducto">Entidad con los datos por modificar</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Mensaje del Autorizador, con el resultado de la actualización</returns>
        public static string ModificaProducto(Producto elProducto, Usuario elUsuario, ILogHeader logHeader)
        {
            try
            {
                return DAOProducto.ActualizaProducto(elProducto, elUsuario, logHeader);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(logHeader);
                pCI.ErrorException(ex);
                throw new CAppException(8011, "Falla al modificar los datos del producto");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para crear un nuevo BIN del producto en el Autorizador
        /// </summary>
        /// <param name="IdProducto">Identificador del producto</param>
        /// <param name="ClaveBIN">Clave del nuevo BIN</param>
        /// <param name="DescripcionBIN">Descripción del nuevo BIN</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con las respuestas del SP</returns>
        public static DataTable CreaNuevoBinDelProducto(int IdProducto, String ClaveBIN, String DescripcionBIN, 
            Usuario elUsuario, ILogHeader logHeader)
        {
            try
            {
                return DAOProducto.InsertaBIN(IdProducto, ClaveBIN, DescripcionBIN, elUsuario, logHeader);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(logHeader);
                pCI.ErrorException(ex);
                throw new CAppException(8011, "Falla al crear el BIN del producto");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para modificar los datos de un BIN en el Autorizador
        /// </summary>
        /// <param name="IdBIN">Identificador del BIN a modificar</param>
        /// <param name="ClaveBIN">Nueva clave del BIN</param>
        /// <param name="DescripcionBIN">Nueva descripción del BIN</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>Mensaje del Autorizador, con el resultado de la actualización</returns>
        public static string ModificaBIN(int IdBIN, String ClaveBIN, String DescripcionBIN, Usuario elUsuario,
            ILogHeader logHeader)
        {
            try
            {
                return DAOProducto.ActualizaBINProducto(IdBIN, ClaveBIN, DescripcionBIN, elUsuario, logHeader);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(logHeader);
                pCI.ErrorException(ex);
                throw new CAppException(8011, "Falla al modificar el BIN del producto");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la modificación del valor de un parámetro
        /// multiasignación
        /// </summary>
        /// <param name="unParametro">Objeto del parámetro por modificar</param>
        /// <param name="elUser">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void ModificaValorParametro(ParametroValor unParametro, Usuario elUser, ILogHeader logHeader, string pantalla = "")
        {
            try
            {
                DAOParametroMA.ActualizaValorParametro(unParametro, elUser, logHeader, pantalla);
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(logHeader);
                pCI.ErrorException(ex);
                throw new CAppException(8011, "Falla al actualizar el valor del parámetro");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para insertar un parámetro multiasignación a 
        /// un subproducto en el Autorizador, controlando la transacción (commit o rollback)
        /// </summary>
        /// <param name="IdParametroMA">Idetnificador del parametro</param>
        /// <param name="IdSubproducto">Identificador del subproducto</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void AgregaParametroMAASubproducto(int IdParametroMA, int IdSubproducto, Usuario elUsuario, ILogHeader logHeader, string pantalla = "")
        {
            try
            {
                DAOProducto.InsertaPMASubproducto(IdParametroMA, IdSubproducto, elUsuario, logHeader, pantalla);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(logHeader);
                pCI.ErrorException(ex);
                throw new CAppException(8011, "Falla al agregar el valor del parámetro al subproducto");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para eliminar el valor de parámetro multiasignación
        /// en el Autorizador
        /// </summary>
        /// <param name="IdValorParametroMA">Identificador del valor de parámetro multiasignación</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void BorraValorParametro(int IdValorParametroMA, Usuario elUsuario, ILogHeader logHeader, string pantalla = "")
        {
            try
            {
                DAOParametroMA.EliminaValorParametro(IdValorParametroMA, elUsuario, logHeader, pantalla);
            }

            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(logHeader);
                pCI.ErrorException(ex);
                throw new CAppException(8011, "Falla al borrar el valor del parámetro");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para crear un nuevo subproducto del producto
        /// en el Autorizador, controlando la transacción (commit o rollback)
        /// </summary>
        /// <param name="elSubproducto">Objeto con los datos del nuevo subproducto</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con las respuestas del SP</returns>
        public static DataTable CreaNuevoSubproductoDelProducto(Plantilla elSubproducto, Usuario elUsuario, ILogHeader logHeader)
        {
            try
            {
               return DAOProducto.InsertaSubproducto(elSubproducto, elUsuario, logHeader);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(logHeader);
                pCI.ErrorException(ex);
                throw new CAppException(8011, "Falla al crear el subproducto");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para modificar los datos de un subproducto en el Autorizador,
        /// controlando la transacción (commit o rollback)
        /// </summary>
        /// <param name="elSubproducto">Entidad con los datos por modificar</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Mensaje del Autorizador, con el resultado de la actualización</returns>
        public static string ModificaSubproducto(Plantilla elSubproducto, Usuario elUsuario, ILogHeader logHeader, string pantalla = "")
        {
            try
            {
                return DAOProducto.ActualizaSubproducto(elSubproducto, elUsuario, logHeader, pantalla);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(logHeader);
                pCI.ErrorException(ex);
                throw new CAppException(8011, "Falla al modificar el subproducto");
            }
        }

        /// <summary>
        /// Controla las validaciones y solicita la ejecución del evento manual con el identificador
        /// proporcionado (eventos relacionados con Cuentas)
        /// </summary>
        /// <param name="elEvento">Entidad con los datos del evento</param>
        /// <param name="usuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void RegistraPertenenciaManual(EventoManual elEvento, long IdPertenencia, Usuario usuario,
            Guid AppID, ILogHeader logHeader)
        {
            LogPCI unLog = new LogPCI(logHeader);
            Poliza laPoliza = null;

            try
            {
                unLog.Info("INICIA ListaEventosDePertenenciaManual");
                List<string> eventos =
                    DAOProducto.ListaEventosDePertenenciaManual(IdPertenencia, usuario, AppID, logHeader);
                unLog.Info("TERMINA ListaEventosDePertenenciaManual");

                using (SqlConnection conn = BDAutorizador.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            foreach (string evento in eventos)
                            {
                                Dictionary<String, Parametro> TodosLosParametros = new Dictionary<string, Parametro>();

                                //se consultan los parámetros del contrato
                                unLog.Info("INICIA Executer_ListaParametrosDeContrato()");
                                TodosLosParametros = Executer.BaseDatos.DAOEvento.ListaParametrosDeContrato
                                    ("CCM", elEvento.MedioAcceso, evento, logHeader);
                                unLog.Info("TERMINA Executer_ListaParametrosDeContrato()");

                                //Se sustituyen los valores de los parámetros obtenidos del contrato por los requeridos para
                                //el evento manual, y se añaden los que se necesitan adicionalmente
                                TodosLosParametros["@FechaAplicacion"] = new Parametro() { Nombre = "@FechaAplicacion", Valor = elEvento.FechaAplicacion, Descripcion = "Fecha Aplicacion" };
                                TodosLosParametros["@Importe"] = new Parametro() { Nombre = "@Importe", Valor = elEvento.Importe, Descripcion = "Importe" };
                                TodosLosParametros["@IVA"] = new Parametro() { Nombre = "@IVA", Valor = elEvento.IVA, Descripcion = "IVA" };
                                TodosLosParametros["@ID_CadenaComercial"].Valor = elEvento.IdCadenaComercial.ToString();
                                TodosLosParametros["@ID_CuentaHabiente"].Valor = elEvento.IdColectivaOrigen.ToString();

                                laPoliza = null;

                                //Se genera y aplica la póliza
                                unLog.Info("INICIA new Executer.EventoManual()");
                                Executer.EventoManual aplicador = new Executer.EventoManual(
                                    Convert.ToInt32(TodosLosParametros["@ID_Evento"].Valor),
                                    TodosLosParametros["@DescEvento"].Valor, false, 0, TodosLosParametros,
                                    elEvento.Observaciones, conn, transaccionSQL, logHeader);
                                unLog.Info("TERMINA new Executer.EventoManual()");

                                unLog.Info("INICIA Executer.AplicaContablilidad()");
                                laPoliza = aplicador.AplicaContablilidad(logHeader);
                                unLog.Info("TERMINA Executer.AplicaContablilidad()");

                                if (laPoliza.CodigoRespuesta != 0)
                                {
                                    string msg = "Error al generar la póliza del evento manual. CodigoRespuesta: " +
                                    laPoliza.CodigoRespuesta + "; DescripcionRespuesta: " + laPoliza.DescripcionRespuesta;
                                    unLog.Warn(msg);
                                    throw new CAppException(8011, msg);
                                }

                                LNEvento.InsertaRegistroBitacoraDetalle("pantalla_AdminCuentas_AplicaEventoManual", "Polizas", "ID_Poliza", 
                                    laPoliza.ID_Poliza.ToString(), elEvento.Importe, "Aplicación Evento Manual. Tarjeta: '" + elEvento.MedioAcceso + "'", 
                                    conn, transaccionSQL, usuario, logHeader);
                            }

                            transaccionSQL.Commit();
                        }
                        catch (CAppException caEx)
                        {
                            transaccionSQL.Rollback();
                            throw caEx;
                        }
                        catch (Exception err)
                        {
                            transaccionSQL.Rollback();
                            Loguear.Error(err, "");
                            throw err;
                        }
                    }
                }
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8011, "Falla al registrar el movimiento manual");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para crear una nueva plantilla de tipo preautorizador
        /// al producto en el Autorizador
        /// </summary>
        /// <param name="laPlantilla">Objeto con los datos de la nueva plantilla</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>DataTable con las respuestas del SP</returns>
        public static DataTable CreaPlantillaPreAutDelProducto(Plantilla laPlantilla, ILogHeader logHeader)
        {
            try
            {
                return DAOProducto.InsertaPlantillaPreautNivel2(laPlantilla, logHeader);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }

            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(logHeader);
                pCI.ErrorException(ex);
                throw new CAppException(8011, "Falla al crear la plantilla");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para modificar la plantilla de tipo preautorizador, en las
        /// cuentas asociadas a la tarjeta, dentro del Autorizador
        /// </summary>
        /// <param name="IdTarjeta">Identificador de la tarjeta</param>
        /// <param name="IdPlantilla">Identificador de la plantilla a la que se actualizan las cuentas</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void ModificaPlantillaDeTarjeta(int IdTarjeta, Int64 IdPlantilla, Usuario elUsuario,
            ILogHeader logHeader)
        {
            try
            {
                DAOProducto.ActualizaPlantillaPreautTarjeta(IdTarjeta, IdPlantilla, elUsuario, logHeader);

            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(logHeader);
                pCI.ErrorException(ex);
                throw new CAppException(8011, "Falla al modificar la plantilla de las cuentas");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtFileToImport"></param>
        /// <param name="usuario"></param>
        public static void InsertaArchivoTarjetas(DataTable dtFileToImport, Usuario usuario)
        {
            SqlConnection conn = null;

            try
            {
                using (conn = BDAutorizador.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            //DAOEcommercePrana.InsertaPromocionesTMP(dtFileToImport, conn, transaccionSQL, usuario);
                            transaccionSQL.Commit();
                        }

                        catch (CAppException caEx)
                        {
                            transaccionSQL.Rollback();
                            throw caEx;
                        }

                        catch (Exception ex)
                        {
                            transaccionSQL.Rollback();
                            throw new CAppException(8006, "Falla al Cargar el Archivo en Base de Datos ", ex);
                        }
                    }
                }
            }

            catch (CAppException caEx)
            {
                Loguear.Error(caEx, usuario.ClaveUsuario);
                throw caEx;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, usuario.ClaveUsuario);
                throw ex;
            }

            finally
            {
                if (null != conn && ConnectionState.Open == conn.State)
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para modificar la plantilla de tipo preautorizador, en las
        /// cuentas asociadas a las tarjetas, dentro del Autorizador
        /// </summary>
        /// <param name="IdPlantilla">Identificador de la plantilla a la que se actualizan las cuentas</param>
        /// <param name="dtTarjetas">Tabla con los números de tarjet</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void ModificaPlantillaTarjetas(Int64 IdPlantilla, DataTable dtTarjetas, Usuario elUsuario,
            ILogHeader logHeader)
        {
            try
            {
                DAOProducto.ActualizaPlantillaPreautTarjetas(IdPlantilla, dtTarjetas, elUsuario, logHeader);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(logHeader);
                pCI.ErrorException(ex);
                throw new CAppException(8011, "Falla al modificar la plantilla de las tarjetas");
            }
        }
        /// <summary>
        /// Establece las condiciones de validación para crear o modificar una campaña en el Autorizador
        /// </summary>
        /// <param name="IdProducto">Identificador del producto</param>
        /// <param name="laCampanya">Entidad con los datos de la campaña por crear o modificar</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Mensaje del Autorizador, con el resultado de la creación o actualización</returns>
        public static void CreaOModificaCampanya(int IdProducto, CampanyaMSI laCampanya, Usuario elUsuario, ILogHeader logHeader)
        {
            LogPCI pCI = new LogPCI(logHeader);

            try
            {
                string resp = DAOProducto.InsertaOActualizaCampanya(IdProducto, laCampanya, elUsuario, logHeader);

                if (!resp.ToUpper().Contains("OK"))
                {
                    pCI.Warn(resp);
                    throw new CAppException(8011, resp);
                }
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                throw new CAppException(8011, "Falla al crear o modificar la campaña");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para modificar el estatus de una campaña de producto en el Autorizador
        /// </summary>
        /// <param name="ID_Campanya">Identificador de la campaña por actualizar</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void ModificaEstatusCampanya(int ID_Campanya, Usuario elUsuario, ILogHeader logHeader)
        {
            try
            {
                DAOProducto.ActualizaEstatusCampanya(ID_Campanya, elUsuario, logHeader);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(logHeader);
                pCI.ErrorException(ex);
                throw new CAppException(8011, "Falla al modificar el estatus de la campaña");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para crear o modificar la configuración de una campaña en el Autorizador
        /// </summary>
        /// <param name="IdCampanya">Identificador de la campaña</param>
        /// <param name="laPromocion">Entidad con los datos a modificar</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Mensaje del Autorizador, con el resultado de la creación o actualización</returns>
        public static void CreaOModificaConfiguracionDeCampanya(int IdCampanya, PromocionMSI laPromocion, Usuario elUsuario, ILogHeader logHeader)
        {
            LogPCI pCI = new LogPCI(logHeader);

            try
            {
                string resp = DAOProducto.InsertaOActualizaCampaniaConfiguracion(IdCampanya, laPromocion, elUsuario, logHeader);

                if (!resp.ToUpper().Contains("OK"))
                {
                    pCI.Warn(resp);
                    throw new CAppException(8011, resp);
                }
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                throw new CAppException(8011, "Falla al crear o modificar la configuración de la campaña");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para desactivar la configuración de una campaña en el Autorizador
        /// </summary>
        /// <param name="IdCampanyaCfg">Identificador de la configuración de campaña por eliminar</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void DesactivaConfiguracionDeCampanya(int IdCampanyaCfg, Usuario elUsuario, ILogHeader logHeader)
        {
            try
            {
                DAOProducto.ActualizaEstatusCampaniaConfiguracion(IdCampanyaCfg, elUsuario, logHeader);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                LogPCI pCI = new LogPCI(logHeader);
                pCI.ErrorException(ex);
                throw new CAppException(8011, "Falla al eliminar la configuración de la campaña");
            }
        }
    }
}