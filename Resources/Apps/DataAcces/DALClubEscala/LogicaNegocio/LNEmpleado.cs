using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DALClubEscala.Entidades;
//using ClubEscala.Utilidades;
//using DALCentralAplicaciones.Utilidades;
using DALCentralAplicaciones.Entidades;
using Framework;
using System.Diagnostics;
using DALCentralAplicaciones.Utilidades;
using DALClubEscala.BaseDatos;
using Interfases;
using DALCentralAplicaciones.LogicaNegocio;
using DALClubEscala.Utilidades;
using System.Configuration;
using System.IO;

namespace DALClubEscala.LogicaNegocio
{
   public  class LNEmpleado
    {

        public static int ImportarArchivo(String RutaArchivo, int ID_CadenaComercial, DALCentralAplicaciones.Entidades.Usuario elUsuario, Guid AppID)
        {
            try
            {

                return 0;
            }
            catch (Exception ex)
            {
                ClubEscala.Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new Exception(ex.Message);
            }
        }

        public static int Agregar(Empleado elEmpleado, DALCentralAplicaciones.Entidades.Usuario elUsuario, Guid AppID)
        {
            try
            {

                return 0;
            }
            catch (Exception ex)
            {

                ClubEscala.Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new Exception(ex.Message);
            }
        }

        public static int Modificar(Empleado elEmpleado, DALCentralAplicaciones.Entidades.Usuario elUsuario, Guid AppID)
        {
            try
            {

                return 0;
            }
            catch (Exception ex)
            {

                ClubEscala.Utilidades.Loguear.Error(ex, elUsuario.ClaveUsuario);
                throw new Exception(ex.Message);
            }
        }

        public static int ProcesarEmpleadoPendientes(Int64 ID_Empleado,  Guid AppID, Usuario elUser, bool SolicitaCambioNIP)
        {
            try
            {
                int respuesta = 1;

                Empleado unEmpleado = DAOEmpleado.ObtieneEmpleadoPorProcesar(ID_Empleado);


                switch (unEmpleado.ID_Estatus)
                {
                    case 1://Sin Procesar
                    case 8://Error en Crear Colectiva

                        respuesta = LNEmpleado.CrearColectiva(unEmpleado, AppID, elUser);

                        if (respuesta == 0)
                            respuesta= LNEmpleado.CrearCuentas(unEmpleado, AppID, elUser);

                        if (respuesta == 0)
                            respuesta = LNEmpleado.CrearDepositoInicial(unEmpleado, AppID);

                        if (respuesta == 0)
                            respuesta = LNEmpleado.CrearEnClubEscala(unEmpleado, SolicitaCambioNIP);

                        break;
                    case 2://Colectiva Creada
                    case 9://Error al Crear las Cuentas
                        
                            respuesta= LNEmpleado.CrearCuentas(unEmpleado, AppID, elUser);

                        if (respuesta == 0)
                            respuesta = LNEmpleado.CrearDepositoInicial(unEmpleado, AppID);

                        if (respuesta == 0)
                            respuesta = LNEmpleado.CrearEnClubEscala(unEmpleado, SolicitaCambioNIP);
                        break;
                    case 3://Cuentas Creadas
                    case 10://Error al Abonar
                            respuesta = LNEmpleado.CrearDepositoInicial(unEmpleado, AppID);

                        if (respuesta == 0)
                            respuesta = LNEmpleado.CrearEnClubEscala(unEmpleado, SolicitaCambioNIP);
                        break;
                    case 4://Abono Realizado
                    case 11://Error al Crear en Club Escala
                        respuesta = LNEmpleado.CrearEnClubEscala(unEmpleado, SolicitaCambioNIP);
                        break;
                    case 5://Creado en Autorizador sin Abono.
                    
                        
                        break;

                }
                return respuesta;
            }
            catch (Exception ex)
            {
                Loguear.Error(ex, "ProcesadorNocturno");
                throw new Exception(ex.Message);
            }
        }
   

