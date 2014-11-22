var mainApp = angular.module('mainApp', ['cardsControllers']);

var cardsControllers = angular.module('cardsControllers', []);

cardsControllers.controller('cardsListController', [
    '$scope', function($scope) {
        $scope.cards = [{ id: 1 }, { id: 2 }];
    }
]);

