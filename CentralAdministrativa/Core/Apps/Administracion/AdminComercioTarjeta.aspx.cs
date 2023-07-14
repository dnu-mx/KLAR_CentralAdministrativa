using ClosedXML.Excel;
using DALAdministracion.BaseDatos;
using DALAdministracion.LogicaNegocio;
using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using Ext.Net;
using Interfases.Exceptiones;
using Log_PCI;
using Log_PCI.Entidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using Utilerias;

namespace Administracion
{
    public partial class AdminComercioTarjeta : DALCentralAplicaciones.PaginaBaseCAPP
    {
        private LogHeader LH_ComTarjeta = new LogHeader();

        protected void Page_Load(object sender, EventArgs e)
        {
            ///LOG HEADER
            LH_ComTarjeta.IP_Address = Request.ServerVariables["REMOTE_ADDR"].ToString();
            LH_ComTarjeta.Application_ID = Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString());
            LH_ComTarjeta.User = this.Usuario.ClaveUsuario;
            LH_ComTarjeta.Trace_ID = Guid.NewGuid();

            LogPCI log = new LogPCI(LH_ComTarjeta);
            string errRedirect = string.Empty;

            try
            {
                log.Info("INICIA AdminComercioTarjeta Page_Load()");

                if (!IsPostBack)
                {
                    log.Info("INICIA ListaColectivasSubEmisor()");
                    DataTable dtSubEmisores = DAOColectiva.ListaColectivasSubEmisor("GCM", this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                        LH_ComTarjeta);
                    log.Info("TERMINA ListaColectivasSubEmisor()");

                    List<ColectivaComboPredictivo> ComboList = new List<ColectivaComboPredictivo>();

                    foreach (DataRow drCol in dtSubEmisores.Rows)
                    {
                        var colectivaCombo = new ColectivaComboPredictivo()
                        {
                            ID_Colectiva = Convert.ToInt32(drCol["ID_Colectiva"]),
                            ClaveColectiva = drCol["ClaveColectiva"].ToString(),
                            NombreORazonSocial = drCol["NombreORazonSocial"].ToString()
                        };
                        ComboList.Add(colectivaCombo);
                    }

                    this.StoreSubEmisores.DataSource = ComboList;
                    this.StoreSubEmisores.DataBind();

                    HttpContext.Current.Session.Add("dsResultados", null);
                }

                log.Info("TERMINA AdminComercioTarjeta Page_Load()");
            }

