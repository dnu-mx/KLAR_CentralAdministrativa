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
    public class ProductosCombosController : ApiController
    {

        [HttpGet]
        public List<DtoList> GetFamilias()
        {
            var idApp = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());
            var user = this.GetDtoUser();

            var result = DaoProductosCombos.GetFamilias(user, idApp);

            return result;
        }


        [HttpGet]
        public List<DtoProduct> GetProductos()
        {
            var idApp = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());
            var user = this.GetDtoUser();

            var result = DaoProductosCombos.GetProductosSinCombo(user, idApp);

            return result;
        }


        
        [HttpGet]
        public List<Producto> GetProductosCombos()
        {

            var idApp = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());

            var user = this.GetDtoUser();

            //if (!ApiControllerExtension.DtoUsers.Any())  return new List<Sucursal>();



            //var user = ApiControllerExtension.DtoUsers.First();

            var result = DaoProductosCombos.GetProductosDeCombos(user, idApp);

            return result;

            //return ApiControllerExtension.DtoUsers.Count.ToString();
        }

        [HttpPost]
        public void UpdateProductoToCombo(Producto producto)
        {
            var idApplication = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());

            var user = this.GetDtoUser();

            DaoProductosCombos.UpdateProductoToCombo(producto, user, idApplication);

            //return value;
        }



        
        [HttpPost]
        public int AddProductoCombo(Producto producto)
        {
            var idApplication = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());

            var user = this.GetDtoUser();

            var value=DaoProductosCombos.AddProductoCombo(producto, user, idApplication);

            return value;
        }



        [HttpPost]
        public void EditProductoCombo(Producto producto)
        {
            var idApplication = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());

            var user = this.GetDtoUser();

            DaoProductosCombos.EditProductoCombo(producto, user, idApplication);
        }


        // ReSharper disable once InconsistentNaming
        public List<PasoCombo> GetPasosCombos(int id)
        {
            var idApp = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());
            var user = this.GetDtoUser();

            var result = DaoProductosCombos.GetPasosCombos(id, user, idApp);

            return result;
        }


        public void UpdatePasosCombos(List<PasoCombo> pasosCombos)
        {

            var idApp = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());
            var user = this.GetDtoUser();

            if(pasosCombos.Count==0)
                throw new Exception("Se requiere al menos un paso ");

            DaoProductosCombos.UpdatePasosCombos(pasosCombos,user,idApp);
        }


    }
}
