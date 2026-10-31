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

All commands are run from `Backend/UserServer/` unless otherwise specified.

### 1. Create the Database

Create a local PostgreSQL database named `sep3_user_db` using Rider, pgAdmin, or your preferred database tool.

### 2. Configure User Secrets

Navigate to the `UserAPI` directory:

```powershell
cd UserAPI
```

Generate a secure random JWT signing key and save it directly to User Secrets:

```powershell
dotnet user-secrets set "Jwt:Key" ([Convert]::ToBase64String([Security.Cryptography.RandomNumberGenerator]::GetBytes(32)))
```

#### Editing Secrets in Rider

1. Right-click the **UserAPI** project in Solution Explorer.
2. Select **Tools → .NET User Secrets → Open User Secrets** (menu wording may vary).
3. Add your local PostgreSQL connection string.

Your `secrets.json` should look like this:

```json
{
  "Jwt:Key": "YOUR_GENERATED_KEY",
  "ConnectionStrings:DefaultConnection": "Host=localhost;Port=5432;Database=sep3_user_db;Username=postgres;Password=YOUR_PASSWORD"
}
```

The JWT key will already be populated if you ran the command above. Replace the database credentials with your own.

Return to the `UserServer` directory:

```powershell
cd ..
```

> User Secrets are stored locally and are not committed to Git. Each developer should configure their own credentials and JWT signing key.

### 3. Apply Database Migrations

```powershell
dotnet ef database update --project Context --startup-project UserAPI
```

This applies all existing migrations to your local database.

### 4. Run the API

```powershell
dotnet run --project UserAPI
```

The API starts on the local address shown in the terminal.

## Development

When changing entities or database configuration in a way that requires a schema change, generate a migration:

```powershell
dotnet ef migrations add MigrationName --project Context --startup-project UserAPI
```

Apply it using:

```powershell
dotnet ef database update --project Context --