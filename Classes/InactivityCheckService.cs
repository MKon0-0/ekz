using System;
using System.Timers;
using System.Windows;

namespace ekz.Classes
{
    public static class InactivityCheckService
    {
        private static Timer _timer;

        // Запуск фоновой проверки
        public static void Start()
        {
            // Проверяем каждые 24 часа
            _timer = new Timer(24 * 60 * 60 * 1000); // 24 часа в миллисекундах
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;

            // Первая проверка сразу
            CheckInactivity();
        }

        // Остановка сервиса
        public static void Stop()
        {
            _timer?.Stop();
            _timer?.Dispose();
        }

        private static void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            CheckInactivity();
        }

        public static void CheckInactivity()
        {
            try
            {
                UserBlockService.CheckAllUsersForInactivity();
                Console.WriteLine($"[{DateTime.Now}] Проверка неактивности выполнена");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now}] Ошибка проверки неактивности: {ex.Message}");
            }
        }
    }
}