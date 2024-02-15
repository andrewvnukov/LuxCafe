using Cafe_Managment.Utilities;
using Cafe_Managment.Model;
using Cafe_Managment.Repositories;
using Cafe_Managment.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Threading;
using System.Windows;

namespace Cafe_Managment.ViewModel
{
    class NavigationVM : ViewModelBase
    {
        private IUserRepository userRepository;
        private bool _isViewVisible = true;
        private bool _isEnabled = true;
        private bool _isMenuHidden = false;

        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; OnPropertyChanged(nameof(CurrentView)); }
        }

        public bool IsViewVisible
        {
            get => _isViewVisible;
            set { _isViewVisible = value; OnPropertyChanged(nameof(IsViewVisible)); }
        }
        public bool IsEnabled
        {
            get => _isEnabled;
            set { _isEnabled = value; OnPropertyChanged(nameof(IsEnabled)); }
        }

        public bool IsMenuHidden
        {
            get=> _isMenuHidden;
            set
            {
                _isMenuHidden = value; OnPropertyChanged(nameof(IsMenuHidden));
            }
        }

        public ICommand DishCommand { get; set; }
        public ICommand EmployeeCommand { get; set; }
        public ICommand KitchenCommand { get; set; }
        public ICommand MenuCommand { get; set; }
        public ICommand OrderCommand { get; set; }
        public ICommand ProfileCommand { get; set; }
        public ICommand StatisticCommand { get; set; }
        public ICommand BarCommand { get; set; }

        public ICommand CloseTabCommand { get; set; }
        public ICommand ReturnCommand { get; set; }

        private void Dish(object obj) => CurrentView = new DishVM();
        private void Employee(object obj) => CurrentView = new EmployeeVM();
        private void Kitchen(object obj) => CurrentView = new KitchenVM();
        private void Menu(object obj) => CurrentView = new MenuVM();
        private void Profile(object obj) => CurrentView = new ProfileVM();
        private void Order(object obj) => CurrentView = new OrderVM();
        private void Statistic(object obj) => CurrentView = new StatisticVM();
        private void Bar(object obj) => CurrentView = new BarVM(); 

        public NavigationVM()
        {
            userRepository = new UserRepository();
            LoadCurrentUserData();

            DishCommand = new RelayCommand(Dish, CanGoDish);
            EmployeeCommand = new RelayCommand(Employee, CanGoEmployee);
            KitchenCommand = new RelayCommand(Kitchen, CanGoKitchen);
            MenuCommand = new RelayCommand(Menu, CanGoMenu);
            OrderCommand = new RelayCommand(Order);
            ProfileCommand = new RelayCommand(Profile);
            StatisticCommand = new RelayCommand(Statistic, CanGoStatistic);
            BarCommand = new RelayCommand(Bar);

            ReturnCommand = new RelayCommand(ExecuteReturnCommand);
            CloseTabCommand = new RelayCommand(ExecuteCloseTabCommand);


            CurrentView = new HelloPage();
        }

        private void ExecuteCloseTabCommand(object obj)
        {
            IsMenuHidden = IsMenuHidden == false;
        }

        private void LoadCurrentUserData()
        {
            userRepository.GetById();
        }

        private bool CanGoBar(object arg)
        {
            bool CanGoBar = false;
            if (UserData.RoleId == 1 ||
                UserData.RoleId == 2 ||
                UserData.RoleId == 3) CanGoBar = true;
            return CanGoBar;
        }

        private bool CanGoStatistic(object arg)
        {
            bool CanGoStatistic = false;
            if(UserData.RoleId == 1 || 
                UserData.RoleId == 2 ||
                UserData.RoleId == 3) CanGoStatistic = true;
            return CanGoStatistic;
        }

        private bool CanGoMenu(object arg)
        {
            bool CanGoMenu = false;
            if (UserData.RoleId == 1 ||
                UserData.RoleId == 2 ||
                UserData.RoleId == 3) CanGoMenu = true;
            return CanGoMenu;
        }

        private bool CanGoKitchen(object arg)
        {
            bool CanGoKitchen = false;
            if (UserData.RoleId == 1 ||
                UserData.RoleId == 2  ||
                UserData.RoleId == 3 ||
                UserData.RoleId == 4) CanGoKitchen = true;
            return CanGoKitchen;
        }

        private bool CanGoEmployee(object arg)
        {
            bool CanGoEmployee = false;
            if (UserData.RoleId == 1 ||
                UserData.RoleId == 2 ||
                UserData.RoleId == 3) CanGoEmployee = true;
            return CanGoEmployee;
        }

        private bool CanGoDish(object arg)
        {
            bool CanGoDish = false;
            if(UserData.RoleId == 1 ||
                UserData.RoleId == 2 ||
                UserData.RoleId == 3) CanGoDish=true;
            return CanGoDish;
        }

        

        private void ExecuteReturnCommand(object obj)
        {
            IsViewVisible = false;
        }
    }
}
