using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPF_APP.Commands;
using WPF_APP.Model;
using WPF_APP.Services;

namespace WPF_APP.ViewModel
{
    public class RegistrationVM : INotifyPropertyChanged
    {
        private readonly RegService _regService;
        private readonly ReferenceService _referenceService;

        private string _login = string.Empty;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;
        private string _countryCode = "29";
        private string _phoneNumber = string.Empty;
        private string _firstName = string.Empty;
        private string _lastName = string.Empty;
        private DateTime? _dateOfBirth = DateTime.Now.AddYears(-18);
        private int _selectedEducationId = 0;
        private int _selectedRoleId = 0;
        private bool _canGoToNextStep = false;
        private bool _canRegister = false;
        private bool _isRegistering = false;
        private int _currentStep = 1;

        private ObservableCollection<ReferenceItem> _educations = new ObservableCollection<ReferenceItem>();
        private ObservableCollection<ReferenceItem> _roles = new ObservableCollection<ReferenceItem>();

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand RegisterCommand { get; private set; }
        public ICommand NextStepCommand { get; private set; }
        public ICommand PreviousStepCommand { get; private set; }

        public ObservableCollection<ReferenceItem> Educations
        {
            get => _educations;
            set
            {
                _educations = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ReferenceItem> Roles
        {
            get => _roles;
            set
            {
                _roles = value;
                OnPropertyChanged();
            }
        }

        public string Login
        {
            get => _login;
            set
            {
                if (_login != value)
                {
                    _login = value;
                    OnPropertyChanged();
                    ValidateStep1();
                }
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged();
                    ValidateStep1();
                }
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged();
                    ValidateStep1();
                }
            }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                if (_confirmPassword != value)
                {
                    _confirmPassword = value;
                    OnPropertyChanged();
                    ValidateStep1();
                }
            }
        }

