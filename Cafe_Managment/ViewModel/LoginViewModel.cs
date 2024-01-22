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
        private string _password;
        private string _errorMessage;
        private bool _isViewVisible=true;

        protected IUserRepository userRepository;

        public string  Username { get { return _username; }
            set { _username = value; OnPropertyChanged(nameof(Username)); } }
        public string Password { get { return _password; }
            set { _password = value; OnPropertyChanged(nameof(Password)); } }
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; OnPropertyChanged(nameof(ErrorMessage)); }
        }
        public bool IsViewVisible
        {
            get { return _isViewVisible; }
            set
            {
                _isViewVisible = value; OnPropertyChanged(nameof(IsViewVisible));
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
            System.Windows.Application.Current.Shutdown();
        }

        private bool CanExecuteLoginCommand(object arg)
        {
            bool DataValid;

            if (string.IsNullOrWhiteSpace(Username) || Username.Length <= 3
                || Password == null || Password.Length <= 3)
                DataValid = false;
            else DataValid= true;

            return DataValid;
        }

        private void ExecuteLoginCommand(object obj)
        {
            var isValidUser = userRepository.AuthenticateUser(new System.Net.NetworkCredential(Username, Password));
            if (isValidUser)
            {
                Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(Username), null);
                IsViewVisible = false;
            }
        }
    }
}
