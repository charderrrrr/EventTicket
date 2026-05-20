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