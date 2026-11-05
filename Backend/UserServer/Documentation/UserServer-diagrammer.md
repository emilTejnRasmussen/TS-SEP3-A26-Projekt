# UserServer – klasse- og sekvensdiagrammer (Auth + login)

Dette dokument samler de færdige diagrammer for UserServer. Billederne er de aktuelle Astah-eksporter fra mapperne `Klassediagrammer` og `Sekvensdiagrammer` ved siden af denne fil, og de vises via relative stier.

| Hvad              | Astah-fil                                                             | SVG                                                |
| ----------------- | --------------------------------------------------------------------- | -------------------------------------------------- |
| Klassediagram     | `Klassediagrammer/Klassediagram-V1-Auth+login.asta`                   | `Klassediagrammer/Klassediagram-V1-Auth+login.svg` |
| Sekvensdiagrammer | `Sekvensdiagrammer/Auth+login/SEP3-Sekvensdiagrammer-Auth+login.asta` | `Sekvensdiagrammer/Auth+login/`                    |

Kilde til indholdet: branch `24-setup-c-backend-and-auth`, mappen `Backend/UserServer`.

## 1. Lagdeling (projektafhængigheder)

Afhængighederne er læst fra `ProjectReference` i hver `.csproj`:

| Projekt         | Afhænger af                                         |
| --------------- | --------------------------------------------------- |
| `UserAPI`       | `Service`, `DTO`, `Entity`, `Repository`, `Context` |
| `Service`       | `Repository`, `Mapper`, `DTO`, `Entity`             |
| `Repository`    | `Context`, `Entity`                                 |
| `Mapper`        | `DTO`, `Entity`                                     |
| `Context`       | `Entity`                                            |
| `DTO`, `Entity` | ingen                                               |

`UserAPI` er composition root (`Program.cs`) og registrerer alle implementationer i DI. `Service` kender kun `ITokenService` som interface. `JwtTokenService` ligger i `UserAPI` og bliver koblet på i `Program.cs`.

## 2. Klassediagram

![Klassediagram – Auth og login](Klassediagrammer/Klassediagram-V1-Auth%2Blogin.svg)

Multiplicity er sat ved begge ender af alle associationer. Fuld linje er association (felt i klassen), stiplet linje er afhængighed (bruges i en metode eller oprettes lokalt), og stiplet linje med hul trekant er "implementerer". Dependency- og realiseringspile har ingen multiplicity.

### Fejlhåndtering

`GlobalExceptionHandler` er ikke tegnet med pile til alle exceptions, fordi det ville gøre diagrammet uoverskueligt. Den mapper exceptions til HTTP-statuskoder således:

| Exception                         | HTTP-status      | Kastes af         |
| --------------------------------- | ---------------- | ----------------- |
| `UserNotFoundException`           | 404 Not Found    | `UserRepository`  |
| `EmailAlreadyExistsException`     | 409 Conflict     | `AuthService`     |
| `InvalidCredentialsException`     | 401 Unauthorized | `AuthService`     |
| `InvalidCurrentPasswordException` | 400 Bad Request  | ikke kastet endnu |
| `ForbiddenOperationException`     | 403 Forbidden    | ikke kastet endnu |

Andre exceptions bliver ikke håndteret af `GlobalExceptionHandler` og bliver til almindelige fejl fra ASP.NET.

## 3. Sekvensdiagrammer

Diagrammerne følger notationen fra NEC1 08 (slide 43, Communication Order Specification): hvert async-kald har sit eget svar, og hver gren i `alt` svarer hele vejen tilbage til `Client`.

### 3.1 Register – `POST /auth/register`

![Sekvensdiagram – Register](Sekvensdiagrammer/Auth%2Blogin/Register.svg)

1. `AuthService` tjekker først, om e-mailen findes (`emailAlreadyExistsAsync` → `anyAsync`).
2. Findes den, kastes `EmailAlreadyExistsException`, og klienten får **409 Conflict**.
3. Ellers hashes adgangskoden (`IPasswordHasher`), brugeren gemmes (`addAsync` → `add` og `saveChangesAsync`), og et JWT oprettes (`JwtTokenService.generateToken`). Klienten får **201 Created**.

### 3.2 Login – `POST /auth/login`

![Sekvensdiagram – Login](Sekvensdiagrammer/Auth%2Blogin/Login.svg)

1. `AuthService` henter brugeren på e-mail (`getByEmailAsync` → `firstOrDefaultAsync`).
2. Findes brugeren ikke, kastes `UserNotFoundException`, og klienten får **404 Not Found**.
3. Ellers kontrolleres adgangskoden (`verifyHashedPassword`). Er resultatet `Failed`, kastes `InvalidCredentialsException`, og klienten får **401 Unauthorized**.
4. Ellers oprettes et JWT, og klienten får **200 OK**.

Login med en ukendt e-mail giver altså 404, ikke 401. Det kommer af, at `GetByEmailAsync` kaster `UserNotFoundException`, før koden når til at tjekke adgangskoden.

### 3.3 Hent bruger – `GET /users/{userId}`

![Sekvensdiagram – Get user by id](Sekvensdiagrammer/Auth%2Blogin/Get_user_by_id.svg)

1. `UserService` henter brugeren (`getSingleByIdAsync` → `findAsync`).
2. Findes brugeren ikke, kastes `UserNotFoundException`, og klienten får **404 Not Found**.
3. Ellers mappes brugeren til en `UserResponse` (`UserMapper.toDto`), og klienten får **200 OK**.

## 4. JWT

Login og register returnerer et JSON Web Token (JWT). Levetiden er sat til 60 minutter i `UserAPI/appsettings.json`:

```json
"Jwt": {
  "Issuer": "SEP3.UserServer",
  "Audience": "SEP3.Client",
  "ExpirationMinutes": 60
}
```

`JwtTokenService` bruger `ExpirationMinutes` til at sætte udløbstidspunktet. Ved hver anmodning kontrollerer API'et, at tokenet ikke er udløbet.

**Test:** JWT'en er testet med jwt.io og Postman på registerUser, loginUser og getUserById, hvor tokenet blev anvendt succesfuldt.

## 5. Observationer fra koden

Det kan være nyttigt i analysen. Intet af dette er rettet i koden.

1. **Login med ukendt e-mail giver 404, ikke 401.** `?? throw new InvalidCredentialsException()` i `AuthService.LoginAsync` bliver aldrig nået, fordi repositoryet kaster først. Det afslører også, om en e-mail findes.
2. **`UserService.CreateUserAsync` sætter ikke `HashedPassword`.** `CreateUserRequest.password` bliver ikke brugt, og metoden er ikke koblet til nogen controller. Register-flowet gennem `AuthService` har overtaget den rolle.
3. **`InvalidCurrentPasswordException` og `ForbiddenOperationException` kastes ikke endnu.** De er kun mappet i `GlobalExceptionHandler`.
4. **`IUserRepository` har `updateAsync`, `deleteAsync` og `getMany`, som ingen service bruger endnu.**
5. **Validering sker før servicen.** `[ApiController]` afviser ugyldige `RegisterRequest` og `LoginRequest` (`Required`, `EmailAddress`, `MinLength(8)`) med 400, og det er ikke tegnet i sekvensdiagrammerne.
6. **Endpointet `GET /auth/test` er udeladt fra diagrammerne.** Det er et testendpoint, og det samme gælder hjælpeklassen `ClaimsPrincipalExtensions`.
7. **Namespaces er ikke ensartede.** Fx `Entities`, `Exceptions`, `Services.Exceptions` og `WebApi.Authentication` ligger ved siden af projektnavnene `Entity`, `Service` og `UserAPI`.
