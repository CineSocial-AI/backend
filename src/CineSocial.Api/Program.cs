using System.Text;
using CineSocial.Api.GraphQL;
using CineSocial.Api.GraphQL.Mutations;
using CineSocial.Api.GraphQL.Queries;
using CineSocial.Application.DependencyInjection;
using CineSocial.Infrastructure.DependencyInjection;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Load .env file
var envPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "..", "..", ".env");
if (File.Exists(envPath))
{
    Env.Load(envPath);
}

// Add configuration from environment variables
builder.Configuration.AddEnvironmentVariables();

// Add Application & Infrastructure services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

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
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
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
    .AddMutationType<Mutation>()
    .AddTypeExtension<AuthMutations>()
    .AddTypeExtension<UserMutations>()
    .AddTypeExtension<FollowMutations>()
    .AddTypeExtension<BlockMutations>()
    .AddTypeExtension<CommentMutations>()
    .AddTypeExtension<ReactionMutations>()
    .AddTypeExtension<RateMutations>()
    .AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline
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

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

// Map REST Controllers
app.MapControllers();

// Redirect root to Swagger (REST API documentation)
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

// Map GraphQL endpoint with Banana Cake Pop IDE enabled
app.MapGraphQL().WithOptions(new HotChocolate.AspNetCore.GraphQLServerOptions
{
    Tool = { Enable = true }
});

app.Run();
