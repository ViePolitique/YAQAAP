var yaqaap = angular.module("yaqaap", ["ngRoute", "ngSanitize", "ngAnimate", "ngMessages", "ngPassword", "angular-humanize-duration", "ngCookies"]);

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
            .when("/Answers/:questionId/:title", {
                templateUrl: "/Content/_Answers.html",
                controller: "answersController"
            })
            .when("/SignIn", {
                templateUrl: "/Content/_SignIn.html",
                controller: "signInController"
            })
            .when("/Disconnect", {
                templateUrl: "/Content/_SignIn.html",
                controller: "signInController"
            })
            .when("/Register", {
                templateUrl: "/Content/_Register.html",
                controller: "registerController"
            })
            .when("/User/:username", {
                templateUrl: "/Content/_User.html",
                controller: "userController"
            })
            .when("/NotFound", {
                templateUrl: "/Content/_NotFound.html"
            })
            .otherwise({ redirectTo: "/NotFound" });

        $locationProvider.html5Mode(true);
    }
]);

yaqaap.service("$authService", ["$http", "$location", "$cookies", authService]);


yaqaap.controller("yaqaapController", ["$scope", "$route", "$routeParams", "$authService", "$location", yaqaapController]);
yaqaap.controller("signInController", ["$scope", "$http", "$authService", "$location", signInController]);
yaqaap.controller("registerController", ["$scope", "$http", "$authService", "$location", registerController]);
yaqaap.controller("askController", ["$scope", "$http", "$location", askController]);
yaqaap.controller("answersController", ["$scope", "$http", "$route", "$routeParams", "$location", answersController]);
yaqaap.controller("userController", ["$scope", "$http", "$route", "$routeParams", "$location", userController]);
yaqaap.controller("searchController", ["$scope", "$http", "$location", "$authService", searchController]);


