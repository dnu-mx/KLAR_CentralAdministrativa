using DALCajero.Entidades;
using DALCajero.Utilidades;
using DALCentralAplicaciones.Entidades;
using Interfases;
using Interfases.Exceptiones;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace DALCajero.BaseDatos
{
    public class DAOFichaDeposito
    {


        //static string _DBCajeroConsulta = DALCajero.BaseDatos.BDCajero.strBDLectura;// ConfigurationManager.ConnectionStrings["CajeroConsulta"].ToString();
        //static string _DBCajeroEscritura = DALCajero.BaseDatos.BDCajero.strBDEscritura;// ConfigurationManager.ConnectionStrings["CajeroEscritura"].ToString();


        public static Int64 Insertar(FichaDeposito laFichaDeposito, IUsuario elUsuario, SqlConnection DBConexion, SqlTransaction transaccionSQL)
        {
            try{

            SqlParameter param = null;

            SqlCommand comando = new SqlCommand("web_AgregarFichaDepositoBancario", DBConexion);
            comando.CommandType = CommandType.StoredProcedure;
            comando.Transaction = transaccionSQL;

            //int resp = -1;

            param = new SqlParameter("@ClaveUsuario", SqlDbType.VarChar);
            param.Value = elUsuario.ClaveUsuario.Trim();
            comando.Parameters.Add(param);

            param = new SqlParameter("@ClaveColectivaBanco", SqlDbType.VarChar);
            param.Value = laFichaDeposito.ClaveColectivaBanco.Trim();
            comando.Parameters.Add(param);

            param = new SqlParameter("@ClaveSucursalBancaria", SqlDbType.VarChar);
            param.Value = laFichaDeposito.ClaveSucursalBancaria.Trim();
            comando.Parameters.Add(param);

            param = new SqlParameter("@ClaveCajaBancaria", SqlDbType.VarChar);
            param.Value = laFichaDeposito.ClaveCajaBancaria.Trim();
            comando.Parameters.Add(param);

            param = new SqlParameter("@ClaveOperador", SqlDbType.VarChar);
            param.Value = laFichaDeposito.ClaveOperador.Trim();
            comando.Parameters.Add(param);

            param = new SqlParameter("@FechaOperacion", SqlDbType.DateTime);
            param.Value = laFichaDeposito.FechaOperacion;
            comando.Parameters.Add(param);

            param = new SqlParameter("@Consecutivo", SqlDbType.BigInt);
            param.Value = laFichaDeposito.Consecutivo;
            comando.Parameters.Add(param);

            param = new SqlParameter("@Referencia", SqlDbType.VarChar);
            param.Value = laFichaDeposito.Referencia.Trim();
            comando.Parameters.Add(param);

            param = new SqlParameter("@Importe", SqlDbType.Money);
            param.Value = laFichaDeposito.Importe;
            comando.Parameters.Add(param);

            param = new SqlParameter("@Observaciones", SqlDbType.VarChar);
            param.Value = laFichaDeposito.Observaciones.Trim();
            comando.Parameters.Add(param);

            param = new SqlParameter("@ClaveTipoMA", SqlDbType.VarChar);
            param.Value = laFichaDeposito.ClaveTipoMA.Trim();
            comando.Parameters.Add(param);

            param = new SqlParameter("@ClaveMedioAcceso", SqlDbType.VarChar);
            param.Value = laFichaDeposito.ClaveMedioAcceso.Trim();
            comando.Parameters.Add(param);

            param = new SqlParameter("@Afiliacion", SqlDbType.VarChar);
            param.Value = laFichaDeposito.Afiliacion.Trim();
            comando.Parameters.Add(param);

            param = new SqlParameter("@AFLDescripcion", SqlDbType.VarChar);
            param.Value = laFichaDeposito.AFLDescripcion.Trim();
            comando.Parameters.Add(param);

            param = new SqlParameter("@DataTransaccionales", SqlDbType.VarChar);
            param.Value = laFichaDeposito.DataTransaccionales.Trim();
            comando.Parameters.Add(param);

            param = new SqlParameter("@ID_TipoOperacion", SqlDbType.Int);
            param.Value = laFichaDeposito.Operacion.ID_TipoOperacionTransaccional;
            comando.Parameters.Add(param);

            SqlParameter param2 = new SqlParameter("@ID_FichaDeposito", SqlDbType.BigInt);
            param2.Value = -1;
            param2.Direction = ParameterDirection.InputOutput;
            comando.Parameters.Add(param2);
            
           
            
                comando.ExecuteNonQuery();

                if (param2 != null)
                {
                    Loguear.Evento("Se ha Creado Correctamente la Ficha de Deposito: " + (Int64)param2.Value, elUsuario.ClaveUsuario);
                    return (Int64)param2.Value;
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

        public static int Eliminar(FichaDeposito laFichaDeposito, IUsuario elUsuario, SqlConnection DBConexion, SqlTransaction transaccionSQL)
        {
            try
            {

                SqlParameter param = null;

                SqlCommand comando = new SqlCommand("web_EliminarFichaDepositoBancario", DBConexion);
                comando.CommandType = CommandType.StoredProcedure;
                comando.Transaction = transaccionSQL;

                //int resp = -1;

                param = new SqlParameter("@ID_FichaDeposito", SqlDbType.BigInt);
                param.Value = laFichaDeposito.ID_FichaDeposito;
                comando.Parameters.Add(param);

                param = new SqlParameter("@ClaveUsuario", SqlDbType.VarChar);
                param.Value = elUsuario.ClaveUsuario;
                comando.Parameters.Add(param);

                param = new SqlParameter("@Auditor", SqlDbType.VarChar);
                param.Value = "";
                comando.Parameters.Add(param);

                //resp = database.ExecuteNonQuery(command);

                comando.ExecuteNonQuery();
                Loguear.Evento("Se ha Eliminado Correctamente la Ficha de Deposito: " + laFichaDeposito.ID_FichaDeposito, elUsuario.ClaveUsuario);
                return 0;
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        public static int Modificar(FichaDeposito laFichaDeposito, IUsuario elUsuario, SqlConnection DBConexion, SqlTransaction transaccionSQL)
        {
            try
            {

                SqlParameter param = null;

                SqlCommand comando = new SqlCommand("web_ModificarFichaDepositoBancario", DBConexion);
                comando.CommandType = CommandType.StoredProcedure;
                comando.Transaction = transaccionSQL;

                //int resp = -1;

                param = new SqlParameter("@ID_FichaDeposito", SqlDbType.VarChar);
                param.Value = laFichaDeposito.ID_FichaDeposito;
                comando.Parameters.Add(param);

                param = new SqlParameter("@ClaveUsuario", SqlDbType.VarChar);
                param.Value = elUsuario.ClaveUsuario;
                comando.Parameters.Add(param);

                param = new SqlParameter("@ClaveColectivaBanco", SqlDbType.VarChar);
                param.Value = laFichaDeposito.ClaveColectivaBanco;
                comando.Parameters.Add(param);

                param = new SqlParameter("@ClaveSucursalBancaria", SqlDbType.VarChar);
                param.Value = laFichaDeposito.ClaveSucursalBancaria;
                comando.Parameters.Add(param);

                param = new SqlParameter("@ClaveCajaBancaria", SqlDbType.VarChar);
                param.Value = laFichaDeposito.ClaveCajaBancaria;
                comando.Parameters.Add(param);

                param = new SqlParameter("@ClaveOperador", SqlDbType.VarChar);
                param.Value = laFichaDeposito.ClaveOperador;
                comando.Parameters.Add(param);

                param = new SqlParameter("@FechaOperacion", SqlDbType.VarChar);
                param.Value = laFichaDeposito.FechaOperacion;
                comando.Parameters.Add(param);

                param = new SqlParameter("@Consecutivo", SqlDbType.BigInt);
                param.Value = laFichaDeposito.Consecutivo;
                comando.Parameters.Add(param);

                param = new SqlParameter("@Referencia", SqlDbType.VarChar);
                param.Value = laFichaDeposito.Referencia;
                comando.Parameters.Add(param);

                param = new SqlParameter("@Importe", SqlDbType.Money);
                param.Value = laFichaDeposito.Importe;
                comando.Parameters.Add(param);

                param = new SqlParameter("@Observaciones", SqlDbType.VarChar);
                param.Value = laFichaDeposito.Observaciones;
                comando.Parameters.Add(param);

                param = new SqlParameter("@ClaveTipoMA", SqlDbType.VarChar);
                param.Value = laFichaDeposito.ClaveTipoMA;
                comando.Parameters.Add(param);

                param = new SqlParameter("@ClaveMedioAcceso", SqlDbType.VarChar);
                param.Value = laFichaDeposito.ClaveMedioAcceso;
                comando.Parameters.Add(param);

                param = new SqlParameter("@Afiliacion", SqlDbType.VarChar);
                param.Value = laFichaDeposito.Afiliacion;
                comando.Parameters.Add(param);

                param = new SqlParameter("@Auditor", SqlDbType.VarChar);
                param.Value = "";
                comando.Parameters.Add(param);

                param = new SqlParameter("@AFLDescripcion", SqlDbType.VarChar);
                param.Value = laFichaDeposito.AFLDescripcion;
                comando.Parameters.Add(param);

                //resp = database.ExecuteNonQuery(command);

                comando.ExecuteNonQuery();

                Loguear.Evento("Se ha Modificado Correctamente la Ficha de Deposito: " + laFichaDeposito.ID_FichaDeposito, elUsuario.ClaveUsuario);

                return 0;
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        public static FichaDeposito BuscarFichaParaAsignarMovimiento(Movimiento elMovimiento, IUsuario elUsuario, Guid AppId)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDCajero.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_SeleccionaFichaDeposito");

                DataSet resp;

                database.AddInParameter(command, "@ClaveUsuarioRegistro", DbType.String, elUsuario.ClaveUsuario);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@ClaveColectivaBanco", DbType.String, elMovimiento.ClaveColectivaBanco);
                database.AddInParameter(command, "@ClaveSucursalBancaria", DbType.String, elMovimiento.ClaveSucursalBancaria);
                database.AddInParameter(command, "@ClaveCajaBancaria", DbType.String, elMovimiento.ClaveCajaBancaria);
                database.AddInParameter(command, "@ClaveOperador", DbType.String, elMovimiento.ClaveOperador);
                database.AddInParameter(command, "@FechaOperacion", DbType.DateTime, elMovimiento.FechaOperacion);
                database.AddInParameter(command, "@ConsecutivoBancario", DbType.String, elMovimiento.ConsecutivoBancario);
                database.AddInParameter(command, "@Referencia", DbType.String, elMovimiento.Referencia);
                //database.AddInParameter(command, "@ID_TipoMovimiento", DbType.Int32, elMovimiento.elTipoMovimiento.ID_TipoMovimiento);
                database.AddInParameter(command, "@Importe", DbType.Currency, elMovimiento.Importe);
                database.AddInParameter(command, "@Auditor", DbType.String, "");
                database.AddInParameter(command, "@AppId", DbType.Guid, AppId);

                resp = database.ExecuteDataSet(command);

                

                return ObtieneFichaDeUnDataSet(database.ExecuteDataSet(command).Tables[0].Rows[0],elUsuario.ClaveUsuario); // new FichaDeposito();
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        public static DataSet ListaFichasDeposito(Usuario elUsuario, Guid AppId/*, String DBConexion*/)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDCajero.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_SeleccionaFichasDeposito");
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppId);
                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }


        public static DataSet ListaFichasDepositoResguardo(Usuario elUsuario, Guid AppId)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDCajero.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_SeleccionaFichasDepositoResguardo");
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppId);
                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        public static FichaDeposito ConsultaFichaDeposito(Int64 ID_Ficha, IUsuario elUsuario, Guid AppId)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDCajero.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_SeleccionaUnaFichaDeposito");
                database.AddInParameter(command, "@ID_Ficha", DbType.Int64, ID_Ficha);
                //database.AddInParameter(command, "@ClaveUsuario", DbType.String, elUsuario.ClaveUsuario);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppId);
                database.AddInParameter(command, "@Auditor", DbType.String, "");


                return ObtieneFichaDeUnDataSet(database.ExecuteDataSet(command).Tables[0].Rows[0],elUsuario.ClaveUsuario);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        private static FichaDeposito ObtieneFichaDeUnDataSet(DataRow elData, String User)
        {
            FichaDeposito laFichaDeposito = new FichaDeposito();

            laFichaDeposito.ID_FichaDeposito=Int64.Parse(elData["ID_FichaDeposito"].ToString());
            laFichaDeposito.elEstatus= new EstatusFichaDeposito (elData["Clave"].ToString(),elData["Descripcion"].ToString(),Int32.Parse(elData["ID_EstatusFichaDeposito"].ToString()));
            laFichaDeposito.Operacion = new TipoOperacionTransaccional(Int32.Parse(elData["ID_TipoOperacionTransaccional"].ToString()), elData["CodigoProceso"].ToString(), elData["DescripcionCodigoProceso"].ToString());
            laFichaDeposito.ClaveUsuario= elData["ClaveUsuario"].ToString();
            laFichaDeposito.ClaveColectivaBanco= elData["ClaveColectivaBanco"].ToString();
            laFichaDeposito.ClaveSucursalBancaria= elData["ClaveSucursalBancaria"].ToString();
            laFichaDeposito.ClaveCajaBancaria= elData["ClaveCajaBancaria"].ToString();
            laFichaDeposito.ClaveOperador= elData["ClaveOperador"].ToString();
            laFichaDeposito.FechaOperacion= elData["FechaOperacion"].ToString();
            laFichaDeposito.Consecutivo= Int64.Parse(elData["Consecutivo"].ToString());
            laFichaDeposito.Referencia= elData["Referencia"].ToString();
            laFichaDeposito.Importe= float.Parse(elData["Importe"].ToString());
            laFichaDeposito.ClaveTipoMA= elData["ClaveTipoMA"].ToString();
            laFichaDeposito.ClaveMedioAcceso= elData["ClaveMedioAcceso"].ToString();
            laFichaDeposito.Observaciones= elData["Observaciones"].ToString();
            laFichaDeposito.Afiliacion= elData["Afiliacion"].ToString();
            laFichaDeposito.FechaRegistro= elData["FechaRegistro"].ToString();
            laFichaDeposito.AFLDescripcion = elData["AFLDescripcion"].ToString();
            laFichaDeposito.DataTransaccionales = elData["DataTransaccionales"].ToString();

            Loguear.Evento("Se ha Consultado Correctamente la Ficha de Deposito: " + laFichaDeposito.ID_FichaDeposito, User);
            return laFichaDeposito;
        }
    }
}
