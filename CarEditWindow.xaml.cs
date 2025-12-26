using System.Windows;
using ekz.Models;

namespace ekz
{
    public partial class CarEditWindow : Window
    {
        private car _car;
        private bool _isEditMode;

        public CarEditWindow()
        {
            InitializeComponent();
            _isEditMode = false;
            Title = "Добавление автомобиля";
        }

        public CarEditWindow(car car) : this()
        {
            _car = car;
            _isEditMode = true;
            Title = "Редактирование автомобиля";
            LoadCarData();
        }

        private void LoadCarData()
        {
            if (_car != null)
            {
                txtMarka.Text = _car.marka ?? "";
                txtNumber.Text = _car.number ?? "";
            }
        }

        public car GetCar()
        {
            if (!_isEditMode)
            {
                _car = new car();
            }

            _car.marka = txtMarka.Text;
            _car.number = txtNumber.Text;

            return _car;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMarka.Text))
            {
                MessageBox.Show("Введите марку автомобиля", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNumber.Text))
            {
                MessageBox.Show("Введите госномер", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}