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
    /// Objetos de acceso a datos para la funcionalidad de CashAE
    /// </summary>
    public class DAOCash
    {
        /// <summary>
        /// Consulta el catálogo de niveles de lealtad en el Autorizador
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los datos del catálogo</returns>
        public static DataSet ObtieneCatalogoNivelesLealtad(IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorCash.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneNivelesLealtad");

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta el catálogo de promociones especiales en el Autorizador
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>DataSet con los datos del catálogo</returns>
        public static DataSet ObtieneCatalogoPromociones(IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorCash.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtienePromociones");

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta los parámetros necesarios para obtener los valores de la promoción especial en base de datos
        /// </summary>
        /// <param name="IdPromocion">Identificador de la promoción</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataTable con los registros</returns>
        public static DataTable ObtieneParamsPromoEspecial(int IdPromocion, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorCash.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneParamsPromoEspecial");

                database.AddInParameter(command, "@IdEvento", DbType.Int32, IdPromocion);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                return database.ExecuteDataSet(command).Tables[0];
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Consulta en base de datos la lista de valores que corresponden a la cadena comercial,
        /// promoción y grupo de cuentas indicados
        /// </summary>
        /// <param name="ID_CadenaComercial">Identificador de la cadena comercial</param>
        /// <param name="ID_Promocion">Identificador de la promoción</param>
        /// <param name="ID_GrupoCuenta">Identificador del grupo de cuentas</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>Diccionario con los valores de la regla</returns>
        public static Dictionary<String, ValorRegla> ListaValoresReglaPromoEspecial(Int64 ID_CadenaComercial, int ID_Promocion,
            int ID_GrupoCuenta, IUsuario elUsuario, Guid AppID)
        {
            try
            {
                Dictionary<String, ValorRegla> larespuesta = new Dictionary<String, ValorRegla>();
                DataTable laTabla = null;

                SqlDatabase database = new SqlDatabase(BDAutorizadorCash.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneValoresReglaPromoEspecial");

                database.AddInParameter(command, "@ID_CadenaComercial", DbType.Int64, ID_CadenaComercial);
                database.AddInParameter(command, "@ID_Evento", DbType.Int32, ID_Promocion);
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
        /// Consulta el catálogo de sucursales del Autorizador
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los datos del catálogo</returns>
        public static DataSet ObtieneCatalogoSucursales(IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorCash.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneCatalogoSucursales");

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
        /// Obtiene los detalles del nuevo evento (promoción especial) por crear 
        /// de base de datos
        /// </summary>
        /// <param name="elUser">Usuario en sesión</param>
        public static DataSet ObtieneDetallesEvento(IUsuario elUser)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorCash.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneDatosEventoPromoEspecial");

                return database.ExecuteDataSet(command);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUser.ClaveUsuario);
                throw ex;
            }
        }

        /// <summary>
        /// Realiza la creación de la colectiva, el medio de acceso y las cuentas en base de datos,
        /// así como la relación entre el medio de acceso creado con la colectiva y la cuenta tipo CCLC
        /// recién creadas de los roles Diconsa que lo requieren
        /// </summary>
        /// <param name="userId">Identificador del usuario al que se le dará el rol</param>
        /// <param name="name">Nombre propio del usuario al que se le dará el rol</param>
        /// <param name="userId">Identificador del usuario al que se le dará el rol</param>
        /// <param name="name">Nombre propio del usuario al que se le dará el rol</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <returns>Código y descripción del resultado de la ejecución del SP en base de datos</returns>
        public static void InsertaNuevaPromocionEspecial(int idEvento, string claveEvento, string promocion,
            long idCadena, IUsuario elUsuario)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(BDAutorizadorCash.strBDEscritura))
                using (SqlCommand cmd = new SqlCommand("web_CA_ClonarEventoYAsignarAPertencias_CASHAE", conn))
                {

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@ID_EventoOrigen", SqlDbType.Int);
                    cmd.Parameters.Add("@ClaveEventoNuevo", SqlDbType.VarChar, 10);
                    cmd.Parameters.Add("@DescripcionEventoNuevo", SqlDbType.VarChar, 100);
                    cmd.Parameters.Add("@ID_CadenaComercial", SqlDbType.BigInt);

                    cmd.Parameters["@ID_EventoOrigen"].Value = idEvento;
                    cmd.Parameters["@ClaveEventoNuevo"].Value = claveEvento;
                    cmd.Parameters["@DescripcionEventoNuevo"].Value = promocion;
                    cmd.Parameters["@ID_CadenaComercial"].Value = idCadena;

                    conn.Open();
                    cmd.ExecuteNonQuery();

                    conn.Close();
                }
            }

            catch (SqlException sqlEx)
            {
                throw sqlEx;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw ex;
            }
        }

        /// <summary>
        /// Consulta el catálogo de beneficiarios del Autorizador
        /// </summary>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppID">Identificador de la aplicación</param>
        /// <returns>DataSet con los datos del catálogo</returns>
        public static DataSet ObtieneCatalogoBeneficiarios(IUsuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizadorCash.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneCatalogoBeneficiarios");

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
        /// Actualiza en base de datos los valores de la regla, cadena y grupo de cuentas indicados
        /// </summary>
        /// <param name="elValor">Valor del parámetro</param>
        /// <param name="Id_cadena">Identificador de la cadena comercial</param>
        /// <param name="ID_Regla">Identificador de la regla</param>
        /// <param name="ID_GrupoCuenta">Identificador del grupo de cuentas</param>
        /// <param name="elUser">Usuario en sesión</param>
        /// <param name="connection">Conexión SQL prestablecida a la BD</param>
        /// <param name="transaccionSQL">Transacción SQL prestablecida</param>
        public static void ActualizaValorReglaPorGpoCuenta(ValorRegla elValor, Int64 Id_cadena, Int64 ID_Regla,
            int ID_GrupoCuenta, IUsuario elUser, SqlConnection connection, SqlTransaction transaccionSQL)
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
    }
}
