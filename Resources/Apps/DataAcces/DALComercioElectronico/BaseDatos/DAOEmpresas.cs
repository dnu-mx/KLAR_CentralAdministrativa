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
    public class DaoEmpresas
    {

        public static List<Empresa> GetEmpresas(Usuario elUsuario, Guid AppId)
        {
            
            try
            {
                SqlDatabase database = new SqlDatabase(BdEcommerce.strBdEcomerce);
                DbCommand command = database.GetStoredProcCommand("web_EC_empresas_lista");

                
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppId);

                var dataset = database.ExecuteDataSet(command);
                var dataList = dataset.Tables[0].DataTableToList<Empresa>();
                //vat test = (DataTable)dataset;
                return dataList;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }

        public static int  AddEmpresa(Empresa empresa, Usuario elUsuario, Guid AppId)
        {
            try
            {

                var date = DateTime.Now;
                SqlDatabase database = new SqlDatabase(BdEcommerce.strBdEcomerce);
                DbCommand command = database.GetStoredProcCommand("web_EC_empresas_agregar");

                
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppId);

                
                database.AddInParameter(command, "@ClaveEmpresa", DbType.String, empresa.ClaveEmpresa);
                database.AddInParameter(command, "@RazonSocial", DbType.String, empresa.RazonSocial);
                database.AddInParameter(command, "@NombreComercial", DbType.String, empresa.NombreComercial);



                database.AddInParameter(command, "@AsociarCorreo", DbType.Boolean, empresa.AsociarCorreo);

                database.AddInParameter(command, "@DominiosCorreo", DbType.String, empresa.DominiosCorreo);

                
           
           

                database.AddInParameter(command, "@modificado_por", DbType.String, elUsuario.ClaveUsuario);
                database.AddInParameter(command, "@fecha_modificacion", DbType.DateTime, date);
                database.AddInParameter(command, "@Insertado_por", DbType.String, elUsuario.ClaveUsuario);
                database.AddInParameter(command, "@fecha_Insertado", DbType.DateTime, date);

   

                database.ExecuteScalar(command);

                AddGrupoMaMoshi(empresa, elUsuario, AppId);

                //var dataset = database.ExecuteDataSet(command);
                //var dataList = dataset.Tables[0].DataTableToList<Empresa>();
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





        public static int EditEmpresa(Empresa empresa, Usuario elUsuario, Guid AppId)
        {

            try
            {

                var date = DateTime.Now;
                SqlDatabase database = new SqlDatabase(BdEcommerce.strBdEcomerce);
                DbCommand command = database.GetStoredProcCommand("web_EC_empresas_modificar");

                
                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppId);


                database.AddInParameter(command, "@ID_Empresa", DbType.String, empresa.ID_Empresa);

                database.AddInParameter(command, "@ClaveEmpresa", DbType.String, empresa.ClaveEmpresa);
                database.AddInParameter(command, "@RazonSocial", DbType.String, empresa.RazonSocial);
                database.AddInParameter(command, "@NombreComercial", DbType.String, empresa.NombreComercial);



                database.AddInParameter(command, "@AsociarCorreo", DbType.Boolean, empresa.AsociarCorreo);

                database.AddInParameter(command, "@DominiosCorreo", DbType.String, empresa.DominiosCorreo);





                database.AddInParameter(command, "@modificado_por", DbType.String, elUsuario.ClaveUsuario);
                database.AddInParameter(command, "@fecha_modificacion", DbType.DateTime, date);
                

                database.ExecuteNonQuery(command);

                UpdateGrupoMaMoshi(empresa, elUsuario, AppId);

                
                return 0;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw ex;
                //throw new CAppException(8010, ex.Message, ex);
            }
        }



        public static List<string> GetEmpresasClaves()
        {
            try
            {
                SqlDatabase database = new SqlDatabase(BdEcommerce.strBdEcomerce);
                DbCommand command = database.GetStoredProcCommand("web_EC_empresas_claves");


                //database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                //database.AddInParameter(command, "@AppId", DbType.Guid, AppId);

                var dataset = database.ExecuteDataSet(command);
                var dataList = dataset.Tables[0].DataTableToList<Empresa>();

                var list= dataList.Select(m=>m.ClaveEmpresa).ToList();

                //vat test = (DataTable)dataset;
                return list;
            }

            catch (Exception ex)
            {
                //Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }
        public static void AddGrupoMaMoshi(Empresa empresa, Usuario elUsuario, Guid AppId)
        {
            //DaoEmpresas.lel();
            try
            {
                SqlDatabase database = new SqlDatabase(BdEcommerce.strDbMoshiprod);
                DbCommand command = database.GetStoredProcCommand("web_EC_empresas_GRUPOMA_agregar");


                database.AddInParameter(command, "@ClaveEmpresa", DbType.String, empresa.ClaveEmpresa);
                database.AddInParameter(command, "@NombreComercial", DbType.String, empresa.NombreComercial);

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppId);

                var dataset = database.ExecuteNonQuery(command);

            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }


        public static void UpdateGrupoMaMoshi(Empresa empresa, Usuario elUsuario, Guid AppId)
        {
            //DaoEmpresas.lel();
            try
            {
                SqlDatabase database = new SqlDatabase(BdEcommerce.strDbMoshiprod);
                DbCommand command = database.GetStoredProcCommand("web_EC_empresas_GRUPOMA_update");


                database.AddInParameter(command, "@ClaveEmpresa", DbType.String, empresa.ClaveEmpresa);
                database.AddInParameter(command, "@NombreComercial", DbType.String, empresa.NombreComercial);

                database.AddInParameter(command, "@UserTemp", DbType.Guid, elUsuario.UsuarioTemp);
                database.AddInParameter(command, "@AppId", DbType.Guid, AppId);

                var dataset = database.ExecuteNonQuery(command);

            }

            catch (Exception ex)
            {
                Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new CAppException(8010, ex.Message, ex);
            }
        }


    }
}
