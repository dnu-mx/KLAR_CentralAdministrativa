using DALCajero.Entidades;
using DALCajero.Utilidades;
using Interfases;
using Interfases.Exceptiones;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace DALCajero.BaseDatos
{
    public class DAOMovimiento
    {
        //static string _DBCajeroConsulta = DALCajero.BaseDatos.BDCajero.strBDLectura;// ConfigurationManager.ConnectionStrings["CajeroConsulta"].ToString();
        //static string _DBCajeroEscritura = DALCajero.BaseDatos.BDCajero.strBDEscritura;// ConfigurationManager.ConnectionStrings["CajeroEscritura"].ToString();

        public static Int64 Insertar(Movimiento elMovimiento, IUsuario elUsuario, SqlConnection DBConexion, SqlTransaction transaccionSQL)
        {
            try
            {

                SqlParameter param = null;

                SqlCommand comando = new SqlCommand("web_AgregarMovimientoBancario", DBConexion);
                comando.CommandType = CommandType.StoredProcedure;
                comando.Transaction = transaccionSQL;

                //int resp = -1;

                param = new SqlParameter("@ClaveUsuarioRegistro", SqlDbType.VarChar);
                param.Value = elUsuario.ClaveUsuario;
                comando.Parameters.Add(param);

                param = new SqlParameter("@ClaveTipoRegistro", SqlDbType.VarChar);
                param.Value = elMovimiento.elTipoRegistro.Clave;
                comando.Parameters.Add(param);

                param = new SqlParameter("@ClaveColectivaBanco", SqlDbType.VarChar);
                param.Value = elMovimiento.ClaveColectivaBanco;
                comando.Parameters.Add(param);

                param = new SqlParameter("@ClaveSucursalBancaria", SqlDbType.VarChar);
                param.Value = elMovimiento.ClaveSucursalBancaria;
                comando.Parameters.Add(param);

                param = new SqlParameter("@ClaveCajaBancaria", SqlDbType.VarChar);
                param.Value = elMovimiento.ClaveCajaBancaria;
                comando.Parameters.Add(param);

                param = new SqlParameter("@ClaveOperador", SqlDbType.VarChar);
                param.Value = elMovimiento.ClaveOperador;
                comando.Parameters.Add(param);

                param = new SqlParameter("@FechaOperacion", SqlDbType.DateTime);
                param.Value = elMovimiento.FechaOperacion;
                comando.Parameters.Add(param);

                param = new SqlParameter("@ConsecutivoBancario", SqlDbType.VarChar);
                param.Value = elMovimiento.ConsecutivoBancario;
                comando.Parameters.Add(param);

                param = new SqlParameter("@Referencia", SqlDbType.VarChar);
                param.Value = elMovimiento.Referencia;
                comando.Parameters.Add(param);

                param = new SqlParameter("@Importe", SqlDbType.Money);
                param.Value = elMovimiento.Importe;
                comando.Parameters.Add(param);

                param = new SqlParameter("@Observaciones", SqlDbType.VarChar);
                param.Value = elMovimiento.Observaciones;
                comando.Parameters.Add(param);


                SqlParameter param2 = null;
                param2 = new SqlParameter("@ID_MensajeISO", SqlDbType.VarChar);
                param2.Value = elMovimiento.ID_MensajeISO;
                comando.Parameters.Add(param2);

                //param2 = new SqlParameter("@CodigoRespuesta", SqlDbType.VarChar);
                //param2.Value = elMovimiento.CodigoRespuesta;
                //comando.Parameters.Add(param2);

                if (elMovimiento.FechaValor.Year>2000)
                {
                    param2 = new SqlParameter("@FechaValor", SqlDbType.DateTime);
                    param2.Value = elMovimiento.FechaValor;
                    comando.Parameters.Add(param2);
                }
                else
                {
                    param2 = new SqlParameter("@FechaValor", SqlDbType.DateTime);
                    param2.Value = DateTime.Now;
                    comando.Parameters.Add(param2);
                }

                param2 = new SqlParameter("@NumeroCheque", SqlDbType.VarChar);
                param2.Value = elMovimiento.NumeroCheque;
                comando.Parameters.Add(param2);

                param2 = new SqlParameter("@ClaveTipoOperacionTransaccional", SqlDbType.VarChar);
                param2.Value = "000000"; // elMovimiento.elTipoOperacion.CodigoProceso;
                comando.Parameters.Add(param2);

                param2 = new SqlParameter("@Auditor", SqlDbType.VarChar);
                param2.Value = "";
                comando.Parameters.Add(param2);

                SqlParameter param3 = new SqlParameter("@ID_Movimiento", SqlDbType.BigInt);
                param3.Value = -1;
                param3.Direction = ParameterDirection.InputOutput;
                comando.Parameters.Add(param3);



                comando.ExecuteNonQuery();

                if (param2 != null)
                {
                    return (Int64)param3.Value;
                }
                else
                {
                    return -1;
                }

               
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        public static int Eliminar(Movimiento elMovimiento, IUsuario elUsuario, SqlConnection DBConexion, SqlTransaction transaccionSQL)
        {
            try
            {

                SqlParameter param = null;

                SqlCommand comando = new SqlCommand("web_EliminarMovimientoBancario", DBConexion);
                comando.CommandType = CommandType.StoredProcedure;
                comando.Transaction = transaccionSQL;

                //int resp = -1;

                param = new SqlParameter("@ID_Movimiento", SqlDbType.BigInt);
                param.Value = elMovimiento.ID_Movimiento;
                comando.Parameters.Add(param);

                param = new SqlParameter("@ClaveUsuarioRegistro", SqlDbType.VarChar);
                param.Value = elUsuario.ClaveUsuario;
                comando.Parameters.Add(param);

                param = new SqlParameter("@Observaciones", SqlDbType.VarChar);
                param.Value = elMovimiento.Observaciones;
                comando.Parameters.Add(param);

                param = new SqlParameter("@ID_MensajeISO", SqlDbType.VarChar);
                param.Value = elMovimiento.ID_MensajeISO;
                comando.Parameters.Add(param);

                //param = new SqlParameter("@CodigoRespuesta", SqlDbType.VarChar);
                //param.Value = elMovimiento.CodigoRespuesta;
                //comando.Parameters.Add(param);

                param = new SqlParameter("@ClaveTipoOperacionTransaccional", SqlDbType.VarChar);
                param.Value = "000000"; // elMovimiento.elTipoOperacion.CodigoProceso;
                comando.Parameters.Add(param);

                param = new SqlParameter("@Auditor", SqlDbType.VarChar);
                param.Value = "";
                comando.Parameters.Add(param);



                comando.ExecuteNonQuery();

                return 0;
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
           
        }

        public static int Modificar(Movimiento elMovimiento, IUsuario elUsuario, SqlConnection DBConexion, SqlTransaction transaccionSQL)
        {
            try
            {

                SqlParameter param = null;

                SqlCommand comando = new SqlCommand("web_ActualizarMovimientoBancario", DBConexion);
                comando.CommandType = CommandType.StoredProcedure;
                comando.Transaction = transaccionSQL;

                //int resp = -1;

                param = new SqlParameter("@ID_Movimiento", SqlDbType.BigInt);
                param.Value = elMovimiento.ID_Movimiento;
                comando.Parameters.Add(param);

                param = new SqlParameter("@ClaveEstatusMovimiento", SqlDbType.VarChar);
                param.Value = String.Format("{0}",(int)elMovimiento.elEstatus).PadLeft(3,'0');
                comando.Parameters.Add(param);

                param = new SqlParameter("@ClaveUsuarioRegistro", SqlDbType.VarChar);
                param.Value = elUsuario.ClaveUsuario;
                comando.Parameters.Add(param);

                //param = new SqlParameter("@ClaveTipoRegistro", SqlDbType.VarChar);
                //param.Value = elMovimiento.elTipoRegistro.Clave;
                //comando.Parameters.Add(param);

                param = new SqlParameter("@ClaveColectivaBanco", SqlDbType.VarChar);
                param.Value = elMovimiento.ClaveColectivaBanco;
                comando.Parameters.Add(param);

                param = new SqlParameter("@ClaveSucursalBancaria", SqlDbType.VarChar);
                param.Value = elMovimiento.ClaveSucursalBancaria;
                comando.Parameters.Add(param);

                param = new SqlParameter("@ClaveCajaBancaria", SqlDbType.VarChar);
                param.Value = elMovimiento.ClaveCajaBancaria;
                comando.Parameters.Add(param);

                param = new SqlParameter("@ClaveOperador", SqlDbType.VarChar);
                param.Value = elMovimiento.ClaveOperador;
                comando.Parameters.Add(param);

                param = new SqlParameter("@FechaOperacion", SqlDbType.DateTime);
                param.Value = elMovimiento.FechaOperacion;
                comando.Parameters.Add(param);

                param = new SqlParameter("@ConsecutivoBancario", SqlDbType.VarChar);
                param.Value = elMovimiento.ConsecutivoBancario;
                comando.Parameters.Add(param);

                param = new SqlParameter("@Referencia", SqlDbType.VarChar);
                param.Value = elMovimiento.Referencia;
                comando.Parameters.Add(param);

                param = new SqlParameter("@Importe", SqlDbType.Money);
                param.Value = elMovimiento.Importe;
                comando.Parameters.Add(param);

                param = new SqlParameter("@Observaciones", SqlDbType.VarChar);
                param.Value = elMovimiento.Observaciones;
                comando.Parameters.Add(param);

                param = new SqlParameter("@ID_MensajeISO", SqlDbType.VarChar);
                param.Value = elMovimiento.ID_MensajeISO;
                comando.Parameters.Add(param);

                //param = new SqlParameter("@CodigoRespuesta", SqlDbType.VarChar);
                //param.Value = elMovimiento.CodigoRespuesta;
                //comando.Parameters.Add(param);

                //param = new SqlParameter("@FechaValor", SqlDbType.DateTime);
                //param.Value = elMovimiento.FechaValor;
                //comando.Parameters.Add(param);

                param = new SqlParameter("@NumeroCheque", SqlDbType.VarChar);
                param.Value = elMovimiento.NumeroCheque == null ? "" : elMovimiento.NumeroCheque;
                comando.Parameters.Add(param);

                param = new SqlParameter("@ClaveTipoOperacionTransaccional", SqlDbType.VarChar);
                param.Value = "140002";// elMovimiento.elTipoOperacion.CodigoProceso;
                comando.Parameters.Add(param);

                param = new SqlParameter("@Auditor", SqlDbType.VarChar);
                param.Value = "";
                comando.Parameters.Add(param);
                comando.ExecuteNonQuery();

                return 0;
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
            
        }

        public static Movimiento BuscarParaAsginarFichaDeposito(FichaDeposito laFichaDeposito, IUsuario elUsuario, Guid AppId)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDCajero.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_SeleccionaMovimientoBancario");

                DataSet resp;

                database.AddInParameter(command, "@ClaveUsuarioRegistro", DbType.String, elUsuario.ClaveUsuario);
               // database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@ClaveColectivaBanco", DbType.String, laFichaDeposito.ClaveColectivaBanco);
                database.AddInParameter(command, "@ClaveSucursalBancaria", DbType.String, laFichaDeposito.ClaveSucursalBancaria);
                database.AddInParameter(command, "@ClaveCajaBancaria", DbType.String, laFichaDeposito.ClaveCajaBancaria);
                database.AddInParameter(command, "@ClaveOperador", DbType.String, laFichaDeposito.ClaveOperador);
                database.AddInParameter(command, "@FechaOperacion", DbType.DateTime, laFichaDeposito.FechaOperacion);
                database.AddInParameter(command, "@ConsecutivoBancario", DbType.String, laFichaDeposito.Consecutivo);
                database.AddInParameter(command, "@Referencia", DbType.String, laFichaDeposito.Referencia);
                database.AddInParameter(command, "@Importe", DbType.Currency, laFichaDeposito.Importe);
                database.AddInParameter(command, "@ID_TipoMovimiento", DbType.Int32, laFichaDeposito.Operacion.ID_TipoOperacionTransaccional);
                database.AddInParameter(command, "@Auditor", DbType.String, "");

                resp = database.ExecuteDataSet(command);

                //TODO: Generar el Objeto Movimiento para regresarlo en caso que exista
                Loguear.Evento("Se Consulto el Movimiento [" + ObtieneFichaDeUnDataSet(resp.Tables[0].Rows[0], elUsuario.ClaveUsuario).ID_Movimiento + "]", elUsuario.ClaveUsuario);
                //return resp;
                return ObtieneFichaDeUnDataSet(resp.Tables[0].Rows[0],elUsuario.ClaveUsuario);
            }
            catch (SqlException err)
            {
                Loguear.Error(err, elUsuario.ClaveUsuario);
                throw new CAppException(8001, err.Message, err);
            }
            catch (Exception err)
            {
                Loguear.Error(err, elUsuario.ClaveUsuario);
                throw new CAppException(8001, "Ocurrio un error en la Busqueda del Movimiento Bancario de la Ficha de Depósito", err);
            }


        }

        public static DataSet ListaMovimientos(IUsuario elUsuario, Guid AppId)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDCajero.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_SeleccionaMovimientosBancarios");
                //database.AddInParameter(command, "@ClaveUsuario", DbType.String, elUsuario.ClaveUsuario);
                //database.AddInParameter(command, "@ClaveUsuario", DbType.String, elUsuario.ClaveUsuario);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppId);

                Loguear.Evento("Consulto Movimientos Bancarios", elUsuario.ClaveUsuario);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        public static DataSet ListaMovimientosResguardo(IUsuario elUsuario, Guid AppId)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDCajero.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_SeleccionaMovimientosBancariosResguardo");
                //database.AddInParameter(command, "@ClaveUsuario", DbType.String, elUsuario.ClaveUsuario);
                //database.AddInParameter(command, "@ClaveUsuario", DbType.String, "");
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppId);
                Loguear.Evento("Consulto Movimientos Bancarios en Resguardo", elUsuario.ClaveUsuario);
                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        public static DataSet BuscaMovimientosResguardo(IUsuario elUsuario, Movimiento MovimientoUsuario, Guid AppId)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDCajero.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_BuscaMovimientoBancario");
                //database.AddInParameter(command, "@ClaveUsuario", DbType.String, elUsuario.ClaveUsuario);
                database.AddInParameter(command, "@ClaveUsuario", DbType.String, elUsuario.ClaveUsuario);
                database.AddInParameter(command, "@ClaveColectivaBanco", DbType.String, MovimientoUsuario.ClaveColectivaBanco);
                database.AddInParameter(command, "@ClaveSucursalBancaria", DbType.String, MovimientoUsuario.ClaveSucursalBancaria);
                database.AddInParameter(command, "@FechaOperacion", DbType.DateTime, MovimientoUsuario.FechaOperacion);
                database.AddInParameter(command, "@ConsecutivoBancario", DbType.Int64, MovimientoUsuario.ConsecutivoBancario);
                database.AddInParameter(command, "@Importe", DbType.Currency, MovimientoUsuario.Importe);
                database.AddInParameter(command, "@Auditor", DbType.String, "");
                database.AddInParameter(command, "@AppId", DbType.Guid, AppId);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);

                Loguear.Evento("Realizo una busqueda en Resguardo Movimientos Bancarios", elUsuario.ClaveUsuario);

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        public static Movimiento ConsultaMovimiento(Int64 ID_Movimiento, IUsuario elUsuario, Guid AppId)
        {
            try
            {

                SqlDatabase database = new SqlDatabase(BDCajero.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_SeleccionaUnMovimiento");
                database.AddInParameter(command, "@ID_Movimiento", DbType.Int64, ID_Movimiento);
                //database.AddInParameter(command, "@ClaveUsuario", DbType.String, elUsuario.ClaveUsuario);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@Auditor", DbType.String, "");
                database.AddInParameter(command, "@AppId", DbType.Guid, AppId);

                Loguear.Evento("Consulto Movimientos Bancarios ID [" + ID_Movimiento + "]", elUsuario.ClaveUsuario);
                return ObtieneFichaDeUnDataSet(database.ExecuteDataSet(command).Tables[0].Rows[0],elUsuario.ClaveUsuario);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        private static Movimiento ObtieneFichaDeUnDataSet(DataRow elData, String user)
        {
            try
            {
                Movimiento elMovimiento = new Movimiento
                {

                    ID_Movimiento = Int64.Parse(elData["ID_Movimiento"].ToString()),
                    elEstatus = (enumEstatusMovimiento)Int32.Parse(elData["Clave"].ToString()), //new EstatusMovimiento(elData["Clave"].ToString(), elData["Descripcion"].ToString(), Int32.Parse(elData["ID_EstatusMovimiento"].ToString())),
                    //elTipoOperacion = new TipoOperacionTransaccional(Int32.Parse(elData["ID_TipoOperacionTransaccional"].ToString()), elData["CodigoProceso"].ToString(), elData["DescripcionCodigoProceso"].ToString()),
                    ClaveUsuarioRegistro = elData["ClaveUsuario"].ToString(),
                    ClaveColectivaBanco = elData["ClaveColectivaBanco"].ToString(),
                    ClaveSucursalBancaria = elData["ClaveSucursalBancaria"].ToString(),
                    ClaveCajaBancaria = elData["ClaveCajaBancaria"].ToString(),
                    ClaveOperador = elData["ClaveOperador"].ToString(),
                    FechaOperacion = DateTime.Parse(elData["FechaOperacion"].ToString()),
                    ConsecutivoBancario = Int64.Parse(elData["ConsecutivoBancario"].ToString()),
                    Referencia = elData["Referencia"].ToString(),
                    Importe = float.Parse(elData["Importe"].ToString()),
                    Observaciones = elData["Observaciones"].ToString(),
                    FechaRegistro = DateTime.Parse(elData["FechaRegistro"].ToString())
                };

                return elMovimiento;
            }
            catch (Exception err)
            {
                Loguear.Error(err,user);
                throw new CAppException(8001, "Error al Obtener un Movimiento a partir del DataRow",err);
            }
           
        }
    }
}
