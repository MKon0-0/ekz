using System;
using System.Windows;
using System.Windows.Controls;

namespace ekz.Pages
{
    public partial class MainMenuPage : Page
    {
        public MainMenuPage()
        {
            InitializeComponent();
            ShowUserInfo();
        }

        private void ShowUserInfo()
        {
            try
            {
                if (Classes.AppData.CurrentUser != null)
                {
                    string userName = !string.IsNullOrEmpty(Classes.AppData.CurrentUser.FullName)
                        ? Classes.AppData.CurrentUser.FullName
                        : Classes.AppData.CurrentUser.login;

                    tbStatus.Text = $"Вы вошли как: {userName} ({Classes.AppData.CurrentUser.role})";
                }
                else
                {
                    tbStatus.Text = "Гость";
                }
            }
            catch (Exception)
            {
                tbStatus.Text = "Гость";
            }
        }

        private void BtnUsers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверяем, есть ли права администратора
                if (Classes.AppData.CurrentUser?.role == "admin")
                {
                    NavigationService.Navigate(new UsersPage());
                }
                else
                {
                    MessageBox.Show("Только администратор может управлять пользователями!",
                        "Ошибка доступа", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCars_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NavigationService.Navigate(new CarsPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ============ НОВЫЕ МЕТОДЫ ============

        private void BtnOrders_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверяем роль (логист или администратор)
                if (Classes.AppData.CurrentUser?.role == "admin" ||
                    Classes.AppData.CurrentUser?.role == "manager" ||
                    Classes.AppData.CurrentUser?.role == "logist")
                {
                    // Покажем сообщение, что страница в разработке
                    MessageBox.Show("Управление заказами - в разработке", "Информация",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    // Позже можно будет перейти на OrdersPage
                    // NavigationService.Navigate(new OrdersPage());
                }
                else
                {
                    MessageBox.Show("Только администратор, менеджер или логист может управлять заказами!",
                        "Ошибка доступа", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRoutes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Classes.AppData.CurrentUser?.role == "admin" ||
                    Classes.AppData.CurrentUser?.role == "manager" ||
                    Classes.AppData.CurrentUser?.role == "logist")
                {
                    MessageBox.Show("Планирование маршрутов - в разработке", "Информация",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Только администратор, менеджер или логист может планировать маршруты!",
                        "Ошибка доступа", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnMaintenance_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Classes.AppData.CurrentUser?.role == "admin" ||
                    Classes.AppData.CurrentUser?.role == "manager" ||
                    Classes.AppData.CurrentUser?.role == "logist")
                {
                    MessageBox.Show("Техническое обслуживание - в разработке", "Информация",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Только администратор, менеджер или логист может управлять ТО!",
                        "Ошибка доступа", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnStatistics_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Classes.AppData.CurrentUser?.role == "admin" ||
                    Classes.AppData.CurrentUser?.role == "manager")
                {
                    // Показать окно статистики
                    ShowStatisticsWindow();
                }
                else
                {
                    MessageBox.Show("Только администратор или менеджер может просматривать статистику!",
                        "Ошибка доступа", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Classes.AppData.CurrentUser?.role == "admin")
                {
                    // Открыть окно импорта
                    var importWindow = new ImportWindow();
                    importWindow.Owner = Window.GetWindow(this);
                    importWindow.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Только администратор может импортировать данные!",
                        "Ошибка доступа", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите выйти?", "Подтверждение",
                                       MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Очищаем данные пользователя
                Classes.AppData.ClearUserData();

                // Возвращаемся на страницу входа
                var mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.NavigateToPage(new LoginPage());
                }
                else
                {
                    Application.Current.Shutdown();
                }
            }
        }

        private void BtnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ekz.Classes.AppData.CurrentUser != null)
                {
                    var changePasswordWindow = new ChangePasswordWindow(ekz.Classes.AppData.CurrentUser.ID);
                    changePasswordWindow.Owner = Window.GetWindow(this);
                    changePasswordWindow.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Сначала войдите в систему", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ============ ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ============

        private void ShowStatisticsWindow()
        {
            try
            {
                // Создаем простое окно статистики
                var statsWindow = new Window
                {
                    Title = "Статистика транспортной компании",
                    Width = 600,
                    Height = 400,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    Owner = Window.GetWindow(this)
                };

                var textBlock = new TextBlock
                {
                    Text = "Статистика:\n\n" +
                           "• Количество пользователей: ...\n" +
                           "• Количество автомобилей: ...\n" +
                           "• Загруженность автопарка: ...\n" +
                           "• Выручка за месяц: ...\n\n" +
                           "Функционал статистики находится в разработке.",
                    FontSize = 14,
                    Padding = new Thickness(20),
                    TextWrapping = TextWrapping.Wrap
                };

                statsWindow.Content = textBlock;
                statsWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка открытия статистики: {ex.Message}", "Ошибка");
            }
        }
    }
}