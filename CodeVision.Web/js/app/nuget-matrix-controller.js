'use strict';

graphModule.controller("NugetMatrixController", function ($scope, nugetMatrixService, bootstrap) {
    $scope.dm = bootstrap.dependencyMatrix;
    $scope.projects = "";
    $scope.packages = "";

    $scope.refresh = function () {
        return nugetMatrixService.getMatrix($scope.projects, $scope.packages)
            .$promise
            .then(function (response) {                
                $scope.dm = response;
            });
    };
});