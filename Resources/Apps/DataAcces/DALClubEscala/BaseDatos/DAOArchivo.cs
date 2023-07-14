using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DALClubEscala.Entidades;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data;
using ClubEscala.Utilidades;
using System.Data.SqlClient;

namespace DALClubEscala.BaseDatos
{
    public class DAOArchivo
    {

        public static int Agregar(ref Archivo elArchivo, DALCentralAplicaciones.Entidades.Usuario elUsuario, DbConnection UnaConexion, DbTransaction UnaTransaccion)
        {
            try
            {

                DbCommand command = UnaConexion.CreateCommand();
                command.CommandText = "web_InsertarArchivo";
                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = UnaTransaccion;
                //int resp = -1;



                command.Parameters.Add(new SqlParameter("@Nombre", elArchivo.Nombre));
                command.Parameters.Add(new SqlParameter("@CA_Usuario", elArchivo.CA_Usuario));
                command.Parameters.Add(new SqlParameter("@ID_CadenaComercial", elArchivo.ID_CadenaComercial));
                //                command.Parameters.Add(new SqlParameter(   "@ResultadoProceso",  elArchivo.ResultadoProceso));
                command.Parameters.Add(new SqlParameter("@NombreArchivo", elArchivo.UrlArchivo));
                command.Parameters.Add(new SqlParameter("@EmailDistribucion", elUsuario.Email));

                SqlParameter salida = new SqlParameter("@ID_Archivo", 0);
                salida.Direction = ParameterDirection.Output;

                command.Parameters.Add(salida);

                command.ExecuteNonQuery();

                elArchivo.ID_Archivo = (Int64)command.Parameters["@ID_Archivo"].Value;

                return 0;

            }
            catch (Exception ex)
            {

                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new Exception(ex.Message);
            }
        }

        public static Int64 AgregarDetalle(Int64 Id_Archivo, ref List<Detalle> losDetalles, DALCentralAplicaciones.Entidades.Usuario elUsuario, DbConnection UnaConexion, DbTransaction UnaTransaccion)
        {
            try
            {
                foreach (Detalle unDetalle in losDetalles)
                {

                    DbCommand command = UnaConexion.CreateCommand();
                    command.CommandText = "web_InsertarDetalle";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Transaction = UnaTransaccion;
                    //int resp = -1;

                    command.Parameters.Add(new SqlParameter("@ID_Archivo", unDetalle.ID_Archivo));
                    command.Parameters.Add(new SqlParameter("@Fila", unDetalle.FilaCompleta));


                    SqlParameter salida = new SqlParameter("@ID_Detalle", 0);

                    salida.Direction = ParameterDirection.Output;

                    command.Parameters.Add(salida);
                    command.ExecuteNonQuery();

                    unDetalle.ID_Detalle = (Int64)command.Parameters["@ID_Detalle"].Value;
                }
                return 0;

            }
            catch (Exception ex)
            {

                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new Exception(ex.Message);
            }
        }

        public static DataSet ListaArchivos(DALCentralAplicaciones.Entidades.Usuario elUsuario, Guid AppId)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEmpleados.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_SeleccionaArchivos");
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppId);
                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new Exception(ex.Message);
            }
        }

        public static DataSet ListaDetalleArchivos(Int64 ID_Archivo, DALCentralAplicaciones.Entidades.Usuario elUsuario, Guid AppId)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDEmpleados.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_SeleccionaDetallesArchivos");
                database.AddInParameter(command, "@ID_Archivo", DbType.Int64, ID_Archivo);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppId);
                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new Exception(ex.Message);
            }
        }

        public static int Actualiza(Int64 ID_Archivo, EstatusArchivo elEstatus)
        {
            try
            {


                DbCommand command = BDEmpleados.BDEscritura.CreateCommand();
                command.CommandText = "web_ActualizaEstatusArchivo";
                command.CommandType = CommandType.StoredProcedure;
                //command.Transaction = UnaTransaccion;

                int resp = -1;

                command.Parameters.Add(new SqlParameter("@ID_Archivo", ID_Archivo));
                command.Parameters.Add(new SqlParameter("@ID_Estatus", (int)elEstatus));


                resp = command.ExecuteNonQuery();

                return 0;

            }
            catch (Exception ex)
            {
                Loguear.Error(ex, "ProcesadorNocturno");
                throw new Exception(ex.Message);
            }
        }


    }
}
