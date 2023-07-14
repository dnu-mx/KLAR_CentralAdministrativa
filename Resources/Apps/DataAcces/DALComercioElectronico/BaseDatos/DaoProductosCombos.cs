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
    public class DaoProductosCombos
    {
        public static List<Producto> GetProductosDeCombos( Usuario elUsuario, Guid appId)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BdEcommerce.strBdEcomerce);
                DbCommand command = database.GetStoredProcCommand("web_EC_lista_productos_de_combos");

                /*database.AddInParameter(command, "@FechaInicial", DbType.DateTime, fechaIni);
                database.AddInParameter(command, "@FechaFinal", DbType.DateTime, fechaFin);*/
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, appId);

                var dataset = database.ExecuteDataSet(command);
                var dataList = dataset.Tables[0].DataTableToList<Producto>();
                //vat test = (DataTable)dataset;
                return dataList;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

   
        public static int AddProductoCombo(Producto producto, Usuario user, Guid idApplication)
        {

            try
            {

                var date = DateTime.Now;
                SqlDatabase database = new SqlDatabase(BdEcommerce.strBdEcomerce);
                DbCommand command = database.GetStoredProcCommand("web_EC_agregar_producto_combo");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, user.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, idApplication);

                //database.AddInParameter(command, "@producto_id", DbType.String, producto.producto_id);
                database.AddInParameter(command, "@sku", DbType.String, producto.sku);
                database.AddInParameter(command, "@familia_id", DbType.Int32, producto.familia_id);
                database.AddInParameter(command, "@nombre", DbType.String, producto.nombre);


                database.AddInParameter(command, "@descripcion", DbType.String, producto.descripcion);
                database.AddInParameter(command, "@path_imagen", DbType.String, producto.path_imagen);
                database.AddInParameter(command, "@activo", DbType.Boolean, producto.activo);
                database.AddInParameter(command, "@secuencia", DbType.Int32, producto.secuencia);


                database.AddInParameter(command, "@modificado_por", DbType.String, user.ClaveUsuario);
                database.AddInParameter(command, "@fecha_modificacion", DbType.DateTime, date);
                database.AddInParameter(command, "@Insertado_por", DbType.String, user.ClaveUsuario);
                database.AddInParameter(command, "@fecha_Insertado", DbType.DateTime, date);


                var value = int.Parse(database.ExecuteScalar(command).ToString());

                //var dataset = database.ExecuteDataSet(command);
                //var dataList = dataset.Tables[0].DataTableToList<Sucursal>();
                //vat test = (DataTable)dataset;
                return value;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, user.ClaveUsuario);

                throw ex;
                //throw new ex(8010, ex.Message, ex);
            }
            
        }

        public static void EditProductoCombo(Producto producto, Usuario user, Guid idApplication)
        {

            try
            {

                var date = DateTime.Now;
                SqlDatabase database = new SqlDatabase(BdEcommerce.strBdEcomerce);
                DbCommand command = database.GetStoredProcCommand("web_EC_editar_producto_combo");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, user.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, idApplication);

                database.AddInParameter(command, "@producto_id", DbType.String, producto.producto_id);
                database.AddInParameter(command, "@sku", DbType.String, producto.sku);
                database.AddInParameter(command, "@familia_id", DbType.Int32, producto.familia_id);
                database.AddInParameter(command, "@nombre", DbType.String, producto.nombre);


                database.AddInParameter(command, "@descripcion", DbType.String, producto.descripcion);
                database.AddInParameter(command, "@path_imagen", DbType.String, producto.path_imagen);
                database.AddInParameter(command, "@activo", DbType.Boolean, producto.activo);
                database.AddInParameter(command, "@secuencia", DbType.Int32, producto.secuencia);


                database.AddInParameter(command, "@modificado_por", DbType.String, user.ClaveUsuario);
                database.AddInParameter(command, "@fecha_modificacion", DbType.DateTime, date);
                //database.AddInParameter(command, "@Insertado_por", DbType.String, user.ClaveUsuario);
                //database.AddInParameter(command, "@fecha_Insertado", DbType.DateTime, date);


                //var value = int.Parse(database.ExecuteScalar(command).ToString());

                //var dataset = database.ExecuteDataSet(command);
                //var dataList = dataset.Tables[0].DataTableToList<Sucursal>();
                //vat test = (DataTable)dataset;
                //return value;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, user.ClaveUsuario);

                throw ex;
                //throw new ex(8010, ex.Message, ex);
            }

        }


        public static void UpdateProductoToCombo(Producto producto, Usuario user, Guid idApplication)
        {

            try
            {

                var date = DateTime.Now;
                SqlDatabase database = new SqlDatabase(BdEcommerce.strBdEcomerce);
                DbCommand command = database.GetStoredProcCommand("web_EC_agregar_producto_combo_parcial");

                database.AddInParameter(command, "@UserTemp", DbType.Guid, user.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, idApplication);

                database.AddInParameter(command, "@producto_id", DbType.Int32, producto.producto_id);
                database.AddInParameter(command, "@IdCombo", DbType.Int32, producto.IdCombo==0?null:producto.IdCombo);


                database.AddInParameter(command, "@modificado_por", DbType.String, user.ClaveUsuario);
                database.AddInParameter(command, "@fecha_modificacion", DbType.DateTime, date);

                database.ExecuteNonQuery(command);


                //var value = int.Parse(database.ExecuteScalar(command).ToString());

                //var dataset = database.ExecuteDataSet(command);
                //var dataList = dataset.Tables[0].DataTableToList<Sucursal>();
                //vat test = (DataTable)dataset;
                //return value;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, user.ClaveUsuario);

                throw ex;
                //throw new ex(8010, ex.Message, ex);
            }

        }


        public static List<DtoList> GetFamilias(Usuario elUsuario, Guid appId)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BdEcommerce.strBdEcomerce);
                DbCommand command = database.GetStoredProcCommand("web_EC_lista_familias");

                /*database.AddInParameter(command, "@FechaInicial", DbType.DateTime, fechaIni);
                database.AddInParameter(command, "@FechaFinal", DbType.DateTime, fechaFin);*/
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, appId);

                var dataset = database.ExecuteDataSet(command);
                var dataList = dataset.Tables[0].DataTableToList<DtoList>();

                //vat test = (DataTable)dataset;
                return dataList;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }



        public static List<DtoProduct> GetProductosSinCombo(Usuario elUsuario, Guid appId)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BdEcommerce.strBdEcomerce);
                DbCommand command = database.GetStoredProcCommand("web_EC_lista_productos");

                /*database.AddInParameter(command, "@FechaInicial", DbType.DateTime, fechaIni);
                database.AddInParameter(command, "@FechaFinal", DbType.DateTime, fechaFin);*/
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, appId);

                var dataset = database.ExecuteDataSet(command);
                var dataList = dataset.Tables[0].DataTableToList<DtoProduct>();

                //vat test = (DataTable)dataset;
                return dataList;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }


        // ReSharper disable once InconsistentNaming
        public static List<PasoCombo> GetPasosCombos(int producto_id, Usuario user, Guid appId)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BdEcommerce.strBdEcomerce);
                DbCommand command = database.GetStoredProcCommand("web_EC_obtener_pasos_combos_producto");

                
                database.AddInParameter(command, "@UserTemp", DbType.Guid, user.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, appId);
                database.AddInParameter(command, "@producto_id", DbType.String, producto_id);
                var dataset = database.ExecuteDataSet(command);

                var dataList = dataset.Tables[0].DataTableToList<PasoCombo>();


                foreach (var pasoCombo in dataList)
                {
                    pasoCombo.ProductosCombos = GetProductosCombos(pasoCombo.id, user, appId);
                }
                //vat test = (DataTable)dataset;


                return dataList;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, user.ClaveUsuario);
                throw ex;
                //throw new CAppException(8010, ex.Message, ex);
            }

        }


        // ReSharper disable once InconsistentNaming
        public static List<ProductoCombo> GetProductosCombos(int id_pasos_combos, Usuario user, Guid idApp)
        {

            try
            {
                SqlDatabase database = new SqlDatabase(BdEcommerce.strBdEcomerce);
                DbCommand command = database.GetStoredProcCommand("web_EC_obtener_productos_de_pasos");


                database.AddInParameter(command, "@UserTemp", DbType.Guid, user.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, idApp);


                database.AddInParameter(command, "@id_pasos_combos", DbType.String, id_pasos_combos);
                var dataset = database.ExecuteDataSet(command);

                var dataList = dataset.Tables[0].DataTableToList<ProductoCombo>();

                return dataList;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, user.ClaveUsuario);
                throw ex;
                //throw new CAppException(8010, ex.Message, ex);
            }

        }


        public static void UpdatePasosCombos(List<PasoCombo> pasosCombos, Usuario user, Guid idApp)
        {

            try
            {
                var idProducto=pasosCombos.First().id_producto;


                var actualData= GetPasosCombos(idProducto, user, idApp);

                foreach (var originalPasoCombo in actualData)
                {

                    var editedPasoCombo = pasosCombos.FirstOrDefault(m => m.id == originalPasoCombo.id);

                    if (editedPasoCombo == null)
                        UpdatePasoCombo(originalPasoCombo, user, idApp, true);
                    else
                    {
                        UpdatePasoCombo(editedPasoCombo, user, idApp);


                        foreach (var originalProductoCombo in originalPasoCombo.ProductosCombos)
                        {
                            var editedProductoCombo = editedPasoCombo.ProductosCombos.FirstOrDefault(m => m.id == originalProductoCombo.id);

                            if (editedProductoCombo == null) 
                                UpdateProductoCombo(originalProductoCombo, user, idApp,true);
                            else
                                UpdateProductoCombo(editedProductoCombo,user,idApp);
                            
                        }
                        var nuevosProductosCombos = editedPasoCombo.ProductosCombos.Where(m => m.id <= 0);

                        foreach (var productoCombo in nuevosProductosCombos)
                        {
                            UpdateProductoCombo(productoCombo, user, idApp);
                        }
                    }
                    
                }

                var nuevos = pasosCombos.Where(m => m.id <= 0);

                foreach (var pasoCombo in nuevos)
                {
                    UpdatePasoCombo(pasoCombo, user, idApp);
                }


                /*
                foreach (var pasosCombo in pasosCombos)
                {
                    UpdatePasoCombo(pasosCombo, user, idApp);
                }



                
                var faltantes = actualData.Where(m => !pasosCombos.Exists(z => z.id == m.id));

                foreach (var pasoCombo in faltantes)
                {
                    UpdatePasoCombo(pasoCombo, user, idApp, true);
                }
                */




                /* SqlDatabase database = new SqlDatabase(BdEcommerce.strBdEcomerce);
                 DbCommand command = database.GetStoredProcCommand("web_EC_obtener_productos_de_pasos");
 
 
                 database.AddInParameter(command, "@UserTemp", DbType.Guid, user.UsuarioTemp);
                 database.AddInParameter(command, "@AppId", DbType.Guid, idApp);
 
 
                 database.AddInParameter(command, "@id_pasos_combos", DbType.String, id_pasos_combos);
                 var dataset = database.ExecuteDataSet(command);
 
                 var dataList = dataset.Tables[0].DataTableToList<ProductoCombo>();
 
                 return dataList;*/
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, user.ClaveUsuario);
                throw ex;
                //throw new CAppException(8010, ex.Message, ex);
            }
        }

        private static  void UpdatePasoCombo(PasoCombo pasoCombo, Usuario user, Guid idApp,bool delete= false)
        {
            try
            {

                //SqlDatabase database = new SqlDatabase(BdEcommerce.strBdEcomerce);
                var database = new SqlDatabase(BdEcommerce.strBdEcomerce);

                DbCommand command = database.GetStoredProcCommand("web_EC_actualizar_pasos_combos");


                database.AddInParameter(command, "@UserTemp", DbType.Guid, user.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, idApp);

                //if (pasoCombo.id > 0)
                    database.AddInParameter(command, "@id", DbType.Int32, pasoCombo.id);
                database.AddInParameter(command, "@id_producto", DbType.Int32, pasoCombo.id_producto);

                database.AddInParameter(command, "@descripcion", DbType.String, pasoCombo.descripcion);
                database.AddInParameter(command, "@cantidad", DbType.Int32, pasoCombo.cantidad);
                database.AddInParameter(command, "@secuencia", DbType.Int32, pasoCombo.secuencia);

                var date = DateTime.Now;
                database.AddInParameter(command, "@modificado_por", DbType.String, user.ClaveUsuario);
                database.AddInParameter(command, "@fecha_modificacion", DbType.DateTime, date);



                if (delete)
                    database.AddInParameter(command, "@eliminar", DbType.Int32, 1);


                //database.AddInParameter(command, "@id_pasos_combos", DbType.String, id_pasos_combos);


                var dataset = database.ExecuteScalar(command);

                var value = int.Parse(dataset.ToString());


                if (pasoCombo.id <= 0)
                    foreach (var productoCombo in pasoCombo.ProductosCombos)
                    {
                        productoCombo.id_pasos_combos = value;

                        UpdateProductoCombo(productoCombo, user, idApp);


                    }

            }
            catch (Exception ex)
            {
                Loguear.Error(ex, user.ClaveUsuario);
                throw ex;
                //throw new CAppException(8010, ex.Message, ex);
            }
            /*
            if(!delete)
                foreach (var productoCombo in pasoCombo.ProductosCombos)
                {
                    
                    
                }*/
            
            //var dataList = dataset.Tables[0].DataTableToList<ProductoCombo>();

            //return dataList;

        }


        public static void UpdateProductoCombo(ProductoCombo productoCombo, Usuario user, Guid idApp, bool delete = false)
        {
            var database = new SqlDatabase(BdEcommerce.strBdEcomerce);

            DbCommand command = database.GetStoredProcCommand("web_EC_actualizar_productos_combos");


            database.AddInParameter(command, "@UserTemp", DbType.Guid, user.UsuarioTemp);
            database.AddInParameter(command, "@AppId", DbType.Guid, idApp);
            //if (productoCombo.id > 0)
                database.AddInParameter(command, "@id", DbType.Int32, productoCombo.id);

            database.AddInParameter(command, "@id_pasos_combos", DbType.Int32, productoCombo.id_pasos_combos);
            database.AddInParameter(command, "@id_producto", DbType.Int32, productoCombo.id_producto);
            


            var date = DateTime.Now;
            database.AddInParameter(command, "@modificado_por", DbType.String, user.ClaveUsuario);
            database.AddInParameter(command, "@fecha_modificacion", DbType.DateTime, date);


            if (delete)
                database.AddInParameter(command, "@eliminar", DbType.Int32, 1);

            database.ExecuteNonQuery(command);
        }



        //extra
        public static void UpdatePasoComboV2(PasoCombo pasoCombo, Usuario user, Guid idApp,bool delete=false)
        {

            try
            {

                //SqlDatabase database = new SqlDatabase(BdEcommerce.strBdEcomerce);
                var database = new SqlDatabase(BdEcommerce.strBdEcomerce);

                DbCommand command = database.GetStoredProcCommand("web_EC_actualizar_pasos_combos");


                database.AddInParameter(command, "@UserTemp", DbType.Guid, user.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, idApp);

                //if (pasoCombo.id > 0)
                database.AddInParameter(command, "@id", DbType.Int32, pasoCombo.id);
                database.AddInParameter(command, "@id_producto", DbType.Int32, pasoCombo.id_producto);

                database.AddInParameter(command, "@descripcion", DbType.String, pasoCombo.descripcion);
                database.AddInParameter(command, "@cantidad", DbType.Int32, pasoCombo.cantidad);
                database.AddInParameter(command, "@secuencia", DbType.Int32, pasoCombo.secuencia);

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

        public static List<Producto> GetProductos(Usuario elUsuario, Guid appId)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BdEcommerce.strBdEcomerce);
                DbCommand command = database.GetStoredProcCommand("web_EC_lista_productos_full");

                /*database.AddInParameter(command, "@FechaInicial", DbType.DateTime, fechaIni);
                database.AddInParameter(command, "@FechaFinal", DbType.DateTime, fechaFin);*/
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, appId);

                var dataset = database.ExecuteDataSet(command);
                var dataList = dataset.Tables[0].DataTableToList<Producto>();
                //vat test = (DataTable)dataset;
                return dataList;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        public static DataSet GetProductosLayout(Usuario user, Guid idApp, int idFamilia, string sku, string nombre)
        {

            try
            {
                SqlDatabase database = new SqlDatabase(BdEcommerce.strBdEcomerce);
                DbCommand command = database.GetStoredProcCommand("sp_layout_productos");

                /*database.AddInParameter(command, "@FechaInicial", DbType.DateTime, fechaIni);
                database.AddInParameter(command, "@FechaFinal", DbType.DateTime, fechaFin);*/
                //database.AddInParameter(command, "@UserTemp", DbType.Guid, user.UsuarioTemp);
                //database.AddInParameter(command, "@AppId", DbType.Guid, idApp);

                if(idFamilia>0)
                database.AddInParameter(command, "@idFamilia", DbType.Int32, idFamilia);


                database.AddInParameter(command, "@sku", DbType.String, sku);
                database.AddInParameter(command, "@nombreProducto", DbType.String, nombre);

                var dataset = database.ExecuteDataSet(command);
                //var dataList = dataset.Tables[0].DataTableToList<Producto>();
                //vat test = (DataTable)dataset;
                return dataset;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, user.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        public static object GetFamiliasV2(Usuario user, Guid idApp)
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BdEcommerce.strBdEcomerce);
                DbCommand command = database.GetStoredProcCommand("web_EC_lista_familias");

                /*database.AddInParameter(command, "@FechaInicial", DbType.DateTime, fechaIni);
                database.AddInParameter(command, "@FechaFinal", DbType.DateTime, fechaFin);*/
                database.AddInParameter(command, "@UserTemp", DbType.Guid, user.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, idApp);

                var dataset = database.ExecuteDataSet(command);
                //var dataList = dataset.Tables[0].DataTableToList<DtoList>();

                //vat test = (DataTable)dataset;
                return dataset;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, user.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }
    }
}
