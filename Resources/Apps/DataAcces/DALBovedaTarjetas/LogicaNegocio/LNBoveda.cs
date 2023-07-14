using DALBovedaTarjetas.BaseDatos;
using DALBovedaTarjetas.Entidades;
using DALCentralAplicaciones.Entidades;
using DALCentralAplicaciones.Utilidades;
using DNU.Cifrado.DES;
using Interfases;
using Interfases.Exceptiones;
using Log_PCI;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using WebServices;
using WebServices.Entidades;
using WebServices.Utilerias;

namespace DALBovedaTarjetas.LogicaNegocio
{
    /// <summary>
    /// Establece la lógica del negocio para la aplicación Bóveda
    /// </summary>
    public class LNBoveda
    {
        //#region Variables privadas

        //private static DataTable dtContenidoFile;
        //private static DataTable dtContenidoFileProducto;

        //#endregion

        #region Constantes

        private const string expRegFecha2 = @"^\d{4}((0[1-9])|(1[012]))((0[1-9]|[12]\d)|3[01])$";

        #endregion

        /// <summary>
        /// Establece las condiciones de validación para realizar la conexión (Login) al API de información
        /// en Bóveda Digital
        /// </summary>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="token">Referencia a la cadena donde se regresa el token de conexión</param>
        /// <param name="credenciales">Referencia a la cadena donde se regresan las credenciales de conexión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void ConectaAPI(Guid AppID, ref string token, ref string credenciales, ILogHeader logHeader)
        {
            LogPCI log = new LogPCI(logHeader);

            try
            {
                string wsLoginResp = null;
                Parametros.Headers_BD _headers = new Parametros.Headers_BD();
                Parametros.LoginBody_BD _body = new Parametros.LoginBody_BD();

                _headers.URL = Configuracion.Get(AppID, "APIBovDig_URL").Valor;
                _body.user_name = Configuracion.Get(AppID, "APIBovDig_Usr").Valor;
                _body.password = Configuracion.Get(AppID, "APIBovDig_Pwd").Valor;

                log.Info("INICIA BovedaDigital.Login()");
                wsLoginResp = BovedaDigital.Login(_headers, _body, logHeader);
                log.Info("TERMINA BovedaDigital.Login()");

                if (wsLoginResp.ToUpper().Contains("ERROR"))
                {
                    throw new CAppException(8006, wsLoginResp);
                }

                _headers.Token = wsLoginResp;
                _headers.Credentials = Cifrado.Base64Encode(_body.user_name + ":" + _body.password);

                token = wsLoginResp;
                credenciales = _headers.Credentials;
            }
            catch (CAppException err)
            {
                log.Error(err.Mensaje());
                throw err;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8011, "Falla en la conexión al API.");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la solicitud de productos al API de información
        /// en Bóveda Digital
        /// </summary>
        /// <param name="parametros">Parámetros API requeridos para el método</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Lista de productos del emisor</returns>
        public static List<ProductoBovedaDigital> SolicitaProductosAPI(ParametrosAPI parametros, Guid AppID,
            ILogHeader logHeader)
        {
            LogPCI logPCI = new LogPCI(logHeader);

            try
            {
                Parametros.Headers_BD _headers = new Parametros.Headers_BD();
                Parametros.Productos_BD _bodyProductos = new Parametros.Productos_BD();

                _headers.URL = Configuracion.Get(AppID, "APIBovDig_URL").Valor;
                _headers.Token = parametros.Token;
                _headers.Credentials = parametros.Credenciales;

                _bodyProductos.issuer_key = parametros.ClaveEmisor;
                _bodyProductos.sub_bins_group_key = parametros.ClaveProducto;

                logPCI.Info("INICIA BovedaDigital.Productos()");
                List<ProductoBovedaDigital> Productos = BovedaDigital.Productos(_headers, _bodyProductos, logHeader);
                logPCI.Info("TERMINA BovedaDigital.Productos()");

                if (Productos.Count == 0)
                {
                    throw new CAppException(8011, "El servicio web no devolvió Productos para el Emisor.");
                }
                
                return Productos;
            }
            catch (CAppException err)
            {
                logPCI.Error(err.Mensaje());
                throw err;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8011, "Falla en la solicitud de productos al API.");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la solicitud de tipos de manufactura al API de información
        /// en Bóveda Digital
        /// </summary>
        /// <param name="parametros">Parámetros API requeridos para el método</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Lista de tipos de manufactura del emisor</returns>
        public static List<TipoManufacturaBovedaDigital> SolicitaTiposTarjetaAPI(ParametrosAPI parametros, Guid AppID,
            ILogHeader logHeader)
        {
            LogPCI logPCI = new LogPCI(logHeader);

            try
            {
                Parametros.Headers_BD _headers = new Parametros.Headers_BD();

                _headers.URL = Configuracion.Get(AppID, "APIBovDig_URL").Valor;
                _headers.Token = parametros.Token;
                _headers.Credentials = parametros.Credenciales;

                logPCI.Info("INICIA BovedaDigital.TiposManufactura()");
                List<TipoManufacturaBovedaDigital> Tipos = BovedaDigital.TiposManufactura(_headers, logHeader);
                logPCI.Info("TERMINA BovedaDigital.TiposManufactura()");

                if (Tipos.Count == 0)
                {
                    throw new CAppException(8011, "El servicio web no devolvió Tipos de Manufactura.");
                }

                return Tipos;
            }
            catch (CAppException err)
            {
                logPCI.Error(err.Mensaje());
                throw err;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8011, "Falla en la solicitud de tipos de tarjeta al API.");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la generación de lotes en el API de información
        /// en Bóveda Digital
        /// </summary>
        /// <param name="parametros">Parámetros API requeridos para el método</param>
        /// <param name="usuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>NúmerMensaje de respuesta del API</returns>
        public static RespuestasJSON.LotValues[] GeneraLoteAPI(ParametrosAPI parametros, ref string NumeroLote,
            Usuario usuario, Guid AppID, ILogHeader logHeader)
        {
            LogPCI unLog = new LogPCI(logHeader);

            try
            {
                string wsLotsResp = null;
                Parametros.Headers_BD _headers = new Parametros.Headers_BD();
                Parametros.Lots_BD _body = new Parametros.Lots_BD();

                _headers.URL = Configuracion.Get(AppID, "APIBovDig_URL").Valor;
                _headers.Token = parametros.Token;
                _headers.Credentials = parametros.Credenciales;

                _body.lot_key = parametros.ClaveLote == "-1" ? GeneraNumeroDeLote(logHeader) :
                    parametros.ClaveLote;
                _body.sub_bins_group_key = parametros.ClaveProducto;
                _body.lot_type_key = parametros.ClaveTipoLote;
                _body.manufacturing_type_key = parametros.ClaveTipoManufactura;
                _body.items = parametros.Cantidad;
                _body.user = usuario.ClaveUsuario;

                unLog.Info("INICIA BovedaDigital.CrearLote()");
                RespuestasJSON.LotValues[] elLote = BovedaDigital.CrearLote(_headers, _body, ref wsLotsResp, logHeader);
                unLog.Info("TERMINA BovedaDigital.CrearLote()");

                if (wsLotsResp.ToUpper().Contains("ERROR"))
                {
                    throw new CAppException(8006, wsLotsResp);
                }

                NumeroLote = wsLotsResp;
                return elLote;
            }
            catch (CAppException err)
            {
                unLog.Error(err.Mensaje());
                throw err;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8011, "Falla en la generación del lote en el API.");
            }
        }

        /// <summary>
        /// Genera el número de lote que corresponde a la fecha y hora actual en el sistema
        /// </summary>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Cadena con el número de lote</returns>
        public static string GeneraNumeroDeLote(ILogHeader logHeader)
        {
            LogPCI log = new LogPCI(logHeader);
            log.Info("GeneraNumeroDeLote()");

            try
            {
                StringBuilder sb = new StringBuilder();
                DateTime laFecha = DateTime.Now;

                // lot_Key - cadena ‘AADDhhmmss’
                sb.Append(laFecha.Year.ToString().Substring(2, 2));
                sb.Append(DecimalToArbitrarySystem(laFecha.DayOfYear, 32));
                sb.Append(laFecha.Hour.ToString().PadLeft(2, '0'));
                sb.Append(laFecha.Minute.ToString().PadLeft(2, '0'));
                sb.Append(laFecha.Second.ToString().PadLeft(2, '0'));

                return sb.ToString();
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8011, "Ocurrió un error al generar el número de lote.");
            }
        }

        /// <summary>
        /// Converts the given decimal number to the numeral system with the
        /// specified radix (in the range [2, 36]).
        /// </summary>
        /// <param name="decimalNumber">The number to convert.</param>
        /// <param name="radix">The radix of the destination numeral system (in the range [2, 36]).</param>
        /// <returns></returns>
        public static string DecimalToArbitrarySystem(long decimalNumber, int radix)
        {
            const int BitsInLong = 64;
            const string Digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            if (radix < 2 || radix > Digits.Length)
                throw new ArgumentException("The radix must be >= 2 and <= " + Digits.Length.ToString());

            if (decimalNumber == 0)
                return "0";

            int index = BitsInLong - 1;
            long currentNumber = Math.Abs(decimalNumber);
            char[] charArray = new char[BitsInLong];

            while (currentNumber != 0)
            {
                int remainder = (int)(currentNumber % radix);
                charArray[index--] = Digits[remainder];
                currentNumber = currentNumber / radix;
            }

            string result = new String(charArray, index + 1, BitsInLong - index - 1);
            if (decimalNumber < 0)
            {
                result = "-" + result;
            }

            return result;
        }

        /// <summary>
        /// Establece las condiciones de negocio para realizar la solicitud de las tarjetas stock (prevalidaciones,
        /// solicitud a bóveda, generación de archivo para procesar y acttualización de petición a base de datos)
        /// </summary>
        /// <param name="laPeticion">Datos de la entidad PeticionAltaTarjetas</param>
        /// <param name="losParametros">Parámetros de petición a bóveda</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void SolicitarTarjetasStock(PeticionAltaTarjetas laPeticion, ParametrosAPI losParametros,
            Usuario elUsuario, Guid AppID, ILogHeader logHeader)
        {
            LogPCI logPCI = new LogPCI(logHeader);

            try
            {
                logPCI.Info("INICIA PreValidacionesArchivosStock()");
                PreValidacionesArchivosStock(ref laPeticion, elUsuario, AppID, logHeader);
                logPCI.Info("TERMINA PreValidacionesArchivosStock()");

                logPCI.Info("INICIA SolicitaTarjetasAPI()");
                RespuestasJSON.LotValues[] elLote = SolicitaTarjetasAPI(laPeticion, losParametros,
                    elUsuario, AppID, logHeader);
                logPCI.Info("TERMINA SolicitaTarjetasAPI()");

                logPCI.Info("INICIA GeneraArchivoStock()");
                GeneraArchivoStock(losParametros.ClaveProducto, laPeticion, elLote, logHeader);
                logPCI.Info("TERMINA GeneraArchivoStock()");

                logPCI.Info("INICIA DAOBoveda.ActualizaPeticionTarjetas()");
                DAOBoveda.ActualizaPeticionTarjetas(laPeticion, elUsuario, logHeader);
                logPCI.Info("TERMINA DAOBoveda.ActualizaPeticionTarjetas()");
            }
            catch (CAppException err)
            {
                logPCI.Error(err.Mensaje());
                throw err;
            }
            catch (Exception ex)
            {
                logPCI.ErrorException(ex);
                throw new CAppException(8011, "Falla en la validación del contenido del archivo.");
            }
        }

