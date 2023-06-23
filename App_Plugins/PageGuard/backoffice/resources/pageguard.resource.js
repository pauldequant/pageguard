angular.module('umbraco.resources').factory('pageguardResource', function ($http, umbRequestHelper) {
    return {
        NotifyPage: function (nodeId, userId) {
            return umbRequestHelper.resourcePromise($http.post("backoffice/PageGuardApi/PageGuardApi/CheckOutPage?nodeId=" + nodeId + "&userId=" + userId), "Check Out Failed");
        },
        CheckoutPage: function (nodeId, userId) {
            return umbRequestHelper.resourcePromise($http.post("backoffice/PageGuardApi/PageGuardApi/CheckOutPage?nodeId=" + nodeId + "&userId=" + userId), "Check In Failed");
        },
        CheckinPage: function (nodeId, userId) {
            return umbRequestHelper.resourcePromise($http.post("backoffice/PageGuardApi/PageGuardApi/CheckInPage?nodeId=" + nodeId + "&userId=" + userId), "Check In Failed");
        },
        OverridePage: function (nodeId, userId) {
            return umbRequestHelper.resourcePromise($http.post("backoffice/PageGuardApi/PageGuardApi/OverridePage?nodeId=" + nodeId + "&userId=" + userId), "Override Failed");
        },
        CheckPageStatus: function (nodeId, userId) {
            return umbRequestHelper.resourcePromise($http.get("backoffice/PageGuardApi/PageGuardApi/CheckPageStatus?nodeId=" + nodeId + "&userId=" + userId), "Failed Status Check");
        },
        GetLatestPageStatus: function (nodeId) {
            return umbRequestHelper.resourcePromise($http.get("backoffice/PageGuardApi/PageGuardApi/GetLatestPageStatus?nodeId=" + nodeId), "Failed Latest Status Check");
        },
        GetLastFiveActions: function (nodeId) {
            return umbRequestHelper.resourcePromise($http.get("backoffice/PageGuardApi/PageGuardApi/GetLastFiveActions?nodeId=" + nodeId), "Failed To Get Last Five");
        },
        CheckCurrentAccessDetails: function (nodeId) {
            return umbRequestHelper.resourcePromise($http.get("backoffice/PageGuardApi/PageGuardApi/CheckCurrentAccessDetails?nodeId=" + nodeId), "Failed To Check Latest Access Details");
        },
        LoadAdminListForNotifications: function (nodeId) {
            return umbRequestHelper.resourcePromise($http.get("backoffice/PageGuardApi/PageGuardApi/LoadAdminListForNotifications?nodeId=" + nodeId), "Failed To Load Admin Details");
        },
        LoadAllMyCheckOuts: function (userId) {
            return umbRequestHelper.resourcePromise($http.get("backoffice/PageGuardApi/PageGuardApi/LoadAllMyCheckOuts?userId=" + userId), "Failed To Load My Checkout Details");
        },
        LoadEveryonesCheckouts: function (userId) {
            return umbRequestHelper.resourcePromise($http.get("backoffice/PageGuardApi/PageGuardApi/LoadEveryonesCheckouts?userId=" + userId), "Failed To Load All Checkouts");
        },
        Notify: function (nodeId, userId, myUserId) {
            return umbRequestHelper.resourcePromise($http.post("backoffice/PageGuardApi/PageGuardApi/Notify?nodeId=" + nodeId + "&userId=" + userId + "&myUserId=" + myUserId), "Failed To Notify User");
        },
        getNameInitials: function getNameInitials(name) {
            if (name) {
                var names = name.split(' '), initials = names[0].substring(0, 1);
                if (names.length > 1) {
                    initials += names[names.length - 1].substring(0, 1);
                }
                return initials.toUpperCase();
            }
            return null;
        },
        convertToUTC: function convertToUTC(date) {
            if (date) {

                var newDate = new Date(date);
                var _utc = new Date(newDate.getUTCFullYear(), newDate.getUTCMonth(), newDate.getUTCDate(), newDate.getUTCHours(), newDate.getUTCMinutes(), newDate.getUTCSeconds());

                return new Date(_utc);
            }
        }
    };
});