        public static int CrearEnClubEscala(Empleado elEmpleado, bool SolicitaCambiarNIP)
        {

            try
            {
                Cliente objetoCliente = Cliente.Get("");
                string emailLogIn;

                if (elEmpleado.Baja.Equals("1")) // si es una operacion de baja solo inactiva el usuario en la DB
                {
                    //TODO: Inactivar el Operador en ClubEscala
                    try
                    {
                        Cliente.ActualizaEstatus(elEmpleado.EmailPersonal, 2);
                    }
                    catch (Exception)
                    {
                        try
                        {
                            Cliente.ActualizaEstatus(elEmpleado.EmailPersonal, 2);
                        }
                        catch (Exception)
                        {
                            throw new Exception("No se pudo eliminar el usuario de Club Escala: " + elEmpleado.NumeroEmpleado + " " + elEmpleado.APaterno);
                        }
                    }
                    return 0;
                }

                //validar que venga un email, se privilegia el Email personal
                if (elEmpleado.EmailPersonal == null || elEmpleado.EmailPersonal.Length == 0)
                {
                    if (elEmpleado.EmailEmpresarial != null || elEmpleado.EmailEmpresarial.Length != 0)
                    {
                        if (Cliente.EstaEmailEnBaseDatos(elEmpleado.EmailEmpresarial))
                        {
                            throw new Exception("El Email " + elEmpleado.EmailEmpresarial + "  Ya se encuentra en la BD");
                        }
                        else
                        {
                            emailLogIn = elEmpleado.EmailEmpresarial;
                        }
                    }
                    else
                    {
                        throw new Exception("Es Necesario un Email para agregar el Usuario en Club Escala");
                    }
                }
                else
                {
                    if (Cliente.EstaEmailEnBaseDatos(elEmpleado.EmailPersonal))
                    {
                        throw new Exception("El Email " + elEmpleado.EmailPersonal + " Ya se encuentra en la BD");
                    }
                    else
                    {
                        emailLogIn = elEmpleado.EmailPersonal;
                    }
                }

                if (elEmpleado.TelefonoMovil != null || elEmpleado.TelefonoMovil.Length == 0)
                {
                    if (Cliente.EstaTelefonoEnBaseDatos(elEmpleado.TelefonoMovil))
                    {
                        throw new Exception("El Telefono Movil " + elEmpleado.TelefonoMovil + " Ya se encuentra en la BD");
                    }
                }
                else
                {
                    throw new Exception("Es Necesario un Telefono para agregar el Usuario en Club Escala");
                }




                Cliente.TipoSexo elSexo = new Cliente.TipoSexo();

                String confirmacionSitio = Guid.NewGuid().ToString().ToUpper();
                String ConfirmacionCel = getNumero(1);
                String NIPTelefonico = "";
                //Int64 elPass;

                if (elEmpleado.Password == null)
                {
                    elEmpleado.Password = getNumero(2);
                }

                //if (!Int64.TryParse(elEmpleado.Password, out elPass) & elEmpleado.Password.ToString().Length!=6)
                //{
                    NIPTelefonico = getNumero(3);
                //}
                //else
                //{
                //    NIPTelefonico = elEmpleado.Password;
                //}

                objetoCliente.Add(Framework.ContextoInicial.adminServicio,
                    ContextoInicial.ContextoServicio,
                    emailLogIn,
                    elEmpleado.Password,
                    elEmpleado.Nombre,
                    elEmpleado.APaterno + " " + elEmpleado.AMaterno,
                    elEmpleado.FechaNacimiento.Year.ToString(),
                    Convert.ToInt32(elEmpleado.FechaNacimiento.Month),
                    Convert.ToInt32(elEmpleado.FechaNacimiento.Day),
                    elEmpleado.TelefonoMovil,
                     elSexo, true,
                    Cliente.ReferenciaCliente.Servicio, false, confirmacionSitio, NIPTelefonico, "", "", elEmpleado.ID_CadenaComercial);

                Framework.Log.EscribirEventoProceso("Error 5152. El usuario fue creado.",
                    5310,
                    "CreateUser.btnAdd_Click",
                    EventLogEntryType.Information,
                    emailLogIn);
                ClubEscala.Utilidades.Loguear.Evento("Se Creo el Usuario en Club Escala :" + emailLogIn, "ProcesadorNocturno");

                //genera el correo electronico de Bienvenida

                GeneraMailConfirmacion(confirmacionSitio, emailLogIn);

                //cambiamos el Estatus del cliente, para que la proxima vez que se registre solicitte cambio de password.
                if (SolicitaCambiarNIP)
                {
                    Cliente.ActualizaEstatus(emailLogIn, 2);
                }

                //Agregar medios de pago default
                agregarMedioPagoDefault(objetoCliente, emailLogIn, (string)Mensajeria.Configuracion.GetConfiguracion("DescripcionCredito"), (string)Mensajeria.Configuracion.GetConfiguracion("TipoCuentaCredito"));
                //agregarMedioPagoDefault(objetoCliente, elEmpleado.EmailPersonal, (string)Mensajeria.Configuracion.GetConfiguracion("DescripcionMonedero"), (string)Mensajeria.Configuracion.GetConfiguracion("TipoCuentaMonedero"));



                ClubEscala.Utilidades.Loguear.Evento("Envio de Correo de confirmacion ", "ProcesadorNocturno");
                Framework.Log.EscribirEventoProceso("Se Envio Correo con clave de activacion",
                5310,
                "CreateUser.btnAccept_Click",
                EventLogEntryType.Information,
                emailLogIn);

                //return 0;

             
                    DAOEmpleado.Actualiza(elEmpleado, ClubEscala.Utilidades.EstatusEmpleado.Procesada);
                    return 0;

            }
            catch (Exception err)
            {
                DAOEmpleado.Actualiza(elEmpleado, ClubEscala.Utilidades.EstatusEmpleado.ErrorAlCrearClubEscala);
                ClubEscala.Utilidades.Loguear.Error(err, "ClubEscala");
                return -1;
            }

        }


