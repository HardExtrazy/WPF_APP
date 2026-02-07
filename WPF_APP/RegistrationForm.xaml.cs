using WPF_APP.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace WPF_APP
{
    public partial class RegistrationForm : Window
    {
        public RegistrationForm()
        {
            InitializeComponent();     
            registrarion.SelectedIndex = 0;
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegistrationVM vm && sender is PasswordBox passwordBox)
            {
                vm.ConfirmPassword = passwordBox.Password;
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegistrationVM vm && sender is PasswordBox passwordBox)
            {
                vm.Password = passwordBox.Password;
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            registrarion.SelectedItem = stepTwo;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            registrarion.SelectedItem = stepOne;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                var element = sender as UIElement;
                if (element != null)
                {
                    element.MoveFocus(new System.Windows.Input.TraversalRequest(
                        System.Windows.Input.FocusNavigationDirection.Next));
                }
            }
        }
    }
}