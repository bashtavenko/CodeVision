'use strict';

graphModule.controller("NugetController", function ($scope, nugetService, $log) {
    $scope.selectedPackage = null;

    $scope.getPackages = function (name) {
        return nugetService.getPackages(name)
            .$promise
            .then(function (response) {                
                return response;
            });
    };

    $scope.getProjects = function () {
        nugetService.getProjects($scope.selectedPackage.id)
            .$promise.then(function (response) {
                $scope.projects = response;
        });
    }

    $scope.onSelected = function () {
        $scope.getProjects();
    };
});