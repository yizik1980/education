angular.module("boardingSchoolApp").controller("MainController", [
  "$scope",
  "SchoolService",
  "AppConfig",
  function ($scope, SchoolService, AppConfig) {
    $scope.schools = [];
    $scope.filteredSchools = [];
    $scope.loading = true;
    $scope.error = null;
    $scope.searchQuery = "";
    $scope.citySuggestions = [];
    $scope.totalStudents = 0;
    $scope.globalAverageAges = 0;

    const updateCitySuggestions = () => {
      $scope.citySuggestions = $scope.schools
        .map((s) => s.city)
        .filter((v, i, a) => v && a.indexOf(v) === i)
        .sort();
    };

    $scope.fetchSchools = async function () {
      $scope.loading = true;
      $scope.error = null;

      try {
        $scope.schools = await SchoolService.getBoardingSchools();
        $scope.filteredSchools = Array.isArray($scope.schools) ? [...$scope.schools] : [];
        updateCitySuggestions();
        $scope.updateStatistics();
      } catch (err) {
        console.error("Error in controller:", err);

        if (err.status === -1) {
          $scope.error = AppConfig.LABELS.CONNECTION_ERROR + AppConfig.API_BASE_URL;
        } else if (err.status >= 500) {
          $scope.error = AppConfig.LABELS.SERVER_ERROR;
        } else if (err.status === 404) {
          $scope.error = AppConfig.LABELS.NOT_FOUND;
        } else {
          $scope.error = err.message || AppConfig.LABELS.UNKNOWN_ERROR;
        }
      } finally {
        $scope.loading = false;
        $scope.$apply();
      }
    };

    $scope.filterByCity = function () {
      if (!$scope.searchQuery) {
        $scope.filteredSchools = [...$scope.schools];
      } else {
        const query = $scope.searchQuery.toLowerCase();
        $scope.filteredSchools = $scope.schools.filter(
          (school) => school.city && school.city.toLowerCase().includes(query),
        );
      }
      $scope.updateStatistics();
    };

    $scope.updateStatistics = function() {
      $scope.calculateTotalStudents();
      $scope.calculateWeightedAverageAge();
    };

    $scope.calculateTotalStudents = function () {
      $scope.totalStudents = $scope.filteredSchools.reduce(
        (acc, school) => acc + (school.activeStudentCount || 0),
        0,
      );
    };

    $scope.calculateWeightedAverageAge = function () {
      if (!$scope.filteredSchools.length) {
        $scope.globalAverageAges = 0;
        return;
      }
      
      let totalWeightedAge = 0;
      let totalStudents = 0;
      
      $scope.filteredSchools.forEach(school => {
        const count = school.activeStudentCount || 0;
        const age = school.avrageAges || 0;
        if (count > 0) {
          totalWeightedAge += (age * count);
          totalStudents += count;
        }
      });
      
      $scope.globalAverageAges = totalStudents > 0 ? (totalWeightedAge / totalStudents) : 0;
    };

    $scope.retry = async function () {
      await $scope.fetchSchools();
    };

    // Initial fetch
    $scope.fetchSchools();
  },
]);
