angular.module('boardingSchoolApp', [])
.controller('MainController', ['$scope', '$http', function($scope, $http) {
    $scope.schools = [];
    $scope.loading = true;
    $scope.error = null;

    // First, get the API URL from the config endpoint
    $http.get('/api/config').then(function(response) {
        const apiUrl = response.data.apiUrl;
        
        $http.get(apiUrl + '/boardingschools').then(function(res) {
            $scope.schools = res.data;
            $scope.loading = false;
        }, function(err) {
            console.error('Error fetching schools:', err);
            $scope.error = 'Failed to load boarding schools. Make sure the API is running.';
            $scope.loading = false;
        });
    }, function(err) {
        $scope.error = 'Failed to load configuration.';
        $scope.loading = false;
    });
}]);
