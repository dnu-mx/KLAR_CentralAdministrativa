<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterAngular.Master" CodeBehind="Sucursales.aspx.cs" Inherits="ComercioElectronico.Sucursales" %>

<asp:Content runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
<script src="Components/Sucursales/sucursalesController.js"></script>

<div ng-controller="Controllers.Sucursales" layout="column"  layout-fill>
    <md-toolbar  md-theme="lowBlue">
        <div class="md-toolbar-tools" > 

            <label flex >  </label>



            <md-button class="md-icon-button" ng-click="getElements()" >
                <md-tooltip md-direction="botton" >Actualizar</md-tooltip>
                <md-icon md-svg-icon="refresh"></md-icon>
            </md-button>


            <md-button class="md-icon-button" ng-click="openAddSucursal()" style="margin-right: 10px;"> 
                <md-tooltip md-direction="botton" >Agregar Sucursal                </md-tooltip>
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
                    <th id="sucursal" md-column md-numeric md-order-by="id_sucursal"><span>Id</span></th>
                    <th id="clave" md-column md-order-by="clave"><span>Clave</span></th>
                    <th id="nombre" md-column md-order-by="nombre"><span>Nombre</span></th>
                    <%--<th md-column md-numeric md-order-by="Monto"><span>Monto</span></th>--%>
                    
                    
                    <th id="activo" md-column md-order-by="activa"><span>Activo</span></th>
                    
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

                    <td md-cell>{{item.id_sucursal}}</td>
                    <td md-cell>{{item.clave}}</td>
                    <td md-cell>{{item.nombre}}</td>
                    
                    <%--<td md-cell>{{item.Monto |currency}}</td>--%>
                    
                    <%--<td md-cell>{{item.fecha_Insertado | date:'dd/MM/yyyy'}}</td>--%>
                    <td md-cell><md-checkbox ng-model="item.activa" ng-disabled="true" aria-label="boleanoalv"></md-checkbox></td>
                  
                    <td md-cell>

                       

                        
                            <md-button class="md-icon-button"  ng-click="openDialogTimes($event,item)" >
                            <md-tooltip md-direction="top"> Horarios</md-tooltip>
                            <md-icon md-svg-icon="clock"></md-icon>

                        </md-button>
                        
                                <md-button class="md-icon-button"  ng-click="openDialogCoverage($event,item)">
                            <md-tooltip md-direction="top"> Cobertura</md-tooltip>
                            <md-icon md-svg-icon="map-marker"></md-icon>

                        </md-button>

                    
                              <md-button class="md-icon-button "  ng-click="openDialogBranchOffice($event,item)"
                                  md-theme="hasDataColor"
                                   ng-class="item. id_suc_sustituta!==null ? 'md-primary':'' "
                                   >
                            <md-tooltip md-direction="top"> Sucursal substituta </md-tooltip>
                            <md-icon md-svg-icon="store"   md-icon> </md-icon>

                        </md-button>


                        <md-button class="md-icon-button"  ng-click="openEditSucursal($event,item)">
                            <md-tooltip md-direction="top"> Editar</md-tooltip>
                            <md-icon md-svg-icon="pencil"></md-icon>

                        </md-button>


                   <%--     <md-icon class="md-secondary" ng-click="showAdvanced($event,item)" aria-label="Abrir" md-svg-icon="open-in-new">
                            <md-tooltip md-direction="left"> Acciones</md-tooltip>
                        </md-icon>--%>


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
                <label > Nombre</label>
                <input type="text"  ng-model="newElement.nombre" name="nombre"  required>

                    <div ng-messages="baseEditForm.nombre.$error">
                          <div ng-messages-include="message-template"></div>
                    </div>
              </md-input-container>


                <div>
                <md-input-container class="md-icon-float"  >
                  <md-icon md-svg-icon="format-text"></md-icon>
                <label >Clave</label>              
                <input type="text"  ng-model="newElement.clave"  name="clave"   >
                     <div ng-messages="baseEditForm.clave.$error">
                          <div ng-messages-include="message-template"></div>
                    </div>
              </md-input-container>
            
                <md-input-container  class=" md-icon-float "  >
                    <md-icon md-svg-icon="format-text"></md-icon>
                <label >Calle</label>
                <input type="text"  ng-model="newElement.calle" name="calle" required >
                      <div ng-messages="baseEditForm.calle.$error">
                          <div ng-messages-include="message-template"></div>
                    </div>
              </md-input-container>


               
                <md-input-container  class=" md-icon-float " >
                    <md-icon md-svg-icon="format-text"></md-icon>
                <label >Colonia</label>
                <input type="text"  ng-model="newElement.colonia"  name ="colonia" required>
                      <div ng-messages="baseEditForm.colonia.$error">
                          <div ng-messages-include="message-template"></div>
                    </div>
              </md-input-container>


                   
                <md-input-container  class=" md-icon-float " >
                    <md-icon md-svg-icon="format-text"></md-icon>
                <label >Ciudad</label> 
                <input type="text"  ng-model="newElement.ciudad" name="ciudad" required>
                      <div ng-messages="baseEditForm.ciudad.$error">
                          <div ng-messages-include="message-template"></div>
                    </div>
              </md-input-container>


                          
                <md-input-container  class=" md-icon-float " >
                    <md-icon md-svg-icon="format-text"></md-icon>
                <label >Estado            </label>
                <input type="text"  ng-model="newElement.estado" name="estado"  required>
                      <div ng-messages="baseEditForm.estado.$error">
                          <div ng-messages-include="message-template"></div>
                    </div>
              </md-input-container>


                           
                <md-input-container  class=" md-icon-float " >
                    <md-icon md-svg-icon="numeric"></md-icon>
                <label >Codigo postal            </label>
                <input type="text"  ng-model="newElement.cp" io-number-mask="0" name="cp"  required>
                      <div ng-messages="baseEditForm.cp.$error">
                          <div ng-messages-include="message-template"></div>
                    </div>
              </md-input-container>



                                    
                <md-input-container  class=" md-icon-float " >
                    <md-icon md-svg-icon="format-text"></md-icon>
                <label >    Telefono        </label>
                <input type="text"  ng-model="newElement.telefono" name="telefono"  required>
                      <div ng-messages="baseEditForm.telefono.$error">
                          <div ng-messages-include="message-template"></div>
                    </div>
              </md-input-container>


                    </div>


                
                                    
                <md-input-container  class=" md-icon-float " >
                    <md-icon md-svg-icon="format-text"></md-icon>
                <label >    Coordenadas        </label>
                <input type="text"  ng-model="newElement.coordenadas" name="coordenadas"  required>
                      <div ng-messages="baseEditForm.coordenadas.$error">
                          <div ng-messages-include="message-template"></div>
                    </div>
              </md-input-container>




                <md-checkbox ng-model="newElement.activa" class="md-primary" > Activo</md-checkbox>


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