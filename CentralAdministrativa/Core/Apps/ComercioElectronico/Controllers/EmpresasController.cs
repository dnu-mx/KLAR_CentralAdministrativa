using ComercioElectronico.Controllers.Extensions;
using DALComercioElectronico.BaseDatos;
using DALComercioElectronico.Entidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;

namespace ComercioElectronico.Controllers
{
    public class EmpresasController : ApiController
    {
     
        [HttpGet]
        public List<Empresa> GetEmpresas()
        {

            var id = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());

            var user = this.GetDtoUser();

            //if (!ApiControllerExtension.DtoUsers.Any())  return new List<Empresa>();
            
            

            //var user = ApiControllerExtension.DtoUsers.First();

            var result = DaoEmpresas.GetEmpresas(user, id);

            return result; 

            //return ApiControllerExtension.DtoUsers.Count.ToString();
        }

        [HttpPost]
        public void AddEmpresa(Empresa empresa)
        {
            var idApplication = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());

            var user =  this.GetDtoUser();

            var claves = DaoEmpresas.GetEmpresasClaves();

            if (claves.Any())
            {
                claves.Sort();
                var lasClave = claves.Last();
                var numericStr = lasClave.Remove(0,1);
                var value = int.Parse(numericStr);

                empresa.ClaveEmpresa = (++value).ToString("'C'000");

            }
            else
            {
                empresa.ClaveEmpresa = "C001";
            }

            if (empresa.AsociarCorreo)
            {
                if(empresa.DominiosCorreo==null|| string.IsNullOrWhiteSpace(empresa.DominiosCorreo))
                    throw new Exception("Se requieren correos de dominio ");
            }
            else
            {
                empresa.DominiosCorreo = null;
            }

            DaoEmpresas.AddEmpresa(empresa, user, idApplication);
        }



        [HttpPost]
        public void EditEmpresa(Empresa empresa)
        {
            var idApplication = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());

            var user = this.GetDtoUser();

            if (empresa.AsociarCorreo)
            {
                if (empresa.DominiosCorreo == null || string.IsNullOrWhiteSpace(empresa.DominiosCorreo))
                    throw new Exception("Se requieren correos de dominio ");
            }
            else
            {
                empresa.DominiosCorreo = null;
            }


            DaoEmpresas.EditEmpresa(empresa, user, idApplication);
        }




    }
}
