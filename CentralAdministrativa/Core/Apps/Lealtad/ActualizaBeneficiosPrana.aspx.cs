using DALAutorizador.Utilidades;
using DALCentralAplicaciones.Utilidades;
using DALLealtad.BaseDatos;
using DALLealtad.LogicaNegocio;
using Excel;
using Ext.Net;
using System;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;

namespace Lealtad
{
    public partial class ActualizaBeneficiosPrana : DALCentralAplicaciones.PaginaBaseCAPP
    {
        //Indice base 0, inicia archivo con ClabeCadena en col "A"
        const int COL_CLAVE_CADENA = 0;
        const int COL_CADENA = 1;
        const int COL_GIRO = 2;
        const int COL_CLAVE_PROMOCION = 3;
        const int COL_DESC_BENEFICIO = 6;
        const int COL_ES_HOTDEAL = 10;
        const int COL_CARRUSEL_HOME = 11;
        const int COL_PROMO_HOME = 12;
        const int COL_ORDEN = 13;
        const int COL_ACTIVA = 14;
        const int COL_FECHA_INICIO = 15;
        const int COL_FECHA_FIN = 16;
        const int COL_TWIST = 17;
        const int COL_TERRA = 18;
        const int COL_PURINA = 19;
        const int COL_EDENRED = 20;
        const int COL_SAMS_BENFITS = 21;
        const int COL_SAMS_PLUS = 22;
        const int COL_CUPON_CLICK = 23;
        const int COL_BOXITO = 24;
        const int COL_BROXEL = 25;
        const int COL_BIOBOX = 26;
        const int COL_ADVANTAGE = 27;
        const int COL_SIXTYNINE = 28;
        const int COL_BONNUS = 29;
        const int COL_SANTANDER_AFFLUENT = 30;
        const int COL_CC_ROYALTY = 31;
        const int COL_CC_BETS = 32;
        const int COL_BENFUL = 33;
        const int COL_EDOMEX = 34;
        const int COL_SMARTGIFT = 35;
        const int COL_BACALAR = 36;
        const int COL_MASPAMI = 37;
        const int COL_AIRPAK = 38;
        const int COL_PARCO = 39;
        const int COL_YOURPAYCHOICE = 40;

        const int COL_PRESENCIA = 41;
        const int COL_CLAVECLASIFICACION = 42;        
        const int COL_CTA_CLABE = 44;
        const int COL_NOMBRE_CONTACTO = 45;
        const int COL_TEL_CONTACTO = 46;
        const int COL_CARGO = 47;
        const int COL_CEL_CONTACTO = 48;
        const int COL_CORREO = 49;
        const int COL_EXTRACTO = 50;
        const int COL_SUBGIRO = 51;
        const int COL_TICKET_PROMEDIO = 51;
        const int COL_PERFIL_NSE = 53;
        const int COL_TIPO_ESTABLECIMIENTO = 59;                

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                }

