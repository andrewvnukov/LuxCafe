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
        UserRepository userRepository;
        private bool _isViewVisible = true;
        private object _activePage;
        private EmpData _newEmp = new EmpData();
        private object FirstPage = new RegisterFirst();
        private object SecondPage = new RegisterSecond();
        private string _loginerrorMessage;
        private string _birtherrorMessage;



        public List<string> RoleTable { get; set; }
        public List<string> BranchTable { get; set; }

        public string LoginErrorMessage
        {
            get { return _loginerrorMessage; }
            set { _loginerrorMessage = value; OnPropertyChanged(nameof(LoginErrorMessage)); }
        }
        public string BirthErrorMessage
        {
            get { return _birtherrorMessage; }
            set { _birtherrorMessage = value; OnPropertyChanged(nameof(BirthErrorMessage)); }
        }

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
                _newEmp.Branch = value.Branch;
                OnPropertyChanged(nameof(NewEmp));
            }
        }

        public ICommand CloseWindowCommand { get; set; }
        public ICommand RegisterCommand { get; set; }
        public ICommand NextPageCommand { get; }
        public ICommand PreviousPageCommand { get; }

        public RegistrationVM() 
        {
            userRepository = new UserRepository();
            RoleTable = userRepository.GetRoles();
            RoleTable.Add("Выберите роль");
            BranchTable = userRepository.GetBranches();
            BranchTable.Add("Выберите филиал");
            LoginErrorMessage = "";
            BirthErrorMessage = "";

            NewEmp = new EmpData
            {
                Name = "",
                Surname = "",
                Patronomic = "",
                BirthDay = "",
                Login = "",
                Password = "",
                Role = RoleTable.Count()-1,
                Branch = BranchTable.Count()-1,
            };

            ActivePage = new RegisterFirst();

            CloseWindowCommand = new RelayCommand(ExecuteCloseWindowCommand);
            NextPageCommand = new RelayCommand(ExecuteNextPageCommand, CanExecuteNextCommand);
            PreviousPageCommand = new RelayCommand(ExecutePreviousPageCommand);
            RegisterCommand = new RelayCommand(ExecuteRegisterCommand, CanExecuteRegisterCommand);
           
        }

        
        private void ExecuteRegisterCommand(object obj)
        {
            LoginErrorMessage = "";
            
            if (userRepository.IfUserExists(NewEmp.Login))
            {
                LoginErrorMessage = "Данный пользователь уже существует";
            }
            else
            {
                userRepository.Add(NewEmp);
                MessageBox.Show($"Новый пользователь с логином {NewEmp.Login} был добавлен!");
                IsViewVisible = false;
            }
        }
        private bool CanExecuteRegisterCommand(object arg)
        {

            return _newEmp.Login.Length > 0 
                && _newEmp.Password.Length > 0 
                && _newEmp.Role != RoleTable.Count-1
                && _newEmp.Branch != BranchTable.Count - 1;

        }

        private void ExecutePreviousPageCommand(object obj)
        {
            ActivePage = FirstPage;

        }
        private void ExecuteNextPageCommand(object obj)
        {
            try { 
                DateTime.Parse(NewEmp.BirthDay);
                ActivePage = SecondPage;
                BirthErrorMessage = "";
            }
            catch
            {
                BirthErrorMessage = "Введите корректную дату";
            };
        }
        private bool CanExecuteNextCommand(object arg)
        {
            return _newEmp.Name.Length > 0 && _newEmp.Surname.Length > 0 
                && _newEmp.Patronomic.Length > 0 && _newEmp.BirthDay.Length >= 6;
        }


        private void ExecuteCloseWindowCommand(object obj)
        {
            IsViewVisible = false;
        }


    }
}
