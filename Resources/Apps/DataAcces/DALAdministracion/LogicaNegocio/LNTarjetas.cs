using DALAdministracion.BaseDatos;
using DALAdministracion.Entidades;
using DALAutorizador.BaseDatos;
using DALAutorizador.Utilidades;
using DALCentralAplicaciones.Entidades;
using HSM.Entidades;
using HSM.LogicaNegocio;
using Interfases;
using Interfases.Exceptiones;
using Log_PCI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DALAdministracion.LogicaNegocio
{
    /// <summary>
    /// Establece la lógica de negocio para la entidad Tarjeta
    /// </summary>
    public class LNTarjetas
    {
        /// <summary>
        /// Establece las condiciones de validación para la inserción de nuevos registros de tarjetas
        /// </summary>
        /// <param name="IdGrupoMA">Identificador del Grupo de Medios de Acceso</param>
        /// <param name="numInicial">Número inicial del rango de tarjetas</param>
        /// <param name="numFinal">Número final del rango de tarjetas</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void CreaLoteDeTarjetas(int IdGrupoMA, decimal numInicial, decimal numFinal, Usuario usuario)
        {
            try
            {
                using (SqlConnection conn = BDAutorizador.BDEscritura)
                {
                    conn.Open();

                    //Se crea la tabla de tarjetas
                    DataTable nuevasTarjetas = CreaTarjetas(IdGrupoMA, numInicial, numFinal, usuario);

                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                    {
                        bulkCopy.DestinationTableName = "dbo.MediosAcceso";

                        try
                        {
                            bulkCopy.WriteToServer(nuevasTarjetas);
                        }
                        catch (Exception ex)
                        {
                            throw new CAppException(8006, "Falla al Insertar el Lote de Tarjetas en Base de Datos ", ex);
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
        /// 
        /// </summary>
        /// <param name="IdGrupoMA"></param>
        /// <param name="numInicial"></param>
        /// <param name="numFinal"></param>
        /// <param name="usuario"></param>
        /// <returns></returns>
        private static DataTable CreaTarjetas(int IdGrupoMA, decimal numInicial, decimal numFinal, Usuario usuario)
        {
            try
            {
                DataTable nuevasTarjetas = new DataTable("NuevosMediosAcceso");

                //Se obtiene el consecutivo del id de MA de base de datos
                int idMACounter = DAOTarjeta.ConsultaUltimoIdMA(
                        usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                idMACounter++;

                // Add three column objects to the table. 
                DataColumn Id_MA = new DataColumn();
                Id_MA.DataType = System.Type.GetType("System.Int32");
                Id_MA.ColumnName = "ID_MA";
                nuevasTarjetas.Columns.Add(Id_MA);

                DataColumn Id_EstatusMA = new DataColumn();
                Id_EstatusMA.DataType = System.Type.GetType("System.Int32");
                Id_EstatusMA.ColumnName = "ID_EstatusMA";
                nuevasTarjetas.Columns.Add(Id_EstatusMA);

                DataColumn Id_TipoMA = new DataColumn();
                Id_TipoMA.DataType = System.Type.GetType("System.Int32");
                Id_TipoMA.ColumnName = "ID_TipoMA";
                nuevasTarjetas.Columns.Add(Id_TipoMA);

                DataColumn claveMA = new DataColumn();
                claveMA.DataType = System.Type.GetType("System.String");
                claveMA.ColumnName = "ClaveMA";
                nuevasTarjetas.Columns.Add(claveMA);

                DataColumn fechaAlta = new DataColumn();
                fechaAlta.DataType = System.Type.GetType("System.DateTime");
                fechaAlta.ColumnName = "FechaAlta";
                nuevasTarjetas.Columns.Add(fechaAlta);

                DataColumn expira = new DataColumn();
                expira.DataType = System.Type.GetType("System.DateTime");
                expira.ColumnName = "Expiracion";
                nuevasTarjetas.Columns.Add(expira);

                DataColumn cvv2 = new DataColumn();
                cvv2.DataType = System.Type.GetType("System.String");
                cvv2.ColumnName = "CVV2";
                nuevasTarjetas.Columns.Add(cvv2);

                DataColumn pin = new DataColumn();
                pin.DataType = System.Type.GetType("System.String");
                pin.ColumnName = "PIN";
                nuevasTarjetas.Columns.Add(Id_EstatusMA);

                DataColumn track1 = new DataColumn();
                track1.DataType = System.Type.GetType("System.String");
                track1.ColumnName = "Track1";
                nuevasTarjetas.Columns.Add(track1);

                DataColumn track2 = new DataColumn();
                track2.DataType = System.Type.GetType("System.String");
                track2.ColumnName = "Track2";
                nuevasTarjetas.Columns.Add(track2);

                DataColumn track3 = new DataColumn();
                track3.DataType = System.Type.GetType("System.String");
                track3.ColumnName = "Track3";
                nuevasTarjetas.Columns.Add(track3);

                DataColumn Id_GpoMA = new DataColumn();
                Id_GpoMA.DataType = System.Type.GetType("System.Int32");
                Id_GpoMA.ColumnName = "ID_GrupoMA";
                nuevasTarjetas.Columns.Add(Id_GpoMA);

                DataColumn cvv = new DataColumn();
                cvv.DataType = System.Type.GetType("System.String");
                cvv.ColumnName = "CVV";
                nuevasTarjetas.Columns.Add(cvv);

                // Create an array for DataColumn objects.
                DataColumn[] keys = new DataColumn[1];
                keys[0] = Id_MA;
                nuevasTarjetas.PrimaryKey = keys;

                DataRow row = null;
                DateTime dtFechaHoy = DateTime.Now;
                DateTime dtFechaExpiracion = dtFechaHoy.AddYears(20);
                Tarjeta unaTarjeta = new Tarjeta();
                int iDV = 0;
                string numTarjeta = "";

                for (decimal d = numInicial; d <= numFinal; d++)
                {   
                    row = nuevasTarjetas.NewRow();

                    row["ID_MA"] = idMACounter;
                    row["ID_EstatusMA"] = 1;
                    row["ID_TipoMA"] = 1;

                    //Si es el primer registro, se almacena tal cual
                    if (d == numInicial)
                    {
                        row["ClaveMA"] = d.ToString();
                        unaTarjeta.NumeroTarjeta = d.ToString();
                    }
                    else
                    {
                        iDV = GeneraDigitoVerificador(d++.ToString());
                        numTarjeta = d.ToString().Substring(0, 15) + iDV.ToString();
                        row["ClaveMA"] = numTarjeta;
                        unaTarjeta.NumeroTarjeta = numTarjeta;
                    }
                    
                    row["FechaAlta"] = dtFechaHoy;
                    row["Expiracion"] = dtFechaExpiracion;

                    unaTarjeta.FechaExpiracion = dtFechaExpiracion.ToString();
                    unaTarjeta.CodigoServicio = "000";

                    unaTarjeta = GeneraCVVs(unaTarjeta);

                    row["CVV2"] = unaTarjeta.CVV2;
                    row["PIN"] = unaTarjeta.NIP;
                    row["Track1"] = "";
                    row["Track2"] = "";
                    row["Track3"] = "";
                    row["Track3"] = "";
                    row["ID_GrupoMA"] = IdGrupoMA;
                    row["CVV"] = unaTarjeta.CVV;

                    nuevasTarjetas.Rows.Add(row);
                    idMACounter++;
                }

                nuevasTarjetas.AcceptChanges();

                // Return the new DataTable. 
                return nuevasTarjetas;
            }

            catch (Exception err)
            {
                Loguear.Error(err, usuario.ClaveUsuario);
                throw err;
            }
        }

        /// <summary>
        /// Genera en HSM los números de CVV de la tarjeta indicada
        /// </summary>
        /// <param name="laNuevaTarjeta">Datos de la nueva tarjeta</param>
        /// <returns>Valores de CVV devueltos por el HSM</returns>
        private static Tarjeta GeneraCVVs(Tarjeta laNuevaTarjeta)
        {

            Tarjeta valoresCVV = new Tarjeta();
               
            try
            {
                Dictionary<String, String> losValores = new Dictionary<String, String>();

                losValores.Add("@Tarjeta", laNuevaTarjeta.NumeroTarjeta);
                losValores.Add("@Expiracion", laNuevaTarjeta.FechaExpiracion);
                losValores.Add("@ServiceCode", laNuevaTarjeta.CodigoServicio);


                ResponseMensajeHSM laRespuestaCVV1 = LNProcesarToHSM.Procesar("CW", "172.16.10.12", 1500, losValores);

                if (laRespuestaCVV1.isAutorizado())
                {
                    valoresCVV.CVV = laRespuestaCVV1.getCampo("4");
                }
                else
                {
                    throw new Exception("NO SE PUDO GENERAR EL CVV");
                }


                losValores["@ServiceCode"] = "000";

                ResponseMensajeHSM laRespuestaCVV2 = LNProcesarToHSM.Procesar("CW", "172.16.10.12", 1500, losValores);

                if (laRespuestaCVV2.isAutorizado())
                {
                    valoresCVV.CVV2 = laRespuestaCVV2.getCampo("4");
                }
                else
                {
                    throw new Exception("NO SE PUDO GENERAR EL CVV2");
                }
            }

            catch (Exception err)
            {
                throw err;
            }

            return valoresCVV;
        }

        /// <summary>
        /// Genera el dígito verificador de la tarjeta usando el algoritmo de Luhn. 
        /// </summary>
        /// <param name="numTarjeta">Número de la tarjeta</param>
        /// <returns>Dígito verificador</returns>
        private static int GeneraDigitoVerificador(string numTarjeta)
        {
            int longtarjeta, suma, resultado, i;

            longtarjeta = numTarjeta.Length;
            suma = 0;
            resultado = 0;
            i = 1;

            while (i != longtarjeta)
            {
                resultado = ((i % 2) + 1) * (int.Parse(numTarjeta.Substring(longtarjeta - i - 1, 1)) & 15);

                if (resultado > 9)
                {
                    resultado -= 9;
                }

                suma += resultado;

                i++;
            }

            suma = (10 - (suma % 10)) % 10;

            return suma;
        }

        /// <summary>
        /// Establece las condiciones de validación para la modificación del grupo de medios
        /// de acceso de un BIN-GrupoMA en el Autorizador, controlando la transacción
        /// (commit o rollback)
        /// </summary>
        /// <param name="IdBinGrupoMA">Identificador del registro por modificar</param>
        /// <param name="IdGrupoMA">Identificador del nuevo grupo de medios de acceso</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void AsignaBinGrupoMA(int IdBinGrupoMA, int IdGrupoMA, Usuario usuario)
        {
            try
            {
                using (SqlConnection conn = BDAutorizador.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DAOTarjeta.ActualizaGrupoMABin(IdBinGrupoMA, IdGrupoMA, conn, transaccionSQL, usuario);
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
                            throw new CAppException(8006, "AsignaBinGrupoMA() Falla al modificar el grupo de medios de acceso del BIN/GrupoMA en el Autorizador", err);
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
        }

        /// <summary>
        /// Establece las condiciones de validación para la modificación del estatus de un BIN-
        /// Grupo de Medios de Acceso en el Autorizador, controlando la transacción
        /// (commit o rollback)
        /// </summary>
        /// <param name="IdBinGrupoMA">Identificador del Grupo de Medios de Acceso</param>
        /// <param name="Estatus">Valor del nuevo estatus (activo o inactivo)</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void ModificaEstatusBinGrupoMA(int IdBinGrupoMA, int Estatus, Usuario usuario)
        {
            try
            {
                using (SqlConnection conn = BDAutorizador.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DAOTarjeta.ActualizaEstatusBinGrupoMA(IdBinGrupoMA, Estatus, conn, transaccionSQL, usuario);
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
                            throw new CAppException(8006, "ModificaEstatusBinGrupoMA() Falla al modificar el estatus del BIN/GrupoMA en el Autorizador", err);
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
        }

        /// <summary>
        /// Establece las condiciones de validación para la inserción de un nuevo BIN-Grupo de
        /// Medios de Acceso en el Autorizador, controlando la transacción (commit o rollback)
        /// </summary>
        /// <param name="IdGrupoMA">Identificador del grupo de medios de acceso</param>
        /// <param name="ClaveBin">Clave del nuevo BIN</param>
        /// <param name="Descripcion">Descripción del nuevo BIN</param>
        /// <param name="Estatus">Número inicial del rango de tarjetas</param>
        /// <param name="usuario">Usuario en sesión</param>
        public static void CreaNuevoBinGrupoMA(int IdGrupoMA, string ClaveBin, string Descripcion, Usuario usuario)
        {
            if (ClaveBin.Length != 8)
            {
                throw new CAppException(8006, "La longitud del BIN es únicamente de 8 posiciones.");
            }

            try
            {
                using (SqlConnection conn = BDAutorizador.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            DAOTarjeta.InsertaBinGrupoMA(IdGrupoMA, ClaveBin, Descripcion, conn,
                                transaccionSQL, usuario);
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
                            throw new CAppException(8006, "CreaNuevoBinGrupoMA() Falla al insertar el BIN/GrupoMA en el Autorizador", err);
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
        }

        /// <summary>
        /// Establece las condiciones de validación para la modificación del estatus de un
        /// evento manual en el Autorizador
        /// </summary>
        /// <param name="idProducto">Identificador del Producto</param>
        /// <param name="Estatus">Valor del nuevo estatus (activo o inactivo)</param>
        /// <param name="usuario">Usuario en sesión</param>
        /// <param name="elLog">Instancia heredada del LogHeader para PCI</param>
        public static void ModificaEstatusEventoManual(int idProducto, int Estatus, Usuario elUsuario, Guid AppID,
            ILogHeader elLog)
        {
            LogPCI unLog = new LogPCI(elLog);

            try
            {
                DAOTarjeta.ActualizaEstatusEventoManual(idProducto, Estatus
                    ,  elUsuario, AppID, elLog);
            }
            catch (CAppException caEx)
            {
                throw caEx;
            }

            catch (Exception ex)
            {
                unLog.ErrorException(ex);
                throw new CAppException(8011, "Falla al modificar el estatus del evento manual.");
            }
        }
    }
}