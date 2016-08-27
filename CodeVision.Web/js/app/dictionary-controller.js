'use strict';

graphModule.controller("DictionaryController", function ($scope, bootstrappedData, dictionaryService, $log) {
    $scope.selectedObject = null;
    $scope.selectedSproc = null;
    $scope.properties = bootstrappedData.properties;
    $scope.selectedProperty = $scope.properties[0];
    $scope.addPropertyError = null;
    $scope.direction = 0;
    $scope.levels = 1;

    $scope.getObjectNames = function (name) {
        return dictionaryService.getObjectNames(name, "2,3,4")
            .$promise
            .then(function (response) {                
                return response;
            });
    };

    $scope.getColumnNames = function (name) {
        return dictionaryService.getObjectNames(name, "3")
            .$promise
            .then(function (response) {
                return response;
            });
    };

    $scope.addProperty = function () {
        $scope.addPropertyError = null;
        var property = {
            propertyType: $scope.selectedProperty.propertyType
        };
        dictionaryService.addProperty($scope.selectedObject.id, property)
          .$promise.then(function(data) {
                 $scope.selectedObject.properties = data.properties;
              },
              function (error) {
                  $scope.addPropertyError = error.data;
              });
    };

    $scope.deleteProperty = function(propertyType) {
        dictionaryService.deleteProperty($scope.selectedObject.id, propertyType)
            .$promise.then(
                function(data) {
                    $scope.selectedObject.properties = data.properties;
                });
    };

    $scope.deleteColumn = function (columnId) {
        dictionaryService.deleteColumn($scope.selectedObject.id, columnId)
            .$promise.then(
                function (data) {
                    $scope.selectedSproc = data;
                });
    };

    $scope.updateComment = function(property) {
        dictionaryService.updateComment($scope.selectedObject.id, property.propertyValue);
    }

    $scope.addColumn = function() {
        dictionaryService.addColumn($scope.selectedObject.id, $scope.selectedColumn.id)
            .$promise.then(function(response) {
                $scope.selectedSproc = response;
            });
    };

    $scope.addDependentColumn = function() {
        dictionaryService.addDependentObject($scope.selectedObject.id, $scope.dependentColumn.id)
            .then(function() {
                dictionaryService.getDependencies($scope.selectedObject.id, 0, 0, 3)
                    .then(function(response) {
                        $scope.columns = response;
                    });
            });
    };

    $scope.deleteDependentColumn = function (columnId) {
        dictionaryService.deleteDependentColumn($scope.selectedObject.id, columnId)
            .then(function () {
                dictionaryService.getDependencies($scope.selectedObject.id, 0, 0, 3)
                    .then(function (response) {
                        $scope.columns = response;
                    });
            });
    };


    $scope.getTableDependencies = function() {
        dictionaryService.getDependencies($scope.selectedObject.id, $scope.direction, $scope.levels, 2)
            .then(function(response) {
                $scope.objects = response;
            });
    };

    // Get dependend sprocs and columns in one call
    $scope.getColumnDependencies = function() {
        dictionaryService.getDependencies($scope.selectedObject.id, 1, 0, 4)
            .then(function(response) {
                $scope.sprocs = response;
            })
            .then(function() {
                dictionaryService.getDependencies($scope.selectedObject.id, 0, 0, 3)
                    .then(function(response) {
                        $scope.columns = response;
                    });
            });
    };

    $scope.onSelected = function (item, model, label) {
        $scope.addPropertyError = null;
        if (item.objectType === 4) {
            // Sproc
            dictionaryService.getSproc(item.id)
                .$promise.then(function(response) {
                    $scope.selectedSproc = response;
                });
        }

        if (item.objectType === 2) {
            // Table
            $scope.getTableDependencies();
        }

        if (item.objectType === 3) {
            // Column
            $scope.getColumnDependencies();
        }
    };

    $scope.onColumnSelected = function (item, model, label) {
    };

    $scope.save = function () {
        dictionaryService.save()
          .$promise.then(function (data) {},
           function (error) { $log.error(error); });
    };

    // This is just to avoid typing it in during debugging
    $scope.setDebugObjectToColumn = function () {
        $scope.selectedObject = {
            id: 7,
            name: "DatabaseLogID",
            fullyQualifiedName: "AdventureWorks2012.dbo.DatabaseLog.DatabaseLogID",
            objectType: 3,
            objectTypeName: "Column",
            properties: []
        };
    };
    $scope.setDebugObjectToSproc = function () {
        $scope.selectedObject = {
            id: 558,
            name: "uspGetBillOfMaterials",
            fullyQualifiedName: "AdventureWorks2012.dbo.uspGetBillOfMaterials",
            objectType: 4,
            objectTypeName: "StoredProcedure",
            properties: []
        };
    };

    $scope.setDebugObjectToTable = function () {
        $scope.selectedObject = {
            id: 74,
            name: "Address",
            fullyQualifiedName: "AdventureWorks2012.Person.Address",
            objectType: 2,
            objectTypeName: "Table",
            properties: []
        };
    };

    //$scope.setDebugObjectToColumn();
});