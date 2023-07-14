using DALAutorizador.BaseDatos;
using DALAutorizador.LogicaNegocio;
using DALCentralAplicaciones;
using DALEventos.LogicaNegocio;
using Ext.Net;
using Interfases.Entidades;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;

namespace Cortador
{
    public partial class EjecutarEventoManual : PaginaBaseCAPP
    {
        #region Variables privadas

        //LOG HEADER EVENTO MANUAL
        private LogHeader LH_EventoManual = new LogHeader();

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_EventoManual.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_EventoManual.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_EventoManual.User = this.Usuario.ClaveUsuario;
            LH_EventoManual.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_EventoManual);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA EjecutarEventoManual Page_Load()");

                if (!IsPostBack)
                {
                    llenarEventosNoTrx();
                }

                log.Info("TERMINA EjecutarEventoManual Page_Load()");
            }

            catch (Exception err)
            {
                log.ErrorException(err);
                errRedirect = "../ErrorInicializarPagina.aspx";
            }

            finally
            {
                if (!string.IsNullOrEmpty(errRedirect))
                {
                    Response.Redirect(errRedirect, false);

                    Response.Flush();
                    Response.Close();
                }
            }
        }

        private void llenarEventosNoTrx()
        {
            LogPCI unLog = new LogPCI(LH_EventoManual);
            int i = 0;

            try
            {
                unLog.Info("INICIA ListaEventosManuales()");
                DataSet dsEvMan = DAOCatalogos.ListaEventosManuales(this.Usuario, 
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), LH_EventoManual);
                unLog.Info("TERMINA ListaEventosManuales()");

                object[] listaEventos = new object[dsEvMan.Tables[0].Rows.Count];
                i = 0;

                foreach (DataRow evento in dsEvMan.Tables[0].Rows)
                {
                    listaEventos[i] = new object[] { evento["ID_Evento"].ToString(),
                        evento["Clave"].ToString(),
                        evento["Descripcion"].ToString() };
                    i++;
                }

                this.stEventosManuales.DataSource = listaEventos;
                this.stEventosManuales.DataBind();

                unLog.Info("INICIA ListaContratos()");
                DataSet dsContratos = DAOCatalogos.ListaContratos(this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), LH_EventoManual);
                unLog.Info("INICIA ListaContratos()");

                object[] listaContratos = new object[dsContratos.Tables[0].Rows.Count];
                i = 0;

                foreach (DataRow contrato in dsContratos.Tables[0].Rows)
                {
                    listaContratos[i] = new object[] { contrato["ID_Contrato"].ToString(),
                        contrato["CadenaComercial"].ToString(),
                        contrato["Descripcion"].ToString() };
                    i++;
                }

                this.stContratos.DataSource = listaContratos;
                this.stContratos.DataBind();
            }
            catch (Exception err)
            {
                unLog.ErrorException(err);
                X.Msg.Alert("Consulta de Movimientos", "Error al consultar los eventos.").Show();
            }
        }

        protected void PreparaGripPropiedades()
        {
            LogPCI unLog = new LogPCI(LH_EventoManual);

            try
            {
                PropertyGridParameterCollection source = new PropertyGridParameterCollection();
                // 0000000000

                this.Propiedades.SetSource(source);
                DataTable laTabla = new DataTable();

                unLog.Info("INICIA GetParametrosFaltantesEvento()");
                List<Parametro> propiedades =
                    LNValores.GetParametrosFaltantesEvento(Int64.Parse(cmbEventos.Value.ToString()),
                    Int64.Parse(cmbContratos.Value.ToString()), this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), LH_EventoManual);
                unLog.Info("TERMINA GetParametrosFaltantesEvento()");

                foreach (Parametro unaProp in propiedades)
                {
                    if (unaProp.EsPolimorfico)
                    {
                        PropertyGridParameter GridProp = new PropertyGridParameter(unaProp.Nombre, "(Teclea el Identificador de " + unaProp.Nombre.Substring(1) + ")");
                        GridProp.DisplayName = unaProp.Nombre;
                        Propiedades.AddProperty(GridProp);
                    }
                    else
                    {
                        if (unaProp.ID_TipoColectiva != 0)
                        {
                            //significa que solicita una colectiva
                            ComboBox comboBox = new ComboBox { EmptyText = "Selecciona una Opción...", AllowBlank = false };

                            unLog.Info("INICIA ListaColectivasPorIDTipoColectiva()");
                            laTabla = DAOColectiva.ListaColectivasPorIDTipoColectiva(unaProp.ID_TipoColectiva, 
                                Int64.Parse(cmbContratos.Value.ToString()), this.Usuario, 
                                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                                LH_EventoManual).Tables[0];
                            unLog.Info("TERMINA ListaColectivasPorIDTipoColectiva()");

                            int k;
                            for (k = 0; k < laTabla.Rows.Count; k++)
                            {
                                ListItem unItem = new ListItem(laTabla.Rows[k]["Nombre"].ToString().Trim(), 
                                    laTabla.Rows[k]["ID_Colectiva"].ToString());
                                comboBox.Items.Add(unItem);

                            }

                            PropertyGridParameter GridProp = new PropertyGridParameter(unaProp.Nombre, "(Selecciona la Colectiva " + unaProp.Nombre.Substring(1) + ")");
                            GridProp.DisplayName = unaProp.Nombre;

                            if (k == 0)
                            {
                                //selecciona por defaul si solo hay un registro
                                GridProp = new PropertyGridParameter(unaProp.Nombre, "(NO EXISTEN COLECTIVAS EN DB)");
                            }

                            GridProp.Editor.Add(comboBox);
                            Propiedades.AddProperty(GridProp);
                        }
                        else
                        {
                            PropertyGridParameter GridProp = new PropertyGridParameter(unaProp.Nombre, "(Introduce un valor Válido)");
                            GridProp.DisplayName = unaProp.Nombre;

                            if (unaProp.Nombre == "@Importe")
                            {
                                TextField txtImporte = new TextField();
                                txtImporte.MaskRe = @"/[0-9\.]/";
                                GridProp.Editor.Add(txtImporte);
                            }

                            Propiedades.AddProperty(GridProp);
                        }
                    }
                }
            }
            catch(CAppException caEx)
            {
                X.Msg.Alert("Contratos", caEx.Mensaje() + " Cod.(" + caEx.CodigoError().ToString() + ")").Show();
            }
            catch (Exception err)
            {
                unLog.WarnException(err);
                X.Msg.Alert("Contratos", "Error al establecer los valores de los contratos.").Show();
            }
        }


        protected void refreshData(object sender, StoreRefreshDataEventArgs e)
        {
            llenarEventosNoTrx();
        }

        protected void Contrato_Select(object sender, DirectEventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_EventoManual);

            try
            {
                pCI.Info("INICIA ObtieneValoresContrato()");
                this.stGridVariables.DataSource = DAOValores.ObtieneValoresContrato(
                    Int64.Parse(cmbContratos.Value.ToString()), this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_EventoManual);
                pCI.Info("TERMINA ObtieneValoresContrato()");

                this.stGridVariables.DataBind();

                Propiedades.Items.Clear();
                PreparaGripPropiedades();
            }
            
            catch(CAppException caEx)
            {
                X.Msg.Alert("Contratos", caEx.Mensaje() + " Cod.(" + caEx.CodigoError().ToString() + ")").Show();
            }

            catch (Exception Error)
            {
                pCI.WarnException(Error);
                X.Msg.Alert("Contratos", "Error al obtener los valores  de contrato.").Show();
            }
        }

        protected void LanzarEvento_Click(object sender, DirectEventArgs e)
        {
            LogPCI unLog = new LogPCI(LH_EventoManual);
            int respuesta = -1;

            try
            {
                Dictionary<string, Parametro> losParame = new Dictionary<string, Parametro>();

                unLog.Info("INICIA GetParametrosParaEjecutor()");
                List<Parametro> losParametros = LNValores.GetParametrosParaEjecutor(
                    Int64.Parse(cmbEventos.Value.ToString()), Int64.Parse(cmbContratos.Value.ToString()),
                    this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_EventoManual);
                unLog.Info("TERMINA GetParametrosParaEjecutor()");

                //se crea el Dictionary.
                foreach (Parametro unaProp in losParametros)
                {
                    if (!losParame.ContainsKey(unaProp.Nombre))
                    {
                        losParame.Add(unaProp.Nombre, unaProp);
                    }
                }

                foreach (PropertyGridParameter param in this.Propiedades.Source)
                {
                    if ((param.Value.ToString().Trim().Length == 0) || (param.Value.ToString().Contains('(')) || (param.Value.ToString().Contains(')')))
                    {
                        X.Msg.Alert("Eventos Manuales", "El Parámetro [<b>" + param.Name + "</b>] es obligatorio.").Show();
                        return;
                    }

                    Parametro unParam = losParame[param.Name];

                    if (unParam.Nombre == "@Importe")
                    {
                        if (param.Value.ToString().Contains("-"))
                        {
                            X.Msg.Alert("Eventos Manuales", "El importe no puede ser negativo. Favor de verificar.").Show();
                            return;
                        }
                    }

                    unParam.Valor = unParam.Nombre == "@Importe" ? param.Value.ToString().Replace(",", "") : param.Value.ToString();
                    losParame[param.Name] = unParam;
                }

                String DescripcionPoliza = "EJECUCIÓN MANUAL: " + cmbEventos.SelectedItem.Text + " Por [" + this.Usuario.Email + "]";

                Int64 refnum;

                if (txtRefenreciaNumerica.Text.Trim().Length != 0)
                {
                    if (!Int64.TryParse(txtRefenreciaNumerica.Text, out refnum))
                    {
                        X.Msg.Alert("Eventos Manuales", "El Valor [<b>" + txtRefenreciaNumerica.Text + "</b>] no es númerico").Show();
                        return;
                    }
                }
                else
                {
                    refnum = 0;
                }

                unLog.Info("INICIA Ejecutar()");
                respuesta = LNEvento.Ejecutar(int.Parse(cmbEventos.Value.ToString()),
                    int.Parse(cmbContratos.Value.ToString()), DescripcionPoliza, losParame, this.Usuario, 
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), TxtObservaciones.Text,
                    refnum, LH_EventoManual);
                unLog.Info("TERMINA Ejecutar()");

                if (respuesta != 0)
                {
                    X.Msg.Notify("Ejecución Manual", "No se ejecutó el Evento. </br> Código de Respuesta:<b> [" + respuesta + "]</b>").Show();
                    return;
                }
                else
                {
                    //limpieza del formulario.
                    limpiarForm();

                    X.Msg.Notify("Ejecución Manual", "Resultados de la Operación <br /> <b>" + cmbEventos.SelectedItem.Text + "</b> " + "<br/> Ejecutor: <b> " + this.Usuario.Email + "</b>").Show();
                    X.Msg.Notify("Ejecución Manual", "Ejecución de Proceso Manual <br />  <br /> <b>  A U T O R I Z A D O  </b> <br />  <br /> ").Show();
                }

            }
            catch (CAppException ex)
            {
                X.Msg.Alert("Ejecución Manual", ex.Mensaje() + " Cod.(" + ex.CodigoError().ToString() + ")").Show();
            }
            catch (Exception Error)
            {
                unLog.ErrorException(Error);
                X.Msg.Notify("Ejecución Manual", "Ejecución del Evento:<br /> <b>" + cmbEventos.SelectedItem.Text + "</b> ").Show();
                X.Msg.Notify("Ejecución Manual", "Ejecución de Proceso Manual <br />  <br /> <b>  D E C L I N A D O  </b> <br /> " + respuesta + "  <br /> ").Show();
            }
        }

        protected void limpiarForm()
        {
            LogPCI log = new LogPCI(LH_EventoManual);

            try
            {
                log.Info("INICIA limpiarForm()");
                PropertyGridParameterCollection source = new PropertyGridParameterCollection();
                this.Propiedades.SetSource(source);

                stGridVariables.RemoveAll();
                FormPanel1.Reset();
                log.Info("TERMINA limpiarForm()");
            }
            catch (Exception err)
            {
                log.WarnException(err);
            }
        }
    }
}