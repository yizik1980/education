angular.module("boardingSchoolApp").controller("MainController", [
  "$scope",
  "SchoolService",
  "StudentService",
  "ToastService",
  "AppConfig",
  function ($scope, SchoolService, StudentService, ToastService, AppConfig) {
    $scope.schools = [];
    $scope.filteredSchools = [];
    $scope.loading = true;
    $scope.error = null;
    $scope.citySuggestions = [];
    $scope.totalStudents = 0;
    $scope.globalAverageAges = 0;
    $scope.cityFilter = "";

    $scope.selectedSchool = null;
    $scope.students = [];
    $scope.loadingStudents = false;
    $scope.studentsError = null;
    $scope.newStudent = {};
    $scope.addingStudent = false;

    $scope.toasts = ToastService.toasts;
    $scope.dismissToast = function (toast) { ToastService.dismiss(toast); };

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

    $scope.filterByCity = function ($event) {
      let searchQuery  = $event && $event.target.value.trim().toLowerCase();
      console.log($event.target, "Search query:", searchQuery);
      if (!searchQuery) {
        $scope.filteredSchools = [...$scope.schools];
      } else {
        $scope.filteredSchools = $scope.schools.filter(
          (school) => school.city && school.city.toLowerCase().includes(searchQuery),
        );
      }
      $scope.updateStatistics();
    };

    $scope.updateStatistics = function () {
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
      $scope.filteredSchools.forEach((school) => {
        const count = school.activeStudentCount || 0;
        const age = school.avrageAges || 0;
        if (count > 0) {
          totalWeightedAge += age * count;
          totalStudents += count;
        }
      });
      $scope.globalAverageAges = totalStudents > 0 ? totalWeightedAge / totalStudents : 0;
    };

    $scope.selectSchool = async function (school) {
      if ($scope.selectedSchool && $scope.selectedSchool.educationPlaceId === school.educationPlaceId) {
        $scope.closeStudents();
        return;
      }
      $scope.selectedSchool = school;
      $scope.students = [];
      $scope.studentsError = null;
      $scope.newStudent = { educationPlaceId: school.educationPlaceId };
      $scope.loadingStudents = true;

      try {
        $scope.students = await StudentService.getStudentsByPlace(school.educationPlaceId);
      } catch (err) {
        $scope.studentsError = "שגיאה בטעינת תלמידים: " + (err.data?.message || err.statusText || "שגיאה לא ידועה");
      } finally {
        $scope.loadingStudents = false;
        $scope.$apply();
      }
    };

    $scope.closeStudents = function () {
      $scope.selectedSchool = null;
      $scope.students = [];
      $scope.studentsError = null;
      $scope.newStudent = {};
    };

    $scope.addStudent = async function () {
      $scope.addingStudent = true;

      const payload = {
        fullName: $scope.newStudent.fullName,
        nationalId: $scope.newStudent.nationalId,
        age: parseInt($scope.newStudent.age),
        educationPlaceId: $scope.selectedSchool.educationPlaceId,
        statusId: parseInt($scope.newStudent.statusId),
      };

      try {
        await StudentService.addStudent(payload);
        ToastService.show("התלמיד נוסף בהצלחה!", "success");
        $scope.newStudent = { educationPlaceId: $scope.selectedSchool.educationPlaceId };
        $scope.students = await StudentService.getStudentsByPlace($scope.selectedSchool.educationPlaceId);
        const school = $scope.schools.find((s) => s.educationPlaceId === $scope.selectedSchool.educationPlaceId);
        if (school) {
          school.activeStudentCount = (school.activeStudentCount || 0) + 1;
          $scope.updateStatistics();
        }
      } catch (err) {
        const errors = err.data?.errors;
        const msg = Array.isArray(errors)
          ? errors.map((e) => e.message).join(" | ")
          : errors
            ? Object.values(errors).flat().join(" | ")
            : err.data?.title || err.data?.message || "שגיאה בהוספת תלמיד";
        ToastService.show(msg, "error");
      } finally {
        $scope.addingStudent = false;
        $scope.$apply();
      }
    };

    $scope.retry = async function () {
      await $scope.fetchSchools();
    };

    $scope.fetchSchools();
  },
]);
