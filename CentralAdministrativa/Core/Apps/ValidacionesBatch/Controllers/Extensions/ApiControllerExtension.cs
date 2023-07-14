/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ValidacionesBatch.Controllers.Extensions
{
  public static class ApiControllerExtension
    {
        //public static ServiceSessions ServiceSessions = new ServiceSessions();
        public static List<UserWebApi> DtoUsers = new List<UserWebApi>();

        public static UserWebApi GetDtoUser(this ApiController controlador)
        {
            var authValue = "authToken";
            IEnumerable<string> some;//=new List<object>();
            if (!controlador.Request.Headers.TryGetValues(authValue, out some))
            {
                throw new Exception("Encabezado Sesion no enviado");
            }

            if (controlador.Request.Headers.GetValues(authValue).ToList().Count == 0)
            {
                throw new Exception("No ha iniciado session");
            }

            var token = controlador.Request.Headers.GetValues(authValue).ToList()[0];


            var dtoUserSession = DtoUsers.FirstOrDefault(m => Equals(m.Token, token));

            if (dtoUserSession == null)
                throw new Exception("Sesión no encontrada");

            var user = dtoUserSession;

            return user;
        }

        public static UserWebApi GetDtoUser(this ApiController controlador, string token)
        {
            return GetDtoUser(token);
        }

        public static UserWebApi GetDtoUser(string token)
        {
            var dtoUserSession = DtoUsers.FirstOrDefault(m => Equals(m.Token, token));

            if (dtoUserSession == null)
                throw new Exception("Sesión no encontrada");

            var user = dtoUserSession;

            return user;

        }

    

        /// <summary>
        /// METHOD
        /// <para>02/12/2015</para>
        /// <para>by Mauricio Alberto Perez Poot - Zeruel01</para>
        /// </summary>
        public static bool Verify(string token)
        {
            var dtoUserSession = DtoUsers.FirstOrDefault(m => Equals(m.Token, token));

            if (dtoUserSession == null)
                return false;

            //if (dtoUserSession.DateStart.Date.AddDays(2) < DateTime.Now)
              //  return false;



            return true;
        }

        /// <summary>
        /// METHOD
        /// <para>02/12/2015</para>
        /// <para>by Mauricio Alberto Perez Poot - Zeruel01</para>
        /// </summary>
        public static void CloseSesssion(string token)
        {
            var dtoUserSession = DtoUsers.FirstOrDefault(m => Equals(m.Token, token));

            if (dtoUserSession != null)
                DtoUsers.Remove(dtoUserSession);

        }



        internal static void AddUser(UserWebApi user)
        {
            user.Token = Guid.NewGuid().ToString();
            if(!ApiControllerExtension.DtoUsers.Exists(m => m.Token == user.Token))             
                ApiControllerExtension.DtoUsers.Add(user);
        }
    }
}*/