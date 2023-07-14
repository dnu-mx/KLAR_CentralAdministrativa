using CentroContacto.LogicaNegocio;
using ClosedXML.Excel;
using DALCentroContacto.BaseDatos;
using DALCentroContacto.Entidades;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Xsl;

namespace CentroContacto
{
    public partial class ConsultaTarjetas_SmartTicketMC : DALCentralAplicaciones.PaginaBaseCAPP
    {
        /// <summary>
        /// Realiza y controla la carga de la página Consulta de Tarjetas
        /// de Clientes Smart Ticket
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    StoreEstatus.DataSource = DAOConsultaTarjetasSmartTicketMC.ListaCatalogoEstatusTarjeta(this.Usuario);
                    StoreEstatus.DataBind();
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
                Cliente elCliente = new Cliente();

                elCliente.Nombre = txtNombre.Text;
                elCliente.ApellidoPaterno = txtApPaterno.Text;
                elCliente.ApellidoMaterno = txtApMaterno.Text;
                elCliente.Email = txtCorreo.Text;
                
                DataSet dsResultados = LNTarjetasSmartTicketMC.ConsultaClientes(elCliente, this.Usuario);

                limpiaBusquedaPrevia(false);

                int numRecords = dsResultados.Tables[0].Rows.Count;

                if (numRecords == 100)
                {
                    X.Msg.Alert("Consulta de Clientes", "Demasiadas coincidencias, por favor afine su búsqueda").Show();
                }
                else if (numRecords == 0)
                {
                    X.Msg.Alert("Consulta de Clientes", "No existen coincidencias con la búsqueda solicitada").Show();
                }

                GridResultados.GetStore().DataSource = dsResultados;
                GridResultados.GetStore().DataBind();
            }

            catch (CAppException caEx)
            {
                DALLoyalty.Utilidades.Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Consulta de Clientes", caEx.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALLoyalty.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Consulta de Clientes", "Ocurrió un Error al Ejecutar la Búsqueda con los Datos Proporcionados").Show();
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
        protected void StoreClientes_Refresh(object sender, StoreRefreshDataEventArgs e)
        {
            LlenarGridResultados();
        }

        /// <summary>
        /// Limpia los controles, páneles, grids asociados a la búsqueda previa de clientes dentro
        /// del Grid de Resultados Clientes
        /// </summary>
        /// <param name="esBtnLimpiar">Indica si el método de origen fue el clic al botón Limpiar</param>
        protected void limpiaBusquedaPrevia(bool esBtnLimpiar)
        {
            if (esBtnLimpiar)
            {
                FormPanelBusqueda.Reset();
                StoreClientes.RemoveAll();
            }

            StoreTarjetas.RemoveAll();
            FormPanelResultados.Reset();

            FormPanelCentral.Reset();
            hdnIdCliente.Clear();
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
            int IdCliente = int.Parse(e.ExtraParams["IdCliente"]);

            hdnIdCliente.Text = IdCliente.ToString();

            LlenaGridPanelTarjetas(IdCliente);
        }

