using DALAutorizador.BaseDatos;
using DALAutorizador.Utilidades;
using DALCentralAplicaciones.Entidades;
using DALPuntoVentaWeb.Entidades;
using Interfases;
using Interfases.Exceptiones;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace DALPuntoVentaWeb.BaseDatos
{
    public class DAOPromociones
    {
        public static DataSet ListaPromociones(Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_TPV_ObtienePromociones");

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

        public static int insertar(Promocion laPromocion, Usuario elUsuario, Guid AppID)
        {
            try
            {
                int codigoRespuesta = -1;

                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_TPV_InsertarPromocion");

                database.AddInParameter(command, "@Clave_Promocion", DbType.String, laPromocion.Clave);
                database.AddInParameter(command, "@Descripcion_Promocion", DbType.String, laPromocion.Descripcion);
                database.AddInParameter(command, "@Meses_Promocion", DbType.Int32, laPromocion.Meses);
                database.AddInParameter(command, "@Etiqueta_Promocion", DbType.String, laPromocion.Etiqueta);
                database.AddInParameter(command, "@PrimerPago_Promocion", DbType.Int32, laPromocion.PrimerPago);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);
                
                codigoRespuesta = database.ExecuteNonQuery(command);
                Loguear.Evento("Se ha Agregado un Promocion al Autorizador", elUsuario.ClaveUsuario);
                return codigoRespuesta;
            }
            catch (Exception ex)
            {

                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        public static int actualizar(Promocion laPromocion, Usuario elUsuario, Guid AppID)
        {

            try
            {
                int codigoRespuesta = -1;

                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_TPV_ActualizarPromocion");

                database.AddInParameter(command, "@ID_Promocion", DbType.Int32, laPromocion.ID_Promocion);
                database.AddInParameter(command, "@Clave_Promocion", DbType.String, laPromocion.Clave);
                database.AddInParameter(command, "@Descripcion_Promocion", DbType.String, laPromocion.Descripcion);
                database.AddInParameter(command, "@Meses_Promocion", DbType.Int32, laPromocion.Meses);
                database.AddInParameter(command, "@Etiqueta_Promocion", DbType.String, laPromocion.Etiqueta);
                database.AddInParameter(command, "@PrimerPago_Promocion", DbType.Int32, laPromocion.PrimerPago);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                codigoRespuesta = database.ExecuteNonQuery(command);
                Loguear.Evento(String.Format("Se ha actualizado la promoción {0}", laPromocion.ID_Promocion), elUsuario.ClaveUsuario);
                return codigoRespuesta;
            }
            catch (Exception ex)
            {

                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        public static int eliminar(Promocion laPromocion, Usuario elUsuario, Guid AppID)
        {
            try
            {
                int codigoRespuesta = -1;

                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_TPV_EliminarPromocion");

                database.AddInParameter(command, "@ID_Promocion", DbType.Int32, laPromocion.ID_Promocion);
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppID);

                codigoRespuesta = database.ExecuteNonQuery(command);
                Loguear.Evento("Se ha eliminado una promoción del Autorizador", elUsuario.ClaveUsuario);
                return codigoRespuesta;
            }
            catch (Exception ex)
            {

                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        /// <summary>
        /// Obtiene las colectivas requeridas para los filtros de los datos de cupones
        /// </summary>
        /// <param name="ID_CadenaComercial">Identificador de la cadena comercial</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppId">Identificador de la aplicación</param>
        /// <returns>DataSet con la información de las colectivas requeridas</returns>
        public static DataSet ObtieneColectivasFiltrosDatosCupones(int ID_CadenaComercial, Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneColectivasFiltroCupones");

                database.AddInParameter(command, "@ID_CadenaComercial", DbType.Int32, ID_CadenaComercial);
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
        /// Consuta en base de datos los cupones con los filtros indicados
        /// </summary>
        /// <param name="IdCadena">Identificador de la cadena comercial</param>
        /// <param name="IdPromocion">Identificador de la promoción</param>
        /// <param name="Cupon">Clave del cupón</param>
        /// <param name="elUser">Usuario en sesión</param>
        /// <returns>DataSet con los registros de la consulta</returns>
        public static DataSet ConsultaCuponesPromocion(int IdCadena, int IdPromocion, String Cupon, 
            Usuario elUsuario, Guid AppID)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDLectura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ObtieneCuponesPromociones");

                database.AddInParameter(command, "@IdCadena", DbType.Int32, IdCadena);
                database.AddInParameter(command, "@IdPromocion", DbType.Int32, IdPromocion);
                database.AddInParameter(command, "@Cupon", DbType.String, Cupon);

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
        /// Actualiza los datos del cupón en base de datos
        /// </summary>
        /// <param name="idOp">Identificador de la operación</param>
        /// <param name="idCad">Identificador de la cadena comercial</param>
        /// <param name="cupon">Número de cupón</param>
        /// <param name="tkt">Nuevo valor del ticket</param>
        /// <param name="pago">Nueva forma de pago</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        public static void ActualizaDatosCupon(int idOp, int idCad, string cupon, string tkt, string pago,
            IUsuario elUsuario)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BDAutorizador.strBDEscritura);
                DbCommand command = database.GetStoredProcCommand("web_CA_ActualizaTicketFormaPagoCupon");

                database.AddInParameter(command, "@IdOperacion", DbType.Int32, idOp);
                database.AddInParameter(command, "@IdCadena", DbType.Int32, idCad);
                database.AddInParameter(command, "@Cupon", DbType.String, cupon);
                database.AddInParameter(command, "@Ticket", DbType.String, tkt);
                database.AddInParameter(command, "@FormaPago", DbType.String, pago);

                database.ExecuteNonQuery(command);
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }
    }
}
