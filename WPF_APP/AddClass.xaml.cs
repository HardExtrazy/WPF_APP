using System.Windows;
using System.Windows.Input;

namespace WPF_APP
{
    public partial class AddClass : Window
    {
        public AddClass()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            DragMove();
        }
    }
}