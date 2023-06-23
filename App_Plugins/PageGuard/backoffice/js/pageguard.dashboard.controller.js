(function () {
    "use strict";

    function ctrl($scope, $routeParams, pageguardResource, userService, notificationsService, localizationService) {

        var vm = this;
        vm.millisToUTCDate = pageguardResource.convertToUTC;

        var nodeId = $routeParams.id;

        userService.getCurrentUser().then(function (user) {

            $scope.currentUserId = user.id;
            $scope.currentUserRole = user.userType;

            pageguardResource.LoadAllMyCheckOuts($scope.currentUserId).then(function (data) {
                $scope.list = data;
            });

            pageguardResource.LoadEveryonesCheckouts($scope.currentUserId).then(function (data) {
                $scope.everyone = data;
            });
        });

        $scope.PerformUnlock = function PerformUnlock(item) {
            var nodeId = item.NodeId;

            userService.getCurrentUser().then(function (user) {
                $scope.currentUserId = user.id;

                pageguardResource.OverridePage(nodeId, $scope.currentUserId).then(function (unlock) {
                    $scope.unlockSuccess = unlock;

                    if ($scope.unlockSuccess === "true") {

                        notificationsService.remove(0);

                        var message = "The page has been unlocked successfully";
                        localizationService.localize("pageGuardDashboard_successmsg").then(function (value) {
                            message = value;
                        });

                        notificationsService.success(message);

                        pageguardResource.LoadEveryonesCheckouts($scope.currentUserId).then(function (data) {
                            $scope.everyone = data;
                        });
                    }
                });
            });
        };

        $scope.PerformCheckin = function PerformCheckin(item) {
            var nodeId = item.NodeId;

            userService.getCurrentUser().then(function (user) {
                $scope.currentUserId = user.id;

                pageguardResource.CheckinPage(nodeId, $scope.currentUserId).then(function (checkin) {
                    $scope.checkinSuccess = checkin;

                    if ($scope.checkinSuccess === "true") {
                        notificationsService.remove(0);
                        notificationsService.success("Your page has checked in successfully");

                        pageguardResource.LoadAllMyCheckOuts($scope.currentUserId).then(function (data) {
                            $scope.list = data;
                        });
                    }
                });
            });
        };

        $scope.DaysCheckedOut = function (firstDate) {
            var date2 = new Date();
            var date1 = new Date(firstDate);
            var timeDiff = Math.abs(date2.getTime() - date1.getTime());

            var difference_ms = timeDiff;
            difference_ms = difference_ms / 1000;
            var seconds = Math.floor(difference_ms % 60);
            difference_ms = difference_ms / 60;
            var minutes = Math.floor(difference_ms % 60);
            difference_ms = difference_ms / 60;
            var hours = Math.floor(difference_ms % 24);
            var days = Math.floor(difference_ms / 24);

            var format = days + ' Days | ' + hours + ' Hours | ' + minutes + ' Mins';

            return format;
        };

    }

    angular.module('umbraco').controller('pageguard.dashboard.controller', ['$scope', '$routeParams', 'pageguardResource', 'userService', 'notificationsService','localizationService', ctrl]);

})();