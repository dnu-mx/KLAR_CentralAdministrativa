using DALLealtad.BaseDatos;
using DALLealtad.LogicaNegocio;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace Lealtad
{
    public partial class CambioVigenciaPromociones : DALCentralAplicaciones.PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    this.LlenaComboPromociones();
                    //this.StoreVigencias.DataSource = this.GetData();
                    //this.StoreVigencias.DataBind();
                }
            }

            catch (Exception err)
            {
                DALLealtad.Utilidades.Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        /// <summary>
        /// Llena el combo con las promociones permitidas para el usuario y la aplicación
        /// </summary>
        protected void LlenaComboPromociones()
        {
            try
            {
                cBoxPromocion.GetStore().DataSource = DAOPromociones.ObtienePromociones(this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                cBoxPromocion.GetStore().DataBind();
            }

            catch (CAppException caEx)
            {
                X.Msg.Alert("Cambio de Vigencia", "Ocurrió un error al consultar las Promociones.").Show();
                DALLealtad.Utilidades.Loguear.Error(caEx, this.Usuario.ClaveUsuario);
            }
        }


        /// <summary>
        /// Controla el evento de selección de un elemento del combo de Promoción
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void selectPromocion(object sender, DirectEventArgs e)
        {
            try
            {
                this.cBoxDetalle.Reset();

                cBoxDetalle.GetStore().DataSource = DAOPromociones.ObtieneDetallePromocion(
                    int.Parse(this.cBoxPromocion.SelectedItem.Value), this.Usuario,
                    Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                cBoxDetalle.GetStore().DataBind();
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Cambio de Vigencia", "Ocurrió un error al consultar los Detalles de la Promoción.").Show();
                DALLealtad.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
            }
        }

        /// <summary>
        /// Controla el evento de selección de un elemento del detalle de promoción
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void selectDetalle(object sender, DirectEventArgs e)
        {
            try
            {
                LlenaGridVigencias(
                    Convert.ToInt32(this.cBoxPromocion.SelectedItem.Value),
                    Convert.ToInt32(this.cBoxDetalle.SelectedItem.Value));
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Cambio de Vigencia", "Ocurrió un error al consultar las fechas de vigencia de la promoción.").Show();
                DALLealtad.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
            }
        }

        /// <summary>
        /// Llena el PropertyGrid con los valores de la vigencia
        /// </summary>
        /// <param name="IdPromocion">Identificador de la promoción</param>
        /// <param name="IdDetalle">Identificador del detalle de la promoción</param>
        protected void LlenaGridVigencias(int IdPromocion, int IdDetalle)
        {
            try
            {
                StoreVigencias.RemoveAll();

                StoreVigencias.DataSource = DAOPromociones.ObtieneFechasVigencia(
                       IdPromocion, IdDetalle, this.Usuario,
                       Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString())
                   );
                StoreVigencias.DataBind();                
            }

            catch (Exception ex)
            {
                DALLealtad.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                throw new Exception("LlenaGridPropiedades()", ex);
            }
        }


        /// <summary>
        /// Controla el evento Click al botón de Guardar en PropertyGrid con los
        /// valores de los parámetros
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void btnGuardar_Click(object sender, DirectEventArgs e)
        {
            try
            {
                DataTable dt = new DataTable();
                int fila = 0;

                dt.Columns.Add("ID_Evento");
                dt.Columns.Add("Valor");
                dt.Columns.Add("EsFechaInicial");

                string json = e.ExtraParams["Values"];
                Dictionary<string, string>[] pertenencias = JSON.Deserialize<Dictionary<string, string>[]>(json);

                foreach (Dictionary<string, string> pertenencia in pertenencias)
                {
                    dt.Rows.Add();
                    dt.Rows[fila]["ID_Evento"] = this.cBoxPromocion.SelectedItem.Value;

                    foreach (KeyValuePair<string, string> columna in pertenencia)
                    {
                        switch (columna.Key)
                        {
                            case "Valor": dt.Rows[fila]["Valor"] = columna.Value; break;
                            case "EsFechaInicial": dt.Rows[fila]["EsFechaInicial"] = columna.Value; break;
                            default:
                                break;
                        }
                    }

                    fila++;
                }

                CambiaVigenciaPromocion(dt);
            }

            catch (Exception err)
            {
                DALLealtad.Utilidades.Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Notify("Cambio de Vigencia", "Cambio de Vigencia de la Promoción <br /><br />  <b> D E C L I N A D O </b> <br />  <br /> ").Show();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtCambios"></param>
        protected void CambiaVigenciaPromocion(DataTable dtCambios)
        {
            try
            {
                DateTime laFechaInicial = new DateTime(), laFechaFinal = new DateTime();
                bool esFechaIni;

                for (int reg = 0; reg < dtCambios.Rows.Count; reg++)
                {
                    esFechaIni = Convert.ToBoolean(dtCambios.Rows[reg]["EsFechaInicial"]);

                    if (esFechaIni)
                    {
                        laFechaInicial = Convert.ToDateTime(dtCambios.Rows[reg]["Valor"].ToString());
                    }
                    else
                    {
                        laFechaFinal = Convert.ToDateTime(dtCambios.Rows[reg]["Valor"].ToString());
                    }                    
                }

                ////Se validan fechas
                if (DateTime.Compare(laFechaInicial, laFechaFinal) > 0)
                {
                    X.Msg.Notify("Cambio de Vigencia", "<br /> <b> La Fecha Inicial debe ser menor o igual que la Fecha Final </b> <br /> ").Show();
                    return;
                }

                if (DateTime.Compare(laFechaFinal, DateTime.Now.Date) == 0)
                {
                    X.Msg.Notify("Cambio de Vigencia", "<br />  <b> La Fecha Final no puede ser igual a la actual </b> <br /> ").Show();
                    return;
                }               

                //Guardar Valores
                LNPromociones.ModificaVigenciaPromocion(dtCambios, this.Usuario);

                LlenaGridVigencias(int.Parse(this.cBoxPromocion.SelectedItem.Value),
                    int.Parse(this.cBoxDetalle.SelectedItem.Value));

                X.Msg.Notify("Cambio de Vigencia", "Cambio de Vigencia de la Promoción <br /><br />  <b> E X I T O S O </b> <br />  <br /> ").Show();
            }

            catch (Exception err)
            {
                DALLealtad.Utilidades.Loguear.Error(err, this.Usuario.ClaveUsuario);
                X.Msg.Notify("Cambio de Vigencia", "Cambio de Vigencia de la Promoción <br /><br />  <b> D E C L I N A D O </b> <br />  <br /> ").Show();
            }
        }

        /// <summary>
        /// Controla el evento Click al botón de Cancelar en el PropertyGrid
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directos del evento que se ejecutó</param>
        protected void btnCancelar_Click(object sender, DirectEventArgs e)
        {
            try
            {
                LlenaGridVigencias(int.Parse(this.cBoxPromocion.SelectedItem.Value),
                    int.Parse(this.cBoxDetalle.SelectedItem.Value));
            }

            catch (Exception ex)
            {
                DALLealtad.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                return;
            }
        }

        //public class VIGENCIA
        //{
        //    public int ID_FechaVigencia { get; set; }
        //    public string Descripcion { get; set; }
        //    public string Valor { get; set; }
        //    public bool EsFechaInicial { get; set; }
        //}


        //private List<VIGENCIA> GetData()
        //{
        //    return new List<VIGENCIA> 
        //    {
        //        new VIGENCIA { ID_FechaVigencia = 1, Descripcion = "Fecha Inicial", Valor = "2017-05-01 00:00:00.000", EsFechaInicial = true },
        //        new VIGENCIA { ID_FechaVigencia = 2, Descripcion = "Fecha Final", Valor = "2017-05-31 00:00:00.000", EsFechaInicial = false },
        //    };
        //}
    }
}