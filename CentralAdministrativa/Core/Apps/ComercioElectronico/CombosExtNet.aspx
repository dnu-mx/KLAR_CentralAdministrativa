<%@ Page Language="C#" AutoEventWireup="true"  CodeBehind="CombosExtNet.aspx.cs" Inherits="ComercioElectronico.CombosExtNet" %>

<!DOCTYPE html>

<html lang="en" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    
    
    <style>
        .x-grid-row-over .x-grid-cell-inner {
            font-weight : bold;
        }
    </style>
</head>
<body>
    <form runat="server">
        <ext:ResourceManager runat="server" />

        <asp:ObjectDataSource
            ID="ObjectDataSource1"
            runat="server"
            SelectMethod="GetProductos"
            TypeName="ComercioElectronico.CombosExtNet"
            
            
            />
        

        
    
        
        
             <ext:Store ID="ProductosStore" runat="server">
                      <Model>
                            <ext:Model runat="server" IDProperty="id">
                        <Fields>
                            <ext:ModelField Name="id" Type="String" ServerMapping="Id" />
                            <ext:ModelField Name="name" Type="String" ServerMapping="Name" />
                        </Fields>
                    </ext:Model>

                      </Model>
                    </ext:Store>
        
           
<%--        
             <ext:Store ID="PasosStore" runat="server">
                      <Model>
                            <ext:Model runat="server" IDProperty="id">
                        <Fields>
                            <ext:ModelField Name="id" Type="String" ServerMapping="id" />
                            <ext:ModelField Name="descripcion" Type="String" ServerMapping="descripcion" />
                        </Fields>
                    </ext:Model>

                      </Model>
                    </ext:Store>--%>


       
        
        <ext:Viewport runat="server" Layout="Fit">
            <Items>
                
                <ext:NumberField runat="server" ID="currentProductId" Hidden="true"></ext:NumberField>
                <ext:NumberField runat="server" ID="currentPasoId" Hidden="true" ></ext:NumberField>

        <ext:GridPanel
            ID="gridPanelBase"
            runat="server"
            Title="Productos"
            Frame="true"
            Height="600"
            
            >
            <Store>
                <ext:Store runat="server" DataSourceID="ObjectDataSource1">
                    <Model>
                        <ext:Model runat="server" IDProperty="producto_id">
                            <Fields>
                                <ext:ModelField Name="producto_id" Type="Int" />
                                <ext:ModelField Name="sku" />
                                <ext:ModelField Name="nombre"    />
                                <ext:ModelField Name="fecha_Insertado" Type="Date" />
                                
                                 <ext:ModelField Name="IdCombo" Type="Int" />
                                <ext:ModelField Name="secuencia" Type="Int" />

                                <ext:ModelField Name="activo" Type="Boolean" />
                                

                                <%--<ext:ModelField Name="LastName" />--%>
                            <%--    <ext:ModelField Name="Title" />
                                <ext:ModelField Name="TitleOfCourtesy" />
                                <ext:ModelField Name="BirthDate" Type="Date" />
                                <ext:ModelField Name="HireDate" Type="Date" />
                                <ext:ModelField Name="Address" />--%>
               
                            </Fields>
                        </ext:Model>
                    </Model>
                </ext:Store>
            </Store>
            <ColumnModel runat="server">
                <Columns>
                    
                     <ext:Column runat="server" DataIndex="producto_id" Text="Id" MinWidth="10" />
                    <ext:Column runat="server" DataIndex="sku" Text="SKU" MinWidth="20" />
                    <ext:Column runat="server" DataIndex="nombre" Text="Nombre" MinWidth="150" />
                    <%--<ext:DateColumn runat="server" DataIndex="fecha_Insertado" Text="Fecha" MinWidth="100" Format="yyyy-MM-dd" ></ext:DateColumn>--%>
                    
                    <ext:Column runat="server" DataIndex="IdCombo" Text="IdCombo" MinWidth="10" />
                    <ext:CheckColumn  runat="server"  DataIndex="activo" Text="Activo"  ></ext:CheckColumn>
                    
                       <ext:CommandColumn runat="server" MinWidth="150">
                        <Commands>
                        
                            
                              <ext:GridCommand Icon="TextListNumbers" CommandName="Pasos"  Text="Pasos"  >
                                <%--<ToolTip Text="Pasos" />--%>
                            </ext:GridCommand>

                            <ext:CommandSeparator />
                            

                            <ext:GridCommand Icon="NoteEdit" CommandName="Editar"  Text="Editar"   >
                                

                                <%--<ToolTip Text="Editar" />--%>
                            </ext:GridCommand>
                            

                        </Commands>
                           
                           <DirectEvents>
                               <command OnEvent="EditProductEvent" >
                                   
                                   <ExtraParams>
                                        <ext:Parameter Name="Data" Value="record.data" Encode="true" Mode="Raw" />
                                          <ext:Parameter Name="CommandName" Value="command" Mode="Raw" />
                                   </ExtraParams>
                                       

                               </command>


                           </DirectEvents>
                           

                        <Listeners>
                            <Command Handler="">
                                
                                

                            </Command>
                        </Listeners>
                           
                    </ext:CommandColumn>

                    
             <%--       <ext:Column ID="fullName" runat="server" Text="Full Name" Width="150" DataIndex="LastName">
                        <Renderer Fn="fullName" />
                    </ext:Column>--%>

