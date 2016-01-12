'use strict';

graphModule.factory("nugetService", function ($resource) {
    return {
        getPackages: function (name) {
            return $resource("/api/nugets/packages/", { name: "@name" })
                .query({ name: name });
        },
        getProjects (packageId) {
            return $resource("/api/nugets/projects/:packageId", { packageId: "@packageId" })
                .query({ packageId: packageId });
        }
    }
});