            catch (Exception ex)
            {
                log.Error(ex.Message);
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

        protected void btnNuevoProducto_Click(object sender, EventArgs e)
        {
            LogPCI log = new LogPCI(LH_ComTarjeta);
            try
            {
                string afiliacion = this.NumAfiliacion.RawValue.ToString();
                string tarjeta = this.NumTarjeta.RawValue.ToString();

                log.Info("INICIA ListaTarjeta()");
                DataSet ds = DAOComercioTarjeta.ListaTarjeta(tarjeta, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    LH_ComTarjeta);
                log.Info("TERMINA ListaTarjeta()");

                if (ds.Tables[0].Rows.Count > 0)
                {
                    log.Info("INICIA ListaComercioTarjeta()");
                    DataSet data = DAOComercioTarjeta.ListaComercioTarjeta(afiliacion, tarjeta, LH_ComTarjeta);
                    log.Info("TERMINA ListaComercioTarjeta()");

                    if (data.Tables[0].Rows.Count > 0)
                    {
                        X.Msg.Alert("Comercio-Tarjeta", "El Número de Tarjeta y el Número de Afiliación ya se encuentran registrados.").Show();
                    }
                    else
                    {

                        log.Info("INICIA CreaNuevoComercioTarjeta()");
                        LNComercioTarjeta.CreaNuevoComercioTarjeta(afiliacion, tarjeta, this.Usuario, LH_ComTarjeta);
                        log.Info("TERMINA CreaNuevoComercioTarjeta()");

                        this.NumAfiliacion.Value = "";
                        this.NumTarjeta.Value = "";

                        X.Msg.Notify("Comercio-Tarjeta", "Registro guardado <br />  <br /> <b>  E X I T O S A M E N T E </b> <br />  <br /> ").Show();
                    }
                }
                else
                {
                    X.Msg.Alert("Comercio-Tarjeta", "El Número de Tarjeta es inválido o no tienes permisos para registrarla.").Show();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                X.Msg.Alert("Comercio-Tarjeta", "Ocurrió un error al crear el nuevo registro.").Show();
            }
        }

        protected void btnConsultar_Click(object sender, EventArgs e)
        {
            LogPCI log = new LogPCI(LH_ComTarjeta);
            try
            {
             
                string cliente = this.cBoxCliente.SelectedItem.Text;
                string afiliacion = this.txtNumeroAfiliciacion.Text;
                string tarjeta = this.txtNumeroTarjeta.Text;
                bool entro = false;
                if (cliente != "" && tarjeta != "" && tarjeta != null)
                {
                    log.Info("INICIA ObtienNombreORazonsocial()");
                    DataSet data = DAOComercioTarjeta.ObtienNombreORazonsocial(tarjeta, this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), LH_ComTarjeta);
                    log.Info("TERMINA ObtienNombreORazonsocial()");

                    if (data.Tables[0].Rows.Count == 0)
                    {
                       entro = true;
                        this.StoreComercio.DataSource = "";
                        this.StoreComercio.DataBind();
                        X.Msg.Alert("Comercio-Tarjeta", "El número de tarjeta no es de " + cliente + "").Show();
                       
                    }
                }
                if(entro == false)
                {

                    log.Info("INICIA ObtieneComercioTarjeta()");
                    DataSet dsResultados = DAOComercioTarjeta.ObtieneComercioTarjeta(
                        Convert.ToInt64(this.cBoxCliente.SelectedItem.Value), afiliacion, tarjeta, this.Usuario, 
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), LH_ComTarjeta);
                    log.Info("TERMINA ObtieneComercioTarjeta()");

                    if (dsResultados.Tables[0].Rows.Count > 0)
                    {
                        DataTable dt = Tarjetas.EnmascaraTablaConTarjetas(dsResultados.Tables[0], "ClaveMA", "Enmascara", LH_ComTarjeta);

                        HttpContext.Current.Session.Add("dsResultados", dt);

                        this.StoreComercio.DataSource = dt;
                        this.StoreComercio.DataBind();
                        this.btnExcel.Disabled = false;
                    }
                    else
                    {
                        this.StoreComercio.DataSource = "";
                        this.StoreComercio.DataBind();
                        X.Msg.Alert("Comercio-Tarjeta", "No existen coincidencias con la búsqueda solicitada").Show();
                    }                  
                }
            }

            catch (CAppException err)
            {
                X.Msg.Alert("Comercio-Tarjeta", err.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                log.Error(ex.Message);
                X.Msg.Alert("Comercio-Tarjeta", "Ocurrió un error al obtener la relación Comercio-Tarjetas").Show();
            }
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            this.cBoxCliente.Reset();
            this.txtNumeroAfiliciacion.Reset();
            this.txtNumeroTarjeta.Reset();

            this.StoreComercio.RemoveAll();

        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            this.NumAfiliacion.Value = "";
            this.NumTarjeta.Value = "";
            this.WdwNuevoComercioTarjeta.Hide();

        }

        [DirectMethod(Namespace = "AdminComTarj")]
        public void StopMask()
        {
            Thread.Sleep(5000);
            X.Mask.Hide();
        }

        protected void DownloadComercioTarjeta(object sender, DirectEventArgs e)
        {
            ExportDTDetalleToExcel();
        }

        [DirectMethod(Namespace = "AdminComTarj")]
        public void ExportDTDetalleToExcel()
        {
            LogPCI unLog = new LogPCI(LH_ComTarjeta);
            try
            {
                string reportName = "ComercioTarjeta";
                DataTable _dsResultados = HttpContext.Current.Session["dsResultados"] as DataTable;

                XLWorkbook wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(_dsResultados, reportName);

                //Se da el formato deseado a las columnas
                ws = FormatWSComercioTarjeta(ws, ws.Column(1).CellsUsed().Count());

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
            }

            catch (Exception ex)
            {
                unLog.Error("Error al exportar el reporte a Excel");
                unLog.ErrorException(ex);;
            }

            finally
            {
                if (this.Response != null)
                {
                    this.Response.Clear();
                    this.Response.ClearContent();
                    this.Response.OutputStream.Dispose();

                    this.Response.Flush();
                    this.Response.Close();

                    GC.Collect();
                }
            }
        }

        protected IXLWorksheet FormatWSComercioTarjeta(IXLWorksheet ws, int rowsNum)
        {
            try
            {
                ws.Column(4).Hide();

                return ws;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void EjecutarComando(object sender, DirectEventArgs e)
        {
            LogPCI pCI = new LogPCI(LH_ComTarjeta);
            pCI.Info("EjecutarComando()");

            try
            {
                char[] charsToTrim = { '*', '"', ' ' };
                string comando = (string)e.ExtraParams["Comando"];
                string afiliacion = string.Empty;

                switch (comando)
                {
                    case "Eliminar":
                        afiliacion = e.ExtraParams["Afiliacion"].Trim(charsToTrim);
                        EliminaComercioTarjeta(afiliacion);
                        break;

                    default: break;
                }
            }
            catch (Exception ex)
            {
                pCI.ErrorException(ex);
                X.Msg.Alert("Acciones", "Ocurrió un error al ejecutar la acción seleccionada").Show();
            }
        }


        protected void EliminaComercioTarjeta(string afiliacion)
        {
            LogPCI log = new LogPCI(LH_ComTarjeta);

            try
            {
                log.Info("INICIA EliminaComercioTarjeta()");
                LNComercioTarjeta.EliminaComercioTarjeta(afiliacion, this.Usuario, LH_ComTarjeta);
                log.Info("TERMINA EliminaComercioTarjeta()");

                X.Msg.Notify("Comercio-Tarjeta", "Registro eliminado <br />  <br /> <b>  E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                log.Info("INICIA ObtieneComercioTarjeta()");
                DataSet dsResultados = DAOComercioTarjeta.ObtieneComercioTarjeta(
                    Convert.ToInt64(this.cBoxCliente.SelectedItem.Value),
                    this.txtNumeroAfiliciacion.Text, this.txtNumeroTarjeta.Text, this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()), LH_ComTarjeta);
                log.Info("INICIA ObtieneComercioTarjeta()");

                DataTable dt = Tarjetas.EnmascaraTablaConTarjetas(dsResultados.Tables[0], "ClaveMA", "Enmascara", LH_ComTarjeta);

                HttpContext.Current.Session.Add("dsResultados", dt);

                this.StoreComercio.DataSource = dt;
                this.StoreComercio.DataBind();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                X.Msg.Alert("Comercio-Tarjeta", "Ocurrió un error al eliminar el registro.").Show();
            }
        }
    }
}