# User Server

Updated 2026-11-23

The User Server is the user part of KIN: an **ASP.NET Core Web API** built with .NET 10, Entity Framework Core, PostgreSQL, and JWT authentication. It handles registration, login, and user lookups, and it is the entry point for events: event requests are forwarded to the EventServer over gRPC, and the hosts are filled in with user data from here.

Open the sections you want to work with. The documentation is collected here.

<details>
<summary><strong>Features and endpoints</strong></summary>

All routes start with `api/v1/`.

| Controller | Endpoint | What it does |
| --- | --- | --- |
| Auth | `POST /api/v1/auth/register` | Creates a user and returns a JWT (201) |
| Auth | `POST /api/v1/auth/login` | Logs in and returns a JWT |
| Auth | `GET /api/v1/auth/test` | Requires login. Used to check that the token works |
| Users | `GET /api/v1/users/{userId}` | Gets one user |
| Users | `GET /api/v1/users?nameContains=` | Gets users, optionally filtered by name |
| Events | `POST /api/v1/events` | Creates an event through the EventServer (201) |
| Events | `GET /api/v1/events/{eventId}` | Gets an event with the host names filled in |
| Events | `GET /api/v1/events` | Gets an overview of events |

- Exceptions are translated to status codes by `GlobalExceptionHandler`: user not found gives 404, email already registered gives 409, and invalid credentials give 401.
- Passwords are stored as hashes. The token contains the user id, email, and name.

</details>

<details>
<summary><strong>Prerequisites</strong></summary>

- .NET 10 SDK
- PostgreSQL
- EF Core CLI

Install the EF Core CLI if needed:

```powershell
dotnet tool install --global dotnet-ef --version "10.*"
```

</details>

<details>
<summary><strong>Local setup</strong></summary>

Run all commands from `Backend/UserServer/` unless otherwise specified.

**1. Create the database**

Create a local PostgreSQL database named `sep3_user_db` using Rider, pgAdmin, or another PostgreSQL tool.

**2. Configure user secrets**

Navigate to the API project:

```powershell
cd UserAPI
```

Generate and save a JWT signing key:

```powershell
$key = New-Object byte[] 32
$rng = [System.Security.Cryptography.RandomNumberGenerator]::Create()
$rng.GetBytes($key)
$rng.Dispose()
dotnet user-secrets set "Jwt:Key" ([Convert]::ToBase64String($key))
```

Configure your PostgreSQL connection:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=sep3_user_db;Username=postgres;Password=YOUR_PASSWORD"
```

Replace `YOUR_PASSWORD` with your local PostgreSQL password.

Viewing secrets in Rider:

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

**3. Run the API**

```powershell
dotnet run --project UserAPI
```

The API automatically applies existing database migrations when running in Development. The address of the EventServer is read from the `Grpc:EventServer` setting.

Open Swagger UI at:

```text
http://localhost:<PORT>/swagger
```

Use the port displayed in the terminal.

</details>

<details>
<summary><strong>Project structure and layers</strong></summary>

```text
Backend/UserServer/
├── UserAPI/          Controllers, JWT, exception handling, and Program.cs (wires everything together)
├── Service/          Business logic: AuthService, UserService, EventService
├── Repository/       Data access for users
├── Context/          AppDbContext and migrations
├── Entity/           User
├── Mapper/           UserMapper (User to UserResponse)
├── GrpcClient/       Client for the EventServer
└── Tests-UserServer/ Unit tests
Shared/DTO/           Request and response classes
```

- Controllers only call service interfaces. `Service` uses `Repository` and `Mapper`, and `Repository` uses `Context`.
- `JwtTokenService` lives in `UserAPI` but implements `ITokenService` from `Service`. The wiring happens in `Program.cs`.

</details>

<details>
<summary><strong>Database migrations</strong></summary>

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

</details>

<details>
<summary><strong>Tests</strong></summary>

The unit tests are in `Backend/UserServer/Tests-UserServer` and use **NUnit**. The repository and the token service are hand-written fakes in the `Fakes/` folder, and the password hasher is the real one from ASP.NET, so the tests need neither a database nor any JWT setup.

Run them from `Backend/UserServer/`:

```powershell
dotnet test Tests-UserServer
```

| Folder | What is tested |
| --- | --- |
| `Service/AuthServiceTests.cs` | Register (creates the user, password hash, token, email already registered) and login (correct password, wrong password, unknown email) |
| `Entity/UserTests.cs` | Creating a `User` |
| `Mapper/UserMapperTests.cs` | `UserMapper.ToDto`, and that the response contains no password |
| `Validation/AuthRequestValidationTests.cs` | The rules on `RegisterRequest` and `LoginRequest` |
| `Fakes/` | `FakeUserRepository` and `FakeTokenService` |

</details>

<details>
<summary><strong>API documentation</strong></summary>

Swagger UI provides interactive documentation for the available REST endpoints. It is only enabled in Development.

</details>

<details>
<summary><strong>Known behaviour</strong></summary>

- Logging in with an email that does not exist gives **404** (user not found) and not 401. The repository throws `UserNotFoundException` before `AuthService` can throw `InvalidCredentialsException`. A test documents this.
- `GET /api/v1/auth/test` is a test endpoint for trying out login and is not part of the final functionality.
- `CreateUserRequest` exists in `Shared/DTO` but is not used. Users are created through registration.

</details>
