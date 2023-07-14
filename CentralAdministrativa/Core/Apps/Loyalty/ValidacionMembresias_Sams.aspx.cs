using CentroContacto.LogicaNegocio;
using ClosedXML.Excel;
using DALCentralAplicaciones.Utilidades;
using DALCentroContacto.BaseDatos;
using DALCentroContacto.Entidades;
using DALCentroContacto.LogicaNegocio;
using Ext.Net;
using Interfases.Exceptiones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using WebServices.Entidades;
using WebServices;


namespace CentroContacto
{
    public partial class ValidacionMembresias_Sams : DALCentralAplicaciones.PaginaBaseCAPP
    {
        /// <summary>
        /// Controla el evento Click al botón Validar del formulario invocando la validación
        /// de la membresia en WS
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnValidar_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtMembresia.Text == "") {
                    X.Msg.Alert("Validación", "La membresia no puede estar vacía.").Show();
                    return;
                }
                
                limpiaBusquedaPrevia();

                RespuestasJSON.ValidaMembresiaSams wsLoginResp;
                Parametros.BodyValidarMembresiaSams _body = new Parametros.BodyValidarMembresiaSams();

                _body.WsUsuario = Configuracion.Get(Guid.Parse(
                                        ConfigurationManager.AppSettings["IDApplication"].ToString().Trim()),
                                        "WsSams_Usr").Valor;

                _body.WsPassword = Configuracion.Get(Guid.Parse(
                                        ConfigurationManager.AppSettings["IDApplication"].ToString().Trim()),
                                        "WsSams_Pwd").Valor;

                var URL = Configuracion.Get(Guid.Parse(
                                ConfigurationManager.AppSettings["IDApplication"].ToString().Trim()),
                                "WsSams_URL").Valor;

                _body.NumeroMembresia = txtMembresia.Text;
                wsLoginResp = Lealtad.ValidarMembresiaSams(URL, _body, this.Usuario);

                if (wsLoginResp == null)
                {
                    X.Msg.Alert("Validación", "Ocurrió un error al intentar validar la membresia.").Show();
                    
                }
                else
                {
                    string nivel = "";

                    txtCodigoRespuesta.Text = wsLoginResp.CodigoRespuesta;
                    txtDescripcion.Text = wsLoginResp.Descripcion;

                    if (wsLoginResp.MembresiaSams != null) {
                        if (wsLoginResp.MembresiaSams.TierType == 1)
                        {
                            nivel = "Clásica";
                        }
                        else if (wsLoginResp.MembresiaSams.TierType == 2)
                        {
                            nivel = "Benefits";
                        }
                        else if (wsLoginResp.MembresiaSams.TierType == 3)
                        {
                            nivel = "Plus";
                        }
                        
                        txtNivel.Text = nivel;
                        dtFechaVencimiento.Text = wsLoginResp.MembresiaSams.EndDate;
                        txtAsociado.Text = wsLoginResp.MembresiaSams.QualifyOrg.Equals(101) ? "Si" : "No";
                    }
                }
            }

            catch (Exception ex)
            {
                X.Msg.Alert("Validación", ex.Message).Show();
            }
        }
        
        /// <summary>
        /// Limpia los controles de la búsqueda previa
        /// </summary>
        protected void limpiaBusquedaPrevia()
        {
            txtCodigoRespuesta.Text = "";
            txtDescripcion.Text = "";
            txtNivel.Text = "";
            dtFechaVencimiento.Text = "";
            txtAsociado.Text = "";
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar del formulario de validación 
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            limpiaBusquedaPrevia();
            txtMembresia.Text = "";
        }
    }
}