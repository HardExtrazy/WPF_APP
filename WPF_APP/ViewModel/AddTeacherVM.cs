using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using WPF_APP.Models;
using WPF_APP.Models.Dto;
using WPF_APP.Services;
using WPF_APP.Utilities;

namespace WPF_APP.ViewModel
{
    public class AddTeacherVM : INotifyPropertyChanged
    {
        private readonly TeachersService _teachersService;
        private readonly Dictionary<string, List<int>> _subjectGroups;
        private readonly Dispatcher _dispatcher;

        // Основные свойства
        private string _firstName;
        private string _lastName;
        private string _surename;
        private DateTime? _dateOfBirth;
        private Education _selectedEducation;
        private Role _selectedRole;

        private ObservableCollection<Education> _educations;
        private ObservableCollection<Role> _roles;
        private ObservableCollection<SubjectCheckBox> _subjectCheckBoxes;
        private bool _isCreateButtonEnabled;
        private string _statusMessage;

        public event PropertyChangedEventHandler PropertyChanged;

        public AddTeacherVM()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;

            _teachersService = new TeachersService();
            _subjectGroups = InitializeSubjectGroups();
            _subjectCheckBoxes = new ObservableCollection<SubjectCheckBox>();

            // Подписываемся на изменения коллекции
            _subjectCheckBoxes.CollectionChanged += SubjectCheckBoxes_CollectionChanged;

            // Инициализация команд
            LoadDataCommand = new RelayCommand(async _ => await LoadDataAsync());
            CreateTeacherCommand = new RelayCommand(async _ => await CreateTeacherAsync(), _ => CanCreateTeacher());
            CloseCommand = new RelayCommand(Close);

            // Загрузка данных
            Task.Run(async () => await LoadDataAsync());
        }

