using Cafe_Managment.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Cafe_Managment.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private string _username;
        private SecureString _password;
        private string _errorMessage;
        private bool _isViewVisible;

        public string  Username { get { return _username; }
            set { _username = value; OnPropertyChanged(nameof(Username)); } }
        public SecureString Password { get { return _password; }
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

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(ExecuteLoginCommand,CanExecuteLoginCommand);
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
            
        }
    }
}
