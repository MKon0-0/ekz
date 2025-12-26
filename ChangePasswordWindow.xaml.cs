using System;
using System.Linq;
using System.Windows;
using ekz.Models;

namespace ekz
{
    public partial class ChangePasswordWindow : Window
    {
        private int _userId;
        private bool _isFirstLogin;

        public ChangePasswordWindow(int userId, bool isFirstLogin = false)
        {
            InitializeComponent();
            _userId = userId;
            _isFirstLogin = isFirstLogin;

            if (_isFirstLogin)
            {
                Title = "Первая смена пароля";
                btnCancel.IsEnabled = false;
                btnCancel.Visibility = Visibility.Collapsed;
            }

            txtCurrentPassword.Focus();
        }

        private void BtnChange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Console.WriteLine($"=== Начало смены пароля ===");
                Console.WriteLine($"UserID: {_userId}, Первый вход: {_isFirstLogin}");
                Console.WriteLine($"Текущий пароль: {txtCurrentPassword.Password}");
                Console.WriteLine($"Новый пароль: {txtNewPassword.Password}");

                borderError.Visibility = Visibility.Collapsed;

                if (!ValidateInput())
                {
                    Console.WriteLine("Валидация не пройдена");
                    return;
                }

                using (var context = new qwsEntities1())
                {
                    Console.WriteLine("Подключение к БД установлено");

                    var user = context.users.FirstOrDefault(u => u.ID == _userId);
                    if (user == null)
                    {
                        Console.WriteLine("Пользователь не найден");
                        ShowError("Пользователь не найден");
                        return;
                    }

                    Console.WriteLine($"Найден пользователь: {user.login}, текущий пароль в БД: {user.password}");

                    // Если не первый вход, проверяем текущий пароль
                    if (!_isFirstLogin)
                    {
                        bool passwordCorrect = user.password == txtCurrentPassword.Password;
                        Console.WriteLine($"Проверка пароля: {passwordCorrect}");

                        if (!passwordCorrect)
                        {
                            ShowError("Неверный текущий пароль");
                            txtCurrentPassword.Focus();
                            txtCurrentPassword.SelectAll();
                            return;
                        }

                        // Проверка, что новый пароль отличается
                        if (user.password == txtNewPassword.Password)
                        {
                            ShowError("Новый пароль должен отличаться от старого");
                            txtNewPassword.Focus();
                            txtNewPassword.SelectAll();
                            return;
                        }
                    }

                    Console.WriteLine("Обновляем пароль...");

                    // Сохраняем старый пароль для логов
                    string oldPassword = user.password;

                    // Обновляем пароль
                    user.password = txtNewPassword.Password;

                    // Снимаем флаг обязательной смены пароля
                    user.MustChangePassword = false;

                    // Сохраняем изменения
                    int changes = context.SaveChanges();

                    Console.WriteLine($"Изменения сохранены. Затронуто записей: {changes}");
                    Console.WriteLine($"Старый пароль: {oldPassword}");
                    Console.WriteLine($"Новый пароль: {user.password}");
                    Console.WriteLine($"MustChangePassword: {user.MustChangePassword}");

                    MessageBox.Show("Пароль успешно изменен!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Исключение: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                ShowError($"Ошибка при изменении пароля: {ex.Message}");
            }
        }

        private void BtnTestDB_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var context = new qwsEntities1())
                {
                    var user = context.users.FirstOrDefault(u => u.ID == _userId);
                    if (user != null)
                    {
                        MessageBox.Show($"Тест БД успешен!\nПользователь: {user.login}\nТекущий пароль: {user.password}\nMustChangePassword: {user.MustChangePassword}",
                            "Тест БД", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Пользователь с ID {_userId} не найден!",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка БД: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateInput()
        {
            // Если не первый вход, проверяем текущий пароль
            if (!_isFirstLogin && string.IsNullOrWhiteSpace(txtCurrentPassword.Password))
            {
                ShowError("Введите текущий пароль");
                txtCurrentPassword.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNewPassword.Password))
            {
                ShowError("Введите новый пароль");
                txtNewPassword.Focus();
                return false;
            }

            if (txtNewPassword.Password.Length < 3)
            {
                ShowError("Новый пароль должен содержать минимум 3 символа");
                txtNewPassword.Focus();
                txtNewPassword.SelectAll();
                return false;
            }

            if (txtNewPassword.Password != txtConfirmPassword.Password)
            {
                ShowError("Новый пароль и подтверждение не совпадают");
                txtConfirmPassword.Focus();
                txtConfirmPassword.SelectAll();
                return false;
            }

            return true;
        }

        private void ShowError(string message)
        {
            txtErrorMessage.Text = message;
            borderError.Visibility = Visibility.Visible;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (_isFirstLogin)
            {
                MessageBox.Show("Вы должны сменить пароль при первом входе!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DialogResult = false;
            Close();
        }

        // Навигация по Enter
        private void TxtCurrentPassword_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                txtNewPassword.Focus();
        }

        private void TxtNewPassword_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                txtConfirmPassword.Focus();
        }

        private void TxtConfirmPassword_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                BtnChange_Click(sender, e);
        }
    }
}