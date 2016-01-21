'use strict';

graphModule.factory("nugetMatrixService", function ($resource) {
    return {
        getMatrix: function (project, nuGetPackage) {
            return $resource("/api/nugets/matrix", { project: "@project", nuGetPackage: "@package" })
                .get({ project: project, "package": nuGetPackage });
        }
    }
});