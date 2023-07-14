using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ComercioElectronico.Controllers.Extensions;
using DALCentralAplicaciones;
using  DALComercioElectronico;
using DALComercioElectronico.BaseDatos;
using DALComercioElectronico.Entidades;
using Ext.Net;

namespace ComercioElectronico
{
    public partial class CombosExtNet : PaginaBaseCAPP
    {
        //public int CurrentProductoId;
        //public int  current_idPaso = 0;

        public List<DtoProduct> ProductosSinCombo = new List<DtoProduct>();
        protected void Page_Load(object sender, EventArgs e)
        {

            Session["Ext.Net.Theme"] = Ext.Net.Theme.Default;
            
        }
        

        public  List<Producto> GetProductos()
        {
            var user = this.Usuario;

            var idApp = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());

            var values = DaoProductosCombos.GetProductosDeCombos(user, idApp);

            return values;
        }

        public void LoadPasos(int id=0)
        {

            this.currentPasoId.SetValue(0);

            if (id == 0)
            this.productosGridPanel.Title = "Productos de paso: ";

            var user = this.Usuario;
            var idApp = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());

            var value = int.Parse(this.currentProductId.Value.ToString());
            var pasos = DaoProductosCombos.GetPasosCombos(value, user, idApp);
            this.pasosGridPanel.Store.Primary.DataSource = pasos;
            //this.pasosGridPanel.Store.Primary.DataBind();
            this.pasosGridPanel.DataBind();
            if(id==0)
                LoadProductosCombos();
        }

        public void LoadProductosCombos()
        {
            var value = int.Parse(this.currentPasoId.Value.ToString());

            /*if (value == 0)
            {
                this.productosGridPanel.ClearContent();
            }*/

            var user = this.Usuario;
            var idApp = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());
            //var value = ;
            var pasos = DaoProductosCombos.GetProductosCombos(value, user, idApp);
            this.productosGridPanel.Store.Primary.DataSource = pasos;
            //this.pasosGridPanel.Store.Primary.DataBind();
            this.productosGridPanel.DataBind();
        }

     
     

        public void OpenProductForm(object sender, DirectEventArgs e)
        {
            //this.AddProductForm.
            this.AddProductForm.Show();
        }

        public void OpenProductForm()
        {
            //this.AddProductForm.
            this.AddProductForm.Show();
        }


        protected void AddProductEvent(object sender, DirectEventArgs e)
        {

            this.AddProductView.Show();

            var user = this.Usuario;

            var idApp = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());

            ProductosSinCombo = DaoProductosCombos.GetProductosSinCombo(user, idApp);

            this.ProductosStore.DataSource = ProductosSinCombo;
            this.ProductosStore.DataBind();

            //this.FormPanel1.SetValues();



            //string employeeID = e.ExtraParams["EmployeeID"];

            //Employee empl = Employee.GetEmployee(int.Parse(employeeID));

            /*this.FormPanel1.SetValues(new
            {
                empl.EmployeeID,
                empl.FirstName,
                empl.LastName,
                empl.Title,
                ReportsTo = empl.ReportsTo.HasValue ? (Employee.GetEmployee(empl.ReportsTo.Value).LastName) : "",
                empl.HireDate,
                empl.Extension,
                empl.Address,
                empl.City,
                empl.PostalCode,
                empl.HomePhone,
                empl.TitleOfCourtesy,
                empl.BirthDate,
                empl.Region,
                empl.Country,
                empl.Notes
            });*/
        }

        protected void EditProductEvent(object sender, DirectEventArgs e)
        {
                 var user = this.Usuario;

            var idApp = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());


            if (e.ExtraParams["CommandName"] == "Pasos")
            {
                

                var data = e.ExtraParams["Data"];
                var producto = data.FromJsonStringNull<Producto>();

                var id= (Ext.Net.TextField)EditPasosWindow.Items.First(m => m.TagHiddenName == "id_producto");
                var nameField = (Ext.Net.TextField)EditPasosWindow.Items.First(m => m.TagHiddenName == "nombre");

                this.currentProductId.SetValue(producto.producto_id);
                //this.CurrentProductoId = producto.producto_id;
                id.SetValue(producto.producto_id);
                nameField.SetValue(producto.nombre);
                //hidden.Value = producto.producto_id;

                EditPasosWindow.Title = "Editar pasos producto " + producto.nombre;

                LoadPasos();
                
                
                EditPasosWindow.Show();


            }
            else if (e.ExtraParams["CommandName"] == "Editar")
            {

                var data = e.ExtraParams["Data"];

                var producto = data.FromJsonStringNull<Producto>();
                this.EditProductForm.SetValues(producto);

                this.EditProductWindow.Show();
            }


            //var user = this.Usuario;

            //var idApp = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());

            //ProductosSinCombo = DaoProductosCombos.GetProductosSinCombo(user, idApp);

            //this.ProductosStore.DataSource = ProductosSinCombo;
            //this.ProductosStore.DataBind();

            //this.FormPanel1.SetValues();



        }


        protected void AddStepEvent(object sender, DirectEventArgs e)
        {

            //this.pasosGridPanel.

            

            this.AddStepWindow.Title = "Agregar paso";

            var idProducto = int.Parse( this.currentProductId.Value.ToString());
                //e.ExtraParams["id_producto"];

            this.AddStepForm.Reset();
            this.AddStepForm.SetValues(new { id_producto = idProducto ,id=0 });



            this.AddStepWindow.Show();

            //var user = this.Usuario;
            //var idApp = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());
            //ProductosSinCombo = DaoProductosCombos.GetProductosSinCombo(user, idApp);
            //this.ProductosStore.DataSource = ProductosSinCombo;
            //this.ProductosStore.DataBind();
            //this.FormPanel1.SetValues();




            }

        


        protected void EditStepEvent(object sender, DirectEventArgs e)
        {
            var data = e.ExtraParams["Data"];
            var pasoCombo = data.FromJsonStringNull<PasoCombo>();


            if (e.ExtraParams["CommandName"] == "Editar")
            {


                this.AddStepWindow.Title = "Editar paso";
                this.AddStepForm.SetValues(pasoCombo);

                AddStepWindow.Show();
            }


            else if (e.ExtraParams["CommandName"] == "Eliminar")
            {

                var value = new Ext.Net.MessageBox().Confirm("Borrar paso", "¿confirma borrar este paso ?"
                    ,
                    new MessageBoxButtonsConfig()
                    {
                        Yes = new MessageBoxButtonConfig()
                        {

                            Handler = "App.direct.DeleteStep('"+data+"')",
                            Text = "Eliminar",
                            
                            
                        }
         ,
            No = new MessageBoxButtonConfig
            {
                //Handler = "Assignments.DoNo()",
                Text = "Cancelar"
            }
                    }
                    
                    );

                
                
               value.Show();
                
                /*var data = e.ExtraParams["Data"];

                var producto = data.FromJsonStringNull<Producto>();
                this.EditProductForm.SetValues(producto);

                this.EditProductWindow.Show();*/
            }


            //var user = this.Usuario;

            //var idApp = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());

            //ProductosSinCombo = DaoProductosCombos.GetProductosSinCombo(user, idApp);

            //this.ProductosStore.DataSource = ProductosSinCombo;
            //this.ProductosStore.DataBind();

            //this.FormPanel1.SetValues();



        }



        protected void AddProduct(object sender, DirectEventArgs e)
        {
            var vals = e.ExtraParams["values"];

            var producto= vals.FromJsonString<Producto>();

            var user = this.Usuario;

            var idApp = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());

            DaoProductosCombos.UpdateProductoToCombo(producto,user, idApp);
         

            this.AddProductForm.Reset();
            this.AddProductView.Hide();
            this.gridPanelBase.DataBind();

        }

        protected void EditProducto(object sender, DirectEventArgs e)
        {
            var vals = e.ExtraParams["values"];

            var producto = vals.FromJsonString<Producto>();

            if (producto.IdCombo == 0) producto.IdCombo = null;
            var user = this.Usuario;

            var idApp = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());

            DaoProductosCombos.UpdateProductoToCombo(producto, user, idApp);
            //this.AddProductForm.
            //this.AddProductForm.find

            //asd.LoadRecord();
            //producto.IdCombo = 

            this.EditProductForm.Reset();
            this.EditProductWindow.Hide();


            //this.;

            //this.gridPanelBase.Refresh();
            //ObjectDataSource1.DataBind();
            this.gridPanelBase.DataBind();



        }


        protected void AddStep(object sender, DirectEventArgs e)
        {
            var vals = e.ExtraParams["values"];
            

            var pasoCombo = vals.FromJsonStringNull<PasoCombo>();

            //var value = pasoCombo.id;

            var user = this.Usuario;

            var idApp = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());

            DaoProductosCombos.UpdatePasoComboV2(pasoCombo, user, idApp);


            this.AddStepWindow.Hide();
            this.AddStepForm.Reset();

            this.LoadPasos(pasoCombo.id);

