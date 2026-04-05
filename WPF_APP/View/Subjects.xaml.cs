using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WPF_APP.ViewModel;

namespace WPF_APP.View
{
    public partial class Subjects : UserControl
    {
        private SubjectVM _viewModel;

        public Subjects()
        {
            InitializeComponent();
            _viewModel = new SubjectVM();
            DataContext = _viewModel;
        }

        private void search_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (teacherLoadDataGrid.ItemsSource == null) return;

            var searchText = search.Text.ToLower();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                teacherLoadDataGrid.ItemsSource = _viewModel.TeacherLoads;
            }
            else
            {
                var filtered = _viewModel.TeacherLoads
                    .Where(x => x.TeacherName.ToLower().Contains(searchText) ||
                               x.SubjectName.ToLower().Contains(searchText) ||
                               x.ClassName.ToLower().Contains(searchText))
                    .ToList();

                teacherLoadDataGrid.ItemsSource = filtered;
            }
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            var lessonHoursWindow = new LessonHours();
            lessonHoursWindow.ShowDialog();
            _viewModel.LoadTeacherLoads();
        }

        /// <summary>
        /// Полная генерация: начальная школа + предметники
        /// </summary>
        private async void btnGenerateAll_Click(object sender, RoutedEventArgs e)
        {
            btnGenerateAll.IsEnabled = false;
            btnGenerateAll.Content = "ГЕНЕРАЦИЯ...";

            await _viewModel.GenerateFullSchedule();

            btnGenerateAll.Content = "Генерировать";
            btnGenerateAll.IsEnabled = true;
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.LoadTeacherLoads();
        }

        private void btnClearLog_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ClearLogs();
        }
    }
}