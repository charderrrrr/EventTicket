# EventTicket - Система продажи билетов на мероприятия
Система бронирования и покупки билетов с динамическим ценообразованием. Сделано в рамках учебной практики. 

## Выполняемые функции

- Просмотр активных событий и выбор конкретного мероприятия
- Схема зала с 2D-сеткой мест и ценовыми категориями (Партер, Балкон, VIP)
- Бронирование мест с временной блокировкой
- Покупка билетов с формой оплаты (заглушка)
- Динамическое ценообразование: чем меньше свободных мест, тем выше цена
- Возврат билетов с комиссией, зависящей от времени до события
- История покупок пользователя с ценами и статусами

## Технологии

- C# .NET 10.0
- ASP.NET Core Web API (REST контроллеры)
- Dapper/Npgsql для работы с БД
- PostgreSQL
- HTML/CSS/JavaScript

## База данных

При первом запуске автоматически создаются таблицы и заполняются тестовыми данными:

- `venues` - залы (id, name, rows, seats_per_row, blocked_seats)
- `categories` - ценовые категории (id, name, base_price, multiplier)
- `events` - события (id, name, date, venue_id, status)
- `seats` - места (id, event_id, row, number, category_id, status)
- `tickets` - билеты (id, user_id, event_id, seat_id, purchase_date, price, status)

## Установка и запуск

1. Клонировать репозиторий:

```bash
git clone https://github.com/username/EventTicket
cd EventTicket
```

2. Установить зависимости:

```bash
dotnet restore
dotnet add package Dapper
dotnet add package Npgsql
```

3. Создать базу данных в PostgreSQL:

```sql
CREATE DATABASE eventticket;
```

4. Настроить подключение в appsettings.json или через переменную окружения DB_CONNECTION:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=eventticket;Username=postgres;Password=pass123"
  }
}
```

5. Запустить проект:

```bash
dotnet run
```

6. Открыть в браузере:

```text
http://localhost:5003
```

## Структура проекта мейби

```text
EventTicket/
├── Controllers/
│   ├── EventController.cs
│   ├── VenueController.cs
│   ├── SeatController.cs
│   ├── TicketController.cs
│   └── CategoryController.cs
├── Data/
│   ├── DatabaseService.cs
│   └── Repositories/
│       ├── EventRepository.cs
│       ├── VenueRepository.cs
│       ├── SeatRepository.cs
│       ├── CategoryRepository.cs
│       └── TicketRepository.cs
├── Models/
│   ├── Enums/
│   │    ├──EventStatus.cs
│   │    ├──SeatStatus.cs
│   │    └──TicketStatus.cs
│   ├── Event.cs
│   ├── Venue.cs
│   ├── Seat.cs
│   ├── Category.cs
│   └── Ticket.cs
├── Services/
│   ├── PricingService.cs
│   ├── BookingService.cs
│   ├── RefundService.cs
│   └── VenueLayoutService.cs
├── UI/
│   └── Services/
│       └── SessionManager.cs
├── wwwroot/
│   ├── js/
│   │   └── app.js
│   ├── index.html
│   ├── events.html
│   ├── seats.html
│   └── tickets.html
├── Program.cs
└── EventTicket.csproj
```

## API Эндпоинты

| Метод | Эндпоинт | Описание |
| --- | --- | --- |
| GET | /api/events | Список активных событий |
| GET | /api/events/{id} | Информация о событии |
| POST | /api/events | Создание события |
| GET | /api/venues | Список залов |
| GET | /api/venues/{id} | Информация о зале |
| GET | /api/events/{eventId}/seats | Схема зала с местами и спросом |
| GET | /api/events/{eventId}/seats/{seatId} | Информация о месте |
| GET | /api/events/{eventId}/seats/{seatId}/price | Текущая цена места |
| GET | /api/events/{eventId}/seats/{seatId}/availability | Проверка доступности места |
| POST | /api/tickets/purchase | Покупка билета |
| POST | /api/tickets/{ticketId}/refund | Возврат билета |
| GET | /api/tickets/{ticketId}/refund-commission | Расчет комиссии за возврат |
| GET | /api/tickets | Билеты пользователя |
| GET | /api/categories | Список ценовых категорий |