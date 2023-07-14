using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DALCertificados.Entidades;
using Interfases;
using System.Data.SqlClient;
using DALCertificados.BaseDatos;
using System.Data;
using DALCertificados.Utilidades;

namespace DALCertificados.LogicaNegocio
{
    public class LNCertificado
    {

        public static Boolean Crear(long ID_cadena, IUsuario elUser, int DiasCaducidad, int noCertificados)
        {
            using (SqlConnection conn = BDCertificado.BDEscritura)
            {
                conn.Open();

                using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                {
                    try
                    {

                        for (int k = 0; k <= noCertificados; k++)
                        {
                            Certificado unNuevoCertificado = new Certificado();

                            unNuevoCertificado.ID_CadenaComercial = ID_cadena;
                            unNuevoCertificado.Clave = Guid.NewGuid().ToString().Replace('-', 'A').Substring(0, 10);
                            unNuevoCertificado.ID_Certificado = Guid.NewGuid();
                            unNuevoCertificado.ID_Estatus = 1;
                            unNuevoCertificado.DiasExpiracion = DiasCaducidad;

                            DAOCertificado.Crear(unNuevoCertificado, elUser, conn, transaccionSQL);
                        }

                        transaccionSQL.Commit();
                        return true;

                    }
                    catch (Exception err)
                    {
                        Loguear.Error(err, "");
                        transaccionSQL.Rollback();
                        return false;

                    }
                }
            }
        }

        public static Boolean CrearYAsignarColectivas(Certificado elCertificado, IUsuario elUser, Guid AppID)
        {
            using (SqlConnection conn = BDCertificado.BDEscritura)
            {
                conn.Open();

                using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                {
                    try
                    {
                        elCertificado.Clave = Guid.NewGuid().ToString().Replace('-', 'A').Substring(0, 10);
                        elCertificado.ID_Certificado = Guid.NewGuid();
                        elCertificado.ID_Estatus = 1;
                        elCertificado.DiasExpiracion = 2000;

                       elCertificado.ID_Certificado= DAOCertificado.Crear(elCertificado, elUser, conn, transaccionSQL);


                        transaccionSQL.Commit();
                       //

                    }
                    catch (Exception err)
                    {
                        Loguear.Error(err, "");
                        transaccionSQL.Rollback();
                        return false;

                    }

                    try
                    {

                        DAOCertificado.AsignarColectivas(elCertificado, elUser.UsuarioTemp, AppID);

                        return true;
                    }
                    catch (Exception err)
                    {
                        Loguear.Error(err, "");
                        return false;
                    }


                }
            }
        }

        public static Certificado CrearYAsignarColectivas_TPVWEB(Certificado elCertificado, IUsuario elUser, Guid AppID)
        {
            using (SqlConnection conn = BDCertificado.BDEscritura)
            {
                conn.Open();

                using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                {
                    try
                    {
                        elCertificado.Clave = Guid.NewGuid().ToString().Replace('-', 'A').Substring(0, 10);
                        elCertificado.ID_Certificado = Guid.NewGuid();
                        elCertificado.ID_Estatus = 1;
                        elCertificado.DiasExpiracion = 2000;

                        elCertificado.ID_Certificado = DAOCertificado.Crear(elCertificado, elUser, conn, transaccionSQL);


                        transaccionSQL.Commit();
                        //

                    }
                    catch (Exception err)
                    {
                        Loguear.Error(err, "");
                        transaccionSQL.Rollback();
                        return elCertificado;

                    }

                    try
                    {

                        DAOCertificado.AsignarColectivas(elCertificado, elUser.UsuarioTemp, AppID);

                        return elCertificado;
                    }
                    catch (Exception err)
                    {
                        Loguear.Error(err, "");
                        return elCertificado;
                    }


                }
            }
        }


        public static Certificado PDVOL_CrearYAsignarColectivas(Certificado elCertificado, string Telefono, IUsuario elUser, Guid AppID)
        {
            using (SqlConnection conn = BDCertificado.BDEscritura)
            {
                conn.Open();

                using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                {
                    try
                    {
                        elCertificado.Clave = Telefono;
                        elCertificado.ID_Certificado = Guid.NewGuid();
                        elCertificado.ID_Estatus = 1;
                        elCertificado.DiasExpiracion = 2000;

                        elCertificado.ID_Certificado = DAOCertificado.Crear(elCertificado, elUser, conn, transaccionSQL);


                        transaccionSQL.Commit();
                        //

                    }
                    catch (Exception err)
                    {
                        Loguear.Error(err, "");
                        transaccionSQL.Rollback();
                        return elCertificado;

                    }

                    try
                    {

                        DAOCertificado.AsignarColectivas(elCertificado, elUser.UsuarioTemp, AppID);

                        return elCertificado;
                    }
                    catch (Exception err)
                    {
                        Loguear.Error(err, "");
                        return elCertificado;
                    }


                }
            }
        }



