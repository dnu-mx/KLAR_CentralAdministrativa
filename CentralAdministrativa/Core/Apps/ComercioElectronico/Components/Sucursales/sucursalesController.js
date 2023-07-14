(function (zmodule) {

    zmodule.controller("Controllers.Sucursales",
    ["$scope", "$rootScope", "$mdDialog", "$filter", "$mdSidenav"
        , "service.loading", "service.messages"
        , "service.sucursales"
        , function (
            $scope, $rootScope, $mdDialog, $filter, $mdSidenav
        , serviceLoading, serviceMessages
        , serviceSucursales
            ) {

            var oldShow = $mdDialog.show;
            $mdDialog.show = function (options) {
                if (options.hasOwnProperty("skipHide")) {
                    options.multiple = options.skipHide;
                }
                return oldShow(options);
            };

            $scope.newElement = {}
            $scope.elements = [];



            $scope.query = {
                order: "id_sucursal",
                limit: 10,
                page: 1
            };

            $scope.options = {
                rowSelection: false,
                multiSelect: false,
                autoSelect: true,
                decapitate: false,
                largeEditDialog: false,
                boundaryLinks: false,
                limitSelect: true,
                pageSelect: true
            };



            $scope.getElements = function () {
                serviceLoading.start();
                serviceSucursales.getSucursales(function (result) {

                    //$scope.elements = result.plain();
                    $scope.elements = result;
                    //serviceMessages.success("test");

                    serviceLoading.stop();
                });
            }


            $scope.openAddSucursal = function () {

                $scope.textSidenav = "Agregar Sucursal";
                $scope.newElement = {}

                
                $mdSidenav("right").open().then(
                    function () {                        
                    });
            }


            $scope.openEditSucursal = function ($event,item) {

                $scope.textSidenav = "Editar Sucursal";
                $scope.newElement = serviceSucursales.copy( item);


                $mdSidenav("right").open().then(
                    function () {
                    });
            }
            
            var add= function(newElement){
                
                serviceSucursales.addSucursal(newElement, function () {

                    $mdSidenav("right").close();

                    serviceMessages.success("Sucursal '"+ newElement.nombre + "' agregada con exito.");

                    $scope.getElements();
                    serviceLoading.stop();
                });

            }
            
            var edit= function(newElement){
                
                serviceSucursales.editSucursal(newElement, function () {

                    $mdSidenav("right").close();
                    serviceMessages.success("Sucursal '"+ newElement.nombre + "' editada con exito.");

                    $scope.getElements();
                    serviceLoading.stop();
                });

            }
            
            $scope.saveNewElement = function (newElement) {

                if (!$scope.baseEditForm.$valid)
                {
                    serviceMessages.alert("Existen campos invalidos, no se puede guardar");
                    return;
                }

                serviceLoading.start();
                if (newElement.id_sucursal > 0) {
                    edit(newElement);
                } else {
                    add(newElement);
                }


            }

            
            $scope.closeSideNav = function () {
                $mdSidenav("right").close().then(function () { $scope.newElement = {} });
            }

            
            
            $scope.openDialogTimes = function ($event, item) {

                serviceLoading.start();
                
                serviceSucursales.getHorarios(item.id_sucursal, function (sucursalHorario)
                {
                    serviceLoading.stop();

                    $mdDialog.show({
                        controller: dialogTimesController,
                        //contentElement: '#myDialog',
                        parent: angular.element(document.body),
                        targetEvent: $event,
                        hasBackDrop: false,
                        clickOutsideToClose: true,
                        templateUrl: "Components/Sucursales/dialogTimes.html",
                        multiple:true,
                        locals:{

                            sucursalHorario: sucursalHorario,
                            sucursal:item
                        }
                    });
                });
                
            }


            function dialogTimesController(sucursalHorario,sucursal,$scope, $mdDialog) {
                //$scope.selectedClient = client;

                $scope.sucursalHorario = sucursalHorario;
                $scope.sucursal = sucursal;

                $scope.hide = function () {
                    $mdDialog.hide();
                };
                $scope.cancel = function () {
                    $mdDialog.cancel();
                };

                $scope.delete = function (item) {
                    var index = $scope.sucursalHorario.WorkDays.indexOf(item);
                    sucursalHorario.WorkDays.splice(index, 1);
                };


                $scope.addItem = function () {
                    
                    var item = {
                        IdDay:7,
                        Range: [
                        new Date(), new Date()
                        ]
                    }

                    sucursalHorario.WorkDays.push(item);
                };
                $scope.updateHorarios = function () {




                    serviceLoading.start();

                    

                    serviceSucursales.updateSucursalHorario($scope.sucursalHorario,
                        function (result) {

                            serviceMessages.success("Horarios de "+sucursal.nombre+" actualizados con exito.");
                            serviceLoading.stop();
                            $mdDialog.cancel();
                        });

                    
                    
                };
                


            }



            $scope.openDialogCoverage= function ($event, item) {

                serviceLoading.start();

                serviceSucursales.getCoverage(item.id_sucursal, function (sucursalCoverage) {
                    serviceLoading.stop();

                    $mdDialog.show({
                        controller: dialogCoverageController,
                        //contentElement: '#myDialog',
                        parent: angular.element(document.body),
                        targetEvent: $event,
                        hasBackDrop: false,
                        clickOutsideToClose: true,
                        templateUrl: "Components/Sucursales/dialogCoverage.html",
                        multiple: true,
                        locals: {

                            sucursalCoverage: sucursalCoverage,
                            sucursal: item
                        }
                    });
                });

            }



            function dialogCoverageController(sucursalCoverage, sucursal, $scope, $mdDialog) {
                //$scope.selectedClient = client;

                $scope.sucursalCoverage = sucursalCoverage;
                $scope.sucursal = sucursal;

                $scope.hide = function () {
                    $mdDialog.hide();
                };
                $scope.cancel = function () {
                    $mdDialog.cancel();
                };

                $scope.delete = function (item) {
                    var index = $scope.sucursalHorario.WorkDays.indexOf(item);
                    sucursalHorario.WorkDays.splice(index, 1);
                };


                $scope.querySearch = function (query) {

                    return serviceSucursales.getAsentamientosFilter(query);

                    //var results = query ? $rootScope.productosList.filter(createFilterFor(query)) : $rootScope.productosList, deferred;
                    //var results = query ? self.states.filter(createFilterFor(query)) : self.states,deferred;

                    //deferred = $q.defer();
                    //$timeout(function () { deferred.resolve(results); }, Math.random() * 1000, false);
                    //return deferred.promise;


                    /*
                    serviceSucursales.getAsentamientosFilter(query,
                        function(result) {

                            return result;
                        });

                    return [];*/

                }

                $scope.searchTextChange = function (text) {
                    //$log.info('Text changed to ' + text);
                }

                $scope.addCoverage = function (item) {

                    var values = $filter("filter")($scope.sucursalCoverage, { clave_asentamiento: item.ID_Asentamiento });


                    if (values.length) {
                        serviceMessages.alert("Codigo postal existente");

                    } else {
                        $scope.sucursalCoverage.push({
                            id: 0,
                            id_sucursal: $scope.sucursal.id_sucursal,
                            clave_asentamiento: item.ID_Asentamiento,
                            descripcion_asentamiento: item.DesAsentamiento,
                            codigo_postal :item.CodigoPostal 
                        });
                    }

                }

                $scope.deleteItem=function(item,ev)
                {
                    var confirm = $mdDialog.confirm({ multiple: true })
                     .title('Borrar cobertura')
                     .textContent('¿Desea borrar este asentamiento ' + item.descripcion_asentamiento + "?")
                     .ariaLabel('Lucky day')
                     .targetEvent(ev)
                     .ok('Borrar')
                     .cancel('Cancelar')
                    ;

                    $mdDialog.show(confirm).then(function () {


                        $scope.sucursalCoverage.splice($scope.sucursalCoverage.indexOf(item), 1);

                        //$scope.status = 'You decided to get rid of your debt.';
                    }, function () {
                        //$scope.status = 'You decided to keep your debt.';
                    });

                    
                }
                $scope.updateCoverage = function () {


                    var dtoSucursal = {
                        id_sucursal : $scope.sucursal.id_sucursal,
                        AreasServicios: $scope.sucursalCoverage
                    }
                    
                    serviceLoading.start();
                    
                    serviceSucursales.updateCoverage(dtoSucursal, function() {
                        serviceMessages.success("Cobertura de " + sucursal.nombre + " actualizada con exito.");
                        serviceLoading.stop();
                        $mdDialog.cancel();
                    });
                }




        
            }


            
            $scope.openDialogBranchOffice= function ($event,item)
            {

                $mdDialog.show({
                    controller: dialogBranchOfficeController,
                    //contentElement: '#myDialog',
                    parent: angular.element(document.body),
                    targetEvent: $event,
                    hasBackDrop: false,
                    clickOutsideToClose: true,
                    templateUrl: "Components/Sucursales/dialogBranchOffice.html",
                    multiple: true,
                    locals: {

                        sucursales: $scope.elements,
                        sucursal: item
                    }
                }).then(function() {
                    $scope.getElements();
                });
                
            }


            function dialogBranchOfficeController (sucursales, sucursal, $scope, $mdDialog)
            {


                $scope.sucursal = serviceSucursales.copy(sucursal);
                $scope.sucursales = sucursales;

                $scope.selectedItem = null;


                if (sucursal.id_suc_sustituta !== null) {
                    var values = $filter("filter")($scope.sucursales, { id_sucursal: sucursal.id_suc_sustituta });

                    if (values.length > 0)
                        $scope.selectedItem = values[0];

                }

                $scope.hide = function () {
                    $mdDialog.hide();
                };
                $scope.cancel = function () {
                    $mdDialog.cancel();
                };



                $scope.querySearch = function (query) {
                    //var results = query ? $rootScope.productosList.filter(createFilterFor(query)) : $rootScope.productosList, deferred;
                    var values = $filter("filter")($scope.sucursales, { nombre: query });

                    if (values.length)
                        return values;
                    else
                        //var results = $rootScope.productosList;
                        return values;
                }

                $scope.searchTextChange = function (text) {
                    //$log.info('Text changed to ' + text);
                }

                $scope.pickBranchOffice = function () {

                    //if ($scope.selectedItem == null) serviceMessages.alert("null");
                    //sucursal.id_suc_sustituta = selectedItem.id_sucursal;

                    if ($scope.selectedItem == null)
                    //serviceMessages.alert("null");
                        $scope.sucursal.id_suc_sustituta = null;
                    else {
                        //serviceMessages.alert($scope.selectedItem.id_sucursal);
                        $scope.sucursal.id_suc_sustituta = $scope.selectedItem.id_sucursal;
                    }

                    //

                    /*serviceLoading.start();
                    serviceSucursales.editSucursal(element,
                        function() {
                            
                        });*/


                }

                $scope.updateBranchOffice=function(){

                    serviceLoading.start();

                    serviceSucursales.editSucursalSub($scope.sucursal,
                        function() {

                            serviceMessages.success("Sucursal "+$scope.sucursal.nombre+" modificada con exito." );
                            $mdDialog.hide();
                            serviceLoading.stop();
                        });

                }



            }




            $scope.getElements();
            //serviceLoading.stop();



        }]);



    //service
    zmodule.service("service.sucursales",
    [
        "Restangular", "service.messages", function(Restangular, serviceMessages) {

            var service = {}

            //var list = {};

            service.copy = function(element) {
                return Restangular.copy(element);
            }
            service.getSucursales = function(fnRespuesta) {
                Restangular.all("Sucursales").all("GetSucursales").getList().then(fnRespuesta);
                //.then(fnRespuesta);

            }

            service.addSucursal = function(element, fnRespuesta) {
                var val = {};
                Restangular.all("Sucursales").all("AddSucursal").post(element).then(fnRespuesta);
                //.then(fnRespuesta);

            }

            service.editSucursal = function(element, fnRespuesta) {
                var val = {};
                Restangular.all("Sucursales").all("EditSucursal").post(element).then(fnRespuesta);
            }

            service.editSucursalSub = function(element, fnRespuesta) {
                var val = {};
                Restangular.all("Sucursales").all("EditSucursalSub").post(element).then(fnRespuesta);
            }


            service.getHorarios = function(id, fnRespuesta) {
                var val = {};
                Restangular.all("Sucursales").all("GetSucursalHorario").get(id).then(fnRespuesta);
                //.then(fnRespuesta);

            }

            service.getCoverage = function(id, fnRespuesta) {
                var val = {};
                Restangular.all("Sucursales").all("GetCoverage").get(id).then(fnRespuesta);
                //.then(fnRespuesta);

            }


            service.getAsentamientosFilter = function(query, fnRespuesta) {
                var val = {};
                return Restangular.all("Sucursales").all("GetAsentamientosFilter").get(query);
                //.then(fnRespuesta);
                //.then(fnRespuesta);

            }


            service.updateCoverage = function(element, fnRespuesta) {
                var val = {};
                Restangular.all("Sucursales").all("UpdateCoverage").post(element).then(fnRespuesta);
                //.then(fnRespuesta);

            }


            service.updateSucursalHorario = function(element, fnRespuesta) {
                var val = {};
                Restangular.all("Sucursales").all("UpdateSucursalHorario").post(element).then(fnRespuesta);
                //.then(fnRespuesta);

            }



            

            return service;


        }
    ]);


})(moduleApp);

