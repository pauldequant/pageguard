(function () {
    "use strict";

    function ctrl($scope, $routeParams, $q, pageguardResource, userService, notificationsService, overlayService, localizationService) {

        var vm = this;

        vm.DisplayOverride = DisplayOverride;
        vm.DisplayNotify = DisplayNotify;
        vm.DisplayCheckout = DisplayCheckout;
        vm.getNameInitials = pageguardResource.getNameInitials;

        if ($routeParams.tree === "content") {
            DeterminePageStatus();
        }

        var nodeId = $routeParams.id;

        pageguardResource.GetLastFiveActions(nodeId).then(function (data) {
            $scope.list = data;
        });

        function DeterminePageStatus() {

            var nodeId = $routeParams.id;

            userService.getCurrentUser().then(function (user) {
                $scope.currentUserId = user.id;
                $scope.currentUserRole = user.userGroups;

                pageguardResource.CheckPageStatus(nodeId, $scope.currentUserId).then(function (pgStatus) {
                    $scope.showPageGuardPrompt = pgStatus;

                    pageguardResource.GetLatestPageStatus(nodeId).then(function (latestStatus) {
                        $scope.latestStatus = latestStatus;

                        if ($scope.showPageGuardPrompt === true) {
                            if ($scope.latestStatus === 1 && $scope.currentUserRole.indexOf("admin") !== -1 && $routeParams.create !== "true") {
                                /// CHECK OUT - ADMIN
                                vm.DisplayOverride($scope.control); /// SHOW ADMIN OVERRIDE OPTIONS
                            } else if ($scope.latestStatus === 1 && $scope.currentUserRole.indexOf("admin") == -1 && $routeParams.create !== "true") {
                                /// CHECK OUT - USER
                                vm.DisplayNotify($scope.control); /// SHOW NOTIFY ADMIN OPITONS
                            }
                            else {
                                /// CHECK OUT
                                if ($routeParams.create !== "create") {
                                    vm.DisplayCheckout(); /// SHOW CHECKOUT FOR EDITING OPTIONS
                                }
                            }
                        }
                    });
                });
            });
        }

        function DisplayOverride() {

            var submitLabelPromise = localizationService.localize('pageGuardJsController_displayOverrideSubmitButton');
            var closeLabelPromise = localizationService.localize('pageGuardJsController_displayOverrideExitButton');
            var titleLabelPromise = localizationService.localize('pageGuardJsController_displayOverrideTitle');
            var descriptionLabelPromise = localizationService.localize('pageGuardJsController_displayOverrideDescription');

            var promises = [submitLabelPromise, closeLabelPromise, titleLabelPromise, descriptionLabelPromise];

            $q.all(promises).then(function (results) {
                var submitLabel = results[0];
                var closeLabel = results[1];
                var titleLabel = results[2];
                var descriptionLabel = results[3];

                var options = {
                    view: Umbraco.Sys.ServerVariables.umbracoSettings.appPluginsPath + '/PageGuard/backoffice/views/override.html',
                    title: titleLabel,
                    description: descriptionLabel,
                    disableBackdropClick: true,
                    disableEscKey: true,
                    submitButtonLabel: submitLabel,
                    closeButtonLabel: closeLabel,
                    submit: function (model) {

                        PerformOverride();
                        // what happens when the user presses submit
                        overlayService.close();

                        location.reload();
                    },
                    close: function () {
                        overlayService.close();

                        document.location = '/umbraco#/content';
                    }
                };

                overlayService.open(options);
            });
        }

        function DisplayCheckout() {

            var submitLabelPromise = localizationService.localize('pageGuardJsController_displayCheckoutSubmitButton');
            var closeLabelPromise = localizationService.localize('pageGuardJsController_displayCheckoutExitButton');
            var titleLabelPromise = localizationService.localize('pageGuardJsController_displayCheckoutTitle');
            var descriptionLabelPromise = localizationService.localize('pageGuardJsController_displayCheckoutDescription');

            var promises = [submitLabelPromise, closeLabelPromise, titleLabelPromise, descriptionLabelPromise];

            $q.all(promises).then(function (results) {

                var submitLabel = results[0];
                var closeLabel = results[1];
                var titleLabel = results[2];
                var descriptionLabel = results[3];

                var options = {
                    view: Umbraco.Sys.ServerVariables.umbracoSettings.appPluginsPath + '/PageGuard/backoffice/views/checkout.html',
                    title: titleLabel,
                    description: descriptionLabel,
                    disableBackdropClick: true,
                    disableEscKey: true,
                    submitButtonLabel: submitLabel,
                    closeButtonLabel: closeLabel,
                    submit: function (model) {

                        PerformCheckout();
                        // what happens when the user presses submit
                        overlayService.close();
                    },
                    close: function () {
                        overlayService.close();

                        document.location = '/umbraco#/content';
                    }
                }

                overlayService.open(options);
            });
        }

        function DisplayNotify() {

            var submitLabelPromise = localizationService.localize('pageGuardJsController_displayNotifySubmitButton');
            var closeLabelPromise = localizationService.localize('pageGuardJsController_displayNotifyExitButton');
            var titleLabelPromise = localizationService.localize('pageGuardJsController_displayNotifyTitle');
            var descriptionLabelPromise = localizationService.localize('pageGuardJsController_displayNotifyDescription');

            var promises = [submitLabelPromise, closeLabelPromise, titleLabelPromise, descriptionLabelPromise];

            $q.all(promises).then(function (results) {

                var submitLabel = results[0];
                var closeLabel = results[1];
                var titleLabel = results[2];
                var descriptionLabel = results[3];

                var options = {
                    view: Umbraco.Sys.ServerVariables.umbracoSettings.appPluginsPath + '/PageGuard/backoffice/views/notify.html',
                    title: titleLabel,
                    description: descriptionLabel,
                    disableBackdropClick: true,
                    disableEscKey: true,
                    submitButtonLabel: submitLabel,
                    closeButtonLabel: closeLabel,
                    submit: function (model) {

                        PerformNotify();
                        // what happens when the user presses submit
                        overlayService.close();

                        document.location = '/umbraco#/content';
                    },
                    close: function () {
                        overlayService.close();

                        document.location = '/umbraco#/content';
                    }
                }

                overlayService.open(options);
            }); 
        }

        function PerformOverride() {
            var nodeId = $routeParams.id;

            userService.getCurrentUser().then(function (user) {
                $scope.currentUserId = user.id;

                pageguardResource.OverridePage(nodeId, $scope.currentUserId).then(function (checkout) {
                    $scope.checkoutSuccess = checkout;
                });
            });
        }

        function PerformCheckout() {
            var nodeId = $routeParams.id;

            userService.getCurrentUser().then(function (user) {
                $scope.currentUserId = user.id;

                pageguardResource.CheckoutPage(nodeId, $scope.currentUserId).then(function (checkout) {
                    $scope.checkoutSuccess = checkout;

                    $scope.$emit("DeterminePageStatusAlert", {});
                });
            });
        }

        function PerformNotify() {

            var nodeId = $routeParams.id;

            userService.getCurrentUser().then(function (user) {
                $scope.currentUserId = user.id;

                pageguardResource.CheckCurrentAccessDetails(nodeId).then(function (data) {
                    $scope.pageOwnerId = data[0].UserId;

                    pageguardResource.Notify(nodeId, $scope.pageOwnerId, $scope.currentUserId).then(function (mailstatus) {
                        $scope.mailsent = mailstatus;

                        var successPromise = localizationService.localize('pageGuardJsController_performNotifySuccess');
                        var failurePromise = localizationService.localize('pageGuardJsController_performNotifyError');

                        var promises = [successPromise, failurePromise];

                        $q.all(promises).then(function (results) {
                            var successLabel = results[0];
                            var failureLabel = results[1];

                            if ($scope.mailsent === true) {
                                notificationsService.remove(0);
                                notificationsService.success(successLabel);
                            } else {
                                notificationsService.remove(0);
                                notificationsService.error(failureLabel);
                            }
                        });
                    });
                });
            });
        }
    }

    angular.module('umbraco').controller('pageguard.controller', ['$scope', '$routeParams', '$q', 'pageguardResource', 'userService', 'notificationsService', 'overlayService', 'localizationService', ctrl]);

})();