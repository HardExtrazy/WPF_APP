using System.Windows;
using System.Windows.Controls;
using WPF_APP.ViewModel;

namespace WPF_APP.View
{
    public partial class Classes : UserControl
    {
        private ClassVM _viewModel;

        public Classes()
        {
            InitializeComponent();
            this.Loaded += Classes_Loaded;
        }

        private async void Classes_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = (ClassVM)this.DataContext;
            await _viewModel.LoadClassesAsync();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var addClassWindow = new AddClass();
            addClassWindow.ShowDialog();

            // Обновляем список после добавления
            if (_viewModel != null)
            {
                _ = _viewModel.LoadClassesAsync();
            }
        }

        private void search_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_viewModel != null)
            {
                var textBox = sender as TextBox;
                _viewModel.SearchText = textBox?.Text ?? string.Empty;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
          
        }  
    }
}