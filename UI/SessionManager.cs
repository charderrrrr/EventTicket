namespace EventTicket.UI
{
    public class SessionManager
    {
        private static SessionManager? _instance;
        public int CurrentUserId { get; private set; }

        private SessionManager() { }

        public static SessionManager Instance => _instance ??= new SessionManager();

        public void SetUser(int userId)
        {
            CurrentUserId = userId;
        }

        public int GetCurrentUserId()
        {
            return CurrentUserId;
        }
    }
}