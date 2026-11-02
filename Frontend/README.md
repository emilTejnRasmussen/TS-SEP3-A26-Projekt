# KIN – Blazor frontend

Blazor Web App med render-mode **Interactive Server** (ingen WebAssembly). Frontenden indeholder ingen forretningslogik.
Den viser kun data og kalder backend (User Server) over HTTP/REST.

Komponenterne kører på frontend-serveren, og browseren får UI-opdateringer over SignalR.
Det er frontend-serveren - ikke browseren - der kalder User Server, så CORS er ikke nødvendigt.

## Struktur

```
WebApp/
  Components/
    App.razor              Render mode (InteractiveServer, uden prerendering)
    Routes.razor
    Layout/                MainLayout, NavMenu, ReconnectModal
    Pages/                 Home, Login, Register, Users, Events, CreateEvent, NotFound, Error
  Services/
    ApiEndpoints.cs        ALLE backend-ruter samlet ét sted  <-- ret ruter her
    ServiceRegistration.cs Registrerer HttpClient, login-tilstand og services
    TokenStore.cs          JWT i krypteret localStorage (ProtectedLocalStorage)
    JwtAuthStateProvider.cs, AuthHeaderHandler.cs
    I*Service.cs + Http*Service.cs   Auth, Users, Events
```

DTO'erne ligger kun i det fælles klassebibliotek `Shared/DTO` (kontrakten mellem frontend og User Server).

## Backend-adresse

`ApiBaseUrl` i `WebApp/appsettings.json`. Standard: `http://localhost:5243/` (User Servers http-profil).
Event-kald går gennem User Server, som videresender til Event Server (gRPC).

## Login

JWT'en gemmes krypteret (ASP.NET Core Data Protection) i browserens localStorage, så JavaScript
ikke kan læse den. `TokenStore` er Scoped, dvs. én pr. kredsløb (bruger/fane).
Data Protection-nøglerne gemmes i `WebApp/dp-keys/` (gitignored), så man ikke bliver logget ud ved genstart.
Stien kan ændres med `DataProtection:KeysPath` (fx en volume i Docker).

## Kør

1. Start User Server (http-profilen, port 5243).
2. Start frontenden:

   ```
   cd WebApp
   dotnet run --launch-profile http
   ```

3. Åbn http://localhost:5000
