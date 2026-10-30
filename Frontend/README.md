# KIN – Blazor frontend

Blazor Web App med render-mode **InteractiveAuto**. Frontenden indeholder ingen forretningslogik.
Den viser kun data og kalder backend (User Server) over HTTP/REST.

## Struktur

```
WebApp/            Server-projektet (host, prerendering, App.razor)
WebApp.Client/     Alt det, der også kører i browseren (WebAssembly)
  DTO/User/                CreateUserRequest, UserResponse   (matcher backendens DTO'er)
  DTO/Event/               CreateEventRequest, EventResponse (TODO: afventer backend)
  Services/
    ApiEndpoints.cs        ALLE backend-ruter samlet ét sted  <-- ret ruter her
    IUserService.cs        + HttpUserService.cs   (POST /Users, GET /Users)
    IEventService.cs       + HttpEventService.cs  (POST /Events, GET /Events)
    ServiceRegistration.cs Registrerer HttpClient + services (bruges af begge Program.cs)
  Pages/
    Users.razor            GET alle brugere          (/users)
    CreateUser.razor       POST opret bruger         (/users/create)
    Events.razor           GET alle begivenheder     (/events)
    CreateEvent.razor      POST opret begivenhed     (/events/create)
```

## Backend-adresse

Står i `ApiBaseUrl` to steder (begge skal ændres, fordi Auto kører både på server og i browser):

- `WebApp/appsettings.json`
- `WebApp.Client/wwwroot/appsettings.json`

Standard: `http://localhost:5243/` (User Servers http-profil i `launchSettings.json`).

## Det skal backend (User Server) have

1. **Endpoints**
   - `POST /Users`  body: `{ name, password, email, location }` → svarer `UserResponse`
   - `GET  /Users`  → svarer `UserResponse[]`
   - `POST /Events` body: se `CreateEventRequest.cs` → svarer `EventResponse`
   - `GET  /Events` → svarer `EventResponse[]`

   Event-kaldene går gennem User Server, som videresender til Event Server (gRPC).

2. **CORS.** Når siden kører som WebAssembly, kalder browseren backend direkte fra
   `http://localhost:5000`. Uden CORS bliver kaldet blokeret. Tilføj i User Servers `Program.cs`:

   ```csharp
   builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
       p.WithOrigins("http://localhost:5000").AllowAnyHeader().AllowAnyMethod()));
   // ...
   app.UseCors();
   ```

## Kør

1. Start User Server (http-profilen, port 5243).
2. Start frontenden med http-profilen:

   ```
   cd WebApp
   dotnet run --launch-profile http
   ```

3. Åbn http://localhost:5000

Brug http-profilen, mens backend kører på http. En https-side må ikke kalde en http-adresse
fra browseren (mixed content).

## TODO

- Ret felterne i `DTO/Event/*`, så de matcher backendens Event-DTO'er.
- Fjern `HostId` og værts-dropdown'en i `CreateEvent.razor`, når login (JWT) er på plads.
