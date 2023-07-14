using DALAdministracion.BaseDatos;
using DALCentralAplicaciones.Utilidades;
using Ext.Net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Administracion
{
    public partial class Asignacion : DALCentralAplicaciones.PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!this.IsPostBack)
                {
                    initComponents();
                }
            }
            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
            }
        }

        /***********************************************************************************
         * Crea e inicializa todos los componentes de la página
         **********************************************************************************/
        private void initComponents() {
            /*Crear panel de opciones para asignación*/
            CrearPanelOpciones();
            LlenarPanelOpciones();

            /*Crear panel que muestra las asignaciones*/
            CrearPanelAsignaciones();
            LlenarPanelAsignaciones();
        }

        /***********************************************************************************
         * Crea el panel ue muestra las opciones para asignación
         **********************************************************************************/
        private void CrearPanelOpciones() { 
            /*Columna de productos*/
            GroupingSummaryColumn ID_GrupoMA = new GroupingSummaryColumn();
            ID_GrupoMA.DataIndex = "ID_GrupoMA";
            ID_GrupoMA.Hidden = true;
            this.GridProductos.ColumnModel.Columns.Add(ID_GrupoMA);

            GroupingSummaryColumn ProductoDescripcion = new GroupingSummaryColumn();
            ProductoDescripcion.DataIndex = "Descripcion";
            ProductoDescripcion.Header = "Descripción";
            this.GridProductos.ColumnModel.Columns.Add(ProductoDescripcion);

            this.GridProductos.AutoExpandColumn = "Descripcion";

            /*Columna programas*/
            GroupingSummaryColumn ID_GrupoCuenta = new GroupingSummaryColumn();
            ID_GrupoCuenta.DataIndex = "ID_GrupoCuenta";
            ID_GrupoCuenta.Hidden = true;
            this.GridProgramas.ColumnModel.Columns.Add(ID_GrupoCuenta);

            GroupingSummaryColumn ProgramaDescripcion = new GroupingSummaryColumn();
            ProgramaDescripcion.DataIndex = "Descripcion";
            ProgramaDescripcion.Header = "Descripcion";
            this.GridProgramas.ColumnModel.Columns.Add(ProgramaDescripcion);

            this.GridProgramas.AutoExpandColumn = "Descripcion";

            /*Columna tipo cuenta*/
            GroupingSummaryColumn ID_TipoCuenta = new GroupingSummaryColumn();
            ID_TipoCuenta.DataIndex = "ID_TipoCuenta";
            ID_TipoCuenta.Hidden = true;
            this.GridTiposCuenta.ColumnModel.Columns.Add(ID_TipoCuenta);

            GroupingSummaryColumn TipocuentaDescripcion = new GroupingSummaryColumn();
            TipocuentaDescripcion.DataIndex = "Descripcion";
            TipocuentaDescripcion.Header = "Descripción";
            this.GridTiposCuenta.ColumnModel.Columns.Add(TipocuentaDescripcion);

            this.GridTiposCuenta.AutoExpandColumn = "Descripcion";

            /*Columna Cadena comercial*/
            GroupingSummaryColumn CadenaDescripcion = new GroupingSummaryColumn();
            CadenaDescripcion.DataIndex = "Descripcion";
            CadenaDescripcion.Header = "Descripción";
            this.GridCadenaComercial.ColumnModel.Columns.Add(CadenaDescripcion);

            this.GridCadenaComercial.AutoExpandColumn = "Descripcion";

        }

        /***********************************************************************************
         * Llena el panel con las opciones de la base de datos
         **********************************************************************************/
        private void LlenarPanelOpciones() { 
            /*Productos*/
            this.StoreProductos.DataSource = DAOGruposMA.ListaGruposMA(
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            this.StoreProductos.DataBind();

            /*Programas*/
            this.StoreProgramas.DataSource = DAOProgramas.ListarGrupoCuentas(
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            this.StoreProgramas.DataBind();

            /*Tipo cuenta*/
            this.StoreTiposCuenta.DataSource = DAOTipoCuenta.ListarTiposCuenta(
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            this.StoreTiposCuenta.DataBind();

            /*Cadena comercial*/
            this.StoreCadenaComercial.DataSource = DAOColectivas.ListaCadenasComercial(
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            this.StoreCadenaComercial.DataBind();
        }

        /***********************************************************************************
         * Crea el panel que visualiza las asignaciones
         **********************************************************************************/
        private void CrearPanelAsignaciones() {
            GroupingSummaryColumn ID_GrupoMA = new GroupingSummaryColumn();
            ID_GrupoMA.DataIndex = "ID_GrupoMA";
            ID_GrupoMA.Hidden = true;
            this.GridAsignaciones.ColumnModel.Columns.Add(ID_GrupoMA);

            GroupingSummaryColumn ProductoDescripcion = new GroupingSummaryColumn();
            ProductoDescripcion.DataIndex = "ProductoDescripcion";
            ProductoDescripcion.Header = "Producto";
            ProductoDescripcion.Width = 200;
            this.GridAsignaciones.ColumnModel.Columns.Add(ProductoDescripcion);

            GroupingSummaryColumn ID_GrupoCuenta = new GroupingSummaryColumn();
            ID_GrupoCuenta.DataIndex = "ID_GrupoCuenta";
            ID_GrupoCuenta.Hidden = true;
            this.GridAsignaciones.ColumnModel.Columns.Add(ID_GrupoCuenta);

            GroupingSummaryColumn ProgramaDescripcion = new GroupingSummaryColumn();
            ProgramaDescripcion.DataIndex = "ProgramaDescripcion";
            ProgramaDescripcion.Header = "Programa";
            ProgramaDescripcion.Width = 200;
            this.GridAsignaciones.ColumnModel.Columns.Add(ProgramaDescripcion);

            GroupingSummaryColumn ID_TipoCuenta = new GroupingSummaryColumn();
            ID_TipoCuenta.DataIndex = "ID_TipoCuenta";
            ID_TipoCuenta.Hidden = true;
            this.GridAsignaciones.ColumnModel.Columns.Add(ID_TipoCuenta);

            GroupingSummaryColumn TipocuentaDescripcion = new GroupingSummaryColumn();
            TipocuentaDescripcion.DataIndex = "TipoCuentaDescripcion";
            TipocuentaDescripcion.Header = "Tipo de Cuenta";
            TipocuentaDescripcion.Width = 200;
            this.GridAsignaciones.ColumnModel.Columns.Add(TipocuentaDescripcion);

            GroupingSummaryColumn CadenaDescripcion = new GroupingSummaryColumn();
            CadenaDescripcion.DataIndex = "CadenaDescripcion";
            CadenaDescripcion.Header = "Cadena Comercial";
            this.GridAsignaciones.ColumnModel.Columns.Add(CadenaDescripcion);

            this.GridAsignaciones.Title = "Lista de Asignaciones";
            this.GridAsignaciones.AutoExpandColumn = "CadenaDescripcion";
            
        }

        /***********************************************************************************
         * Llena el panel con las asignaciones de la base de datos
         **********************************************************************************/
        private void LlenarPanelAsignaciones() {
            this.StoreProductos.DataSource = DAOGruposMA.ListaGruposMA(
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            this.StoreProductos.DataBind();
        }
    }
}