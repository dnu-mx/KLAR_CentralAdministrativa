using ClosedXML.Excel;
using DALCentralAplicaciones;
using DALCentralAplicaciones.Utilidades;
using DALLealtad.BaseDatos;
using DALLealtad.LogicaNegocio;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Threading;
using System.Web;
using WebServices;
using WebServices.Entidades;
using static WebServices.Entidades.RespuestasJSON;

namespace Lealtad
{
    public partial class AdminCertificados_Amazon : PaginaBaseCAPP
    {
        /// <summary>
        /// Realiza y controla la carga de la página Administrar Promociones para Prana
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    HttpContext.Current.Session.Add("DtPrecalculo", null);
                    //cBoxMultiRandoEdad.Selectable = true;
                    //LlenaCombosCadena();
                    var fecha = DateTime.Now;
                    dtLote.SelectedDate = fecha.Date;

                    this.StoreMontos.DataSource = DAOCertificadosAmazon.ObtieneMontos(this.Usuario);
                    this.StoreMontos.DataBind();
                }
            }

            catch (Exception err)
            {
                DALLealtad.Utilidades.Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar del panel izquierdo, limpiando los controles,
        /// páneles y grids asociados a alguna previa
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiarIzq_Click(object sender, EventArgs e)
        {
            LimpiaVentanasAdd();

            this.dtLote.Reset();
            StoreLotes.RemoveAll();

            LimpiaSeleccionPrevia();
            PanelCentral.Disabled = true;
        }

        /// <summary>
        /// Limpia los controles de las ventanas pop up para añadir nuevas promociones o cadenas
        /// </summary>
        protected void LimpiaVentanasAdd()
        {
            FormPanelNP.Reset();
        }

        /// <summary>
        /// Limpia los controles, páneles, grids asociados a la selección previa de
        /// una promoción en el Grid de Resultados
        /// </summary>
        protected void LimpiaSeleccionPrevia()
        {
            PanelCentral.SetTitle("_");
            FormPanelInfoAd.Reset();
            FormPanelInfoAd.Show();
        }


        protected void GenerarPedido(object sender, DirectEventArgs e)
        {
            try
            {
                Int32 total = (Int32.Parse(e.ExtraParams["TotalRegistros"]));

                if (total == 0)
                {
                    X.Msg.Notify("Nuevo Pedido", "El lote no tiene detalles").Show();
                    X.Msg.Notify("Nuevo Pedido", "<br />  <br /> <b> D E N E G A D A </b> <br />  <br /> ").Show();
                    return;
                }

                string json = e.ExtraParams["Values"];
                Dictionary<string, string>[] losPedidos = JSON.Deserialize<Dictionary<string, string>[]>(json);
                string pedidosDetalle = "";

                foreach (Dictionary<string, string> row in losPedidos)
                {
                    pedidosDetalle = pedidosDetalle + row["Cantidad"] + ":" + row["Monto"] + "|";
                }

                DataTable _dtNuevoPedido = LNEcommerceRedVoucher.CreaNuevoPedido(
                    Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()), "IdClienteAmazon").Valor),
                    Convert.ToInt32(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()), "IdSucursalAmazon").Valor), 
                    pedidosDetalle, this.Usuario);

                string msj = _dtNuevoPedido.Rows[0]["Mensaje"].ToString();
                int idNuevoPedido = Convert.ToInt32(_dtNuevoPedido.Rows[0]["ID_Pedido"]);

                if (idNuevoPedido == -1)
                {
                    X.Msg.Alert("Nuevo Pedido", msj).Show();
                }
                else
                {
                    WdwNuevoLote.Hide();
                    this.hdnIdLote.Text = idNuevoPedido.ToString();

                    SolicitaCertificados();

                    if (this.dtLote.RawText != "")
                    {
                        LlenaGridResultados();

                        RowSelectionModel rsm = GridResultados.GetSelectionModel() as RowSelectionModel;
                        rsm.SelectedRows.Add(new SelectedRow(0));
                        rsm.UpdateSelection();
                        GridResultados.FireEvent("RowClick");
                    }
                }
            }
            catch (CAppException err)
            {
                X.Msg.Alert("Nuevo Pedido", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Nuevo Pedido", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar del panel izquierdo
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LlenaGridResultados();
        }

        /// <summary>
        /// Llena el grid de resultados de promociones con la información de base de datos
        /// </summary>
        protected void LlenaGridResultados()
        {
            try
            {
                string [] fecha = this.dtLote.RawText.Split('/');
                int anio = Convert.ToInt16(fecha[1]);
                int mes = Convert.ToInt16(fecha[0]);

                DateTime fechaSel = new DateTime(anio, mes, 1);
                DataSet dsLotes = DAOCertificadosAmazon.ObtienePedidosPorMes(fechaSel, this.Usuario);

                if (dsLotes.Tables[0].Rows.Count == 0)
                {
                    X.Msg.Alert("Certificados", "No existen coincidencias con los datos solicitados").Show();
                }
                else
                {
                    StoreLotes.DataSource = dsLotes;
                    StoreLotes.DataBind();
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Certificados", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALLealtad.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Certificados", ex.Message).Show();
            }
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
                int IdPedido = 0;
                string Fecha = "";  //, DescripcionPromo = "";

                LimpiaSeleccionPrevia();

                string json = e.ExtraParams["Values"];
                IDictionary<string, string>[] promocion = JSON.Deserialize<Dictionary<string, string>[]>(json);

                foreach (KeyValuePair<string, string> column in promocion[0])
                {
                    switch (column.Key)
                    {
                        case "ID_Pedido": IdPedido = int.Parse(column.Value); break;
                        case "Fecha": Fecha = column.Value; break;
                        default:
                            break;
                    }
                }

                this.hdnIdLote.Value = IdPedido;

                LlenaFormPanelInfoAd(IdPedido);
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Certificados Amazon", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALLealtad.Utilidades.Loguear.Error(ex, "");
                X.Msg.Alert("Certificados Amazon", ex.Message).Show();
            }
        }

        /// <summary>
        /// Llena el panel de Información Adicional con los datos consultados a base de datos
        /// </summary>
        /// <param name="IdPedido">Identificador de la promoción</param>
        protected void LlenaFormPanelInfoAd(int IdPedido)
        {
            try
            {
                DataSet dsDetalle = DAOCertificadosAmazon.ObtieneDetallePedido(IdPedido, this.Usuario);

                if (dsDetalle.Tables[0].Rows.Count == 0)
                {
                    X.Msg.Alert("Certificados", "No existen coincidencias con los datos solicitados").Show();
                }
                else
                {
                    StoreDetalle.DataSource = dsDetalle;
                    StoreDetalle.DataBind();
                    pnlBotones.Hidden = false;

                    DataRow row = dsDetalle.Tables[0].Rows[0];
                    PanelCentral.SetTitle("ID: " + IdPedido.ToString() + " / " + row.ItemArray[4].ToString());
                    PanelCentral.Disabled = false;

                    if (Convert.ToBoolean(row.ItemArray[3]) == true) //si hay codigos no generados
                    {
                        this.lblDescripcionRegla.Hidden = true;
                        this.btnReintentar.Hidden = true;
                        this.btnExportExcel.Hidden = false;
                    }
                    else
                    {
                        this.lblDescripcionRegla.Hidden = false;
                        this.btnReintentar.Hidden = false;
                        this.btnExportExcel.Hidden = false;
                    }
                }
            }
            catch (CAppException caEx)
            {
                DALLealtad.Utilidades.Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Información Adicional", caEx.Mensaje()).Show();
            }
            catch (Exception ex)
            {
                DALLealtad.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Información Adicional", ex.Message).Show();
            }
        }

        protected void Download(object sender, DirectEventArgs e)
        {
            ExportToExcel();
        }

        protected void Reintentar(object sender, DirectEventArgs e)
        {
            SolicitaCertificados();
        }

        [DirectMethod(Namespace = "Lealtad")]
        private void SolicitaCertificados()
        {
            try
            {
                //RespuestasJSON.ValidaMembresiaSams wsLoginResp;
                Parametros.BodyCertificadosAmazon _body = new Parametros.BodyCertificadosAmazon();

                _body.WsUsuario = Configuracion.Get(Guid.Parse(
                                        ConfigurationManager.AppSettings["IDApplication"].ToString().Trim()),
                                        "WsCertAmazon_Usr").Valor;

                _body.WsPassword = Configuracion.Get(Guid.Parse(
                                        ConfigurationManager.AppSettings["IDApplication"].ToString().Trim()),
                                        "WsCertAmazon_Pwd").Valor;

                var URL = Configuracion.Get(Guid.Parse(
                                ConfigurationManager.AppSettings["IDApplication"].ToString().Trim()),
                                "WsCertAmazon_URL").Valor;

                _body.IdPedido = Convert.ToInt32(this.hdnIdLote.Text);

                SolictiudPedidosAmazon wsResp = CertificadosAmazonWS.GeneraCertificadosAmazon(URL, _body, this.Usuario);

                if (wsResp == null)
                {
                    X.Msg.Alert("Certificados Amazon", "Ocurrió un error al intentar generar los certificados.").Show();
                }
                else
                {
                    if (wsResp.CodigoRespuesta != 0)
                    {
                        X.Msg.Alert("Certificados Amazon", "Ocurrió un error al intentar generar los certificados.").Show();
                    }else
                    {
                        X.Msg.Alert("Certificados Amazon", "Certificados generados.").Show();
                    }
                }

                LlenaFormPanelInfoAd(_body.IdPedido);
            }
            catch (Exception)
            {
                X.Msg.Alert("Certificados Amazon", "Ocurrió un error al intentar solicitar los certificados.").Show();
                throw;
            }
        }

        /// <summary>
        /// Exporta el reporte generado a un archivo Excel 
        /// </summary>
        [DirectMethod(Namespace = "Lealtad")]
        public void ExportToExcel()
        {
            try
            {
                DataTable _dtCertificados = new DataTable();
                _dtCertificados = HttpContext.Current.Session["  "] as DataTable;
                _dtCertificados = DAOCertificadosAmazon.ObtieneCodigosPedido(Convert.ToInt32(this.hdnIdLote.Text), this.Usuario);
                HttpContext.Current.Session.Add("DtCodigos", _dtCertificados);

                string reportName = "CertificadosAmazon";

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dtCertificados, reportName);

                //Se prepara la respuesta
                this.Response.Clear();
                this.Response.ClearContent();
                this.Response.ClearHeaders();
                this.Response.Buffer = false;

                this.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                this.Response.AddHeader("Content-Disposition", "attachment; filename=" + reportName + ".xlsx");

                //Se envía el reporte como respuesta
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    wb.SaveAs(memoryStream);
                    memoryStream.WriteTo(this.Response.OutputStream);
                    memoryStream.Close();
                }

                this.Response.End();
            }

            catch (Exception ex)
            {
                DALLealtad.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Certificados Amazon", "Ocurrió un Error al Exportar los Certificados a Excel").Show();
            }
        }

        /// <summary>
        /// Da una pausa al sistema para mostrar y finaliza la máscara al botón de
        /// Exportar a Excel
        /// </summary>
        [DirectMethod(Namespace = "Lealtad")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }
                
    }
}
       