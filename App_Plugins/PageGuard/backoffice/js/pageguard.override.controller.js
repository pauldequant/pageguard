(function () {
    "use strict";

    function ctrl($scope, $routeParams, $filter, pageguardResource, contentResource) {

        var vm = this;

        vm.getNameInitials = pageguardResource.getNameInitials;
        vm.millisToUTCDate = pageguardResource.convertToUTC;

        var nodeId = $routeParams.id;

        $scope.nodeId = nodeId;

        $scope.lastPageUpdatedDate = "";

        contentResource.getById($routeParams.id)
            .then(function (content) {
                $scope.lastPageUpdatedDate = $filter('date')(new Date(content.updateDate), 'dd/MM/yyyy');
                $scope.lastPageUpdatedTime = $filter('date')(new Date(content.updateDate), 'HH:mm');
            });

        var defaultHeading = $(".umb-overlay__title").html();
        $(".umb-overlay__title").html("<span class=\"icon-lock large\"></span>" + defaultHeading);

        pageguardResource.CheckCurrentAccessDetails(nodeId).then(function (data) {
            $scope.latestUsername = data[0].Username;
            $scope.latestDateRecord = $filter('date')(data[0].DateTimeRecord, 'dd/MM/yyyy');
            $scope.latestDateTimeRecord = $filter('date')(data[0].DateTimeRecord, 'HH:mm');
        });

        pageguardResource.GetLastFiveActions(nodeId).then(function (data) {
            $scope.list = data;
        });

        this.preview = function (content) {
            // Chromes popup blocker will kick in if a window is opened 
            // outwith the initial scoped request. This trick will fix that.
            var previewWindow = $window.open("preview/?id=" + content.id, 'umbpreview'); // Build the correct path so both /#/ and #/ work.

            var redirect = Umbraco.Sys.ServerVariables.umbracoSettings.umbracoPath + '/preview/?id=' + content.id;
            previewWindow.location.href = redirect;
        }; // it all starts here
    }

    angular.module('umbraco').controller('pageguard.override.controller', ['$scope', '$routeParams', '$filter', 'pageguardResource', 'contentResource', ctrl]);

})();