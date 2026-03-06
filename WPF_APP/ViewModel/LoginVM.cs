using WPF_APP.Commands;
using WPF_APP.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WPF_APP.ViewModel
{
    public class LoginVM : INotifyPropertyChanged
    {
        private readonly AuthService _authService;
        private string _login;
        private string _password;
        private bool _isLoading;

        public string Login
        {
            get => _login;
            set
            {
                if (SetField(ref _login, value))
                {
                    OnPropertyChanged(nameof(CanLogin));
                }
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (SetField(ref _password, value))
                {
                    OnPropertyChanged(nameof(CanLogin));
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (SetField(ref _isLoading, value))
                {
                    OnPropertyChanged(nameof(CanLogin));
                }
            }
        }

        public string LoginButtonContent => IsLoading ? "Вход" : "Войти";
        public bool CanLogin => !IsLoading &&
                               !string.IsNullOrWhiteSpace(Login) &&
                               !string.IsNullOrWhiteSpace(Password);

        public ICommand LoginCommand { get; }
        public System.Action OnLoginSuccess { get; set; }
        public System.Action OnLoginFailed { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public LoginVM()
        {
            _authService = new AuthService();
            LoginCommand = new RelayCommand(async _ => await LoginAsync(), _ => CanLogin);
        }

        private async Task LoginAsync()
        {
            IsLoading = true;

            try
            {
                var result = await _authService.LoginAsync(Login, Password);

                if (result.Success)
                {
                    OnLoginSuccess?.Invoke();
                }
                else
                {
                    var cusmomWindow = new CustomMessageBox("Неправильный Логин/Пароль");
                    cusmomWindow.Show();
                    OnLoginFailed?.Invoke();
                }
            }
            catch (System.Exception ex)
            {
                var cusmomWindow = new CustomMessageBox($"Ошибка сервера: {ex.Message}");
                cusmomWindow.Show();
            }
            finally
            {
                IsLoading = false;
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (System.Collections.Generic.EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}