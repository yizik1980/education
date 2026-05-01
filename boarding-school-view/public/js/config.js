angular.module('boardingSchoolApp')
.constant('AppConfig', {
    API_BASE_URL: 'http://localhost:5268/api',
    LABELS: {
        CONNECTION_ERROR: 'לא ניתן להתחבר לשרת. וודא שה-API פועל בכתובת ',
        SERVER_ERROR: 'שגיאת שרת פנימית (500). אנא נסה שוב מאוחר יותר.',
        NOT_FOUND: 'נתיב הנתונים לא נמצא (404).',
        UNKNOWN_ERROR: 'אירעה שגיאה בלתי צפויה בעת טעינת הנתונים.',
        FORMAT_ERROR: 'פורמט נתונים לא תקין שהתקבל מהשרת.'
    }
});
