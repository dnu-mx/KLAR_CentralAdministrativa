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
    public class DAOOperaciones
    {
        static string _DBCajeroConsulta = DALCajero.BaseDatos.BDCajero.strBDLectura;// ConfigurationManager.ConnectionStrings["CajeroConsulta"].ToString();
        static string _DBCajeroEscritura = DALCajero.BaseDatos.BDCajero.strBDEscritura;// ConfigurationManager.ConnectionStrings["CajeroEscritura"].ToString();


        public static DataSet ListaOperaciones(/*IUsuario elUsuario*/)
        {
            SqlDatabase database = new SqlDatabase(_DBCajeroConsulta);
            DbCommand command = database.GetStoredProcCommand("web_SeleccionaOperacionesTransaccionales");
            //database.AddInParameter(command, "@ClaveUsuario", DbType.String, elUsuario.ClaveUsuario);
            //database.AddInParameter(command, "@ClaveUsuario", DbType.String, "");
            return database.ExecuteDataSet(command);
        }

        public static int AgregarOperacion(TrxOperacion laTrx, RespuestaTransaccional laResp, IUsuario elUsuario, SqlConnection DBConexion, SqlTransaction transaccionSQL)
        {
            try
            {

                SqlParameter param = null;

                SqlCommand comando = new SqlCommand("web_AgregarOperacionTransaccional", DBConexion);
                comando.CommandType = CommandType.StoredProcedure;
                comando.Transaction = transaccionSQL;

                //int resp = -1;

                param = new SqlParameter("@NumeroAuditoria", SqlDbType.VarChar);
                param.Value = laTrx.Ticket;
                comando.Parameters.Add(param);


                param = new SqlParameter("@Sucursal", SqlDbType.VarChar);
                param.Value = laTrx.Sucursal;
                comando.Parameters.Add(param);

                param = new SqlParameter("@Afiliacion", SqlDbType.VarChar);
                param.Value = laTrx.Afiliacion;
                comando.Parameters.Add(param);

                param = new SqlParameter("@Terminal", SqlDbType.VarChar);
                param.Value = laTrx.Terminal;
                comando.Parameters.Add(param);

                param = new SqlParameter("@Ticket", SqlDbType.VarChar);
                param.Value = laTrx.Ticket;
                comando.Parameters.Add(param);

                    
                param = new SqlParameter("@Operador", SqlDbType.VarChar);
                param.Value = laTrx.Operador;
                comando.Parameters.Add(param);
                
                param = new SqlParameter("@Cod_Respuesta", SqlDbType.VarChar);
                param.Value = laResp.CodigoRespuesta == null ? "NULA" : laResp.CodigoRespuesta;
                comando.Parameters.Add(param);
                
                param = new SqlParameter("@Autorizacion", SqlDbType.VarChar);
                param.Value = laResp.Autorizacion == null ? "NULA" : laResp.Autorizacion;
                comando.Parameters.Add(param);

                    
                param = new SqlParameter("@DBUser", SqlDbType.VarChar);
                param.Value = elUsuario.ClaveUsuario;
                comando.Parameters.Add(param);

                param = new SqlParameter("@CveCiaServicios", SqlDbType.VarChar);
                param.Value = laTrx.Beneficiario;
                comando.Parameters.Add(param);

                param = new SqlParameter("@TipoMA", SqlDbType.VarChar);
                param.Value = laTrx.TipoMedioAcceso;
                comando.Parameters.Add(param);

                param = new SqlParameter("@ClaveMA", SqlDbType.VarChar);
                param.Value = laTrx.MedioAcceso;
                comando.Parameters.Add(param);

                param = new SqlParameter("@ReferenciaPagoServicio", SqlDbType.VarChar);
                param.Value = laTrx.Referencia;
                comando.Parameters.Add(param);

                param = new SqlParameter("@CodigoProceso", SqlDbType.VarChar);
                param.Value = laTrx.ProccesingCode;
                comando.Parameters.Add(param);

                param = new SqlParameter("@FechaOperacion", SqlDbType.DateTime);
                param.Value = laTrx.FechaTransaccion;
                comando.Parameters.Add(param);

                param = new SqlParameter("@CodigoMoneda", SqlDbType.VarChar);
                param.Value = laTrx.CodigoMoneda;
                comando.Parameters.Add(param);

                param = new SqlParameter("@Importe", SqlDbType.Decimal);
                param.Value = laTrx.Monto;
                comando.Parameters.Add(param);


                //resp = database.ExecuteNonQuery(command);

                comando.ExecuteNonQuery();

                //Loguear.Evento("Se ha Modificado Correctamente la Ficha de Deposito: " + laFichaDeposito.ID_FichaDeposito, elUsuario.ClaveUsuario);

                return 0;
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

    }
}
