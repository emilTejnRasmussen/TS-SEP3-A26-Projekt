# User Server

ASP.NET Core Web API built with .NET 10, Entity Framework Core, PostgreSQL, and JWT authentication.

## Prerequisites

- .NET 10 SDK
- PostgreSQL
- EF Core CLI

Install the EF Core CLI if needed:

```powershell
dotnet tool install --global dotnet-ef --version "10.*"
```

## Local Setup

Run all commands from `Backend/UserServer/` unless otherwise specified.

### 1. Create the Database

Create a local PostgreSQL database named `sep3_user_db` using Rider, pgAdmin, or another PostgreSQL tool.

### 2. Configure User Secrets

Navigate to the API project:

```powershell
cd UserAPI
```

**Generate and save a JWT signing key:**

```powershell
$key = New-Object byte[] 32
$rng = [System.Security.Cryptography.RandomNumberGenerator]::Create()
$rng.GetBytes($key)
$rng.Dispose()
dotnet user-secrets set "Jwt:Key" ([Convert]::ToBase64String($key))
```

**Configure your PostgreSQL connection:**

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=sep3_user_db;Username=postgres;Password=YOUR_PASSWORD"
```

Replace `YOUR_PASSWORD` with your local PostgreSQL password.

#### Viewing Secrets in Rider

1. Right-click **UserAPI** in Solution Explorer.
2. Select **Tools → .NET User Secrets → Open User Secrets**.
3. Verify your settings.

Your `secrets.json` should look similar to:

```json
{
  "Jwt:Key": "YOUR_GENERATED_BASE64_KEY",
  "ConnectionStrings:DefaultConnection": "Host=localhost;Port=5432;Database=sep3_user_db;Username=postgres;Password=YOUR_PASSWORD"
}
```

> Secrets are stored locally outside the repository. Each developer uses their own database credentials and JWT signing key.

Return to the `UserServer` directory:

```powershell
cd ..
```

### 3. Run the API

```powershell
dotnet run --project UserAPI
```

The API automatically applies existing database migrations when running in Development.

Open Swagger UI at:

```text
http://localhost:<PORT>/swagger
```

Use the port displayed in the terminal.

## Development

### Database Migrations

When modifying entities in a way that changes the database schema, generate a migration:

```powershell
dotnet ef migrations add MigrationName --project Context --startup-project UserAPI
```

Pending migrations are automatically applied when the API starts in Development.

You can also apply them manually:

```powershell
dotnet ef database update --project Context --startup-project UserAPI
```

Commit generated migrations alongside the corresponding model changes.

### API Documentation

Swagger UI provides interactive documentation for the available REST endpoints.