using System.Windows;
using System.Windows.Controls;
using ekz.Pages;

namespace ekz
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                MainFrame.Navigate(new LoginPage());
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации: {ex.Message}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void NavigateToPage(Page page)
        {
            if (MainFrame != null)
            {
                MainFrame.Navigate(page);
            }
        }
    }
}