function authService($http, $location, $cookies) {

    this.isAuth = function () {
        return this.userId != undefined;
    }

    this.getUserId = function () {

        if (this.userId == undefined)
            this.userId = $cookies.get('userId');

        return this.userId;
    };

    this.setUserId = function (userId) {
        this.userId = userId;
        $cookies.put('userId', userId);
    };

    this.signIn = function(username, password) {
        var authData = {
            username: username,
            password: password
        };

        var me = this;

        $http.post("/auth", authData)
                      .success(function (data, status, headers, config) {
                          // do what you do
                          // $scope.askResult = data.Result;
                          me.setUserId(username);
                          $location.path("/");
                      })
                      .error(function (data, status, headers, config) {

                          //$scope.askResult = undefined;

                          if (status === 401) {
                              // handle redirecting to the login page
                              //$location.path("/SignIn");
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

    this.getUserId();
};

function yaqaapController($scope, $route, $routeParams, $authService, $location) {

    // Using 'Controller As' syntax, so we assign this to the vm variable (for viewmodel).
    var vm = this;

    vm.$route = $route;
    vm.$location = $location;
    vm.$routeParams = $routeParams;
    vm.$authService = $authService;
};

function signInController($scope, $http, $authService, $location) {

    // Using 'Controller As' syntax, so we assign this to the vm variable (for viewmodel).
    var vm = this;

    $scope.username = undefined;
    $scope.password = undefined;

    if ($authService.getUserId() != undefined) {
        $authService.setUserId(undefined);
    }

    $scope.signin = function () {
        try {
            $scope.signing = true;

            if ($authService.getUserId() != undefined) {
                $location.path("/");
                return;
            }

            if ($scope.username != undefined && $scope.username !== '') {

                $authService.signIn($scope.username, $scope.password);
            }
        } finally {
            $scope.signing = false;
        }
    };

};

function registerController($scope, $http, $authService, $location) {

    // Using 'Controller As' syntax, so we assign this to the vm variable (for viewmodel).
    var vm = this;

    if ($authService.getUserId() != undefined) {
        $location.path("/");
        return;
    }

    $scope.register = function () {

        try {
            $scope.registerResult = undefined;
            $scope.registering = true;

            if ($authService.getUserId() != undefined) {
                $location.path("/");
                return;
            }

            var registerData = {
                email: $scope.email,
                username: $scope.username,
                password: $scope.password
            };

            $http.post("/api/register", registerData)
                          .success(function (data, status, headers, config) {
                              // do what you do
                              // $scope.registerResult = data.Result;
                              $authService.signIn($scope.username, $scope.password);
                          })
                          .error(function (data, status, headers, config) {

                              if (status === 401) {
                                  // handle redirecting to the login page
                                  $location.path("/SignIn");
                              } else if (status === 500 || status === 503) {
                                  // retry the call and eventually handle too many failures
                              } else if (data != undefined) {
                                  if (data.ResponseStatus != null &&
                                          data.ResponseStatus.Errors != null &&
                                          data.ResponseStatus.Errors.length > 0)
                                      // handle validation error
                                  {
                                      var errors = data.ResponseStatus.Errors;
                                      $scope.registerResult = data.ResponseStatus.Message;
                                  } else {
                                      // handle non validation error
                                      var errorCode = data.ResponseStatus.ErrorCode;
                                      var message = data.ResponseStatus.Message;
                                      $scope.registerResult = message;
                                  }
                              }
                          });
        } finally {
            $scope.registering = false;
        }
    };

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

    $scope.top = function () {
        $http.get("/api/top")
                  .success(function (data, status, headers, config) {
                      // do what you do
                      for (var i = 0; i < data.Questions.length; i++) {
                          data.Questions[i].TitleUri = data.Questions[i].Title.replace(/\s/g, "-");
                      }
                      $scope.topQuestions = data.Questions;

                  })
                  .error(function (data, status, headers, config) {

                      $scope.topQuestions = undefined;

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

    $scope.searchChange = function () {

        if ($scope.search) {
            $http.get("/api/search/" + $scope.search)
                .success(function (data, status, headers, config) {
                    // do what you do
                    for (var i = 0; i < data.Questions.length; i++) {
                        data.Questions[i].TitleUri = data.Questions[i].Title.replace(/\s/g, "-");
                    }
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
        } else {
            this.questions = undefined;
        }

    };

    $scope.top();
};


function askController($scope, $http, $location) {
    // Using 'Controller As' syntax, so we assign this to the vm variable (for viewmodel).
    var vm = this;

    $scope.markdownPreview = "";

    $scope.updateMarkdownPreview = function (data) {
        this.markdownPreview = markdown.toHTML(data);
    };

    $scope.ask = function () {

        try {
            $scope.asking = true;

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
        } finally {
            $scope.asking = false;
        }


    };
};


function userController($scope, $http, $route, $routeParams, $location) {
    // Using 'Controller As' syntax, so we assign this to the vm variable (for viewmodel).
    var vm = this;

    $scope.username = $routeParams.username;

    $scope.getUser = function () {

        $http.get("/api/user/" + $routeParams.username)
                  .success(function (data, status, headers, config) {
                      // do what you do
                      $scope.user = data;
                      $scope.user.Created = new Date() - new Date($scope.user.Created);
                //$scope.user.Created = new Date().getMilliseconds() - new Date($scope.user.Created).getMilliseconds();
            })
                  .error(function (data, status, headers, config) {

                      $scope.topQuestions = undefined;

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

    $scope.getUser();
};

function answersController($scope, $http, $route, $routeParams, $location) {
    // Using 'Controller As' syntax, so we assign this to the vm variable (for viewmodel).
    var vm = this;


    $scope.reload = function () {
        $http.get("/api/answers/" + $routeParams.questionId)
                          .success(function (data, status, headers, config) {
                              // do what you do
                              $scope.question = data;
                              $scope.question.Detail = markdown.toHTML($scope.question.Detail);
                              for (var i = 0; i < $scope.question.Answers.length; i++) {
                                  $scope.question.Answers[i].Content = markdown.toHTML($scope.question.Answers[i].Content);
                              }
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
    };

    $scope.reload();
    $scope.markdownPreview = "";

    $scope.updateMarkdownPreview = function (data) {
        this.markdownPreview = markdown.toHTML(data);
    };

    $scope.questionVotes = function() {
        return $scope.question.Votes;
    };

    $scope.love = function (questionId) {

        var loveData = {
            questionId: questionId
        };

        $http.post("/api/love", loveData)
                       .success(function (data, status, headers, config) {
                           // do what you do
                           //$scope.voteResult = data.Result;

                           if (data.Result === 'OK') {
                               $scope.question.Love = !$scope.question.Love;
                           }
                       })
                       .error(function (data, status, headers, config) {

                           //$scope.voteResult = undefined;

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

    $scope.vote = function (questionId, ownerId, target, kind) {

        var voteData = {
            questionId: questionId,
            ownerId: ownerId,
            voteTarget: target,
            voteKind: kind
        };

        $http.post("/api/vote", voteData)
                       .success(function (data, status, headers, config) {
                           // do what you do
                           //$scope.voteResult = data.Result;

                           if (data.Result === 'OK' && target === 'question') {
                               $scope.question.Votes = data.VoteValue;
                               $scope.question.VoteKind = data.VoteKind;
                           }
                           else if (data.Result === 'OK' && target === 'answer') {
                               var answer = $scope.question.Answers.filter(function(obj) {
                                   return obj.Owner.Id === ownerId;
                               })[0];
                               
                               answer.Votes = data.VoteValue;
                               answer.VoteKind = data.VoteKind;
                           }
                       })
                       .error(function (data, status, headers, config) {

                           //$scope.voteResult = undefined;

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

    $scope.answer = function () {

        try {
            $scope.anwsering = true;

            if (!this.answerContent) {
                this.answerResult = "NeedContent";
                return;
            }


            var answerData = {
                questionId: $routeParams.questionId,
                content: this.answerContent
            };

            var me = this;

            $http.post("/api/answer", answerData)
                .success(function (data, status, headers, config) {
                    // do what you do
                    $scope.answerResult = data.Result;
                    me.answerContent = '';
                    me.updateMarkdownPreview('');
                    $scope.reload();
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
        } finally {
            $scope.anwsering = false;
        }

    };
};




/////////////////////////////////////////////////////////////////////


var compareTo = function () {
    return {
        require: "ngModel",
        scope: {
            otherModelValue: "=compareTo"
        },
        link: function (scope, element, attributes, ngModel) {

            ngModel.$validators.compareTo = function (modelValue) {
                return modelValue == scope.otherModelValue;
            };

            scope.$watch("otherModelValue", function () {
                ngModel.$validate();
            });
        }
    };
};

var ngFocus = function() {
    var FOCUS_CLASS = "ng-focused";
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function(scope, element, attrs, ctrl) {
            ctrl.$focused = false;
            element.bind('focus', function(evt) {
                element.addClass(FOCUS_CLASS);
                scope.$apply(function() { ctrl.$focused = true; });
            }).bind('blur', function(evt) {
                element.removeClass(FOCUS_CLASS);
                scope.$apply(function() { ctrl.$focused = false; });
            });
        }
    }
};

yaqaap.directive("compareTo", compareTo);
yaqaap.directive('ngFocus', ngFocus);