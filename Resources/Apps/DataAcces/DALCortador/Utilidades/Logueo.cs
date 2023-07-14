using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using log4net.Config;

namespace DALEventos.Utilidades
{
   public static class Loguear
    {

          //private static readonly ILog _loggerError = LogManager.GetLogger("");
        private static readonly log4net.ILog _loggerError = log4net.LogManager.GetLogger(ConfigurationManager.AppSettings["LogError"]);
        private static readonly log4net.ILog _loggerEventos = log4net.LogManager.GetLogger(ConfigurationManager.AppSettings["LogEvento"]);
        private static readonly log4net.ILog _loggerRecibidos = log4net.LogManager.GetLogger(ConfigurationManager.AppSettings["LogEntrada"]);

        static Loguear()
        {
            XmlConfigurator.Configure();
        }

        public static void Error(Exception Error, String User)
        {
            _loggerError.Error("[ " + User + "]  " + Error.Message);// + ". ORIGINAL: " + Error.InnerException == null ? "" : Error.InnerException.Message);

        }

        public static void Error(Interfases.Exceptiones.CAppException Error, String User)
        {
            _loggerError.Error("[ " + User + "]  " + Error.Mensaje() + ". ORIGINAL: " + Error.MensajeOriginal());

        }

        public static void EntradaSalida(string message, String User, Boolean esEntrada)
        {
            String leyenda = esEntrada ? "ENTRADA <<" : "SALIDA >>";
            _loggerRecibidos.Error("[ " + User + "]  " + leyenda + message);
        }

        public static void Evento(String Evento, String User)
        {
            _loggerEventos.Error("[ " + User + "]  " + Evento);
        }


           
        }

}
