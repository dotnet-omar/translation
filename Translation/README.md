# DotnetOmar Logger

DotnetOmar Logger project

## Usage

Program.cs

```csharp
using Microsoft.AspNetCore.Mvc.Versioning;
using Translation.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Services.AddTranslation(builder.Configuration.GetConnectionString("DefaultConnection")!);

var app = builder.Build();

app.InitTranslations(@"i18n/frontend", @"i18n/languages.json");

app.MapControllers();

app.Run();
```

### Languages File

i18n/languages.json

```json
{
  "languages": [
    {
      "label": "العربية",
      "code": "ar",
      "isRtl": true,
      "order": 1
    },
    {
      "label": "English",
      "code": "en",
      "isRtl": false,
      "order": 2
    }
  ]
}
```

### Translation Files
- i18n/frontend/user/ar.json

```json
{
  "translate_new_user": "مستخدم جديد",
  "translate_edit_user": "تعديل مستخدم",
  "translate_add_new_user": "إضافة مستخدم جديد",
  "translate_update_existing_user": "تحديث مستخدم",
  "translate_user_details": "تفاصيل المستخدم",
  "translate_users_list": "قائمة المستخدمين",
  "translate_image": "الصورة",
  "translate_employee_number": "الرقم الوظيفي",
  "translate_permissions": "الصلاحيات",
  "translate_confirm_password": "تأكيد كلمة المرور",
  "translate_registration_time": "وقت التسجيل",
  "translate_update_user_permission_between_brackets_name": "تحديث صلاحيات المستخدم ({{name}})",
  "translate_update_permissions": "تحديث الصلاحيات",
  "translate_update_user_permissions": "تحديث صلاحيات المستخدم",
  "translate_is_signatory_qm": "مصرح بالتوقيع؟",
  "translate_assigned_permissions": "الصلاحيات المعطاه"
}
```

- i18n/frontend/user/en.json

```json
{
  "translate_new_user": "New User",
  "translate_edit_user": "Edit User",
  "translate_add_new_user": "Add new user",
  "translate_update_existing_user": "Update existing user",
  "translate_user_details": "User details",
  "translate_users_list": "Users list",
  "translate_image": "Image",
  "translate_employee_number": "Employee number",
  "translate_permissions": "Permissions",
  "translate_confirm_password": "Confirm password",
  "translate_registration_time": "Registration Time",
  "translate_update_user_permission_between_brackets_name": "Update user permission ({{name}})",
  "translate_update_permissions": "Update permissions",
  "translate_update_user_permissions": "Update user permissions",
  "translate_is_signatory_qm": "Is signatory",
  "translate_assigned_permissions": "Assigned permissions"
}
```

## Development

To run this project in development use

Clone the project

```bash
  git clone https://github.com/dotnet-omar/translation.git
```

Install Packages

```bash
  dotnet restore
```

Start the server

```bash
  dotnet run --project Test
```

## Contributing

Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## Authors

- [@omar-elsayed](https://github.com/omar-elsayed97)

## Hi, I'm Omar Elsayed! 👋

I'm a full stack javascript developer...

## 🛠 Skills

Typescript, Javascript, Angular, Ionic, Nest.js, Node.js, HTML, CSS...

## License

[MIT](https://choosealicense.com/licenses/mit/)

## Feedback

If you have any feedback, please reach out to us at challengeromar97@gmail.com
