using DALCentralAplicaciones;
using DALCentralAplicaciones.BaseDatos;
using DALLealtad.BaseDatos;
using DALLealtad.Entidades;
using DALLealtad.LogicaNegocio;
using DALLealtad.Utilidades;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web;

namespace Lealtad
{
    public partial class AdministrarSubGirosPrana : PaginaBaseCAPP
    {
        /// <summary>
        /// Carga de la página Administrar la configuración de objetos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    StoreGiros.DataSource = DAOEcommercePrana.ListaGiros(this.Usuario);
                    StoreGiros.DataBind();

                    cBoxGiro.SelectedIndex = 0;
                }
            }

            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LlenaGridResultados();
        }

        /// <summary>
        /// Controla el evento Click al botón Guardar del panel para Editar y Agregar un Subgiro
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnAddSubGiro_Click(object sender, EventArgs e)
        {
            try
            {
                //string ArchivoFTP = CargaArchivoFTP(0);
                AgregarSubGiro();
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Actualización de SubGiro", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Actualización de SubGiro", ex.Message).Show();
            }
        }

        private void LlenaGridResultados()
        {
            try
            {
                DataSet dsObjetos = DAOEcommercePrana.ListaSubGiros(this.Usuario, Convert.ToInt32(this.cBoxGiro.SelectedItem.Value));

                if (dsObjetos.Tables[0].Rows.Count == 0)
                {
                    StoreSubGiros.RemoveAll();
                    X.Msg.Alert("SubGiros", "No existen coincidencias con los datos solicitados").Show();
                }
                else
                {
                    StoreSubGiros.DataSource = dsObjetos;
                    StoreSubGiros.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("SubGiros", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("SubGiros", ex.Message).Show();
            }
        }

        private void AgregarSubGiro()
        {
            SubGiro objetoSubGiro = new SubGiro();

            objetoSubGiro.Clave = txtClave.Text;
            objetoSubGiro.Descripcion = txtDescripcion.Text;
            objetoSubGiro.Id_Giro = Convert.ToInt32(cBoxGiro.Value);

            DataTable dtNuevoObjeto = LNEcommercePrana.CreaNuevoSubGiro(objetoSubGiro, this.Usuario);

            string msj = dtNuevoObjeto.Rows[0]["Mensaje"].ToString();
            int idNuevoSubgiro = Convert.ToInt32(dtNuevoObjeto.Rows[0]["IdNuevoSubGiro"]);

            if (idNuevoSubgiro == -1)
            {
                X.Msg.Alert("Nuevo SubGiro", msj).Show();
            }
            else
            {
                Modal_AltaSubGiro.Hide();
                LlenaGridResultados();

                X.Msg.Notify("", "Nuevo SubGiro Agregado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
            }
        }

        protected void btnEditaSubGiro_Click(object sender, EventArgs e)
        {
            try
            {
                SubGiro objetoSubGiro = new SubGiro();

                objetoSubGiro.Id_SubGiro = Convert.ToInt32(hdnIdSubGiro.Text);
                objetoSubGiro.Clave = txtClave_Update.Text;
                objetoSubGiro.Descripcion = txtDescripcion_Update.Text;

                DataTable dtObjetoMod =  LNEcommercePrana.ModificaSubGiro(objetoSubGiro, this.Usuario);
                string msj = dtObjetoMod.Rows[0]["Mensaje"].ToString();

                if (msj != "")
                {
                    Modal_EditaSubGiro.Hide();
                    X.Msg.Alert("Actualización de SubGiro", msj).Show();
                }
                else
                {
                    LlenaGridResultados();
                    Modal_EditaSubGiro.Hide();
                    X.Msg.Notify("", "SubGiro Actualizado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
                }
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Actualización de SubGiro", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Actualización de SubGiro", ex.Message).Show();
            }
        }

        [DirectMethod]
        public void Editar_Event(int ID_SubGiro, string Clave, string Descripcion)
        {
            lblIDSubGiro.Text = ID_SubGiro.ToString();
            hdnIdSubGiro.Text = ID_SubGiro.ToString();
            txtClave_Update.Text = Clave;
            txtDescripcion_Update.Text = Descripcion;

            Modal_EditaSubGiro.Show();
        }

        //[DirectMethod]
        //public void Eliminar_Event(string id)
        //{
        //    X.Msg.Confirm("Eliminar Objeto", "¿Esta seguro que desea eliminar el Objeto?",
        //        new MessageBoxButtonsConfig
        //        {
        //            Yes = new MessageBoxButtonConfig
        //            {
        //                Handler = "AdminObjeto.Elimina("+id+")",
        //                Text = "Si"
        //            },
        //            No = new MessageBoxButtonConfig
        //            {
        //                Handler = "",
        //                Text = "No"
        //            }
        //        }).Show();
        //}

        /// <summary>
        /// Controla el evento clic al botón Eliminar
        /// </summary>
        //[DirectMethod(Namespace = "AdminObjeto")]
        //public void Elimina(string id)
        //{
        //    ObjetoPrograma objetoPrograma = new ObjetoPrograma();
        //    objetoPrograma.ID_ProgramaObjeto = Convert.ToInt32(id);
        //    objetoPrograma.Activo = 0;
        //    objetoPrograma.ID_Entidad = 0;
        //    objetoPrograma.ID_Programa = 0;
        //    objetoPrograma.ID_TipoEntidad = 0;
        //    objetoPrograma.ID_TipoObjeto = 0;
        //    objetoPrograma.Orden = "";
        //    objetoPrograma.PathImagen = "";
        //    objetoPrograma.URL = "";

        //    LNEcommercePrana.ModificaObjeto(objetoPrograma, this.Usuario);
        //    LlenaGridResultados();

        //    X.Msg.Notify("", "Objeto Eliminado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
        //}
    }
}