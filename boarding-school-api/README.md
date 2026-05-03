# Boarding School API

## Design Patterns

### 1. Repository Pattern
**קבצים:** `IStudentsRepository` / `StudentRepository`, `IBoardingSchoolQuery`, `IEducationPlaceSummaryRepo`

הפרדה בין שכבת הלוגיקה לשכבת הגישה לנתונים. הcontroller עובד מול interface בלבד ואינו מודע לEF Core או לSQL.

---

### 2. Dependency Injection
**קבצים:** `Program.cs`, כל הcontrollers והrepositories

כל התלויות (repositories, validator, DbContext) מוזרקות דרך constructor ורשומות ב-DI container. מאפשר החלפה ובדיקות יחידה ללא שינוי בקוד הצורך.

---

### 3. Middleware Pipeline (Chain of Responsibility)
**קובץ:** `ErrorHandlingMiddleware.cs`

כל בקשה עוברת דרך שרשרת handlers. ה-middleware לוכד חריגות שלא טופלו, ממפה אותן לHTTP status codes מתאימים ומדווח לשירות הלוגינג — מבלי שה-controller צריך לדעת על כך.

| Exception | Status Code |
|---|---|
| `KeyNotFoundException` | 404 Not Found |
| `InvalidOperationException` | 409 Conflict |
| `ArgumentException` | 400 Bad Request |
| אחר | 500 Internal Server Error |

---

### 4. Strategy Pattern (Validator)
**קובץ:** `Validators/StudentValidator.cs`

`StudentValidator` מממש `AbstractValidator<Student>` של FluentValidation. ניתן להחליף את אסטרטגיית הולידציה (למשל בסביבת בדיקות) מבלי לשנות את ה-controller.

---

## Unit Tests

**פרויקט:** `boarding-school-api.Tests/`

**הרצה:**
```bash
dotnet test boarding-school-api.Tests
```
> הערה: יש לעצור את ה-API לפני הרצת הבדיקות (הוא נועל את ה-exe).

### חבילות
| חבילה | מטרה |
|---|---|
| `xunit` | framework לבדיקות |
| `Moq` | mocking של interfaces |
| `Microsoft.EntityFrameworkCore.InMemory` | DB אינ-ממורי לבדיקות validator |

---

### `StudentsControllerTests.cs` — 8 בדיקות
**אסטרטגיה:** Moq על `IStudentsRepository`, `IValidator<Student>`, `ILoggingService`.

| בדיקה | תרחיש | תוצאה |
|---|---|---|
| `GetAll_ReturnsOkWithStudents` | repository מחזיר רשימה | 200 OK |
| `GetByPlace_ReturnsOkWithStudents` | סינון לפי פנימייה | 200 OK |
| `Create_ValidationFails_Returns400` | שגיאות ולידציה | 400 + רשימת שגיאות |
| `Create_ValidationPasses_Returns201` | תלמיד תקין | 201 Created |
| `Update_StudentIdZero_Returns400` | `StudentId == 0` | 400 |
| `Update_ValidationFails_Returns400` | שגיאות ולידציה | 400 |
| `Update_ValidationPasses_Returns200` | עדכון תקין | 200 OK |
| `Delete_Returns204NoContent` | מחיקה תקינה | 204 No Content |

---

### `StudentValidatorTests.cs` — 12 בדיקות
**אסטרטגיה:** `StudentContext` עם InMemory DB. פנימייה 1 מושתלת מראש.

| בדיקה | תרחיש | תוצאה |
|---|---|---|
| `FullName_Empty_Fails` | שם מלא ריק | נכשל |
| `FullName_TooShort_Fails` | שם מלא קצר מ-2 תווים | נכשל |
| `FullName_TooLong_Fails` | שם מלא ארוך מ-100 תווים | נכשל |
| `NationalId_WrongLength_Fails` | ת"ז שאינה 9 ספרות | נכשל |
| `NationalId_NonNumeric_Fails` | ת"ז עם תווים לא מספריים | נכשל |
| `Age_Below6_Fails` | גיל 5 | נכשל |
| `Age_Above120_Fails` | גיל 121 | נכשל |
| `EducationPlaceId_Zero_Fails` | מזהה פנימייה 0 | נכשל |
| `EducationPlaceId_NotInDb_Fails` | פנימייה לא קיימת ב-DB | נכשל |
| `EducationPlaceId_ExistsInDb_NoError` | פנימייה קיימת ב-DB | עובר |
| `StatusId_Zero_Fails` | מזהה סטטוס 0 | נכשל |
| `ValidStudent_PassesAllRules` | כל השדות תקינים | עובר |

---

### `BoardingSchoolsControllerTests.cs` — 5 בדיקות
**אסטרטגיה:** Moq על `IBoardingSchoolQuery`, `IEducationPlaceSummaryRepo`.

| בדיקה | תרחיש | תוצאה |
|---|---|---|
| `Get_ReturnsOkWithSchools` | repository מחזיר פנימיות | 200 OK + 2 רשומות |
| `Get_EmptyList_ReturnsOkWithEmptyCollection` | אין פנימיות | 200 OK |
| `Post_Summary_ReturnsOkWithResult` | סיכום לפי עיר ומינימום תלמידים | 200 OK |
| `Post_Summary_NoFilter_ReturnsOkWithAllResults` | ללא פילטר | 200 OK |
| `TriggerCriticalIncident_AlwaysThrowsException` | endpoint משמש לסימולציה | זורק Exception עם ההודעה |

---

### `ErrorHandlingMiddlewareTests.cs` — 5 בדיקות
**אסטרטגיה:** `DefaultHttpContext` עם `MemoryStream`. בדיקת exception → HTTP status code + log level.

| בדיקה | Exception | Status | Level |
|---|---|---|---|
| `KeyNotFoundException_Returns404_LogsWarn` | `KeyNotFoundException` | 404 | WARN |
| `InvalidOperationException_Returns409_LogsWarn` | `InvalidOperationException` | 409 | WARN |
| `ArgumentException_Returns400_LogsWarn` | `ArgumentException` | 400 | WARN |
| `UnknownException_Returns500_LogsError` | `Exception` | 500 | ERROR |
| `NoException_PassesThrough_DoesNotLog` | אין | 200 | לא מתועד |

---

**סה"כ: 30 בדיקות — כולן עוברות ✓**
