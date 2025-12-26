using System;
using System.Windows;

using ekz.Models;

namespace ekz
{
    public partial class UserEditWindow : Window
    {
        private users _user;
        private bool _isEditMode;

        public UserEditWindow()
        {
            InitializeComponent();
            _isEditMode = false;
            Title = "Добавление пользователя";
        }

        public UserEditWindow(users user) : this()
        {
            _user = user;
            _isEditMode = true;
            Title = "Редактирование пользователя";
            LoadUserData();
        }

        private void LoadUserData()
        {
            if (_user != null)
            {
                txtLogin.Text = _user.login;
                txtFullName.Text = _user.FullName;

                // Выбираем роль
                foreach (var item in cbRole.Items)
                {
                    if (item is System.Windows.Controls.ComboBoxItem comboItem)
                    {
                        if (comboItem.Content.ToString() == _user.role)
                        {
                            cbRole.SelectedItem = item;
                            break;
                        }
                    }
                }

                chkBlocked.IsChecked = _user.IsBlocked == "1" ||
                                       _user.IsBlocked?.ToUpper() == "Y";

                txtOrder.Text = _user.Заказ;
                txtDeparture.Text = _user.Пункт_отправления;
                txtDestination.Text = _user.Пункт_выдачи;
            }
        }

        public users GetUser()
        {
            if (!_isEditMode)
            {
                _user = new users();
            }

            _user.login = txtLogin.Text;

            if (!string.IsNullOrEmpty(txtPassword.Password))
            {
                _user.password = txtPassword.Password;
            }

            _user.FullName = txtFullName.Text;

            if (cbRole.SelectedItem is System.Windows.Controls.ComboBoxItem selectedItem)
            {
                _user.role = selectedItem.Content.ToString();
            }

            _user.IsBlocked = chkBlocked.IsChecked == true ? "1" : "0";
            _user.Заказ = txtOrder.Text;
            _user.Пункт_отправления = txtDeparture.Text;
            _user.Пункт_выдачи = txtDestination.Text;

            // Оставьте эти поля
            _user.Дата_доставки = DateTime.Now.ToString("yyyy-MM-dd");
            _user.Дата_оплаты = DateTime.Now;
            _user.Сумма = "0";

            // ВАЖНО: id_car устанавливайте в NULL или оставьте пустым
            // Не устанавливайте 0, т.к. в car нет записи с ID=0
            _user.id_car = null; // <- ИЗМЕНИТЕ НА NULL!

            return _user;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLogin.Text))
            {
                MessageBox.Show("Введите логин", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!_isEditMode && string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                MessageBox.Show("Введите пароль", "Ошибка",
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