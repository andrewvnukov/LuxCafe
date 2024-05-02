using Cafe_Managment.Model;
using Cafe_Managment.Repositories;
using Cafe_Managment.Utilities;
using Cafe_Managment.View.DialogWindows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ToastNotifications;
using ToastNotifications.Messages;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using System.Diagnostics;

namespace Cafe_Managment.ViewModel
{
    internal class EmployeeVM : ViewModelBase
    {
        private Notifier _notifier;

        UserRepository userRepository;
        private DataTable _employees;
        private int _selectedEmployee;
        private bool _isReadOnly;
        private object _selectedEmployeeItem;
        private DataTable _dismissedEmployees;


        DataTable temp = new DataTable();


        public ICommand HireCommand { get; set; }
        public ICommand FireCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand InfoCommand { get; set; }

        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set
            {
                _isReadOnly = value;
                OnPropertyChanged(nameof(IsReadOnly));
            }
        }
        public DataTable DismissedEmployees
        {
            get { return _dismissedEmployees; }
            set { _dismissedEmployees = value; }
        }

        public int SelectedEmployee
        {
            get { return _selectedEmployee; } 
            set 
            {
                _selectedEmployee = value;
                OnPropertyChanged(nameof(SelectedEmployee));
            } 
        }

        public object SelectedEmployeeItem
        {
            get { return _selectedEmployeeItem; }
            set
            {
                _selectedEmployeeItem = value;
                OnPropertyChanged(nameof(SelectedEmployeeItem));
            }
        }

        public DataTable Employees
        {
            get { return _employees; }
            set { _employees = value; }
        }

        public EmployeeVM()
        {
            userRepository = new UserRepository();
            _notifier = CreateNotifier();


            temp = userRepository.GetByAll();
            Employees = temp.Copy();
            Employees.Columns.Remove("Id");

            IsReadOnly = true;

            SelectedEmployee = -1;

            DismissedEmployees = userRepository.GetDismissedEmployees();
            IsReadOnly = true;

            HireCommand = new RelayCommand(ExecuteHireCommand, CanExecuteHireCommand);
            FireCommand = new RelayCommand(ExecuteFireCommand, CanExecuteFireCommand);
            EditCommand = new RelayCommand(ExecuteEditCommand);
            SaveCommand = new RelayCommand(ExecuteSaveCommand);
            InfoCommand = new RelayCommand(ExecuteInfoCommand);
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

        private void ExecuteSaveCommand(object obj)
        {
            // Проверка, что SelectedEmployeeItem не равен null
            if (SelectedEmployeeItem == null)
            {
                _notifier.ShowError("Нет выбранного сотрудника.");
                return;
            }

            DataRowView dataRowView = SelectedEmployeeItem as DataRowView;

            if (dataRowView == null || dataRowView.Row == null) // Дополнительная проверка
            {
                _notifier.ShowError("Некорректные данные.");
                return;
            }

            int EmpId;
            if (!int.TryParse(dataRowView.Row[0].ToString(), out EmpId))
            {
                _notifier.ShowError("Некорректный идентификатор сотрудника.");
                return;
            }

            // Создаем новый объект с данными сотрудника
            EmpData newdata = new EmpData
            {
                Id = EmpId,
                Name = dataRowView.Row[3].ToString(),
                Surname = dataRowView.Row[4].ToString(),
                Patronomic = dataRowView.Row[5].ToString(),
                PhoneNumber = dataRowView.Row[6].ToString(),
                Email = dataRowView.Row[7].ToString(),
                Address = dataRowView.Row[9].ToString(),
            };

            userRepository.UpdateEmployee(newdata); // Обновление данных сотрудника

            temp = userRepository.GetByAll(); // Обновляем данные таблицы
            Employees = temp.Copy();
            Employees.Columns.Remove("Id");

            // Проверяем, если измененный сотрудник является текущим пользователем
            if (EmpId == UserData.Id)
            {
                userRepository.GetById();
            }

            // Конкатенируем фамилию, имя и отчество
            string fullName = $"{dataRowView.Row[4].ToString()} {dataRowView.Row[3].ToString()} {dataRowView.Row[5].ToString()}";

            _notifier.ShowSuccess($"Данные сотрудника {fullName} успешно изменены!");
            IsReadOnly = true;
        }


        private void ExecuteEditCommand(object obj)
        {
            IsReadOnly = !IsReadOnly;
        }

        private void ExecuteInfoCommand(object obj)
        {
            MessageBox.Show("Изменению подлежат только следующие поля:\n" +
                "Имя, Фамилия, Отчество, Почта, Номер телефона и адрес.\n" +
                "Остальные данные изменены не будут!!!",
                "Внимание!!!", MessageBoxButton.YesNoCancel);
        }

        private bool CanExecuteFireCommand(object arg)
        {
            return !(SelectedEmployee == -1 || SelectedEmployee >= Employees.Rows.Count);
        }

        private void ExecuteFireCommand(object obj)
        {
            DataRowView dataRowView = SelectedEmployeeItem as DataRowView;

            int temp = int.Parse(Employees.Rows[SelectedEmployee][0].ToString());
            string fullName = $"{dataRowView.Row[4].ToString()} {dataRowView.Row[3].ToString()} {dataRowView.Row[5].ToString()}";

            if (MessageBoxResult.Yes== MessageBox.Show($"Вы уверены что хотите уволить сотрудника\nПод номером {temp}?",
                "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.None))
            {
                if (temp == UserData.Id)
                {
                    _notifier.ShowError("Извините, но вы не можете уволить себя");
                }
                else
                {
                    userRepository.FireEmployee(temp);
                    _notifier.ShowSuccess($"Сотрудник {fullName} уволен.");
                }
            }
            
        }

        private bool CanExecuteHireCommand(object arg)
        {
            bool istrue = false;
            if (UserData.RoleId == 1 ||
                UserData.RoleId == 2 ||
                UserData.RoleId == 3 ||
                UserData.RoleId == 4) istrue = true;
            return istrue;
        }

        private void ExecuteHireCommand(object obj)
        {
            Registration registration = new Registration();
            registration.ShowDialog();
            registration.IsVisibleChanged += (s, ev) =>
            {
                registration.Close();
                temp = userRepository.GetByAll();
                Employees = temp.Copy();
                Employees.Columns.Remove("Id");
                _notifier.ShowSuccess("Сотрудник успешно нанят!");

            };
        }
    }
    
}
