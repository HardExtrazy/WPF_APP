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

namespace WPF_APP.View
{
    /// <summary>
    /// Логика взаимодействия для Teachers.xaml
    /// </summary>
    public partial class Teachers : UserControl
    {
        private TeacherVM _viewModel;
        public Teachers()
        {
            InitializeComponent();
            _viewModel = DataContext as TeacherVM;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadTeachers();
        }

        private async Task LoadTeachers()
        {          
                string token = AuthService.Token;      
                if (_viewModel != null)
                {
                    await _viewModel.LoadTeachersAsync(token);
                }                       
        }
    }
}
