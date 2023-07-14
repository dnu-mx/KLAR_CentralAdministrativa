
(function () {
    
    var zmodule = angular.module("module.loading", []);

    zmodule.run(["$rootScope"

        , function ($rootScope) {

 
            $rootScope.disableActions = false;
            
            

        }]);

    zmodule.service("service.loading", ["$rootScope", function ($rootScope) {
        return {

            start: function () {

                //$rootScope.mdloading = "indeterminate";
                $rootScope.disableActions = true;
                
            },

            stop: function () {

                $rootScope.disableActions = false;

                //$rootScope.mdloading = "determinate";
                //$rootScope.mdloadingvalue = 100;
                
            },
            isVisible: function () {
                
            }
        };
    }]);
})();




(function () {

    
    var zmodule = angular.module("module.messages", []);
    /*
    zmodule.run([function () {

    

    }]);
    */
  
    zmodule.service("service.messages", ["$rootScope", "$mdToast", function ($rootScope, $mdToast) {
        return {
            success: function (mensaje) {
                //$mdToast.s.how($mdToast.simple().content(mensaje).theme("success-toast"));
                $mdToast.show($mdToast.simple({ hideDelay: 7000 }).content(mensaje));
                //modulo.toast.setAttribute('text', mensaje);
                //modulo.toast.classList.remove("error");
                //modulo.toast.s.how();
            },
            alert: function (mensaje) {
                $mdToast.show($mdToast.simple({ hideDelay: 7000 }).content(mensaje));
                //modulo.toast.setAttribute('text', mensaje);
                //modulo.toast.classList.remove("error");
                //modulo.toast.s.how();
            },
            error: function (mensaje) {
                //$mdToast.s.how($mdToast.simple().content(mensaje).theme("success-toast"));
                $mdToast.show($mdToast.simple({ hideDelay: 7000 }).content(mensaje));
                //modulo.toast.setAttribute('text', mensaje);
                //modulo.toast.classList.add("error");
                //modulo.toast.s.how();
            }

        };
    }]);
})();