        public static int CrearDepositoInicial(Empleado elEmpleado,  Guid AppId)
        {
            try
            {

                //si es baja genera otro tipo de Operacion Transaccional o deja los saldos como estan.
                if (elEmpleado.Baja == "1")
                {
                    return 0;
                }

                //no genera transaccion porque no tiene saldo inicial
                if (decimal.Parse(elEmpleado.LimiteCompra.Trim().Length == 0 ? "0" : elEmpleado.LimiteCompra) == 0)
                {
                    return 0;
                }

                RespuestaTransaccional laRespTrx = new RespuestaTransaccional();
                TrxOperacion laOperacion = null;

                laOperacion = new TrxOperacion
                {
                    Afiliacion = ContextoInicial.GetCCMConfiguracion(elEmpleado.ID_CadenaComercial, "Afiliacion"), //DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "Afiliacion").Valor,
                    Beneficiario = ContextoInicial.GetCCMConfiguracion(elEmpleado.ID_CadenaComercial, "Beneficiario"),
                    FechaTransaccion = DateTime.Now,
                    HoraTransaccion = DateTime.Now,
                    Monto = elEmpleado.LimiteCompra,
                    Operador = elEmpleado.NumeroEmpleado,
                    ProccesingCode = ContextoInicial.GetCCMConfiguracion(elEmpleado.ID_CadenaComercial, "ProccesingCode"),
                    Referencia = elEmpleado.ID_Empleado.ToString(),
                    Sucursal = ContextoInicial.GetCCMConfiguracion(elEmpleado.ID_CadenaComercial, "Sucursal"),
                    Terminal = ContextoInicial.GetCCMConfiguracion(elEmpleado.ID_CadenaComercial, "Terminal"),
                    Adquirente = ContextoInicial.GetCCMConfiguracion(elEmpleado.ID_CadenaComercial, "Adquirente"),
                    Ticket = Guid.NewGuid().ToString().Replace('-', '0').Substring(0, 12),
                    NIP = "0000",
                    CodigoMoneda = "MXN",
                    Track2 = "0000000000000000=0000",
                    MedioAcceso = elEmpleado.TelefonoMovil,// DALCentralAplicaciones.Utilidades.Configuracion.Get(AppID, "MedioAccesoCajero").Valor,
                    TipoMedioAcceso = "TEL",// "IDCTA"
                    elTipoOperacion = TipoOperacion.Requerimiento
                };

                laRespTrx = LNTransaccional.ProcesaOperacion(laOperacion, DateTime.Now.ToString("yyyyMMddHHmmss"), new Usuario());

                if (int.Parse(laRespTrx.CodigoRespuesta) == 0)
                {
                    DAOEmpleado.Actualiza(elEmpleado, ClubEscala.Utilidades.EstatusEmpleado.AbonoRealizado);
                }
                else
                {
                    DAOEmpleado.Actualiza(elEmpleado, ClubEscala.Utilidades.EstatusEmpleado.CuentaCreada);
                }

                return int.Parse(laRespTrx.CodigoRespuesta);
            }
            catch (Exception err)
            {
                DAOEmpleado.Actualiza(elEmpleado, ClubEscala.Utilidades.EstatusEmpleado.ErrorAlAbonar);
                ClubEscala.Utilidades.Loguear.Error(err, "");
                return -1;
            }
        }

