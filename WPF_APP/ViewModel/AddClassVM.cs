// WPF_APP/ViewModel/AddClassVM.cs
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPF_APP.Models;
using WPF_APP.Models.Dto;
using WPF_APP.Services;
using WPF_APP.Utilities;

namespace WPF_APP.ViewModel
{
    public class AddClassVM : INotifyPropertyChanged
    {
        private readonly ClassesService _classesService;

        // Коллекции
        private ObservableCollection<NumberItem> _classNumbers;
        private ObservableCollection<LetterItem> _classLetters;
        private ObservableCollection<MentorItem> _availableMentors;

        // Выбранные элементы
        private NumberItem _selectedNumber;
        private LetterItem _selectedLetter;
        private MentorItem _selectedMentor;
        private bool _isFirstShift;

        // Состояния
        private bool _isCreateButtonEnabled;
        private string _statusMessage;
        private int _workLoad;

        public event PropertyChangedEventHandler PropertyChanged;

        public AddClassVM()
        {
            _classesService = new ClassesService();

            ClassNumbers = new ObservableCollection<NumberItem>();
            ClassLetters = new ObservableCollection<LetterItem>();
            AvailableMentors = new ObservableCollection<MentorItem>();

            // Инициализация команд
            LoadDataCommand = new RelayCommand(async _ => await LoadDataAsync());
            CreateClassCommand = new RelayCommand(async _ => await CreateClassAsync(), _ => CanCreateClass());
            CloseCommand = new RelayCommand(Close);

            // Загрузка данных
            Task.Run(async () => await LoadDataAsync());
        }

        // Коллекции
        public ObservableCollection<NumberItem> ClassNumbers
        {
            get => _classNumbers;
            set { _classNumbers = value; OnPropertyChanged(); }
        }

        public ObservableCollection<LetterItem> ClassLetters
        {
            get => _classLetters;
            set { _classLetters = value; OnPropertyChanged(); }
        }

        public ObservableCollection<MentorItem> AvailableMentors
        {
            get => _availableMentors;
            set { _availableMentors = value; OnPropertyChanged(); }
        }

        // Выбранные элементы
        public NumberItem SelectedNumber
        {
            get => _selectedNumber;
            set
            {
                _selectedNumber = value;
                OnPropertyChanged();
                UpdateWorkLoad();
                LoadAvailableMentors();
                UpdateCreateButtonState();
            }
        }

        public LetterItem SelectedLetter
        {
            get => _selectedLetter;
            set
            {
                _selectedLetter = value;
                OnPropertyChanged();
                UpdateCreateButtonState();
            }
        }

        public MentorItem SelectedMentor
        {
            get => _selectedMentor;
            set
            {
                _selectedMentor = value;
                OnPropertyChanged();
            }
        }

        public bool IsFirstShift
        {
            get => _isFirstShift;
            set
            {
                _isFirstShift = value;
                OnPropertyChanged();
            }
        }

        public int WorkLoad
        {
            get => _workLoad;
            set
            {
                _workLoad = value;
                OnPropertyChanged();
            }
        }

        public bool IsCreateButtonEnabled
        {
            get => _isCreateButtonEnabled;
            set { _isCreateButtonEnabled = value; OnPropertyChanged(); }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        // Команды
        public ICommand LoadDataCommand { get; }
        public ICommand CreateClassCommand { get; }
        public ICommand CloseCommand { get; }

        private async Task LoadDataAsync()
        {
            try
            {
                await Application.Current.Dispatcher.InvokeAsync(() => StatusMessage = "Загрузка...");

                var numbersTask = _classesService.GetClassNumbersAsync();
                var lettersTask = _classesService.GetClassLettersAsync();

                await Task.WhenAll(numbersTask, lettersTask);

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    ClassNumbers.Clear();
                    foreach (var number in numbersTask.Result)
                    {
                        ClassNumbers.Add(number);
                    }

                    ClassLetters.Clear();
                    foreach (var letter in lettersTask.Result)
                    {
                        ClassLetters.Add(letter);
                    }

                    StatusMessage = "Готово";
                });
            }
            catch (Exception ex)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    StatusMessage = $"Ошибка: {ex.Message}";
                });
            }
        }

        private async void LoadAvailableMentors()
        {
            if (SelectedNumber == null)
            {
                AvailableMentors.Clear();
                SelectedMentor = null;
                return;
            }

            try
            {
                StatusMessage = "Загрузка классных руководителей...";

                var mentors = await _classesService.GetAvailableMentorsAsync(SelectedNumber.Id);

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    AvailableMentors.Clear();
                    foreach (var mentor in mentors)
                    {
                        AvailableMentors.Add(mentor);
                    }

                    SelectedMentor = null;
                    StatusMessage = "Готово";
                });
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
            }
        }

        private void UpdateWorkLoad()
        {
            if (SelectedNumber == null)
            {
                WorkLoad = 0;
                return;
            }

            int number = SelectedNumber.Id;

            if (number == 1)
                WorkLoad = 18;
            else if (number == 2)
                WorkLoad = 19;
            else if (number >= 3 && number <= 4)
                WorkLoad = 22;
            else if (number == 5)
                WorkLoad = 27;
            else if (number == 6)
                WorkLoad = 29;
            else if (number == 7)
                WorkLoad = 30;
            else if (number >= 8 && number <= 11)
                WorkLoad = 31;
            else
                WorkLoad = 0;
        }

        private void UpdateCreateButtonState()
        {
            IsCreateButtonEnabled = SelectedNumber != null && SelectedLetter != null;
        }

        private bool CanCreateClass()
        {
            return IsCreateButtonEnabled;
        }

        private async Task CreateClassAsync()
        {
            try
            {
                StatusMessage = "Создание класса...";

                var classDto = new ClassCreateDto
                {
                    Number = SelectedNumber.Id,
                    Letter = SelectedLetter.Id,
                    MentorId = SelectedMentor?.Id,
                    IsFirstShift = IsFirstShift,
                    WorkLoad = WorkLoad
                };

                var result = await _classesService.CreateClassAsync(classDto);

                StatusMessage = "Готово";

                MessageBox.Show(
                    $"Класс успешно создан!\n\n" +
                    $"Класс: {result.Number}{result.Letter}\n" +
                    $"Смена: {(result.Shift ? "Первая" : "Вторая")}\n" +
                    $"Нагрузка: {result.WorkLoad} часов",
                    "Успех",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                Close(null);
            }
            catch (Exception ex)
            {
                StatusMessage = "Ошибка";
                MessageBox.Show($"Ошибка при создании класса: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Close(object parameter)
        {
            if (parameter is Window window)
            {
                window.Close();
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}