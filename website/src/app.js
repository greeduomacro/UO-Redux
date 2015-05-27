angular.module("uor", [
	"ngRoute",
	"uor.home",
	"uor.about",
    "uor.contact"
]).config(["$routeProvider", function($routeProvider) {
	$routeProvider.otherwise({redirectTo: "/home"});
}]);