        public static Boolean Desactivar(Guid ID_Certificado, IUsuario elUser)
        {
            using (SqlConnection conn = BDCertificado.BDEscritura)
            {
                conn.Open();

                using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                {
                    try
                    {


                        DAOCertificado.Desactivar(ID_Certificado, elUser, conn, transaccionSQL);


                        transaccionSQL.Commit();
                        return true;

                    }
                    catch (Exception err)
                    {
                        Loguear.Error(err, "");
                        transaccionSQL.Rollback();
                        return false;

                    }
                }
            }
        }

        public static Boolean Desactivar(string ElCertificado)
        {
            using (SqlConnection conn = BDCertificado.BDEscritura)
            {
                conn.Open();

                using (SqlTransaction transaccionSQL = conn.BeginTransaction())
                {
                    try
                    {


                        DAOCertificado.PDVOL_Desactivar(ElCertificado, conn, transaccionSQL);


                        transaccionSQL.Commit();
                        return true;

                    }
                    catch (Exception err)
                    {
                        Loguear.Error(err, "");
                        transaccionSQL.Rollback();
                        return false;

                    }
                }
            }
        }


        public static Respuesta Activar(Certificado elCertificado, Guid elUsuario, Guid AppId)
        {

            try
            {
                Certificado nuevoCertificado = LNCertificado.Obtener(elCertificado.Clave, elUsuario, AppId);
                elCertificado.ID_Certificado = nuevoCertificado.ID_Certificado;

                if (nuevoCertificado.Clave == null)
                {
                    return new Respuesta(1454, "Certificado no Encontrado", "");
                }

                if (nuevoCertificado.ID_Estatus == 2)
                {
                    return new Respuesta(1454, "Certificado Activo asignado a otra Terminal", "");
                }

                if (!nuevoCertificado.Sucursal.Equals(elCertificado.Sucursal))
                {
                    return new Respuesta(1455, "Sucursal No Coincide", "");
                }

                if (!nuevoCertificado.Terminal.Equals(elCertificado.Terminal))
                {
                    return new Respuesta(1455, "Terminal no Coincide", "");
                }

                if (!nuevoCertificado.Afiliacion.Equals(elCertificado.Afiliacion))
                {
                    return new Respuesta(1455, "Afiliacion no Coincide", "");
                }

                DAOCertificado.Activar(elCertificado, elUsuario, AppId);

                return new Respuesta(0, "Activación Autorizada", nuevoCertificado.ClaveCadena); ;

            }
            catch (Exception err)
            {
                Loguear.Error(err, "");
                return new Respuesta(1456, "Ocurrio un Error en la Activación" + System.Environment.NewLine + err.Message,"");
            }

        }


        public static Certificado Obtener(String ClaveCertificado, Guid elUsuario, Guid AppId)
        {

            try
            {

                return DAOCertificado.ObtieneCertificado(ClaveCertificado, elUsuario, AppId);

            }
            catch (Exception err)
            {
                Loguear.Error(err, "");
                return new Certificado();
            }

        }

        public static Certificado Obtener(String ClaveCertificado)
        {

            try
            {

                return DAOCertificado.APP_ObtieneCertificado(ClaveCertificado);

            }
            catch (Exception err)
            {
                Loguear.Error(err, "");
                throw new Exception("NO EXISTE EL CERTIFICADO:" + err.Message);
            }

        }



