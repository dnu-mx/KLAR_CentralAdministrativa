using DALAutorizador.Utilidades;
using DALCentralAplicaciones.Entidades;
using Interfases.Exceptiones;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace DALLealtad.BaseDatos
{
    public class DAOCertificadosAmazon
    {
        
        /// <summary>
        /// Obtiene pedidos por mes
        /// </summary>
        /// <param name="Fecha">Fecha (mes/año) del periodo de consulta</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataTable con los datos del reporte</returns>
        public static DataSet ObtienePedidosPorMes(DateTime fecha, Usuario elUsuario)
        {
            try
            {
                Loguear.Evento("INICIO ObtienePedidosPorMes()", "");

                SqlDatabase database = new SqlDatabase(BDEcommerceCertificados.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtienePedidosPorMes");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@Fecha", DbType.Date, fecha);

                DataSet ds = database.ExecuteDataSet(command);
                Loguear.Evento("FIN ObtienePedidosPorMes()", "");

                return ds;
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Obtiene detalle del pedido
        /// </summary>
        /// <param name="IdPedido">Identificador del pedido</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataTable con los datos del reporte</returns>
        public static DataSet ObtieneDetallePedido(int idPedido, Usuario elUsuario)
        {
            try
            {
                Loguear.Evento("INICIO ObtieneDetallePedido()", "");

                SqlDatabase database = new SqlDatabase(BDEcommerceCertificados.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneDetallePedido");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@idPedido", DbType.Int32, idPedido);

                DataSet ds = database.ExecuteDataSet(command);
                Loguear.Evento("FIN ObtieneDetallePedido()", "");

                return ds;
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Obtiene codigos del pedido
        /// </summary>
        /// <param name="IdPedido">Identificador del pedido</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataTable con los datos del reporte</returns>
        public static DataTable ObtieneCodigosPedido(int idPedido, Usuario elUsuario)
        {
            try
            {
                Loguear.Evento("INICIO ObtieneCodigosPedido()", "");

                SqlDatabase database = new SqlDatabase(BDEcommerceCertificados.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneCertificados");
                command.CommandTimeout = 0;

                database.AddInParameter(command, "@idPedido", DbType.Int32, idPedido);

                DataSet ds = database.ExecuteDataSet(command);
                Loguear.Evento("FIN ObtieneCodigosPedido()", "");

                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Obtiene montos
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataTable con los datos del reporte</returns>
        public static DataSet ObtieneMontos(Usuario elUsuario)
        {
            try
            {
                Loguear.Evento("INICIO ObtieneMontos()", "");

                SqlDatabase database = new SqlDatabase(BDEcommerceCertificados.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneMontos");
                command.CommandTimeout = 0;

                DataSet ds = database.ExecuteDataSet(command);
                Loguear.Evento("FIN ObtieneMontos()", "");

                return ds;
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Inserta los pedidos en base de datos
        /// </summary>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>Cadena con el resultado del SP</returns>
        public static DataTable InsertaPedidoAmazon(int idCliente, int idSucursal, string detallePedidos, Usuario elUsuario)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(BDEcommerceCertificados.strBDEscritura))
                {
                    using (SqlCommand command = new SqlCommand("web_CA_GeneraPedidoAmazon", conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandTimeout = 0;

                        command.Parameters.Add(new SqlParameter("@Operador", elUsuario.ClaveUsuario));
                        command.Parameters.Add(new SqlParameter("@IdCliente", idCliente));
                        command.Parameters.Add(new SqlParameter("@IdSucursal", idSucursal));
                        command.Parameters.Add(new SqlParameter("@DetallePedidos", detallePedidos));

                        var sqlParameter1 = new SqlParameter("@IdPedido", SqlDbType.BigInt);
                        sqlParameter1.Direction = ParameterDirection.Output;
                        command.Parameters.Add(sqlParameter1);

                        var sqlParameter2 = new SqlParameter("@Mensaje", SqlDbType.VarChar);
                        sqlParameter2.Direction = ParameterDirection.Output;
                        sqlParameter2.Size = 200;
                        command.Parameters.Add(sqlParameter2);

                        conn.Open();

                        command.ExecuteNonQuery();

                        DataTable dt = new DataTable();
                        dt.Columns.Add("ID_Pedido");
                        dt.Columns.Add("Mensaje");
                        dt.Rows.Add();

                        dt.Rows[0]["ID_Pedido"] = sqlParameter1.Value.ToString();
                        dt.Rows[0]["Mensaje"] = sqlParameter2.Value.ToString();

                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }
    }
}
