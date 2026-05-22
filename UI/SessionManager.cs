// SessionManager - синглтон для хранения данных текущего пользователя.
// _instance - единственный экземпляр менеджера сессии (потокобезопасная ленивая инициализация).
// CurrentUserId - идентификатор текущего авторизованного пользователя (0 если не задан).
// Instance - свойство доступа к синглтону, создает экземпляр при первом обращении.
// SetUser - устанавливает идентификатор пользователя при входе в систему.
// GetCurrentUserId - возвращает идентификатор текущего пользователя.

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