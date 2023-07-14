<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MasterAngular.Master" CodeBehind="ChartsEfectivale.aspx.cs" Inherits="ValidacionBatch.ChartsEfetivale" %>

<asp:Content runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
<script src="Content/Components/Efectivale/queriesController.js"></script>

    
    <div   layout="column"   ng-controller="QueriesController"  layout-fill >

        <md-toolbar>
            <div class="md-toolbar-tools">

                <label flex>  Charts </label>
                
                <mdp-date-picker  ng-model="params.DateStart"></mdp-date-picker>
                <mdp-date-picker ng-model="params.DateEnd"></mdp-date-picker>
                


                <md-button class="md-icon-button" ng-click="getElements()">
                    <md-tooltip md-direction="botton">Actualizar</md-tooltip>
                    <md-icon md-svg-icon="refresh"></md-icon>
                </md-button>

                
                <%--<md-datepicker ng-model="params.DateStart"></md-datepicker>
                <md-datepicker ng-model="params.DateEnd"></md-datepicker>--%>



                <%--<md-button class="md-icon-button" ng-click="openAddSucursal()" style="margin-right: 10px;">
                        <md-tooltip md-direction="botton">Agregar Sucursal                </md-tooltip>
                        <md-icon md-svg-icon="plus"></md-icon>
                    </md-button>--%>


            </div>

        </md-toolbar >

        <md-content  >



            
            <div layout="row" >
                <md-card flex>
                    <canvas id="line" class="chart chart-line"
                            chart-data="chart1.Data"
                            chart-labels="chart1.Labels"
                            chart-series="chart1.Series"
                            chart-options="chart1.options"
                            chart-dataset-override="datasetOverrideA"
                            
                            chart-click="onClick"></canvas>

                </md-card>
                
                <md-card flex>
                    <canvas class="chart chart-line"
                            chart-data="chart1.DataAmount"
                            chart-labels="chart1.Labels"
                            chart-series="chart1.Series"
                            chart-options="chart2.options"
                            chart-dataset-override="datasetOverrideA" chart-click="onClick"></canvas>
                </md-card>

            </div>


            <div layout="row">
                <md-card flex>
                    
                    <canvas class="chart chart-bar"
                            chart-data="chart1.DataPercent"
                            chart-labels="chart1.Labels"
                            chart-series="chart1.Series"
                            chart-options="chart3.options"
                            chart-dataset-override="datasetOverrideA" chart-click="onClick"></canvas>

                </md-card>
                <md-card flex>

                    <canvas class="chart chart-bar"
                            chart-data="chart1.DataAmountPercent"
                            chart-labels="chart1.Labels"
                            chart-series="chart1.Series"
                            chart-options="chart4.options"
                            chart-dataset-override="datasetOverrideA" chart-click="onClick"></canvas>

                </md-card>


            </div>


            <div layout="row">
                
                       <md-card flex>
                    
                    
                     <canvas  class="chart chart-line"
                            chart-data="chart6.Data"
                            chart-labels="chart6.Labels"
                            chart-series="chart6.Series"
                            chart-options="chart6.options"
                            chart-dataset-override="datasetOverrideA"
                            
                            >
                         

                     </canvas>

                </md-card>
                
                <md-card flex>

                    <canvas class="chart chart-line"
                            chart-data="chart5.Data"
                            chart-labels="chart5.Labels"
                            chart-series="chart5.Series"
                            chart-options="chart5.options"
                            chart-dataset-override="datasetOverrideA" chart-click="onClick"></canvas>
                </md-card>
         
                    

                </div>
</md-content>
    </div>

    </asp:Content>