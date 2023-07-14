<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterAngular.Master" CodeBehind="Empresas.aspx.cs" Inherits="ComercioElectronico.Empresas" %>

<asp:Content runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
<script src="Components/Empresas/empresasController.js"></script>

<div ng-controller="EmpresasController" layout="column"  layout-fill>
    <md-toolbar  md-theme="lowBlue">
        <div class="md-toolbar-tools" > 

            <label flex >  </label>



            <md-button class="md-icon-button" ng-click="getElements()" >
                <md-tooltip md-direction="botton" >Actualizar</md-tooltip>
                <md-icon md-svg-icon="refresh"></md-icon>
            </md-button>


            <md-button class="md-icon-button" ng-click="openAddEmpresa()" style="margin-right: 10px;"> 
                <md-tooltip md-direction="botton" >Agregar Empresa                </md-tooltip>
                <md-icon md-svg-icon="plus"></md-icon>
            </md-button>


        </div>

    </md-toolbar>


    <md-content >


            <md-table-container style="overflow:auto; max-height:75vh;">
        <table md-table md-row-select="options.rowSelection" multiple="{{options.multiSelect}}"
               ng-model="selected" md-progress="mdloadingvalue">
            <caption></caption>
            <thead md-head md-order="query.order" fix-head>
                <tr md-row>
                    <th id="id" md-column md-numeric md-order-by="ID_Empresa"><span>Id</span></th>
                    <th id="clave" md-column md-order-by="ClaveEmpresa"><span>Clave</span></th>
                    <th id="razonSocial" md-column md-order-by="RazonSocial"><span>Razon Social</span></th>
                    <th id="nombreComercial" md-column md-order-by="NombreComercial"><span>NombreComercial</span></th>
                    
                    <%--<th md-column md-numeric md-order-by="Monto"><span>Monto</span></th>--%>
                    
                    
                    <th id="asociarUsuario" md-column md-order-by="AsociarCorreo"><span>Asociar usuario</span></th>
                    
                    <th id="acciones" md-column> Acciones</th>
                    <!--<th md-column md-numeric md-order-by="calories.value"><span>Calories</span></th>
                    <th md-column md-numeric>Fat (g)</th>
                    <th md-column md-numeric>Carbs (g)</th>-->

                </tr>
            </thead>
            <tbody md-body>
                <!--<tr md-row md-select="nationality" md-select-id="Id" md-auto-select ng-repeat="nationality in currentNationalities">
                    <td md-cell>{{nationality.Name}}</td>
                    <td md-cell>{{nationality.NameShort}}</td>
                    <td md-cell>{{nationality.NameNationality}}</td>
                    <td md-cell>{{nationality.NameNationalityShort}}</td>

                </tr>-->
                <tr md-row ng-repeat="item in elements | orderBy: query.order  | limitTo: query.limit: (query.page - 1 ) * query.limit">

                    <td md-cell>{{item.ID_Empresa}}</td>
                    <td md-cell>{{item.ClaveEmpresa}}</td>
                    <td md-cell>{{item.RazonSocial}}</td>
                    <td md-cell>{{item.NombreComercial}}</td>

                    
                    <%--<td md-cell>{{item.Monto |currency}}</td>--%>
                    
                    <%--<td md-cell>{{item.fecha_Insertado | date:'dd/MM/yyyy'}}</td>--%>
                    <td md-cell><md-checkbox ng-model="item.AsociarCorreo" ng-disabled="true" aria-label="boleanoalv"></md-checkbox></td>
                  
                    <td md-cell>

                       


                        <md-button class="md-icon-button"  ng-click="openEditEmpresa($event,item)">
                            <md-tooltip md-direction="top"> Editar</md-tooltip>
                            <md-icon md-svg-icon="pencil"></md-icon>

                        </md-button>


                    </td>
                </tr>
            </tbody>
        </table>
    </md-table-container>

    <!--<md-table-pagination md-limit="query.limit" md-limit-options="[5, 10, 15]" md-page="query.page"
                             md-total="{{nationalities.length}}" md-on-paginate="getDesserts" md-page-select>
    </md-table-pagination>-->

    <md-table-pagination md-limit="query.limit" md-page="query.page" md-total="{{elements.length}}"
                         md-limit-options="[5, 10, 15]"></md-table-pagination>



    </md-content>

     <md-sidenav class="md-sidenav-right md-whiteframe-4dp"  md-component-id="right">

          <md-toolbar class="md-theme-light">
            <h1 class="md-toolbar-tools">{{textSidenav}}</h1>
          </md-toolbar>
          <md-content  layout-padding>
            <form name="baseEditForm" layout="column">
          

       
                
           <md-input-container class="md-icon-float " >
                    <md-icon md-svg-icon="format-text"></md-icon>
                <label > Nombre comercial</label>
                <input type="text"  ng-model="newElement.NombreComercial" name="NombreComercial"  required>

                    <div ng-messages="baseEditForm.NombreComercial.$error">
                          <div ng-messages-include="message-template"></div>
                    </div>
              </md-input-container>


 

                         <md-input-container class="md-icon-float " >
                    <md-icon md-svg-icon="format-text"></md-icon>
                <label > Razon Social</label>
                <input type="text"  ng-model="newElement.RazonSocial" name="RazonSocial"  >

                    <div ng-messages="baseEditForm.RazonSocial.$error">
                          <div ng-messages-include="message-template"></div>
                    </div>
              </md-input-container>



                <md-checkbox ng-model="newElement.AsociarCorreo" class="md-primary" > Asociar Usuario</md-checkbox>
                
                
                      <md-input-container class="md-icon-float " ng-show="newElement.AsociarCorreo">
                    <md-icon md-svg-icon="format-text"></md-icon>
                <label >Dominios de correo (separar con comas ej: @empresa.com, @empresa.com.mx)</label>
                <textarea type="text"  ng-model="newElement.DominiosCorreo" name="DominiosCorreo"  > </textarea>


                    <div ng-messages="baseEditForm.DominiosCorreo.$error">
                          <div ng-messages-include="message-template"></div>
                    </div>
              </md-input-container>

                
                

            </form>
        
              <div layout="row">

                            
      
              <md-button class=" md-raised md-primary" ng-click="saveNewElement(newElement)">
              Guardar
            </md-button>
            
            <md-button ng-click="closeSideNav()" class="md-primary">
              Cancelar
            </md-button>
                  </div>
          </md-content>

    </md-sidenav>



    




</div>

    </asp:Content>