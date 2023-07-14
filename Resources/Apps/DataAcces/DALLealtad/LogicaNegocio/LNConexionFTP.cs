using DALCentralAplicaciones.Utilidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace DALLealtad.LogicaNegocio
{
    /// <summary>
    /// Establece la lógica del negocio para las conexiones a servidores FTP
    /// </summary>
    public class LNConexionFTP
    {
        /// <summary>
        /// Verifica la existencia de un sólo archivo en el servidor que corresponde al programa
        /// </summary>
        /// <param name="Programa">Programa u otorgante de las promociones</param>
        /// <param name="Archivo">Nombre del archivo por buscar</param>
        /// <returns>TRUE si encuentra el archivo</returns>
        public static bool VerificaSiExisteArchivoImagen(string Programa, string Archivo)
        {
            string server = "", user = "", pswd = "";

            try
            {
                server = Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                    "FTP_" + Programa + "_ServidorLogos").Valor.ToString();
                user = Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                    "FTP_" + Programa + "_User").Valor.ToString();
                pswd = Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                    "FTP_" + Programa + "_Password").Valor.ToString();

                var request = (FtpWebRequest)WebRequest.Create("ftp://" + server + Archivo);

                request.EnableSsl = true;
                request.Credentials = new NetworkCredential(user, pswd);
                request.Method = WebRequestMethods.Ftp.GetFileSize;

                try
                {
                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                    return true;
                }
                catch (WebException ex)
                {
                    FtpWebResponse response = (FtpWebResponse)ex.Response;

                    if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    {
                        return false;
                    }
                    else
                    {
                        throw new Exception("Ocurrió un error en la consulta FTP " + ex.Message);
                    }
                }
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, "");
                throw ex;
            }
        }

        /// <summary>
        /// Consulta la lista de archivos en el servidor que corresponden al programa y
        /// añade la leyenda SI o NO, según exista el archivo, en la columna del
        /// DataTable asignada al Programa
        /// </summary>
        /// <param name="Programa">Programa u otorgante de las promociones</param>
        /// <param name="dtFiles">Objeto DataTable con las claves de cadenas (archivos)
        /// por buscar</param>
        public static void ValidaArchivosCadenas(string Programa, string Columna, string Extension, DataTable dtFiles)
        {
            string server = "", user = "", pswd = "", ssl = "";

            try
            {
                server = Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                    "FTP_" + Programa + "_ServidorLogos").Valor.ToString();
                user = Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                    "FTP_" + Programa + "_User").Valor.ToString();
                pswd = Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                    "FTP_" + Programa + "_Password").Valor.ToString();
                ssl = Configuracion.Get(Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString()),
                    "FTP_" + Programa + "_SSL").Valor.ToString();

                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create("ftp://" + server);

                ftpRequest.EnableSsl = ssl.ToUpper() == "SI" ? true : false;
                ftpRequest.Credentials = new NetworkCredential(user, pswd);
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;


                FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse();
                StreamReader streamReader = new StreamReader(response.GetResponseStream());

                List<string> archivos = new List<string>();
                string NombreArchivo = streamReader.ReadLine();

                Loguear.Evento(Programa + " - Inicia ciclo FTP.", "");

                while (!string.IsNullOrEmpty(NombreArchivo))
                {
                    NombreArchivo = streamReader.ReadLine();
                    archivos.Add(NombreArchivo);
                }

                Loguear.Evento(Programa + " - Finaliza ciclo FTP.", "");
                streamReader.Close();

                if (!dtFiles.Columns.Contains(Columna))
                    dtFiles.Columns.Add(Columna);

                Loguear.Evento(Programa + " - Inicia escritura DataTable.", "");

                foreach (DataRow cadena in dtFiles.Rows)
                {
                    if (archivos.Contains(cadena["ClaveCadena"].ToString() + Extension))
                    {
                        cadena[Columna] = "SI";
                    }
                    else
                    {
                        cadena[Columna] = "NO";
                    }
                }

                Loguear.Evento(Programa + " - Finaliza escritura DataTable.", "");
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, "");
                throw ex;
            }
        }

        /// <summary>
        /// Consulta la lista de directorios en el servidor, estableciendo su nombre y su tipo
        /// </summary>
        /// <param name="Servidor">Servidor y carpeta del repositorio FTP</param>
        /// <param name="Usuario">Usuario para conectarse al repositorio FTP</param>
        /// <param name="Password">Contraseña para conectarse al repositorio FTP</param>
        /// <param name="SSL">Bandera que indica si se requiere o no el uso de SLL en la conexión FTP</param>
        /// <returns>Lista con los nombres del directorio</returns>
        public static List<object> ObtieneListaDirectoriosFTP(string Servidor, string Usuario, 
            string Password, string SSL)
        {
            try
            {
                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create("ftp://" + Servidor);

                ftpRequest.EnableSsl = SSL.ToUpper() == "SI" ? true : false;
                ftpRequest.Credentials = new NetworkCredential(Usuario, Password);
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                ftpRequest.KeepAlive = false;

                FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse();

                List<object> directorio = new List<object>();
                Regex regex = new Regex(@"(^\d{2}[-]\d{2}[-]\d{2}\s+\d{2}:\d{2}\w+)(\s+[<DIR>]{0,})(\s+\d{0,})(\s+\w+.+)");

                using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                {
                    while (!streamReader.EndOfStream)
                    {
                        string line = streamReader.ReadLine();

                        Match match = regex.Match(line);

                        directorio.Add(new
                        {
                            NombreArchivo = match.Groups[4].Value.Trim(),
                            Tamanyo = match.Groups[3].Value.Trim(),
                            Tipo = String.IsNullOrEmpty(match.Groups[2].Value.Trim()) ? "Archivo" : "Carpeta"
                        });
                    }
                }

                return directorio;
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, "");
                throw ex;
            }
        }

        /// <summary>
        /// Realiza la carga de una lista de archivos en el servidor FTP indicado
        /// </summary>
        /// <param name="Servidor">Servidor y carpeta del repositorio FTP</param>
        /// <param name="Usuario">Usuario para conectarse al repositorio FTP</param>
        /// <param name="Password">Contraseña para conectarse al repositorio FTP</param>
        /// <param name="SSL">Bandera que indica si se requiere o no el uso de SLL en la conexión FTP</param>
        /// <param name="fileList">Lista de archivos a subir</param>
        /// <param name="path">Ruta origen de los archivos en la máquina del cliente</param>
        public static void SubeArchivosADirectorioFTP(string Servidor, string Usuario, string Password, string SSL,
            List<string> fileList, string path)
        {
            try
            {
                Loguear.Evento("INICIA CARGA DE ARCHIVOS FTP. No. ARCHIVOS: " + fileList.Count, "");

                foreach (string FileName in fileList)
                {
                    FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create("ftp://" + Servidor + "/" + FileName);

                    ftpRequest.EnableSsl = SSL.ToUpper() == "SI" ? true : false;
                    ftpRequest.Credentials = new NetworkCredential(Usuario, Password);
                    ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                    ftpRequest.Timeout = -1;

                    using (Stream fileStream = File.OpenRead(path + "\\" + FileName))
                    using (Stream ftpStream = ftpRequest.GetRequestStream())
                    {
                        fileStream.CopyTo(ftpStream);
                    }
                }

                Loguear.Evento("SE CARGARON " + fileList.Count + " ARCHIVOS CON EXITO.", "");
            }

            catch (Exception ex)
            {
                Loguear.Error(ex, "");
                throw ex;
            }
        }
    }
}