        public static int CrearColectiva(Empleado elEmpleado , Guid AppID, Usuario elUser )
        {
            try
            {
                if (elEmpleado.Nombre.Trim().Length == 0 || elEmpleado.NumeroEmpleado.Trim().Length == 0 || elEmpleado.APaterno.Trim().Length == 0)
                {
                    Loguear.Error("Empleado: " + elEmpleado.NumeroEmpleado + " " + elEmpleado.Nombre + " " + elEmpleado.APaterno + " -No se Puede crear la Colectiva con los datos proporcionados", elUser.ClaveUsuario);
                    return -1;
                }

                //Valida que traiga un numero de Empleado
                //Valida que traiga un telefono

                String Evento = ContextoInicial.GetCCMConfiguracion(elEmpleado.ID_CadenaComercial, "WsEvento");//Configuracion.Get(AppID, "WsEvento").Valor;

                ws_CEscala.WS_Administracion webService = new ws_CEscala.WS_Administracion();

                //int resp = webService.CrearEmpleadoClubEscala(Configuracion.Get(AppID, "wsUser").Valor, Configuracion.Get(AppID, "WsPass").Valor,
                //    Evento, Configuracion.Get(AppID, "WsCveTipoCol").Valor, "", elEmpleado.NumeroEmpleado, elEmpleado.Nombre, elEmpleado.APaterno,
                //    elEmpleado.AMaterno, elEmpleado.TelefonoMovil, elEmpleado.EmailPersonal == "" ? elEmpleado.EmailEmpresarial : elEmpleado.EmailPersonal,
                //    int.Parse(elEmpleado.ID_CadenaComercial.ToString()),elEmpleado.Baja);

                int resp = webService.CrearEmpleadoClubEscala(ContextoInicial.GetCCMConfiguracion(elEmpleado.ID_CadenaComercial, "WsUser"), ContextoInicial.GetCCMConfiguracion(elEmpleado.ID_CadenaComercial, "wsPass"),
                    Evento, ContextoInicial.GetCCMConfiguracion(elEmpleado.ID_CadenaComercial, "WsCveTipoCol"), "", elEmpleado.NumeroEmpleado, elEmpleado.Nombre, elEmpleado.APaterno,
                    elEmpleado.AMaterno, elEmpleado.TelefonoMovil, elEmpleado.EmailPersonal == "" ? elEmpleado.EmailEmpresarial : elEmpleado.EmailPersonal,
                    int.Parse(elEmpleado.ID_CadenaComercial.ToString()), elEmpleado.Baja);

                if (resp == 0)
                {
                    DAOEmpleado.Actualiza(elEmpleado, ClubEscala.Utilidades.EstatusEmpleado.ColectivaCreada);

                }
                else
                {
                    DAOEmpleado.Actualiza(elEmpleado, ClubEscala.Utilidades.EstatusEmpleado.SinProcesar);
                }

                return resp;
            }
            catch (Exception err)
            {
                DAOEmpleado.Actualiza(elEmpleado, ClubEscala.Utilidades.EstatusEmpleado.ErrorAlCrearColectiva);
                ClubEscala.Utilidades.Loguear.Error(err, "");
                return -1;
            }
        }

