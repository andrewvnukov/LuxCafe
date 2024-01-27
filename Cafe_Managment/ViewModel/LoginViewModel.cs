using Cafe_Managment.Repositories;
using Cafe_Managment.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Policy;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Cafe_Managment.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private string _username;
        private SecureString _password;
        private string _loginerrorMessage;
        private string _passworderrorMessage;
        private bool _isViewVisible=true;
        private bool _isEnabled = true;
        
        private int Userid;

        protected IUserRepository userRepository;

        public string  Username { get { return _username; }
            set { _username = value; OnPropertyChanged(nameof(Username)); } }
        public SecureString Password { get { return _password; }
            set { _password = value; OnPropertyChanged(nameof(Password)); } }
        public string LoginErrorMessage
        {
            get { return _loginerrorMessage; }
            set { _loginerrorMessage = value; OnPropertyChanged(nameof(LoginErrorMessage)); }
        }
        public string PasswordErrorMessage
        {
            get { return _passworderrorMessage; }
            set { _passworderrorMessage = value;OnPropertyChanged(nameof(PasswordErrorMessage)); }
        }
        public bool IsViewVisible
        {
            get { return _isViewVisible; }
            set
            {
                _isViewVisible = value; OnPropertyChanged(nameof(IsViewVisible));
            }
        }
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value; OnPropertyChanged(nameof(IsEnabled));
            }
        }

        public ICommand LoginCommand { get; set; }
        public ICommand ShowPasswordCommand { get; set; }

        public ICommand CloseAppCommand { get; set; }

        public LoginViewModel()
        {

            userRepository = new UserRepository();
            LoginCommand = new RelayCommand(ExecuteLoginCommand,CanExecuteLoginCommand);
            CloseAppCommand = new RelayCommand(ExecuteCloseAppCommand);
        }

        private void ExecuteCloseAppCommand(object obj)
        {
            IsEnabled = false;
            System.Windows.Application.Current.Shutdown();
        }

        private bool CanExecuteLoginCommand(object arg)
        {
            bool DataValid;

            if (string.IsNullOrWhiteSpace(Username) || Username.Length <= 3 || Username.Length >=45
                || Password == null || Password.Length <= 3 || Password.Length >= 45)
                DataValid = false;
            else DataValid= true;

            return DataValid;
        }

        private void ExecuteLoginCommand(object obj)
        {
            LoginErrorMessage = "";
            PasswordErrorMessage = "";
            var isValidUser = userRepository.AuthenticateUser(new System.Net.NetworkCredential(Username, Password), out Userid);
            switch (isValidUser) 
            {   
                case 0:
                    Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(Userid.ToString()), null);
                    IsViewVisible = false;
                    break;
                case 1:
                    LoginErrorMessage = "Сотрудник уволен";
                    PasswordErrorMessage = "Сотрудник уволен";
                    break;
                case 2:
                    LoginErrorMessage = "Неправильный логин";
                    break;
                case 3:
                    PasswordErrorMessage = "Неправильный пароль";
                    break;
            }
        }
    }
}