        /// <summary>
        /// Consulta a BD los datos del cliente seleccionado y los llena en el FieldSet correspondiente,
        /// almacenando en controles ocultos los datos clave para las funcionalidades de las pestañas.
        /// </summary>
        /// <param name="idcliente">Identificador del cliente</param>
        protected void LlenaGridPanelTarjetas(int idcliente)
        {
            try
            {
                btnExportExcel.Disabled = true;
                btnExportCSV.Disabled = true;

                DataSet dsTarjetas = DAOConsultaTarjetasSmartTicketMC.ObtieneTarjetasCliente(idcliente, this.Usuario);

                if (dsTarjetas.Tables[0].Rows.Count == 0)
                {
                    X.Msg.Alert("Consulta de Tarjetas", "No existen coincidencias con la búsqueda solicitada").Show();
                }
                else
                {
                    StoreTarjetas.DataSource = dsTarjetas;
                    StoreTarjetas.DataBind();

                    btnExportExcel.Disabled = false;
                    btnExportCSV.Disabled = false;
                }
            }

            catch (CAppException caEx)
            {
                DALLoyalty.Utilidades.Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Consulta de Tarjetas", caEx.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALLoyalty.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Consulta de Tarjetas", "Ocurrió un Error al Consultar las Tarjetas del Cliente").Show();
            }
        }

        /// <summary>
        /// Extrae el grid de tarjetas del backend para generar y exportarlo a un archivo Excel
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void ExportGridToExcel(object sender, DirectEventArgs e)
        {
            string gridResultJson = e.ExtraParams["GridResult"];
            string reportName = "ReporteTarjetas";

            XmlNode gridResultXml = JSON.DeserializeXmlNode("{records:{record:" + gridResultJson + "}}");
            XmlTextReader xtr = new XmlTextReader(gridResultXml.OuterXml, XmlNodeType.Element, null);

            DataSet ds = new DataSet();
            ds.ReadXml(xtr);

            XLWorkbook wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(reportName);

            //Se inserta la tabla completa a la hoja de Excel
            ws.Cell(1, 1).InsertTable(ds.Tables[0].AsEnumerable());

            //Se da el formato deseado a las columnas
            ws = FormatWsColumns(ws, ws.Column(1).CellsUsed().Count());

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

        /// <summary>
        /// Establece el formato deseado a las columnas de la hoja de trabajo por exportar
        /// </summary>
        /// <param name="ws">Hoja de trabajo</param>
        /// <param name="rowsNum">Total de filas de la hoja de trabajo</param>
        /// <returns></returns>
        protected IXLWorksheet FormatWsColumns(IXLWorksheet ws, int rowsNum)
        {
            ws.Column(1).Hide();

            for (int rowsCounter = 2; rowsCounter <= rowsNum; rowsCounter++)
            {
                ws.Cell(rowsCounter, 5).SetDataType(XLCellValues.DateTime);
            }

            return ws;
        }

        /// <summary>
        /// Controla el evento SUBMIT al querer exportar al formato seleccionado
        /// los resultados de la consulta
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
        /// Llama al método de llenado del grid de tarjetas, para restablecer la consulta
        /// como estaba antes de la edición
        /// </summary>
        [DirectMethod(Namespace = "CentroContacto")]
        public void RestableceEstatusTarjetas()
        {
            LlenaGridPanelTarjetas(int.Parse(hdnIdCliente.Text));
        }

        /// <summary>
        /// Controla la actualización del estatus de la tarjeta
        /// </summary>
        /// <param name="IdTarjeta">Identificador de la tarjeta</param>
        /// <param name="Valor">Nuevo valor</param>
        [DirectMethod(Namespace = "CentroContacto")]
        public void ActualizaEstatusTarjeta(int IdTarjeta, string Valor)
        {
            try
            {
                LNTarjetasSmartTicketMC.ModificaEstatusTarjeta(IdTarjeta, int.Parse(Valor), this.Usuario);

                X.Msg.Notify("", "Estatus de la Tarjeta Actualizado <br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                LlenaGridPanelTarjetas(int.Parse(hdnIdCliente.Text));
            }

            catch (CAppException caEx)
            {
                DALLoyalty.Utilidades.Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Estatus", caEx.Mensaje()).Show();
            }

            catch (Exception ex)
            {
                DALLoyalty.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Estatus", "Ocurrió un Error al Actualizar el Estatus de la Tarjeta del Cliente").Show();
            }
        }

        /// <summary>
        /// Da una pausa al sistema para mostrar y finaliza la máscara al botón de
        /// Exportar a Excel
        /// </summary>
        [DirectMethod(Namespace = "CentroContacto")]
        public void StopMask()
        {
            Thread.Sleep(1000);
            X.Mask.Hide();
        }
    }
}