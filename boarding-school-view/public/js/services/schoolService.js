angular.module('boardingSchoolApp')
.service('SchoolService', ['$http', 'AppConfig', function($http, AppConfig) {
    
    this.getBoardingSchools = async function() {
        try {
            const response = await $http.get(`${AppConfig.API_BASE_URL}/boardingschools`);
            if (Array.isArray(response.data)) {
                return response.data;
            }
            throw new Error(AppConfig.LABELS.FORMAT_ERROR);
        } catch (err) {
            // Propagate the error object for status checks
            throw err;
        }
    };

}]);