        public static int CrearCuentas(Empleado elEmpleado, Guid AppID, Usuario elUser)
        {
            try
            {

                //Valida que traiga un numero de Empleado
                //Valida que traiga un telefono


                if (elEmpleado.Baja == "1")
                {
                    return 0;
                }

                if (elEmpleado.TelefonoMovil.Trim().Length == 0 || (elEmpleado.EmailEmpresarial.Trim().Length == 0 && elEmpleado.EmailPersonal.Trim().Length == 0))
                {
                    Loguear.Error("Empleado: " + elEmpleado.NumeroEmpleado + " " + elEmpleado.Nombre + " " + elEmpleado.APaterno + " -Para Crear las Cuentas es necesario que un Email y un Telefono para asignarle a los medios de acceso", elUser.ClaveUsuario);
                    return -1;
                }


                String Evento = Configuracion.Get(AppID, "WsEvento").Valor;

                ws_CEscala.WS_Administracion webService = new ws_CEscala.WS_Administracion();

                int resp = webService.CrearCuentasaEmpleadoClubEscala(
                    Configuracion.Get(AppID, "wsUser").Valor, 
                    Configuracion.Get(AppID, "WsPass").Valor,
                    Evento,
                    Configuracion.Get(AppID, "WsCveTipoCol").Valor, 
                    "", 
                    elEmpleado.NumeroEmpleado,
                    0,
                    elEmpleado.Nombre, 
                    elEmpleado.APaterno,
                    elEmpleado.AMaterno, 
                    elEmpleado.FechaNacimiento.ToString("yyyy-MM-dd"), 
                    elEmpleado.TelefonoMovil,
                    elEmpleado.EmailPersonal == "" ? elEmpleado.EmailEmpresarial : elEmpleado.EmailPersonal,
                    Configuracion.Get(AppID, "WsCvegpoMA").Valor,
                    getNumero(3), 
                    int.Parse(Configuracion.Get(AppID, "wsAniosExpira").Valor), 
                    Configuracion.Get(AppID, "wsCveGpoCta").Valor, 
                    int.Parse(elEmpleado.ID_CadenaComercial.ToString()),
                    Configuracion.Get(AppID, "wsCveTipoCta").Valor,
                    "0",
                    elEmpleado.Baja,
                    elEmpleado.CicloNominal,
                    elEmpleado.DiaPago);

                if (resp == 0)
                {
                    DAOEmpleado.Actualiza(elEmpleado, ClubEscala.Utilidades.EstatusEmpleado.CuentaCreada);
                }
                else
                {
                    DAOEmpleado.Actualiza(elEmpleado, ClubEscala.Utilidades.EstatusEmpleado.ColectivaCreada);
                }

                return resp;
            }
            catch (Exception err)
            {
                DAOEmpleado.Actualiza(elEmpleado, ClubEscala.Utilidades.EstatusEmpleado.ErrorAlCrearCuentas);
                ClubEscala.Utilidades.Loguear.Error(err, "");
                return -1;
            }
        }

        private static string getNumero(int var)
        {
            Random r = new Random(DateTime.Now.Millisecond+var);
            int num = r.Next(0, 999999);

            return num.ToString().PadLeft(6, '0');

        }
       
        //private static void GeneraMailConfirmacion(String CveConfirmacion, Cliente clienteNuevo)
        //{

        //    StringBuilder strMessage = new StringBuilder();
        //    try
        //    {

        //        //Cliente.TipoSexo sexo= ( this.RadioButtonFemale.Checked ) ? Cliente.TipoSexo.Femenino : Cliente.TipoSexo.Masculino;
        //        if (clienteNuevo.Sexo == Cliente.TipoSexo.Femenino)
        //        {
        //            strMessage.AppendFormat("Estimada ");
        //        }
        //        else
        //        {
        //            strMessage.AppendFormat("Estimado ");
        //        }

        //        strMessage.AppendFormat(clienteNuevo.Nombre);
        //        strMessage.AppendFormat(" ");
        //        strMessage.AppendFormat(clienteNuevo.Apellido);
        //        strMessage.AppendFormat(": \n");
        //        strMessage.AppendFormat("Te enviamos la clave para activar la venta de Tiempo Aire Electronico a traves de nuestro Sitio.\n");
        //        strMessage.AppendFormat("Esta clave te sera solicitada al realizar tu primer compra\n");
        //        strMessage.AppendFormat("\n");
        //        strMessage.AppendFormat("\n");
        //        //strMessage.AppendFormat(String.Format("CLAVE DE ACTIVACION DE RECARGA WEB: {0}", CveConfirmacion.ToUpper()));
        //        strMessage.AppendFormat("\n");
        //      try
        //        {
        //            strMessage.AppendFormat("Usuario: {0} \n Contraseña : {1}",
        //                           clienteNuevo.Mail,
        //                           clienteNuevo.User.UserPassword);

