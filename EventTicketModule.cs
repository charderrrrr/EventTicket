// EventTicketModule - основной модуль приложения, фабрика подключений к базе данных.
// _connectionString - строка подключения, полученная из конфигурации при старте.
// CreateConnection - создает новое подключение к PostgreSQL для каждого запроса.
// Каждый контроллер создает свое подключение через using, что исключает утечки соединений.

using System.Data;
using EventTicket.Data;

namespace EventTicket
{
    public class EventTicketModule
    {
        private readonly string _connectionString;

        public EventTicketModule(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new DatabaseService(_connectionString).CreateConnection();
        }
    }
}