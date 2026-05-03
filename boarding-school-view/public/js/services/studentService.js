angular.module('boardingSchoolApp')
.service('StudentService', ['$http', 'AppConfig', function($http, AppConfig) {

    this.getStudentsByPlace = async function(placeId) {
        const response = await $http.get(`${AppConfig.API_BASE_URL}/students/place/${placeId}`);
        return response.data;
    };

    this.addStudent = async function(student) {
        const response = await $http.post(`${AppConfig.API_BASE_URL}/students`, student);
        return response.data;
    };

}]);
