(function () {
    "use strict";

    function ctrl($scope, $routeParams, pageguardResource, userService, notificationsService, localizationService) {

        var vm = this;

        vm.CheckInPage = PerformCheckin;

        var nodeId = $routeParams.id;

        $scope.nodeId = nodeId;

        var defaultHeading = $(".umb-overlay__title").html();
        $(".umb-overlay__title").html("<span class=\"icon-unlocked large\"></span>" + defaultHeading);

        function PerformCheckin() {
            var nodeId = $routeParams.id;

            userService.getCurrentUser().then(function (user) {
                $scope.currentUserId = user.id;

                pageguardResource.CheckinPage(nodeId, $scope.currentUserId).then(function (checkin) {
                    $scope.checkinSuccess = checkin;

                    if ($scope.checkinSuccess === "true") {
                        notificationsService.remove(0);
                        var message = "Your page has checked in successfully";
                        localizationService.localize("pageGuardCheckIn_successmsg").then(function (value) {
                            message = value;
                        });
                        notificationsService.success(message);

                        location.reload();
                    }
                });
            });
        }
    }

    angular.module('umbraco').controller('pageguard.checkin.controller', ['$scope', '$routeParams', 'pageguardResource', 'userService', 'notificationsService', 'localizationService', ctrl]);

})();