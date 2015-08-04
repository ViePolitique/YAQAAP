var yaqaap = angular.module("yaqaap", ["ngSanitize", "ngAnimate"]);
yaqaap.controller("yaqqapController", ["$scope", yaqqapController]);
yaqaap.controller("askController", ["$scope", askController]);


function yaqqapController($scope) {

    // Using 'Controller As' syntax, so we assign this to the vm variable (for viewmodel).
    var vm = this;

    vm.partialViews = new Array();
    vm.partialViews["search"] = { url: "/Content/_Search.html" };
    vm.partialViews["ask"] = { url: "/Content/_Ask.html" };

    $scope.switchView = function (route) {
        $scope.partialView = vm.partialViews[route];
    };

    $scope.switchView("search");
};

function askController($scope) {
    // Using 'Controller As' syntax, so we assign this to the vm variable (for viewmodel).
    var vm = this;

    $scope.markdownPreview = "";

    $scope.updateMarkdownPreview = function (data) {
        $scope.markdownPreview = markdown.toHTML(data);
    };
};