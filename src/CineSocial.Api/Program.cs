using System.Text;
using CineSocial.Api.GraphQL;
using CineSocial.Api.GraphQL.Filters;
using CineSocial.Api.GraphQL.Mutations;
using CineSocial.Api.GraphQL.Queries;
using CineSocial.Api.Middleware;
using CineSocial.Application.DependencyInjection;
using CineSocial.Infrastructure.DependencyInjection;
using DotNetEnv;
using Elastic.Apm.NetCoreAll;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog.Enrichers.Span;

var builder = WebApplication.CreateBuilder(args);

// Load .env file
var envPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".env");
if (File.Exists(envPath))
{
    Env.Load(envPath);
}

// Add configuration from environment variables
builder.Configuration.AddEnvironmentVariables();

// Configure Serilog with OpenTelemetry enricher
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.WithSpan();
});

// Configure OpenTelemetry Tracing
var serviceName = builder.Configuration["OpenTelemetry:ServiceName"] ?? "CineSocial.Api";
var serviceVersion = builder.Configuration["OpenTelemetry:ServiceVersion"] ?? "1.0.0";
var otlpEndpoint = builder.Configuration["OpenTelemetry:OtlpEndpoint"] ?? "http://localhost:4317";

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(serviceName: serviceName, serviceVersion: serviceVersion))
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation(options =>
        {
            options.RecordException = true;
            options.Filter = (httpContext) =>
            {
                // Filtrele: Swagger ve health check isteklerini izleme
                var path = httpContext.Request.Path.Value?.ToLower() ?? "";
                return !path.Contains("/swagger") && !path.Contains("/_framework");
            };
        })
        .AddHttpClientInstrumentation(options =>
        {
            options.RecordException = true;
        })
        .AddEntityFrameworkCoreInstrumentation()
        .AddSource(serviceName)
        .AddConsoleExporter()
        .AddOtlpExporter(otlpOptions =>
        {
            otlpOptions.Endpoint = new Uri(otlpEndpoint);
        }));

// Add Application & Infrastructure services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add Elastic APM
builder.Services.AddAllElasticApm();

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<GraphQLUserContextAccessor>();

// Add Controllers for REST API
builder.Services.AddControllers();

// Add Swagger for REST & GraphQL documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "CineSocial API - REST & GraphQL",
        Version = "v1",
        Description = "Hybrid API: REST endpoints at /api/* and GraphQL at /graphql"
    });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Enter your token in the text input below."
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// JWT Authentication
var jwtSecret = builder.Configuration["JWT_SECRET"] ?? throw new InvalidOperationException("JWT_SECRET not configured");
var jwtIssuer = builder.Configuration["JWT_ISSUER"] ?? "CineSocial";
var jwtAudience = builder.Configuration["JWT_AUDIENCE"] ?? "CineSocialUsers";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .SetIsOriginAllowed(origin => true)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

// GraphQL Configuration
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddTypeExtension<UserQueries>()
    .AddTypeExtension<MovieQueries>()
    .AddTypeExtension<FollowQueries>()
    .AddTypeExtension<BlockQueries>()
    .AddTypeExtension<CommentQueries>()
    .AddTypeExtension<RateQueries>()
    .AddTypeExtension<ReactionQueries>()
    .AddTypeExtension<GenreQueries>()
    .AddTypeExtension<PersonQueries>()
    .AddTypeExtension<CollectionQueries>()
    .AddTypeExtension<CountryQueries>()
    .AddTypeExtension<LanguageQueries>()
    .AddTypeExtension<KeywordQueries>()
    .AddTypeExtension<ProductionCompanyQueries>()
    .AddTypeExtension<MovieListQueries>()
    .AddMutationType<Mutation>()
    .AddTypeExtension<AuthMutations>()
    .AddTypeExtension<UserMutations>()
    .AddTypeExtension<FollowMutations>()
    .AddTypeExtension<BlockMutations>()
    .AddTypeExtension<CommentMutations>()
    .AddTypeExtension<ReactionMutations>()
    .AddTypeExtension<RateMutations>()
    .AddTypeExtension<MovieListMutations>()
    .AddErrorFilter<GraphQLErrorFilter>()
    .AddProjections()
    .AddFiltering()
    .AddSorting()
    .AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "CineSocial API v1");
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "CineSocial API - REST & GraphQL";
    });
}

// app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Add Serilog request logging middleware
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        diagnosticContext.Set("RemoteIpAddress", httpContext.Connection.RemoteIpAddress);
        diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
    };
});

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();


app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();


app.MapGraphQL().WithOptions(new HotChocolate.AspNetCore.GraphQLServerOptions
{
    Tool = { Enable = true }
});

try
{
    Log.Information("Starting CineSocial API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// Make the implicit Program class public so test projects can access it
public partial class Program { }