        // Обработчик изменения коллекции
        private void SubjectCheckBoxes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (SubjectCheckBox item in e.NewItems)
                {
                    item.PropertyChanged += SubjectCheckBox_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (SubjectCheckBox item in e.OldItems)
                {
                    item.PropertyChanged -= SubjectCheckBox_PropertyChanged;
                }
            }
        }

        // Обработчик изменения свойства чекбокса
        private void SubjectCheckBox_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SubjectCheckBox.IsChecked))
            {
                UpdateCreateButtonState();
            }
        }

        // Основные свойства
        public string FirstName
        {
            get => _firstName;
            set { _firstName = value; OnPropertyChanged(); UpdateCreateButtonState(); }
        }

        public string LastName
        {
            get => _lastName;
            set { _lastName = value; OnPropertyChanged(); UpdateCreateButtonState(); }
        }

        public string Surename
        {
            get => _surename;
            set { _surename = value; OnPropertyChanged(); }
        }

        public DateTime? DateOfBirth
        {
            get => _dateOfBirth;
            set { _dateOfBirth = value; OnPropertyChanged(); UpdateCreateButtonState(); }
        }

        public ObservableCollection<Education> Educations
        {
            get => _educations;
            set { _educations = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Role> Roles
        {
            get => _roles;
            set { _roles = value; OnPropertyChanged(); }
        }

        public Education SelectedEducation
        {
            get => _selectedEducation;
            set { _selectedEducation = value; OnPropertyChanged(); UpdateCreateButtonState(); }
        }

        public Role SelectedRole
        {
            get => _selectedRole;
            set
            {
                _selectedRole = value;
                OnPropertyChanged();
                UpdateSubjectCheckBoxesState();
                UpdateCreateButtonState();
            }
        }

        public ObservableCollection<SubjectCheckBox> SubjectCheckBoxes
        {
            get => _subjectCheckBoxes;
            set { _subjectCheckBoxes = value; OnPropertyChanged(); }
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
        public ICommand CreateTeacherCommand { get; }
        public ICommand CloseCommand { get; }

        private async Task LoadDataAsync()
        {
            try
            {
                await _dispatcher.InvokeAsync(() => StatusMessage = "Загрузка...");

                var educationsTask = _teachersService.GetEducationsAsync();
                var rolesTask = _teachersService.GetTeacherRolesAsync();
                var subjectsTask = _teachersService.GetAllSubjectsAsync();

                await Task.WhenAll(educationsTask, rolesTask, subjectsTask);

                var educations = await educationsTask;
                var roles = await rolesTask;
                var subjects = await subjectsTask;

                await _dispatcher.InvokeAsync(() =>
                {
                    Educations = new ObservableCollection<Education>(educations ?? new List<Education>());
                    Roles = new ObservableCollection<Role>(roles ?? new List<Role>());

                    InitializeSubjectCheckBoxes(subjects ?? new List<Subject>());

                    StatusMessage = "Готово";
                });
            }
            catch (Exception ex)
            {
                await _dispatcher.InvokeAsync(() =>
                {
                    StatusMessage = $"Ошибка: {ex.Message}";
                });
            }
        }

        private void InitializeSubjectCheckBoxes(List<Subject> allSubjects)
        {
            SubjectCheckBoxes.Clear();

            foreach (var group in _subjectGroups)
            {
                var existingSubjectIds = group.Value
                    .Where(id => allSubjects.Any(s => s.Id == id))
                    .ToList();

                if (existingSubjectIds.Any())
                {
                    var checkBox = new SubjectCheckBox
                    {
                        Id = group.Value.First(),
                        DisplayName = group.Key,
                        SubjectIds = existingSubjectIds,
                        IsChecked = false,
                        IsEnabled = true
                    };

                    // Подписываемся на изменение свойства
                    checkBox.PropertyChanged += SubjectCheckBox_PropertyChanged;

                    SubjectCheckBoxes.Add(checkBox);
                }
            }
        }

        private void UpdateSubjectCheckBoxesState()
        {
            if (SelectedRole?.Id == 4) // Учитель начальных классов
            {
                foreach (var checkBox in SubjectCheckBoxes)
                {
                    checkBox.IsEnabled = false;
                    checkBox.IsChecked = false;
                }
            }
            else if (SelectedRole?.Id == 5) // Учитель-предметник
            {
                foreach (var checkBox in SubjectCheckBoxes)
                {
                    checkBox.IsEnabled = true;
                }
            }
        }

        private void UpdateCreateButtonState()
        {
            bool isBasicInfoValid = !string.IsNullOrWhiteSpace(FirstName) &&
                                    !string.IsNullOrWhiteSpace(LastName) &&
                                    DateOfBirth.HasValue &&
                                    SelectedEducation != null &&
                                    SelectedRole != null;

            if (SelectedRole?.Id == 5) // Для предметников
            {
                bool hasSelectedSubject = SubjectCheckBoxes.Any(sb => sb.IsChecked);
                System.Diagnostics.Debug.WriteLine($"Предметников: hasSelectedSubject = {hasSelectedSubject}, выбрано предметов: {SubjectCheckBoxes.Count(sb => sb.IsChecked)}");
                IsCreateButtonEnabled = isBasicInfoValid && hasSelectedSubject;
            }
            else
            {
                IsCreateButtonEnabled = isBasicInfoValid;
            }

            System.Diagnostics.Debug.WriteLine($"IsCreateButtonEnabled = {IsCreateButtonEnabled}");
        }

        private bool CanCreateTeacher()
        {
            return IsCreateButtonEnabled;
        }

        private async Task CreateTeacherAsync()
        {
            try
            {
                StatusMessage = "Создание...";

                var teacherDto = new TeacherCreateDto
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    Surename = Surename,
                    DateOfBirth = DateOfBirth.Value, // Отправляем DateTime
                    EducationId = SelectedEducation.Id,
                    RoleId = SelectedRole.Id,

                    Login = null,
                    HashPassword = null,
                    Email = null,
                    PhoneNumber = null
                };

                if (SelectedRole.Id == 5)
                {
                    foreach (var checkBox in SubjectCheckBoxes.Where(sb => sb.IsChecked))
                    {
                        teacherDto.SelectedSubjectIds.AddRange(checkBox.SubjectIds);
                        System.Diagnostics.Debug.WriteLine($"Добавлены предметы: {string.Join(",", checkBox.SubjectIds)}");
                    }
                }

                var result = await _teachersService.CreateTeacherAsync(teacherDto);

                StatusMessage = "Готово";

                MessageBox.Show(
                    $"Учитель успешно создан!\n\n" +
                    $"ID: {result.Id}\n" +
                    $"ФИО: {result.LastName} {result.FirstName} {result.Surename}\n" +
                    $"Роль: {result.Role}\n" +
                    $"Образование: {result.Education}",
                    "Успех",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                ClearForm();
            }
            catch (Exception ex)
            {
                StatusMessage = "Ошибка";
                MessageBox.Show($"Ошибка при создании учителя: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearForm()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            Surename = string.Empty;
            DateOfBirth = null;
            SelectedEducation = null;
            SelectedRole = null;

            foreach (var checkBox in SubjectCheckBoxes)
            {
                checkBox.IsChecked = false;
            }
        }

        private void Close(object parameter)
        {
            if (parameter is Window window)
            {
                window.Close();
            }
        }

        private Dictionary<string, List<int>> InitializeSubjectGroups()
        {
            return new Dictionary<string, List<int>>
            {
                ["Бел.яз и лит."] = new List<int> { 10, 11 },
                ["Русск. яз и лит"] = new List<int> { 12, 13 },
                ["Ин.яз"] = new List<int> { 19 },
                ["Матем"] = new List<int> { 9 },
                ["Информ"] = new List<int> { 23 },
                ["История"] = new List<int> { 26, 27, 28, 34 },
                ["Геогр."] = new List<int> { 29, 14 },
                ["Биолог."] = new List<int> { 25 },
                ["Физика"] = new List<int> { 21, 9 },
                ["Химия"] = new List<int> { 22 },
                ["Астр."] = new List<int> { 24 },
                ["Музыка"] = new List<int> { 18, 30 },
                ["Труд. об."] = new List<int> { 16 },
                ["Физ-ра"] = new List<int> { 15, 32 },
                ["Черч."] = new List<int> { 31 }
            };
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}