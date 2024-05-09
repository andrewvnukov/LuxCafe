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
        private int _selectedEmployee;
        private bool _isReadOnly;
        private object _selectedEmployeeItem;


        DataTable temp = new DataTable();
        DataTable tempdel = new DataTable();
        


        public ICommand HireCommand { get; set; }
        public ICommand FireCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand InfoCommand { get; set; }
        public ICommand EditPhotoCommand { get; set; }

        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set
            {
                _isReadOnly = value;
                OnPropertyChanged(nameof(IsReadOnly));
            }
        }
     
        private DataTable _dismissedEmployees;

        public DataTable DismissedEmployees

        {
            get => _dismissedEmployees;
            set
            {
                if (_dismissedEmployees != value) // Проверяем, изменилось ли значение
                {
                    _dismissedEmployees = value;
                    OnPropertyChanged(nameof(DismissedEmployees)); // Уведомляем об изменении
                }
            }
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

        private DataTable _employees;

        public DataTable Employees

        {
            get => _employees;
            set
            {
                if (_employees != value) // Проверяем, изменилось ли значение
                {
                    _employees = value;
                    OnPropertyChanged(nameof(Employees)); // Уведомляем об изменении
                }
            }
        }

        public EmployeeVM()
        {
            userRepository = new UserRepository();
            _notifier = CreateNotifier();


            temp = userRepository.GetByAll();
            Employees = temp.Copy();
            Employees.Columns.Remove("Id");

            tempdel = userRepository.GetDismissedEmployees();
            DismissedEmployees = tempdel.Copy();



            IsReadOnly = true;

            SelectedEmployee = -1;

            DismissedEmployees = userRepository.GetDismissedEmployees();
            IsReadOnly = true;

            HireCommand = new RelayCommand(ExecuteHireCommand, CanExecuteHireCommand);
            FireCommand = new RelayCommand(ExecuteFireCommand, CanExecuteFireCommand);
            EditCommand = new RelayCommand(ExecuteEditCommand);
            SaveCommand = new RelayCommand(ExecuteSaveCommand);
            InfoCommand = new RelayCommand(ExecuteInfoCommand);
            EditPhotoCommand = new RelayCommand(ExecuteEditPhotoCommand);
        }

        private void ExecuteEditPhotoCommand(object obj)
        {
            throw new NotImplementedException();
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
                Login = dataRowView.Row[8].ToString(),
                Surname = dataRowView.Row[4].ToString(),
                Patronomic = dataRowView.Row[5].ToString(),
                PhoneNumber = dataRowView.Row[6].ToString(),
                Email = dataRowView.Row[7].ToString(),
                Address = dataRowView.Row[9].ToString(),
            };
            Debug.WriteLine(dataRowView.Row[1], "1");
            Debug.WriteLine(dataRowView.Row[2], "2");
            Debug.WriteLine(dataRowView.Row[3], "3");
            Debug.WriteLine(dataRowView.Row[4], "4");
            Debug.WriteLine(dataRowView.Row[5], "5");
            Debug.WriteLine(dataRowView.Row[6], "6");
            Debug.WriteLine(dataRowView.Row[7], "7");
            Debug.WriteLine(dataRowView.Row[8], "8");
            Debug.WriteLine(dataRowView.Row[9], "9");
            Debug.WriteLine(dataRowView.Row[10], "10");

            userRepository.UpdateEmployee(newdata); // Обновление данных сотрудника

            RefreshAll();


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
                    DismissedEmployees = userRepository.GetDismissedEmployees();
                    RefreshAll();

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
                RefreshAll();
                _notifier.ShowSuccess("Сотрудник успешно зарегистрирован!");

            };
        }
        private void RefreshAll()
        {
            temp = userRepository.GetByAll();
            Employees = temp.Copy();
            Employees.Columns.Remove("Id");
            DismissedEmployees = userRepository.GetDismissedEmployees();

            temp = userRepository.GetByAll();
            DataTable dtEmp = temp.Copy();
            dtEmp.Columns.Remove("Id");
            Employees = dtEmp.Copy();

            tempdel = userRepository.GetDismissedEmployees();
            DataTable dtEmpdel = tempdel.Copy();
            DismissedEmployees = dtEmp.Copy();
        }

    }

}
