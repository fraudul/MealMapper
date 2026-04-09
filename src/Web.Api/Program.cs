using System.Reflection;
using System.Threading.RateLimiting;
using Application;
using HealthChecks.UI.Client;
using Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi;
using Serilog;
using Web.Api;
using Web.Api.Endpoints;
using Web.Api.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = 
        Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | 
        Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto | 
        Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedHost;
    
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MealMapper API",
        Version = "v1",
        Description = "Умный помощник по выбору еды в Минске (гео + бюджет + предпочтения)"
    });

    // Самое надёжное решение для Vertical Slice с nested Request/Response
    options.CustomSchemaIds(type =>
    {
        if (type.IsNested)
        {
            // Для вложенных типов: Todos_Create_Request, Todos_Copy_Request и т.д.
            return type.DeclaringType!.Name + "_" + type.Name;
        }

        // Для обычных типов используем полное имя без конфликтов
        return type.FullName!
            .Replace("Web.Api.Endpoints.", "")
            .Replace("+", "_");
    });

    options.UseAllOfForInheritance();
    options.UseOneOfForPolymorphism();
});

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication()
    .AddPresentation();
    

builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("Fixed", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 30,
                Window = TimeSpan.FromMinutes(1)
            }));
});

/*builder.Services.AddOpenTelemetry()
    .WithMetrics(builder =>
    {
        builder.AddAspNetCoreInstrumentation();
        builder.AddHttpClientInstrumentation();
    })
    .WithTracing(builder =>
    {
        builder.AddAspNetCoreInstrumentation();
        builder.AddHttpClientInstrumentation();
    });
*/

WebApplication app = builder.Build();

app.MapEndpoints();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseForwardedHeaders();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = "swagger";   // чтобы swagger был по /swagger
    });
    await app.ApplyMigrationsAsync();
}

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseRequestContextLogging();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapRecommendationsEndpoints();
app.MapFallbackToFile("index.html");
// REMARK: If you want to use Controllers, you'll need this.
app.MapControllers();

await app.RunAsync();

// REMARK: Required for functional and integration tests to work.
namespace Web.Api
{
    public partial class Program;
}
