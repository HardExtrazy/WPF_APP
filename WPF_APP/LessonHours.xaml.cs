using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPF_APP.ViewModel;

namespace WPF_APP
{
    public partial class LessonHours : Window
    {
        public LessonHours()
        {
            InitializeComponent();
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            // Даем время на завершение редактирования
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var vm = DataContext as LessonHoursVM;
                vm?.UpdateTotalHours();
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        // Сохранение данных
        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as LessonHoursVM;
            if (vm == null) return;

            try
            {
                // Блокируем кнопку на время сохранения
                btnSave.IsEnabled = false;
                btnSave.Content = "Сохранение...";

                // Сохраняем данные
                var result = await vm.SaveDataAsync();

                if (result)
                {
                    MessageBox.Show("Данные успешно сохранены!",
                                  "Успех",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Information);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
            finally
            {
                // Разблокируем кнопку
                btnSave.IsEnabled = true;
                btnSave.Content = "Сохранить";
            }
        }

        // Закрытие окна
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // Для перетаскивания окна (так как WindowStyle="None")
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        // Закрытие по Escape
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }
    }
}