<%--                    <ext:Column runat="server" DataIndex="Title" Text="Title" Width="150" />
                    <ext:Column runat="server" DataIndex="TitleOfCourtesy" Text="Title Of Courtesy" Width="150" />
                    <ext:DateColumn runat="server" DataIndex="BirthDate" Text="BirthDate" Width="110" Format="yyyy-MM-dd" />
                    <ext:DateColumn runat="server" DataIndex="HireDate" Text="HireDate" Width="110" Format="yyyy-MM-dd" />
                    <ext:Column runat="server" DataIndex="Address" Text="Address" Width="150" />--%>
         
                </Columns>
            </ColumnModel>
            

            <View>
              
                <ext:GridView runat="server" LoadMask="true" LoadingText="Cargando" />
            </View>
            
            
            

            <SelectionModel>
                <%--<ext:RowSelectionModel runat="server" Mode="Multi" />--%>
            </SelectionModel>
            
                 <BottomBar>
                       
                     

                        <ext:PagingToolbar
                            runat="server"
                            DisplayInfo="true"
                            DisplayMsg="Mostrando productos {0} - {1} of {2}"
                            >
                            
                            <Items>
                    <ext:Button runat="server" Text="Agregar" Icon="Add" >
                          <DirectEvents>
                    <Click OnEvent="AddProductEvent" />
                </DirectEvents>

                    </ext:Button>
                </Items>
                        </ext:PagingToolbar>
                     

                    </BottomBar>
           
        </ext:GridPanel>
                           </Items>
        </ext:Viewport>
        
        
        <%--Width="400"
        Height="295"
            
            Title="Form View"
            Width="400"
            Height="240"
            BodyPadding="10"
            Resizable="false"
            Closable="false"
            Layout="Fit"
            
            AutoScroll="true"
                    
        AutoDataBind="true"
                 BodyPadding="10"
            --%>

          <ext:Window
                ID="AddProductView"
        runat="server"
        Title="Agregar Producto"
              Hidden="true"
            Width="400"
            Height="240"
            BodyPadding="10"
            Resizable="true"
            Closable="true"
            Layout="Fit"
        >
              
              <Items>
              
              
                  <%--Split="true"--%>
                  <%--Title="Agregar Producto" Icon="User"--%>
                  <%--ProductosStore        QueryMode="Local" Region="East"--%>
                     <ext:FormPanel
                    ID="AddProductForm"
                    runat="server"
                    
                    
                    MarginSpec="0 5 5 5"
                    BodyPadding="2"
                    Frame="false"
                    Border="false"
                    BodyStyle="background-color:transparent"
                    Width="280"
                    DefaultAnchor="100%"
                    AutoScroll="True"
                         
                         >
                    <FieldDefaults ReadOnly="false" />
                    <Items>
                        <ext:ComboBox
                              
                runat="server"
                FieldLabel="Producto"
                DisplayField="name"
                Width="320"
                LabelWidth="130"
                Name="producto_id"
                ValueField="id"
                TypeAhead="true"
                              Store="ProductosStore"
                              >
                    
            </ext:ComboBox>

                        <ext:NumberField runat="server" FieldLabel="IdCombo"   Name="IdCombo" MinValue="0"    AllowBlank="False"/>
                
                    </Items>
                         
                          <Buttons >
                    <ext:Button
                        runat="server"
                        Text="Guardar"
                        Disabled="true"
                        FormBind="true" 
                        
                        >
                        
                         <DirectEvents>
                        <Click OnEvent="AddProduct" Before="return #{AddProductForm}.isValid();">
                            <ExtraParams>
                                <ext:Parameter
                                    Name="values"
                                    Value="#{AddProductForm}.getForm().getValues()"
                                    Mode="Raw"
                                    Encode="true" />
                            </ExtraParams>
                        </Click>
                    </DirectEvents>

                    </ext:Button>
                </Buttons>

                </ext:FormPanel>
              </Items>
              
          </ext:Window>
        
        
        
               <ext:Window
                ID="EditProductWindow"
        runat="server"
        Title="Editar Producto"
              Hidden="true"
            Width="400"
            Height="240"
            BodyPadding="10"
            Resizable="true"
            Closable="true"
            Layout="Fit"
        >
              
              <Items>
              
              
                     <ext:FormPanel
                    ID="EditProductForm"
                    runat="server"
                    MarginSpec="0 5 5 5"
                    BodyPadding="2"
                    Frame="false"
                    Border="false"
                    BodyStyle="background-color:transparent"
                    Width="280"
                    DefaultAnchor="100%"
                    AutoScroll="True"
                         
                         >
                    <FieldDefaults ReadOnly="false" />
                    <Items>
                        
        
                        <ext:TextField runat="server" FieldLabel="Nombre"   Name="nombre" Editable="False" />
                        <ext:Hidden runat="server" FieldLabel="id"   Name="producto_id" Editable="False" />

                        <ext:NumberField runat="server" FieldLabel="IdCombo"   Name="IdCombo" MinValue="0"    AllowBlank="False"/>
                        
                        
                        


                        <%--<ext:Hidden runat="server" FieldLabel="IdCombo"   Name="IdCombo" MinValue="0"    AllowBlank="False"/>--%>
                    </Items>
                         
                          <Buttons >
                    <ext:Button
                        runat="server"
                        Text="Guardar"
                        Disabled="true"
                        FormBind="true" 
                        
                        >
                        
                         <DirectEvents>
                        <Click OnEvent="EditProducto" Before="return #{EditProductForm}.isValid();">
                            <ExtraParams>
                                <ext:Parameter
                                    Name="values"
                                    Value="#{EditProductForm}.getForm().getValues()"
                                    Mode="Raw"
                                    Encode="true" />
                            </ExtraParams>
                        </Click>
                    </DirectEvents>

                    </ext:Button>
                </Buttons>

                </ext:FormPanel>
              </Items>
              
          </ext:Window>
        
        <%--Width="400"
            Height="240"--%>
        <%--Maximized="True"
            
            Maximized="True"
            
            --%>
             <ext:Window
                ID="EditPasosWindow"
        runat="server"
        Title="Editar Pasos"
              Hidden="true"
               
                  
                  
                 MinWidth="550"
                 MinHeight="350"
            
            BodyPadding="10"
                 Maximized="True"
                 
            
            Resizable="true"
            Closable="true"
             Layout="VBoxLayout"
                 
        >
                 
        
                     <LayoutConfig >
                         
                         <ext:VBoxLayoutConfig Align="Stretch" />

                     </LayoutConfig>
                     <Items >
                         
                         
                               
                         
                         
                         
                            <ext:GridPanel
                                
                                Header="False"
            ID="pasosGridPanel"
            runat="server"
            Title="Pasos"
            Frame="true"
                                Layout="fit"

             Flex="1"
                                AutoLoad="False"
                                

            
            >
            <Store>
                     

                <ext:Store runat="server"   AutoLoad="False"   >
                    
                
                   

                    <Model>
                        <ext:Model runat="server" IDProperty="id">
                            <Fields>
                                <ext:ModelField Name="id" Type="Int" />
                                  <ext:ModelField Name="secuencia" Type="Int" />
                              
                                <ext:ModelField Name="id_producto" Type="Int" />
                                <ext:ModelField Name="descripcion" />

                                
                                <ext:ModelField Name="cantidad" Type="Int" />
                                <ext:ModelField Name="fecha_Insertado" Type="Date" />
                                

               
                            </Fields>
                        </ext:Model>
                    </Model>
                   
                </ext:Store>
            </Store>
            <ColumnModel runat="server" >
                <Columns>
                    
                     <ext:Column runat="server" DataIndex="id" Text="Id" MinWidth="10" />
                    <ext:Column runat="server" DataIndex="secuencia" Text="Secuencia" MinWidth="10" />
                    
                    <ext:Column runat="server" DataIndex="descripcion" Text="Descripcion" MinWidth="250" Flex="1"  />
                    <ext:Column runat="server" DataIndex="cantidad" Text="cantidad" MinWidth="10" />
                    
                    
                    <%--<ext:DateColumn runat="server" DataIndex="fecha_Insertado" Text="Fecha" MinWidth="100" Format="yyyy-MM-dd" ></ext:DateColumn>--%>
                    
                    
                       <ext:CommandColumn runat="server" MinWidth="200">
                        <Commands>
                        
                            
                              <ext:GridCommand Icon="TextListNumbers" CommandName="Editar"  Text="Editar"  >
                                <%--<ToolTip Text="Pasos" />--%>
                            </ext:GridCommand>

                            <ext:CommandSeparator />
                            

                            <ext:GridCommand Icon="Delete" CommandName="Eliminar"  Text="Eliminar"   >
                                

                                <%--<ToolTip Text="Editar" />--%>
                            </ext:GridCommand>
                            

                        </Commands>
                           
                           <DirectEvents>
                               <command OnEvent="EditStepEvent" >
                                   
                                   <ExtraParams>
                                        <ext:Parameter Name="Data" Value="record.data" Encode="true" Mode="Raw" />
                                          <ext:Parameter Name="CommandName" Value="command" Mode="Raw" />
                                   </ExtraParams>
                                       

                               </command>


                           </DirectEvents>
                           

                           
                    </ext:CommandColumn>

   
         
                </Columns>
            </ColumnModel>
            

                                <SelectionModel>
                        <ext:RowSelectionModel runat="server" Mode="Single">
                            <DirectEvents>
                                <Select OnEvent="RowSelectStep" Buffer="250">
                                    <%--<EventMask ShowMask="true" Target="CustomTarget" CustomTarget="#{FormPanel1}" />--%>
                                    <ExtraParams>
                                        <%--or can use params[2].id as value--%>
                                        <ext:Parameter Name="Data" Value="record.data" Mode="Raw" />
                                    </ExtraParams>
                                </Select>
                            </DirectEvents>
                        </ext:RowSelectionModel>
                    </SelectionModel>

            
            

            <SelectionModel>
                <%--<ext:RowSelectionModel runat="server" Mode="Multi" />--%>
            </SelectionModel>
            
                 <BottomBar>
                       
                     

                        <ext:PagingToolbar
                            runat="server"
                            DisplayInfo="true"
                            HideRefresh="true"
                            RefreshHandler=""
                            DisplayMsg="Mostrando pasos {0} - {1} of {2}"
                            >
                            
                            <Items>
                    <ext:Button runat="server" Text="AgregarPaso" Icon="Add" >
                          <DirectEvents>
                    <Click OnEvent="AddStepEvent" >
                     <%--   <ExtraParams>
                            
                                    <ext:Parameter
                                Name="id_producto"
                                Value=""
                                Mode="Raw"
                                Encode="true" />
                        </ExtraParams>--%>

                    </Click>
                </DirectEvents>

                    </ext:Button>
                </Items>
                        </ext:PagingToolbar>
                     

                    </BottomBar>
           
        </ext:GridPanel>
                         
                 
                   
                          
                         
                            <ext:GridPanel
                                
                                Header="true"
            ID="productosGridPanel"
            runat="server"
            Title="Productos de paso "
            Frame="true"
                                Layout="fit"

             Flex="1"
                                AutoLoad="False"
                                

            
            >
            <Store>
                     

                <ext:Store runat="server"   AutoLoad="False"   >
                    
                
                   

                    <Model>
                        <ext:Model runat="server" IDProperty="id">
                            <Fields>
                                <ext:ModelField Name="id" Type="Int" />
                                  <ext:ModelField Name="id_pasos_combos" Type="Int" />
                              
                                <ext:ModelField Name="id_producto" Type="Int" />
                                <ext:ModelField Name="NameProducto" />
                                <ext:ModelField Name="SkuProducto" />

                                
                                
                                <ext:ModelField Name="fecha_Insertado" Type="Date" />
                                

               
                            </Fields>
                        </ext:Model>
                    </Model>
                   
                </ext:Store>
            </Store>
            <ColumnModel runat="server" >
                <Columns>
                    
                     <ext:Column runat="server" DataIndex="id_producto" Text="Id producto" MinWidth="10" />
                    
                    
                    <ext:Column runat="server" DataIndex="SkuProducto" Text="Sku" MinWidth="50"  />
                    <ext:Column runat="server" DataIndex="NameProducto" Text="Nombre" MinWidth="150"  Flex="1"  />
                    
                    
                    
                    <%--<ext:DateColumn runat="server" DataIndex="fecha_Insertado" Text="Fecha" MinWidth="100" Format="yyyy-MM-dd" ></ext:DateColumn>--%>
                    
                    
                       <ext:CommandColumn runat="server" MinWidth="150">
                        <Commands>
                        
                            
                            <ext:GridCommand Icon="Delete" CommandName="Eliminar"  Text="Eliminar"   >                                
                                
                            </ext:GridCommand>
                            

                        </Commands>
                           
                           <DirectEvents>
                               <command OnEvent="DeleteProductStepEvent" >
                                   <ExtraParams>
                                        <ext:Parameter Name="Data" Value="record.data" Encode="true" Mode="Raw" />
                                          <ext:Parameter Name="CommandName" Value="command" Mode="Raw" />
                                   </ExtraParams>

                               </command>


                           </DirectEvents>
                           

                           
                    </ext:CommandColumn>

   
         
                </Columns>
            </ColumnModel>
            

            
            

            <SelectionModel>
                <%--<ext:RowSelectionModel runat="server" Mode="Multi" />--%>
            </SelectionModel>
            
                 <BottomBar>
                       
                     

                        <ext:PagingToolbar
                            runat="server"
                            DisplayInfo="true"
                            HideRefresh="true"
                            RefreshHandler=""
                            DisplayMsg="Mostrando pasos {0} - {1} of {2}"
                            >
                            
                            <Items>
                    <ext:Button runat="server" Text="Agregar producto de paso" Icon="Add" >
                          <DirectEvents>
                    <Click OnEvent="AddProductStepEvent" >
                     <%--   <ExtraParams>
                            
                                    <ext:Parameter
                                Name="id_producto"
                                Value=""
                                Mode="Raw"
                                Encode="true" />
                        </ExtraParams>--%>

                    </Click>
                </DirectEvents>

                    </ext:Button>
                </Items>
                        </ext:PagingToolbar>
                     

                    </BottomBar>
           
        </ext:GridPanel>
                         
                 
                         


                    <ext:TextField Hidden="True" runat="server" Name="nombre" TagHiddenName="nombre" ID="zfield02z"></ext:TextField>
                         <ext:TextField  Hidden="True" runat="server" Name="id_producto" TagHiddenName="id_producto" ID="zfield01z"></ext:TextField>
                         

                     </Items>

             </ext:Window>
        
        
        
        
        
        
          <ext:Window
                ID="AddStepWindow"
        runat="server"
        Title="Agregar paso"
              Hidden="true"
            Width="400"
            Height="300"
            BodyPadding="10"
            Resizable="true"
            Closable="true"
            Layout="fit"
        >
              
              <Items>
              
              
                  <%--Split="true"--%>
                  <%--Title="Agregar Producto" Icon="User"--%>
                  <%--ProductosStore        QueryMode="Local" Region="East"--%>
                     <ext:FormPanel
                    ID="AddStepForm"
                    runat="server"
                    
                    
                    MarginSpec="0 5 5 5"
                    BodyPadding="2"
                    Frame="false"
                    Border="false"
                    BodyStyle="background-color:transparent"
                    Width="380"
                    DefaultAnchor="100%"
                         Height="400"

                    AutoScroll="True"
                         
                         >
                    <FieldDefaults ReadOnly="false" />
                    <Items>
                        
                        <ext:NumberField runat="server" FieldLabel="Secuencia"   Name="secuencia" MinValue="0"    AllowBlank="False"/>

                        <ext:TextField runat="server" FieldLabel="Descripcion"   Name="descripcion" MinValue="0"    AllowBlank="False"/>
                        
                        <ext:NumberField runat="server" FieldLabel="Cantidad"   Name="cantidad" MinValue="0"    AllowBlank="False"/>
                        <ext:Hidden runat="server" FieldLabel="id"   Name="id" InputType="Number"     />
                        <ext:Hidden runat="server" FieldLabel="id_producto"   Name="id_producto" />
                
                    </Items>
                         
                          <Buttons >
                    <ext:Button
                        runat="server"
                        Text="Guardar"
                        Disabled="true"
                        FormBind="true" 
                        
                        >
                        
                         <DirectEvents>
                        <Click OnEvent="AddStep" Before="return #{AddStepForm}.isValid();">
                            <ExtraParams>
                                <ext:Parameter
                                    Name="values"
                                    Value="#{AddStepForm}.getForm().getValues()"
                                    Mode="Raw"
                                    Encode="true" />
                            </ExtraParams>
                        </Click>
                    </DirectEvents>

                    </ext:Button>
                </Buttons>

                </ext:FormPanel>
              </Items>
              
          </ext:Window>
        

        
        
        
             <ext:Window
                ID="AddProductStepWindow"
        runat="server"
        Title="Agregar Producto a paso"
              Hidden="true"
            Width="400"
            Height="240"
            BodyPadding="10"
            Resizable="true"
            Closable="true"
            Layout="Fit"
        >
              
              <Items>
              
              
                  <%--Split="true"--%>
                  <%--Title="Agregar Producto" Icon="User"--%>
                  <%--ProductosStore        QueryMode="Local" Region="East"--%>
                     <ext:FormPanel
                    ID="AddProductStepForm"
                    runat="server"
                    
                    
                    MarginSpec="0 5 5 5"
                    BodyPadding="2"
                    Frame="false"
                    Border="false"
                    BodyStyle="background-color:transparent"
                    Width="280"
                    DefaultAnchor="100%"
                    AutoScroll="True"
                         
                         >
                    <FieldDefaults ReadOnly="false" />
                    <Items>
                        <ext:ComboBox
                              
                runat="server"
                FieldLabel="Producto"
                DisplayField="name"
                Width="320"
                LabelWidth="130"
                Name="id_producto"
                ValueField="id"
                TypeAhead="true"
                              Store="ProductosStore"
                              >
                    
            </ext:ComboBox>

                        <ext:NumberField runat="server" FieldLabel="id_pasos_combos"   Name="id_pasos_combos" MinValue="0"    Hidden="True"/>
                
                    </Items>
                         
                          <Buttons >
                    <ext:Button
                        runat="server"
                        Text="Guardar"
                        Disabled="true"
                        FormBind="true" 
                        
                        >
                        
                         <DirectEvents>
                        <Click OnEvent="AddProductStep" Before="return #{AddProductStepForm}.isValid();">
                            <ExtraParams>
                                <ext:Parameter
                                    Name="Data"
                                    Value="#{AddProductStepForm}.getForm().getValues()"
                                    Mode="Raw"
                                    Encode="true" />
                            </ExtraParams>
                        </Click>
                    </DirectEvents>

                    </ext:Button>
                </Buttons>

                </ext:FormPanel>
              </Items>
              
          </ext:Window>
       

        

    </form>
</body>
</html>
