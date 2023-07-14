using DALCentralAplicaciones.Utilidades;
using DALCentroContacto.BaseDatos;
using DALCentroContacto.Entidades;
using DALCentroContacto.LogicaNegocio;
using Ext.Net;
using Framework;
using Interfases.Exceptiones;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Xsl;


namespace CentroContacto
{
    public partial class TiendasDiconsa : DALCentralAplicaciones.PaginaBaseCAPP
    {
        #region Variables privadas

        protected enum AccionBitacora
        {
            Datos = 1,
            Movimientos = 2,
            Llamada = 3
        };

        #endregion

        /// <summary>
        /// Realiza y controla la carga de la página Consulta de Clientes
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    if (ContextoInicial.ContextoServicio == null)
                    {
                        ContextoInicial.InicializarContexto();
                    }

                    //Prestablecemos las fechas de consulta de los movimientos
                    dfFechaInicialMov.SetValue(DateTime.Today.AddDays(-7));
                    dfFechaInicialMov.MaxDate = DateTime.Today;

                    dfFechaFinalMov.SetValue(DateTime.Today);
                    dfFechaFinalMov.MaxDate = DateTime.Today;

                    //Prestablecemos las fechas de consulta de los cobros con tarjeta
                    dfFechaInicialCob.SetValue(DateTime.Today.AddDays(-7));
                    dfFechaInicialCob.MaxDate = DateTime.Today;

                    dfFechaFinalCob.SetValue(DateTime.Today);
                    dfFechaFinalCob.MaxDate = DateTime.Today;

                    //Prestablecemos las fechas de consulta de los rembolsos
                    dfFechaInicialRmb.SetValue(DateTime.Today.AddDays(-7));
                    dfFechaInicialRmb.MaxDate = DateTime.Today;

                    dfFechaFinalRmb.SetValue(DateTime.Today);
                    dfFechaFinalRmb.MaxDate = DateTime.Today;

