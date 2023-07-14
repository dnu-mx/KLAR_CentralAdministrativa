using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using DALCentralAplicaciones;
using DALValidacionesBatchPPF.BaseDatos;
using DALValidacionesBatchPPF.Utilidades;
using Ext.Net;
using Interfases.Exceptiones;


namespace ValidacionesBatch
{
    public partial class ConsultaOperaciones : PaginaBaseCAPP
    {
        /// <summary>
        /// Realiza y controla la carga de la página Consulta de Operaciones
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    //Prestablecemos el periodo de consulta de las operaciones
                    dfFechaInicialOper.MaxDate = DateTime.Today;
                    dfFechaInicialOper.MinDate = DateTime.Today.AddDays(-180);
                    dfFechaInicialOper.SetValue(DateTime.Today.AddDays(-180));

                    dfFechaFinalOper.MaxDate = DateTime.Today;
                    dfFechaFinalOper.SetValue(DateTime.Today);
                }
            }
            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar de la pestaña de Operaciones, invocando a la 
        /// búsqueda de operaciones de la tarjeta en base de datos
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscarOper_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(this.txtAfiliacionOper.Text))
                {
                    Int64 afil;

                    if (!Int64.TryParse(this.txtAfiliacionOper.Text, out afil))
                    {
                        X.Msg.Alert("", "La Afiliación debe ser numérica").Show();
                        return;
                    }
                }

                if (!String.IsNullOrEmpty(this.txtTarjeta.Text))
                {
                    Int64 tarjeta;

                    if (!Int64.TryParse(this.txtTarjeta.Text, out tarjeta))
                    {
                        X.Msg.Alert("Filtros de búsqueda", "La Tarjeta debe ser numérica").Show();
                        return;
                    }
                }

                StoreResultadosOper.RemoveAll();
                this.lblTituloTarjeta.Text = "";

                DataSet dsOperaciones = DAOEfectivaleOffline.ObtieneOperacionesPorTarjeta(
                    Convert.ToDateTime(this.dfFechaInicialOper.SelectedDate),
                    Convert.ToDateTime(this.dfFechaFinalOper.SelectedDate),
                    this.txtAfiliacionOper.Text, this.txtTarjeta.Text, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                if (dsOperaciones.Tables[0].Rows.Count == 0)
                {
                    X.Msg.Alert("Operaciones", "No existen coincidencias con los datos solicitados").Show();
                }
                else
                {
                    this.lblTituloTarjeta.Text = "Tarjeta: T" + this.txtTarjeta.Text;
                    StoreResultadosOper.DataSource = dsOperaciones;
                    StoreResultadosOper.DataBind();
                }
            }

            catch (CAppException caEx)
            {
                Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Operaciones", "Ocurrió un Error en la Consulta de Operaciones").Show();

            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Operaciones", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar de la pestaña de Operaciones,
        /// restableciendo los controles de la página al valor de carga origen
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiarOper_Click(object sender, EventArgs e)
        {
            FormPanelBuscarOperaciones.Reset();
            this.txtAfiliacionOper.Reset();
            this.txtTarjeta.Reset();
            this.lblTituloTarjeta.Text = "";

            dfFechaInicialOper.SetValue(DateTime.Today.AddDays(-180));

            dfFechaFinalOper.SetValue(DateTime.Today);

            StoreResultadosOper.RemoveAll();
        }

        /// <summary>
        /// Controla el evento SUBMIT al querer exportar al formato seleccionado
        /// los resultados de la consulta de operaciones
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void StoreSubmit(object sender, StoreSubmitDataEventArgs e)
        {
            string format = this.FormatType.Value.ToString();

            XmlNode xml = e.Xml;

            this.Response.Clear();

            switch (format)
            {
                case "xls":
                    this.Response.ContentType = "application/vnd.ms-excel";
                    this.Response.AddHeader("Content-Disposition", "attachment; filename=Reporte.xls");
                    XslCompiledTransform xtExcel = new XslCompiledTransform();
                    xtExcel.Load(Server.MapPath("xslFiles/Excel.xsl"));
                    xtExcel.Transform(xml, null, Response.OutputStream);

                    break;

                case "csv":
                    this.Response.ContentType = "application/octet-stream";
                    this.Response.AddHeader("Content-Disposition", "attachment; filename=Reporte.csv");
                    XslCompiledTransform xtCsv = new XslCompiledTransform();
                    xtCsv.Load(Server.MapPath("xslFiles/Csv.xsl"));
                    xtCsv.Transform(xml, null, Response.OutputStream);

                    break;
            }
            this.Response.End();
        }

        /// <summary>
        /// Controla el evento de selección de una celda del grid de Operaciones
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void CellGridResultadosOper_Click(object sender, DirectEventArgs e)
        {
            CellSelectionModel sm = GridResultadosOper.SelectionModel.Primary as CellSelectionModel;

            if (String.Compare(sm.SelectedCell.Name, "Afiliacion") != 0)
            {
                return;
            }

            if (!String.IsNullOrEmpty(sm.SelectedCell.Value))
            {
                VentanaAfiliacion(sm.SelectedCell.Value);
            }
        }

        /// <summary>
        /// Solicita la consulta de datos de la afiliación con el valor indicado y muestra
        /// la ventana correspondiente
        /// </summary>
        /// <param name="afiliacion">Afiliación por consultar y mostrar sus datos</param>
        protected void VentanaAfiliacion(String afiliacion)
        {
            try
            {
                DataSet dsAfiliacion =
                    DAOEfectivaleOffline.ObtieneDatosAfiliacion(afiliacion,
                    this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                this.txtAfiliacion.Text = dsAfiliacion.Tables[0].Rows[0]["Afiliacion"].ToString().Trim();
                this.txtNombre.Text = dsAfiliacion.Tables[0].Rows[0]["Nombre"].ToString().Trim();
                this.txtPropietario.Text = dsAfiliacion.Tables[0].Rows[0]["Propietario"].ToString().Trim();
                this.txtRazonSocial.Text = dsAfiliacion.Tables[0].Rows[0]["Razon_Social"].ToString().Trim();
                this.txtRFC.Text = dsAfiliacion.Tables[0].Rows[0]["RFC"].ToString().Trim();

                this.txtDomicilio.Text = dsAfiliacion.Tables[0].Rows[0]["Domicilio"].ToString().Trim();
                this.txtColonia.Text = dsAfiliacion.Tables[0].Rows[0]["Colonia"].ToString().Trim();
                this.txtCodigoPostal.Text = dsAfiliacion.Tables[0].Rows[0]["CP"].ToString().Trim();
                this.txtEstado.Text = dsAfiliacion.Tables[0].Rows[0]["Estado"].ToString().Trim();
                this.txtDescripcion.Text = dsAfiliacion.Tables[0].Rows[0]["Descripcion_SIC"].ToString().Trim();

                this.WdwAfiliacion.Show();
            }

            catch (CAppException caEx)
            {
                Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Afiliación", "Ocurrió un Error en la Consulta de Datos de la Afiliación").Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Afiliación", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Aceptar de la ventana Afiliación, para ocultarla
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnAceptar_Click(object sender, EventArgs e)
        {
            this.WdwAfiliacion.Hide();
        }
    }
}