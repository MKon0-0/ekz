using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ekz.Classes;
using ekz.Models;

namespace ekz.Pages
{
    public partial class LoginPage : Page
    {
        private int failedAttempts = 0;


        public LoginPage()
        {
            InitializeComponent();
            txtLogin.Focus();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                lblError.Text = "Заполните логин и пароль!";
                return;
            }

            // Тестовый вход
            if (login == "admin" && password == "123")
            {
                TestLogin(login, "admin");
                return;
            }

            try
            {
                using (var context = new qwsEntities1())
                {
                    var user = context.users
                        .FirstOrDefault(u => u.login == login && u.password == password);

                    if (user != null)
                    {
                        // ПРОВЕРКА БЛОКИРОВКИ ПО НЕАКТИВНОСТИ
                        if (Classes.UserBlockService.IsUserBlockedForInactivity(user))
                        {
                            lblError.Text = "Аккаунт заблокирован за неактивность!";
                            MessageBox.Show("Ваш аккаунт заблокирован из-за неактивности более 1 месяца.\n" +
                                          "Обратитесь к администратору для разблокировки.",
                                "Блокировка за неактивность",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        // Проверка обычной блокировки
                        if (user.IsBlocked == "1" || user.IsBlocked == "Y" || user.IsBlocked == "y")
                        {
                            lblError.Text = "Ваш аккаунт заблокирован!";
                            MessageBox.Show("Ваш аккаунт заблокирован. Обратитесь к администратору.",
                                "Блокировка", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        // Успешный вход
                        ProcessSuccessfulLogin(user);
                    }
                    else
                    {
                        // Неверный логин/пароль
                        ProcessFailedLogin();
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Ошибка подключения к базе данных";
                MessageBox.Show($"Не удалось подключиться к базе данных:\n{ex.Message}",
                    "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);

                if (MessageBox.Show("Использовать тестовый вход?", "База данных недоступна",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    TestLogin(login, "user");
                }
            }
        }

        // Метод для обновления даты последнего входа
        private void UpdateLastLoginDate(int userId)
        {
            try
            {
                using (var context = new qwsEntities1())
                {
                    var user = context.users.FirstOrDefault(u => u.ID == userId);
                    if (user != null)
                    {
                        user.LastLoginDate = DateTime.Now;

                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка обновления даты входа: {ex.Message}");
            }
        }

        // Метод для блокировки пользователя после неудачных попыток
        private void BlockUserAfterFailedAttempts(string login)
        {
            try
            {
                using (var context = new qwsEntities1())
                {
                    var user = context.users.FirstOrDefault(u => u.login == login);
                    if (user != null)
                    {
                        user.IsBlocked = "1";
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка блокировки пользователя: {ex.Message}");
            }
        }

        

        private void ProcessSuccessfulLogin(users user)
        {
            try
            {
                // Проверка обязательной смены пароля
                if (user.MustChangePassword == true)
                {
                    var changePasswordWindow = new ChangePasswordWindow(user.ID, true);
                    changePasswordWindow.Owner = Window.GetWindow(this);

                    if (changePasswordWindow.ShowDialog() != true)
                    {
                        MessageBox.Show("Вы должны сменить пароль при первом входе!",
                            "Обязательная смена пароля",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Обновляем данные пользователя после смены пароля
                    using (var context = new qwsEntities1())
                    {
                        user = context.users.FirstOrDefault(u => u.ID == user.ID);
                    }
                }

                // Сохраняем пользователя
                ekz.Classes.AppData.CurrentUser = user;

                // Обновляем дату последнего входа
                UpdateLastLoginDate(user.ID);

                // Приветствие
                string greeting = $"Добро пожаловать, {user.FullName}!";
                if (!string.IsNullOrEmpty(user.role))
                {
                    greeting += $"\nРоль: {user.role}";
                }

                MessageBox.Show(greeting, "Успешный вход",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                // Переход на главное меню
                var mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.NavigateToPage(new MainMenuPage());
                }

                failedAttempts = 0;
                lblError.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка входа: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

       


        private void TestLogin(string login, string role)
        {
            // Создаем тестового пользователя
            var testUser = new users
            {
                login = login,
                FullName = "Тестовый пользователь",
                role = role,
                IsBlocked = "0",
                Заказ = "Тестовый заказ",
                Пункт_отправления = "Москва",
                Пункт_выдачи = "Санкт-Петербург",
                Дата_доставки = "2024-01-01",
                Дата_оплаты = DateTime.Now,
                Сумма = "1000",
                id_car = null // <- ИЗМЕНИТЕ С 1 НА NULL!
            };

            // Сохраняем пользователя (ЭТА СТРОКА ВАЖНА!)
            AppData.CurrentUser = testUser;

            MessageBox.Show($"Тестовый вход как {role}!\nЛогин: {login}\nПароль: 123",
                "Тестовый режим", MessageBoxButton.OK, MessageBoxImage.Information);

            NavigateToMainMenu();
        }

        

        private void ProcessFailedLogin()
        {
            failedAttempts++;
            int attemptsLeft = 3 - failedAttempts;

            if (attemptsLeft > 0)
            {
                lblError.Text = $"Неверный логин или пароль. Осталось попыток: {attemptsLeft}";
            }
            else
            {
                lblError.Text = "Доступ заблокирован! Слишком много неудачных попыток.";
                MessageBox.Show("Слишком много неудачных попыток входа. Приложение будет закрыто.",
                    "Блокировка", MessageBoxButton.OK, MessageBoxImage.Warning);

                // Ждем 2 секунды и закрываем
                System.Threading.Tasks.Task.Delay(2000).ContinueWith(t =>
                {
                    Dispatcher.Invoke(() => Application.Current.Shutdown());
                });
            }
        }

        private void NavigateToMainMenu()
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.NavigateToPage(new MainMenuPage());
            }
        }

        // Навигация по Enter
        private void TxtLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtPassword.Focus();
            }
        }

        private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnLogin_Click(sender, e);
            }
        }

        // Кнопка тестирования БД
        private void BtnTestDB_Click(object sender, RoutedEventArgs e)
        {
            TestDatabaseConnection();
        }

        private void TestDatabaseConnection()
        {
            try
            {
                using (var context = new qwsEntities1())
                {
                    // Пробуем получить данные
                    var userCount = context.users.Count();
                    var sampleUser = context.users.FirstOrDefault();

                    string message = $"Подключение к БД успешно!\n";
                    message += $"Всего пользователей: {userCount}\n";

                    if (sampleUser != null)
                    {
                        message += $"Пример пользователя: {sampleUser.login} ({sampleUser.FullName})";
                    }

                    MessageBox.Show(message, "Тест БД",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к БД:\n{ex.Message}\n\n" +
                              $"Проверьте:\n" +
                              $"1. Запущен ли SQL Server\n" +
                              $"2. Правильность строки подключения\n" +
                              $"3. Существует ли база данных qws",
                    "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}