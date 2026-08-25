
using Application;
using Infrastructure;
using Infrastructure.Identity;
using Infrastructure.Persistence.Seeding;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Serilog;
using WebApi.Middlewares;

namespace WebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
               .ReadFrom.Configuration(new ConfigurationBuilder()
                   .AddJsonFile("appsettings.json")
                   .Build())
               .Enrich.FromLogContext()
               .CreateLogger();

            try
            {
                var builder = WebApplication.CreateBuilder(args);
                builder.Host.UseSerilog();
                builder.Services.AddInfrastructure(builder.Configuration);
                builder.Services.AddApplication();
                builder.Services.AddProblemDetails();
                builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
                builder.Services.AddHttpContextAccessor();
                builder.Services.AddControllers();

                builder.Services.AddSwaggerGen(c =>
                {

                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Warsha API", Version = "v1" });

                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
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
                    c.UseInlineDefinitionsForEnums();
                });
                var app = builder.Build();
                using (var scope = app.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    try
                    {
                        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
                        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                        var config = services.GetRequiredService<IConfiguration>();

                        await DatabaseSeeder.SeedAllAsync(roleManager, userManager, config);
                    }
                    catch (Exception ex)
                    {
                        var logger = services.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "An error occurred during database seeding");
                    }
                }
                app.UseExceptionHandler();

                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Warsha API V1");
                    c.RoutePrefix = string.Empty;
                });
                app.UseAuthentication();
                app.UseAuthorization();
                app.MapControllers();
                app.Run();

            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {

                Log.CloseAndFlush();
            }
        }
    }
}
