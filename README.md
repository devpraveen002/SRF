# SRF
Student Registration Form

1
```
dotnet new gitignore
dotnet new sln
dotnet new mvc -n StudentRegistration
dotnet sln add StudentRegistration/StudentRegistration.csproj
cd StudentRegistration
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.0
```
2
```
dotnet ef migrations add InitialCreate
dotnet ef database update
```