        public static Boolean ValidaCertificado(Certificado elCertificado)
        {

            Boolean laRespuesta = false;
            try
            {

                Certificado nuevoCertificado = DAOCertificado.ObtieneCertificado(elCertificado.Clave);

                elCertificado.ID_Certificado = nuevoCertificado.ID_Certificado;

                if (nuevoCertificado.Clave == null)
                {
                   // return new Respuesta(1454, "Certificado no Encontrado o Ya ha sido asignado a otra Terminal", "");
                    Loguear.Evento("No Hay Certificado con la clave " + elCertificado.Clave, "");
                    laRespuesta = false;
                    return laRespuesta;
                }

                if (!nuevoCertificado.Sucursal.Equals(elCertificado.Sucursal))
                {
                    Loguear.Evento("Sucursal No coindice: Dice: " + elCertificado.Sucursal + ", Debe Decir:" + nuevoCertificado.Sucursal, "");
                    laRespuesta = false;
                    return laRespuesta;
                }

                if (!nuevoCertificado.Terminal.Equals(elCertificado.Terminal))
                {
                    Loguear.Evento("Terminal No coindice: Dice: " + elCertificado.Terminal + ", Debe Decir:" + nuevoCertificado.Terminal, "");
                    laRespuesta = false;
                    return laRespuesta;
                    //return laRespuesta;
                }

                if (!nuevoCertificado.Afiliacion.Equals(elCertificado.Afiliacion))
                {
                    Loguear.Evento("Afiliacion No coindice: Dice: " + elCertificado.Afiliacion + ", Debe Decir:" + nuevoCertificado.Afiliacion, "");
                    laRespuesta = false;
                    return laRespuesta;
                }


                if (!nuevoCertificado.MAC.Equals(elCertificado.MAC))
                {
                    Loguear.Evento("MAC No coindice: Dice: " + elCertificado.MAC + ", Debe Decir:" + nuevoCertificado.MAC, "");
                    laRespuesta = false;
                    return laRespuesta;
                }

                if (!nuevoCertificado.IDMB.Equals(elCertificado.IDMB))
                {
                    Loguear.Evento("IDMB No coindice: Dice: " + elCertificado.IDMB + ", Debe Decir:" + nuevoCertificado.IDMB, "");
                    laRespuesta = false;
                    return laRespuesta;
                }

                if (!nuevoCertificado.IDPROC.Equals(elCertificado.IDPROC))
                {
                    Loguear.Evento("IDMB No coindice: Dice: " + elCertificado.IDPROC + ", Debe Decir:" + nuevoCertificado.IDPROC, "");
                    laRespuesta = false;
                    return laRespuesta;
                }

                //if (!nuevoCertificado.IDWIN.Equals(elCertificado.IDWIN))
                //{
                //    Loguear.Evento("IDMB No coindice: Dice: " + elCertificado.IDWIN + ", Debe Decir:" + nuevoCertificado.IDWIN, "");
                //    laRespuesta = false;
                //}

                laRespuesta = true;

            }
            catch (Exception err)
            {
                Loguear.Error(err, "");
                laRespuesta = false;
            }

            return laRespuesta;
        }


        public static List<Certificado> Obtener(Dictionary<Int64, Terminal> Terminales, Dictionary<Int64, Certificado> Certificados)
        {
            List<Certificado> losCertificados = new List<Certificado>();
            try
            {

                foreach (Terminal unaTerm in Terminales.Values)
                {

                    Certificado unNuevoCertificado = new Certificado();

                    if (Certificados.ContainsKey(unaTerm.ID_ColectivaTerminal))
                    {
                        unNuevoCertificado = Certificados[unaTerm.ID_ColectivaTerminal];


                        unNuevoCertificado.Sucursal = unaTerm.Sucursal;
                        unNuevoCertificado.Afiliacion = unaTerm.Afiliacion;
                        unNuevoCertificado.Terminal = unaTerm.laTerminal;
                        unNuevoCertificado.ClaveCadena = unaTerm.ClaveCadena;

                    }
                    else
                    {
                        unNuevoCertificado.Sucursal = unaTerm.Sucursal;
                        unNuevoCertificado.Afiliacion = unaTerm.Afiliacion;
                        unNuevoCertificado.Terminal = unaTerm.laTerminal;
                        unNuevoCertificado.ID_ColectivaTerminal = unaTerm.ID_ColectivaTerminal;
                        unNuevoCertificado.ID_CadenaComercial = unaTerm.ID_CadenaComercial;
                        unNuevoCertificado.ClaveCadena = unaTerm.ClaveCadena;

                    }

                    losCertificados.Add(unNuevoCertificado);
                }

            }
            catch (Exception err) { Loguear.Error(err, ""); }



            return losCertificados;
        }

        public static  Dictionary<Int64,Terminal>  ObtenerTerminales(DataTable Terminales)
        {
            Dictionary<Int64, Terminal> lasTerminales = new Dictionary<Int64, Terminal>();
            try
            {

                for (int x = 0; x < Terminales.Rows.Count; x++)
                {
                    Terminal unaTerminal = new Terminal();


                    unaTerminal.Sucursal = Terminales.Rows[x]["Sucursal"].ToString();
                    unaTerminal.Afiliacion = Terminales.Rows[x]["Afiliacion"].ToString();
                    unaTerminal.laTerminal = Terminales.Rows[x]["Terminal"].ToString();
                    unaTerminal.ID_ColectivaTerminal = Int64.Parse(Terminales.Rows[x]["ID_Colectiva"].ToString());
                    unaTerminal.ID_CadenaComercial = Int64.Parse(Terminales.Rows[x]["ID_CadenaComercial"].ToString());
                    unaTerminal.ClaveCadena = Terminales.Rows[x]["ClaveCadena"].ToString();
                  
                    lasTerminales.Add( unaTerminal.ID_ColectivaTerminal, unaTerminal);
                }


            }
            catch (Exception err)
            {
                Loguear.Error(err, "");

            }

            return lasTerminales;
        }


