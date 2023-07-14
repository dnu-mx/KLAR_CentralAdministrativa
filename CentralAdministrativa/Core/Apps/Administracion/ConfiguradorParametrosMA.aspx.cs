using DALAdministracion.BaseDatos;
using DALAdministracion.LogicaNegocio;
using DALAutorizador.Entidades;
using DALCentralAplicaciones.Utilidades;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace Administracion
{
    public partial class ConfiguradorParametrosMA : DALCentralAplicaciones.PaginaBaseCAPP
    {
        /// <summary>
        /// Realiza y controla la carga de la página Configurador de Tarjetas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!this.IsPostBack)
                {
                    if (!X.IsAjaxRequest)
                    {
                        DataSet dsCadenaComercial = DAOProductos.ListaCadenasComerciales(
                            this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                        List<ColectivaComboPredictivo> cadenaList = new List<ColectivaComboPredictivo>();

                        foreach (DataRow cadena in dsCadenaComercial.Tables[0].Rows)
                        {
                            var cadenaCombo = new ColectivaComboPredictivo()
                            {
                                ID_Colectiva = Convert.ToInt64(cadena["ID_Colectiva"].ToString()),
                                ClaveColectiva = cadena["ClaveColectiva"].ToString(),
                                NombreORazonSocial = cadena["NombreORazonSocial"].ToString()
                            };
                            cadenaList.Add(cadenaCombo);
                        }

                        StoreCCM.DataSource = cadenaList;
                        StoreCCM.DataBind();

                        LlenaComboProductos();

                        LlenaComboParametros();
                    }
                }
            }
            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
            }
        }

        /// <summary>
        /// Alimenta el combo de productos con el catálogo de base de datos
        /// </summary>
        protected void LlenaComboProductos()
        {
            try
            {
                StoreProductos.DataSource = DAOProductos.ListaCatalogoProductos(this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                StoreProductos.DataBind();
            }

            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Productos", "Ocurrió un error al consultar el catálogo de Productos").Show();
            }
        }

        /// <summary>
        /// Alimenta el combo de parámetros con el catálogo de base de datos
        /// </summary>
        protected void LlenaComboParametros()
        {
            try
            {
                StoreParametrosMA.DataSource = DAOProductos.ListaCatalogoParametrosMA(this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                StoreParametrosMA.DataBind();
            }

            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Parámetros", "Ocurrió un error al consultar el catálogo de Parámetros").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón de Guardar en la ventana de nuevo parámetro
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardarNuevoPMA_Click(object sender, EventArgs e)
        {
            try
            {
                LNProductos.CreaNuevoParametroMultiasignacion(txtClavePMA.Text, txtDescripcion.Text,
                    txtTipoDatoJava.Text, txtTipoDatoSQL.Text, txtValorDefault.Text, this.Usuario);

                WdwAddParam.Hide();

                LlenaComboProductos();

                X.Msg.Alert("", "Parámetro creado <br /> <br />  <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Nuevo Parámetro Multiasignación", err.Mensaje()).Show();
                Loguear.Error(err, this.Usuario.ClaveUsuario);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Nuevo Parámetro Multiasignación", "Ocurrio un Error al Crear el Parámetro Multiasignación").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LlenaGridParamsAsignados();
        }

        /// <summary>
        /// Controla el evento click al botón Limpiar, restableciendo los controles,
        /// páneles y grids a su estatus de carga inicial
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            cBoxCadenaComercial.Reset();
            cBoxProductos.Reset();

            RowSelectionModel rsm = this.GridParametrosMA.GetSelectionModel() as RowSelectionModel;
            rsm.SelectedRows.Clear();
            rsm.UpdateSelection();

            StoreParamsAsignados.RemoveAll();
            GridParamsAsignados.Disabled = true;
            btnAddParams.Disabled = true;
        }

        /// <summary>
        /// Controla el establecimiento de los datos que corresponden en el grid de
        /// de parámetros asignados a la cadena y producto
        /// </summary>
        protected void LlenaGridParamsAsignados()
        {
            try
            {
                StoreParamsAsignados.RemoveAll();

                DataSet dsParams = DAOProductos.ConsultaParametrosMA_Asignados(
                    Convert.ToInt64(cBoxCadenaComercial.SelectedItem.Value),
                    Convert.ToInt32(cBoxProductos.SelectedItem.Value), this.Usuario);

                if (dsParams == null || dsParams.Tables.Count == 0 || dsParams.Tables[0].Rows.Count == 0)
                {
                    X.Msg.Alert("Parámetros", "No existen coincidencias con la búsqueda solicitada").Show();
                }
                else
                {
                    StoreParamsAsignados.DataSource = dsParams;
                    StoreParamsAsignados.DataBind();

                    //GridParamsAsignados.Disabled = false;
                    //btnAddParams.Disabled = false;
                }
            }

            catch (Exception err)
            {
                Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Parámetros", "Ocurrió un error al consultar los Parámetros Multiasignación").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botó de Añadir Parámetros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnAddParams_Click(object sender, EventArgs e)
        {
            try
            {
                RowSelectionModel losParametros = this.GridParametrosMA.GetSelectionModel() as RowSelectionModel;

                foreach (SelectedRow parametro in losParametros.SelectedRows)
                {
                    LNProductos.AsignaParametroACadena(int.Parse(parametro.RecordID),
                        int.Parse(cBoxCadenaComercial.SelectedItem.Value),
                        int.Parse(cBoxProductos.SelectedItem.Value), this.Usuario);  
                }

                LlenaGridParamsAsignados();

                losParametros.SelectedRows.Clear();
                X.Msg.Alert("", "Parámetros añadidos <br /> <br />  <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("", "Ocurrio un Error al Añadir los Parámetros a la Cadena").Show();
            }
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid de parámetros asignados
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void EjecutarComando(object sender, DirectEventArgs e)
        {
            try
            {
                String comando = (String)e.ExtraParams["Comando"];
                hdnIdValorPMA.Text = e.ExtraParams["ID_ValorParametroMultiasignacion"];
                string ClaveParam = e.ExtraParams["Clave"];
                string DescripcionParam = e.ExtraParams["Descripcion"];
                int IdEntidad = e.ExtraParams["ID_Entidad"] == "null" ? 0 : int.Parse(e.ExtraParams["ID_Entidad"]);
                int IdRegistroEntidad = e.ExtraParams["ID_RegistroEntidad"] == "null" ? 0 : int.Parse(e.ExtraParams["ID_RegistroEntidad"]);
                string Valor = e.ExtraParams["Valor"] == "null" ? "" : e.ExtraParams["Valor"];
                
                char[] charsToTrim = { '*', '"', '/' };

                switch (comando)
                {
                    case "Edit":
                        FormPanel_WdwEditPMA.Reset();
                      
                        StoreEntidad.DataSource = DAOProductos.ListaEntidades(this.Usuario,
                            Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                        StoreEntidad.DataBind();

                        if (IdEntidad > 0)
                        {
                            cBoxEntidades.SetValue(IdEntidad);
                        }

                        if (IdRegistroEntidad > 0)
                        {
                            LlenaComboRegistrosEntidad(IdEntidad);
                            cBoxRegistrosEntidad.SetValue(IdRegistroEntidad);
                        }

                        txtValorPMA.Text = Valor.Trim(charsToTrim);

                        WdwEditPMA.Title += " - " + ClaveParam.Trim(charsToTrim) +
                            " " + DescripcionParam.Trim(charsToTrim);
                        WdwEditPMA.Show();

                        break;

                    case "Delete":
                        LNProductos.BajaValorParametroMultiasignacion(int.Parse(hdnIdValorPMA.Text), this.Usuario);

                        X.Msg.Notify("", "Parámetro Eliminado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                        LlenaGridParamsAsignados();

                        break;

                    default: break;
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Editar Parámetro Multiasignación", err.Mensaje()).Show();
                Loguear.Error(err, this.Usuario.ClaveUsuario);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Editar Parámetro Multiasignación", "Ocurrio un error al ejecutar la acción seleccionada").Show();
            }
        }

        /// <summary>
        /// Controla el evento select del combo de entidades
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void selectEntidad(object sender, EventArgs e)
        {
            LlenaComboRegistrosEntidad(int.Parse(cBoxEntidades.SelectedItem.Value));
        }

        /// <summary>
        /// Establece los datos del combo de registros entidad
        /// </summary>
        /// <param name="idEntidad">Identificador de la entidad elegida</param>
        protected void LlenaComboRegistrosEntidad(int idEntidad)
        {
            try
            {
                StoreRegEntidad.DataSource = DAOProductos.ListaRegistrosEntidad(idEntidad, this.Usuario);
                StoreRegEntidad.DataBind();
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Editar Parámetro Multiasignación", err.Mensaje()).Show();
                Loguear.Error(err, this.Usuario.ClaveUsuario);
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Editar Parámetro Multiasignación", "Ocurrió un error al obtener los Registros de la Entidad").Show();
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
            }
        }

        /// <summary>
        /// Controla el evento Click al botón de Guardar en la ventana de edición del parámetro
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardarValorPMA_Click(object sender, EventArgs e)
        {
            try
            {
                string r = LNProductos.ModificaValorParametroMultiasignacion(Convert.ToInt64(hdnIdValorPMA.Text),
                    int.Parse(cBoxEntidades.SelectedItem.Value),
                    Convert.ToInt64(cBoxRegistrosEntidad.SelectedItem.Value),
                    txtValorPMA.Text, this.Usuario);

                if (String.Compare(r, "OK") == 0)
                {
                    WdwEditPMA.Hide();

                    LlenaGridParamsAsignados();

                    X.Msg.Notify("Editar Parámetro Multiasignación", "Parámetro modificado <br /> <br />  <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
                }
                else
                {
                    X.Msg.Alert("Editar Parámetro Multiasignación", r).Show();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Editar Parámetro Multiasignación", err.Mensaje()).Show();
                Loguear.Error(err, this.Usuario.ClaveUsuario);
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Editar Parámetro Multiasignación", "Ocurrio un Error al Editar el Parámetro Multiasignación").Show();
            }
        }
    }
}