                    FormPanelDatos.Show();
                }
            }

            catch (Exception err)
            {
                DALLoyalty.Utilidades.Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        /// <summary>
        /// Llena el grid de resultado de búsqueda de clientes
        /// </summary>
        protected void LlenarGridResultados()
        {
            try
            {
                TiendaDiconsa laTienda = new TiendaDiconsa();

                laTienda.ClaveColectiva = this.txtMovil.Text;
                laTienda.ClaveAlmacen = this.txtClaveAlmacen.Text;
                laTienda.ClaveTienda = this.txtClaveTienda.Text;
                laTienda.Nombre = this.txtNombre.Text;
                laTienda.ApellidoPaterno = this.txtApPaterno.Text;
                laTienda.ApellidoMaterno = this.txtApMaterno.Text;


                DataSet dsResultados = LNTiendasDiconsa.ConsultaTiendas(laTienda, 
                        this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                limpiaBusquedaPrevia(false);

                int numRecords = dsResultados.Tables[0].Rows.Count;

                if (numRecords == 100)
                {
                    X.Msg.Alert("Consulta de Tiendas", "Demasiadas coincidencias, por favor afine su búsqueda").Show();
                }
                else if (numRecords == 0)
                {
                    X.Msg.Alert("Consulta de Tiendas", "No existen coincidencias con la búsqueda solicitada").Show();
                }

                GridResultados.GetStore().DataSource = dsResultados;
                GridResultados.GetStore().DataBind();
            }

            catch (CAppException err)
            {
                DALLoyalty.Utilidades.Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Consulta de Tiendas", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALLoyalty.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Consulta de Tiendas", "Ocurrió un Error al Ejecutar la Búsqueda con los Datos Proporcionados").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar del formulario de búsqueda, invocando la búsqueda
        /// de clientes a base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LlenarGridResultados();
        }

        /// <summary>
        /// Controla el evento Refresh en el grid de resultados, invocando nuevamente
        /// la búsqueda de clientes a base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento Refresh Data del Store Clientes</param>
        protected void StoreTiendas_Refresh(object sender, StoreRefreshDataEventArgs e)
        {
            LlenarGridResultados();
        }

        /// <summary>
        /// Limpia los controles, páneles, grids asociados a la selección previa de un cliente en
        /// el Grid de Resultados Clientes
        /// </summary>
        protected void limpiaSeleccionPrevia()
        {
            FormPanelDatos.Reset();

            FormPanelOperador.Reset();

            StoreTipoOperacion.RemoveAll();
            StoreTipoCuenta.RemoveAll();
            StoreResultadosMov.RemoveAll();
            FormPanelBuscarMov.Reset();
            dfFechaInicialMov.SetValue(DateTime.Today.AddDays(-7));
            dfFechaFinalMov.SetValue(DateTime.Today);
            GridResultadosMov.Title = "Tienda:";

            FormPanelCapturarLlamada.Reset();
            FormPanelLlamada.Title = "Tienda:";

            StoreResultadosCob.RemoveAll();
            FormPanelBuscarCobros.Reset();
            dfFechaInicialCob.SetValue(DateTime.Today.AddDays(-7));
            dfFechaFinalCob.SetValue(DateTime.Today);
            GridResultadosCob.Title = "Tienda:";

            StoreResultadosRmb.RemoveAll();
            FormPanelBuscarRmb.Reset();
            dfFechaInicialRmb.SetValue(DateTime.Today.AddDays(-7));
            dfFechaFinalRmb.SetValue(DateTime.Today);
            GridResultadosRmb.Title = "Tienda:";

            FormPanelDatos.Show();
        }

        /// <summary>
        /// Limpia los controles, páneles, grids asociados a la búsqueda previa de clientes dentro
        /// del Grid de Resultados Tiendas
        /// </summary>
        /// <param name="esBtnLimpiar">Indica si el método de origen fue el clic al botón Limpiar</param>
        protected void limpiaBusquedaPrevia(bool esBtnLimpiar)
        {
            if (esBtnLimpiar)
            {
                FormPanelBusqueda.Reset();
            }

            StoreTiendas.RemoveAll();
            FormPanelResultados.Reset();

            FormPanelDatos.Reset();

            FormPanelOperador.Reset();

            StoreTipoOperacion.RemoveAll();
            StoreTipoCuenta.RemoveAll();
            StoreResultadosMov.RemoveAll();
            FormPanelBuscarMov.Reset();
            dfFechaInicialMov.SetValue(DateTime.Today.AddDays(-7));
            dfFechaFinalMov.SetValue(DateTime.Today);
            GridResultadosMov.Title = "Tienda:";

            FormPanelCapturarLlamada.Reset();
            FormPanelLlamada.Title = "Tienda:";

            StoreResultadosCob.RemoveAll();
            FormPanelBuscarCobros.Reset();
            dfFechaInicialCob.SetValue(DateTime.Today.AddDays(-7));
            dfFechaFinalCob.SetValue(DateTime.Today);
            GridResultadosCob.Title = "Tienda:";

            StoreResultadosRmb.RemoveAll();
            FormPanelBuscarRmb.Reset();
            dfFechaInicialRmb.SetValue(DateTime.Today.AddDays(-7));
            dfFechaFinalRmb.SetValue(DateTime.Today);
            GridResultadosRmb.Title = "Tienda:";

            FormPanelDatos.Show();
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar del formulario de búsqueda, limpiando los controles,
        /// páneles y grids asociados a alguna previa
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            limpiaBusquedaPrevia(true);
        }


        /// <summary>
        /// Controla el evento de selección de una fila del grid de Resultados
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos programados del evento que se ejecutó</param>
        protected void selectRowResultados_Event(object sender, DirectEventArgs e)
        {
            try
            {
                int IdColectiva = 0;

                string json = e.ExtraParams["Values"];
                IDictionary<string, string>[] cuentaSeleccionada = JSON.Deserialize<Dictionary<string, string>[]>(json);

                foreach (KeyValuePair<string, string> column in cuentaSeleccionada[0])
                {
                    switch (column.Key)
                    {
                        case "ID_Colectiva": IdColectiva = int.Parse(column.Value); break;
                        default:
                            break;
                    }
                }

                limpiaSeleccionPrevia();

                LlenaFieldsSetsDatosYOperador(IdColectiva);
                LlenaFieldSetBuscarMov();
                LlenaFormPanelLlamada();

            }

            catch (CAppException err)
            {
                DALLoyalty.Utilidades.Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Resultados", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALLoyalty.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Resultados", "Ocurrió un Error al Seleccionar el Cliente").Show();
            }
        }

        /// <summary>
        /// Registra en bitácora la acción realizada en el formulario correspondiente
        /// </summary>
        /// <param name="Id_Cliente">Identificador del cliente</param>
        /// <param name="Formulario">AccionBitacora</param>
        protected void RegistraEnBitacora(int Id_Cliente, int Formulario)
        {
            try
            {
                string Accion;

                switch (Formulario)
                {
                    case (ushort)AccionBitacora.Datos:
                        Accion = "Actualización Datos Tienda Diconsa";
                        break;

                    case (ushort)AccionBitacora.Movimientos:
                        Accion = "Consulta de Movimientos Tienda Diconsa";
                        break;

                    default:    //case (ushort)AccionBitacora.Llamada
                        Accion = "Registro de Llamada";
                        break;
                }

                DAOTiendasDiconsa.InsertaActividad(Id_Cliente, Accion, this.Usuario);
            }

            catch (Exception ex)
            {
                DALLoyalty.Utilidades.Loguear.Evento("NO se Insertó una Acción en la Bitácora de Actividades del Autorizador. " + ex.Message, this.Usuario.ClaveUsuario);
            }
        }

       /// <summary>
        /// Consulta a BD los datos de la tienda seleccionada y los llena en el FieldSet correspondiente
       /// </summary>
       /// <param name="idcolectiva">Identificador de la tienda seleccionada</param>
        protected void LlenaFieldsSetsDatosYOperador(int idcolectiva)
        {
            try
            {
                FormPanelDatos.Reset();

                DataSet dsDatos = DAOTiendasDiconsa.ObtieneDatosTienda(
                    idcolectiva, 
                    this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                
                this.txtID_Colectiva.Text = idcolectiva.ToString();

                this.txtNombreTienda.Text       =   dsDatos.Tables[0].Rows[0]["NombreTienda"].ToString().Trim();
                this.txtClaveAlmacenTienda.Text =   dsDatos.Tables[0].Rows[0]["ClaveAlmacen"].ToString().Trim();
                this.txtClaveTienda_Tienda.Text =   dsDatos.Tables[0].Rows[0]["ClaveTienda"].ToString().Trim();
                this.txtEmailTienda.Text        =   dsDatos.Tables[0].Rows[0]["EmailTienda"].ToString().Trim();
                this.txtMovilTienda.Text        =   dsDatos.Tables[0].Rows[0]["MovilTienda"].ToString().Trim();
                this.txtLimiteCredito.Text      =   String.IsNullOrEmpty(dsDatos.Tables[0].Rows[0]["LimiteCredito"].ToString().Trim()) ? "_" : 
                                                        String.Format("{0:C}", decimal.Parse(dsDatos.Tables[0].Rows[0]["LimiteCredito"].ToString().Trim()));
                this.txtAdeudoDia.Text          =   String.IsNullOrEmpty(dsDatos.Tables[0].Rows[0]["SaldoAlDia"].ToString().Trim()) ? "_" : 
                                                        String.Format("{0:C}", decimal.Parse(dsDatos.Tables[0].Rows[0]["SaldoAlDia"].ToString().Trim()));
                this.txtSaldoDisponible.Text    =   String.Format("{0:C}", (decimal.Parse(dsDatos.Tables[0].Rows[0]["LimiteCredito"].ToString().Trim()) - 
                                                        decimal.Parse(dsDatos.Tables[0].Rows[0]["SaldoAlDia"].ToString().Trim())));
                this.dfFechaUltCorte.Value      =   String.IsNullOrEmpty(dsDatos.Tables[0].Rows[0]["FechaUltimoCorte"].ToString()) ? "_" : 
                                                        String.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(dsDatos.Tables[0].Rows[0]["FechaUltimoCorte"].ToString()));
                this.txtAdeudoUltCorte.Text     =   String.IsNullOrEmpty(dsDatos.Tables[0].Rows[0]["SaldoUltimoCorte"].ToString()) ? "_" : 
                                                        String.Format("{0:C}", decimal.Parse(dsDatos.Tables[0].Rows[0]["SaldoUltimoCorte"].ToString().Trim()));
                this.dfFechaUltRec.Value        =   String.IsNullOrEmpty(dsDatos.Tables[0].Rows[0]["FechaUltimaRecoleccion"].ToString()) ? "_" : 
                                                        String.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(dsDatos.Tables[0].Rows[0]["FechaUltimaRecoleccion"].ToString()));
                this.txtMontoUltRec.Text        =   String.IsNullOrEmpty(dsDatos.Tables[0].Rows[0]["MontoUltimaRecoleccion"].ToString()) ? "_" : 
                                                        String.Format("{0:C}", decimal.Parse(dsDatos.Tables[0].Rows[0]["MontoUltimaRecoleccion"].ToString().Trim()));

                this.txtID_Operador.Text        =   dsDatos.Tables[0].Rows[0]["IdOperador"].ToString().Trim();

                this.txtNombreOperador.Text     =   dsDatos.Tables[0].Rows[0]["NombreOperador"].ToString().Trim();
                this.txtApPaternoOperador.Text  =   dsDatos.Tables[0].Rows[0]["ApPaternoOperador"].ToString().Trim();
                this.txtApMaternoOperador.Text  =   dsDatos.Tables[0].Rows[0]["ApMaternoOperador"].ToString().Trim();
                this.txtEmailOperdor.Text       =   dsDatos.Tables[0].Rows[0]["EmailOperador"].ToString().Trim();
                if (dsDatos.Tables[0].Rows[0]["MovilOperador"].ToString().Trim() != "")
                {
                    this.nfMovilOperador.Text = dsDatos.Tables[0].Rows[0]["MovilOperador"].ToString().Trim();
                }
            }
            catch (CAppException err)
            {
                DALLoyalty.Utilidades.Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Datos del Cliente", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALLoyalty.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Datos del Cliente", "Ocurrió un Error al Consultar los Datos del Cliente").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Guardar del formulario de Operador, invocando la actualización
        /// de los datos del operador en base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardaOperador_Click(object sender, EventArgs e)
        {
            try
            {
                TiendaDiconsa tienda = new TiendaDiconsa();

                tienda.ID_Operador = Convert.ToInt32(txtID_Operador.Text);
                tienda.Email = this.txtEmailOperdor.Text;
                tienda.Movil = this.nfMovilOperador.Text;

                LNTiendasDiconsa.ActualizaDatosOperador(tienda, this.Usuario);

                X.Msg.Notify("", "Datos del Operador Actualizados Exitosamente").Show();

                //RegistraEnBitacora(tienda.ID_Operador, (int)AccionBitacora.Datos);
            }

            catch (CAppException err)
            {
                DALLoyalty.Utilidades.Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Datos del Operador", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALLoyalty.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Datos del Operador", "Ocurrió un Error al Actualizar los Datos del Operador").Show();
            }
        }

        /// <summary>
        /// Consulta en base de datos los tipos de operación y cuenta para llenar los combos del FieldSet
        /// de Movimientos.
        /// </summary>
        protected void LlenaFieldSetBuscarMov()
        {
            try
            {
                this.StoreTipoOperacion.DataSource = DAOTiendasDiconsa.ListaTiposOperacion (this.Usuario,
                                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                this.StoreTipoOperacion.DataBind();


                this.StoreTipoCuenta.DataSource =
                    DAOTiendasDiconsa.ListaTiposCuenta(this.Usuario,
                                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                this.StoreTipoCuenta.DataBind();

                this.cBoxTipoCuenta.SelectedIndex = 0;
            }

            catch (CAppException err)
            {
                DALLoyalty.Utilidades.Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Movimientos", err.Mensaje()).Show();
            }
        }

        /// <summary>
        /// Formatea la cadena título del panel de movimientos (Tienda, línea de crédito y saldo actual)
        /// </summary>
        /// <param name="credito">Monto de la línea de crédito</param>
        /// <param name="saldo">Monto del saldo</param>
        /// <returns>Cadena con formato</returns>
        protected string HeaderPanel(string credito, string saldo)
        {
            decimal cred = Convert.ToDecimal(credito);
            decimal sld = Convert.ToDecimal(saldo);

            return "Tienda: " + this.txtNombreTienda.Text +
                       "&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp" +
                       "     Línea de Crédito: " + String.Format("{0:C}", decimal.Parse(credito)) +
                       "&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp" +
                       "     Saldo Disponible: " + String.Format("{0:C}", decimal.Parse(saldo));
        }

        /// <summary>
        /// Da el formato "NNN,NNN.NN" a cada saldo de los movimientos
        /// </summary>
        /// <param name="dsPorFormatear">DataSet con los movimientos recibidos de base de datos</param>
        /// <returns>DataSet con los saldos formateados</returns>
        protected DataSet SaldoConFormato(DataSet dsPorFormatear)
        {
            string montoConPesos;
            string montoSinPesos;

            for (int counter = 0; counter < dsPorFormatear.Tables[0].Rows.Count; counter++)
            {
                montoConPesos = String.Format("{0:C}", decimal.Parse(dsPorFormatear.Tables[0].Rows[counter]["SaldoFinal"].ToString()));
                montoSinPesos = montoConPesos.Substring(1, montoConPesos.Length - 1);
                dsPorFormatear.Tables[0].Rows[counter]["SaldoFinal"] = montoSinPesos;
            }

            return dsPorFormatear;
        }

         /// <summary>
        /// Controla el evento Click al botón Buscar del formulario de movimientos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscarMov_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet dsMovimientos = new DataSet();

                dsMovimientos = LNTiendasDiconsa.ConsultaMovimientos(
                    Convert.ToDateTime(this.dfFechaInicialMov.Text),
                    Convert.ToDateTime(this.dfFechaFinalMov.Text),
                    this.cBoxTipoCuenta.SelectedItem.Value == null ? 0 : int.Parse(this.cBoxTipoCuenta.SelectedItem.Value),
                    this.cBoxTipoOperacion.SelectedItem.Value == null ? 0 : int.Parse(this.cBoxTipoOperacion.SelectedItem.Value),
                    int.Parse(this.txtID_Colectiva.Text), this.Usuario,
                Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

                if (dsMovimientos.Tables[0].Rows.Count > 0)
                {
                    string saldoActual = dsMovimientos.Tables[0].Rows[0]["SaldoActual"].ToString().Trim();
                    string limiteCredito = dsMovimientos.Tables[0].Rows[0]["LineaCredito"].ToString().Trim();
                    this.GridResultadosMov.Title = HeaderPanel(limiteCredito, saldoActual);

                    GridResultadosMov.GetStore().DataSource = SaldoConFormato(dsMovimientos);
                    GridResultadosMov.GetStore().DataBind();
                }
                else
                {
                    X.Msg.Alert("Movimientos", "No Existen Movimientos de la Tienda en el Periodo Solicitado").Show();
                }

                //RegistraEnBitacora(int.Parse(this.txtID_Colectiva.Text), (int)AccionBitacora.Movimientos);
            }

            catch (CAppException err)
            {
                DALLoyalty.Utilidades.Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Movimientos", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALLoyalty.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Movimientos", "Ocurrió un Error al Buscar los Movimientos").Show();
            }
        }

        /// <summary>
        /// Controla el evento SUBMIT al querer exportar los resultados de movimientos en Excel
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void StoreResultadosMov_Submit(object sender, StoreSubmitDataEventArgs e)
        {
            string format = this.FormatType.Value.ToString();

            XmlNode xml = e.Xml;

            this.Response.Clear();

            switch (format)
            {
                case "xls":
                    this.Response.ContentType = "application/vnd.ms-excel";
                    this.Response.AddHeader("Content-Disposition", "attachment; filename=Movimientos.xls");
                    XslCompiledTransform xtExcel = new XslCompiledTransform();
                    xtExcel.Load(Server.MapPath("xslFiles/Excel.xsl"));
                    xtExcel.Transform(xml, null, Response.OutputStream);
                    break;
            }

            this.Response.End();

        }

        /// <summary>
        /// Consulta en base de datos el cátálogo de actividades para el motivo de la llamada 
        /// para llenar el combo correspondiente dentro del FieldSetCapturarLlamada
        /// </summary>
        protected void LlenaFormPanelLlamada()
        {
            try
            {
                this.StoreMotivos.DataSource = DAOTiendasDiconsa.ListaMotivosLlamada(this.Usuario);
                this.StoreMotivos.DataBind();

                this.FormPanelLlamada.Title = "Tienda: " + this.txtNombreTienda.Text;
            }

            catch (CAppException err)
            {
                DALLoyalty.Utilidades.Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Capturar Llamada", err.Mensaje()).Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Guardar del formulario de captura de llamada,
        /// invocando la inserción del refistro correspondiente en la bitácora de base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnGuardarLlamada_Click(object sender, EventArgs e)
        {
            try
            {
                LNTiendasDiconsa.RegistraLlamadaCliente(
                    int.Parse(this.txtID_Colectiva.Text),
                    int.Parse(this.cBoxMotivoLlamada.SelectedItem.Value),
                    this.txtComentarios.Text, this.Usuario);

                X.Msg.Notify("", "La Llamada se Capturó Exitosamente").Show();

                //RegistraEnBitacora(int.Parse(this.txtID_Colectiva.Text), (int)AccionBitacora.Llamada);

                FormPanelCapturarLlamada.Reset();
            }

            catch (CAppException err)
            {
                DALLoyalty.Utilidades.Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Captura de Llamada", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALLoyalty.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Captura de Llamada", "Ocurrió un Error al Registrar la Llamada del Cliente").Show();
            }
        }

        /// <summary>
        /// Procesa la cadena JSON con las operaciones recibidas del Web Service
        /// </summary>
        /// <param name="operationsResult">JSON con las operaciones</param>
        /// <returns>DataSet con los datos requeridos para mostrar en el Grid</returns>
        protected DataSet procesaOperaciones(WSJsonResponses.GetOperationsResult operationsResult)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            int operacion;
            string moneda;

            WSJsonResponses.Operations[] records = JsonConvert.DeserializeObject<WSJsonResponses.Operations[]>(operationsResult.Operations.ToString());

            dt.Columns.Add("Transaction");
            dt.Columns.Add("Fecha");
            dt.Columns.Add("MetodoPago");
            dt.Columns.Add("Referencia");
            dt.Columns.Add("Total");
            dt.Columns.Add("Comision");

            for (operacion = 0; operacion < records.Length; operacion++)
            {
                dt.Rows.Add();

                dt.Rows[operacion]["Transaction"] = records[operacion].Transaction;

                DateTime fechaOperacion = Convert.ToDateTime(records[operacion].Timestamp);
                dt.Rows[operacion]["Fecha"] = String.Format("{0:dd/MM/yyyy}", fechaOperacion);

                WSJsonResponses.OperationsCard card = JsonConvert.DeserializeObject<WSJsonResponses.OperationsCard>(records[operacion].Card.ToString());
                dt.Rows[operacion]["MetodoPago"] = card.Type + " ****" + card.Number;

                WSJsonResponses.OperationsReference refer = JsonConvert.DeserializeObject<WSJsonResponses.OperationsReference>(records[operacion].Reference.ToString());
                dt.Rows[operacion]["Referencia"] = String.IsNullOrEmpty(refer.Description) ? "Sin referencia" : refer.Description;

                WSJsonResponses.OperationsAmmounts total = JsonConvert.DeserializeObject<WSJsonResponses.OperationsAmmounts>(records[operacion].Total.ToString());
                moneda = total.Currency == "MXN" ? "$" : total.Currency == "USD" ? "USD" : "";
                dt.Rows[operacion]["Total"] = moneda + total.Amount;

                WSJsonResponses.OperationsAmmounts comision = JsonConvert.DeserializeObject<WSJsonResponses.OperationsAmmounts>(records[operacion].Commission.ToString());
                moneda = comision.Currency == "MXN" ? "$" : comision.Currency == "USD" ? "USD" : "";
                dt.Rows[operacion]["Comision"] = moneda + comision.Amount;
            }

            ds.Tables.Add(dt);

            return ds;
        }

        /// <summary>
        /// Establece la llamada POST del Login al Web Service
        /// </summary>
        /// <returns>Token de autenticación recibido</returns>
        protected string WSLogin()
        {
            //Se genera el POST para el login
            var clientRS = new RestClient(Configuracion.Get
                (Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "WS_URL").Valor);
            var request = new RestRequest(Configuracion.Get
                (Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "WS_RequestLogin").Valor, 
                Method.POST);

            //Body
            DataSet dsCredenciales = DAOTiendasDiconsa.ObtenerCredencialesWS
                                    (Convert.ToInt32(this.txtID_Colectiva.Text),
                                    ContextoInicial.ContextoServicio.ConnectionString, this.Usuario,
                                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));

            if (dsCredenciales.Tables[0].Rows.Count == 0)
            {
                throw new CAppException(8006, "La tienda no tiene credenciales registradas para el WS.");
            }

            string user = dsCredenciales.Tables[0].Rows[0]["Usuario"].ToString().Trim();
            string pswd = dsCredenciales.Tables[0].Rows[0]["PSP"].ToString().Trim();

            request.AddParameter("username", user);
            request.AddParameter("password", pswd);

            //Headers HTTP
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("AuthorizationToken", Configuracion.Get(
                Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "WS_Token").Valor);

            //Ejecuta la petición
            IRestResponse response = clientRS.Execute(request);
            var content = response.Content;

            //Procesa la respuesta
            WSJsonResponses.Login wsLoginParameters = JsonConvert.DeserializeObject<WSJsonResponses.Login>(content);
            if (!wsLoginParameters.Success)
            {
                WSJsonResponses.Error elError = JsonConvert.DeserializeObject<WSJsonResponses.Error>(wsLoginParameters.Error.ToString());
               
                throw new Exception("No se pudo establecer conexión exitosa con el Web Service durante el Login. Tienda con ID Colectiva (" +
                    this.txtID_Colectiva.Text + "). Mensaje de error del Web Service: Código(" + elError.Code + ") " + elError.Message);
            }

            //Se valida el token obtenido
            WSJsonResponses.LoginConnection wsConnection = JsonConvert.DeserializeObject<WSJsonResponses.LoginConnection>(wsLoginParameters.Connection.ToString());
            if (String.IsNullOrEmpty(wsConnection.Token))
            {
                throw new CAppException(8006, "No se pudo obtener autenticación con el WS.");
            }

            return wsConnection.Token;
        }

        /// <summary>
        /// Establece la llamada GET de las operaciones hacia el Web Service
        /// </summary>
        /// <param name="token">Token de autenticación recibido en el login.</param>
        /// <returns>Operaciones de la tienda consultada</returns>
        protected WSJsonResponses.GetOperations WSGetOperations(string token)
        {
            //Se genera el GET con las fechas ingresadas
            using (var clientWC = new WebClient())
            {
                clientWC.Headers.Add("Content-Type", "application/json");
                clientWC.Headers.Add("AuthorizationToken", "Bearer " + token);

                StringBuilder sbUrlOperations = new StringBuilder();
                DateTime dtIni = Convert.ToDateTime(dfFechaInicialCob.Text);
                DateTime dtFin = Convert.ToDateTime(dfFechaFinalCob.Text);

                sbUrlOperations.AppendFormat("{0}{1}?",
                    Configuracion.Get(Guid.Parse
                        (ConfigurationManager.AppSettings["IDApplication"].ToString()), "WS_URL").Valor, 
                    Configuracion.Get(Guid.Parse
                        (ConfigurationManager.AppSettings["IDApplication"].ToString()), "WS_RequestOper").Valor);
                sbUrlOperations.AppendFormat("start_date={0}", String.Format("{0:yyyy-MM-dd}", dtIni));
                sbUrlOperations.AppendFormat("&end_date={0}", String.Format("{0:yyyy-MM-dd}", dtFin));
                sbUrlOperations.AppendFormat("&payment_method={0}", "POS");
                    
                var responseString = clientWC.DownloadString(sbUrlOperations.ToString());

                //Procesa la respuesta
                WSJsonResponses.GetOperations wsGetOperations = JsonConvert.DeserializeObject<WSJsonResponses.GetOperations>(responseString);
                if (!wsGetOperations.Success)
                {
                    WSJsonResponses.Error elError = JsonConvert.DeserializeObject<WSJsonResponses.Error>(wsGetOperations.Error.ToString());
                        
                    throw new Exception("No se pudo establecer conexión exitosa con el Web Service (Operations). Tienda con ID Colectiva (" +
                        this.txtID_Colectiva.Text + "). Mensaje de error del Web Service: Código("  + elError.Code + ") " + elError.Message);
                }

                return wsGetOperations;
            }
        }

        /// <summary>
        /// Controla el evento click al botón buscar del formulario de cobros con tarjeta
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscarCob_Click(object sender, EventArgs e)
        {
            try
            {
                //Login
                string token = WSLogin();

                //Obtiene operaciones
                WSJsonResponses.GetOperations opers = WSGetOperations(token);

                //Se procesan las operaciones
                WSJsonResponses.GetOperationsResult wsOperations = JsonConvert.DeserializeObject<WSJsonResponses.GetOperationsResult>(opers.Result.ToString());

                DataSet dsOperaciones = procesaOperaciones(wsOperations);

                if (dsOperaciones.Tables[0].Rows.Count > 0)
                {
                    GridResultadosCob.GetStore().DataSource = dsOperaciones;
                    GridResultadosCob.GetStore().DataBind();

                    GridResultadosCob.Title = "Tienda: " + this.txtNombreTienda.Text;
                }
                else
                {
                    X.Msg.Alert("Cobros con Tarjeta", "No Existen Cobros con Tarjeta en el Periodo Solicitado").Show();
                }

                //RegistraEnBitacora(int.Parse(this.txtID_Colectiva.Text), (int)AccionBitacora.Movimientos);
            }

            catch (CAppException err)
            {
                DALLoyalty.Utilidades.Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Cobros con Tarjeta", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALLoyalty.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Cobros con Tarjeta", "Ocurrió un Error al Buscar los Cobros con Tarjeta").Show();
            }
        }

        /// <summary>
        /// Controla el evento SUBMIT al querer exportar los resultados de cobros con tarjeta en Excel
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void StoreResultadosCob_Submit(object sender, StoreSubmitDataEventArgs e)
        {
            string format = this.FormatTypeCob.Value.ToString();

            XmlNode xml = e.Xml;

            this.Response.Clear();

            switch (format)
            {
                case "xls":
                    this.Response.ContentType = "application/vnd.ms-excel";
                    this.Response.AddHeader("Content-Disposition", "attachment; filename=Cobros con Tarjeta.xls");
                    XslCompiledTransform xtExcel = new XslCompiledTransform();
                    xtExcel.Load(Server.MapPath("xslFiles/Excel.xsl"));
                    xtExcel.Transform(xml, null, Response.OutputStream);
                    break;
            }

            this.Response.End();
        }

        /// <summary>
        /// Establece la llamada GET de los rembolsos hacia el Web Service
        /// </summary>
        /// <param name="token">Token de autenticación recibido en el login.</param>
        /// <returns>Rembolsos de la tienda consultada</returns>
        protected WSJsonResponses.GetWithdrawal WSGetWithdrawal(string token)
        {
            //Se genera el GET con las fechas ingresadas
            using (var clientWC = new WebClient())
            {
                clientWC.Headers.Add("Content-Type", "application/json");
                clientWC.Headers.Add("AuthorizationToken", "Bearer " + token);

                StringBuilder sbUrlWithdrawal = new StringBuilder();
                DateTime dtIni = Convert.ToDateTime(dfFechaInicialRmb.Text);
                DateTime dtFin = Convert.ToDateTime(dfFechaFinalRmb.Text);

                sbUrlWithdrawal.AppendFormat("{0}{1}?",
                    Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "WS_URL").Valor,
                    Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "WS_RequestWith").Valor);
                sbUrlWithdrawal.AppendFormat("start_date={0}", String.Format("{0:yyyy-MM-dd}", dtIni));
                sbUrlWithdrawal.AppendFormat("&end_date={0}", String.Format("{0:yyyy-MM-dd}", dtFin));

                var responseString = clientWC.DownloadString(sbUrlWithdrawal.ToString());

                //Procesa la respuesta
                WSJsonResponses.GetWithdrawal wsGetWithdrawal = JsonConvert.DeserializeObject<WSJsonResponses.GetWithdrawal>(responseString);
                if (!wsGetWithdrawal.Success)
                {
                    throw new CAppException(8006, "No se pudo establecer conexión exitosa con el WS (Rmb).");
                }

                return wsGetWithdrawal;
            }
        }

        /// <summary>
        /// Procesa la cadena JSON con los rembolsos recibidos del Web Service
        /// </summary>
        /// <param name="operationsResult">JSON con los rembolsos</param>
        /// <returns>DataSet con los datos requeridos para mostrar en el Grid</returns>
        protected DataSet procesaRembolsos(WSJsonResponses.GetWithdrawalResult withdrawalResult)
        {
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            int rembolso;
            string moneda;

            WSJsonResponses.WithdrawalOperations[] records = JsonConvert.DeserializeObject<WSJsonResponses.WithdrawalOperations[]>(withdrawalResult.Operations.ToString());

            dt.Columns.Add("DateRequest");
            dt.Columns.Add("Fecha");
            dt.Columns.Add("Banco");
            dt.Columns.Add("Tarjetahabiente");
            dt.Columns.Add("Importe");
            dt.Columns.Add("Comision");
            dt.Columns.Add("Estado");

            for (rembolso = 0; rembolso < records.Length; rembolso++)
            {
                dt.Rows.Add();

                dt.Rows[rembolso]["DateRequest"] = records[rembolso].Date_Request;

                DateTime fecharembolso = Convert.ToDateTime(records[rembolso].Date_Apply);
                dt.Rows[rembolso]["Fecha"] = String.Format("{0:dd/MM/yyyy}", fecharembolso);

                WSJsonResponses.BankAccount bank = JsonConvert.DeserializeObject<WSJsonResponses.BankAccount>(records[rembolso].Bank_Account.ToString());
                dt.Rows[rembolso]["Banco"] = bank.Bank_Name;
                dt.Rows[rembolso]["Tarjetahabiente"] = bank.Alias;

                WSJsonResponses.OperationsAmmounts importe = JsonConvert.DeserializeObject<WSJsonResponses.OperationsAmmounts>(records[rembolso].Total.ToString());
                moneda = importe.Currency == "MXN" ? "$" : importe.Currency == "USD" ? "USD" : "";
                dt.Rows[rembolso]["Importe"] = moneda + importe.Amount;

                WSJsonResponses.OperationsAmmounts comision = JsonConvert.DeserializeObject<WSJsonResponses.OperationsAmmounts>(records[rembolso].Commission.ToString());
                moneda = comision.Currency == "MXN" ? "$" : comision.Currency == "USD" ? "USD" : "";
                dt.Rows[rembolso]["Comision"] = moneda + comision.Amount;

                dt.Rows[rembolso]["Estado"] = records[rembolso].Status;
            }

            ds.Tables.Add(dt);

            return ds;
        }
        

        /// <summary>
        /// Controla el evento click al botón buscar del formulario de rembolsos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscarRmb_Click(object sender, EventArgs e)
        {
            try
            {
                //Login
                string token = WSLogin();

                //Obtiene los rembolsos
                WSJsonResponses.GetWithdrawal rembolsos = WSGetWithdrawal(token);

                //Se procesan las operaciones
                WSJsonResponses.GetWithdrawalResult wsWithdrawals = JsonConvert.DeserializeObject<WSJsonResponses.GetWithdrawalResult>(rembolsos.Result.ToString());
                DataSet dsRembolsos = procesaRembolsos(wsWithdrawals);

                if (dsRembolsos.Tables[0].Rows.Count > 0)
                {
                    GridResultadosRmb.GetStore().DataSource = dsRembolsos;
                    GridResultadosRmb.GetStore().DataBind();

                    GridResultadosRmb.Title = "Tienda: " + this.txtNombreTienda.Text;
                }
                else
                {
                    X.Msg.Alert("Rembolsos", "No Existen Rembolsos en el Periodo Solicitado").Show();
                }

                //RegistraEnBitacora(int.Parse(this.txtID_Colectiva.Text), (int)AccionBitacora.Movimientos);
            }

            catch (CAppException err)
            {
                DALLoyalty.Utilidades.Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Rembolsos", err.Mensaje()).Show();

            }

            catch (Exception ex)
            {
                DALLoyalty.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Rembolsos", "Ocurrió un Error al Buscar los Rembolsos").Show();
            }
        }

        /// <summary>
        /// Controla el evento SUBMIT al querer exportar los resultados de rembolsos en Excel
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void StoreResultadosRmb_Submit(object sender, StoreSubmitDataEventArgs e)
        {
            string format = this.FormatTypeRmb.Value.ToString();

            XmlNode xml = e.Xml;

            this.Response.Clear();

            switch (format)
            {
                case "xls":
                    this.Response.ContentType = "application/vnd.ms-excel";
                    this.Response.AddHeader("Content-Disposition", "attachment; filename=Rembolsos.xls");
                    XslCompiledTransform xtExcel = new XslCompiledTransform();
                    xtExcel.Load(Server.MapPath("xslFiles/Excel.xsl"));
                    xtExcel.Transform(xml, null, Response.OutputStream);
                    break;
            }

            this.Response.End();
        }
    }
}