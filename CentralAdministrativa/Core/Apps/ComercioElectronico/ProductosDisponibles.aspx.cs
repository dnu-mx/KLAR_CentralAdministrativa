using ClosedXML.Excel;
using DALCentralAplicaciones;
using DALComercioElectronico.BaseDatos;
using DALComercioElectronico.LogicaNegocio;
using DALComercioElectronico.Utilidades;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Xsl;

namespace ComercioElectronico
{
    public partial class ProductosDisponibles : PaginaBaseCAPP
    {
        /// <summary>
        /// Realiza y controla la carga de la página Productos Disponibles
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["Ext.Net.Theme"] = Ext.Net.Theme.Default;

            try
            {               
                if (!IsPostBack)
                {
                    this.StoreFamilias.DataSource = DAOMoshi.ListaFamilias(this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                    this.StoreFamilias.DataBind();

                    this.StoreSucursales.DataSource = DAOMoshi.ListaSucursales(this.Usuario,
                        Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                    this.StoreSucursales.DataBind();

                    cBoxDisponibilidad.SetValue(-1);
                }
            }

            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Buscar
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            LlenaGridProductos();
        }

        /// <summary>
        /// Solicita la consulta de productos en base de datos y llena el grid correspondiente
        /// </summary>
        protected void LlenaGridProductos()
        {
            try
            {
                StoreProductos.RemoveAll();

                DataSet dsProductos = DAOMoshi.ObtieneProductos(
                    string.IsNullOrEmpty(cBoxFamilia.SelectedItem.Value) ? -1 : int.Parse(cBoxFamilia.SelectedItem.Value),
                    this.txtSKU.Text, this.txtProducto.Text, int.Parse(cBoxSucursal.SelectedItem.Value),
                    int.Parse(cBoxDisponibilidad.SelectedItem.Value), this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));

                if (dsProductos.Tables[0].Rows.Count == 0)
                {
                    X.Msg.Alert("Búsqueda de Productos", "No existen coincidencias con los datos solicitados").Show();
                }
                else
                {
                    this.btnExportExcel.Disabled = false;

                    StoreProductos.DataSource = dsProductos;
                    StoreProductos.DataBind();
                }
            }

            catch (CAppException caEx)
            {
                Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Búsqueda de Productos", "Ocurrió un Error en la Consulta de Productos").Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Búsqueda de Productos", ex.Message).Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar, restableciendo
        /// los controles de la página al valor de carga origen
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            cBoxFamilia.Reset();
            txtSKU.Reset();
            txtProducto.Reset();
            cBoxSucursal.Reset();
            cBoxDisponibilidad.SetValue(-1);

            StoreProductos.RemoveAll();
            btnExportExcel.Disabled = true;
        }

        /// <summary>
        /// Exporta el grid de resultados al backend, para generar y exportarlo a un archivo Excel
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void ExportGridToExcel(object sender, DirectEventArgs e)
        {
            string gridResultJson = e.ExtraParams["GridToExport"];
            string reportName = "ProductosSucursal";

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
            this.Response.AddHeader("Content-Disposition", "attachment; filename=Reporte_" + reportName + ".xlsx");

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
                ws.Cell(rowsCounter, 6).SetDataType(XLCellValues.Boolean);
            }

            return ws;
        }

        /// <summary>
        /// Controla el evento de ejecución de comandos en el grid de cuentas
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos ddirectos el evento que se ejecutó</param>
        protected void EjecutarComando(object sender, DirectEventArgs e)
        {
            try
            {
                String comando = (String)e.ExtraParams["Comando"];

                int IdProducto = int.Parse(e.ExtraParams["ID_Producto"]);
                int Disponible = Convert.ToInt32(bool.Parse(e.ExtraParams["Disponible"]));

                LNMoshi.ModificaDisponibilidadProducto(IdProducto, int.Parse(cBoxSucursal.SelectedItem.Value),
                    Disponible, this.Usuario);

                X.Msg.Notify("Producto", "Disponibilidad Modificada" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
                LlenaGridProductos();
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Acción", "Ocurrio un Error al Ejecutar la Acción Seleccionada").Show();
            }
        }
    }
}
