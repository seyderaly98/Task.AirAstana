using Task.AirAstana.Application;
using Task.AirAstana.Infrastructure;
using Task.AirAstana.Infrastructure.Persistence;
using Task.AirAstana.API.Presentation.Middleware;
using Serilog;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// Serilog конфигурация
builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));
builder.Services.AddApplication();

// Временно для отладки
var jwtSection = builder.Configuration.GetSection("JwtSettings");
Console.WriteLine($"JWT Section Exists: {jwtSection.Exists()}");
Console.WriteLine($"JWT Secret: {jwtSection["Secret"]?.Substring(0, 10)}...");
Console.WriteLine($"JWT Issuer: {jwtSection["Issuer"]}");
Console.WriteLine($"JWT Audience: {jwtSection["Audience"]}");
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AirAstanaAPI",
        Version = "v1",
        Description = "Web API по получению данных о статусе рейсов",
        Contact = new OpenApiContact
        {
            Name = "AirAstana",
            Email = "support@test.test"
        }
    });

    // Добавление JWT авторизации в Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Введите JWT токен в формате: Bearer {ваш токен}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

#region Инициализация базы данных

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();

    try
    {
        await initializer.MigrateAsync();
        await initializer.SeedAsync();
        Log.Information("База данных успешно инициализирована");
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "Ошибка при инициализации базы данных");
        throw;
    }
}

#endregion

app.UseMiddleware<ExceptionHandlingMiddleware>(); 


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "AirAstanaAPI v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");
app.UseSerilogRequestLogging(); 

app.Use(async (context, next) =>
{
    var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
    if (!string.IsNullOrEmpty(authHeader))
    {
        Console.WriteLine($"[DEBUG] Authorization header: {authHeader}");
        if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            var token = authHeader.Substring("Bearer ".Length).Trim();
            Console.WriteLine($"[DEBUG] Extracted token: {token}");
        }
    }
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
Log.Information("Приложение запущено");

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Приложение неожиданно завершилось");
}
finally
{
    Log.CloseAndFlush();
}

