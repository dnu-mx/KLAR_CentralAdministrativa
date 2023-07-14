<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterAngular.Master" CodeBehind="CombosBase.aspx.cs" Inherits="ComercioElectronico.CombosBase" %>

<asp:Content runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <script src="Components/ProductosCombos/productosCombosController.js"></script>

    <div ng-controller="Controllers.ProductosCombos" layout="column" layout-fill>
        <md-toolbar md-theme="lowBlue">
            <div class="md-toolbar-tools">

                <label flex> </label>


                <md-button class="md-icon-button" ng-click="getElements()">
                    <md-tooltip md-direction="botton">Actualizar</md-tooltip>
                    <md-icon md-svg-icon="refresh"></md-icon>
                </md-button>


                <md-button class="md-icon-button" ng-click="openAddSideNav()" style="margin-right: 10px;">
                    <md-tooltip md-direction="botton">Agregar producto </md-tooltip>
                    <md-icon md-svg-icon="plus"></md-icon>
                </md-button>


            </div>

        </md-toolbar>


        <md-content >


            <md-table-container style="overflow: auto; max-height: 75vh;">
                <table md-table md-row-select="options.rowSelection" multiple="{{options.multiSelect}}"
                       ng-model="selected" md-progress="mdloadingvalue">
                    <caption></caption>
                    <thead md-head md-order="query.order" fix-head>
                    <tr md-row>
                        <th id="producto" md-column md-numeric md-order-by="producto_id">
                            <span>Id</span></th>
                        <th id="sku" md-column md-order-by="sku">
                            <span>Sku</span></th>
                        <th id="nombre" md-column md-order-by="nombre">
                            <span>Nombre</span></th>


                        <th id="fechaAlta" md-column md-order-by="fecha_Insertado"><span>Fecha alta</span></th>
                        <th id="activo" md-column md-order-by="activo">
                            <span>Activo</span></th>

                        <th id="acciones" md-column> Acciones</th>

                    </tr>
                    </thead>
                    <tbody md-body>

                    <tr md-row ng-repeat="item in elements | orderBy: query.order  | limitTo: query.limit: (query.page - 1 ) * query.limit">

                        <td md-cell>{{item.producto_id}}</td>
                        <td md-cell>{{item.sku}}</td>
                        <td md-cell>{{item.nombre}}</td>


                        <td md-cell>{{item.fecha_Insertado | date:'dd/MM/yyyy'}}</td>
                        <td md-cell>
                            <md-checkbox ng-model="item.activo" ng-disabled="true" aria-label="boleanoalv"></md-checkbox>
                        </td>

                        <td md-cell>


                            <md-button class="md-icon-button" ng-click="openDialogPasos($event,item)">
                                <md-tooltip md-direction="top"> Pasos</md-tooltip>
                                <md-icon md-svg-icon="format-list-numbers"></md-icon>
                            </md-button>


                            <md-button class="md-icon-button" ng-click="openEditSideNav($event,item)">
                                <md-tooltip md-direction="top"> Editar</md-tooltip>
                                <md-icon md-svg-icon="pencil"></md-icon>

                            </md-button>


                        </td>
                    </tr>
                    </tbody>
                </table>
            </md-table-container>


            <md-table-pagination md-limit="query.limit" md-page="query.page" md-total="{{elements.length}}"
                                 md-limit-options="[5, 10, 15]">
            </md-table-pagination>


        </md-content>

        <md-sidenav class="md-sidenav-right md-whiteframe-4dp" md-component-id="right">

            <md-toolbar class="md-theme-light">
                <h1 class="md-toolbar-tools">{{textSidenav}}</h1>
            </md-toolbar>
            <md-content layout-padding>
                <form name="baseEditForm" layout="column">
                    <div layout="row">

                        <md-input-container class="md-icon-float" >
                            <md-icon md-svg-icon="format-text"></md-icon>
                            <label >Sku</label>
                            <input type="text" ng-model="newElement.sku" name="sku" required>
                            <div ng-messages="baseEditForm.sku.$error">
                                <div ng-messages-include="message-template"></div>
                            </div>
                        </md-input-container>


                        <md-input-container class="md-icon-float " flex >
                            <md-icon md-svg-icon="format-text"></md-icon>
                            <label > Nombre</label>
                            <input type="text" ng-model="newElement.nombre" name="nombre" required >

                            <div ng-messages="baseEditForm.nombre.$error">
                                <div ng-messages-include="message-template"></div>
                            </div>


                        </md-input-container>


                    </div>

                    
                       
                <md-input-container flex class="md-icon-float">
                    <md-icon md-svg-icon="arrow-down-drop-circle-outline"></md-icon>
                     <label>Familia</label>
              <md-select ng-model="newElement.familia_id"  name="familia_id" placeholder="Familia" required  >
                <md-option ng-value="item.Id" ng-repeat="item in familiasList">{{ item.Name }}</md-option>
              </md-select>
                        <div class="md-errors-spacer" ng-messages="baseEditForm.familia_id.$error">
                            <div ng-messages-include="message-template"></div>
                        </div>

                </md-input-container>



                    <md-input-container class="md-icon-float ">
                        <md-icon md-svg-icon="format-text"></md-icon>
                        <label > Descripcion</label>
                        <textarea  type="text"  rows="1" ng-model="newElement.descripcion" name="descripcion" required>
                        </textarea>

                        <div   ng-messages="baseEditForm.descripcion.$error">
                            <div ng-messages-include="message-template"></div>
                        </div>


                    </md-input-container>


                    <div layout="row">


                        <md-input-container class="md-icon-float ">
                            <md-icon md-svg-icon="numeric"></md-icon>
                            <label > Secuencia</label>
                            <input type="text" ng-model="newElement.secuencia" ui-number-mask="0" name="secuencia" required>

                            <div ng-messages="baseEditForm.secuencia.$error">
                                <div ng-messages-include="message-template"></div>
                            </div>
                        </md-input-container>


                        <md-input-container class="md-icon-float " flex>
                            <md-icon md-svg-icon="format-text"></md-icon>
                            <label > Path imagen</label>
                            <input type="text" ng-model="newElement.path_imagen" name="path_imagen" required>

                            <div ng-messages="baseEditForm.path_imagen.$error">
                                <div ng-messages-include="message-template"></div>
                            </div>


                        </md-input-container>


                    </div>


                    <md-switch ng-model="newElement.activo" class="md-primary"> Activo</md-switch>


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

          <md-sidenav class="md-sidenav-right md-whiteframe-4dp" md-component-id="newRight">
              
              <md-toolbar class="md-theme-light">
                <h1 class="md-toolbar-tools">{{textSidenav}}</h1>
            </md-toolbar>
                  <md-content layout-padding>
                      
                      
                
                    
                    <form name="baseEditForm2" layout="column">
                        
                                <md-input-container flex class="md-icon-float">
                    <md-icon md-svg-icon="arrow-down-drop-circle-outline"></md-icon>
                             <label>Producto</label>
                      <md-select ng-model="newElement.producto_id"  name="producto_id" placeholder="Producto" required  >
                        <md-option ng-value="item.Id" ng-repeat="item in productosList">{{ item.Name }}</md-option>
                      </md-select>
                                <div class="md-errors-spacer" ng-messages="baseEditForm2.producto_id.$error">
                                    <div ng-messages-include="message-template"></div>
                                </div>

                 </md-input-container>
                        
                        
                        <md-input-container class="md-icon-float ">
                            <md-icon md-svg-icon="numeric"></md-icon>
                            <label > Id Combo</label>
                            <input type="text" ng-model="newElement.IdCombo" ui-number-mask="0" name="Id Combo" required>

                            <div ng-messages="baseEditForm2.IdCombo.$error">
                                <div ng-messages-include="message-template"></div>
                            </div>
                        </md-input-container>

                        

                        </form>
                    



                    <md-button class=" md-raised md-primary" ng-click="updateNewElement(newElement)">
                        Guardar
                    </md-button>

                    <md-button ng-click="closeSideNav('newRight')" class="md-primary">
                        Cancelar
                    </md-button>
              
                      

                        </md-content>


              </md-sidenav>
    </div>

</asp:Content>