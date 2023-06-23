(function () {
    "use strict";

    function ctrl($scope, $routeParams, $location, $filter, pageguardResource) {

        var vm = this;
        vm.millisToUTCDate = pageguardResource.convertToUTC;

        var nodeId = $routeParams.id;

        var defaultHeading = $(".umb-overlay__title").html();
        $(".umb-overlay__title").html("<span class=\"icon-lock large\"></span>" + defaultHeading);

        $scope.nodeId = nodeId;
        $scope.currentUrl = $location.absUrl();

        pageguardResource.CheckCurrentAccessDetails(nodeId).then(function (data) {
            $scope.latestUsername = data[0].Username;

            $scope.latestDateTimeRecord = $filter('date')(data[0].DateTimeRecord, 'HH:mm') ;
            $scope.latestDateRecord = $filter('date')(data[0].DateTimeRecord, 'dd/MM/yyyy');

            $scope.latestEmail = data[0].UserEmail;
        });

        pageguardResource.LoadAdminListForNotifications(nodeId).then(function (data) {
            $scope.list = data;
        });

    }

    angular.module('umbraco').controller('pageguard.notify.controller', ['$scope', '$routeParams', '$location', '$filter', 'pageguardResource', ctrl]);

})();