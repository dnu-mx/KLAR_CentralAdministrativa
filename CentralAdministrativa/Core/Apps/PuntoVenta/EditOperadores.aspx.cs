﻿using DALAutorizador.BaseDatos;
using DALAutorizador.Entidades;
using DALAutorizador.LogicaNegocio;
using DALAutorizador.Utilidades;
using DALCentralAplicaciones;
using DALCentralAplicaciones.Entidades;
using Ext.Net;
using Framework;
using Interfases.Exceptiones;
using System;
using System.Configuration;
using System.Data;
using System.Web;

namespace TpvWeb
{
    public partial class EditOperadores : PaginaBaseCAPP
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (ContextoInicial.ContextoServicio == null)
                {
                    ContextoInicial.InicializarContexto();
                }

                PreparaGridColectiva();
            }
            catch (Exception err)
            {
                Loguear.Error(err, "");
                Response.Redirect("../ErrorInicializarPagina.aspx");
            }
        }

        protected void RefreshGrid(object sender, StoreRefreshDataEventArgs e)
        {
            try
            {
                PreparaGridColectiva();
                BindDataColectiva();
            }
            catch (Exception )
            {
            }

        }

        private void AgregaRecordFilesColectiva()
        {
            Store2.AddField(new RecordField("ID_Colectiva"));
            Store2.AddField(new RecordField("ClaveColectiva"));
            Store2.AddField(new RecordField("NombreORazonSocial"));
            Store2.AddField(new RecordField("APaterno"));
            Store2.AddField(new RecordField("AMaterno"));
            Store2.AddField(new RecordField("RFC"));
            Store2.AddField(new RecordField("Email"));
            Store2.AddField(new RecordField("Estatus"));
            Store2.AddField(new RecordField("ID_EstatusColectiva"));
            Store2.AddField(new RecordField("Clave"));
            Store2.AddField(new RecordField("Descripcion"));
            Store2.AddField(new RecordField("ColectivaPadre"));
            Store2.AddField(new RecordField("TipoColectiva"));
        }

        protected void LlenaAfiliaciones()
        {
            try
            {
                SAfiliaciones.DataSource = DAOCatalogos.ListaAfiliaciones(this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()));
                SAfiliaciones.DataBind();
            }
            catch (Exception )
            {
            }
        }

        protected void PreparaGridColectiva()
        {
            //LIMPIA GRID
            //PREPARAR CONEXION A DATOS

            AgregaRecordFilesColectiva();


            //AGREGADO DE COLUMNAS
            ColumnModel lasColumnas = new ColumnModel();

            Column colID_Colectiva = new Column();
            colID_Colectiva.DataIndex = "ID_Colectiva";
            colID_Colectiva.Header = "ID";
            colID_Colectiva.Sortable = true;
            colID_Colectiva.Hidden = true;
            lasColumnas.Columns.Add(colID_Colectiva);

            Column ColEstatus = new Column();
            ColEstatus.DataIndex = "Estatus";
            ColEstatus.Header = "Estatus ";
            ColEstatus.Sortable = true;
            lasColumnas.Columns.Add(ColEstatus);

            Column colClaveColectiva = new Column();
            colClaveColectiva.DataIndex = "ClaveColectiva";
            colClaveColectiva.Header = "Clave";
            colClaveColectiva.Sortable = true;
            lasColumnas.Columns.Add(colClaveColectiva);

            Column colTipoColectiva = new Column();
            colTipoColectiva.DataIndex = "Descripcion";
            colTipoColectiva.Header = "Tipo Colectiva";
            colTipoColectiva.Sortable = true;
            lasColumnas.Columns.Add(colTipoColectiva);

            Column colNombreORazonSocial = new Column();
            colNombreORazonSocial.DataIndex = "NombreORazonSocial";
            colNombreORazonSocial.Header = "Nombre";
            colNombreORazonSocial.Sortable = true;
            lasColumnas.Columns.Add(colNombreORazonSocial);

            Column colAPaterno = new Column();
            colAPaterno.DataIndex = "APaterno";
            colAPaterno.Header = "Apellido Paterno";
            colAPaterno.Sortable = true;
            lasColumnas.Columns.Add(colAPaterno);

            Column colAMaterno = new Column();
            colAMaterno.DataIndex = "AMaterno";
            colAMaterno.Header = "Apellido Materno";
            colAMaterno.Sortable = true;
            lasColumnas.Columns.Add(colAMaterno);

            Column colRFC = new Column();
            colRFC.DataIndex = "RFC";
            colRFC.Header = "RFC";
            colRFC.Hidden = true;
            colRFC.Sortable = true;
            lasColumnas.Columns.Add(colRFC);

            Column colEmail = new Column();
            colEmail.DataIndex = "Email";
            colEmail.Header = "Email";
            colEmail.Hidden = true;
            colEmail.Sortable = true;
            lasColumnas.Columns.Add(colEmail);

            Column ColColectivaPadre = new Column();
            ColColectivaPadre.DataIndex = "ColectivaPadre";
            ColColectivaPadre.Header = "Colectiva Padre";
            ColColectivaPadre.Sortable = true;
            lasColumnas.Columns.Add(ColColectivaPadre);


            //AGREGAR COLUMNAS
            CommandColumn acciones = new CommandColumn();

            acciones.Header = "Acciones";
            acciones.Width = 80;
            acciones.PrepareToolbar.Fn = "prepareToolbar";

            GridCommand AddCuenta = new GridCommand();
            AddCuenta.Icon = Icon.MoneyAdd;
            AddCuenta.CommandName = "AddCuenta";
            AddCuenta.ToolTip.Text = "Crear Cuenta Individual";
            acciones.Commands.Add(AddCuenta);


            CommandSeparator separa = new CommandSeparator();
            acciones.Commands.Add(separa);


            GridCommand AddMP = new GridCommand();
            AddMP.Icon = Icon.Creditcards;
            AddMP.CommandName = "AddMedioPago";
            AddMP.ToolTip.Text = "Asignar Medios de Pago";
            acciones.Commands.Add(AddMP);

            CommandSeparator separa2 = new CommandSeparator();
            acciones.Commands.Add(separa);

            GridCommand Bloquear = new GridCommand();
            Bloquear.Icon = Icon.Lightbulb;
            Bloquear.CommandName = "Bloquear";
            Bloquear.ToolTip.Text = "Inactivar Operador";
            acciones.Commands.Add(Bloquear);


            GridCommand play = new GridCommand();
            play.Icon = Icon.LightbulbOff;
            play.CommandName = "Activar";
            play.ToolTip.Text = "Activar Operador";
            acciones.Commands.Add(play);

            CommandSeparator separa3 = new CommandSeparator();
            acciones.Commands.Add(separa3);

            GridCommand colCambioPass = new GridCommand();
            colCambioPass.Icon = Icon.KeyAdd;
            colCambioPass.CommandName = "CambioPass";
            colCambioPass.ToolTip.Text = "Marcar Cambio de contraseña al Operador";
            acciones.Commands.Add(colCambioPass);

            GridPanel1.ColumnModel.Columns.Add(acciones);
            GridPanel1.ColumnModel.Columns.Add(colID_Colectiva);
            GridPanel1.ColumnModel.Columns.Add(ColEstatus);
            GridPanel1.ColumnModel.Columns.Add(colTipoColectiva);
            GridPanel1.ColumnModel.Columns.Add(colClaveColectiva);

            GridPanel1.ColumnModel.Columns.Add(colNombreORazonSocial);
            GridPanel1.ColumnModel.Columns.Add(colAPaterno);
            GridPanel1.ColumnModel.Columns.Add(colAMaterno);
            GridPanel1.ColumnModel.Columns.Add(colRFC);
            GridPanel1.ColumnModel.Columns.Add(colEmail);
            GridPanel1.ColumnModel.Columns.Add(ColColectivaPadre);

            //AGREGAR EVENTOS
            GridPanel1.DirectEvents.RowDblClick.ExtraParams.Add(new Ext.Net.Parameter("ID_Colectiva", "this.getRowsValues({ selectedOnly: true })[0].ID_Colectiva", ParameterMode.Raw));
            GridPanel1.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("ID_Colectiva", "record.data.ID_Colectiva", ParameterMode.Raw));
            GridPanel1.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("ClaveColectiva", "record.data.ClaveColectiva", ParameterMode.Raw));
            GridPanel1.DirectEvents.Command.ExtraParams.Add(new Ext.Net.Parameter("Comando", "command", ParameterMode.Raw));

            GridFilters losFiltros = new GridFilters();



            StringFilter filClaveColectiva = new StringFilter();
            filClaveColectiva.DataIndex = "ClaveColectiva";
            losFiltros.Filters.Add(filClaveColectiva);

            StringFilter filNombreORazonSocial = new StringFilter();
            filNombreORazonSocial.DataIndex = "NombreORazonSocial";
            losFiltros.Filters.Add(filNombreORazonSocial);

            StringFilter filAPaterno = new StringFilter();
            filAPaterno.DataIndex = "APaterno";
            losFiltros.Filters.Add(filAPaterno);

            StringFilter filAMaterno = new StringFilter();
            filAMaterno.DataIndex = "AMaterno";
            losFiltros.Filters.Add(filAMaterno);

            StringFilter filRFC = new StringFilter();
            filRFC.DataIndex = "RFC";
            losFiltros.Filters.Add(filRFC);

            StringFilter filEmail = new StringFilter();
            filEmail.DataIndex = "Email";
            losFiltros.Filters.Add(filEmail);

            GridPanel1.Plugins.Add(losFiltros);

        }

        protected void EjecutarComando(object sender, DirectEventArgs e)
        {

            try
            {
                Int64 ID_Colectiva = Int64.Parse(e.ExtraParams["ID_Colectiva"]);
                String EjecutarComando = (String)e.ExtraParams["Comando"];
                String ClaveUsuario = (String)e.ExtraParams["ClaveColectiva"];

                if (ID_Colectiva == 0)
                {
                    throw new CAppException(8009, "Por favor, Selecciona el Registro al que deseas aplicar el comando <b> " + EjecutarComando + "</b>");
                }
                Usuario elUser = this.Usuario;


                switch (EjecutarComando)
                {
                    case "CambioPass":
                        if (LNColectiva.MarcarCambioPassWord(ClaveUsuario, this.Usuario) != 0)
                        {
                            throw new Exception("No se Cambio el Estatus del Operador");
                        }
                        break;
                    case "Bloquear":
                        //if (LNColectiva.DesactivaColectiva(ID_Colectiva, this.Usuario) != 0)
                        //{
                        //    throw new Exception("No se Desactivo la Afiliación");
                        //}
                        //break;
                          if (LNColectiva.DesactivaColectivaOperadora(ID_Colectiva, ClaveUsuario, this.Usuario) != 0)
                        {
                            throw new Exception("No se Desactivo el Operador");
                        }
                        break;
                    case "Activar":
                        if (DALAutorizador.LogicaNegocio.LNColectiva.ActivaColectiva(ID_Colectiva, this.Usuario) != 0)
                        {
                            throw new Exception("No se Desactivo la Afiliación");
                        }

                        if (LNColectiva.ActivaColectivaOperadora(ID_Colectiva, ClaveUsuario, this.Usuario) != 0)
                        {
                            throw new Exception("No se Desactivo el Operador");
                        }

                        break;
                }

                PreparaGridColectiva();
                this.BindDataColectiva();

                X.Msg.Notify("Operadora Web", "Comando ejecutado con <br />  <br /> <b> E X I T O </b> <br />  <br /> ").Show();
            }
            catch (Interfases.Exceptiones.CAppException err)
            {
                X.Msg.Alert("Operadores Web", err.Mensaje()).Show();
            }
            catch (Exception )
            {
                X.Msg.Alert("Operadores Web", "Ocurrio un Error al Ejecutar la Acción Seleccionada").Show();
            }

        }


        /// <summary>
        /// 
        /// </summary>
        private void BindDataColectiva()
        {
            try
            {
                GridPanel1.GetStore().RemoveAll();

                DataSet dsOperadores = DAOCatalogos.ListaOperadoresPorTipoColectiva
                    (this.txtBuscarNombre.Text, this.txtBuscarPaterno.Text, this.txtBuscarUsuario.Text,
                    this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()),
                    DALCentralAplicaciones.Utilidades.Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()), "ClaveTipoColectivaOperador").Valor);

                if (dsOperadores.Tables[0].Rows.Count <= 0)
                {
                    X.Msg.Alert("Consulta", "La Búsqueda no Devolvió Operadores").Show();
                }
                else
                {
                    GridPanel1.GetStore().DataSource = dsOperadores;
                    GridPanel1.GetStore().DataBind();
                }
            }
            catch (CAppException caEx)
            {
                Loguear.Error(caEx, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Búsqueda de Operadores", "Ocurrió un Error en la Consulta de Operadores").Show();
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, this.Usuario.ClaveUsuario);
                X.Msg.Alert("Búsqueda de Operadores", ex.Message).Show();
            }
        }


        protected void QuitarSeleccion(object sender, DirectEventArgs e)
        {

            FormPanel1.Collapsed = true;
        }

        protected void GridEmpleados_DblClik(object sender, DirectEventArgs e)
        {
            try
            {

                String unvalor = e.ExtraParams["ID_Colectiva"];

                // Panel5.Title = "Colectiva  No. [" + unvalor + "]";

                LlenaTiposColectiva();

                Int64 ID_colectiva = Int64.Parse(unvalor);
                DespliegaDatosColectiva(ID_colectiva);



            }
            catch (Exception )
            {
            }

            //FichaDeposito nuevaFicha = DAOFichaDeposito.ConsultaFichaDeposito(IdFicha, new IUsuario());

            //DAOFichaDeposito.ConsultaFichaDeposito(e.
            FormPanel1.Collapsed = false;
        }

        protected void RowSelect(object sender, DirectEventArgs e)
        {
            //EastPanel.Collapsed = false;


        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                Colectiva laColectiva = new Colectiva();

                laColectiva = AsignarNuevosDatos(laColectiva);

                if (0 == DAOColectiva.Modificar(laColectiva, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString())))
                {
                    if (!txtPass1.Text.Trim().Equals(""))
                    {
                        Cliente BDCliente = Cliente.GetDesdeBaseDatos(laColectiva.ClaveColectiva);

                        if (BDCliente == null)
                        {
                            throw new Exception("No es posible cambiar la contraseña del Operador seleccionado.-No Existe en Cubos");
                        }

                        //actualiza el Password en DB
                        Cliente.editarPasswordDesdeBaseDatos(laColectiva.Password, laColectiva.ClaveColectiva);

                        //Cambia el Estatus a 2
                        Cliente.ActualizaEstatus(laColectiva.ClaveColectiva, 2);
                    }

                    X.Msg.Notify("", "Operador Editado" + "<br />  <br /> <b> E X I T O S A M E N T E </b> <br />  <br /> ").Show();

                    FormPanel1.Reset();
                    FormPanel1.Collapsed = true;

                    BindDataColectiva();
                }
                else
                {
                    throw new Exception("Ocurrio un Error al Modificar, Intentalo nuevamente");
                }

            }
            catch (Exception err)
            {
                X.Msg.Alert("Actualizar Colectiva", "Ocurrio un Error:" + err.Message).Show();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            FormPanel1.Collapsed = true;
        }

        /// <summary>
        /// Controla el evento Click al botón Limpiar, limpiando todos los controles de la página
        /// </summary>
        /// <param name="sender">Objeto que envía el control</param>
        /// <param name="e">Argumentos del evento que se ejecutó</param>
        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            this.txtBuscarUsuario.Clear();
            this.txtBuscarNombre.Clear();
            this.txtBuscarPaterno.Clear();

            GridPanel1.GetStore().RemoveAll();

            FormPanel1.Collapsed = true;
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                FormPanel1.Reset();
                BindDataColectiva();
                FormPanel1.Collapsed = true;
            }

            catch (Exception err)
            {
                X.Msg.Alert("Buscar Colectiva", "Ocurrio un Error:" + err.Message).Show();
            }
        }

        protected void TipoColectiva_Select(object sender, DirectEventArgs e)
        {
            try
            {
                cmbPadreTipoColectiva.Clear();
                LlenaColectivasPosiblePadre(2, cmbTipoUsuario.Value.ToString().Trim());

            }
            catch (Exception )
            {
            }
        }

        protected void LlenaTiposColectiva()
        {
            try
            {
                STipoColectiva.DataSource = DAOCatalogos.ListaTiposColectiva((this.Usuario).ID_Colectiva, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                STipoColectiva.DataBind();

            }
            catch (Exception )
            {
            }
        }

        protected void LlenaColectivasPosiblePadre(Int64 ID_Padre, String ClaveTipoColectivaHijo)
        {
            try
            {
                SCPadre.DataSource = DAOCatalogos.ListaColectivaPosiblePadre(ID_Padre, ClaveTipoColectivaHijo, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
                SCPadre.DataBind();

            }
            catch (Exception )
            {
            }
        }

        protected void DespliegaDatosColectiva(Int64 ID_Colectiva)
        {
            Colectiva laColectivaEditar = DAOColectiva.ObtenerColectiva(ID_Colectiva, this.Usuario, Guid.Parse(ConfigurationManager.AppSettings["IdApplication"].ToString()));
            FormPanel1.Title = "Editando al Operador Número [" + laColectivaEditar.ID_Colectiva + "]";
            //TabPanel1.SetActiveTab(Panel1);

            HttpContext.Current.Session.Add("Colectiva", laColectivaEditar);

            txtUserName.Text = laColectivaEditar.ClaveColectiva;
            txtPass1.Text = "";
            txtPass2.Text = "";

            cmbTipoUsuario.SetValue(laColectivaEditar.TipoColectiva.Clave);
            //cmbTipoUsuario.Text = laColectivaEditar.TipoColectiva.Descripcion;

            //TODO: CAmbiar el fijo por el Padre definido al usuario Logueado
            LlenaColectivasPosiblePadre(2, laColectivaEditar.TipoColectiva.Clave);

            cmbPadreTipoColectiva.SetValue(laColectivaEditar.ID_ColectivaPadre);

            txtCURP.Text = laColectivaEditar.CURP;
            txtemail2.Text = laColectivaEditar.Email;
            txtFijo.Text = laColectivaEditar.Telefono;
            txtMovil.Text = laColectivaEditar.Movil;
            txtNombre.Text = laColectivaEditar.NombreORazonSocial;
            txtRFC.Text = laColectivaEditar.RFC;

            txtAmaterno.Text = laColectivaEditar.AMaterno;
            txtApaterno.Text = laColectivaEditar.APaterno;


        }

        protected Colectiva AsignarNuevosDatos(Colectiva laColectiva)
        {
            Colectiva ColectivaOriginal = (Colectiva)HttpContext.Current.Session.Contents["Colectiva"];

            if (!txtPass1.Text.Trim().Equals(txtPass2.Text))
            {
                throw new Exception("No Coinciden los Passwords");
            }

            laColectiva.ID_Colectiva = ColectivaOriginal.ID_Colectiva;
            laColectiva.ID_ColectivaPadre = ColectivaOriginal.ID_ColectivaPadre;// Int64.Parse(cmbPadreTipoColectiva.Value.ToString());
            laColectiva.ID_ColectivaCCM = ColectivaOriginal.ID_ColectivaCCM;// Int64.Parse(cmbPadreTipoColectiva.Value.ToString());
            laColectiva.Password = txtPass1.Text;

            laColectiva.ClaveColectiva = txtUserName.Text;
            laColectiva.TipoColectiva = ColectivaOriginal.TipoColectiva;
            laColectiva.CURP = ColectivaOriginal.CURP;
            laColectiva.Email = ColectivaOriginal.Email;
            laColectiva.Telefono = ColectivaOriginal.Telefono;
            laColectiva.Movil = ColectivaOriginal.Movil;
            laColectiva.NombreORazonSocial = txtNombre.Text;
            laColectiva.APaterno = txtApaterno.Text;
            laColectiva.AMaterno = txtAmaterno.Text;
            laColectiva.RFC = ColectivaOriginal.RFC;

            return laColectiva;


        }
    }
}