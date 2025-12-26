using ekz.Models;

namespace ekz.Classes
{
    public static class AppData
    {
        // Статическое свойство для хранения текущего пользователя
        public static users CurrentUser { get; set; }

        // Статическое подключение к БД (опционально)
        public static qwsEntities1 db = new qwsEntities1();

        // Метод для получения имени пользователя
        public static string GetUserName()
        {
            if (CurrentUser != null)
            {
                return !string.IsNullOrEmpty(CurrentUser.FullName)
                    ? CurrentUser.FullName
                    : CurrentUser.login;
            }
            return "Гость";
        }

        // Метод для очистки данных
        public static void ClearUserData()
        {
            CurrentUser = null;
            db?.Dispose();
            db = new qwsEntities1(); // Создаем новое подключение
        }

        // Метод для проверки прав администратора
        public static bool IsAdmin()
        {
            return CurrentUser?.role == "admin";
        }
    }
}