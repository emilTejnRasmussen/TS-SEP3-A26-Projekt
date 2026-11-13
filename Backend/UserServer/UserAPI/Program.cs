using Context;
using Entities;
using GrpcClient;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repository;
using Service;
using Service.Interface;
using UserAPI.ExceptionHandlers;
using WebApi.Authentication;

DotNetEnv.Env.Load("../../../.env");

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    $"Host={builder.Configuration["Database:Host"]};" +
    $"Port={builder.Configuration["Database:Port"]};" +
    $"Database={builder.Configuration["USER_DB_NAME"]};" +
    $"Username={builder.Configuration["USER_DB_USER"]};" +
    $"Password={builder.Configuration["USER_DB_PASSWORD"]}";

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));


builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IEventGrpcClient, EventGrpcClient>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();

builder.Services
    .AddGrpcClient<Kin.Events.EventService.EventServiceClient>(
        options =>
        {
            options.Address = new Uri(
                builder.Configuration["Grpc:EventServer"]
                ?? throw new InvalidOperationException(
                    "EventServer address is missing."));
        });

var jwtKey = Environment.GetEnvironmentVariable("USER_JWT_KEY")
    ?? throw new InvalidOperationException("JWT signing key is missing.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("JWT issuer is missing.");

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("JWT audience is missing.");

var keyBytes = Convert.FromBase64String(jwtKey);

if (keyBytes.Length < 32)
{
    throw new InvalidOperationException("JWT signing key must be at least 32 bytes.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),

            ValidateLifetime = true,
            RequireExpirationTime = true,
            RequireSignedTokens = true,

            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Dev tools - OpenApi & Swagger
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "SEP3 User API");
    });
    
}

// Database initialization
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<AppDbContext>();

    await dbContext.Database.MigrateAsync();

    /*  
    CanConnect og skrivning til consollen er fjernet.
    Hvis migrering er der forbindelse til databasen.
    */
    
}


app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();