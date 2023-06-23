(function () {
    "use strict";

    function ctrl($scope, $routeParams) {

        var vm = this;

        var nodeId = $routeParams.id;

        $scope.nodeId = nodeId;

        var defaultHeading = $(".umb-overlay__title").html();
        $(".umb-overlay__title").html("<span class=\"icon-unlocked large\"></span>" + defaultHeading);

    }

    angular.module('umbraco').controller('pageguard.checkout.controller', ['$scope', '$routeParams', ctrl]);

})();