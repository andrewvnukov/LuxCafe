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



        public ICommand HireCommand {  get; set; }
        public ICommand FireCommand { get; set; }

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
            Employees = userRepository.GetByAll();

            HireCommand = new RelayCommand(ExecuteHireCommand, CanExecuteHireCommand);
            FireCommand = new RelayCommand(ExecuteFireCommand);
        }

        private void ExecuteFireCommand(object obj)
        {
            int temp = int.Parse(Employees.Rows[SelectedEmployee][0].ToString());


            if (MessageBoxResult.Yes== MessageBox.Show("Вы уверены что хотите уволить сотрудника?",
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
            };
        }
    }
    
}
