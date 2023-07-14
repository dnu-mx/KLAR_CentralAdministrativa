(function (zmodule) {

    zmodule.controller("EmpresasController",
    ["$scope", "$rootScope", "$mdDialog", "$filter", "$mdSidenav"
        , "service.loading", "service.messages"
        , "serviceEmpresas"
        , function (
            $scope, $rootScope, $mdDialog, $filter, $mdSidenav
        , serviceLoading, serviceMessages
        , serviceEmpresas
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
                order: "ID_Empresa",
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
                serviceEmpresas.getEmpresasApi(function (result) {

                    //$scope.elements = result.plain();
                    $scope.elements = result;
                    //serviceMessages.success("test");

                    serviceLoading.stop();
                });
            }


            $scope.openAddEmpresa = function () {

                $scope.textSidenav = "Agregar Empresa";
                $scope.newElement = {}

                
                $mdSidenav("right").open().then(
                    function () {                        
                    });
            }


            $scope.openEditEmpresa = function ($event,item) {

                $scope.textSidenav = "Editar Empresa";
                $scope.newElement = serviceEmpresas.copy( item);


                $mdSidenav("right").open().then(
                    function () {
                    });
            }
            
            var add= function(newElement){
                
                serviceEmpresas.addEmpresa(newElement, function () {

                    $mdSidenav("right").close();

                    serviceMessages.success("Empresa '"+ newElement.NombreComercial + "' agregada con exito.");

                    $scope.getElements();
                    serviceLoading.stop();
                });

            }
            
            var edit= function(newElement){
                
                serviceEmpresas.editEmpresa(newElement, function () {

                    $mdSidenav("right").close();
                    serviceMessages.success("Empresa '" + newElement.NombreComercial + "' editada con exito.");

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
                if (newElement.ID_Empresa > 0) {
                    edit(newElement);
                } else {
                    add(newElement);
                }


            }

            
            $scope.closeSideNav = function () {
                $mdSidenav("right").close().then(function () { $scope.newElement = {} });
            }

            
    



  


            $scope.getElements();
            //serviceLoading.stop();



        }]);



    //service
    zmodule.service("serviceEmpresas",
    [
        "Restangular", "service.messages", function(Restangular, serviceMessages) {

            var service = {}

            //var list = {};

            service.copy = function(element) {
                return Restangular.copy(element);
            }
            service.getEmpresasApi = function(fnRespuesta) {
                Restangular.all("Empresas").all("GetEmpresas").getList().then(fnRespuesta);
                //.then(fnRespuesta);

            }

            service.addEmpresa = function(element, fnRespuesta) {
                var val = {};
                Restangular.all("Empresas").all("AddEmpresa").post(element).then(fnRespuesta);
                //.then(fnRespuesta);

            }

            service.editEmpresa = function(element, fnRespuesta) {
                var val = {};
                Restangular.all("Empresas").all("EditEmpresa").post(element).then(fnRespuesta);
            }




            

            return service;


        }
    ]);


})(moduleApp);

