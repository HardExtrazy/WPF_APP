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
using WPF_APP.Services;
using WPF_APP.ViewModel;

namespace WPF_APP
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LoginVM _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            var vm = (LoginVM)DataContext;

            // Привязка пароля
            txtPassword.PasswordChanged += (s, e) => vm.Password = txtPassword.Password;

            // Обработчик успешного входа
            vm.OnLoginSuccess = () =>
            {
                new WorkWindow().Show();
                Close();
            };
            // Обработчик неуспешного входа
            vm.OnLoginFailed = () =>
            {
                txtPassword.Clear();
                txtPassword.Focus();
            };

            // Enter для навигации
            txtUsername.KeyDown += (s, e) => { if (e.Key == Key.Enter) txtPassword.Focus(); };
            txtPassword.KeyDown += (s, e) => { if (e.Key == Key.Enter && vm.CanLogin) vm.LoginCommand.Execute(null); };

            // Фокус
            Loaded += (s, e) => txtUsername.Focus();
        }

        private void TextBoxUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtPassword.Focus();
            }
        }
        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (_viewModel != null && _viewModel.CanLogin)
                {
                    _viewModel.LoginCommand.Execute(null);
                }
            }
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
            Window RecoverAccount = new RecoverAccount();
            RecoverAccount.ShowDialog();
        }
    }
}
