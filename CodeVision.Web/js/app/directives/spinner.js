'use strict';

graphModule.directive('spinner', function () {
    return {
        restrict: 'E',
        template: "<div id='spinner' style='display:none;margin-top:15px;'><strong>Wait..</strong></div>"
    }
});