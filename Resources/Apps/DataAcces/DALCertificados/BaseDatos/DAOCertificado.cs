using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Configuration;
using Interfases;
using Interfases.Exceptiones;
using System.Data.SqlClient;
using DALCertificados.Entidades;
using DALCertificados.Utilidades;

namespace DALCertificados.BaseDatos
{
    public class DAOCertificado
    {
        public static Guid Crear(Certificado elCertificado, IUsuario elUsuario, SqlConnection DBConexion, SqlTransaction transaccionSQL)
        {
            try
            {

                SqlParameter param = null;

                SqlCommand comando = new SqlCommand("web_CrearCertificado", DBConexion);
                comando.CommandType = CommandType.StoredProcedure;
                comando.Transaction = transaccionSQL;

                //int resp = -1;


                param = new SqlParameter("@ID_CadenaComercial", SqlDbType.VarChar);
                param.Value = elCertificado.ID_CadenaComercial;
                comando.Parameters.Add(param);

                param = new SqlParameter("@ClaveUsuario", SqlDbType.VarChar);
                param.Value = elUsuario.ClaveUsuario;
                comando.Parameters.Add(param);

                param = new SqlParameter("@ClaveCertificado", SqlDbType.VarChar);
                param.Value = elCertificado.Clave;
                comando.Parameters.Add(param);

                param = new SqlParameter("@DiasExpiracion", SqlDbType.VarChar);
                param.Value = elCertificado.DiasExpiracion;
                comando.Parameters.Add(param);

               
                SqlParameter param3 = new SqlParameter("@ID_Certificado", SqlDbType.UniqueIdentifier);
                param3.Value = Guid.NewGuid();
                param3.Direction = ParameterDirection.InputOutput;
                comando.Parameters.Add(param3);



                comando.ExecuteNonQuery();


                return (Guid)param3.Value;
             


            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        public static int Desactivar(Guid IdCertificado, IUsuario elUsuario, SqlConnection DBConexion, SqlTransaction transaccionSQL)
        {
            try
            {

                SqlParameter param = null;

                SqlCommand comando = new SqlCommand("web_DesactivarCertificado", DBConexion);
                comando.CommandType = CommandType.StoredProcedure;
                comando.Transaction = transaccionSQL;

                //int resp = -1;

                param = new SqlParameter("@ID_Certificado", SqlDbType.UniqueIdentifier);
                param.Value = IdCertificado;
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


        public static int PDVOL_Desactivar(String elCertificado, SqlConnection DBConexion, SqlTransaction transaccionSQL)
        {
            try
            {

                SqlParameter param = null;

                SqlCommand comando = new SqlCommand("web_PDVOL_DesactivarCertificado", DBConexion);
                comando.CommandType = CommandType.StoredProcedure;
                comando.Transaction = transaccionSQL;

                //int resp = -1;

                param = new SqlParameter("@Certificado", SqlDbType.UniqueIdentifier);
                param.Value = elCertificado;
                comando.Parameters.Add(param);


                comando.ExecuteNonQuery();

                return 0;
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, "");
                throw new CAppException(8010, ex.Message, ex);
            }

        }

        public static DataSet ListaCertificados(Guid elUsuario, Guid AppId)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDCertificado.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_ConsultaCertificados");
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppId);

                Loguear.Evento("Consulto Certificados de Punto de Venta Web", elUsuario.ToString());

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ToString());
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        public static DataSet ListaCertificadosPorActivar(Guid elUsuario, Guid AppId)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDCertificado.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_ConsultaCertificadosPorActivar");
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppId);

                Loguear.Evento("Consulto Certificados de Punto de Venta Web", elUsuario.ToString());

                return database.ExecuteDataSet(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ToString());
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        public static Certificado ObtieneCertificado(String ClaveCertificado, Guid elUsuario, Guid AppId)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDCertificado.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_ConsultaCertificado");
                database.AddInParameter(command, "@ClaveCertificado", DbType.String, ClaveCertificado);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppId);

                Loguear.Evento("Consulto Certificados de Punto de Venta Web", elUsuario.ToString());

