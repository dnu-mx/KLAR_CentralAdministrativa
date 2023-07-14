(function (zmodule) {

    zmodule.controller("QueriesController",
    ["$scope", "$rootScope"
        , "$mdDialog", "$filter", "$mdSidenav"
        , "service.loading", "service.messages"
        , "serviceQueries"
        , function (
            $scope, $rootScope, $mdDialog, $filter, $mdSidenav
        , serviceLoading, serviceMessages
        , serviceQueries
            ) {

            //var oldShow = $mdDialog.show;
            //$mdDialog.show = function (options) {
            //    if (options.hasOwnProperty("skipHide")) {
            //        options.multiple = options.skipHide;
            //    }
            //    return oldShow(options);
            //};

            //$scope.newElement = {}
            //$scope.elements = [];

            $scope.params = {
                DateStart: new Date(),
                DateEnd: new Date(),

            }
            function numberWithCommas(x) {
                x = x.toString();
                var pattern = /(-?\d+)(\d{3})/;
                while (pattern.test(x))
                    x = x.replace(pattern, "$1,$2");
                return x;
            }

            $scope.params.DateStart.setDate(1);
            $scope.params.DateEnd.setDate(1);

            $scope.params.DateEnd.setMonth($scope.params.DateEnd.getMonth() + 1);


            $scope.params.DateEnd.setDate(0);

            $scope.datasetOverrideA = [

                {



                    //label: "Blue Stuff",
                    fill: true,
                    lineTension: 0.2,
                    borderColor: "rgba(220,20,60,1)",
                    backgroundColor: "rgba(220,20,60,0.3)",



                },
            {
                //label: "Blue Stuff",
                //fill: true,
                lineTension: 0.2,
                borderColor: "rgba(0,100,192,1)",
                backgroundColor: "rgba(0,100,192,0.4)",
            },
               ,

            ];

            $scope.chart = {};

            //$scope.params.dateEnd.setMonth($scope.params.dateOne.Date() + 1);
            $scope.chart1 = {};
            $scope.chart1.Labels = ["January", "February", "March", "April", "May", "June", "July"];
            $scope.chart1.Series = ['Series A', 'Series B'];
            $scope.chart1.Data = [
              [65, 59, 80, 81, 56, 55, 40],
              [28, 48, 40, 19, 86, 27, 90]
            ];
            $scope.chart1.options = {
                legend: {
                    display: true,
                    labels: {
                        //fontColor: 'rgb(255, 99, 132)'
                    }
                },
                title: {
                    display: true,
                    text: 'Cantidad de operaciones',
                    fontColor: 'blue',
                    fontSize: 16
                }
                ,
                tooltips: {
                    //enabled: false,
                    //backgroundColor: "rgba(0,100,192,1)"
                },

                scales: {
                    yAxes: [
                        {
                            //stacked: true,
                            ticks: {
                                //min: 0,
                                //max: 100,
                                callback: function (value) {
                                    return numberWithCommas(value);
                                }
                            },
                            scaleLabel: {
                                display: false,
                                //labelString: "Numero de operaciones consumos vs rechazos"
                            }
                        }
                    ]
                }
            };


            $scope.chart1.DataAmount = [
           [65, 59, 80, 81, 56, 55, 40],
           [28, 48, 40, 19, 86, 27, 90]
            ];

            $scope.chart2 = {};

            $scope.chart2.options = {
                legend: {
                    display: true,
                    //labels: {
                    ////    // This more specific font property overrides the global property
                    ////    fontColor: 'black'
                    ////}
                },
                title: {
                    display: true,
                    text: 'Montos de las operaciones',
                    fontColor: 'blue',
                    fontSize: 16
                },

                scales: {
                    yAxes: [
                        {
                            //stacked: true,
                            ticks: {
                                //min: 0,
                                //max: 100,
                                callback: function (value) {
                                    return numberWithCommas(value);
                                }
                            },
                            scaleLabel: {
                                display: false,
                                //labelString: "Numero de operaciones consumos vs rechazos"
                            }
                        }
                    ]
                }
            };


            $scope.chart3 = {};

            $scope.chart3.options = {
                legend: {
                    display: true,
                    labels: {
                        //fontColor: 'rgb(255, 99, 132)'
                    }
                },
                title: {
                    display: true,
                    text: "Numero de operaciones consumos vs rechazos",
                    fontColor: "blue",
                    fontSize: 16
                },
                scales: {
                    xAxes: [{
                        stacked: true
                    }],

                    yAxes: [{
                        stacked: true,
                        /* ticks: {
                             min: 0,
                             max: 100000,
                             callback: function (value) {
                                 return value + "%"
                             }
                         },*/
                        scaleLabel: {
                            display: false,
                            labelString: "Numero de operaciones consumos vs rechazos"
                        }
                    }]
                }
            };

            $scope.chart4 = {};

            $scope.chart4.options = {
                legend: {
                    display: true,
                    labels: {
                        //fontColor: 'rgb(255, 99, 132)'
                    }
                },
                title: {
                    display: true,
                    text: "Monto operaciones Consumos vs rechazos",
                    fontColor: "blue",
                    fontSize: 16
                },
                scales: {
                    xAxes: [{
                        stacked: true
                    }],

                    yAxes: [{
                        stacked: true,
                        /* ticks: {
                             min: 0,
                             max: 100000,
                             callback: function (value) {
                                 return value + "%"
                             }
                         },*/
                        scaleLabel: {
                            display: false,
                            labelString: "Porcentajes operaciones"
                        }
                    }]
                }
            };



            $scope.chart5 = {};

            $scope.chart5.options = {
                legend: {
                    display: true,
                    labels: {
                        //fontColor: 'rgb(255, 99, 132)'
                    }
                },
                title: {
                    display: true,
                    text: "Incidencias",
                    fontColor: "blue",
                    fontSize: 16
                },
                scales: {
                    yAxes: [
               {
                   //stacked: true,
                   ticks: {
                       //min: 0,
                       //max: 100,
                       callback: function (value) {
                           return numberWithCommas(value);
                       }
                   },
                   scaleLabel: {
                       display: false,
                       //labelString: "Numero de operaciones consumos vs rechazos"
                   }
               }
                    ]
                }
            };



            $scope.getElements = function () {
                serviceLoading.start();
                serviceQueries.getAsentamientosFilter($scope.params,
                    function (result) {

                        //console.log(result.data);
                        $scope.chart1.Data = result.Data;

                        $scope.chart1.DataAmount = result.DataAmount;

                        $scope.chart1.Series = result.Series;
                        $scope.chart1.Labels = result.Labels;

                        $scope.chart1.DataPercent = result.DataPercent;
                        $scope.chart1.DataAmountPercent = result.DataAmountPercent;

                        $scope.getElementsInidencias();
                        //alert(result);
                        //serviceLoading.stop();


                    });

            }


            $scope.getElementsInidencias = function () {

                serviceLoading.start();
                serviceQueries.getIncidenciasApi($scope.params,
                    function (result) {

                        //console.log(result.data);
                        $scope.chart5.Data = result.Data;
                        $scope.chart5.Series = result.Series;
                        $scope.chart5.Labels = result.Labels;
                        //$scope.chart1.DataAmount = result.DataAmount;

                        //$scope.chart1.DataPercent = result.DataPercent;
                        //$scope.chart1.DataAmountPercent = result.DataAmountPercent;

                        //alert(result);
                        serviceLoading.stop();


                    });
            }


            $scope.getElements();
            //serviceLoading.stop();



        }]);



    //service
    zmodule.service("serviceQueries",
    [
        "Restangular", "service.messages", function (Restangular, serviceMessages) {

            var service = {}

            //var list = {};

            service.copy = function (element) {
                return Restangular.copy(element);
            }

            var all = Restangular.all("Queries");

            //all.customGET("GetAmountOperations", dataQuery).then(fnRespuesta);


            service.getAsentamientosFilter = function (dataQuery, fnRespuesta) {
                var val = {};
                //Restangular.all("Queries").all("GetGraficaTransacciones").get(dataQuery).then(fnRespuesta);

                return all.customGET("GetGraficaTransacciones", dataQuery).then(fnRespuesta);


                //return Restangular.all("Queries").all("GetAsentamientosFilter").get(query);
                //.then(fnRespuesta);
                //.then(fnRespuesta);
            }

            service.getIncidenciasApi = function (dataQuery, fnRespuesta) {
                var val = {};
                //Restangular.all("Queries").all("GetGraficaTransacciones").get(dataQuery).then(fnRespuesta);

                return all.customGET("GetGraficaIndicencias", dataQuery).then(fnRespuesta);


                //return Restangular.all("Queries").all("GetAsentamientosFilter").get(query);
                //.then(fnRespuesta);
                //.then(fnRespuesta);
            }



            //service.getSucursales = function (fnRespuesta) {
            //    Restangular.all("Sucursales").all("GetSucursales").getList().then(fnRespuesta);
            //    //.then(fnRespuesta);

            //}


            //service.getHorarios = function (id, fnRespuesta) {
            //    var val = {};
            //    Restangular.all("Sucursales").all("GetSucursalHorario").get(id).then(fnRespuesta);
            //    //.then(fnRespuesta);

            //}

            //service.getCoverage = function (id, fnRespuesta) {
            //    var val = {};
            //    Restangular.all("Sucursales").all("GetCoverage").get(id).then(fnRespuesta);
            //    //.then(fnRespuesta);

            //}


            //service.getAsentamientosFilter = function (query, fnRespuesta) {
            //    var val = {};
            //    return Restangular.all("Sucursales").all("GetAsentamientosFilter").get(query);
            //    //.then(fnRespuesta);
            //    //.then(fnRespuesta);

            //}





            return service;


        }
    ]);


})(moduleApp);

