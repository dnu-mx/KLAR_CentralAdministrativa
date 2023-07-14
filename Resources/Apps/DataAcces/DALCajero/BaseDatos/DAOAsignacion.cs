using DALCajero.Entidades;
using DALCajero.Utilidades;
using Interfases;
using Interfases.Exceptiones;
using System;
using System.Data;
using System.Data.SqlClient;


namespace DALCajero.BaseDatos
{
    public class DAOAsignacion
    {


        public static int Asignar(Asignacion laAsignacion, IUsuario elUsuario, String ID_msgISO, /*String CodigoRespuesta,*/ SqlConnection DBConexion, SqlTransaction transaccionSQL)
        {

            try
            {
            SqlParameter param = null;

            SqlCommand comando = new SqlCommand("web_AsignarFichaDepositoMovimientoBancario", DBConexion);
			comando.CommandType = CommandType.StoredProcedure;
            comando.Transaction = transaccionSQL;

            
            //int resp = -1;
            param = new SqlParameter("@ID_Movimiento", SqlDbType.BigInt);
            param.Value = laAsignacion.elMovimiento.ID_Movimiento;
            comando.Parameters.Add(param);

            param = new SqlParameter("@ID_FichaDeposito", SqlDbType.BigInt);
            param.Value = laAsignacion.laFichaDeposito.ID_FichaDeposito;
            comando.Parameters.Add(param);

            param = new SqlParameter("@ClaveTipoAsignacion", SqlDbType.VarChar);
            param.Value = laAsignacion.ClaveTipoAsignacion;
            comando.Parameters.Add(param);

            param = new SqlParameter("@ClaveUsuarioAsignador", SqlDbType.VarChar);
            param.Value = elUsuario.ClaveUsuario;
            comando.Parameters.Add(param);

            param = new SqlParameter("@ID_MensajeISO", SqlDbType.VarChar);
            param.Value = ID_msgISO;
            comando.Parameters.Add(param);

            param = new SqlParameter("@ID_TipoOperacionTransaccional", SqlDbType.BigInt);
            param.Value = laAsignacion.tipoOperacionTrx.ID_TipoOperacionTransaccional;
            comando.Parameters.Add(param);

            param = new SqlParameter("@ClaveUsuario", SqlDbType.VarChar);
            param.Value = elUsuario.ClaveUsuario;
            comando.Parameters.Add(param);

            param = new SqlParameter("@PerfilesUsuario", SqlDbType.VarChar);
            param.Value = elUsuario.RolesToString();
            comando.Parameters.Add(param);

            param = new SqlParameter("@Auditor", SqlDbType.VarChar);
            param.Value = "";
            comando.Parameters.Add(param);

           

                comando.ExecuteNonQuery();
                Loguear.Evento("Se Asignó correctamente el Movimiento [" + laAsignacion.elMovimiento.ID_Movimiento + "] con la Ficha de Deposito [" + laAsignacion.laFichaDeposito.ID_FichaDeposito + "]", elUsuario.ClaveUsuario);
                return 0;
            }
            catch (Exception ex)
            {
                Loguear.Error("Error al Intentar Asignar el Movimiento " + laAsignacion.elMovimiento.ID_Movimiento +" con la Ficha de Deposito " + laAsignacion.laFichaDeposito.ID_FichaDeposito, elUsuario.ClaveUsuario);
                throw new CAppException (8007,"Ocurrio un error al Asignar la Ficha de Deposito al Movimiento Bancario",ex );
            }

        }

        public int Desasignar(Asignacion laAsignacion, IUsuario elUsuario, String ID_msgISO, String CodigoRespuesta, SqlConnection DBConexion, SqlTransaction transaccionSQL)
        { 
            
            try
            {

            SqlParameter param = null;

            SqlCommand comando = new SqlCommand("web_DesasignarFichaDepositoMovimientoBancario", DBConexion);
            comando.CommandType = CommandType.StoredProcedure;
            comando.Transaction = transaccionSQL;

            //int resp = -1;

            param = new SqlParameter("@ID_Movimiento", SqlDbType.BigInt);
            param.Value = laAsignacion.elMovimiento.ID_Movimiento;
            comando.Parameters.Add(param);

            param = new SqlParameter("@ID_FichaDeposito", SqlDbType.BigInt);
            param.Value = laAsignacion.laFichaDeposito.ID_FichaDeposito;
            comando.Parameters.Add(param);

            param = new SqlParameter("@ID_MBDeposito", SqlDbType.BigInt);
            param.Value = laAsignacion.ID_MBDeposito;
            comando.Parameters.Add(param);

            param = new SqlParameter("@ClaveUsuarioAsignador", SqlDbType.VarChar);
            param.Value = elUsuario.ClaveUsuario;
            comando.Parameters.Add(param);

            param = new SqlParameter("@ID_MensajeISO", SqlDbType.VarChar);
            param.Value = ID_msgISO;
            comando.Parameters.Add(param);

            param = new SqlParameter("@ID_TipoOperacionTransaccional", SqlDbType.BigInt);
            param.Value = laAsignacion.tipoOperacionTrx.ID_TipoOperacionTransaccional;
            comando.Parameters.Add(param);

            param = new SqlParameter("@ClaveUsuario", SqlDbType.VarChar);
            param.Value = elUsuario.ClaveUsuario;
            comando.Parameters.Add(param);

            param = new SqlParameter("@PerfilesUsuario", SqlDbType.VarChar);
            param.Value = elUsuario.RolesToString();
            comando.Parameters.Add(param);

            param = new SqlParameter("@Auditor", SqlDbType.VarChar);
            param.Value = "";
            comando.Parameters.Add(param);

           

                comando.ExecuteNonQuery();
            
                return 0;
            }
            catch (Exception ex)
            {

                throw new CAppException(8010, ex.Message, ex);
            }
            
        }

    }
}
