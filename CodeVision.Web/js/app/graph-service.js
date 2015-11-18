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
        getTableDependencies (moduleId, direction, level) {
            var params = {
                moduleId: moduleId,
                direction: direction,
                level: level
            };
            return $resource("/api/graph/:moduleId/:direction/:level",
                {
                    moduleId: "@moduleId",
                    direction: "@direction",
                    level: "@level",
                })
                .query(params);
        }
    }
});