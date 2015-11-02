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
            .when("/Answers/:questionId", {
                templateUrl: "/Content/_Answers.html",
                controller: "answersController"
            })
            .when("/SignIn", {
                templateUrl: "/Content/_SignIn.html",
                controller: "signInController"
            })
            .when("/Register", {
                templateUrl: "/Content/_Register.html",
                controller: "registerController"
            })
            .when("/NotFound", {
                templateUrl: "/Content/_NotFound.html"
            })
            .otherwise({ redirectTo: "/NotFound" });

        $locationProvider.html5Mode(true);
    }
]);

yaqaap.service("$authService", authService);


yaqaap.controller("yaqqapController", ["$scope", "$route", "$routeParams", "$location", yaqqapController]);
yaqaap.controller("signInController", ["$scope", "$http", "$authService", "$location", signInController]);
yaqaap.controller("registerController", ["$scope", "$http", "$authService", registerController]);
yaqaap.controller("askController", ["$scope", "$http", "$location", askController]);
yaqaap.controller("answersController", ["$scope", "$http", "$route", "$routeParams", "$location", answersController]);
yaqaap.controller("searchController", ["$scope", "$http", "$location", "$authService", searchController]);


function authService() {

    var userId;

    this.getUserId = function () {
        return this.userId;
    };

    this.setUserId = function (userId) {
        this.userId = userId;
    };
};

function yaqqapController($scope, $route, $routeParams, $location) {

    // Using 'Controller As' syntax, so we assign this to the vm variable (for viewmodel).
    var vm = this;

    vm.$route = $route;
    vm.$location = $location;
    vm.$routeParams = $routeParams;
};

function signInController($scope, $http, $authService, $location) {

    // Using 'Controller As' syntax, so we assign this to the vm variable (for viewmodel).
    var vm = this;

    $scope.username = undefined;
    $scope.password = undefined;

    $scope.signin = function () {

        if ($scope.username != undefined) {
            $authService.setUserId($scope.username);

            $location.path("/");
        }
    };

};

function registerController($scope, $http, $authService) {

    // Using 'Controller As' syntax, so we assign this to the vm variable (for viewmodel).
    var vm = this;
};

function searchController($scope, $http, $location, $authService) {

    // Using 'Controller As' syntax, so we assign this to the vm variable (for viewmodel).
    var vm = this;

    $scope.ask = function () {

        if ($authService.getUserId() == undefined) {
            $location.path("/SignIn");
            return;
        }

        $location.path("/Ask");

    };

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
                        $location.path("/SignIn");
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
            this.questions = undefined;
        }

    };
};


function askController($scope, $http, $location) {
    // Using 'Controller As' syntax, so we assign this to the vm variable (for viewmodel).
    var vm = this;

    $scope.markdownPreview = "";

    $scope.updateMarkdownPreview = function (data) {
        this.markdownPreview = markdown.toHTML(data);
    };

    $scope.ask = function () {

        if (!this.questionTitle) {
            this.askResult = "NeedTitle";
            return;
        }

        if (!this.questionDetail) {
            this.askResult = "NeedDetail";
            return;
        }

        if (!this.questionTags) {
            this.askResult = "NeedTags";
            return;
        }

        if (this.questionTitle && this.questionDetail && this.questionTags) {

            var askData = {
                title: this.questionTitle,
                detail: this.questionDetail,
                tags: this.questionTags.split(",")
            };

            $http.post("/api/ask", askData)
                .success(function (data, status, headers, config) {
                    // do what you do
                    $scope.askResult = data.Result;
                })
                .error(function (data, status, headers, config) {

                    $scope.askResult = undefined;

                    if (status === 401) {
                        // handle redirecting to the login page
                        $location.path("/SignIn");
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


function answersController($scope, $http, $route, $routeParams, $location) {
    // Using 'Controller As' syntax, so we assign this to the vm variable (for viewmodel).
    var vm = this;


    $http.get("/api/answers/" + $routeParams.questionId)
                  .success(function (data, status, headers, config) {
                      // do what you do
                      $scope.question = data;
                      $scope.question.Detail = markdown.toHTML($scope.question.Detail);
                  })
                  .error(function (data, status, headers, config) {

                      $scope.question = undefined;
                      //$scope.answers = undefined;

                      if (status === 401) {
                          // handle redirecting to the login page
                          $location.path("/SignIn");
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


    $scope.markdownPreview = "";

    $scope.updateMarkdownPreview = function (data) {
        this.markdownPreview = markdown.toHTML(data);
    };

    $scope.answer = function () {

        if (!this.answerContent) {
            this.answerResult = "NeedContent";
            return;
        }


        var answerData = {
            questionId: $routeParams.questionId,
            content: this.answerContent
        };


        $http.post("/api/answer", answerData)
            .success(function (data, status, headers, config) {
                // do what you do
                $scope.answerResult = data.Result;
            })
            .error(function (data, status, headers, config) {

                $scope.answerResult = undefined;

                if (status === 401) {
                    // handle redirecting to the login page
                    $location.path("/SignIn");
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


    };
};