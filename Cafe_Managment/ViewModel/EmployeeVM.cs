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

namespace Cafe_Managment.ViewModel
{
    internal class EmployeeVM : ViewModelBase
    {
        UserRepository userRepository;
        private DataTable _employees;
        private int _selectedEmployee;
        private bool _isReadOnly;
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

        public int SelectedEmployee
        {
            get { return _selectedEmployee; } 
            set 
            {
                _selectedEmployee = value;
                OnPropertyChanged(nameof(SelectedEmployee));
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

            
            temp = userRepository.GetByAll();

            Employees = temp.Copy();

            Employees.Columns.Remove("Id");

            IsReadOnly = false;

            SelectedEmployee = -1;

            HireCommand = new RelayCommand(ExecuteHireCommand, CanExecuteHireCommand);
            FireCommand = new RelayCommand(ExecuteFireCommand, CanExecuteFireCommand);
            EditCommand = new RelayCommand(ExecuteEditCommand);
            SaveCommand = new RelayCommand(ExecuteSaveCommand);
            InfoCommand = new RelayCommand(ExecuteInfoCommand);
        }

        private void ExecuteSaveCommand(object obj)
        {
           MessageBox.Show(temp.Rows[SelectedEmployee][1].ToString());
        }

        private void ExecuteEditCommand(object obj)
        {
            IsReadOnly = !IsReadOnly;
        }

        private void ExecuteInfoCommand(object obj)
        {
            MessageBox.Show("Изменению подлежат только следующие поля:\n" +
                "Имя, Фамилия, Отчество, Почта, Номер телефона.\n" +
                "Остальные данные изменены не будут!!!",
                "Внимание!!!", MessageBoxButton.YesNoCancel);
        }

        private bool CanExecuteFireCommand(object arg)
        {
            return !(SelectedEmployee == -1 || SelectedEmployee >= Employees.Rows.Count);
        }

        private void ExecuteFireCommand(object obj)
        {
            int temp = int.Parse(Employees.Rows[SelectedEmployee][0].ToString());

            MessageBox.Show(SelectedEmployee.ToString());

            if (MessageBoxResult.Yes== MessageBox.Show($"Вы уверены что хотите уволить сотрудника\nПод номером {temp}?",
                "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.None))
            {
                if (temp == UserData.Id)
                {
                    MessageBox.Show("Извините, но вы не можете уволить себя","Ошибка");
                }
                else
                {
                    userRepository.FireEmployee(temp);
                    MessageBox.Show("Сотрудник уволен!","Кто-то остался без чая...");
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
                Employees = userRepository.GetByAll();
            };


        }
    }
    
}
