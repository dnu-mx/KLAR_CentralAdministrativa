
var moduleApp = angular.module("ZApp", [
//"ngRoute",
//"ngResource",
"ngMaterial",
"ngAria",
"module.messages",
"module.loading"
, "ngMessages"
//, "md.data.table"
//, "fixed.table.header"
 //, "ui.mask"
//, "angularFileUpload"
//, "ui.utils.masks"
, "restangular"
, "chart.js"
, "mdPickers"
 //, "infinite-scroll"
//, "ui.router"
//, "sasrio.angular-material-sidenav"

]);



// Configure the $httpProvider by adding our date transformer


(function (zmodule) {


    zmodule.config(["$httpProvider", function ($httpProvider) {
        $httpProvider.defaults.transformResponse.push(function (responseData) {
            convertDateStringsToDates(responseData);
            return responseData;
        });
    }]);

    var regexIso8601 = re = /^([\+-]?\d{4}(?!\d{2}\b))((-?)((0[1-9]|1[0-2])(\3([12]\d|0[1-9]|3[01]))?|W([0-4]\d|5[0-2])(-?[1-7])?|(00[1-9]|0[1-9]\d|[12]\d{2}|3([0-5]\d|6[1-6])))([T\s]((([01]\d|2[0-3])((:?)[0-5]\d)?|24\:?00)([\.,]\d+(?!:))?)?(\17[0-5]\d([\.,]\d+)?)?([zZ]|([\+-])([01]\d|2[0-3]):?([0-5]\d)?)?)?)?$/;

    function convertDateStringsToDates(input) {
        // Ignore things that aren't objects.
        if (typeof input !== "object") return input;

        for (var key in input) {
            if (!input.hasOwnProperty(key)) continue;

            var value = input[key];
            var match;
            // Check for string properties which look like dates.
            // TODO: Improve this regex to better match ISO 8601 date strings.
            if (typeof value === "string" && (match = value.match(regexIso8601)) && value.length > 6) {
                // Assume that Date.parse can parse ISO 8601 strings, or has been shimmed in older browsers to do so.
                var milliseconds = Date.parse(match[0]);
                if (!isNaN(milliseconds)) {
                    input[key] = new Date(milliseconds);
                }
            } else if (typeof value === "object") {
                // Recurse into object
                convertDateStringsToDates(value);
            }
        }
    }


    zmodule.config([
        "$mdThemingProvider", "$mdIconProvider" //materialbs
        , "RestangularProvider"
        , "$mdDateLocaleProvider"
        //," service.loading"
        , function ($mdThemingProvider,
            $mdIconProvider,
            RestangularProvider
            , $mdDateLocaleProvider
            //,serviceLoading
        ) {



            //$mdDateLocaleProvider.formatDate = function (date) {
            //    return date ? moment(date).format("DD/MM/YYYY") : "";
            //};

            //$mdDateLocaleProvider.parseDate = function (dateString) {
            //    var m = moment(dateString, "DD/MM/YYYY", true);
            //    return m.isValid() ? m.toDate() : new Date(NaN);
            //};

            $mdIconProvider.defaultIconSet("Content/Mdi/mdi.svg");
            //RestangularProvider.setBaseUrl("/api/");

            RestangularProvider.setBaseUrl("api/");

            $mdThemingProvider.theme("default")
                .primaryPalette("red",
                {
                    //"default": "400"
                });

            //var softBlue = $mdThemingProvider.extendPalette('blue-grey',
            //{
            //    '500': '#D4E0F2',
            //    'contrastDefaultColor': 'dark'
            //});

            //// Register the new color palette map with the name <code>neonRed</code>
            //$mdThemingProvider.definePalette('softBlue', softBlue);


            //$mdThemingProvider.theme("lowBlue").primaryPalette("softBlue");
            //$mdThemingProvider.theme("hasDataColor").primaryPalette("blue", { 'default': "900" });


            ////$mdThemingProvider.theme("green").primaryPalette("green");

        }
    ]);

    zmodule.run(


        ["$rootScope", "Restangular", "service.loading", "service.messages"
            ,
            function (
                $rootScope, Restangular, serviceLoading, serviceMessages) {


                Restangular.setErrorInterceptor(
            function (response) {

                if (response.status === 404) {
                    serviceMessages.error(response.data ? response.data.MessageDetail : "Error en servicio");

                    serviceLoading.stop();
                }
                else
                    if (response.status === 401) {
                        /*dialogs.error("Unauthorized - Error 401", "You must be authenticated in order to access this content.")
                            .result.then(function () {
                                $location.path("/login");
                            });*/

                        serviceMessages.error(response.data ? response.data.ExceptionMessage : "Error en servicio");

                        serviceLoading.stop();
                    }
                    else {

                        serviceMessages.error(response.data ? response.data.ExceptionMessage : "Error en servicio");
                        serviceLoading.stop();
                        // Some other unknown Error.
                        //console.log(response);
                        //dialogs.error(response.statusText + " - Error " + response.status,
                        //    "An unknown error has occurred.<br>Details: " + response.data);
                    }
                // Stop the promise chain.
                return false;
            }
        );
            }


        ]);

})(moduleApp);