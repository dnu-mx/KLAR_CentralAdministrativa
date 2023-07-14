using ComercioElectronico.Controllers.Extensions;
using DALComercioElectronico.BaseDatos;
using DALComercioElectronico.Entidades;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ComercioElectronico.Controllers
{
    public class SucursalesController : ApiController
    {
     
        [HttpGet]
        public List<Sucursal> GetSucursales()
        {

            var id = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());

            var user = this.GetDtoUser();

            //if (!ApiControllerExtension.DtoUsers.Any())  return new List<Sucursal>();
            
            

            //var user = ApiControllerExtension.DtoUsers.First();

            var result = DaoSucursales.GetSucursales(user, id);

            return result; 

            //return ApiControllerExtension.DtoUsers.Count.ToString();
        }

        [HttpPost]
        public void AddSucursal(Sucursal sucursal)
        {
            var idApplication = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());

            var user =  this.GetDtoUser();

             DaoSucursales.AddSucursal(sucursal ,user,idApplication);
        }



        [HttpPost]
        public void EditSucursal(Sucursal sucursal)
        {
            var idApplication = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());

            var user = this.GetDtoUser();

            DaoSucursales.EditSucursal(sucursal, user, idApplication);
        }



        [HttpPost]
        public void EditSucursalSub(Sucursal sucursal)
        {
            var idApplication = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());

            var user = this.GetDtoUser();

            DaoSucursales.EditSucursalSub(sucursal, user, idApplication);
        }

        [HttpGet]
        public SucursalHorario GetSucursalHorario(int id)
        {
            var idApplication = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());

            var user = this.GetDtoUser();

            var data=DaoSucursales.GetSucursalHorario(id, user, idApplication);

            return data;            
        }





  [HttpPost]
        public void UpdateSucursalHorario(SucursalHorario sucursalHorario)
        {
            var idApplication = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());

            var user = this.GetDtoUser();

             DaoSucursales.UpdateSucursalHorario(sucursalHorario, user, idApplication);

            //return data;
        }



          [HttpGet]
        public List<AreaServicio> GetCoverage(int id)
          {
              var idApplication = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());

              var user = this.GetDtoUser();
              var data= DaoSucursales.GetCoverage(id, user, idApplication);

              return data;





              //return data;
          }


          [HttpGet]
          public List<Asentamiento> GetAsentamientosFilter(string id)
          {
              var idApplication = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());

              var user = this.GetDtoUser();
              var data = DaoSucursales.GetAsentamientosFilter(id, user, idApplication);

              foreach (var asentamiento in data)
              {
                  asentamiento.DesAsentamiento = asentamiento.DesAsentamiento.Trim();
              }
              return data;





              //return data;
          }


          [HttpPost]
          public void UpdateCoverage(DtoSucursal dtoSucursal)
          {
              var idApplication = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());

              var user = this.GetDtoUser();

              DaoSucursales.UpdateCoverage(dtoSucursal, user, idApplication);

              //return data;
          }


    }
}