                return ObtieneFichaDeUnDataSet(database.ExecuteDataSet(command).Tables[0].Rows[0]);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ToString());
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        public static Certificado ObtieneCertificado(String ClaveCertificado)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDCertificado.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_ConsultaCertificado");
                database.AddInParameter(command, "@ClaveCertificado", DbType.String, ClaveCertificado);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, Guid.NewGuid());
                database.AddInParameter(command, "@AppId", DbType.Guid, Guid.NewGuid());

                Loguear.Evento("Consulto Certificados de Punto de Venta Web", "");

                DataSet unData = database.ExecuteDataSet(command);

                if (unData.Tables[0].Rows.Count >=1)
                {
                    return ObtieneFichaDeUnDataSet(unData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("Certificado no Existe en Base de Datos: " + ClaveCertificado + ", Registros:" + unData.Tables[0].Rows.Count);
                }
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, "");
                throw new Exception(ex.Message, ex);
            }
        }

        public static Certificado APP_ObtieneCertificado(String ClaveCertificado)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDCertificado.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_APP_ConsultaCertificado");
                database.AddInParameter(command, "@ClaveCertificado", DbType.String, ClaveCertificado);

                Loguear.Evento("Consulto Certificados de Punto de Venta Web", "");

                DataSet unData = database.ExecuteDataSet(command);

                

                if (unData.Tables[0].Rows.Count >= 1)
                {
                    return ObtieneFichaDeUnDataSet(unData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("Certificado no Existe en Base de Datos: " + ClaveCertificado + ", Registros:" + unData.Tables[0].Rows.Count);
                }
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, "");
                throw new Exception(ex.Message, ex);
            }
        }

        public static Boolean Activar(Certificado elCertificado, Guid elUsuario, Guid AppId)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDCertificado.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_ActivarCertificado");
                database.AddInParameter(command, "@ID_Certificado", DbType.Guid, elCertificado.ID_Certificado);
                database.AddInParameter(command, "@UsuarioActiva", DbType.String, elCertificado.UsuarioActivacion);
                database.AddInParameter(command, "@MAC", DbType.String, elCertificado.MAC);
                database.AddInParameter(command, "@IDPROC", DbType.String, elCertificado.IDPROC);
                database.AddInParameter(command, "@IDMB", DbType.String, elCertificado.IDMB);
                database.AddInParameter(command, "@IDWIN", DbType.String, elCertificado.IDWIN);
                database.AddInParameter(command, "@Sucursal", DbType.String, elCertificado.Sucursal);
                database.AddInParameter(command, "@Terminal", DbType.String, elCertificado.Terminal);
                database.AddInParameter(command, "@Afiliacion", DbType.String, elCertificado.Afiliacion);
           

                Loguear.Evento("Activacion de  Certificados", elUsuario.ToString());

                database.ExecuteDataSet(command);

                return true;
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ToString());
                throw new Exception( ex.Message, ex);
            }
        }

        public static Boolean AsignarColectivas(Certificado elCertificado, Guid elUsuario, Guid AppId)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDCertificado.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_AsignarColectivasaCertificado");
                database.AddInParameter(command, "@ClaveCertificado", DbType.String, elCertificado.Clave);
                database.AddInParameter(command, "@ID_Certificado", DbType.Guid, elCertificado.ID_Certificado);
                database.AddInParameter(command, "@ID_CadenaComercial", DbType.Int64, elCertificado.ID_CadenaComercial);
                database.AddInParameter(command, "@UsuarioActiva", DbType.String, elCertificado.UsuarioActivacion);
                database.AddInParameter(command, "@Sucursal", DbType.String, elCertificado.Sucursal);
                database.AddInParameter(command, "@Afiliacion", DbType.String, elCertificado.Afiliacion);
                database.AddInParameter(command, "@Terminal", DbType.String, elCertificado.Terminal);
                database.AddInParameter(command, "@ClaveCadena", DbType.String, elCertificado.ClaveCadena);
                database.AddInParameter(command, "@ID_ColectivaTerminal", DbType.String, elCertificado.ID_ColectivaTerminal);
                
                Loguear.Evento("Activacion de  Certificados", elUsuario.ToString());

                database.ExecuteDataSet(command);

                return true;
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ToString());
                throw new CAppException(8010, ex.Message, ex);
            }
        }



        public static Certificado APP_ObtieneCertificadoFromDispositivo(String ID_dispositivo)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDCertificado.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_APP_ConsultaCertificadoFromDispositivo");
                database.AddInParameter(command, "@IDDispositivo", DbType.String, ID_dispositivo);

                Loguear.Evento("Consulto Certificados de Punto de Venta Web", "");

                DataSet unData = database.ExecuteDataSet(command);



                if (unData.Tables[0].Rows.Count >= 1)
                {
                    return ObtieneFichaDeUnDataSet(unData.Tables[0].Rows[0]);
                }
                else
                {
                    throw new Exception("Certificado no Existe en Base de Datos: " + ID_dispositivo + ", Registros:" + unData.Tables[0].Rows.Count);
                }
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, "");
                throw new Exception(ex.Message, ex);
            }
        }


        public static Certificado ObtieneFichaDeUnDataSet(DataRow elData)
        {
            try
            {
                Certificado unCertificado = new Certificado
                {

                    FechaCreacion = DateTime.Parse(elData["FechaCreacion"].ToString()),
                    ID_CadenaComercial = Int64.Parse(elData["ID_CadenaComercial"].ToString()),
                    ID_Estatus = Int32.Parse(elData["ID_Estatus"].ToString()),
                    UsuarioCreacion = elData["UsuarioCreacion"].ToString(),
                    Clave = elData["Clave"].ToString(),
                    FechaCaducidad = DateTime.Parse(elData["FechaCaducidad"].ToString()),
                    FechaActivacion = DateTime.Parse(elData["FechaActivacion"].ToString()),
                    Sucursal = elData["Sucursal"].ToString(),
                    Afiliacion = elData["Afilacion"].ToString(),
                    Terminal = elData["Terminal"].ToString(),
                    ID_Certificado = Guid.Parse(elData["ID_Certificado"].ToString()),
                    ID_ColectivaTerminal = Int64.Parse(elData["ID_ColectivaTerminal"].ToString()),
                    ID_Activacion = Int32.Parse(elData["ID_Activacion"].ToString()),
                    IDMB = elData["IDMB"].ToString(),
                    IDPROC = elData["IDPROC"].ToString(),
                    MAC = elData["MAC"].ToString(),
                    UsuarioActivacion = elData["UsuarioActivacion"].ToString(),
                    ClaveCadena = elData["ClaveCadena"].ToString(),
                    
                };

                return unCertificado;
            }
            catch (Exception err)
            {
                Loguear.Error(err, "");
                throw new CAppException(8001, "Error al Obtener un Movimiento a partir del DataRow", err);
            }

        }
    }
}
