using Administracion.Componentes;
using DALAdministracion.BaseDatos;
using DALAutorizador.Utilidades;
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
    public partial class ConsultarTipoCuenta : DALCentralAplicaciones.PaginaBaseCAPP
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

        private void initComponents()
        {
            /* Tipos de cuenta */
            CrearGridTiposCuenta();
            LlenarGridTiposCuenta();
        }

        /***************************************************************************************
         * Realiza la creación del componente grid para los productos
         **************************************************************************************/
        private void CrearGridTiposCuenta()
        {
            GridTiposCuenta = TiposCuentaComponent.CrearGridPanelBase(GridTiposCuenta, false);
        }

        /***************************************************************************************
         * Llena el gris de productos con la información de la base de datos
         **************************************************************************************/
        private void LlenarGridTiposCuenta()
        {
            this.StoreTiposCuenta.DataSource = DAOTipoCuenta.ListarTiposCuenta(
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            this.StoreTiposCuenta.DataBind();
        }

        /***************************************************************************************
         * Controla la acción de atualizar los datos del gri productos
         **************************************************************************************/
        protected void StoreTiposCuenta_Refresh(object sender, StoreRefreshDataEventArgs e)
        {
            LlenarGridTiposCuenta();
        }
    }
}