        /// <summary>
        /// Realiza las prevalidaciones de condiciones a cumplir, antes de solicitar el lote de tarjetas
        /// </summary>
        /// <param name="peticionAlta">Objeto con los datos de la petición</param>
        /// <param name="usuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void PreValidacionesArchivosStock(ref PeticionAltaTarjetas peticionAlta, Usuario usuario,
            Guid AppID, ILogHeader logHeader)
        {
            LogPCI log = new LogPCI(logHeader);

            try
            {
                log.Info("INICIA ObtieneRutaEntradaTarjetasStock()");
                peticionAlta.RutaEntradaArchivos = DAOBoveda.ObtieneRutaEntradaTarjetasStock(logHeader);
                log.Info("TERMINA ObtieneRutaEntradaTarjetasStock()");

                //////////TEMP
                //peticionAlta.RutaEntradaArchivos = "C:\\TEMP\\Stock\\";

                if (string.IsNullOrEmpty(peticionAlta.RutaEntradaArchivos))
                {
                    throw new CAppException(8011, "La ruta para almacenar los archivos no está configurada.");
                }

                if (!peticionAlta.RutaEntradaArchivos.EndsWith("\\"))
                {
                    peticionAlta.RutaEntradaArchivos += "\\";
                }

                log.Info("INICIA ObtieneVigenciaPlastico()");
                peticionAlta.VigenciaPlasticos = DAOBoveda.ObtieneVigenciaPlastico(peticionAlta.ID_Producto, 
                    usuario, AppID, logHeader);
                log.Info("TERMINA ObtieneVigenciaPlastico()");
                if (string.IsNullOrEmpty(peticionAlta.VigenciaPlasticos))
                {
                    throw new CAppException(8011, "El Emisor no tiene un valor asociado a la vigencia de los plásticos.");
                }

                peticionAlta.LlaveCifrado = Configuracion.Get(AppID, "CypherKey").Valor;
                peticionAlta.VectorCifrado = Configuracion.Get(AppID, "CypherVector").Valor;
                if (string.IsNullOrEmpty(peticionAlta.LlaveCifrado) || string.IsNullOrEmpty(peticionAlta.VectorCifrado))
                {
                    throw new CAppException(8011, "Los parámetros de cifrado no están configurados.");
                }

                peticionAlta.NombreEmbozo = Configuracion.Get(AppID, "EmbozoStock").Valor;
                if (string.IsNullOrEmpty(peticionAlta.NombreEmbozo))
                {
                    throw new CAppException(8011, "El nombre de embozo no está configurado.");
                }
            }
            catch (CAppException err)
            {
                log.Error(err.Mensaje());
                throw err;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8011, "Falla en la validación del contenido del archivo.");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para solicitar un lote de tarjetas al API Bóveda
        /// </summary>
        /// <param name="laPeticion">Objeto con los datos de la petición</param>
        /// <param name="losParametros">Objeto con los parámetros de la solicitud hacia el API</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Lote devuelto por el API</returns>
        public static RespuestasJSON.LotValues[] SolicitaTarjetasAPI(PeticionAltaTarjetas laPeticion, ParametrosAPI losParametros,
            Usuario elUsuario, Guid AppID, ILogHeader logHeader)
        {
            LogPCI log = new LogPCI(logHeader);
            string elLote = string.Empty;

            try
            {
                log.Info("INICIA DAOBoveda.InsertaPeticionTarjetas()");
                laPeticion.ID_Peticion = DAOBoveda.InsertaPeticionTarjetas(laPeticion, elUsuario, logHeader);
                log.Info("TERMINA DAOBoveda.InsertaPeticionTarjetas()");

                log.Info("INICIA GeneraLoteAPI()");
                RespuestasJSON.LotValues[] loteTarjetas = GeneraLoteAPI(losParametros, ref elLote, elUsuario, AppID, logHeader);
                laPeticion.NumLote = elLote;
                log.Info("TERMINA GeneraLoteAPI()");

                return loteTarjetas;
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }
            catch (Exception ex)
            {
                log.ErrorException(ex);
                throw new CAppException(8011, "Falla en la solicitud de Tarjetas Stock.");
            }
        }

        /// <summary>
        /// Establece las condiciones para generar y escribir en la ruta indicada el archivo
        /// con el lote de tarjetas
        /// </summary>
        /// <param name="claveProducto">Clave del producto del que se solicitó el lote</param>
        /// <param name="peticion">Objeto con los datos de la petición</param>
        /// <param name="lote">Lote de tarjetas recibido del API</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void GeneraArchivoStock(string claveProducto, PeticionAltaTarjetas peticion,
            RespuestasJSON.LotValues[] lote, ILogHeader logHeader)
        {
            LogPCI unLog = new LogPCI(logHeader);

            try
            {
                StringBuilder sbData = new StringBuilder();
                string laTarjeta = string.Empty;
                string fechaVigencia = string.Empty;

                if (!Directory.Exists(peticion.RutaEntradaArchivos))
                {
                    Directory.CreateDirectory(peticion.RutaEntradaArchivos);
                }

                DateTime laFechaHoy = DateTime.Now.AddYears(int.Parse(peticion.VigenciaPlasticos));
                fechaVigencia = laFechaHoy.ToString("dd/MM/yyyy");

                Cifrador elCifrador = new Cifrador(peticion.LlaveCifrado, peticion.VectorCifrado);

                unLog.Info("Genera_Archivo");
                
                foreach (RespuestasJSON.LotValues par in lote)
                {
                    laTarjeta = elCifrador.Descifrar(par.encrypted_value);
                    sbData.AppendFormat("{0}{1}{2}{3}{4}{5}", peticion.ClaveEmisor.PadRight(10, ' '), 
                        new string(' ', 15), laTarjeta.PadRight(29, ' '),
                        peticion.NombreEmbozo.PadRight(24, ' '), fechaVigencia, Environment.NewLine);
                }

                StringBuilder sbFileName = new StringBuilder();
                sbFileName.AppendFormat("STOCKCARDS_{0}_{1}_{2}.txt", peticion.ClaveEmisor.PadRight(10, ' '), claveProducto,
                    DateTime.Now.ToString("yyMMddHHmmss"));
                peticion.NombreArchivo = sbFileName.ToString();

                string path = peticion.RutaEntradaArchivos + peticion.NombreArchivo;
                unLog.Info("Escribe_Archivo");
                using (StreamWriter file = new StreamWriter(path))
                {
                    file.Write(sbData.ToString().Substring(0, sbData.Length - 2));
                }
            }
            catch (CAppException err)
            {
                unLog.Error(err.Mensaje());
                throw err;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8011, "Falla en la validación del contenido del archivo.");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la consulta de peticiones de tarjetas stock
        /// </summary>
        /// <param name="FechaInicial">Fecha inicial del periodo de consulta</param>
        /// <param name="FechaFinal">Fecha final del periodo de consulta</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        /// <returns>Lista con los datos de las solicitudes para petición de tarjetas</returns>
        public static List<PeticionAltaTarjetas> ConsultaPeticionesStock(DateTime FechaInicial, DateTime FechaFinal,
            Usuario elUsuario, Guid AppID, ILogHeader logHeader)
        {
            LogPCI unLog = new LogPCI(logHeader);

            try
            {
                unLog.Info("INICIA ObtienePeticionesStock()");
                List<PeticionAltaTarjetas> peticiones = DAOBoveda.ObtienePeticionesStock(FechaInicial, FechaFinal,
                    elUsuario, AppID, logHeader);
                unLog.Info("TERMINA ObtienePeticionesStock()");

                if (peticiones.Count > 0)
                {
                    unLog.Info("INICIA ObtieneRutaEntradaTarjetasStock()");
                    string dirEntrada = DAOBoveda.ObtieneRutaEntradaTarjetasStock(logHeader);
                    unLog.Info("TERMINA ObtieneRutaEntradaTarjetasStock()");

                    if (string.IsNullOrEmpty(dirEntrada))
                    {
                        throw new CAppException(8011, "El directorio de entrada de los archivos no está configurado.");
                    }
                    //////////TEMP
                    //dirEntrada = "C:\\TEMP\\Stock\\";

                    unLog.Info("INICIA ObtieneRutaSalidaTarjetasStock()");
                    string dirSalida = DAOBoveda.ObtieneRutaSalidaTarjetasStock(logHeader);
                    unLog.Info("TERMINA ObtieneRutaSalidaTarjetasStock()");
                    
                    //////////TEMP
                    //dirSalida = "C:\\TEMP\\Stock\\Salida\\";

                    if (string.IsNullOrEmpty(dirSalida))
                    {
                        throw new CAppException(8011, "El directorio de salida de los archivos no está configurado.");
                    }

                    EstableceEstatusRutaArchivosStock(dirEntrada, dirSalida, ref peticiones, AppID, logHeader);
                }

                return peticiones;
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8011, "Falla en la consulta de peticiones de tarjetas stock.");
            }
        }

