using Cafe_Managment.Model;
using Cafe_Managment.Repositories;
using Cafe_Managment.Utilities;
using Cafe_Managment.View.DialogWindows.RegisterForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cafe_Managment.ViewModel.DialogWindowsVM
{
    public class RegistrationVM : ViewModelBase
    {
        private bool _isViewVisible = true;
        private object _activePage;
        private EmpData _newEmp = new EmpData();
        private object FirstPage = new RegisterFirst();
        private object SecondPage = new RegisterSecond();

        public List<string> RoleTable { get; set; }


        public bool IsViewVisible
        {
            get { return _isViewVisible; }
            set { _isViewVisible = value; OnPropertyChanged(); }
        }

        public object ActivePage
        {
            get => _activePage;
            set
            {
                _activePage = value;
                OnPropertyChanged();
            }
        }
        public EmpData NewEmp
        {
            get { return _newEmp; }
            set
            {
                _newEmp.Name = value.Name;
                _newEmp.Surname = value.Surname;
                _newEmp.Patronomic = value.Patronomic;
                _newEmp.BirthDay = value.BirthDay;
                _newEmp.Login = value.Login;
                _newEmp.Password = value.Password;
                _newEmp.Role = value.Role;
                _newEmp.Passport = value.Role;
                OnPropertyChanged(nameof(NewEmp));
            }
        }

        public ICommand CloseWindowCommand { get; set; }
        public ICommand RegisterCommand { get; set; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }

        public RegistrationVM() 
        {
            UserRepository userRepository = new UserRepository();
            RoleTable = userRepository.GetRoles();

            NewEmp = new EmpData();

            ActivePage = new RegisterFirst();

            CloseWindowCommand = new RelayCommand(ExecuteCloseWindowCommand);
            NextPageCommand = new RelayCommand(ExecuteNextPageCommand);
            PreviousPageCommand = new RelayCommand(ExecutePreviousPageCommand);
            RegisterCommand = new RelayCommand(ExecuteRegisterCommand);
        }

        

        private void ExecuteRegisterCommand(object obj)
        {
            MessageBox.Show($"{_newEmp.Name}\n{_newEmp.Surname}\n{_newEmp.Patronomic}\n{_newEmp.BirthDay}\n{_newEmp.Login}\n{_newEmp.Role}\n");
            IsViewVisible = false;
        }
        private void ExecutePreviousPageCommand(object obj)
        {
            ActivePage = FirstPage;
        }
        private void ExecuteNextPageCommand(object obj)
        {
            ActivePage = SecondPage;
        }

        private void ExecuteCloseWindowCommand(object obj)
        {
            IsViewVisible = false;
        }


    }
}
