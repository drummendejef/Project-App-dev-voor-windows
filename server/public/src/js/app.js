//stijn van hulle controllers
var subdomain='';
var app = angular.module('app',['ngMaterial','ngRoute','ngResource','angular-md5'])
    .directive('onFinishRender', function ($timeout) {
        return {
            restrict: 'A',
            link: function (scope, element, attr)
            {
                if (scope.$last === true) {
                    scope.$evalAsync(attr.onFinishRender);
                }
            }
        }
    });
app.factory('$socket', function(){
    //return io();
    return io.connect('http://37.59.114.49');
});
app.config(['$routeProvider', '$locationProvider', function($routeProvider, $locationProvider) {
    $routeProvider
        .when('/', {
            templateUrl: '/partials/index',
            reloadOnSearch: false
        })
        .otherwise({
            redirectTo: '/404'
        });


    $locationProvider.html5Mode(true);
}]);
function getApp(){
    return app;
}
