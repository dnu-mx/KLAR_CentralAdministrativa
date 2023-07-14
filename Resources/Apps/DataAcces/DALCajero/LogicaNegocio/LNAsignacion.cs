using DALCajero.BaseDatos;
using DALCajero.Entidades;
using DALCajero.Utilidades;
using DALCentralAplicaciones.LogicaNegocio;
using Interfases;
using Interfases.Exceptiones;
using System;
using System.Data.SqlClient;


namespace DALCajero.LogicaNegocio
{
    public class LNAsignacion
    {
      

        public static RespuestaTransaccional AsignarFichaDepositoAMovimiento(FichaDeposito laFicha, IUsuario elUser, enumTipoAsignacion elTipoAsignacion, Guid AppId)
        {
            RespuestaTransaccional laRespTrx= new RespuestaTransaccional();
            TrxOperacion laOperacion = null;
            
            //Crear una Transaccion para los movimientos en la base de datos para mantener la integridad de los datos-
            String ISO_ID=  Guid.NewGuid().ToString();

            try
            {
                //Buscar el Movimiento que coincida con los datos de la Ficha de Deposito.
                Movimiento elMovimiento = DAOMovimiento.BuscarParaAsginarFichaDeposito(laFicha, elUser, AppId);

                if ((elMovimiento == null) || elMovimiento.ID_Movimiento == 0 || elMovimiento.ID_Movimiento <= 0)
                {
                    DALCajero.Utilidades.Loguear.Evento("No se encontró un Movimiento con los datos de la ficha de deposito Seleccionada", elUser.ClaveUsuario);
                    throw new CAppException(8003, "No se encontró un Movimiento con los datos de la ficha de deposito Seleccionada");
                }

                //Asignar la Ficha de Deposito con el Movimiento
                Asignacion laAsignacion = new Asignacion();

                laAsignacion.elMovimiento = elMovimiento;
                laAsignacion.laFichaDeposito = laFicha;
                laAsignacion.tipoOperacionTrx = laFicha.Operacion;
                laAsignacion.FechaAsignacion = DateTime.Now;
                laAsignacion.ClaveTipoAsignacion = ((int)elTipoAsignacion).ToString();


                using (SqlConnection conn = BDCajero.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            int RespAsignacion = DAOAsignacion.Asignar(laAsignacion, elUser, ISO_ID.ToString(), conn, transaccionSQL);

                            String PCAsignacion = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "PrefijoAsignacion").Valor + laFicha.Operacion.CodigoProceso+
                                DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "TCUniversal").Valor  ;

                            //Si la asignacion fue valida entonces se envia la operacion TRansaccional via WebService.
                            if (RespAsignacion == 0)
                            {
                                //Crear el Objeto con los datos para la Transaccion
                                laOperacion = new TrxOperacion
                                {
                                    Afiliacion =laFicha.Afiliacion, //DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "Afiliacion").Valor,
                                    Beneficiario = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "Beneficiario").Valor,
                                    FechaTransaccion = DateTime.Now,
                                    HoraTransaccion = DateTime.Now,
                                    Monto = laFicha.Importe.ToString(),
                                    Operador = elUser.ClaveUsuario,
                                    ProccesingCode = PCAsignacion,
                                    Referencia = elMovimiento.ID_Movimiento.ToString(),
                                    Sucursal = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "Sucursal").Valor,
                                    Terminal = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "Terminal").Valor,
                                    Adquirente = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "Afiliacion").Valor,
                                    Ticket = Guid.NewGuid().ToString().Replace('-', '0').Substring(0, 12),
                                    NIP="0000",
                                    CodigoMoneda = "MXN",
                                    Track2 = "0000000000000000=0000",
                                    MedioAcceso = laFicha.ClaveMedioAcceso,// DALCentralAplicaciones.Utilidades.Configuracion.Get(AppID, "MedioAccesoCajero").Valor,
                                    TipoMedioAcceso = laFicha.ClaveTipoMA,// "IDCTA"
                                    elTipoOperacion = TipoOperacion.Requerimiento
                                };

                                laRespTrx = LNTransaccional.ProcesaOperacion(laOperacion,  ISO_ID, elUser);

                                DAOOperaciones.AgregarOperacion(laOperacion, laRespTrx, elUser, conn, transaccionSQL);
                            }
                            else
                            {
                                Loguear.Evento("Ocurrio un Error al realizar la Asignación de la Ficha de Depósito: " + laAsignacion.ToString(), elUser.ClaveUsuario);
                                throw new CAppException(8005, "Ocurrio un Error al realizar la Asignación de la Ficha de Depósito" + laFicha.ToString());
                            }

                            if (laRespTrx.CodigoRespuesta != "0000")
                            {
                                Loguear.Evento("No se Autorizo la Operacion Transaccional para la Asignación de la Ficha de Depósito:<br/><b>Codigo Respuesta</b>: " + laRespTrx.CodigoRespuesta + "<br/><b>Descripcion</b>: " + laRespTrx.DescripcionRespuesta, elUser.ClaveUsuario);
                                throw new CAppException(8004, "No se Autorizo la Operacion Transaccional para la Asignación de la Ficha de Depósito:<br/><b>Codigo Respuesta</b>: " + laRespTrx.CodigoRespuesta + "<br/><b>Descripcion</b>: " + laRespTrx.DescripcionRespuesta);
                            }

                            //Compromete los cambios en la base de datos
                            transaccionSQL.Commit();
                        }
                        catch (CAppException err)
                        {
                            transaccionSQL.Rollback();

                            if ((laOperacion != null) &&  (laRespTrx.CodigoRespuesta == "0000"))
                            {
                                laOperacion.elTipoOperacion = TipoOperacion.Reverso;
                                laRespTrx = LNTransaccional.ProcesaOperacion(laOperacion, ISO_ID, elUser);
                                DAOOperaciones.AgregarOperacion(laOperacion, laRespTrx, elUser, conn, transaccionSQL);
                            }
                            
                            throw err;
                        }
                        catch (Exception err)
                        {
                            transaccionSQL.Rollback();

                            if ((laOperacion != null) && (laRespTrx.CodigoRespuesta == "0000"))
                            {
                                laOperacion.elTipoOperacion = TipoOperacion.Reverso;
                                laRespTrx = LNTransaccional.ProcesaOperacion(laOperacion, ISO_ID, elUser);
                                DAOOperaciones.AgregarOperacion(laOperacion, laRespTrx, elUser, conn, transaccionSQL);
                            }
                            
                            throw err;
                        }
                        //Si la Operacion es Exitosa hacer commit a los Cambios si no hacer rollbak

                        //Generar la respuesta y enviarla para mostrarla al Usuario


                        return laRespTrx;
                    }
                }
            }
            catch (CAppException err)
            {
                Loguear.Error(err, elUser.ClaveUsuario);
                throw err;

            }
            catch (Exception err)
            {
                Loguear.Error(err, elUser.ClaveUsuario);
                throw new CAppException(8010, err.Message, err);
            }
        }

        public static RespuestaTransaccional AsignarMovimientoAFichaDeposito(Movimiento elMovimiento, IUsuario elUser, enumTipoAsignacion elTipoAsignacion, Guid AppId)
        {
            RespuestaTransaccional laRespTrx = new RespuestaTransaccional();
            TrxOperacion laOperacion=null;// = new Interfases.TrxOperacion();
            //Crear una Transaccion para los movimientos en la base de datos para mantener la integridad de los datos-
            String ISO_ID = Guid.NewGuid().ToString();
            try
            {
                //Buscar el Movimiento que coincida con los datos de la Ficha de Deposito.
                FichaDeposito laFicha = DAOFichaDeposito.BuscarFichaParaAsignarMovimiento(elMovimiento, elUser,AppId);

                if ((laFicha == null) || laFicha.ID_FichaDeposito == 0 || laFicha.ID_FichaDeposito <= 0)
                {
                    throw new CAppException(8003, "No se encontró una Ficha de Deposito con los datos del Movimiento Seleccionado: " + elMovimiento.ToString());
                }

                //Asignar la Ficha de Deposito con el Movimiento
                Asignacion laAsignacion = new Asignacion();

                laAsignacion.elMovimiento = elMovimiento;
                laAsignacion.laFichaDeposito = laFicha;
                laAsignacion.tipoOperacionTrx = laFicha.Operacion;
                laAsignacion.FechaAsignacion = DateTime.Now;
                laAsignacion.ClaveTipoAsignacion = ((int)elTipoAsignacion).ToString();


                using (SqlConnection conn = BDCajero.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            int RespAsignacion = DAOAsignacion.Asignar(laAsignacion, elUser, ISO_ID.ToString(), conn, transaccionSQL);

                            String PCAsignacion = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "PrefijoAsignacion").Valor + laFicha.Operacion.CodigoProceso +
                              DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "TCUniversal").Valor ;

                            //Si la asignacion fue valida entonces se envia la operacion TRansaccional via WebService.
                            if (RespAsignacion == 0)
                            {
                                //Crear el Objeto con los datos para la Transaccion
                                laOperacion = new TrxOperacion
                                {
                                    Afiliacion =laFicha.Afiliacion,// DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "Afiliacion").Valor,
                                    Beneficiario = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "Beneficiario").Valor,
                                    FechaTransaccion = DateTime.Now,
                                    HoraTransaccion = DateTime.Now,
                                    Monto = laFicha.Importe.ToString(),
                                    Operador = elUser.ClaveUsuario,
                                    ProccesingCode = PCAsignacion,
                                    Referencia = elMovimiento.ID_Movimiento.ToString(),

                                    Sucursal = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "Sucursal").Valor,
                                    Terminal = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "Terminal").Valor,
                                    Adquirente = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppId, "Afiliacion").Valor,
                                    Ticket = Guid.NewGuid().ToString().Replace('-', '0').Substring(0, 12),
                                    NIP = "0000",
                                    CodigoMoneda = "MXN",
                                    Track2 = "0000000000000000=0000",
                                    MedioAcceso = laFicha.ClaveMedioAcceso,// DALCentralAplicaciones.Utilidades.Configuracion.Get(AppID, "MedioAccesoCajero").Valor,
                                    TipoMedioAcceso = laFicha.ClaveTipoMA,// "IDCTA"
                                    elTipoOperacion = TipoOperacion.Requerimiento
                                };

                                laRespTrx = LNTransaccional.ProcesaOperacion(laOperacion,  ISO_ID, elUser);
                                
                                //Agregar la Operacion a la BD.
                                DAOOperaciones.AgregarOperacion(laOperacion, laRespTrx, elUser, conn, transaccionSQL);
                            }
                            else
                            {
                                throw new CAppException(8005, "Ocurrio un Error al realizar la Asignación de la Ficha de Depósito: " + laFicha.ToString());
                            }

                            if (laRespTrx.CodigoRespuesta != "0000")
                            {
                                Loguear.Evento("No se Autorizo la Operacion Transaccional para la Asignación de la Ficha de Depósito:<br/><b>Codigo Respuesta</b>: " + laRespTrx.CodigoRespuesta + "<br/><b>Descripcion</b>: " + laRespTrx.DescripcionRespuesta, elUser.ClaveUsuario);
                                throw new CAppException(8004, "No se Autorizo la Operacion Transaccional para la Asignación de la Ficha de Depósito:<br/><b>Codigo Respuesta</b>: " + laRespTrx.CodigoRespuesta + "<br/><b>Descripcion</b>: " + laRespTrx.DescripcionRespuesta);
                            }

                            Loguear.Evento("Se Asigno Exitosamente el Movimiento [" + elMovimiento.ToString() + "] al la Ficha de Deposito [" + laFicha.ToString()+"]", elUser.ClaveUsuario);
                            //Compromete los cambios en la base de datos
                            transaccionSQL.Commit();
                        }
                        catch (CAppException err)
                        {
                            transaccionSQL.Rollback();
                            if ((laOperacion != null) && (laRespTrx.CodigoRespuesta == "0000"))
                            {
                                laOperacion.elTipoOperacion = TipoOperacion.Reverso;
                                laRespTrx = LNTransaccional.ProcesaOperacion(laOperacion, ISO_ID, elUser);
                                DAOOperaciones.AgregarOperacion(laOperacion, laRespTrx, elUser, conn, transaccionSQL);
                            }

                            
                            throw err;
                        }
                        catch (Exception err)
                        {
                            transaccionSQL.Rollback();

                            if (laOperacion != null)
                            {
                                laOperacion.elTipoOperacion = TipoOperacion.Reverso;
                                laRespTrx = LNTransaccional.ProcesaOperacion(laOperacion, ISO_ID, elUser);
                                DAOOperaciones.AgregarOperacion(laOperacion, laRespTrx, elUser, conn, transaccionSQL);
                            }
                            
                            throw err;
                        }
                        //Si la Operacion es Exitosa hacer commit a los Cambios si no hacer rollbak

                        //Generar la respuesta y enviarla para mostrarla al Usuario


                        return laRespTrx;
                    }
                }
            }
            catch (CAppException err)
            {
                Loguear.Error(err, elUser.ClaveUsuario);
                throw err;

            }
            catch (Exception err)
            {
                Loguear.Error(err, elUser.ClaveUsuario);
                throw new CAppException(8010, err.Message, err);
            }
        }

        public static RespuestaTransaccional Asignar(Int64 ID_Movimiento, Int64 ID_Ficha, IUsuario elUser, enumTipoAsignacion elTipoAsignacion, Guid AppID)
        {
            RespuestaTransaccional laRespTrx = new RespuestaTransaccional();
            TrxOperacion laOperacion = null;

            //Crear una Transaccion para los movimientos en la base de datos para mantener la integridad de los datos-
            String ISO_ID = Guid.NewGuid().ToString();
            try
            {
                //Buscar el Movimiento que coincida con los datos de la Ficha de Deposito.
                //Seleccionar la Ficha con el ID que se proporciona
                FichaDeposito laFicha = DAOFichaDeposito.ConsultaFichaDeposito(ID_Ficha, elUser, AppID);
                if ((laFicha.ID_FichaDeposito == 0))
                {
                    throw new CAppException(8009, "El Identificador de la Ficha proporcionado no está Asginado a una Ficha Válida");
                }

                //Seleccionar el Movimiento seleccionado
                Movimiento elMovimiento = DAOMovimiento.ConsultaMovimiento(ID_Movimiento, elUser,AppID);

                if ((elMovimiento.ID_Movimiento == 0))
                {
                    throw new CAppException(8009, "El Movimiento Seleccionado no es Valido:" + elMovimiento.ToString());
                }


                //Asignar la Ficha de Deposito con el Movimiento
                Asignacion laAsignacion = new Asignacion();

                laAsignacion.elMovimiento = elMovimiento;
                laAsignacion.laFichaDeposito = laFicha;
                laAsignacion.tipoOperacionTrx = laFicha.Operacion;
                laAsignacion.FechaAsignacion = DateTime.Now;
                laAsignacion.ClaveTipoAsignacion = ((int)elTipoAsignacion).ToString();


                using (SqlConnection conn = BDCajero.BDEscritura)
                {
                    conn.Open();

                    using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                    {
                        try
                        {
                            int RespAsignacion = DAOAsignacion.Asignar(laAsignacion, elUser, ISO_ID.ToString(), conn, transaccionSQL);

                            String PCAsignacion = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppID, "PrefijoAsignacion").Valor + laFicha.Operacion.CodigoProceso +
                             DALCentralAplicaciones.Utilidades.Configuracion.Get(AppID, "TCResguardo").Valor ;


                            //Si la asignacion fue valida entonces se envia la operacion TRansaccional via WebService.
                            if (RespAsignacion == 0)
                            {
                                //Crear el Objeto con los datos para la Transaccion
                                laOperacion = new TrxOperacion
                                {
                                    Afiliacion =laFicha.Afiliacion,// DALCentralAplicaciones.Utilidades.Configuracion.Get(AppID, "Afiliacion").Valor,
                                    Beneficiario = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppID, "Beneficiario").Valor,
                                    FechaTransaccion = DateTime.Now,
                                    HoraTransaccion = DateTime.Now,
                                    Monto = laFicha.Importe.ToString(),
                                    Operador = elUser.ClaveUsuario,
                                    ProccesingCode = PCAsignacion,
                                    Referencia = elMovimiento.ID_Movimiento.ToString(),
                                    Sucursal = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppID, "Sucursal").Valor,
                                    Terminal = DALCentralAplicaciones.Utilidades.Configuracion.Get(AppID, "Terminal").Valor,
                                    Ticket = Guid.NewGuid().ToString().Replace('-', '0').Substring(0, 12),
                                    Adquirente = laFicha.Afiliacion,// DALCentralAplicaciones.Utilidades.Configuracion.Get(AppID, "Afiliacion").Valor,
                                    NIP = "0000",
                                    CodigoMoneda = "MXN",
                                    Track2 = "0000000000000000=0000",
                                    MedioAcceso = laFicha.ClaveMedioAcceso,// DALCentralAplicaciones.Utilidades.Configuracion.Get(AppID, "MedioAccesoCajero").Valor,
                                    TipoMedioAcceso = laFicha.ClaveTipoMA,// "IDCTA"
                                    elTipoOperacion = TipoOperacion.Requerimiento
                                };

                                laRespTrx = LNTransaccional.ProcesaOperacion(laOperacion, ISO_ID, elUser);

                                //Agregar la Operacion a la BD.
                                DAOOperaciones.AgregarOperacion(laOperacion, laRespTrx, elUser, conn, transaccionSQL);
                            }
                            else
                            {

                                throw new CAppException(8005, "Ocurrio un Error al realizar la Asignación de la Ficha de Depósito: "  + laFicha.ToString());
                            }

                            if (laRespTrx.CodigoRespuesta != "0000")
                            {
                                Loguear.Evento("No se Autorizo la Operacion Transaccional para la Asignación de la Ficha de Depósito:<br/><b>Codigo Respuesta</b>: " + laRespTrx.CodigoRespuesta + "<br/><b>Descripcion</b>: " + laRespTrx.DescripcionRespuesta, elUser.ClaveUsuario);
                                throw new CAppException(8004, "No se Autorizo la Operacion Transaccional para la Asignación de la Ficha de Depósito:<br/><b>Codigo Respuesta</b>: " + laRespTrx.CodigoRespuesta + "<br/><b>Descripcion</b>: " + laRespTrx.DescripcionRespuesta);
                            }

                            //Compromete los cambios en la base de datos
                            transaccionSQL.Commit();
                        }
                        catch (CAppException err)
                        {
                            transaccionSQL.Rollback();

                            if ((laOperacion != null) && (laRespTrx.CodigoRespuesta == "0000"))
                            {
                                laOperacion.elTipoOperacion = TipoOperacion.Reverso;
                                laRespTrx = LNTransaccional.ProcesaOperacion(laOperacion, ISO_ID, elUser);
                                DAOOperaciones.AgregarOperacion(laOperacion, laRespTrx, elUser, conn, transaccionSQL);
                            }
                            
                            throw err;
                        }
                        catch (Exception err)
                        {
                            transaccionSQL.Rollback();

                            if ((laOperacion != null) && (laRespTrx.CodigoRespuesta == "0000"))
                            {
                                laOperacion.elTipoOperacion = TipoOperacion.Reverso;
                                laRespTrx = LNTransaccional.ProcesaOperacion(laOperacion, ISO_ID, elUser);
                                DAOOperaciones.AgregarOperacion(laOperacion, laRespTrx, elUser, conn, transaccionSQL);
                            }
                            
                            throw err;
                        }
                        //Si la Operacion es Exitosa hacer commit a los Cambios si no hacer rollbak

                        //Generar la respuesta y enviarla para mostrarla al Usuario


                        return laRespTrx;
                    }
                }
            }
            catch (CAppException err)
            {
                Loguear.Error(err, elUser.ClaveUsuario);
                throw err;

            }
            catch (Exception err)
            {
                Loguear.Error(err, elUser.ClaveUsuario);
                throw new CAppException(8010, err.Message, err);
            }
        }

    }
}
