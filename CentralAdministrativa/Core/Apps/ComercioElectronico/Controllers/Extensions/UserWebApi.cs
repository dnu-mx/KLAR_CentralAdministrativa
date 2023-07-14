using DALCentralAplicaciones.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ComercioElectronico.Controllers.Extensions
{
    public class UserWebApi :Usuario
    {
        public string Token { get; set; }



        public UserWebApi(Usuario usuario)
        {
            this.UsuarioId = usuario.UsuarioId;
            this .ClaveUsuario  =usuario.ClaveUsuario;

            this.Roles=usuario.Roles;
            this.UsuarioId=usuario.UsuarioId;
            this.UsuarioTemp=usuario.UsuarioTemp;

            this.Email=usuario.Email;
            this.Nombre=usuario.Nombre;
            this.APaterno=usuario.APaterno;
            this.AMaterno=usuario.AMaterno;

            this.Password=usuario.Password;
            this.ClaveColectiva=usuario.ClaveColectiva;
            this.VistaMesesAnterior=usuario.VistaMesesAnterior;
            this.ID_Colectiva=usuario.ID_Colectiva;


               this.ID_CadenaComercial=usuario.ID_CadenaComercial;
            this.ID_GrupoComercial=usuario.ID_GrupoComercial;
            this.Paramentros=usuario.Paramentros;
            this.Token=usuario.UsuarioTemp.ToString();

      /*
        
   
            this.MisAplicaciones=usuario.MisAplicaciones;
       public Dictionary<String, String> Paramentros
       {
           get
           { return _Paramentros; }
           set { _Paramentros = value; }
       }

       List<Aplicacion> MisAplicaciones = new List<Aplicacion>();

       public String RolesToString()
        {
            StringBuilder losRoles = new StringBuilder();

            foreach (string Rol in Roles)
            {

                losRoles.AppendFormat("{0},", Rol);

            }
            return losRoles.ToString();
        }*/
        }
    }
}