var AngularTestApp = angular.module('AngularTestApp', []);

AngularTestApp.controller('LandingPageController', LandingPageController);

var configFunction = function ($routeProvider) {
    $routeProvider.
        when('/routeOne', {
            templateUrl: 'AngularRoutesDemo/one',
        })
        .when('/routeTwo', {
            templateUrl: 'AngularRoutesDemo/two'
        })
        .when('/routeThree', {
            templateUrl: 'AngularRoutesDemo/three'
        });
}
configFunction.$inject = ['$routeProvider'];

AngularTestApp.config(configFunction);
