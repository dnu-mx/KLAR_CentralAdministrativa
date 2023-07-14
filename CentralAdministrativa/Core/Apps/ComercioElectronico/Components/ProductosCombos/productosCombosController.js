(function (zmodule) {

    zmodule.controller("Controllers.ProductosCombos",
    ["$scope", "$rootScope", "$mdDialog", "$filter", "$mdSidenav"
        , "service.loading", "service.messages"
        , "service.productos"
        , function (
            $scope, $rootScope, $mdDialog, $filter, $mdSidenav
        , serviceLoading, serviceMessages
        , serviceProductos
            ) {

            /*var oldShow = $mdDialog.show;
            $mdDialog.show = function (options) {
                if (options.hasOwnProperty("skipHide")) {
                    options.multiple = options.skipHide;
                }
                return oldShow(options);
            };*/

            $scope.newElement = {}
            $scope.elements = [];

            $rootScope.familiasList = [];
            $rootScope.productosList = [];


            $scope.query = {
                order: "producto_id",
                limit: 5,
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


            serviceProductos.getFamilias(function (result) { $rootScope.familiasList = result; });
            serviceProductos.getProductos(function (result) { $rootScope.productosList = result; });


            $scope.getElements = function () {
                serviceLoading.start();
                serviceProductos.getProductosDeCombos(function (result) {

                    //$scope.elements = result.plain();
                    $scope.elements = result;
                    //serviceMessages.success("test");

                    serviceLoading.stop();
                });
            }


            $scope.openAddSideNav= function () {

                $scope.textSidenav = "Agregar Producto";
                $scope.newElement = {}

                
                $mdSidenav("newRight").open().then(function () { });
            }


            $scope.openEditSideNav = function ($event, item) {

                $scope.textSidenav = "Editar Producto";
                $scope.newElement = serviceProductos.copy(item);


                $mdSidenav("right").open().then(
                    function () {
                    });
            }
            
            var add= function(newElement){
                
                serviceProductos.addProductoCombo(newElement, function () {

                    $mdSidenav("right").close();

                    serviceMessages.success("Producto  '"+ newElement.nombre + "' agregado con exito.");

                    $scope.getElements();
                    serviceLoading.stop();
                });

            }
            
            var edit= function(newElement){
                
                serviceProductos.editProductoCombo(newElement, function () {

                    $mdSidenav("right").close();
                    serviceMessages.success("Producto '"+ newElement.nombre + "' editado con exito.");

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
                if (newElement.producto_id > 0) {
                    edit(newElement);
                } else {
                    add(newElement);
                }


            }

            $scope.updateNewElement = function (newElement) {

                if (!$scope.baseEditForm2.$valid) {
                    serviceMessages.alert("Existen campos invalidos, no se puede guardar");
                    return;
                }

                serviceProductos.updateProductoToCombo(newElement, function () {

                    $mdSidenav("newRight").close();

                    serviceMessages.success("Producto   agregado con exito.");

                    $scope.getElements();
                    serviceLoading.stop();
                });



            }


            


            $scope.closeSideNav = function (sideNav) {

                if (sideNav == null)
                    $mdSidenav("right").close().then(function() { $scope.newElement = {} });
                else 
                    $mdSidenav(sideNav).close().then(function () { $scope.newElement = {} });
                
                
                
            }




            
            $scope.openDialogPasos = function ($event, item) {

                 serviceLoading.start();
                
                serviceProductos.getPasosCombos(item.producto_id, function (pasos)
                {
                    serviceLoading.stop();

                    $mdDialog.show({
                        controller: dialogController,
                        //contentElement: '#myDialog',
                        parent: angular.element(document.body),
                        targetEvent: $event,
                        hasBackDrop: false,
                        clickOutsideToClose: true,
                        templateUrl: "Components/ProductosCombos/dialogPasos.html",
                        //multiple: true,
                        //skipHide:true,
                        locals:{

                            pasos: pasos,
                            producto:item
                        }
                    });
                });
                
               

            }


            function dialogController(pasos, producto, $scope, $mdDialog) {
                //$scope.selectedClient = client;

                $scope.pasos = pasos;
                $scope.producto = producto;
                $scope.selectedItem = null;

                $scope.hide = function () {
                    $mdDialog.hide();
                };
                $scope.cancel = function () {
                    $mdDialog.cancel();
                };

          
                 $scope.querySearch=function (query) {
                     //var results = query ? $rootScope.productosList.filter(createFilterFor(query)) : $rootScope.productosList, deferred;
                     var values = $filter("filter")($rootScope.productosList, { Name: query });

                     if (values.length)
                         return values;
                     else
                     //var results = $rootScope.productosList;
                         return values;
                 }

                 $scope.searchTextChange = function (text) {
                     //$log.info('Text changed to ' + text);
                 }



                /*
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
                };*/


                 $scope.addStep = function () {
                     var maxStep = 0;

                     if($scope.pasos.length)
                        maxStep = Math.max.apply(Math, $scope.pasos.map(function (o) { return o.secuencia; }));

                     maxStep += 1;

                     $scope.pasos.push({
                         id:0,
                         secuencia: maxStep,
                         descripcion: "",
                         cantidad: 1,
                         id_producto:producto.producto_id,
                         ProductosCombos:[]
                     });


                 }

                 $scope.deleteStep = function (step,ev) {


                     var confirm = $mdDialog.confirm({multiple:true})
                       .title('Borrar paso')
                       .textContent('¿Desea borrar este paso ?')
                       .ariaLabel('Lucky day')
                       .targetEvent(ev)
                       .ok('Borrar')
                       .cancel('Cancelar')
                     ;

                     $mdDialog.show(confirm).then(function () {

                         
                         $scope.pasos.splice($scope.pasos.indexOf(step),1);

                         //$scope.status = 'You decided to get rid of your debt.';
                     }, function () {
                         //$scope.status = 'You decided to keep your debt.';
                     });
                 }



                 $scope.addProduct = function (selectedItem,tab) {
                     
                     tab.ProductosCombos.push({
                         id_producto: selectedItem.Id,
                         NameProducto: selectedItem.Name,
                         SkuProducto:selectedItem.Sku,
                         id_pasos_combos:tab.id
                     });

                     //$scope.searchText = "";
                     //$scope.selectedItem = [];
                 }



                 $scope.deleteProduct = function (product,tab, ev) {


                     var confirm = $mdDialog.confirm({ multiple: true })
                       .title('Borrar paso')
                       .textContent('¿Desea borrar este producto ' + product.NameProducto + "?")
                       .ariaLabel('Lucky day')
                       .targetEvent(ev)
                       .ok('Borrar')
                       .cancel('Cancelar')
                     ;

                     $mdDialog.show(confirm).then(function () {


                         tab.ProductosCombos.splice(tab.ProductosCombos.indexOf(product),1);

                         //$scope.status = 'You decided to get rid of your debt.';
                     }, function () {
                         //$scope.status = 'You decided to keep your debt.';
                     });
                 }





                 $scope.updateProductosCombos = function () {



                    serviceLoading.start();

                    

                    serviceProductos.updatePasosCombos($scope.pasos,
                        function (result) {

                            serviceMessages.success("Pasos de "+producto.nombre+" actualizados con exito.");
                            serviceLoading.stop();
                            $mdDialog.cancel();
                        });
                    
                    
                    
                };
                



                /*    $scope.accept = function (selfClient) {
    
    
                        if ($scope.selectedUser == null) {
                            serviceMessages.alert("Usuario no seleccionado");
                            return;
                        }
    
                        $scope.buttonDisabled = true;
    
                        serviceLoading.start();
    
                        serviceAutCreditos.sendMailAuthBase($scope.credit.IdCliente, selfClient.Id, function () {
    
                            serviceMessages.success("Correo electrónico enviado a " + selfClient.Name);
                            $scope.buttonDisabled = false;
                            serviceLoading.stop();
                            $mdDialog.hide();
    
    
                        }
                        , function (errorResult) {
                            $scope.buttonDisabled = false;
                            serviceLoading.stop();
                        });
                        //serviceMessages.success("Correo electrónico enviado a " + selfClient.Name);
    
                        //$rootScope.seedIdCliente = selfClient.NumeroOperacion;
                        //$rootScope.seedClientSmall = selfClient;
                        //$location.path('/pmorales/autorizacion/');
    
                        //                    $mdDialog.hide();
                        //  serviceLoading.stop();
                    };
    
                    $scope.answer = function (answer) {
                        $mdDialog.hide(answer);
                    };*/
            }

            

            $scope.getElements();
            //serviceLoading.stop();



        }]);



    //service
    zmodule.service("service.productos",
    [
        "Restangular", "service.messages", function(Restangular, serviceMessages) {

            var service = {}

            service.copy = function(element) {
                return Restangular.copy(element);
            }
            service.getProductosDeCombos = function(fnRespuesta) {
                Restangular.all("ProductosCombos").all("GetProductosCombos").getList().then(fnRespuesta);
            }

            service.getFamilias = function(fnRespuesta) {
                Restangular.all("ProductosCombos").all("GetFamilias").getList().then(fnRespuesta);
            }

            service.getProductos = function (fnRespuesta) {
                Restangular.all("ProductosCombos").all("GetProductos").getList().then(fnRespuesta);
            }


            service.addProductoCombo = function(element, fnRespuesta) {
                Restangular.all("ProductosCombos").all("AddProductoCombo").post(element).then(fnRespuesta);
            }

            service.editProductoCombo = function(element, fnRespuesta) {
                Restangular.all("ProductosCombos").all("EditProductoCombo").post(element).then(fnRespuesta);
            }

            service.updateProductoToCombo = function (element, fnRespuesta) {
                Restangular.all("ProductosCombos").all("UpdateProductoToCombo").post(element).then(fnRespuesta);
            }
            
            service.getPasosCombos = function (id, fnRespuesta) {
                var val = {};
                Restangular.all("ProductosCombos").all("GetPasosCombos").get(id).then(fnRespuesta);
            }
            
            service.updatePasosCombos = function (pasos, fnRespuesta) {
                var val = {};
                Restangular.all("ProductosCombos").all("UpdatePasosCombos").post(pasos).then(fnRespuesta);
            }

            

            return service;


        }
    ]);


})(moduleApp);

