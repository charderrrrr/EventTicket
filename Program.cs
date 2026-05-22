using EventTicket;
using EventTicket.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? Environment.GetEnvironmentVariable("DB_CONNECTION")
    ?? "Host=localhost;Database=eventticket;Username=postgres;Password=pass123"; //дефолт дефолтом

var dbService = new DatabaseService(connectionString);
dbService.Initialize();

builder.Services.AddSingleton(dbService);
builder.Services.AddSingleton(new EventTicketModule(connectionString));
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.Urls.Add("http://localhost:5003");

app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();

app.Run();