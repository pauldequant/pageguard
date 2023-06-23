(function () {
    "use strict";

    function ctrl($scope, $routeParams, pageguardResource, userService, notificationsService, localizationService) {

        var vm = this;

        vm.CheckInPage = PerformCheckin;

        $scope.showAlert = false;

        if ($routeParams.tree === "content") {
            DeterminePageStatusAlert();
        }

        setInterval(DeterminePageStatusAlert, 3000);

        function DeterminePageStatusAlert() {

            var nodeId = $routeParams.id;

            userService.getCurrentUser().then(function (user) {
                $scope.currentUserId = user.id;
                $scope.currentUserRole = user.userType;

                pageguardResource.CheckPageStatus(nodeId, $scope.currentUserId).then(function (pgStatus) {
                    $scope.showPageGuardPrompt = pgStatus;

                    pageguardResource.GetLatestPageStatus(nodeId).then(function (latestStatus) {
                        $scope.latestStatus = latestStatus;

                        if ($scope.showPageGuardPrompt === "false") {
                            $scope.showAlert = true;
                        }
                    });
                });
            });
        }

        function PerformCheckin() {
            var nodeId = $routeParams.id;

            userService.getCurrentUser().then(function (user) {
                $scope.currentUserId = user.id;

                pageguardResource.CheckinPage(nodeId, $scope.currentUserId).then(function (checkin) {
                    $scope.checkinSuccess = checkin;

                    if ($scope.checkinSuccess === "true") {

                        notificationsService.remove(0);

                        var message = "Your page has checked in successfully";
                        localizationService.localize("pageGuardPageStatus_successmsg").then(function (value) {
                            message = value;
                        });

                        notificationsService.success(message);

                        location.reload();
                    }
                });
            });
        }
    }

    angular.module('umbraco').controller('pageguard.status.controller', ['$scope', '$routeParams', 'pageguardResource', 'userService', 'notificationsService', 'localizationService', ctrl]);

})();