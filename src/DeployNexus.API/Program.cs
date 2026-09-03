using System.Text;

using DeployNexus.API.Authorization;
using DeployNexus.API.Exceptions;

using DeployNexus.Application;
using DeployNexus.Application.Common.Interfaces;
using DeployNexus.Infrastructure;
using DeployNexus.Infrastructure.Data;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


// ============================================================
// Services
// ============================================================

builder.Services.AddControllers();


// ============================================================
// CORS
// ============================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://localhost:5174")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


// ============================================================
// Global Exception Handling
// ============================================================

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();


// ============================================================
// Swagger
// ============================================================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",

            Type = SecuritySchemeType.Http,

            Scheme = "bearer",

            BearerFormat = "JWT",

            In = ParameterLocation.Header,

            Description =
                "Enter your JWT token."
        });


    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference =
                        new OpenApiReference
                        {
                            Type =
                                ReferenceType.SecurityScheme,

                            Id = "Bearer"
                        }
                },

                Array.Empty<string>()
            }
        });
});


// ============================================================
// Application Layer
// ============================================================

builder.Services.AddApplication();

// ============================================================
// Current User
// ============================================================

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<
    ICurrentUserService,
    CurrentUserService>();


// ============================================================
// Infrastructure Layer
// ============================================================

builder.Services.AddInfrastructure(
    builder.Configuration.GetConnectionString(
        "DefaultConnection")!,
    builder.Configuration);


// ============================================================
// JWT Authentication
// ============================================================

var jwtSettings =
    builder.Configuration.GetSection("Jwt");


var secretKey =
    jwtSettings["SecretKey"];


builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,

                ValidateAudience = true,

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,


                ValidIssuer =
                    jwtSettings["Issuer"],

                ValidAudience =
                    jwtSettings["Audience"],


                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            secretKey!))
            };
    });


// ============================================================
// Authorization
// ============================================================

builder.Services.AddAuthorization();


builder.Services.AddScoped<
    IAuthorizationHandler,
    PermissionAuthorizationHandler>();


builder.Services.AddSingleton<
    IAuthorizationPolicyProvider,
    PermissionPolicyProvider>();


// ============================================================
// Build Application
// ============================================================

var app = builder.Build();


// ============================================================
// Global Exception Handling
// ============================================================

app.UseExceptionHandler();


// ============================================================
// Database Initialization
// ============================================================

if (!app.Environment.IsEnvironment("Testing"))
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext =
            scope.ServiceProvider
                .GetRequiredService<
                    DeployNexusDbContext>();


        await DatabaseInitializer.InitializeAsync(
            dbContext);
    }
}


// ============================================================
// Swagger
// ============================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}


// ============================================================
// HTTP Request Pipeline
// ============================================================

app.UseHttpsRedirection();


// CORS must be before Authentication
// and Authorization.

app.UseCors("Frontend");


app.UseAuthentication();

app.UseAuthorization();


// ============================================================
// Controllers
// ============================================================

app.MapControllers();


// ============================================================
// Run
// ============================================================

app.Run();


// ============================================================
// Program
// ============================================================

public partial class Program
{
}