using Cafe_Managment.Repositories;
using Cafe_Managment.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using ToastNotifications;
using ToastNotifications.Messages;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Data;

namespace Cafe_Managment.ViewModel
{
    public class LoginVM : ViewModelBase
    {
        private Notifier _notifier;

        private string _username;
        private SecureString _password;
        private string _loginerrorMessage;
        private string _passworderrorMessage;

        private bool _isRemember = true;
        private bool _isViewVisible = true;

        protected IUserRepository userRepository;

        public bool IsViewVisible
        {
            get { return _isViewVisible; }
            set { _isViewVisible = value; OnPropertyChanged(nameof(IsViewVisible)); }
        }


        public string Username
        {
            get { return _username; }
            set { _username = value; OnPropertyChanged(nameof(Username)); }
        }
        public SecureString Password
        {
            get { return _password; }
            set { _password = value; OnPropertyChanged(nameof(Password)); }
        }
        public string LoginErrorMessage
        {
            get { return _loginerrorMessage; }
            set { _loginerrorMessage = value; OnPropertyChanged(nameof(LoginErrorMessage)); }
        }
        public string PasswordErrorMessage
        {
            get { return _passworderrorMessage; }
            set { _passworderrorMessage = value; OnPropertyChanged(nameof(PasswordErrorMessage)); }
        }


        public bool IsRemember
        {
            get { return _isRemember; }
            set { _isRemember = value; OnPropertyChanged(nameof(IsRemember)); }
        }


        public ICommand LoginCommand { get; set; }
        public ICommand ShowPasswordCommand { get; set; }

        public ICommand CloseAppCommand { get; set; }

        public LoginVM()
        {
            _notifier = CreateNotifier();


            userRepository = new UserRepository();

            LoginCommand = new RelayCommand(ExecuteLoginCommand, CanExecuteLoginCommand);
            CloseAppCommand = new RelayCommand(ExecuteCloseAppCommand);
        }

        private Notifier CreateNotifier()
        {
            return new Notifier(cfg =>
            {
                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    TimeSpan.FromSeconds(5),
                    MaximumNotificationCount.FromCount(5));

                cfg.PositionProvider = new PrimaryScreenPositionProvider(
                    corner: Corner.BottomRight,
                    offsetX: 10,
                    offsetY: 10);

                cfg.DisplayOptions.TopMost = true;
                cfg.DisplayOptions.Width = 300;

                cfg.Dispatcher = Application.Current.Dispatcher;
            });
        }
        private void ExecuteCloseAppCommand(object obj)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private bool CanExecuteLoginCommand(object arg)
        {
            bool DataValid;

            if (string.IsNullOrWhiteSpace(Username) || Username.Length <= 3 || Username.Length >= 45
                || Password == null || Password.Length <= 3 || Password.Length >= 45)
                DataValid = false;
            else DataValid = true;

            return DataValid;
        }

        private async void ExecuteLoginCommand(object obj)
        {
            try
            {
                var credential = new System.Net.NetworkCredential(Username, Password);

                // Сначала проверим, не уволен ли пользователь
                var dismissedData = await Task.Run(() => userRepository.GetDismissedEmployees());
                var isDismissed = dismissedData?.AsEnumerable()
                                   .Any(row => row.Field<string>("Login") == Username) ?? false;

                if (isDismissed)
                {
                    _notifier.ShowInformation("Данный сотрудник был уволен.");
                    return;
                }

                // Если не уволен, продолжаем аутентификацию
                var result = await Task.Run(() => userRepository.AuthenticateUser(credential));

                switch (result)
                {
                    case 0: // Успешная аутентификация
                        if (IsRemember)
                        {
                            await Task.Run(userRepository.RememberCurrentUser);
                        }
                        IsViewVisible = false;
                        break;

                    case 2: // Неправильный логин
                        _notifier.ShowWarning("Неправильный логин или пароль. Пожалуйста, попробуйте снова.");
                        break;

                    case 3: // Неправильный пароль
                        _notifier.ShowWarning("Неправильный логин или пароль. Пожалуйста, попробуйте снова.");
                        break;

                    default:
                        _notifier.ShowError("Произошла непредвиденная ошибка при аутентификации.");
                        break;
                }
            }
            catch (Exception ex)
            {
                _notifier.ShowError($"Произошла ошибка при аутентификации: {ex.Message}"); // Обработка исключений
            }
        }


    }
}

