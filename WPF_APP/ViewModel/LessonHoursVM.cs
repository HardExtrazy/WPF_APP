using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WPF_APP.Models;
using WPF_APP.Services;

namespace WPF_APP.ViewModel
{
    public class LessonHoursVM : INotifyPropertyChanged
    {
        private readonly SubjectService _subjectService;
        public ObservableCollection<SubjectHours> SubjectHours { get; set; }

        // Словарь для хранения сумм по классам
        private Dictionary<int, int> _totalHours = new Dictionary<int, int>();

        public Dictionary<int, int> TotalHours
        {
            get { return _totalHours; }
            set
            {
                _totalHours = value;
                OnPropertyChanged();
            }
        }

        //Ограничение нагрузки СанПиН на класс
        private readonly Dictionary<int, (int Min, int Max)> _classLimits = new Dictionary<int, (int Min, int Max)>
        {
            { 1, (18, 18) },   
            { 2, (19, 19) },   
            { 3, (22, 22) },  
            { 4, (22, 22) },   
            { 5, (25, 27) },   
            { 6, (27, 29) },   
            { 7, (28, 30) },   
            { 8, (29, 31) },   
            { 9, (29, 31) },   
            { 10, (28, 31) },  
            { 11, (28, 31) }   
        };

        public LessonHoursVM()
        {
            _subjectService = new SubjectService();
            SubjectHours = new ObservableCollection<SubjectHours>();

            // Инициализируем словарь сумм
            for (int i = 1; i <= 11; i++)
            {
                TotalHours[i] = 0;
            }

            LoadData();

            // Подписываемся на изменения коллекции
            SubjectHours.CollectionChanged += (s, e) => UpdateTotalHours();
        }

