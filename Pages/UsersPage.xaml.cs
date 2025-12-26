using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using ekz.Models;

namespace ekz.Pages
{
    public partial class UsersPage : Page
    {
        public UsersPage()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            try
            {
                using (var context = new qwsEntities1())
                {
                    var users = context.users.ToList();
                    dgUsers.ItemsSource = users;
                    tbStatus.Text = $"Загружено: {users.Count} пользователей";
                }
            }
            catch (Exception ex)
            {
                tbStatus.Text = $"Ошибка: {ex.Message}";
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ВСЕ МЕТОДЫ ДОЛЖНЫ БЫТЬ НАПИСАНЫ:

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var editWindow = new UserEditWindow();
                if (editWindow.ShowDialog() == true)
                {
                    var user = editWindow.GetUser();

                    // Логирование для отладки
                    Console.WriteLine($"Добавляем пользователя: Login={user.login}, Name={user.FullName}");

                    using (var context = new qwsEntities1())
                    {
                        // Пробуем прямое SQL добавление для теста
                        try
                        {
                            // Сначала EF
                            context.users.Add(user);
                            context.SaveChanges();

                            MessageBox.Show($"Пользователь '{user.FullName}' добавлен успешно!", "Успех",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadUsers();
                        }
                        catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                        {
                            string errors = "Ошибки валидации:\n";
                            foreach (var validationError in ex.EntityValidationErrors)
                            {
                                foreach (var err in validationError.ValidationErrors)
                                {
                                    errors += $"{err.PropertyName}: {err.ErrorMessage}\n";
                                }
                            }
                            MessageBox.Show(errors, "Ошибка валидации",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        catch (Exception ex)
                        {
                            // Если EF не работает, пробуем SQL
                            MessageBox.Show($"EF ошибка: {ex.Message}\nПробуем SQL...",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);

                            TrySqlInsert(user);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Общая ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Метод для прямого SQL добавления
        private void TrySqlInsert(users user)
        {
            try
            {
                using (var context = new qwsEntities1())
                {
                    // Экранирование строк
                    string login = (user.login ?? "").Replace("'", "''");
                    string password = (user.password ?? "").Replace("'", "''");
                    string fullName = (user.FullName ?? "").Replace("'", "''");
                    string role = (user.role ?? "user").Replace("'", "''");
                    string isBlocked = user.IsBlocked ?? "0";

                    // id_car: если null, то пишем NULL, иначе число
                    string idCarValue = "NULL";
                    if (user.id_car.HasValue && user.id_car.Value > 0)
                    {
                        // Проверяем, существует ли такой автомобиль
                        var carExists = context.Database.SqlQuery<int>(
                            $"SELECT COUNT(*) FROM car WHERE ID = {user.id_car.Value}").FirstOrDefault();

                        if (carExists > 0)
                        {
                            idCarValue = user.id_car.Value.ToString();
                        }
                        else
                        {
                            idCarValue = "NULL";
                        }
                    }

                    // SQL запрос БЕЗ id_car или с NULL
                    string sql = $@"INSERT INTO users 
                (login, password, FullName, role, IsBlocked, 
                 Заказ, Пункт_отправления, Пункт_выдачи,
                 Дата_доставки, Дата_оплаты, Сумма, id_car)
                VALUES 
                (N'{login}', N'{password}', N'{fullName}', N'{role}', N'{isBlocked}',
                 N'{user.Заказ}', N'{user.Пункт_отправления}', N'{user.Пункт_выдачи}',
                 N'{user.Дата_доставки}', GETDATE(), 
                 N'{user.Сумма}', {idCarValue})";

                    Console.WriteLine($"SQL: {sql}");

                    int rows = context.Database.ExecuteSqlCommand(sql);

                    if (rows > 0)
                    {
                        MessageBox.Show("Пользователь добавлен через SQL!", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadUsers();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"SQL ошибка: {ex.Message}", "Ошибка SQL",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsers.SelectedItem == null)
            {
                MessageBox.Show("Выберите пользователя", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var user = dgUsers.SelectedItem as users;
                var editWindow = new UserEditWindow(user);

                if (editWindow.ShowDialog() == true)
                {
                    var updatedUser = editWindow.GetUser();

                    using (var context = new qwsEntities1())
                    {
                        var existing = context.users.Find(updatedUser.ID);
                        if (existing != null)
                        {
                            // Копируем поля
                            existing.login = updatedUser.login;
                            if (!string.IsNullOrEmpty(updatedUser.password))
                                existing.password = updatedUser.password;
                            existing.FullName = updatedUser.FullName;
                            existing.role = updatedUser.role;
                            existing.IsBlocked = updatedUser.IsBlocked;
                            existing.Заказ = updatedUser.Заказ;
                            existing.Пункт_отправления = updatedUser.Пункт_отправления;
                            existing.Пункт_выдачи = updatedUser.Пункт_выдачи;

                            context.SaveChanges();
                            LoadUsers();
                            MessageBox.Show("Изменения сохранены", "Успех",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsers.SelectedItem == null)
            {
                MessageBox.Show("Выберите пользователя", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = dgUsers.SelectedItem as users;

            var result = MessageBox.Show($"Удалить пользователя '{user.FullName}'?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new qwsEntities1())
                    {
                        var userToDelete = context.users.Find(user.ID);
                        if (userToDelete != null)
                        {
                            context.users.Remove(userToDelete);
                            context.SaveChanges();
                            LoadUsers();
                            MessageBox.Show("Пользователь удален", "Успех",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void BtnUnblock_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsers.SelectedItem == null)
            {
                MessageBox.Show("Выберите пользователя для разблокировки", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = dgUsers.SelectedItem as users;

            var result = MessageBox.Show($"Разблокировать пользователя '{user.FullName}'?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    Classes.UserBlockService.UnblockUser(user.ID);
                    LoadUsers();
                    MessageBox.Show("Пользователь разблокирован", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка разблокировки: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadUsers();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}