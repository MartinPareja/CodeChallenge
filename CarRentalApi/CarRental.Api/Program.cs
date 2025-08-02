using CarRental.Api.Filters;
using CarRental.Application;
using CarRental.Infrastructure;
using CarRental.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CarRental.Api", Version = "v1" });

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
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration);

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret not configured.");
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerHandler>>();
                logger.LogError(context.Exception, "JWT Authentication Failed. Error Type: {ErrorType}", context.Exception?.GetType().Name ?? "Unknown");

                // --- DETAILED JWT AUTHENTICATION ERROR (CONSOLE & FILE) ---
                var errorMessage = new StringBuilder();
                errorMessage.AppendLine("--- DETAILED JWT AUTHENTICATION ERROR ---");
                errorMessage.AppendLine($"Timestamp: {DateTime.UtcNow}");
                if (context.Exception != null)
                {
                    errorMessage.AppendLine($"Exception Type: {context.Exception.GetType().Name}");
                    errorMessage.AppendLine($"Message: {context.Exception.Message}");
                    errorMessage.AppendLine($"Stack Trace: {context.Exception.StackTrace}");
                    if (context.Exception.InnerException != null)
                    {
                        errorMessage.AppendLine($"Inner Exception Type: {context.Exception.InnerException.GetType().Name}");
                        errorMessage.AppendLine($"Inner Exception Message: {context.Exception.InnerException.Message}");
                    }
                }
                else
                {
                    errorMessage.AppendLine("No specific exception provided by authentication handler.");
                }
                errorMessage.AppendLine("-----------------------------------------");

                // Log to console
                Console.WriteLine(errorMessage.ToString());

                // Log to file
                try
                {
                    var logFilePath = Path.Combine(AppContext.BaseDirectory, "jwt_auth_errors.log");
                    File.AppendAllText(logFilePath, errorMessage.ToString() + Environment.NewLine);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error writing to log file: {ex.Message}");
                }
                // -----------------------------------------------------------

                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerHandler>>();
                logger.LogInformation("JWT Token Validated Successfully for user: {Username}", context.Principal?.Identity?.Name);

                if (context.Principal is ClaimsPrincipal principal)
                {
                    foreach (var claim in principal.Claims)
                    {
                        logger.LogDebug("Claim: {Type} = {Value}", claim.Type, claim.Value);
                    }
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      builder =>
                      {
                          builder.WithOrigins("http://localhost:4200") // The Angular app's URL
                                 .AllowAnyHeader()
                                 .AllowAnyMethod();
                      });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();