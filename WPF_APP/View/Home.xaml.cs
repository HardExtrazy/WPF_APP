using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPF_APP.Services;
using WPF_APP.ViewModel;

namespace WPF_APP.View
{
    public partial class Home : UserControl
    {
        public Home()
        {
            InitializeComponent();

        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            string token = AuthService.Token;
            if (DataContext is HomeVM vm)
            {
                await vm.LoadUserDataAsync(token);
            }
        }
        
        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
          
        }
    }
}