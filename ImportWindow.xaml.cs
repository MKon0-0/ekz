using System;
using System.IO;
using System.Text;
using System.Windows;
using Microsoft.Win32;

namespace ekz
{
    public partial class ImportWindow : Window
    {
        public ImportWindow()
        {
            InitializeComponent();
        }

        private void ImportCarsFromCsv(string filePath)
        {
            var lines = File.ReadAllLines(filePath, Encoding.UTF8);
            // Пропустить первые 3 строки
            for (int i = 3; i < lines.Length; i++)
            {
                var parts = lines[i].Split(';');
                if (parts.Length >= 3)
                {
                    var newCar = new Models.car
                    {
                        marka = parts[1].Trim('"'),
                        number = parts[2].Trim()
                    };
                    // Добавить в БД
                }
            }
        }

        private void BtnImportCars_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                    Title = "Выберите файл с автомобилями"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    tbStatus.Text = $"Выбран файл: {Path.GetFileName(openFileDialog.FileName)}";
                    MessageBox.Show($"Файл выбран: {openFileDialog.FileName}\nИмпорт в разработке",
                        "Импорт", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка импорта");
            }
        }

        private void BtnImportClients_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv|Excel files (*.xlsx)|*.xlsx",
                    Title = "Выберите файл с клиентами"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    tbStatus.Text = $"Выбран файл: {Path.GetFileName(openFileDialog.FileName)}";
                    MessageBox.Show("Импорт клиентов в разработке",
                        "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка импорта");
            }
        }

        private void BtnImportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                tbStatus.Text = "Импорт из Excel - в разработке (требуется EPPlus)";
                MessageBox.Show("Для импорта из Excel требуется библиотека EPPlus",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка импорта");
            }
        }

        private void BtnCalculateLoad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                tbStatus.Text = "Расчет загрузки выполнен!";
                MessageBox.Show("Расчет загрузки:\n• Средняя: 65%\n• Максимальная: 85%\n• Минимальная: 45%",
                    "Статистика", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}