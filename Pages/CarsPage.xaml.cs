using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ekz.Models;

namespace ekz.Pages
{
    public partial class CarsPage : Page
    {
        public CarsPage()
        {
            InitializeComponent();
            LoadCars();
        }

        private void LoadCars()
        {
            try
            {
                using (var context = new qwsEntities1())
                {
                    var cars = context.car.ToList();
                    dgCars.ItemsSource = cars;

                    int total = cars.Count;

                    tbStatus.Text = "Данные загружены";
                    tbCount.Text = $"Всего автомобилей: {total}";
                }
            }
            catch (Exception ex)
            {
                tbStatus.Text = "Ошибка загрузки";
                MessageBox.Show($"Ошибка загрузки автомобилей: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAddCar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var editWindow = new CarEditWindow();
                if (editWindow.ShowDialog() == true)
                {
                    var newCar = editWindow.GetCar();

                    using (var context = new qwsEntities1())
                    {
                        context.car.Add(newCar);
                        context.SaveChanges();
                        LoadCars();
                        MessageBox.Show("Автомобиль добавлен", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEditCar_Click(object sender, RoutedEventArgs e)
        {
            if (dgCars.SelectedItem == null)
            {
                MessageBox.Show("Выберите автомобиль для редактирования", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var selectedCar = dgCars.SelectedItem as car;
                var editWindow = new CarEditWindow(selectedCar);

                if (editWindow.ShowDialog() == true)
                {
                    using (var context = new qwsEntities1())
                    {
                        var carToUpdate = context.car.Find(selectedCar.ID);
                        if (carToUpdate != null)
                        {
                            var updatedCar = editWindow.GetCar();

                            // Обновляем поля
                            carToUpdate.marka = updatedCar.marka;
                            carToUpdate.number = updatedCar.number;

                            context.SaveChanges();
                            LoadCars();
                            MessageBox.Show("Изменения сохранены", "Успех",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка редактирования: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDeleteCar_Click(object sender, RoutedEventArgs e)
        {
            if (dgCars.SelectedItem == null)
            {
                MessageBox.Show("Выберите автомобиль для удаления", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedCar = dgCars.SelectedItem as car;

            var result = MessageBox.Show($"Удалить автомобиль '{selectedCar.marka} ({selectedCar.number})'?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new qwsEntities1())
                    {
                        var carToDelete = context.car.Find(selectedCar.ID);
                        if (carToDelete != null)
                        {
                            context.car.Remove(carToDelete);
                            context.SaveChanges();
                            LoadCars();
                            MessageBox.Show("Автомобиль удален", "Успех",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadCars();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}