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
    /// Логика взаимодействия для RegistrationForm.xaml
    /// </summary>
    public partial class RegistrationForm : Window
    {
        public RegistrationForm()
        {
            InitializeComponent();
           
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            registrarion.SelectedItem = stepTwo;
        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            registrarion.SelectedItem = stepOne;
        }
        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void TextBox_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        private void registrarion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void registrarion_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        private void registrarion_SelectionChanged_2(object sender, SelectionChangedEventArgs e)
        {

        }

        private void registrarion_SelectionChanged_3(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