        private async void LoadData()
        {
            try
            {
                var subjects = await _subjectService.GetSubjectsAsync();
                var existingLoads = await _subjectService.GetStudyLoadsAsync();

                if (subjects != null && subjects.Any())
                {
                    SubjectHours.Clear();

                    foreach (var subject in subjects)
                    {
                        var hours = new SubjectHours
                        {
                            SubjectId = subject.Id,
                            SubjectName = subject.Name
                        };

                        // Заполняем часы из существующих данных
                        if (existingLoads != null)
                        {
                            hours.Class1Hours = GetHoursForClass(existingLoads, subject.Id, 1);
                            hours.Class2Hours = GetHoursForClass(existingLoads, subject.Id, 2);
                            hours.Class3Hours = GetHoursForClass(existingLoads, subject.Id, 3);
                            hours.Class4Hours = GetHoursForClass(existingLoads, subject.Id, 4);
                            hours.Class5Hours = GetHoursForClass(existingLoads, subject.Id, 5);
                            hours.Class6Hours = GetHoursForClass(existingLoads, subject.Id, 6);
                            hours.Class7Hours = GetHoursForClass(existingLoads, subject.Id, 7);
                            hours.Class8Hours = GetHoursForClass(existingLoads, subject.Id, 8);
                            hours.Class9Hours = GetHoursForClass(existingLoads, subject.Id, 9);
                            hours.Class10Hours = GetHoursForClass(existingLoads, subject.Id, 10);
                            hours.Class11Hours = GetHoursForClass(existingLoads, subject.Id, 11);
                        }

                        SubjectHours.Add(hours);
                    }

                    // Обновляем суммы после загрузки
                    UpdateTotalHours();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка");
            }
        }

        private int? GetHoursForClass(List<StudyLoad> loads, int subjectId, int classNumber)
        {
            var load = loads.FirstOrDefault(l => l.SubjectId == subjectId && l.ClassNumber == classNumber);
            return load?.Hours;
        }

        // Обновление сумм по классам
        public void UpdateTotalHours()
        {
            // Сбрасываем суммы
            for (int i = 1; i <= 11; i++)
            {
                TotalHours[i] = 0;
            }

            // Суммируем часы по всем предметам
            foreach (var item in SubjectHours)
            {
                if (item.Class1Hours.HasValue) TotalHours[1] += item.Class1Hours.Value;
                if (item.Class2Hours.HasValue) TotalHours[2] += item.Class2Hours.Value;
                if (item.Class3Hours.HasValue) TotalHours[3] += item.Class3Hours.Value;
                if (item.Class4Hours.HasValue) TotalHours[4] += item.Class4Hours.Value;
                if (item.Class5Hours.HasValue) TotalHours[5] += item.Class5Hours.Value;
                if (item.Class6Hours.HasValue) TotalHours[6] += item.Class6Hours.Value;
                if (item.Class7Hours.HasValue) TotalHours[7] += item.Class7Hours.Value;
                if (item.Class8Hours.HasValue) TotalHours[8] += item.Class8Hours.Value;
                if (item.Class9Hours.HasValue) TotalHours[9] += item.Class9Hours.Value;
                if (item.Class10Hours.HasValue) TotalHours[10] += item.Class10Hours.Value;
                if (item.Class11Hours.HasValue) TotalHours[11] += item.Class11Hours.Value;
            }

            // Уведомляем UI об изменении
            OnPropertyChanged(nameof(TotalHours));
        }

        // Валидация перед сохранением
        private (bool IsValid, string Message) ValidateHours()
        {
            // Считаем общее количество часов по каждому классу
            var totalByClass = new Dictionary<int, int>();

            // Инициализируем словарь
            for (int i = 1; i <= 11; i++)
            {
                totalByClass[i] = 0;
            }

            // Суммируем часы по всем предметам для каждого класса
            foreach (var item in SubjectHours)
            {
                if (item.Class1Hours.HasValue) totalByClass[1] += item.Class1Hours.Value;
                if (item.Class2Hours.HasValue) totalByClass[2] += item.Class2Hours.Value;
                if (item.Class3Hours.HasValue) totalByClass[3] += item.Class3Hours.Value;
                if (item.Class4Hours.HasValue) totalByClass[4] += item.Class4Hours.Value;
                if (item.Class5Hours.HasValue) totalByClass[5] += item.Class5Hours.Value;
                if (item.Class6Hours.HasValue) totalByClass[6] += item.Class6Hours.Value;
                if (item.Class7Hours.HasValue) totalByClass[7] += item.Class7Hours.Value;
                if (item.Class8Hours.HasValue) totalByClass[8] += item.Class8Hours.Value;
                if (item.Class9Hours.HasValue) totalByClass[9] += item.Class9Hours.Value;
                if (item.Class10Hours.HasValue) totalByClass[10] += item.Class10Hours.Value;
                if (item.Class11Hours.HasValue) totalByClass[11] += item.Class11Hours.Value;
            }

            // Проверяем каждый класс
            foreach (var classTotal in totalByClass)
            {
                var classNum = classTotal.Key;
                var total = classTotal.Value;

                // Пропускаем, если часы не заданы (0)
                if (total == 0) continue;

                var limits = _classLimits[classNum];

                if (total < limits.Min)
                {
                    return (false, $"Для {classNum} класса сумма часов ({total}) меньше минимально допустимой ({limits.Min})");
                }

                if (total > limits.Max)
                {
                    return (false, $"Для {classNum} класса сумма часов ({total}) превышает максимально допустимую ({limits.Max})");
                }
            }

            // Проверяем каждый предмет на отрицательные значения
            foreach (var item in SubjectHours)
            {
                if (item.Class1Hours < 0 || item.Class2Hours < 0 || item.Class3Hours < 0 ||
                    item.Class4Hours < 0 || item.Class5Hours < 0 || item.Class6Hours < 0 ||
                    item.Class7Hours < 0 || item.Class8Hours < 0 || item.Class9Hours < 0 ||
                    item.Class10Hours < 0 || item.Class11Hours < 0)
                {
                    return (false, "Количество часов не может быть отрицательным");
                }
            }

            return (true, "");
        }

        // Сохранение данных с валидацией
        // Сохранение данных - всегда полное пересоздание таблицы
        public async Task<bool> SaveDataAsync()
        {
            try
            {
                // Проверяем ограничения СанПиН
                var validationResult = ValidateHours();
                if (!validationResult.IsValid)
                {
                    MessageBox.Show(validationResult.Message,
                                  "Ошибка валидации",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Warning);
                    return false;
                }

                // Собираем ВСЕ текущие данные из UI (даже если они не менялись)
                var loadsToSave = new List<StudyLoadSave>();

                foreach (var item in SubjectHours)
                {
                    // 1 класс
                    if (item.Class1Hours.HasValue)
                    {
                        loadsToSave.Add(new StudyLoadSave
                        {
                            SubjectId = item.SubjectId,
                            ClassNumber = 1,
                            Hours = item.Class1Hours.Value
                        });
                    }

                    // 2 класс
                    if (item.Class2Hours.HasValue)
                    {
                        loadsToSave.Add(new StudyLoadSave
                        {
                            SubjectId = item.SubjectId,
                            ClassNumber = 2,
                            Hours = item.Class2Hours.Value
                        });
                    }

                    // 3 класс
                    if (item.Class3Hours.HasValue)
                    {
                        loadsToSave.Add(new StudyLoadSave
                        {
                            SubjectId = item.SubjectId,
                            ClassNumber = 3,
                            Hours = item.Class3Hours.Value
                        });
                    }

                    // 4 класс
                    if (item.Class4Hours.HasValue)
                    {
                        loadsToSave.Add(new StudyLoadSave
                        {
                            SubjectId = item.SubjectId,
                            ClassNumber = 4,
                            Hours = item.Class4Hours.Value
                        });
                    }

                    // 5 класс
                    if (item.Class5Hours.HasValue)
                    {
                        loadsToSave.Add(new StudyLoadSave
                        {
                            SubjectId = item.SubjectId,
                            ClassNumber = 5,
                            Hours = item.Class5Hours.Value
                        });
                    }

                    // 6 класс
                    if (item.Class6Hours.HasValue)
                    {
                        loadsToSave.Add(new StudyLoadSave
                        {
                            SubjectId = item.SubjectId,
                            ClassNumber = 6,
                            Hours = item.Class6Hours.Value
                        });
                    }

                    // 7 класс
                    if (item.Class7Hours.HasValue)
                    {
                        loadsToSave.Add(new StudyLoadSave
                        {
                            SubjectId = item.SubjectId,
                            ClassNumber = 7,
                            Hours = item.Class7Hours.Value
                        });
                    }

                    // 8 класс
                    if (item.Class8Hours.HasValue)
                    {
                        loadsToSave.Add(new StudyLoadSave
                        {
                            SubjectId = item.SubjectId,
                            ClassNumber = 8,
                            Hours = item.Class8Hours.Value
                        });
                    }

                    // 9 класс
                    if (item.Class9Hours.HasValue)
                    {
                        loadsToSave.Add(new StudyLoadSave
                        {
                            SubjectId = item.SubjectId,
                            ClassNumber = 9,
                            Hours = item.Class9Hours.Value
                        });
                    }

                    // 10 класс
                    if (item.Class10Hours.HasValue)
                    {
                        loadsToSave.Add(new StudyLoadSave
                        {
                            SubjectId = item.SubjectId,
                            ClassNumber = 10,
                            Hours = item.Class10Hours.Value
                        });
                    }

                    // 11 класс
                    if (item.Class11Hours.HasValue)
                    {
                        loadsToSave.Add(new StudyLoadSave
                        {
                            SubjectId = item.SubjectId,
                            ClassNumber = 11,
                            Hours = item.Class11Hours.Value
                        });
                    }
                }

                // Отправляем все данные на сервер для полного пересоздания таблицы
                var result = await _subjectService.SaveStudyLoadsAsync(loadsToSave);

                if (result)
                {
                    MessageBox.Show("Данные успешно сохранены", "Успех");
                    LoadData(); // Перезагружаем данные для синхронизации
                }

                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка");
                return false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}