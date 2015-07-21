var graphModule = angular.module("graphModule", ['ngResource', 'ui.bootstrap'])

graphModule.config(function ($httpProvider) {
    $httpProvider.interceptors.push('myInterceptor');
    
    var spinner = function spinnerFunction(data, headersGetter) {
        $("#spinner").show();
        return data;
    };    

    $httpProvider.defaults.transformRequest.push(spinner);
});

graphModule.factory('myInterceptor', function($q) {
    return {
        'response': function (response) {
            $("#spinner").hide();
            return response;
        },
        'responseError': function (rejection) {
            $("#spinner").text("Error occured. See details in server log.");
            $("#spinner").css("color", '#ff0906');
            return $q.reject(rejection);
        }
    };
});
