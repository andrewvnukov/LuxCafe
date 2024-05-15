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
using ToastNotifications;
using ToastNotifications.Messages;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Cafe_Managment.ViewModel.DialogWindowsVM
{
    public class RegistrationVM : ViewModelBase
    {

        UserRepository userRepository;
        private Notifier _notifier;

        DateTime _dateOfBirth;
        private bool _isViewVisible = true;
        private object _activePage;
        private EmpData _newEmp = new EmpData();
        private object FirstPage = new RegisterFirst();
        private object SecondPage = new RegisterSecond();
        private string _loginerrorMessage;
        private string _birtherrorMessage;



        public List<string> RoleTable { get; set; }
        public List<string> BranchTable { get; set; }


        public DateTime DateOfBirth
        {
            get { return _dateOfBirth; }
            set { _dateOfBirth = value;
                OnPropertyChanged(nameof(DateOfBirth));
            }
        }
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
            set { _isViewVisible = value; OnPropertyChanged(nameof(IsViewVisible)); }
        }

        public object ActivePage
        {
            get => _activePage;
            set
            {
                _activePage = value;
                OnPropertyChanged(nameof(ActivePage));
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
        public ICommand GeneratePasswordCommand { get; }


        public RegistrationVM() 
        {
            _notifier = CreateNotifier();

            DateOfBirth = DateTime.Now;
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
            GeneratePasswordCommand = new RelayCommand(ExecuteGeneratePassword);

        }

        private void ExecuteGeneratePassword(object obj)
        {
            string newPassword = PasswordGenerator.GenerateSecurePassword();
            NewEmp.Password = newPassword; 
            OnPropertyChanged(nameof(NewEmp.Password));
            _notifier.ShowWarning("Перед завершением регистрации, убедитесь, что сотрудник сохранил себе пароль!");
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
        private string GetFullName(EmpData emp)
        {
            return $"{emp.Surname} {emp.Name} {emp.Patronomic}"; // Формирование строки ФИО
        }

        private void ExecuteRegisterCommand(object obj)
        {
            // Проверка, соответствует ли логин требованиям (английские буквы и отсутствие пробелов)
            string loginPattern = "^(?=.*[a-zA-Z])[a-zA-Z0-9]+$";
            Regex loginRegex = new Regex(loginPattern);

            if (!loginRegex.IsMatch(NewEmp.Login))
            {
                _notifier.ShowWarning("Логин должен включать в себя только буквы латинского алфавита и не должен состоять исключительно из цифр.");
                return;
            }

            // Проверка длины логина (не менее 5 символов)
            if (NewEmp.Login.Length < 5)
            {
                _notifier.ShowWarning("Длина логина должна быть не минее 5 символов.");
                return;
            }

            // Проверка, существует ли пользователь с таким логином
            if (userRepository.IfUserExists(NewEmp.Login))
            {
                _notifier.ShowError("Данный пользователь уже существует!");
                return;
            }

            // Проверка длины пароля
            if (NewEmp.Password.Length < 8)
            {
                _notifier.ShowWarning("Пароль должен состоять минимум из 8 символов.");
                return;
            }

            // Проверка, содержит ли пароль минимум одну заглавную букву и один специальный символ
            string passwordPattern = @"^(?=.*[A-Z])(?=.*[!@#$%^&*()_+=\[{\]};:<>|./?,-]).*$";
            Regex passwordRegex = new Regex(passwordPattern);

            if (!passwordRegex.IsMatch(NewEmp.Password))
            {
                _notifier.ShowWarning("Пароль должен содержать минимум одну заглавную букву и один специальный символ.");
                return;
            }

            // Если все проверки пройдены, добавляем пользователя
            userRepository.Add(NewEmp);
            string fullName = GetFullName(NewEmp);

            _notifier.ShowSuccess($"Сотрудник \"{fullName}\" был успешно зарегистрирован под логином {NewEmp.Login}!");
            IsViewVisible = false; // Закрыть окно регистрации
            //RefreshAll();
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
        private bool IsValidName(string name)
        {
            string namePattern = "^[a-zA-Zа-яА-ЯёЁ/s]+$"; // Регулярное выражение для ФИО
            Regex nameRegex = new Regex(namePattern);

            return nameRegex.IsMatch(name); // Проверяем, соответствует ли шаблон
        }

        private void ExecuteNextPageCommand(object obj)
        {
            // Проверка имени
            if (!IsValidName(NewEmp.Name))
            {
                _notifier.ShowWarning("Имя не должно содержать цифры, пробелы или специальные символы.");
                return ; // Прекращаем выполнение
            }

            // Проверка фамилии
            if (!IsValidName(NewEmp.Surname))
            {
                _notifier.ShowWarning("Фамилия не должна содержать цифры, пробелы или специальные символы.");
                return; // Прекращаем выполнение
            }

            // Проверка отчества
            if (!IsValidName(NewEmp.Patronomic))
            {
                _notifier.ShowWarning("Отчество не должно содержать цифры, пробелы или специальные символы.");
                return; // Прекращаем выполнение
            }

            // Проверка на возрастной ценз
            if (IsMoreSixteen())
            {
                NewEmp.BirthDay = DateOfBirth.ToString("yyyy-MM-dd");
                ActivePage = SecondPage;
            }
            else
            {
                _notifier.ShowWarning("Сотрудник младше 16 лет и не может быть нанят.");
            }
        }

        private bool IsMoreSixteen()
        {
            int age = DateTime.Today.Year - DateOfBirth.Year;
            if (DateOfBirth > DateTime.Today.AddYears(-age))
                age--;
            return age >= 16;
        }

        private bool CanExecuteNextCommand(object arg)
        {
            return !string.IsNullOrEmpty(_newEmp.Name.Trim()) 
            && !string.IsNullOrWhiteSpace(_newEmp.Surname)
            && !string.IsNullOrWhiteSpace(_newEmp.Patronomic)
            && DateOfBirth != null;
        }


        private void ExecuteCloseWindowCommand(object obj)
        {
            IsViewVisible = false;
        }


    }
}
