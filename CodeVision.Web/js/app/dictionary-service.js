'use strict';

graphModule.factory("dictionaryService", function ($resource) {
    return {
        getObjectNames: function(name, onlyTheseTypes) {
            var params = {
                name: name,
                onlyTheseTypes: onlyTheseTypes
            };
            return $resource("/api/objects/:onlyTheseTypes", { name: "@name", onlyTheseTypes: "@onlyTheseTypes" })
                .query(params);
        },
        getObject: function(objectId) {
            var params = {
                objectId: objectId
            };
            return $resource("/api/object/:objectId", { objectId: "@objectId" })
                .get(params);
        },
        getPropertyList: function() {
            return $resource("/api/properties")
                .query();
        },
        addProperty: function(objectId, property) {
            var params = { objectId: objectId };
            var url = "/api/object/" + objectId + "/property";
            return $resource(url).save(property);
        },
        deleteProperty: function(objectId, propertyType) {
            var params = {
                objectId: objectId,
                propertyType: propertyType
            };
            return $resource("/api/object/:objectId/property/:propertyType", { objectId: "@objectId", propertyType: "@propertyType" })
                .delete(params);
        },
        deleteColumn: function (objectId, columnId) {
            var params = {
                objectId: objectId,
                columnId: columnId
            };
            return $resource("/api/sproc/:objectId/column/:columnId", { objectId: "@objectId", columnId: "@columnId" })
                .delete(params);
        },
        updateComment: function(objectId, text) {
            var url = "/api/object/" + objectId + "/comment";
            var r = $resource(url, null, { "update": { method: "PUT" } });
            return r.update('"' + text + '"');
        },
        getSproc: function(objectId) {
            var params = {
                objectId: objectId
            };
            return $resource("/api/sproc/:objectId", { objectId: "@objectId" })
                .get(params);
        },
        addColumn: function (objectId, columnId) {
            return $resource("/api/sproc/" + objectId + "/columns/" + columnId).save();
        },
        getDependencies: function (objectId, direction, level, objectsType) {
            var params = {
                objectId: objectId,
                direction: direction,
                level: level,
                objectsType: objectsType
            };
            return $resource("/api/objects/:objectId/:direction/:level/:objectsType",
                {
                    objectId: "@objectId",
                    direction: "@direction",
                    level: "@level",
                    objectsType: "@objectsType"
                })
                .query(params);
        },
        save: function() {
            return $resource("/api/objects/save", null, { "update": { method: "PUT" } }).update();
        }
    }
});