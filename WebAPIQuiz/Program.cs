
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using WebAPIQuiz.Utils;
using Scalar.AspNetCore;

namespace WebAPIQuiz
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var jwtKey = builder.Configuration["Security:JwtKey"]; 

            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new Exception("JWT Key is missing in appsettings.json");
            }

            System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    RequireExpirationTime = true,

                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey)
                    ),

                    RoleClaimType = ClaimTypes.Role,
                    NameClaimType = ClaimTypes.Name,
                };

            });

            builder.Services.AddAuthorization();

            builder.Services.AddScoped<JwtService>();

            builder.Services.AddRateLimiter(options =>
            {
                // LOGIN - 5 requests per minute
                options.AddPolicy("LoginPolicy", context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 3,
                            Window = TimeSpan.FromMinutes(1),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 0
                        }));

                // TASK READ - 20 requests per minute per user
                options.AddPolicy("TaskReadPolicy", context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        context.User.Identity?.Name ?? "anon",
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 10,
                            Window = TimeSpan.FromMinutes(1),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 0
                        }));

                // Dynamic Rate Limiting per Role
                options.AddPolicy("DynamicPolicy", context =>
                {
                    var role = context.User.FindFirst(ClaimTypes.Role)?.Value ?? "Anonymous";

                    int limit = role switch
                    {
                        "Admin" => 50,
                        "User" => 20,
                        _ => 5
                    };

                    return RateLimitPartition.GetFixedWindowLimiter(
                        context.User.Identity?.Name ?? "anon",
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = limit,
                            Window = TimeSpan.FromMinutes(1),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 0
                        });
                });

                // Custom 429 Response
                options.OnRejected = async (context, token) =>
                {
                    var logger = context.HttpContext.RequestServices
                        .GetRequiredService<ILogger<Program>>();

                    logger.LogWarning("Rate limit exceeded for {user}",
                        context.HttpContext.User.Identity?.Name ?? "anonymous");

                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                    await context.HttpContext.Response.WriteAsJsonAsync(new
                    {
                        error = "TooManyRequests",
                        message = "You have exceeded the allowed number of requests.",
                        retryAfter = "60 seconds",
                        status = 429,
                        path = context.HttpContext.Request.Path
                    });
                };
            });

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
                
            }

            app.MapOpenApi();
            app.MapScalarApiReference();
            app.UseRateLimiter();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