        //            clienteNuevo = Cliente.GetDesdeBaseDatos(clienteNuevo.Mail);

        //            strMessage.AppendFormat("Usuario IVR-SMS: {0} \n NIP IVR-SMS : {1}",
        //                            clienteNuevo.NumeroTelefono,
        //                            clienteNuevo.NIP);
        //        }
        //        catch (Exception err)
        //        {
        //            strMessage.AppendFormat("Usuario: {0} \n Contraseña : {1}",
        //                           clienteNuevo.Mail,
        //                            clienteNuevo.User.UserPassword);
        //        }
        //        strMessage.AppendFormat("\n");
        //        strMessage.AppendFormat("\n");

        //        strMessage.AppendFormat("Muchas gracias por confiar en nosotros y disfruta de nuestro servicio.");

        //        //Enviamos el correo para confirmacion de Email
        //        Mail.Send(clienteNuevo.Mail, ContextoInicial.AppSettings["MailContacto"],
        //               strMessage.ToString(), "Clave de Activacion de Recarga Web", false);
        //    }
        //    catch (Exception err)
        //    {
        //        ClubEscala.Utilidades.Loguear.Error(err, "");
        //    }
        //}

        private static void GeneraMailConfirmacion(String CveConfirmacion, String eMaildelCliente)
        {

            StringBuilder strMessage = new StringBuilder();
            Cliente elCliente = Cliente.GetDesdeBaseDatos(eMaildelCliente);
            Cliente elCliente2 = Cliente.Get(eMaildelCliente);

            //Cliente.TipoSexo sexo= ( this.cmbSexo.SelectedValue=="1" ) ? Cliente.TipoSexo.Femenino : Cliente.TipoSexo.Masculino;

            StringBuilder emailHtml = new StringBuilder(File.ReadAllText(ConfigurationManager.AppSettings["HtmlBienvenidaEmpleados"]));

            emailHtml.Replace("[SOCIONOMBRE]", elCliente2.Nombre + " " + elCliente2.Apellido);
            emailHtml.Replace("[USUARIO]", elCliente2.Mail);
            emailHtml.Replace("[CONTRA]", FreezeCode.IceCubes.Security.Encryption.AccessEncryption.DecrypPassword(ContextoInicial.idAplicacion, elCliente2.User.UserPassword));
            emailHtml.Replace("[USUARIOIVR]", elCliente.NumeroTelefono);
            emailHtml.Replace("[CONTRAIVR]", elCliente.NIP);
            //emailHtml.Replace("[CLAVEACTIVACION]", CveConfirmacion);

            //Enviamos el correo para confirmacion de Email
            Mail.Send(elCliente2.Mail, ContextoInicial.AppSettings["MailContacto"],
                   emailHtml.ToString(), "Bienvenid@ a Club Escala", true);
        }

        private static void agregarMedioPagoDefault(Cliente clienteNuevo, string emailUsuario, string DescTipoCuenta, string TipoCuenta)
        {
            Tarjeta medioPago = null;
            string Procescode = "14" + TipoCuenta + "00";

            medioPago = Tarjeta.Inicializa(emailUsuario
                , "000"
                , DateTime.Now.AddYears(10).ToString("MMyy")
                , clienteNuevo.Nombre + " " + clienteNuevo.Apellido
                , DescTipoCuenta
                , Tarjeta.EstadoTarjeta.Activa
                , Guid.Empty
                , clienteNuevo.Mail
                , "conocido"
                , "conocido"
                , "conocido"
                , "conocido"
                , "{799CFB5D-F9CE-4B87-B537-6230EFE39897}" //Distrito Federal
                , "00000"
                , "conocido", TipoCuenta, Procescode, false, Config.Get(clienteNuevo.ID_CadenaComercial, "Afiliacion"), emailUsuario);

            clienteNuevo.TarjetasBanco.AgregarABaseDatos(medioPago);

            Framework.Log.EscribirEventoProceso("Error 5151. Se agregó el medio de pago tipo : " + DescTipoCuenta + ": " + medioPago.Numero + ".",
                    5301,
                    "CreateUser.btnAccept_Click",
                    EventLogEntryType.Information,
                    clienteNuevo.Mail);
        }
	


    }
}
