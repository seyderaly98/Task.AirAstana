# AirAstana Flight Status API

REST API для управления статусами авиарейсов с JWT авторизацией, кэшированием и логированием.

---
## 🚀 Технологии

- **.NET 6** | **ASP.NET Core Web API**
- **Entity Framework Core 6** (Code First, PostgreSQL)
- **MediatR** (CQRS паттерн)
- **FluentValidation** (валидация)
- **Serilog** (логирование)
- **Redis** (кэширование)
- **JWT Bearer** (авторизация)
- **Swagger/OpenAPI** (документация)
- **Unit** (тестирование)

## 🏗️ Архитектура - Clean Architecture (DDD)

```
┌─────────────────────────────────┐
│  Presentation (API Controllers) │
├─────────────────────────────────┤
│  Application (CQRS, Validation) │
├─────────────────────────────────┤
│  Infrastructure (EF Core, Cache)│
├─────────────────────────────────┤
│  Domain (Entities, Interfaces)  │
└─────────────────────────────────┘
```

## ⚡ Быстрый старт

## 🔐 Тестовые пользователи

| Username  | Password       | Роль      | Права                    |
|-----------|----------------|-----------|--------------------------|
| user      | user123        | User      | Только чтение            |
| admin     | admin123       | Admin     | Полный доступ            |

## 📡 API Endpoints

### Авторизация
```http
POST /api/auth/login
Body: { "username": "admin", "password": "admin123" }
```

### Рейсы
```http
GET  /api/flights                    # Список (все роли)
GET  /api/flights?origin=Алматы      # С фильтрацией
POST /api/flights                    # Создание (Moderator)
PUT  /api/flights/{id}/status        # Обновление (Moderator)
```


## ✅ Выполненные требования ТЗ

- ✅ .NET 6, EF Core Code First
- ✅ SOLID, KISS, DRY принципы
- ✅ MediatR + CQRS паттерны
- ✅ FluentValidation
- ✅ Serilog 
- ✅ Redis
- ✅ JWT авторизация
- ✅ Swagger документация
- ✅ DDD архитектура (4 слоя)
- ✅ Role-based доступ
- ✅ Фильтрация + сортировка
- ✅ Кэширование (чтение из кэша, запись в БД)
-  Unit + Integration тесты

---
## 💭 Мысля

### Вызовы работы с .NET 6

Требование использовать .NET 6 заметно увеличило время выполнения ТЗ. Пока не совсем понимаю, с чем это связано, и хотел бы узнать, какие качества это позволяет оценить.


**Основные проблемы:**
- 📦 Поиск совместимых версий библиотек 
- ⚠️ Разрешение конфликтов зависимостей 
- 🐛 Отладка ошибок версионирования 