        public string CountryCode
        {
            get => _countryCode;
            set
            {
                if (_countryCode != value)
                {
                    _countryCode = value;
                    OnPropertyChanged();
                    ValidateStep1();
                }
            }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                if (_phoneNumber != value)
                {
                    _phoneNumber = value;
                    OnPropertyChanged();
                    ValidateStep1();
                }
            }
        }

        public string FirstName
        {
            get => _firstName;
            set
            {
                if (_firstName != value)
                {
                    _firstName = value;
                    OnPropertyChanged();
                    ValidateStep2();
                }
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                if (_lastName != value)
                {
                    _lastName = value;
                    OnPropertyChanged();
                    ValidateStep2();
                }
            }
        }

        public DateTime? DateOfBirth
        {
            get => _dateOfBirth;
            set
            {
                if (_dateOfBirth != value)
                {
                    _dateOfBirth = value;
                    OnPropertyChanged();
                    ValidateStep2();
                }
            }
        }

        public int SelectedEducationId
        {
            get => _selectedEducationId;
            set
            {
                if (_selectedEducationId != value)
                {
                    _selectedEducationId = value;
                    OnPropertyChanged();
                    ValidateStep2();
                }
            }
        }

        public int SelectedRoleId
        {
            get => _selectedRoleId;
            set
            {
                if (_selectedRoleId != value)
                {
                    _selectedRoleId = value;
                    OnPropertyChanged();
                    ValidateStep2();
                }
            }
        }

        public bool CanGoToNextStep
        {
            get => _canGoToNextStep;
            set
            {
                if (_canGoToNextStep != value)
                {
                    _canGoToNextStep = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool CanRegister
        {
            get => _canRegister;
            set
            {
                if (_canRegister != value)
                {
                    _canRegister = value;
                    OnPropertyChanged();
                    // Обновляем состояние команды
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public bool IsRegistering
        {
            get => _isRegistering;
            set
            {
                if (_isRegistering != value)
                {
                    _isRegistering = value;
                    OnPropertyChanged();
                    // Обновляем состояние команды
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public int CurrentStep
        {
            get => _currentStep;
            set
            {
                if (_currentStep != value)
                {
                    _currentStep = value;
                    OnPropertyChanged();
                    OnPropertyChanged("IsStepOneVisible");
                    OnPropertyChanged("IsStepTwoVisible");
                }
            }
        }

        public bool IsStepOneVisible => CurrentStep == 1;
        public bool IsStepTwoVisible => CurrentStep == 2;

        public RegistrationVM()
        {
            try
            {
                var baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
                if (string.IsNullOrEmpty(baseUrl))
                {
                    throw new InvalidOperationException("Не настроен ApiBaseUrl в конфигурации");
                }

                _regService = new RegService(baseUrl);
                _referenceService = new ReferenceService(baseUrl);
            }
            catch (Exception ex)
            {
                var cusmomWindow = new CustomMessageBox($"Ошибка инициализации: {ex.Message}");
                cusmomWindow.Show();                
                return;
            }

            RegisterCommand = new RelayCommand(
                async (param) => await RegisterAsync(),
                (param) => CanRegister && !IsRegistering);

            NextStepCommand = new RelayCommand(
                (param) => GoToNextStep(),
                (param) => CanGoToNextStep && CurrentStep == 1);

            PreviousStepCommand = new RelayCommand(
                (param) => GoToPreviousStep(),
                (param) => CurrentStep == 2);

            // Загружаем справочные данные
            LoadReferenceDataAsync();
        }

        private async void LoadReferenceDataAsync()
        {

                var referenceData = await _referenceService.GetRegisterReferenceDataAsync();

                if (referenceData != null && referenceData.Educations != null && referenceData.Roles != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Educations = new ObservableCollection<ReferenceItem>(referenceData.Educations);
                        Roles = new ObservableCollection<ReferenceItem>(referenceData.Roles);
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        // Запасной вариант
                        Educations = new ObservableCollection<ReferenceItem>
                        {
                            new ReferenceItem { Id = 1, Name = "Высшее" },
                            new ReferenceItem { Id = 2, Name = "Среднее специальное" }
                        };

                        Roles = new ObservableCollection<ReferenceItem>
                        {
                            new ReferenceItem { Id = 1, Name = "Директор" },
                            new ReferenceItem { Id = 2, Name = "Зам. дир. по осн. деят." },
                            new ReferenceItem { Id = 3, Name = "Зам. дир. курир. нач. обр." }
                        };
                    });
                }
           
        }

        private void ValidateStep1()
        {
            bool isValid = !string.IsNullOrWhiteSpace(Login) &&
                          Login.Length >= 3 &&
                          IsValidEmail(Email) &&
                          !string.IsNullOrWhiteSpace(Password) &&
                          Password.Length >= 6 &&
                          Password == ConfirmPassword &&
                          !string.IsNullOrWhiteSpace(PhoneNumber) &&
                          PhoneNumber.Length == 7 &&
                          Regex.IsMatch(PhoneNumber, @"^\d+$");

            CanGoToNextStep = isValid;
        }

        private void ValidateStep2()
        {
            bool isValid = !string.IsNullOrWhiteSpace(FirstName) &&
                          !string.IsNullOrWhiteSpace(LastName) &&
                          DateOfBirth.HasValue &&
                          DateOfBirth.Value <= DateTime.Now.AddYears(-18) &&
                          SelectedEducationId > 0 &&
                          SelectedRoleId > 0;

            CanRegister = isValid;
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        private void GoToNextStep()
        {
            CurrentStep = 2;
        }

        private void GoToPreviousStep()
        {
            CurrentStep = 1;
        }

        private async Task RegisterAsync()
        {
            IsRegistering = true;

            try
            {
                string dateOfBirthString = string.Empty;
     
                var request = new RegisterRequest
                {
                    Login = Login,
                    Email = Email,
                    Password = Password,
                    ConfirmPassword = ConfirmPassword,
                    FirstName = FirstName,
                    LastName = LastName,
                    Surename = string.Empty,
                    PhoneNumber = $"+375{CountryCode}{PhoneNumber}",
                    DateOfBirth = dateOfBirthString,
                    EducationId = SelectedEducationId, 
                    RoleId = SelectedRoleId            
                };

                var result = await _regService.RegisterAsync(request);

                if (result.IsSuccess)
                {
                    var cusmomWindow = new CustomMessageBox("Регистрация прошла успешно!");
                    cusmomWindow.Show();
                    CloseWindow();
                }
                else
                {

                    var cusmomWindow = new CustomMessageBox("Сервер недоступен");
                    cusmomWindow.Show();
                }
            }
            catch (Exception ex)
            {
                var cusmomWindow = new CustomMessageBox($"Произошла ошибка: {ex.Message}");
                cusmomWindow.Show();               
            }
            finally
            {
                IsRegistering = false;
            }
        }

        private void CloseWindow()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var window = Application.Current.Windows
                    .OfType<Window>()
                    .FirstOrDefault(w => w.DataContext == this);
                if (window != null)
                {
                    window.Close();
                }
            });
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}