/*            this.LoadProductosCombos();
            

            if (pasoCombo.id > 0)
            {
                this.productosGridPanel.Title = " Productos de paso: " + pasoCombo.descripcion;
                this.currentPasoId.SetValue(pasoCombo.id);

                this.LoadProductosCombos();

            }*/
            //this.currentProductId.SetValue();
            //this.pasosGridPanel.DataBind();
            
            /*
         

            var producto = vals.FromJsonString<Producto>();

            var user = this.Usuario;

            var idApp = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());

            DaoProductosCombos.UpdateProductoToCombo(producto, user, idApp);
            //this.AddProductForm.
            //this.AddProductForm.find

            //asd.LoadRecord();
            //producto.IdCombo = 

            this.AddProductForm.Reset();
            this.AddProductView.Hide();


            //this.;

            //this.gridPanelBase.Refresh();
            //ObjectDataSource1.DataBind();
            this.gridPanelBase.DataBind();
            */


        }

        protected void DeleteProductStep(object sender, DirectEventArgs e)
        {
            var vals = e.ExtraParams["values"];


            var productoCombo = vals.FromJsonStringNull<ProductoCombo>();
            var user = this.Usuario;

            var idApp = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());

        }

        protected void RowSelectStep(object sender, DirectEventArgs e)
        {
            var vals = e.ExtraParams["Data"];
            var pasoCombo = vals.FromJsonStringNull<PasoCombo>();
            this.currentPasoId.SetValue(pasoCombo.id);

            this.productosGridPanel.Title = "Productos de paso: " + pasoCombo.descripcion;
            this.LoadProductosCombos();


        }


