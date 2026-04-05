// WPF_APP/AddTeachers.xaml.cs
using System.Windows;
using System.Windows.Input;

namespace WPF_APP
{
    public partial class AddTeachers : Window
    {
        public AddTeachers()
        {
            InitializeComponent();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}