using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Task.AirAstana.Application.Common.Interfaces;
using Task.AirAstana.Domain.Entities;
using Task.AirAstana.Domain.Enums;

namespace Task.AirAstana.Infrastructure.Persistence;

public class ApplicationDbContextInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<ApplicationDbContextInitializer> _logger;

    public ApplicationDbContextInitializer(ApplicationDbContext context, IPasswordHasher passwordHasher, ILogger<ApplicationDbContextInitializer> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async System.Threading.Tasks.Task MigrateAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();
            _logger.LogInformation("База данных успешно мигрирована");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при миграции базы данных");
            throw;
        }
    }

    /// <summary>
    /// Заполнить базу начальными данными 
    /// </summary>
    public async System.Threading.Tasks.Task SeedAsync()
    {
        try
        {
            await SeedRolesAsync();
            await SeedUsersAsync();
            await SeedFlightsAsync();

            _logger.LogInformation("База данных успешно заполнена начальными данными");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при заполнении базы данных начальными данными");
            throw;
        }
    }

    private async System.Threading.Tasks.Task SeedRolesAsync()
    {
        if (await _context.Roles.AnyAsync())
        {
            _logger.LogInformation("Роли уже существуют, пропускаем создание");
            return;
        }

        var roles = new List<Role>
        {
            new Role { Code = "User" },
            new Role { Code = "Moderator" }
        };

        await _context.Roles.AddRangeAsync(roles);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Создано ролей: {Count}", roles.Count);
    }

    private async System.Threading.Tasks.Task SeedUsersAsync()
    {
        if (await _context.Users.AnyAsync())
        {
            _logger.LogInformation("Пользователи уже существуют, пропускаем создание");
            return;
        }

        var moderatorRole = await _context.Roles.FirstAsync(r => r.Code == "Moderator");
        var userRole = await _context.Roles.FirstAsync(r => r.Code == "User");

        var users = new List<User>
        {
            new User
            {
                Username = "admin",
                PasswordHash = _passwordHasher.HashPassword("admin123"),
                RoleId = moderatorRole.Id,
                Created = DateTimeOffset.UtcNow,
                CreatedBy = "Система"
            },
            new User
            {
                Username = "user",
                PasswordHash = _passwordHasher.HashPassword("user123"),
                RoleId = userRole.Id,
                Created = DateTimeOffset.UtcNow,
                CreatedBy = "Система"
            }
        };

        await _context.Users.AddRangeAsync(users);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Создано пользователей: {Count}", users.Count);
        _logger.LogInformation("Учетные данные:");
        _logger.LogInformation("Moderator: admin / admin123");
        _logger.LogInformation("User: user / user123");
    }

    private async System.Threading.Tasks.Task SeedFlightsAsync()
    {
        if (await _context.Flights.AnyAsync())
        {
            _logger.LogInformation("Рейсы уже существуют, пропускаем создание");
            return;
        }

        var now = DateTimeOffset.UtcNow;

        var flights = new List<Flight>
        {
            new Flight
            {
                Origin = "Алматы",
                Destination = "Астана",
                Departure = now.AddDays(1).AddHours(10),
                Arrival = now.AddDays(1).AddHours(12),
                Status = FlightStatus.InTime,
                Created = now,
                CreatedBy = "Система"
            },
            new Flight
            {
                Origin = "Астана",
                Destination = "Алматы",
                Departure = now.AddDays(1).AddHours(14),
                Arrival = now.AddDays(1).AddHours(16),
                Status = FlightStatus.InTime,
                Created = now,
                CreatedBy = "Система"
            },
            new Flight
            {
                Origin = "Алматы",
                Destination = "Шымкент",
                Departure = now.AddDays(2).AddHours(8),
                Arrival = now.AddDays(2).AddHours(10),
                Status = FlightStatus.Delayed,
                Created = now,
                CreatedBy = "Система"
            },
            new Flight
            {
                Origin = "Шымкент",
                Destination = "Астана",
                Departure = now.AddDays(2).AddHours(15),
                Arrival = now.AddDays(2).AddHours(17),
                Status = FlightStatus.InTime,
                Created = now,
                CreatedBy = "Система"
            },
            new Flight
            {
                Origin = "Астана",
                Destination = "Актау",
                Departure = now.AddDays(3).AddHours(9),
                Arrival = now.AddDays(3).AddHours(12),
                Status = FlightStatus.Cancelled,
                Created = now,
                CreatedBy = "Система"
            }
        };

        await _context.Flights.AddRangeAsync(flights);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Создано тестовых рейсов: {Count}", flights.Count);
    }
}