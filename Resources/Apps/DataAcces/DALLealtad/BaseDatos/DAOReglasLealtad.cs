using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALLealtad.Utilidades;
using Interfases;
using Interfases.Exceptiones;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace DALLealtad.BaseDatos
{
    /// <summary>
    /// Clase de acceso a datos en la configuración de reglas de lealtad
    /// </summary>
    public class DAOReglasLealtad
    {
        /// <summary>
        /// Consulta el identificador de la cadena comercial que corresponde a la clave
        /// indicada dentro del Autorizador
        /// </summary>
        /// <param name="ClaveCadenaComercial">Clave de la cadena comercial</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>ID de la cadena comercial</returns>
        public static int ObtieneIDCadenaComercial(String ClaveCadenaComercial, IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorCash.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneIDColectivaDeClaveColectiva");

                database.AddInParameter(command, "@ClaveColectiva", DbType.String, ClaveCadenaComercial);

                return (int)database.ExecuteScalar(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta el catálogo de reglas de acumulación de puntos en el Autorizador (CASHAE)
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los registros</returns>
        public static DataSet ObtieneReglasAcumulacion(IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorCash.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneReglasAcumulacion");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta el catálogo de niveles de lealtad en el Autorizador
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataTable con los datos del catálogo</returns>
        public static DataTable ObtieneCatalogoNivelesLealtad(IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorCash.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneNivelesLealtad");

                return database.ExecuteDataSet(command).Tables[0];
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta en base de datos la lista de valores que corresponden a la regla,
        /// cadena y grupo de cuentas indicados
        /// </summary>
        /// <param name="ID_Regla">Identificador de la regla</param>
        /// <param name="ID_CadenaComercial">Identificador de la cadena comercial</param>
        /// <param name="ID_GrupoCuenta">Identificador del grupo de cuentas</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>Diccionario con los valores de la regla</returns>
        public static Dictionary<String, ValorRegla> ListaValoresReglaPorGpoCta(Int64 ID_Regla, Int64 ID_CadenaComercial,
            int ID_GrupoCuenta, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                Dictionary<String, ValorRegla> larespuesta = new Dictionary<String, ValorRegla>();
                DataTable laTabla = null;

                SqlDatabase database = new SqlDatabase(BDAutorizadorCash.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneValoresReglaPorGpoCuenta");

                database.AddInParameter(command, "@ID_CadenaComercial", DbType.Int64, ID_CadenaComercial);
                database.AddInParameter(command, "@ID_Regla", DbType.Int64, ID_Regla);
                database.AddInParameter(command, "@ID_GrupoCuenta", DbType.Int32, ID_GrupoCuenta);

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                laTabla = database.ExecuteDataSet(command).Tables[0];

                for (int k = 0; k < laTabla.Rows.Count; k++)
                {
                    ValorRegla unValorRegla = new ValorRegla();

                    unValorRegla.Descripcion = (laTabla.Rows[k]["Descripcion"]).ToString();
                    unValorRegla.esClave = Boolean.Parse((laTabla.Rows[k]["esClave"]).ToString());
                    unValorRegla.ID_TipoColectiva = Int32.Parse((laTabla.Rows[k]["ID_TipoColectiva"]).ToString());
                    unValorRegla.ID_ValorRegla = Int32.Parse((laTabla.Rows[k]["ID_ValorRegla"]).ToString());
                    unValorRegla.Nombre = (laTabla.Rows[k]["Nombre"]).ToString();
                    unValorRegla.TipoDatoSQL = (laTabla.Rows[k]["TipoDatoSQL"]).ToString();
                    unValorRegla.TipoDatoJava = (laTabla.Rows[k]["TipoDatoJava"]).ToString();
                    unValorRegla.Valor = (laTabla.Rows[k]["Valor"]).ToString();

                    larespuesta.Add(unValorRegla.Nombre, unValorRegla);
                }

                return larespuesta;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw ex;
            }
        }

        /// <summary>
        /// Actualiza en base de datos los valores de la regla, cadena y grupo de cuentas indicados
        /// </summary>
        /// <param name="elValor">Valor del parámetro</param>
        /// <param name="Id_cadena">Identificador de la cadena comercial</param>
        /// <param name="ID_Regla">Identificador de la regla</param>
        /// <param name="ID_GrupoCuenta">Identificador del grupo de cuentas</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        /// <param name="elUser">Usuario en sesión</param>
        public static void ActualizaValorReglaPorGpoCuenta(ValorRegla elValor, Int64 Id_cadena, Int64 ID_Regla,
            int ID_GrupoCuenta, SqlConnection connection, SqlTransaction transaccionSQL, IUsuario elUser)
        {
            try
            {
                SqlCommand command = new SqlCommand("web_CA_ActualizaValorReglaPorGpoCuenta", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Transaction = transaccionSQL;

                command.Parameters.Add(new SqlParameter("@ID_Cadena", Id_cadena));
                command.Parameters.Add(new SqlParameter("@ID_Regla", ID_Regla));
                command.Parameters.Add(new SqlParameter("@ID_GrupoCuenta", ID_GrupoCuenta));
                command.Parameters.Add(new SqlParameter("@ValorNuevo", elValor.Valor));
                command.Parameters.Add(new SqlParameter("@Nombre", elValor.Nombre));

                command.ExecuteNonQuery();
            }

            catch (Exception Ex)
            {
                throw new Exception("ActualizaValorReglaPorGpoCuenta()", Ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID_Regla"></param>
        /// <param name="ID_CadenaComercial"></param>
        /// <param name="ID_Valor"></param>
        /// <param name="elUsuario"></param>
        /// <param name="AppID"></param>
        /// <returns></returns>
        public static Dictionary<String, ValorRegla> ListaDeValoresReglaPredefinidos(Int64 ID_Regla, Int64 ID_CadenaComercial, Int64 ID_Valor,
            IUsuario elUsuario, Guid AppID)
        {
            try
            {
                Dictionary<String, ValorRegla> larespuesta = new Dictionary<String, ValorRegla>();
                DataTable laTabla = null;

                SqlDatabase database = new SqlDatabase(BDAutorizadorCash.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_ObtieneValoresReglasPredefinidos");
                database.AddInParameter(command, "@ID_Regla", DbType.Int64, ID_Regla);
                database.AddInParameter(command, "@ID_Valor", DbType.Int64, ID_Valor);
                database.AddInParameter(command, "@ID_CadenaComercial", DbType.Int64, ID_CadenaComercial);

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                laTabla = database.ExecuteDataSet(command).Tables[0];

                for (int k = 0; k < laTabla.Rows.Count; k++)
                {
                    ValorRegla unValorRegla = new ValorRegla();

                    unValorRegla.Descripcion = (laTabla.Rows[k]["Descripcion"]).ToString();
                    unValorRegla.esClave = Boolean.Parse((laTabla.Rows[k]["esClave"]).ToString());
                    unValorRegla.ID_TipoColectiva = Int32.Parse((laTabla.Rows[k]["ID_TipoColectiva"]).ToString());
                    unValorRegla.ID_ValorRegla = Int32.Parse((laTabla.Rows[k]["ID_ValorRegla"]).ToString());
                    unValorRegla.Nombre = (laTabla.Rows[k]["Nombre"]).ToString();
                    unValorRegla.TipoDatoSQL = (laTabla.Rows[k]["TipoDatoSQL"]).ToString();
                    unValorRegla.TipoDatoJava = (laTabla.Rows[k]["TipoDatoJava"]).ToString();
                    unValorRegla.Valor = (laTabla.Rows[k]["Valor"]).ToString();

                    larespuesta.Add(unValorRegla.Nombre + unValorRegla.Valor, unValorRegla);

                }

                return larespuesta;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw ex;
            }
        }
    }
}
