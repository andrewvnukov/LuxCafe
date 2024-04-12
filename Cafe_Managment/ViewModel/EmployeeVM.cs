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
        private object _selectedEmployeeItem;

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

            
            temp = userRepository.GetByAll();
            Employees = temp.Copy();
            Employees.Columns.Remove("Id");

            IsReadOnly = true;

            SelectedEmployee = -1;

            HireCommand = new RelayCommand(ExecuteHireCommand, CanExecuteHireCommand);
            FireCommand = new RelayCommand(ExecuteFireCommand, CanExecuteFireCommand);
            EditCommand = new RelayCommand(ExecuteEditCommand);
            SaveCommand = new RelayCommand(ExecuteSaveCommand);
            InfoCommand = new RelayCommand(ExecuteInfoCommand);
        }

        private void ExecuteSaveCommand(object obj)
        {
            DataRowView dataRowView = SelectedEmployeeItem as DataRowView;

            int EmpId = int.Parse(temp.Rows[int.Parse(dataRowView.Row[0].ToString()) - 1][1].ToString());
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
            
            userRepository.UpdateEmployee(newdata);

            temp = userRepository.GetByAll();
            Employees = temp.Copy();
            Employees.Columns.Remove("Id");

            if (EmpId == UserData.Id)
            {
                userRepository.GetById();
            }
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
            int temp = int.Parse(Employees.Rows[SelectedEmployee][0].ToString());

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
                temp = userRepository.GetByAll();
                Employees = temp.Copy();
                Employees.Columns.Remove("Id");
            };


        }
    }
    
}
