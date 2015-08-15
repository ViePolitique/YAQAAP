var yaqaap = angular.module("yaqaap", ["ngRoute", "ngSanitize", "ngAnimate"]);

yaqaap.config([
    "$routeProvider", "$locationProvider",
    function ($routeProvider, $locationProvider, serviceStackRestConfigProvider) {
        $routeProvider
            .when("/", {
                templateUrl: "/Content/_Search.html",
                controller: "searchController"
            })
            .when("/Ask", {
                templateUrl: "/Content/_Ask.html",
                controller: "askController"
            })
            .when("/NotFound", {
                templateUrl: "/Content/_NotFound.html"
            })
            .otherwise({ redirectTo: "/NotFound" });

        $locationProvider.html5Mode(true);
    }
]);



yaqaap.controller("yaqqapController", ["$scope", "$route", "$routeParams", "$location", yaqqapController]);
yaqaap.controller("askController", ["$scope", "$http", askController]);
yaqaap.controller("searchController", ["$scope", "$http", searchController]);


function yaqqapController($scope, $route, $routeParams, $location) {

    // Using 'Controller As' syntax, so we assign this to the vm variable (for viewmodel).
    var vm = this;

    vm.$route = $route;
    vm.$location = $location;
    vm.$routeParams = $routeParams;
};

function searchController($scope, $http) {

    // Using 'Controller As' syntax, so we assign this to the vm variable (for viewmodel).
    var vm = this;

    $scope.searchChange = function () {

        if ($scope.search) {
            $http.get("/api/search/" + $scope.search)
                .success(function (data, status, headers, config) {
                    // do what you do
                    $scope.questions = data.Questions;
                })
                .error(function (data, status, headers, config) {

                    $scope.questions = undefined;

                    if (status === 401) {
                        // handle redirecting to the login page
                    } else if (status === 500 || status === 503) {
                        // retry the call and eventually handle too many failures
                    } else if (data != undefined) {
                        if (
                            data.responseStatus != null &&
                            data.responseStatus.errors != null &&
                            data.responseStatus.errors.length > 0)
                            // handle validation error
                        {
                            var errors = data.responseStatus.errors;
                        } else {
                            // handle non validation error
                            var errorCode = data.responseStatus.errorCode;
                            var message = data.responseStatus.message;
                        }
                    }
                });




            //.error(function (response, headers, config) {
            //    // handle non validation error
            //    //errorCode = response.error.errorCode;
            //    //message = response.error.message;
            //    $scope.questions = undefined;
            //})
            //.validation(function (response, headers, config) {
            //    // handle validation error
            //    //errors = response.validationErrors;
            //    $scope.questions = undefined;
            //});
        } else {
            $scope.questions = undefined;
        }

    };
};


function askController($scope, $http) {
    // Using 'Controller As' syntax, so we assign this to the vm variable (for viewmodel).
    var vm = this;

    $scope.markdownPreview = "";

    $scope.updateMarkdownPreview = function (data) {
        $scope.markdownPreview = markdown.toHTML(data);
    };

    $scope.ask = function () {
        if ($scope.questionTitle && $scope.questionDetail && $scope.questionTags) {

            var askData = {
                title: $scope.questionTitle,
                detail: $scope.questionDetail,
                tags: $scope.questionTags.split(",")
            };

            $http.get("/api/ask", askData)
                .success(function (data, status, headers, config) {
                    // do what you do
                    $scope.askResult = data.Result;
                })
                .error(function (data, status, headers, config) {

                    $scope.askResult = undefined;

                    if (status === 401) {
                        // handle redirecting to the login page
                    } else if (status === 500 || status === 503) {
                        // retry the call and eventually handle too many failures
                    } else if (data != undefined) {
                        if (
                            data.responseStatus != null &&
                                data.responseStatus.errors != null &&
                                data.responseStatus.errors.length > 0)
                            // handle validation error
                        {
                            var errors = data.responseStatus.errors;
                        } else {
                            // handle non validation error
                            var errorCode = data.responseStatus.errorCode;
                            var message = data.responseStatus.message;
                        }
                    }
                });
        }

    };
};