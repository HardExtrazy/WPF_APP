using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_APP
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void txtUsername_ColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {

        }

        private void btnLogIn_Click(object sender, RoutedEventArgs e)
        {

        }
        //Завершить работу приложения
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        //Свернуть окно
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        //Обработка нажатия кнопки "Создать аккаунт"
        private void hpRegistration_Click(object sender, RoutedEventArgs e)
        {
            Window RegistrationForm = new RegistrationForm();
            RegistrationForm.ShowDialog();         
        }

        private void hpCantLogIn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
