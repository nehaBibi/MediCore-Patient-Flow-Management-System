# MediCore PatientFlow

ASP.NET Core 10 MVC application scaffold for MediCore City Hospital.

Run (dotnet CLI):

```powershell
cd "e:\Final Pro\MediCorePatientFlow"
dotnet restore
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
```

Notes:
- Uses SQLite by default (mediCore.db in app folder).
- GCN calculation and seeding influence logic is in `Services/GcnService.cs`.
- Bed allocation logic is in `Services/BedAllocationService.cs`.
