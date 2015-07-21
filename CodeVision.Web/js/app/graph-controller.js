'use strict';

graphModule.controller("GraphController", function ($scope, graphService, $log) {
    $scope.selectedModule = null;
    $scope.direction = 0;
    $scope.levels = 1;    

    $scope.getModuleNames = function (name) {
        return graphService.getModuleNames(name)
            .$promise
            .then(function (response) {                
                return response;
            });
    };

    $scope.getDependencies = function () {
        graphService.getDependencies($scope.selectedModule, $scope.direction, $scope.levels)
            .$promise.then(function (response) {
                $scope.modules = response;
        });
    }

    $scope.onSelected = function ($item, $model, $label) {
        $scope.modules = null;
    };
});