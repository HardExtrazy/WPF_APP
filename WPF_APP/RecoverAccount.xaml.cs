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
using System.Windows.Shapes;

namespace WPF_APP
{
    /// <summary>
    /// Логика взаимодействия для RecoverAccount.xaml
    /// </summary>
    public partial class RecoverAccount : Window
    {
        public RecoverAccount()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void btnBack_1_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            recoverAccount.SelectedItem = enterCode;
        }
        private void btnBack_2_Click(object sender, RoutedEventArgs e)
        {
            recoverAccount.SelectedItem = searchEmail;
        }
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            recoverAccount.SelectedItem = enterNewPassword;
        }
        private void btnBack_3_Click(object sender, RoutedEventArgs e)
        {
            recoverAccount.SelectedItem = searchEmail;
        }
        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            CustomMessageBox messageBox = new CustomMessageBox("Пароль успешно установлен");
            messageBox.ShowDialog();
            Close();
        }
    }
}
