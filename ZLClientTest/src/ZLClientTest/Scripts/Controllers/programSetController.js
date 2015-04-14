//(function () {
//    'use strict';

//    angular
//        .module('programSetApp')
//        .controller('programSetController', programSetController);

//    programSetController.$inject = ['$scope', 'ProgramSet']; 

//    function programSetController($scope, ProgramSets) {
//    	$scope.title = 'programSetController';
//	    $scope.ProgramSets = ProgramSets.query();
//    }
//})();


(function () {
	'use strict';

	angular
        .module('programSetsApp')
        .controller('programSetsController', programSetsController);

	programSetsController.$inject = ['$scope', 'ProgramSets'];

	function programSetsController($scope, ProgramSets) {
		$scope.programSets = ProgramSets.query();
	}
})();