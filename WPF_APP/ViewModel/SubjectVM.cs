using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WPF_APP.Models;
using WPF_APP.Services;

namespace WPF_APP.ViewModel
{
    /// <summary>
    /// ViewModel для окна Subjects (список уроков и генерация)
    /// </summary>
    public class SubjectVM
    {
        private readonly LessonService _lessonService;
        private readonly LessonGenerationService _generationService;
        private readonly SubjectTeacherService _subjectTeacherService;

        /// <summary>
        /// Коллекция для отображения в DataGrid
        /// </summary>
        public ObservableCollection<TeacherLoadModel> TeacherLoads { get; set; }

        /// <summary>
        /// Лог сообщений для отображения результатов генерации
        /// </summary>
        public ObservableCollection<string> LogMessages { get; set; }

        /// <summary>
        /// Флаг выполнения генерации
        /// </summary>
        private bool _isGenerating;

        public SubjectVM()
        {
            _lessonService = new LessonService();
            _generationService = new LessonGenerationService();
            _subjectTeacherService = new SubjectTeacherService();

            TeacherLoads = new ObservableCollection<TeacherLoadModel>();
            LogMessages = new ObservableCollection<string>();

            // Загружаем данные при создании
            LoadTeacherLoads();
        }

        /// <summary>
        /// Загрузка данных с сервера
        /// </summary>
        public async void LoadTeacherLoads()
        {
            try
            {
                var loads = await _lessonService.GetTeacherLoadsAsync();

                TeacherLoads.Clear();

                foreach (var load in loads)
                {
                    TeacherLoads.Add(load);
                }

                LogMessages.Add($"✅ Данные обновлены: {DateTime.Now:HH:mm:ss}");
                LogMessages.Add($"📊 Загружено записей: {TeacherLoads.Count}");

                ShowTeacherSummary();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Показать суммарную нагрузку каждого учителя
        /// </summary>
        private void ShowTeacherSummary()
        {
            var teacherSummary = TeacherLoads
                .GroupBy(t => t.TeacherName)
                .Select(g => new
                {
                    TeacherName = g.Key,
                    TotalHours = g.Sum(t => t.WeeklyHours),
                    SubjectCount = g.Count(),
                    Classes = string.Join(", ", g.Select(t => t.ClassName).Distinct())
                })
                .OrderByDescending(t => t.TotalHours)
                .ToList();

            if (teacherSummary.Any())
            {
                LogMessages.Add("");
                LogMessages.Add("📊 СУММАРНАЯ НАГРУЗКА УЧИТЕЛЕЙ:");
                LogMessages.Add("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

                foreach (var teacher in teacherSummary)
                {
                    LogMessages.Add($"   👤 {teacher.TeacherName}");
                    LogMessages.Add($"      Всего часов: {teacher.TotalHours} ч");
                    LogMessages.Add($"      Предметов: {teacher.SubjectCount}");
                    LogMessages.Add($"      Классы: {teacher.Classes}");
                    LogMessages.Add("");
                }

                LogMessages.Add($"📈 Всего учителей: {teacherSummary.Count}");
                LogMessages.Add($"📚 Всего предметов: {TeacherLoads.Count}");
                LogMessages.Add($"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            }
        }

        /// <summary>
        /// Полная генерация: сначала начальная школа, потом предметники
        /// </summary>
        /// <summary>
        /// Полная генерация: сначала начальная школа, потом предметники
        /// </summary>
        public async Task<bool> GenerateFullSchedule()
        {
            if (_isGenerating)
            {
                MessageBox.Show("Генерация уже выполняется", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            // Очистка расписания
            Window waring = new CustomMessageBox("Все существующие расписания будут удалены!");
            waring.ShowDialog();
            

            try
            {
                _isGenerating = true;
                LogMessages.Clear();

                LogMessages.Add("🔄 НАЧАЛО ПОЛНОЙ ГЕНЕРАЦИИ РАСПИСАНИЯ");
                LogMessages.Add($"📅 Дата и время: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
                LogMessages.Add("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

                // ========== ОЧИСТКА ТАБЛИЦЫ SCHEDUAL ==========
                LogMessages.Add("");
                LogMessages.Add("🗑️ ОЧИСТКА ТАБЛИЦЫ РАСПИСАНИЯ");

                var clearResult = await _lessonService.ClearSchedualTableAsync();

                if (!clearResult)
                {
                    LogMessages.Add($"❌ Ошибка при очистке таблицы расписания");
                    MessageBox.Show("Не удалось очистить таблицу расписания", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                LogMessages.Add($"   ✅ Таблица расписания успешно очищена");

                // ========== ЭТАП 1: Начальная школа ==========
                LogMessages.Add("");
                LogMessages.Add("🏫 ЭТАП 1: ГЕНЕРАЦИЯ ДЛЯ НАЧАЛЬНОЙ ШКОЛЫ (1-4 КЛАССЫ)");

                var primaryResult = await _generationService.GeneratePrimaryLessonsAsync();

                if (!primaryResult.Success)
                {
                    LogMessages.Add($"❌ Ошибка на этапе начальной школы: {primaryResult.Message}");
                    return false;
                }

                // Выводим результаты начальной школы
                if (primaryResult.LessonsDeleted > 0)
                {
                    LogMessages.Add($"   🧹 Очищено уроков: {primaryResult.LessonsDeleted} записей");
                }

                if (primaryResult.LessonsCreated > 0)
                {
                    LogMessages.Add($"   ✅ Создано уроков: {primaryResult.LessonsCreated}");

                    var groupedByClass = primaryResult.CreatedLessons
                        .GroupBy(l => l.ClassName)
                        .OrderBy(g => g.Key);

                    foreach (var classGroup in groupedByClass)
                    {
                        LogMessages.Add($"      • {classGroup.Key}:");
                        foreach (var lesson in classGroup)
                        {
                            string action = lesson.CreatedLessons > 0 ? "➕" : "🔄";
                            LogMessages.Add($"         {action} {lesson.SubjectName} ({lesson.RequiredHours} ч/нед)");
                        }
                    }
                }

                // Показываем предупреждения начальной школы
                if (primaryResult.Warnings.Count > 0)
                {
                    LogMessages.Add($"   ⚠️ Предупреждений: {primaryResult.Warnings.Count}");
                    foreach (var warning in primaryResult.Warnings.Take(5))
                    {
                        LogMessages.Add($"      • {warning}");
                    }
                }

                // ========== ЭТАП 2: Учителя-предметники ==========
                LogMessages.Add("");
                LogMessages.Add("👨‍🏫 ЭТАП 2: ГЕНЕРАЦИЯ ДЛЯ УЧИТЕЛЕЙ-ПРЕДМЕТНИКОВ (5-11 КЛАССЫ)");

                var subjectResult = await _subjectTeacherService.DistributeSubjectTeachersAsync();

                if (!subjectResult.Success)
                {
                    LogMessages.Add($"❌ Ошибка на этапе предметников: {subjectResult.Message}");
                    return false;
                }

                // Выводим результаты предметников
                if (subjectResult.AssignedLessons.Any())
                {
                    LogMessages.Add($"   ✅ Назначено уроков: {subjectResult.AssignedLessons.Count}");

                    var groupedByTeacher = subjectResult.AssignedLessons
                        .GroupBy(l => l.TeacherName)
                        .OrderBy(g => g.Key);

                    foreach (var teacherGroup in groupedByTeacher)
                    {
                        var totalHours = teacherGroup.Sum(l => l.Hours);
                        LogMessages.Add($"      👤 {teacherGroup.Key}: {totalHours} ч");

                        foreach (var lesson in teacherGroup)
                        {
                            string subgroup = lesson.SubgroupNumber.HasValue
                                ? $" (подгр.{lesson.SubgroupNumber})" : "";
                            string primary = lesson.IsPrimary ? "★" : "☆";
                            LogMessages.Add($"         {primary} {lesson.ClassName} - {lesson.SubjectName}{subgroup}: {lesson.Hours} ч");
                        }
                    }
                }

                // Показываем предупреждения
                if (subjectResult.Warnings.Any())
                {
                    LogMessages.Add("");
                    LogMessages.Add($"⚠️ ПРЕДУПРЕЖДЕНИЯ:");
                    foreach (var warning in subjectResult.Warnings)
                    {
                        LogMessages.Add($"   • {warning}");
                    }
                }

                // ========== ЗАВЕРШЕНИЕ ==========
                LogMessages.Add("");
                LogMessages.Add("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                LogMessages.Add($"✅ ПОЛНАЯ ГЕНЕРАЦИЯ ЗАВЕРШЕНА в {DateTime.Now:HH:mm:ss}");

                // Обновляем данные в таблице
                await LoadTeacherLoadsAsync();

                // Показываем итоговое сообщение
                string summary = $"Начальная школа: {primaryResult.LessonsCreated} уроков\n" +
                                $"Предметники: {subjectResult.AssignedLessons.Count} уроков\n" +
                                $"Расписание: полностью очищено";

                MessageBox.Show(summary, "Генерация завершена",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                return true;
            }
            catch (Exception ex)
            {
                LogMessages.Add($"❌ КРИТИЧЕСКАЯ ОШИБКА: {ex.Message}");
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            finally
            {
                _isGenerating = false;
            }
        }

        /// <summary>
        /// Асинхронная загрузка данных
        /// </summary>
        private async Task LoadTeacherLoadsAsync()
        {
            try
            {
                var loads = await _lessonService.GetTeacherLoadsAsync();

                TeacherLoads.Clear();

                foreach (var load in loads)
                {
                    TeacherLoads.Add(load);
                }

                LogMessages.Add($"✅ Данные обновлены после генерации: {DateTime.Now:HH:mm:ss}");
            }
            catch (Exception ex)
            {
                LogMessages.Add($"❌ Ошибка обновления данных: {ex.Message}");
            }
        }

        /// <summary>
        /// Очистка лога
        /// </summary>
        public void ClearLogs()
        {
            LogMessages.Clear();
            LogMessages.Add("🧹 Лог очищен");
        }
    }
}