/*
                protected void DeleteStep(object sender, DirectEventArgs e)
                {
        
                    var vals = e.ExtraParams["values"];
                    
                    var pasoCombo = vals.FromJsonStringNull<PasoCombo>();
        
                    var user = this.Usuario;
                    var idApp = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());
        
                    DaoProductosCombos.UpdatePasoComboV2(pasoCombo, user, idApp,true);
        
                    LoadPasos();
        
                }*/


        [DirectMethod]
        public void DeleteStep(string data)
        {

            var pasoCombo = data.FromJsonStringNull<PasoCombo>();
            var user = this.Usuario;
            var idApp = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());
            DaoProductosCombos.UpdatePasoComboV2(pasoCombo, user, idApp, true);

            LoadPasos();
        }





        protected void AddProductStepEvent(object sender, DirectEventArgs e)
        {

            var value = int.Parse(this.currentPasoId.Value.ToString());

            if(value<=0)
                return;
            


            this.AddProductStepWindow.Show();

            var user = this.Usuario;

            var idApp = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());

            ProductosSinCombo = DaoProductosCombos.GetProductosSinCombo(user, idApp);

            
             //var vals = e.ExtraParams["Data"];
             //var element = vals.FromJsonString<PasoCombo>();

            this.AddProductStepForm.SetValues(new { id_pasos_combos = value });

            this.ProductosStore.DataSource = ProductosSinCombo;
            this.ProductosStore.DataBind();

        
        }
        protected void AddProductStep(object sender, DirectEventArgs e)
        {
            var vals = e.ExtraParams["Data"];

            var element = vals.FromJsonString<ProductoCombo>();

            var user = this.Usuario;

            var idApp = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());

            DaoProductosCombos.UpdateProductoCombo(element, user, idApp);
            //this.AddProductForm.
            //this.AddProductForm.find

            //asd.LoadRecord();
            //producto.IdCombo = 

            this.AddProductStepForm.Reset();
            this.AddProductStepWindow.Hide();


            //this.;

            //this.gridPanelBase.Refresh();
            //ObjectDataSource1.DataBind();
            //this.productosGridPanel.DataBind();
            this.LoadProductosCombos();
            //this.currentPasoId.SetValue(element.id_producto);




        }

        protected void DeleteProductStepEvent(object sender, DirectEventArgs e)
        {
            var vals = e.ExtraParams["Data"];

            var element = vals.FromJsonString<ProductoCombo>();

            var user = this.Usuario;

            var idApp = Guid.Parse(ConfigurationManager.AppSettings["IDApplication"].ToString());

            DaoProductosCombos.UpdateProductoCombo(element, user, idApp,true);

            this.LoadProductosCombos();
        }
        

    }

}