## Хз пока не знаю че сказать


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