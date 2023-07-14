using DALCentralAplicaciones.Entidades;
using DALPuntoVentaWeb.BaseDatos;
using DALPuntoVentaWeb.Entidades;
using Interfases;
using Interfases.Exceptiones;
using Log_PCI;
using System;

namespace DALPuntoVentaWeb.LogicaNegocio
{
    public class LNAdministrarPersonasMorales
    {
        /// <summary>
        /// Establece las condiciones de validación para crear o modificar una persona moral
        /// </summary>
        /// <param name="personaMoral">Objeto PersonaMoral con los datos por crear o modificar</param>
        /// <param name="Pagina">Nombre de la página que realiza la creación o modificación</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="AppId">Identificador de la aplicación</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static long CreaOModificaPersonaMoral(PersonaMoral personaMoral, string Pagina,
            Usuario elUsuario, Guid AppID, ILogHeader logHeader)
        {
            try
            {
                return DAOAdministrarPersonasMorales.InsertaActualizaPersonaMoral(personaMoral, Pagina, 
                    elUsuario, AppID, logHeader);
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                LogPCI unLog = new LogPCI(logHeader);
                unLog.ErrorException(ex);
                throw new CAppException(8011, "Falla al crear o modificar la persona moral.");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para la creación del registro de un documento
        /// para la persona moral indicada
        /// </summary>
        /// <param name="IdPersonaMoral">Identificador de la persona moral</param>
        /// <param name="Archivo">Nombre del archivo o documento</param>
        /// <param name="Ruta">Ruta del archivo o documento</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void CreaDocumentoPersonaMoral(long IdPersonaMoral, string Archivo, string Ruta,
            Usuario elUsuario, ILogHeader logHeader)
        {
            try
            {
                DAOAdministrarPersonasMorales.InsertaDocumentoPersonaMoral(IdPersonaMoral, Archivo,
                    Ruta, elUsuario, logHeader);
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                LogPCI unLog = new LogPCI(logHeader);
                unLog.ErrorException(ex);
                throw new CAppException(8011, "Falla al crear el documento de la persona moral.");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para eliminar un registro de documento de la persona
        /// moral indicada
        /// </summary>
        /// <param name="IdDocumento">Identificador del documento</param>
        /// <param name="elUsuario">Usuario en sesión</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void BorraDocumentoPersonaMoral(long IdDocumento, Usuario elUsuario, ILogHeader logHeader)
        {
            try
            {
                DAOAdministrarPersonasMorales.EliminaDocumentoPersonaMoral(IdDocumento, elUsuario, logHeader);
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                LogPCI unLog = new LogPCI(logHeader);
                unLog.ErrorException(ex);
                throw new CAppException(8011, "Falla al eliminar el documento de la persona moral.");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para registrar en base de datos la autorización de la
        /// generación de persona moral
        /// </summary>
        /// <param name="IdPersonaMoral">Identificador de la persona moral</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void RegistraAutorizacionPersonaMoral(long IdPersonaMoral, IUsuario elUsuario, ILogHeader logHeader)
        {
            try
            {
                DAOAdministrarPersonasMorales.InsertaAutorizacionPersonaMoral(IdPersonaMoral, elUsuario, logHeader);
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                LogPCI unLog = new LogPCI(logHeader);
                unLog.ErrorException(ex);
                throw new CAppException(8011, "Falla al registrar la autorización de la persona moral.");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para modificar el estatus de la persona moral indicada
        /// a En Revisión
        /// </summary>
        /// <param name="idPersonaMoral">Identificador de la persona moral</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void ModificaEstatusPM_EnRevision(long IdPersonaMoral, IUsuario elUsuario, ILogHeader logHeader)
        {
            try
            {
                DAOAdministrarPersonasMorales.ActualizaEstatusPM_EnRevision(IdPersonaMoral, elUsuario, logHeader);
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                LogPCI unLog = new LogPCI(logHeader);
                unLog.ErrorException(ex);
                throw new CAppException(8011, "Falla al establecer la persona moral en estatus de revisión.");
            }
        }

        /// <summary>
        /// Establece las condiciones de validación para modificar el estatus de la persona moral indicada
        /// a En Proceso
        /// </summary>
        /// <param name="idPersonaMoral">Identificador de la persona moral</param>
        /// <param name="logHeader">Instancia heredada del LogHeader para PCI</param>
        public static void ModificaEstatusPM_EnProceso(long IdPersonaMoral, IUsuario elUsuario, ILogHeader logHeader)
        {
            try
            {
                DAOAdministrarPersonasMorales.ActualizaEstatusPM_EnProceso(IdPersonaMoral, elUsuario, logHeader);
            }
            catch (CAppException err)
            {
                throw err;
            }
            catch (Exception ex)
            {
                LogPCI unLog = new LogPCI(logHeader);
                unLog.ErrorException(ex);
                throw new CAppException(8011, "Falla al establecer la persona moral en estatus de revisión.");
            }
        }
    }
}
