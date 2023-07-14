using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALAutorizador.Utilidades;
using Ext.Net;
using System;
using System.Configuration;

namespace Empresarial
{
    public partial class GrupoComercial : DALCentralAplicaciones.PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    //Valida que el Usuario tenga permisos de Ver la pagina
                  
                }

                DespliegaDatosGrupoComercial(2);


                //SEstado.DataSource = DAOCatalogos.ListaEstados();
                //SEstado.DataBind();

                //SMunicipio.DataSource = DAOCatalogos.ListaMunicipios();
                //SMunicipio.DataBind();

                //SCiudad.DataSource = DAOCatalogos.ListaCiudades();
                //SCiudad.DataBind();

                //SAsentamiento.DataSource = DAOCatalogos.ListaAsentamientos();
                //SAsentamiento.DataBind();
            }
            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        protected void RowSelect(object sender, DirectEventArgs e)
        {
           // EastPanel.Collapsed = false;


        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
        }

        protected void RefreshGrid(object sender, StoreRefreshDataEventArgs e)
        {
        }
       

        protected void Seleccionar(object sender, DirectEventArgs e)
        {

         
        }

        protected void DespliegaDatosGrupoComercial(Int64 ID_GrupoComercial)
        {
            Colectiva elGrupoComercial = DAOColectiva.ObtenerColectiva(ID_GrupoComercial,this.Usuario , Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

            txtclave.Text = elGrupoComercial.ClaveColectiva;
            txtCURP.Text = elGrupoComercial.CURP;
            txtemail.Text = elGrupoComercial.Email;
            txtFijo.Text = elGrupoComercial.Telefono;
            txtMovil.Text = elGrupoComercial.Movil;
            txtNombre.Text = elGrupoComercial.NombreORazonSocial;
            txtRFC.Text = elGrupoComercial.RFC;

            UtxtCalle.Text = elGrupoComercial.DUbicacion.Calle;
            UtxtEntreCalles.Text = elGrupoComercial.DUbicacion.EntreCalles;
            UtxtExterior.Text = elGrupoComercial.DUbicacion.NumExterior;
            Utxtinterior.Text = elGrupoComercial.DUbicacion.NumInterior;
            UtxtReferencias.Text = elGrupoComercial.DUbicacion.Referencias;
            UcmbAsentamiento.Text = elGrupoComercial.DUbicacion.Asentamiento.DesAsentamiento;
            UtxtCodigoPostal.Text = elGrupoComercial.DUbicacion.Asentamiento.CodigoPostal;
            UcmbCiudad.Text = elGrupoComercial.DUbicacion.Asentamiento.LaCiudad.DesCiudad;
            UcmbEstado.Text = elGrupoComercial.DUbicacion.Asentamiento.ElEstado.Descripcion;
            UcmbMunicipios.Text = elGrupoComercial.DUbicacion.Asentamiento.ElMunicipio.DesMunicipio;

            FtxtCalle.Text = elGrupoComercial.DFacturacion.Calle;
            FtxtEntreCalle.Text = elGrupoComercial.DFacturacion.EntreCalles;
            FtxtExterior.Text = elGrupoComercial.DFacturacion.NumExterior;
            FtxtInterior.Text = elGrupoComercial.DFacturacion.NumInterior;
            FtxtReferencia.Text = elGrupoComercial.DFacturacion.Referencias;
            FcmbAsentamiento.Text = elGrupoComercial.DFacturacion.Asentamiento.DesAsentamiento;
            FtxtCodigoPostal.Text = elGrupoComercial.DFacturacion.Asentamiento.CodigoPostal;
            FcmbCiudad.Text = elGrupoComercial.DFacturacion.Asentamiento.LaCiudad.DesCiudad;
            fcmbEstado.Text = elGrupoComercial.DFacturacion.Asentamiento.ElEstado.Descripcion;
            FcmbMunicipio.Text = elGrupoComercial.DFacturacion.Asentamiento.ElMunicipio.DesMunicipio;




        }
    }
}