using DALComercioElectronico.Entidades;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DALComercioElectronico.Utilidades;
using Interfases.Exceptiones;
using DALCentralAplicaciones.Entidades;
using DALComercioElectronico.Negocio;


namespace DALComercioElectronico.BaseDatos
{
    public class DaoSucursales
    {
        public static List<Sucursal> GetSucursales( Usuario elUsuario, Guid AppId)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BdEcommerce.strBdEcomerce);
                DbCommand command = database.GetStoredProcCommand("web_EC_lista_sucursales_full");

                /*database.AddInParameter(command, "@FechaInicial", DbType.DateTime, fechaIni);
                database.AddInParameter(command, "@FechaFinal", DbType.DateTime, fechaFin);*/
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppId);

                var dataset = database.ExecuteDataSet(command);
                var dataList = dataset.Tables[0].DataTableToList<Sucursal>();
                //vat test = (DataTable)dataset;
                return dataList;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        public static int  AddSucursal(Sucursal sucursal, Usuario elUsuario, Guid AppId)
        {
            try
            {

                var date = DateTime.Now;
                SqlDatabase database = new SqlDatabase(BdEcommerce.strBdEcomerce);
                DbCommand command = database.GetStoredProcCommand("web_EC_agregar_sucursal");

                /*database.AddInParameter(command, "@FechaInicial", DbType.DateTime, fechaIni);
                database.AddInParameter(command, "@FechaFinal", DbType.DateTime, fechaFin);*/
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppId);

                //database.AddInParameter(command, "@id_sucursal", DbType.Int32, sucursal.id_sucursal);
                database.AddInParameter(command, "@clave", DbType.String, sucursal.clave);
                database.AddInParameter(command, "@nombre", DbType.String, sucursal.nombre);
                database.AddInParameter(command, "@id_sucursal_madre", DbType.Int32, sucursal.id_sucursal_madre);

                database.AddInParameter(command, "@secuencia", DbType.Int32, sucursal.secuencia);
                database.AddInParameter(command, "@path_imagen", DbType.String, sucursal.path_imagen);
                database.AddInParameter(command, "@coordenadas", DbType.String, sucursal.coordenadas);
                database.AddInParameter(command, "@responsable", DbType.String, sucursal.responsable);

             

                database.AddInParameter(command, "@calle", DbType.String, sucursal.calle);
                database.AddInParameter(command, "@colonia", DbType.String, sucursal.colonia);
                database.AddInParameter(command, "@ciudad", DbType.String, sucursal.ciudad);
                database.AddInParameter(command, "@estado", DbType.String, sucursal.estado);


                database.AddInParameter(command, "@cp", DbType.Int32, sucursal.cp);
                database.AddInParameter(command, "@telefono", DbType.String, sucursal.telefono);
                database.AddInParameter(command, "@activa", DbType.Boolean, sucursal.activa);

                database.AddInParameter(command, "@cargo_envio", DbType.Decimal, sucursal.cargo_envio);
                database.AddInParameter(command, "@minimo_para_entrega", DbType.Decimal, sucursal.minimo_para_entrega);



           

                database.AddInParameter(command, "@modificado_por", DbType.String, elUsuario.ClaveUsuario);
                database.AddInParameter(command, "@fecha_modificacion", DbType.DateTime, date);
                database.AddInParameter(command, "@Insertado_por", DbType.String, elUsuario.ClaveUsuario);
                database.AddInParameter(command, "@fecha_Insertado", DbType.DateTime, date);

                

                database.AddInParameter(command, "@URL_PuntoVenta", DbType.String, sucursal.URL_PuntoVenta);

                

                database.ExecuteScalar(command);

                //var dataset = database.ExecuteDataSet(command);
                //var dataList = dataset.Tables[0].DataTableToList<Sucursal>();
                //vat test = (DataTable)dataset;
                return 0;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);

                throw ex;
                //throw new ex(8010, ex.Message, ex);
            }
        }

        public static int EditSucursal(Sucursal sucursal, Usuario elUsuario, Guid AppId)
        {
            try
            {

                var date = DateTime.Now;
                SqlDatabase database = new SqlDatabase(BdEcommerce.strBdEcomerce);
                DbCommand command = database.GetStoredProcCommand("web_EC_editar_sucursal");

                /*database.AddInParameter(command, "@FechaInicial", DbType.DateTime, fechaIni);
                database.AddInParameter(command, "@FechaFinal", DbType.DateTime, fechaFin);*/
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppId);

                database.AddInParameter(command, "@id_sucursal", DbType.Int32, sucursal.id_sucursal);
                database.AddInParameter(command, "@clave", DbType.String, sucursal.clave);
                database.AddInParameter(command, "@nombre", DbType.String, sucursal.nombre);
                database.AddInParameter(command, "@id_sucursal_madre", DbType.Int32, sucursal.id_sucursal_madre);

                database.AddInParameter(command, "@secuencia", DbType.Int32, sucursal.secuencia);
                database.AddInParameter(command, "@path_imagen", DbType.String, sucursal.path_imagen);
                database.AddInParameter(command, "@coordenadas", DbType.String, sucursal.coordenadas);
                database.AddInParameter(command, "@responsable", DbType.String, sucursal.responsable);



                database.AddInParameter(command, "@calle", DbType.String, sucursal.calle);
                database.AddInParameter(command, "@colonia", DbType.String, sucursal.colonia);
                database.AddInParameter(command, "@ciudad", DbType.String, sucursal.ciudad);
                database.AddInParameter(command, "@estado", DbType.String, sucursal.estado);


                database.AddInParameter(command, "@cp", DbType.Int32, sucursal.cp);
                database.AddInParameter(command, "@telefono", DbType.String, sucursal.telefono);
                database.AddInParameter(command, "@activa", DbType.Boolean, sucursal.activa);

                database.AddInParameter(command, "@cargo_envio", DbType.Decimal, sucursal.cargo_envio);
                database.AddInParameter(command, "@minimo_para_entrega", DbType.Decimal, sucursal.minimo_para_entrega);





                database.AddInParameter(command, "@modificado_por", DbType.String, elUsuario.ClaveUsuario);
                database.AddInParameter(command, "@fecha_modificacion", DbType.DateTime, date);
                //database.AddInParameter(command, "@Insertado_por", DbType.String, elUsuario.ClaveUsuario);
                //database.AddInParameter(command, "@fecha_Insertado", DbType.DateTime, date);

                database.AddInParameter(command, "@URL_PuntoVenta", DbType.String, sucursal.URL_PuntoVenta);

                
                
                database.ExecuteNonQuery(command);

                //var dataset = database.ExecuteDataSet(command);
                //var dataList = dataset.Tables[0].DataTableToList<Sucursal>();
                //vat test = (DataTable)dataset;
                return 0;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw ex;
                //throw new CAppException(8010, ex.Message, ex);
            }
        }

        public static SucursalHorario GetSucursalHorario(int id_sucursal, Usuario elUsuario, Guid AppId)
        {

            try
            {
                SqlDatabase database = new SqlDatabase(BdEcommerce.strBdEcomerce);
                DbCommand command = database.GetStoredProcCommand("web_EC_obtener_horarios_sucursal");

                
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppId);
                database.AddInParameter(command, "@id_sucursal", DbType.Int32, id_sucursal);

                var date = DateTime.Now;
                database.AddInParameter(command, "@Insertado_por", DbType.String, elUsuario.ClaveUsuario);
                database.AddInParameter(command, "@fecha_Insertado", DbType.DateTime, date);

                var dataset = database.ExecuteDataSet(command);
                var dataList = dataset.Tables[0].DataTableToList<SucursalHorario>();

                
                var sucHorario=  dataList[0];

                sucHorario.SetTimeSpans();
                return sucHorario;
                //Extensions.
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                
                throw ex;
            }

        }

        public static void UpdateSucursalHorario(SucursalHorario sucursalHorario, Usuario elUsuario, Guid AppId)
        {

            try
            {
                SqlDatabase database = new SqlDatabase(BdEcommerce.strBdEcomerce);
                DbCommand command = database.GetStoredProcCommand("web_EC_actualizar_horarios_sucursal");


                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppId);
                database.AddInParameter(command, "@id_sucursal", DbType.Int32, sucursalHorario.id_sucursal);
                database.AddInParameter(command, "@id", DbType.Int32, sucursalHorario.id);

                var date = DateTime.Now;
                database.AddInParameter(command, "@modificado_por", DbType.String, elUsuario.ClaveUsuario);
                database.AddInParameter(command, "@fecha_modificacion", DbType.DateTime, date);

                var horarioString=sucursalHorario.GenerateHorariosString();


                database.AddInParameter(command, "@horarios", DbType.String,  horarioString );

                //database.AddInParameter(command, "@Insertado_por", DbType.String, elUsuario.ClaveUsuario);
                //database.AddInParameter(command, "@fecha_Insertado", DbType.DateTime, date);

                var dataset = database.ExecuteNonQuery(command);

                UpdateHorariosDiarios(sucursalHorario,elUsuario, AppId);

                

                //var dataList = dataset.Tables[0].DataTableToList<SucursalHorario>();


                //var sucHorario = dataList[0];

                //sucHorario.SetTimeSpans();
                //return sucHorario;
                //Extensions.
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);

                throw ex;
            }

        }

        public static void UpdateHorariosDiarios(SucursalHorario sucursalHorario, Usuario elUsuario, Guid AppId)
        {
            SqlDatabase database = new SqlDatabase(BdEcommerce.strBdEcomerce);

            DbCommand zcommand = database.GetStoredProcCommand("web_EC_actualizar_horario_diario_sucursal_limpia");

            database.AddInParameter(zcommand, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
            database.AddInParameter(zcommand, "@AppId", DbType.Guid, AppId);
            database.AddInParameter(zcommand, "@id_sucursal", DbType.Int32, sucursalHorario.id_sucursal);
            database.ExecuteNonQuery(zcommand);

            foreach (var item in sucursalHorario.WorkDays.OrderBy(m => m.IdDay))
            {

            
                    DbCommand command = database.GetStoredProcCommand("web_EC_actualizar_horario_diario_sucursal");


                    database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                    database.AddInParameter(command, "@AppId", DbType.Guid, AppId);
                    database.AddInParameter(command, "@id_sucursal", DbType.Int32, sucursalHorario.id_sucursal);

                    database.AddInParameter(command, "@id_diasemana", DbType.Int32, item.IdDay);
                    database.AddInParameter(command, "@horaInicio", DbType.Time, item.Range[0].ToShortTimeString());
                    database.AddInParameter(command, "@horaFin", DbType.Time, item.Range[1].ToShortTimeString());

                    var dataset = database.ExecuteNonQuery(command);

                    

                //database.AddInParameter(command, "@id_diasemana", DbType.Time, item.);
                //database.AddInParameter(command, "@id_diasemana", DbType.Time, item.IdDay);
                


            }



        }

        public static List<AreaServicio> GetCoverage(int id_sucursal, Usuario user, Guid idApplication)
        {

            try
            {
                SqlDatabase database = new SqlDatabase(BdEcommerce.strBdEcomerce);
                DbCommand command = database.GetStoredProcCommand("web_EC_obtener_area_servicio_sucursal");


                database.AddInParameter(command, "@UserTemp", DbType.Guid, user.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, idApplication);
                database.AddInParameter(command, "@id_sucursal", DbType.Int32, id_sucursal);

                //var date = DateTime.Now;
                //database.AddInParameter(command, "@Insertado_por", DbType.String, elUsuario.ClaveUsuario);
                //database.AddInParameter(command, "@fecha_Insertado", DbType.DateTime, date);

                var dataset = database.ExecuteDataSet(command);
                var dataList = dataset.Tables[0].DataTableToList<AreaServicio>();


                //var sucHorario = dataList[0];

                
                return dataList;
                //Extensions.
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, user.ClaveUsuario);

                throw ex;
            }
            
        }

        public static List<Asentamiento> GetAsentamientosFilter(string codigo_postal, Usuario user, Guid idApplication)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BdEcommerce.strBdEcomerce);
                DbCommand command = database.GetStoredProcCommand("web_EC_obtener_asentamientos_filtro");


                database.AddInParameter(command, "@UserTemp", DbType.Guid, user.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, idApplication);
                database.AddInParameter(command, "@CodigoPostal", DbType.String, codigo_postal);

                //var date = DateTime.Now;
                //database.AddInParameter(command, "@Insertado_por", DbType.String, elUsuario.ClaveUsuario);
                //database.AddInParameter(command, "@fecha_Insertado", DbType.DateTime, date);

                var dataset = database.ExecuteDataSet(command);
                var dataList = dataset.Tables[0].DataTableToList<Asentamiento>();


                //var sucHorario = dataList[0];


                return dataList;
                //Extensions.
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, user.ClaveUsuario);

                throw ex;
            }
        }

        public static void UpdateCoverage(DtoSucursal dtoSucursal, Usuario user, Guid idApplication)
        {
            var areasActuales = GetCoverage(dtoSucursal.id_sucursal, user, idApplication);

            var editedAreas = dtoSucursal.AreasServicios;
            foreach (var areaServicioOrig in areasActuales)
            {

                var editedArea = editedAreas.FirstOrDefault(m => m.id == areaServicioOrig.id);

                if (editedArea == null)
                    UpdateAreaServicio(areaServicioOrig, user, idApplication, true);


            }

            var nuevos = editedAreas.Where(m => m.id <= 0);

            foreach (var pasoCombo in nuevos)
            {
                UpdateAreaServicio(pasoCombo, user, idApplication);
            }



            /*  foreach (var areaServicio in areasServicio)
              {
  
                  if (areaServicio.id == 0)
                  {
  
                  }
                  else
                  {
                      
                  }
              }*/

        }

        private static void UpdateAreaServicio(AreaServicio areaServicio, Usuario user, Guid idApplication, bool delete=false)
        {

             try
            {

                //SqlDatabase database = new SqlDatabase(BdEcommerce.strBdEcomerce);
                var database = new SqlDatabase(BdEcommerce.strBdEcomerce);

                DbCommand command = database.GetStoredProcCommand("web_EC_actualizar_area_servicio");


                database.AddInParameter(command, "@UserTemp", DbType.Guid, user.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, idApplication);

                //if (pasoCombo.id > 0)
                database.AddInParameter(command, "@id", DbType.Int32, areaServicio.id);
                database.AddInParameter(command, "@id_sucursal", DbType.Int32, areaServicio.id_sucursal);

                database.AddInParameter(command, "@clave_asentamiento", DbType.Int32, areaServicio.clave_asentamiento);

                //database.AddInParameter(command, "@descripcion", DbType.String, pasoCombo.descripcion);

                database.AddInParameter(command, "@descripcion_asentamiento", DbType.String, areaServicio.descripcion_asentamiento.Trim());

                database.AddInParameter(command, "@codigo_postal", DbType.String, areaServicio.codigo_postal);


                //database.AddInParameter(command, "@secuencia", DbType.Int32, pasoCombo.secuencia);

                var date = DateTime.Now;
                database.AddInParameter(command, "@modificado_por", DbType.String, user.ClaveUsuario);
                database.AddInParameter(command, "@fecha_modificacion", DbType.DateTime, date);



                if (delete)
                    database.AddInParameter(command, "@eliminar", DbType.Int32, 1);


                //database.AddInParameter(command, "@id_pasos_combos", DbType.String, id_pasos_combos);


                var dataset = database.ExecuteScalar(command);

                /*var value = int.Parse(dataset.ToString());


                if (pasoCombo.id <= 0)
                    foreach (var productoCombo in pasoCombo.ProductosCombos)
                    {
                        productoCombo.id_pasos_combos = value;

                        UpdateProductoCombo(productoCombo, user, idApp);


                    }*/

            }
            catch (Exception ex)
            {
                Loguear.Error(ex, user.ClaveUsuario);
                throw ex;
                //throw new CAppException(8010, ex.Message, ex);
            }

            //UpdatePasoCombo(pasoCombo, user, idApp);

        

            

        }

        public static void EditSucursalSub(Sucursal sucursal, Usuario elUsuario, Guid appId)
        {
            try
            {

                var date = DateTime.Now;
                SqlDatabase database = new SqlDatabase(BdEcommerce.strBdEcomerce);
                DbCommand command = database.GetStoredProcCommand("web_EC_editar_sucursal_sustituta");

                /*database.AddInParameter(command, "@FechaInicial", DbType.DateTime, fechaIni);
                database.AddInParameter(command, "@FechaFinal", DbType.DateTime, fechaFin);*/
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, appId);
                database.AddInParameter(command, "@id_sucursal", DbType.Int32, sucursal.id_sucursal);
                database.AddInParameter(command, "@id_suc_sustituta", DbType.Int32, sucursal.id_suc_sustituta);



                database.AddInParameter(command, "@modificado_por", DbType.String, elUsuario.ClaveUsuario);
                database.AddInParameter(command, "@fecha_modificacion", DbType.DateTime, date);
                //database.AddInParameter(command, "@Insertado_por", DbType.String, elUsuario.ClaveUsuario);
                //database.AddInParameter(command, "@fecha_Insertado", DbType.DateTime, date);

                



                database.ExecuteNonQuery(command);

                //var dataset = database.ExecuteDataSet(command);
                //var dataList = dataset.Tables[0].DataTableToList<Sucursal>();
                //vat test = (DataTable)dataset;
                
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw ex;
                //throw new CAppException(8010, ex.Message, ex);
            }
        }
    }
}