        /// <summary>
        /// Establece el estatus de procesamiento y la ruta de donde se leerá el archivo de cada archivo dentro
        /// de la lista de solicitudes de tarjetas stock
        /// </summary>
        /// <param name="rutaEntrada">Ruta de entrada de archivos stock para procesamiento</param>
        /// <param name="rutaSalida">Ruta de salida de archivos stock para procesamiento</param>
        /// <param name="solicitudes">Lista de solicitudes</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        protected static void EstableceEstatusRutaArchivosStock(string rutaEntrada, string rutaSalida, 
            ref List<PeticionAltaTarjetas> solicitudes, Guid AppID, ILogHeader logHeader)
        {
            LogPCI log = new LogPCI(logHeader);
            string ruta, fullPath;

            try
            {
                foreach (PeticionAltaTarjetas solicitud in solicitudes)
                {
                    if (!string.IsNullOrEmpty(solicitud.NombreArchivo))
                    {
                        if (!rutaEntrada.EndsWith("\\"))
                        {
                            rutaEntrada += "\\";
                        }

                        ruta = rutaEntrada + solicitud.NombreArchivo;

                        //Si existe en el directorio de entrada, no se ha procesado
                        if (File.Exists(ruta))
                        {
                            solicitud.RutaArchivo = rutaEntrada;
                            solicitud.EstatusProcesamiento = "No Procesado";
                        }
                        else
                        {
                            if (!rutaSalida.EndsWith("\\"))
                            {
                                rutaSalida += "\\";
                            }

                            ruta = rutaSalida + Configuracion.Get(AppID, "SubCarpetaStockProcOK").Valor + "\\";
                            fullPath = ruta + Configuracion.Get(AppID, "PrefijoStockProcOK").Valor + solicitud.NombreArchivo;

                            //Procesado con éxito
                            if (File.Exists(fullPath))
                            {
                                solicitud.RutaArchivo = ruta;
                                solicitud.NombreArchivo = Configuracion.Get(AppID, "PrefijoStockProcOK").Valor + solicitud.NombreArchivo;
                                solicitud.EstatusProcesamiento = "Procesado";
                            }
                            else
                            {
                                ruta = rutaSalida + Configuracion.Get(AppID, "SubCarpetaStockProcNoOK").Valor + "\\";
                                fullPath = ruta + Configuracion.Get(AppID, "PrefijoStockProcNoOK").Valor + solicitud.NombreArchivo;

                                //Procesado no exitoso
                                if (File.Exists(fullPath))
                                {
                                    solicitud.RutaArchivo = ruta;
                                    solicitud.NombreArchivo = Configuracion.Get(AppID, "PrefijoStockProcNoOK").Valor + solicitud.NombreArchivo;
                                    solicitud.EstatusProcesamiento = "Procesado con Error";
                                }
                                else
                                {
                                    solicitud.RutaArchivo = string.Empty;
                                    solicitud.EstatusProcesamiento = "Archivo no Encontrado";
                                }
                            }
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
                log.ErrorException(ex);
                throw new CAppException(8011, "Falla al establecer los estatus y rutas de los archivos stock.");
            }
        }

        /// <summary>
        /// Realiza el cifrado del archivo con la ruta y el nombre indicados, dejándolo en la ruta de salida indicada
        /// </summary>
        /// <param name="rutaArchivo">Ruta del archivo por cifrar</param>
        /// <param name="nombreArchivo">Nombre del archivo por cifrar</param>
        /// <param name="outputPath">Ruta de salida donde quedará el archivo cifrado</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void CifraArchivoSolicitud(string rutaArchivo, string nombreArchivo, string outputPath, Guid AppID,
            ILogHeader logHeader)
        {
            LogPCI unLog = new LogPCI(logHeader);

            try
            {
                string original = rutaArchivo + nombreArchivo;
                string ascPathFile = outputPath + Configuracion.Get(AppID, "ArchivoSeguridad").Valor;

                //Si existe el directorio, se eliminan todos los archivos preexistentess
                if (Directory.Exists(outputPath))
                {
                    DirectoryInfo di = new DirectoryInfo(outputPath);
                    FileInfo[] files = di.GetFiles("*.txt").Where(p => p.Extension == ".txt").ToArray();

                    foreach (FileInfo file in files)
                    {
                        file.Attributes = FileAttributes.Normal;
                        File.Delete(file.FullName);
                    }
                }
                else //Si no existe el directorio, lo crea
                {
                    Directory.CreateDirectory(outputPath);
                }               

                outputPath += nombreArchivo;
                string cypherFile = outputPath + "_CYPH";

                File.Copy(original, outputPath);

                unLog.Info("getFirstPublicEncryptionKeyFromRing()");
                PgpPublicKey key = LNCifrador.getFirstPublicEncryptionKeyFromRing(
                    LNCifrador.asciiPublicKeyToRing(ascPathFile, logHeader));

                unLog.Info("EncryptFile()");
                LNCifrador.EncryptFile(outputPath, cypherFile, key, false, false, logHeader);

                File.Delete(outputPath);
                File.Move(cypherFile, outputPath);
            }
            catch (CAppException err)
            {
                unLog.Error(err.Mensaje());
                throw err;
            }
            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8011, "Falla al establecer el cifrado del archivo.");
            }
        }
    }
}
