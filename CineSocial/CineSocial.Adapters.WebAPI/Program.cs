using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using FluentValidation;
using System.Text;
using System.Text.RegularExpressions;
using CineSocial.Core.Domain.Entities;
using CineSocial.Core.Application.Ports.Repositories;
using CineSocial.Core.Application.Contracts.Services;
using CineSocial.Adapters.Infrastructure.Database;
using CineSocial.Adapters.Infrastructure.Services;
using CineSocial.Adapters.Infrastructure.Repositories;
using CineSocial.Adapters.WebAPI.Middleware;
using CineSocial.Adapters.WebAPI.HealthChecks;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load("../../.env");

var configurationBuilder = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

var configuration = configurationBuilder.Build();

ReplaceEnvironmentVariables(configuration);
builder.Configuration.AddConfiguration(configuration);

var databaseUrl = builder.Configuration.GetConnectionString("DefaultConnection");
string connectionString;
if (!string.IsNullOrEmpty(databaseUrl) && databaseUrl.StartsWith("postgres://"))
{
    var uri = new Uri(databaseUrl);
    connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={uri.UserInfo.Split(':')[0]};Password={uri.UserInfo.Split(':')[1]};SSL Mode=Require;Trust Server Certificate=true";
}
else
{
    connectionString = databaseUrl ?? throw new InvalidOperationException("Database connection string is required");
}

var jwtSecretKey = builder.Configuration["JwtSettings:SecretKey"]
                  ?? throw new InvalidOperationException("JWT SecretKey is required");

var jwtIssuer = builder.Configuration["JwtSettings:Issuer"] ?? "CineSocial";
var jwtAudience = builder.Configuration["JwtSettings:Audience"] ?? "CineSocial-Users";

var corsOrigins = builder.Configuration["CorsSettings:AllowedOrigins"]?.Split(',')
                 ?? new[] { "http://localhost:3000", "http://localhost:8080", "http://127.0.0.1:3000" };

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;

    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", corsBuilder =>
    {
        corsBuilder.WithOrigins(corsOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add health checks
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database")
    .AddDbContextCheck<ApplicationDbContext>();
builder.Services.AddSwaggerGen(c =>
{
    var apiTitle = builder.Configuration["ApiSettings:Title"] ?? "CineSocial API";
    var apiVersion = builder.Configuration["ApiSettings:Version"] ?? "v1";
    var apiDescription = builder.Configuration["ApiSettings:Description"] ?? "Film yorumlama ve öneri sistemi API'si";

    c.SwaggerDoc(apiVersion, new OpenApiInfo
    {
        Title = apiTitle,
        Version = apiVersion,
        Description = apiDescription
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CineSocial.Core.Application.EventHandlers.SendWelcomeEmailHandler).Assembly);
});

// Add FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(CineSocial.Core.Application.Validators.CreateMovieValidator).Assembly);

// Register repositories and unit of work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IMovieRepository, MovieRepository>();

// Register application services
builder.Services.AddScoped<CineSocial.Core.Application.Services.IAuthService, CineSocial.Core.Application.Services.AuthService>();
builder.Services.AddScoped<CineSocial.Core.Application.Services.IMovieService, CineSocial.Core.Application.Services.MovieService>();

// Register infrastructure services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// TODO: Implement and register other services
// builder.Services.AddScoped<IReviewService, ReviewService>();
// builder.Services.AddScoped<IWatchlistService, WatchlistService>();
// builder.Services.AddScoped<IGroupService, GroupService>();
// builder.Services.AddScoped<IPostService, PostService>();

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
});

var app = builder.Build();

// Add global exception handling
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        var apiVersion = builder.Configuration["ApiSettings:Version"] ?? "v1";
        c.SwaggerEndpoint($"/swagger/{apiVersion}/swagger.json", $"CineSocial API {apiVersion.ToUpper()}");
        c.RoutePrefix = string.Empty;
    });
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigins");
app.UseAuthentication();
app.UseAuthorization();

// Map health checks
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

    await context.Database.MigrateAsync();

    var roles = new[] { "Admin", "User", "Moderator" };
    foreach (var roleName in roles)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid> { Name = roleName });
        }
    }

    await context.SeedDataAsync(userManager, roleManager);
}

app.Run();

static void ReplaceEnvironmentVariables(IConfiguration configuration)
{
    foreach (var section in configuration.GetChildren())
    {
        ReplaceInSection(section);
    }
}

static void ReplaceInSection(IConfigurationSection section)
{
    if (section.Value != null)
    {
        var pattern = @"#\{([^}]+)\}#";
        var matches = Regex.Matches(section.Value, pattern);

        var newValue = section.Value;
        foreach (Match match in matches)
        {
            var envVarName = match.Groups[1].Value;
            var envVarValue = Environment.GetEnvironmentVariable(envVarName);

            if (!string.IsNullOrEmpty(envVarValue))
            {
                newValue = newValue.Replace(match.Value, envVarValue);
            }
        }

        if (newValue != section.Value)
        {
            section.Value = newValue;
        }
    }

    foreach (var child in section.GetChildren())
    {
        ReplaceInSection(child);
    }
}