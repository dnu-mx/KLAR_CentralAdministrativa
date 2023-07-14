using Administracion.Componentes;
using DALAdministracion.BaseDatos;
using DALAutorizador.Utilidades;
using Ext.Net;
using System;
using System.Configuration;

namespace Administracion
{
    public partial class ConsultarPrograma : DALCentralAplicaciones.PaginaBaseCAPP
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
            /*programas*/
            CrearGridProgramas();
            LlenarGridProgramas();

        }

        /***************************************************************************************
         * Realiza la creación del componente grid para los programas
         **************************************************************************************/
        private void CrearGridProgramas()
        {
            GridProgramas = ProgramaComponente.CrearGridPanelBase(GridProgramas, false);
        }

        /***************************************************************************************
         * Llena el gris de productos con la información de la base de datos
         **************************************************************************************/
        private void LlenarGridProgramas()
        {
            StoreProgramas = ProgramaComponente.CrearStore(StoreProgramas);
            StoreProgramas.DataSource = DAOProgramas.ListarGrupoCuentas(
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                );

            StoreProgramas.DataBind();
        }

        /***********************************************************************************
         * Actualiza el panel con los datos de la base de datos
         **********************************************************************************/
        protected void StoreProgramas_Refresh(object sender, StoreRefreshDataEventArgs e)
        {
            LlenarGridProgramas();
        }

    }
}