                if (!X.IsAjaxRequest)
                {

                }
            }

            catch (Exception err)
            {
                DALLealtad.Utilidades.Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        public void btnCargarArchivo_Click(object sender, DirectEventArgs e)
        {
            try
            {                
                //   Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentCulture = new CultureInfo("es-MX");
                HttpPostedFile file = this.FileUploadField1.PostedFile;

                string[] directorios = file.FileName.Split('\\');
                string fileName = directorios[directorios.Count() - 1];

                //Se valida que se haya seleccionado un archivo
                if (String.IsNullOrEmpty(fileName))
                {
                    X.Msg.Alert("Cargar Archivo", "Selecciona un archivo para cargarlo.").Show();
                    return;
                }

                //Se valida que se haya seleccionado un archivo Excel
                if (!fileName.Contains(".xlsx"))
                {
                    X.Msg.Alert("Cargar Archivo", "El archivo seleccionado no es del formato Excel soportado (*.xlsx). Verifica tu archivo.").Show();
                    return;
                }

                string tempPath = "C:\\TmpXlsxFiles\\";

                //Si no existe el directorio, lo crea
                if (!Directory.Exists(tempPath))
                {
                    Directory.CreateDirectory(tempPath);
                }

                tempPath += fileName;

                //Se almacena archivo en directorio temporal del que es dueño el aplicativo
                //para no tener problemas de permisos de apertura
                file.SaveAs(tempPath);

                FileStream stream = File.Open(tempPath, FileMode.Open, FileAccess.Read);
                IExcelDataReader excelReader = null;

                //Reading from a OpenXml Excel file (2007 format; *.xlsx)
                if (fileName.Contains(".xlsx"))
                {
                    excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    //DataSet - Create column names from first row
                    excelReader.IsFirstRowAsColumnNames = true;
                }

                if (!String.IsNullOrEmpty(excelReader.ExceptionMessage))
                {
                    throw new Exception("IExcelDataReader exception: " + excelReader.ExceptionMessage);
                }

                DataSet dsArchivo = excelReader.AsDataSet();


                /* */
                DataTable dtCloned = dsArchivo.Tables[0].Clone();
                DataTable dtClonedShow = dsArchivo.Tables[0].Clone();//tabla a mostrar

                int NumColumnas = dsArchivo.Tables[0].Columns.Count;
                int NumColumnasLayout = int.Parse(Configuracion.Get(Guid.Parse(
                        ConfigurationManager.AppSettings["IDApplication"].ToString()),
                        "NumColumnasBeneficios").Valor.ToString());

                if (NumColumnas > NumColumnasLayout)
                {
                    for (int iCounter = NumColumnasLayout; iCounter < NumColumnas; iCounter++)
                    {
                        dtCloned.Columns.RemoveAt(iCounter);
                        dtClonedShow.Columns.RemoveAt(iCounter);
                    }
                }

                NumColumnas = dtCloned.Columns.Count;

                int val = 0;

                for (int i = 0; i < NumColumnas; i++)
                {
                    dtCloned.Columns[i].DataType = typeof(string);
                    dtClonedShow.Columns[i].ColumnName = dtCloned.Columns[i].ColumnName.ToUpper();
                    dtClonedShow.Columns[i].DataType = typeof(string);
                }

                foreach (DataRow row in dsArchivo.Tables[0].Rows)
                {
                    for (int i = 0; i < NumColumnas; i++)
                    {
                        if (string.IsNullOrEmpty(row.ItemArray[i].ToString()))
                        {
                            dtCloned.Columns[i].DataType = typeof(string);
                        }
                    }
                    val++;
                }


                foreach (DataRow row in dsArchivo.Tables[0].Rows)
                {
                    //Se eliminan espacios en blanco de más, 
                    //al inicio o al final, de todos los datos
                    for (int column = 0; column < NumColumnasLayout; column++)
                    {
                        row[column] = row[column].ToString().Trim();
                    }

                    dtCloned.ImportRow(row);
                    dtClonedShow.ImportRow(row);
                }

                dtCloned.Columns[COL_FECHA_INICIO].ColumnName = "FechaInicio";
                dtCloned.Columns[COL_FECHA_FIN].ColumnName = "FechaFin";

                dtClonedShow.Columns[COL_FECHA_INICIO].ColumnName = "FECHAINICIO(DD/MM/AAAA)";
                dtClonedShow.Columns[COL_FECHA_FIN].ColumnName = "FECHAFIN(DD/MM/AAAA)";

                //Se verifica la consistencia de información en las cadenas
                if (VerificaDatosCadenas(dtCloned))
                {
                    return;
                }

                //Se valida la longitud de los campos de tipo cadena
                if (VerificaLongitudCampos(dtCloned))
                {
                    return;
                }

                //Se verifica contenido y formato de cada columna
                if (VerificaFormatoDatosArchivo(dtCloned, NumColumnas))
                {
                    return;
                }

                //Se verifican los nombres y orden de las columnas de programas
                if (VerificaNombresColumnasProgramas(dtCloned))
                {
                    return;
                }

                //CASO ESPECIAL: Se da formato a los campos de fecha para mostrarlos en el grid
                EstableceFormatoFechas(dtClonedShow);
                
                dtCloned.Columns[COL_CLAVECLASIFICACION].ColumnName = "ClaveClasificacion";
                //El archivo pasó todas las validaciones
                dtCloned.AcceptChanges();

                LNEcommercePrana.InsertaArchivoTMP(dtCloned, this.Usuario);
                X.Msg.Notify("Cargar Archivo", "Archivo cargado correctamente.<br/>Selecciona Aplicar Cambios").Show();

                GridDatosArchivo.GetStore().DataSource = dtClonedShow;
                GridDatosArchivo.GetStore().DataBind();
            }

            catch (Exception ex)
            {
                DALLealtad.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Actualizar Beneficios", ex.Message).Show();
            }
        }

        /// <summary>
        /// Verifica que los datos de la cadena (clave, nombre y giro) sean siempre los mismos, 
        /// sin importar el número de promociones que se deseen añadir.
        /// </summary>
        /// <param name="_dtClonado">DataTable con la información del archivo Excel</param>
        /// <returns>TRUE si encuentran datos inconsistentes</returns>
        protected bool VerificaDatosCadenas(DataTable _dtClonado)
        {
            Boolean resultado = false;

            var clavesDuplicadas = _dtClonado.AsEnumerable()
               .Select(dr => dr.Field<string>("CLAVECADENA"))
               .GroupBy(x => x)
               .Where(g => g.Count() > 1)
               .Select(g => g.Key)
               .ToList();

            if (clavesDuplicadas.Count > 0)
            {
                int i = 0;
                int rowIndex;
                DataTable dtDuplicadas = new DataTable();

                dtDuplicadas.Columns.Add("ClaveCadena");
                dtDuplicadas.Columns.Add("Cadena");
                dtDuplicadas.Columns.Add("Giro");
                dtDuplicadas.Columns.Add("Presencia");
                dtDuplicadas.Columns.Add("SubGiro");
                dtDuplicadas.Columns.Add("CuentaCLABE");
                dtDuplicadas.Columns.Add("NombreContacto");
                dtDuplicadas.Columns.Add("TelefonoContacto");
                dtDuplicadas.Columns.Add("Cargo");
                dtDuplicadas.Columns.Add("CelularContacto");
                dtDuplicadas.Columns.Add("Correo");
                dtDuplicadas.Columns.Add("Extracto");
                dtDuplicadas.Columns.Add("TicketPromedio");
                dtDuplicadas.Columns.Add("PerfilNSE");
                dtDuplicadas.Columns.Add("TipoEstablecimiento");

                foreach (String clave in clavesDuplicadas)
                {
                    foreach (DataRow row in _dtClonado.Rows)
                    {
                        if (row.ItemArray[0].ToString() == clave)
                        {
                            dtDuplicadas.Rows.Add();

                            dtDuplicadas.Rows[i]["ClaveCadena"] = clave;
                            dtDuplicadas.Rows[i]["Cadena"] = row.ItemArray[COL_CADENA].ToString();
                            dtDuplicadas.Rows[i]["Giro"] = row.ItemArray[COL_GIRO].ToString();
                            dtDuplicadas.Rows[i]["Presencia"] = row.ItemArray[COL_PRESENCIA].ToString();
                            dtDuplicadas.Rows[i]["SubGiro"] = row.ItemArray[COL_SUBGIRO].ToString();
                            dtDuplicadas.Rows[i]["CuentaCLABE"] = row.ItemArray[COL_CTA_CLABE].ToString();
                            dtDuplicadas.Rows[i]["NombreContacto"] = row.ItemArray[COL_NOMBRE_CONTACTO].ToString();
                            dtDuplicadas.Rows[i]["TelefonoContacto"] = row.ItemArray[COL_TEL_CONTACTO].ToString();
                            dtDuplicadas.Rows[i]["Cargo"] = row.ItemArray[COL_CARGO].ToString();
                            dtDuplicadas.Rows[i]["CelularContacto"] = row.ItemArray[COL_CEL_CONTACTO].ToString();
                            dtDuplicadas.Rows[i]["Correo"] = row.ItemArray[COL_CORREO].ToString();
                            dtDuplicadas.Rows[i]["Extracto"] = row.ItemArray[COL_EXTRACTO].ToString();
                            dtDuplicadas.Rows[i]["TicketPromedio"] = row.ItemArray[COL_TICKET_PROMEDIO].ToString();
                            dtDuplicadas.Rows[i]["PerfilNSE"] = row.ItemArray[COL_PERFIL_NSE].ToString();
                            dtDuplicadas.Rows[i]["TipoEstablecimiento"] = row.ItemArray[COL_TIPO_ESTABLECIMIENTO].ToString();

                            i++;
                            break;
                        }
                    }
                }

                foreach (DataRow rowdup in dtDuplicadas.Rows)
                {
                    rowIndex = 2;

                    foreach (DataRow row in _dtClonado.Rows)
                    {
                        if (rowdup["ClaveCadena"].ToString() == row.ItemArray[COL_CLAVE_CADENA].ToString())
                        {
                            if (rowdup["Cadena"].ToString() != row.ItemArray[COL_CADENA].ToString())
                            {
                                X.Msg.Alert("Error de contenido en archivo excel",
                                   "El <b>Nombre de la Cadena</b> no es el mismo en todas las promociones con la Clave de Cadena <b>" + rowdup["ClaveCadena"].ToString() +
                                   ".</b> Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                                resultado = true;
                                break;
                            }
                            else if (rowdup["Giro"].ToString() != row.ItemArray[COL_GIRO].ToString())
                            {
                                X.Msg.Alert("Error de contenido en archivo excel",
                                   "El <b>Giro</b> no es el mismo en todas las promociones con Clave de Cadena  <b>" + rowdup["ClaveCadena"].ToString() +
                                   ".</b> Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                                resultado = true;
                                break;
                            }
                            else if (rowdup["Presencia"].ToString() != row.ItemArray[COL_PRESENCIA].ToString())
                            {
                                X.Msg.Alert("Error de contenido en archivo excel",
                                   "La <b>Presencia</b> no es la misma en todas las promociones con Clave de Cadena  <b>" + rowdup["ClaveCadena"].ToString() +
                                   ".</b> Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                                resultado = true;
                                break;
                            }
                            else if (rowdup["SubGiro"].ToString() != row.ItemArray[COL_SUBGIRO].ToString())
                            {
                                X.Msg.Alert("Error de contenido en archivo excel",
                                   "El <b>SubGiro</b> no es el mismo en todas las promociones con Clave de Cadena  <b>" + rowdup["ClaveCadena"].ToString() +
                                   ".</b> Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                                resultado = true;
                                break;
                            }

                            else if (rowdup["CuentaCLABE"].ToString() != row.ItemArray[COL_CTA_CLABE].ToString())
                            {
                                X.Msg.Alert("Error de contenido en archivo excel",
                                   "La <b>CuentaCLABE</b> no es la misma en todas las promociones con Clave de Cadena  <b>" + rowdup["ClaveCadena"].ToString() +
                                   ".</b> Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                                resultado = true;
                                break;
                            }

                            else if (rowdup["NombreContacto"].ToString() != row.ItemArray[COL_NOMBRE_CONTACTO].ToString())
                            {
                                X.Msg.Alert("Error de contenido en archivo excel",
                                   "El <b>NombreContacto</b> no es el mismo en todas las promociones con Clave de Cadena  <b>" + rowdup["ClaveCadena"].ToString() +
                                   ".</b> Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                                resultado = true;
                                break;
                            }

                            else if (rowdup["TelefonoContacto"].ToString() != row.ItemArray[COL_TEL_CONTACTO].ToString())
                            {
                                X.Msg.Alert("Error de contenido en archivo excel",
                                   "El <b>TelefonoContacto</b> no es el mismo en todas las promociones con Clave de Cadena  <b>" + rowdup["ClaveCadena"].ToString() +
                                   ".</b> Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                                resultado = true;
                                break;
                            }

                            else if (rowdup["Cargo"].ToString() != row.ItemArray[COL_CARGO].ToString())
                            {
                                X.Msg.Alert("Error de contenido en archivo excel",
                                   "El <b>Cargo</b> no es el mismo en todas las promociones con Clave de Cadena  <b>" + rowdup["ClaveCadena"].ToString() +
                                   ".</b> Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                                resultado = true;
                                break;
                            }

                            else if (rowdup["CelularContacto"].ToString() != row.ItemArray[COL_CEL_CONTACTO].ToString())
                            {
                                X.Msg.Alert("Error de contenido en archivo excel",
                                   "El <b>CelularContacto</b> no es el mismo en todas las promociones con Clave de Cadena  <b>" + rowdup["ClaveCadena"].ToString() +
                                   ".</b> Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                                resultado = true;
                                break;
                            }

                            else if (rowdup["Correo"].ToString() != row.ItemArray[COL_CORREO].ToString())
                            {
                                X.Msg.Alert("Error de contenido en archivo excel",
                                   "El <b>Correo</b> no es el mismo en todas las promociones con Clave de Cadena  <b>" + rowdup["ClaveCadena"].ToString() +
                                   ".</b> Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                                resultado = true;
                                break;
                            }

                            else if (rowdup["Extracto"].ToString() != row.ItemArray[COL_EXTRACTO].ToString())
                            {
                                X.Msg.Alert("Error de contenido en archivo excel",
                                   "El <b>Extracto</b> no es el mismo en todas las promociones con Clave de Cadena  <b>" + rowdup["ClaveCadena"].ToString() +
                                   ".</b> Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                                resultado = true;
                                break;
                            }

                            else if (rowdup["TicketPromedio"].ToString() != row.ItemArray[COL_TICKET_PROMEDIO].ToString())
                            {
                                X.Msg.Alert("Error de contenido en archivo excel",
                                   "El <b>TicketPromedio</b> no es el mismo en todas las promociones con Clave de Cadena  <b>" + rowdup["ClaveCadena"].ToString() +
                                   ".</b> Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                                resultado = true;
                                break;
                            }

                            else if (rowdup["PerfilNSE"].ToString() != row.ItemArray[COL_PERFIL_NSE].ToString())
                            {
                                X.Msg.Alert("Error de contenido en archivo excel",
                                   "El <b>PerfilNSE</b> no es el mismo en todas las promociones con Clave de Cadena  <b>" + rowdup["ClaveCadena"].ToString() +
                                   ".</b> Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                                resultado = true;
                                break;
                            }

                            else if (rowdup["TipoEstablecimiento"].ToString() != row.ItemArray[COL_TIPO_ESTABLECIMIENTO].ToString())
                            {
                                X.Msg.Alert("Error de contenido en archivo excel",
                                   "El <b>Tipo de Establecimiento</b> no es el mismo en todas las promociones con Clave de Cadena  <b>" + rowdup["ClaveCadena"].ToString() +
                                   ".</b> Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                                resultado = true;
                                break;
                            }
                        }

                        rowIndex++;
                    }

                    if (resultado) break;
                }
            }

            return resultado;
        }

        /// <summary>
        /// Valida que la longitud de los campos de tipo varchar no exceda el tamaño máximo
        /// del campo correspondiente en base de datos
        /// </summary>
        /// <param name="_dt">DataTable con la información del archivo Excel</param>
        /// <returns>TRUE si se excede la longitud en algún dato</returns>
        protected bool VerificaLongitudCampos(DataTable _dt)
        {
            bool validacion = false;
            int ClaveCadena = 0, Cadena = 0, ClavePromocion = 0, TituloPromocion = 0;
            int TipoDescuento = 0, DescripcionBeneficio = 0, RedesSociales = 0;
            int PalabrasClave = 0, CuentaCLABE = 0, NombreContacto = 0, TelefonoContacto = 0;
            int Cargo = 0, CelularContacto = 0, Correo = 0, Extracto = 0, TicketPromedio = 0, URLCupon = 0;
            int rowIndex = 2;

            try
            {
                DataSet dsLongitudes = DAOEcommercePrana.ObtieneLongitudCampos(this.Usuario);

                foreach (DataRow dr in dsLongitudes.Tables[0].Rows)
                {
                    switch (dr["NombreCampo"])
                    {
                        case "ClaveCadena": ClaveCadena = Convert.ToInt32(dr["LongitudCampo"]); break;
                        case "Cadena": Cadena = Convert.ToInt32(dr["LongitudCampo"]); break;
                        case "ClavePromocion": ClavePromocion = Convert.ToInt32(dr["LongitudCampo"]); break;
                        case "TituloPromocion": TituloPromocion = Convert.ToInt32(dr["LongitudCampo"]); break;
                        case "TipoDescuento": TipoDescuento = Convert.ToInt32(dr["LongitudCampo"]); break;
                        case "DescripcionBeneficio": DescripcionBeneficio = Convert.ToInt32(dr["LongitudCampo"]); break;
                        case "RedesSociales": RedesSociales = Convert.ToInt32(dr["LongitudCampo"]); break;
                        case "PalabrasClave": PalabrasClave = Convert.ToInt32(dr["LongitudCampo"]); break;
                        case "CuentaCLABE": CuentaCLABE = Convert.ToInt32(dr["LongitudCampo"]); break;
                        case "NombreContacto": NombreContacto = Convert.ToInt32(dr["LongitudCampo"]); break;
                        case "TelefonoContacto": TelefonoContacto = Convert.ToInt32(dr["LongitudCampo"]); break;
                        case "Cargo": Cargo = Convert.ToInt32(dr["LongitudCampo"]); break;
                        case "CelularContacto": CelularContacto = Convert.ToInt32(dr["LongitudCampo"]); break;
                        case "Correo": Correo = Convert.ToInt32(dr["LongitudCampo"]); break;
                        case "Extracto": Extracto = Convert.ToInt32(dr["LongitudCampo"]); break;
                        case "TicketPromedio": TicketPromedio = Convert.ToInt32(dr["LongitudCampo"]); break;
                        case "URLCupon": URLCupon = Convert.ToInt32(dr["LongitudCampo"]); break;
                    }
                }

                foreach (DataRow row in _dt.Rows)
                {
                    if (row["CLAVECADENA"].ToString().Length > ClaveCadena)
                    {
                        X.Msg.Alert("Error de contenido en archivo excel",
                                   "La longitud de la <b>Clave de Cadena</b> excede el número máximo permitido de " + ClaveCadena.ToString() +
                                   " caracteres. Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                        validacion = true;
                        break;
                    }
                    else if (row["CADENA"].ToString().Length > Cadena)
                    {
                        X.Msg.Alert("Error de contenido en archivo excel",
                                   "La longitud de la <b>Cadena</b> excede el número máximo permitido de " + Cadena.ToString() +
                                   " caracteres. Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                        validacion = true;
                        break;
                    }
                    else if (row["CLAVEPROMO"].ToString().Length > ClavePromocion)
                    {
                        X.Msg.Alert("Error de contenido en archivo excel",
                                   "La longitud de la <b>Clave de Promoción</b> excede el número máximo permitido de " + ClavePromocion.ToString() +
                                   " caracteres. Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                        validacion = true;
                        break;
                    }
                    else if (row["TITULOPROMOCION"].ToString().Length > TituloPromocion)
                    {
                        X.Msg.Alert("Error de contenido en archivo excel",
                                   "La longitud del <b>Título de la Promoción</b> excede el número máximo permitido de " + TituloPromocion.ToString() +
                                   " caracteres. Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                        validacion = true;
                        break;
                    }
                    else if (row["TIPODESCUENTO"].ToString().Length > TipoDescuento)
                    {
                        X.Msg.Alert("Error de contenido en archivo excel",
                                   "La longitud del <b>Tipo de Descuento</b> excede el número máximo permitido de " + TipoDescuento.ToString() +
                                   " caracteres. Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                        validacion = true;
                        break;
                    }
                    else if (row["DESCRIPCIONBENEFICIO"].ToString().Length > DescripcionBeneficio)
                    {
                        X.Msg.Alert("Error de contenido en archivo excel",
                                   "La longitud de la <b>Descripción del Beneficio</b> excede el número máximo permitido de " + DescripcionBeneficio.ToString() +
                                   " caracteres. Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                        validacion = true;
                        break;
                    }
                    else if (row["FACEBOOK"].ToString().Length > RedesSociales)
                    {
                        X.Msg.Alert("Error de contenido en archivo excel",
                                   "La longitud de la celda <b>Facebook</b> excede el número máximo permitido de " + RedesSociales.ToString() +
                                   " caracteres. Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                        validacion = true;
                        break;
                    }
                    else if (row["WEB"].ToString().Length > RedesSociales)
                    {
                        X.Msg.Alert("Error de contenido en archivo excel",
                                   "La longitud de la celda <b>Web</b> excede el número máximo permitido de " + RedesSociales.ToString() +
                                   " caracteres. Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                        validacion = true;
                        break;
                    }

                    else if (row["PALABRASCLAVE"].ToString().Length > PalabrasClave)
                    {
                        X.Msg.Alert("Error de contenido en archivo excel",
                                   "La longitud de la celda <b>PalabrasClave</b> excede el número máximo permitido de " + PalabrasClave.ToString() +
                                   " caracteres. Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                        validacion = true;
                        break;
                    }

                    else if (row["CUENTACLABE"].ToString().Length > CuentaCLABE)
                    {
                        X.Msg.Alert("Error de contenido en archivo excel",
                                   "La longitud de la celda <b>CuentaCLABE</b> excede el número máximo permitido de " + CuentaCLABE.ToString() +
                                   " caracteres. Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                        validacion = true;
                        break;
                    }

                    else if (row["NOMBRECONTACTO"].ToString().Length > NombreContacto)
                    {
                        X.Msg.Alert("Error de contenido en archivo excel",
                                   "La longitud de la celda <b>NombreContacto</b> excede el número máximo permitido de " + NombreContacto.ToString() +
                                   " caracteres. Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                        validacion = true;
                        break;
                    }

                    else if (row["TELEFONOCONTACTO"].ToString().Length > TelefonoContacto)
                    {
                        X.Msg.Alert("Error de contenido en archivo excel",
                                   "La longitud de la celda <b>Tel. Contacto</b> excede el número máximo permitido de " + TelefonoContacto.ToString() +
                                   " caracteres. Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                        validacion = true;
                        break;
                    }

                    else if (row["CARGO"].ToString().Length > Cargo)
                    {
                        X.Msg.Alert("Error de contenido en archivo excel",
                                   "La longitud de la celda <b>Cargo</b> excede el número máximo permitido de " + Cargo.ToString() +
                                   " caracteres. Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                        validacion = true;
                        break;
                    }

                    else if (row["CELULARCONTACTO"].ToString().Length > CelularContacto)
                    {
                        X.Msg.Alert("Error de contenido en archivo excel",
                                   "La longitud de la celda <b>Cel. Contacto</b> excede el número máximo permitido de " + CelularContacto.ToString() +
                                   " caracteres. Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                        validacion = true;
                        break;
                    }

                    else if (row["CORREO"].ToString().Length > Correo)
                    {
                        X.Msg.Alert("Error de contenido en archivo excel",
                                   "La longitud de la celda <b>Correo</b> excede el número máximo permitido de " + Correo.ToString() +
                                   " caracteres. Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                        validacion = true;
                        break;
                    }

                    else if (row["EXTRACTO"].ToString().Length > Extracto)
                    {
                        X.Msg.Alert("Error de contenido en archivo excel",
                                   "La longitud de la celda <b>Extracto</b> excede el número máximo permitido de " + Extracto.ToString() +
                                   " caracteres. Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                        validacion = true;
                        break;
                    }

                    else if (row["TICKETPROMEDIO"].ToString().Length > TicketPromedio)
                    {
                        X.Msg.Alert("Error de contenido en archivo excel",
                                   "La longitud de la celda <b>Ticket Promedio</b> excede el número máximo permitido de " + TicketPromedio.ToString() +
                                   " caracteres. Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                        validacion = true;
                        break;
                    }

                    else if (row["URLCUPON"].ToString().Length > URLCupon)
                    {
                        X.Msg.Alert("Error de contenido en archivo excel",
                                   "La longitud de la celda <b>URL Cupón</b> excede el número máximo permitido de " + URLCupon.ToString() +
                                   " caracteres. Fila no. <b>" + rowIndex.ToString() + "</b>").Show();
                        validacion = true;
                        break;
                    }
                    rowIndex++;
                }
            }

            catch (Exception ex)
            {
                DALLealtad.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Actualizar Beneficios", ex.Message).Show();
            }

            return validacion;
        }

        /// <summary>
        /// Verifica que el nombre y posición de las columnas de los programas sean
        /// los correctos en el archivo de entrada
        /// </summary>
        /// <param name="_dt">DataTable con la información del archivo Excel</param>
        /// <returns>TRUE si se encuentra alguna columna incorrecta</returns>
        protected bool VerificaNombresColumnasProgramas(DataTable _dt)
        {
            bool esIncorrecta = false;

            try
            {
                if (_dt.Columns[COL_TWIST].ToString() != "Twist")
                {
                    X.Msg.Alert("Error de contenido en archivo excel",
                                   "El programa que corresponde a la columna No. <b> " + COL_TWIST.ToString() + " </b> es <b>Twist</b>").Show();
                    esIncorrecta = true;
                }
                else if (_dt.Columns[COL_TERRA].ToString() != "Terra")
                {
                    X.Msg.Alert("Error de contenido en archivo excel",
                                   "El programa que corresponde a la columna No. <b> " + COL_TERRA.ToString() + " </b> es <b>Terra</b>").Show();
                    esIncorrecta = true;
                }
                else if (_dt.Columns[COL_PURINA].ToString() != "Purina")
                {
                    X.Msg.Alert("Error de contenido en archivo excel",
                                   "El programa que corresponde a la columna No. <b> " + COL_PURINA.ToString() + " </b> es <b>Purina</b>").Show();
                    esIncorrecta = true;
                }
                else if (_dt.Columns[COL_EDENRED].ToString() != "Edenred")
                {
                    X.Msg.Alert("Error de contenido en archivo excel",
                                   "El programa que corresponde a la columna No. <b> " + COL_EDENRED.ToString() + " </b> es <b>Edenred</b>").Show();
                    esIncorrecta = true;
                }
                else if (_dt.Columns[COL_SAMS_BENFITS].ToString() != "Sams_Benefits")
                {
                    X.Msg.Alert("Error de contenido en archivo excel",
                                   "El programa que corresponde a la columna No. <b> " + COL_SAMS_BENFITS.ToString() + " </b> es <b>Sams_Benefits</b>").Show();
                    esIncorrecta = true;
                }
                else if (_dt.Columns[COL_SAMS_PLUS].ToString() != "Sams_Plus")
                {
                    X.Msg.Alert("Error de contenido en archivo excel",
                                   "El programa que corresponde a la columna No. <b> " + COL_SAMS_PLUS.ToString() + " </b> es <b>Sams_Plus</b>").Show();
                    esIncorrecta = true;
                }
                else if (_dt.Columns[COL_CUPON_CLICK].ToString() != "CuponClick")
                {
                    X.Msg.Alert("Error de contenido en archivo excel",
                                   "El programa que corresponde a la columna No. <b> " + COL_CUPON_CLICK.ToString() + " </b> es <b>CuponClick</b>").Show();
                    esIncorrecta = true;
                }
                else if (_dt.Columns[COL_BOXITO].ToString() != "Boxito")
                {
                    X.Msg.Alert("Error de contenido en archivo excel",
                                   "El programa que corresponde a la columna No. <b> " + COL_BOXITO.ToString() + " </b> es <b>Boxito</b>").Show();
                    esIncorrecta = true;
                }
                else if (_dt.Columns[COL_BROXEL].ToString() != "Broxel")
                {
                    X.Msg.Alert("Error de contenido en archivo excel",
                                   "El programa que corresponde a la columna No. <b> " + COL_BROXEL.ToString() + " </b> es <b>Broxel</b>").Show();
                    esIncorrecta = true;
                }
                else if (_dt.Columns[COL_BIOBOX].ToString() != "BioBox")
                {
                    X.Msg.Alert("Error de contenido en archivo excel",
                                   "El programa que corresponde a la columna No. <b> " + COL_BIOBOX.ToString() + " </b> es <b>BioBox</b>").Show();
                    esIncorrecta = true;
                }
                else if (_dt.Columns[COL_ADVANTAGE].ToString() != "Advantage")
                {
                    X.Msg.Alert("Error de contenido en archivo excel",
                                   "El programa que corresponde a la columna No. <b> " + COL_ADVANTAGE.ToString() + " </b> es <b>Advantage</b>").Show();
                    esIncorrecta = true;
                }
                else if (_dt.Columns[COL_SIXTYNINE].ToString() != "Sixtynine")
                {
                    X.Msg.Alert("Error de contenido en archivo excel",
                                   "El programa que corresponde a la columna No. <b> " + COL_SIXTYNINE.ToString() + " </b> es <b>Sixtynine</b>").Show();
                    esIncorrecta = true;
                }
                else if (_dt.Columns[COL_BONNUS].ToString() != "Bonnus")
                {
                    X.Msg.Alert("Error de contenido en archivo excel",
                                   "El programa que corresponde a la columna No. <b> " + COL_BONNUS.ToString() + " </b> es <b>Bonnus</b>").Show();
                    esIncorrecta = true;
                }
                else if (_dt.Columns[COL_SANTANDER_AFFLUENT].ToString() != "Santander_Affluent")
                {
                    X.Msg.Alert("Error de contenido en archivo excel",
                                   "El programa que corresponde a la columna No. <b> " + COL_SANTANDER_AFFLUENT.ToString() + " </b> es <b>Santander_Affluent</b>").Show();
                    esIncorrecta = true;
                }
                else if (_dt.Columns[COL_CC_ROYALTY].ToString() != "CC_Royalty")
                {
                    X.Msg.Alert("Error de contenido en archivo excel",
                                   "El programa que corresponde a la columna No. <b> " + COL_CC_ROYALTY.ToString() + " </b> es <b>CC_Royalty</b>").Show();
                    esIncorrecta = true;
                }
                else if (_dt.Columns[COL_CC_BETS].ToString() != "CC_Bets")
                {
                    X.Msg.Alert("Error de contenido en archivo excel",
                                   "El programa que corresponde a la columna No. <b> " + COL_CC_BETS.ToString() + " </b> es <b>CC_Bets</b>").Show();
                    esIncorrecta = true;
                }
                else if (_dt.Columns[COL_BENFUL].ToString() != "Beneful")
                {
                    X.Msg.Alert("Error de contenido en archivo excel",
                                   "El programa que corresponde a la columna No. <b> " + COL_BENFUL.ToString() + " </b> es <b>Beneful</b>").Show();
                    esIncorrecta = true;
                }
                else if (_dt.Columns[COL_EDOMEX].ToString() != "EdoMex")
                {
                    X.Msg.Alert("Error de contenido en archivo excel",
                                   "El programa que corresponde a la columna No. <b> " + COL_EDOMEX.ToString() + " </b> es <b>EdoMex</b>").Show();
                    esIncorrecta = true;
                }
                else if (_dt.Columns[COL_SMARTGIFT].ToString() != "SmartGift")
                {
                    X.Msg.Alert("Error de contenido en archivo excel",
                                   "El programa que corresponde a la columna No. <b> " + COL_SMARTGIFT.ToString() + " </b> es <b>SmartGift</b>").Show();
                    esIncorrecta = true;
                }
                else if (_dt.Columns[COL_BACALAR].ToString() != "Bacalar")
                {
                    X.Msg.Alert("Error de contenido en archivo excel",
                                   "El programa que corresponde a la columna No. <b> " + COL_BACALAR.ToString() + " </b> es <b>Bacalar</b>").Show();
                    esIncorrecta = true;
                }
                else if (_dt.Columns[COL_MASPAMI].ToString() != "MasPaMi")
                {
                    X.Msg.Alert("Error de contenido en archivo excel",
                                   "El programa que corresponde a la columna No. <b> " + COL_MASPAMI.ToString() + " </b> es <b>MasPaMi</b>").Show();
                    esIncorrecta = true;
                }
                else if (_dt.Columns[COL_AIRPAK].ToString() != "AirPak")
                {
                    X.Msg.Alert("Error de contenido en archivo excel",
                                   "El programa que corresponde a la columna No. <b> " + COL_AIRPAK.ToString() + " </b> es <b>AirPak</b>").Show();
                    esIncorrecta = true;
                }
                else if (_dt.Columns[COL_PARCO].ToString() != "Parco")
                {
                    X.Msg.Alert("Error de contenido en archivo excel",
                                   "El programa que corresponde a la columna No. <b> " + COL_PARCO.ToString() + " </b> es <b>Parco</b>").Show();
                    esIncorrecta = true;
                }
                else if (_dt.Columns[COL_YOURPAYCHOICE].ToString() != "YourPayChoice")
                {
                    X.Msg.Alert("Error de contenido en archivo excel",
                                    "El programa que corresponde a la columna No. <b> " + COL_YOURPAYCHOICE.ToString() + " </b> es <b>YourPayChoice</b>").Show();
                    esIncorrecta = true;
                }
                }

            catch (Exception ex)
            {
                DALLealtad.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Actualizar Beneficios", ex.Message).Show();
            }

            return esIncorrecta;
        }



        /// <summary>
        /// Verifica el formato de los datos que corresponden a cada columna, así como aquellas que no deberían tener
        /// celdas en blanco. Si no se cumplen las validaciones, se muestra mensaje de error con la fila y columna
        /// en donde está el error.
        /// </summary>
        /// <param name="_dtFile">Datatable con las columnas del archivo</param>
        /// <param name="totalColumnas">Número total de columnas del archivo</param>
        /// <returns>TRUE en caso de error</returns>
        protected bool VerificaFormatoDatosArchivo(DataTable _dtFile, int totalColumnas)
        {
            int fila = 0;
            int columna = 0;
            bool error = false;
            int errorBlancos = 0;

            foreach (DataRow row in _dtFile.Rows)
            {
                string letra = ((char)((int)'A')).ToString();

                for (int i = 0; i < totalColumnas; i++)
                {
                    if (i <= COL_CLAVE_PROMOCION || i == COL_DESC_BENEFICIO)
                    {
                        if (string.IsNullOrEmpty(row[i].ToString()))
                        {
                            for (int f = fila; f < _dtFile.Rows.Count; f++)
                            {//este buscara que no haya blancos en las subsecuentes si hay hasta terminar el excel entonces es el final del documento y no debe marcar error

                                if (string.IsNullOrEmpty(_dtFile.Rows[f][0].ToString()) && string.IsNullOrEmpty(_dtFile.Rows[f][1].ToString()))
                                { }
                                else
                                {
                                    errorBlancos = 2;
                                    break;
                                }
                            }

                            if (errorBlancos == 2)
                            {
                                X.Msg.Alert("Error de contenido en archivo excel", "La celda de la fila no.  <b>" + (fila + 2)
                                    + "</b>, columna " + letra + " <b>(" + _dtFile.Columns[i].ColumnName
                                    + ")</b> no puede estar vacía.").Show();
                                error = true;
                                break;
                            }
                            else
                            {
                                errorBlancos = 1;
                                break;
                            }
                        }
                    }

                    else if (i == COL_ES_HOTDEAL || i == COL_ACTIVA)
                    {
                        if (row[i].ToString().ToUpper() == "SI" || row[i].ToString().ToUpper() == "NO"
                            || row[i].ToString().ToUpper().Equals("SÍ"))
                        {
                            if (row[i].ToString().ToUpper() == "SI" || row[i].ToString().ToUpper().Equals("SÍ"))
                            {
                                row[i] = "1";
                            }
                            else
                            {
                                row[i] = "0";
                            }
                        }
                        else
                        {
                            try
                            {
                                int celda = int.Parse(row[i].ToString());
                                if (celda != 0 && celda != 1)
                                {
                                    X.Msg.Alert("Error de contenido en archivo excel", "La celda de la fila no.  <b>" + (fila + 2)
                                        + "</b>, columna " + letra + " <b>(" + _dtFile.Columns[i].ColumnName
                                        + ")</b> debe ser numérica (1 ó 0) o en su defecto, la palabra SI o NO").Show();
                                    error = true;
                                    break;
                                }

                            }
                            catch (Exception)
                            {
                                X.Msg.Alert("Error de contenido en archivo excel", "La celda de la fila no.  <b>" + (fila + 2)
                                        + "</b>, columna " + letra + " <b>(" + _dtFile.Columns[i].ColumnName
                                        + ")</b> debe ser numérica (1 ó 0) o en su defecto, la palabra SI o NO").Show();
                                error = true;
                                break;
                            }

                        }
                    }

                    else if (i == COL_CARRUSEL_HOME || i == COL_PROMO_HOME)
                    {
                        row[i] = string.IsNullOrEmpty(row[i].ToString()) ? "0" : row[i];

                        if (row[i].ToString().ToUpper() == "NO")
                        {
                            row[i] = "0";
                        }

                        else
                        {
                            try
                            {
                                int celda = int.Parse(row[i].ToString());
                            }
                            catch (Exception)
                            {
                                X.Msg.Alert("Error de contenido en archivo excel", "La celda de la fila no.  <b>" + (fila + 2)
                                    + "</b>, columna " + letra + " <b>(" + _dtFile.Columns[i].ColumnName
                                    + ")</b> debe ser numérica o en su defecto, la palabra NO").Show();
                                error = true;
                                break;
                            }

                        }
                    }

                    else if (i == COL_ORDEN)
                    {
                        row[i] = string.IsNullOrEmpty(row[i].ToString()) ? "0" : row[i];

                        try
                        {
                            int celda = int.Parse(row[i].ToString());
                        }
                        catch (Exception)
                        {
                            X.Msg.Alert("Error de contenido en archivo excel", "La celda de la fila no.  <b>" + (fila + 2)
                                + "</b>, columna " + letra + " <b>(" + _dtFile.Columns[i].ColumnName
                                + ")</b> debe ser numérica").Show();
                            error = true;
                            break;
                        }
                    }

                    else if (i == COL_FECHA_INICIO || i == COL_FECHA_FIN)
                    {
                        try
                        {
                            string cadenaorig = row[i].ToString();
                            string[] partes = cadenaorig.Split(' ');
                            string subcadena = String.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(partes[0]));

                            DateTime fecha = DateTime.ParseExact(subcadena, "dd/MM/yyyy", CultureInfo.GetCultureInfo("es-MX").DateTimeFormat);
                            row[i] = fecha.ToString("yyyy/MM/dd HH:mm:ss");
                        }
                        catch (Exception)
                        {
                            X.Msg.Alert("Error de contenido en archivo excel", "La celda de la fila no.  <b>" + (fila + 2)
                                + "</b>, columna " + letra + " <b>(" + _dtFile.Columns[i].ColumnName
                                + ")</b>, debe ser una fecha válida en formato(dd/mm/aaaa)").Show();
                            error = true;
                            break;
                        }
                    }

                    else if (i >= COL_TWIST && i <= COL_YOURPAYCHOICE)
                    {
                        if (!String.IsNullOrEmpty(row[i].ToString()))
                        {
                            if (row[i].ToString().ToUpper() == "SI" || row[i].ToString().ToUpper() == "NO"
                            || row[i].ToString().ToUpper().Equals("SÍ"))
                            {
                                if (row[i].ToString().ToUpper() == "SI" || row[i].ToString().ToUpper().Equals("SÍ"))
                                {
                                    row[i] = "1";
                                }
                                else
                                {
                                    row[i] = "0";
                                }
                            }
                            else
                            {
                                X.Msg.Alert("Error de contenido en archivo excel", "La celda de la fila no.  <b>" + (fila + 2)
                                        + "</b>, columna <b>" + _dtFile.Columns[i].ColumnName
                                        + "</b> debe ser la palabra SI o NO").Show();
                                error = true;
                                break;
                            }
                        }
                        else
                        {
                            row[i] = "0";
                        }
                            
                    }

                    else if (i >= COL_PRESENCIA && i <= COL_CLAVECLASIFICACION)
                    {
                        if (string.IsNullOrEmpty(row[i].ToString()))
                        {
                            X.Msg.Alert("Error de contenido en archivo excel", "La celda de la fila no.  <b>" + (fila + 2)
                                + "</b>, columna <b>" + _dtFile.Columns[i].ColumnName
                                + "</b> no puede estar vacía.").Show();
                            error = true;
                            break;
                        }
                    }

                    columna++;
                    letra = ((char)((int)letra[0] + 1)).ToString();
                }

                if (error == true)
                {
                    break;
                }
                else
                {
                    fila++;
                }

                if (errorBlancos == 1)
                {
                    for (int i = fila - 1; i < _dtFile.Rows.Count; i++)
                    {
                        DataRow dr = _dtFile.Rows[i];
                        dr.Delete();
                    }
                    break;
                }
            }

            return error;
        }

        /// <summary>
        /// Establece formato yyyy/MM/dd HH:mm:ss a los campos de fecha del DataTable
        /// que se mostrará en el grid
        /// </summary>
        /// <param name="_dtGrid">Objeto con los datos a mostrar en el grid</param>
        protected void EstableceFormatoFechas(DataTable _dtGrid)
        {
            string cadenaorig, subcadena;
            string[] partes;

            foreach (DataRow row in _dtGrid.Rows)
            {
                foreach (DataColumn dc in _dtGrid.Columns)
                {
                    if (dc.ColumnName.Contains("FECHAINICIO") || dc.ColumnName.Contains("FECHAFIN"))
                    {
                        cadenaorig = row[dc].ToString();
                        partes = cadenaorig.Split(' ');
                        subcadena = String.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(partes[0]));

                        DateTime fecha = DateTime.ParseExact(subcadena, "dd/MM/yyyy",
                            CultureInfo.GetCultureInfo("es-MX").DateTimeFormat);
                        row[dc] = fecha.ToString("yyyy/MM/dd HH:mm:ss");
                    }
                }
            }
        }

        /// <summary>
        /// Controla el evento Click al botón Aplicar Cambios del Grid principal
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos directo del evento que se ejecutó</param>
        public void btnAplicarCambios_Click(object sender, DirectEventArgs e)
        {
            try
            {
                var response = LNEcommercePrana.AplicaCambios(this.Usuario);
                if (response.Contains("Error"))
                {
                    X.Msg.Alert("Actualizar Beneficios", response + "<br /> ").Show();
                  //  Loguear.Error(ex, this.Usuario.ClaveUsuario);

                }
                else
                {
                    X.Msg.Alert("Actualizar Beneficios", response + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();
                }
              
            }
            
            catch (Exception ex)
            {
                DALLealtad.Utilidades.Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Actualizar Beneficios", "Ocurrió un error al aplicar los cambios en base de datos.").Show();
            }
        }
    }
}