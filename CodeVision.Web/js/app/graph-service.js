'use strict';

graphModule.factory("graphService", function ($resource) {
    return {
        getModuleNames: function (name) {
            var params = {
                name: name
            };
            return $resource("/api/modules", { name: "@name" })
                .query(params);
        },
        getDependencies (key, direction, levels) {
            var params = {
                key: key,
                direction: direction,
                levels: levels
            };
            return $resource("/api/graph/:direction/:levels",
                {
                    key: "@key",
                    direction: "@direction",
                    levels: "@levels",
                })
                .query(params);
        }
    }
});