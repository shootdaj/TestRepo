//(function () {
//    'use strict';

//    var programSetService = angular.module('programSetService', ['ngResource']);

//	programSetService.factory('ProgramSets', [
//		'$resource',
//		function($resource) {
//			return $resource('/api/ProgramSet/', {}, {
//				query: { method: 'GET', params: {}, isArray: true }
//			});
//		}
//	]);

//	//angular
//	//	.module('programSetApp')
//	//	.factory('programSetService', [
//	//		'$resource',
//	//		function($resource) {
//	//			return $resource('/api/ProgramSet/', {}, {
//	//				query: { method: 'GET', params: {}, isArray: true }
//	//			});
//	//		}
//	//	]);

//	//programSetService.$inject = ['$http'];

//	//function programSetService() {
//	//    var service = {
//	//        getData: getData
//	//    };

//	//    return service;

//	//    function getData() { }
//	//}
//})();


(function () {
	'use strict';

	var programSetsServices = angular.module('programSetsServices', ['ngResource']);

	programSetsServices.factory('ProgramSets', ['$resource',
      function ($resource) {
      	return $resource('/api/programSets/', {}, {
      		query: { method: 'GET', params: {}, isArray: true }
      	});
      }]);


})();