        public static Dictionary<Int64, Certificado> ObtenerCertificados(DataTable Certificados)
        {
            Dictionary<Int64, Certificado> losCertificados = new Dictionary<Int64, Certificado>();
            try
            {

                for (int i = 0; i < Certificados.Rows.Count; i++)
                {
                    Certificado unCertificado = new Certificado();

                    try
                    {

                        unCertificado.ID_CadenaComercial = Int64.Parse(Certificados.Rows[i]["ID_CadenaComercial"].ToString());
                        unCertificado.ID_ColectivaTerminal = Int64.Parse(Certificados.Rows[i]["ID_ColectivaTerminal"].ToString());

                        unCertificado.UsuarioCreacion = Certificados.Rows[i]["UsuarioCreacion"].ToString();
                        unCertificado.Clave = Certificados.Rows[i]["Clave"].ToString();
                        unCertificado.IDMB = Certificados.Rows[i]["IDMB"].ToString();
                        unCertificado.IDPROC = Certificados.Rows[i]["IDPROC"].ToString();
                        unCertificado.MAC = Certificados.Rows[i]["MAC"].ToString();
                        unCertificado.UsuarioActivacion = Certificados.Rows[i]["UsuarioActivacion"].ToString();
                        unCertificado.ID_Estatus = Int32.Parse(Certificados.Rows[i]["ID_Estatus"].ToString());
                        unCertificado.ID_Activacion = Int32.Parse(Certificados.Rows[i]["ID_Activacion"] == null ? "0" : Certificados.Rows[i]["ID_Activacion"].ToString());
                        unCertificado.ID_Certificado = Guid.Parse(Certificados.Rows[i]["ID_Certificado"] == null ? "00000000-0000-0000-0000-000000000000" : Certificados.Rows[i]["ID_Certificado"].ToString());
                        unCertificado.FechaCreacion = DateTime.Parse(Certificados.Rows[i]["FechaCreacion"] == null ? "" : Certificados.Rows[i]["FechaCreacion"].ToString());
                        unCertificado.FechaCaducidad = DateTime.Parse(Certificados.Rows[i]["FechaCaducidad"] == null ? "" : Certificados.Rows[i]["FechaCaducidad"].ToString());
                        unCertificado.FechaActivacion = DateTime.Parse(Certificados.Rows[i]["FechaActivacion"] == null ? "" : Certificados.Rows[i]["FechaActivacion"].ToString());
                    }
                    catch (Exception err)
                    {
                        Loguear.Error(err, "");
                    }

                    losCertificados.Add(unCertificado.ID_ColectivaTerminal, unCertificado);
                }


            }
            catch (Exception err)
            {
                Loguear.Error(err, "");
            }

            return losCertificados;
        }


        public static Boolean PDVOL_ValidaCertificado(String elCertificado, String ID_Dispositivo)
        {

            Boolean laRespuesta = false;
            try
            {

                Certificado nuevoCertificado = DAOCertificado.APP_ObtieneCertificado(elCertificado);

              
                if (nuevoCertificado.Clave == null)
                {
                    // return new Respuesta(1454, "Certificado no Encontrado o Ya ha sido asignado a otra Terminal", "");
                    Loguear.Evento("No Hay Certificado con la clave " + elCertificado, "");
                    laRespuesta = false;
                    return laRespuesta;
                }




                if (!nuevoCertificado.MAC.Equals(ID_Dispositivo))
                {
                    Loguear.Evento("MAC No coindice: Dice: " + ID_Dispositivo + ", Debe Decir:" + nuevoCertificado.MAC, "");
                    laRespuesta = false;
                    return laRespuesta;
                }

             

                laRespuesta = true;

            }
            catch (Exception err)
            {
                Loguear.Error(err, "");
                laRespuesta = false;
            }

            return laRespuesta;
        }


        public static Certificado ObtenerFromDispositivo( String ID_Dispositivo)
        {

            Certificado laRespuesta = new Certificado();
            try
            {

                Certificado nuevoCertificado = DAOCertificado.APP_ObtieneCertificadoFromDispositivo(ID_Dispositivo);

                return nuevoCertificado;

            }
            catch (Exception err)
            {
                Loguear.Error(err, "");
            }

            return laRespuesta;
        }



    }
}
