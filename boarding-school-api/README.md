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
