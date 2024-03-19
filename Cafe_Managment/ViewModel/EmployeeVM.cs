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
        public ICommand EditCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand InfoCommand { get; set; }


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

            SelectedEmployee = -1;

            HireCommand = new RelayCommand(ExecuteHireCommand, CanExecuteHireCommand);
            FireCommand = new RelayCommand(ExecuteFireCommand, CanExecuteFireCommand);
            EditCommand = new RelayCommand(Edit);
            SaveCommand = new RelayCommand(Save);
            InfoCommand = new RelayCommand(ExecuteInfoCommand);
        }

        private void ExecuteInfoCommand(object obj)
        {
            MessageBox.Show("Изменению подлежат только следующие поля:\n Имя, Фамилия, Отчество, Почта, Номер телефона.\n Остальные данные изменению не подлежат!!!", "Внимание!!!");

        }

        private void Edit(object parameter)
        {
            // Включить режим редактирования
            IsEditing = true;
        }

        private void Save(object parameter)
        {
            // Сохранить изменения в базу данных
            // Здесь должна быть логика для сохранения изменений
            IsEditing = false;
        }
        private bool isEditing;
        public bool IsEditing
        {
            get { return isEditing; }
            set
            {
                isEditing = value;
                OnPropertyChanged(nameof(IsEditing));
                OnPropertyChanged(nameof(IsNotEditing));
            }
        }

        public bool IsNotEditing => !IsEditing;


        private bool CanExecuteFireCommand(object arg)
        {
            return !(SelectedEmployee == -1 || SelectedEmployee >= Employees.Rows.Count);
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
                Employees = userRepository.GetByAll();
            };
        }
    }
    
}
