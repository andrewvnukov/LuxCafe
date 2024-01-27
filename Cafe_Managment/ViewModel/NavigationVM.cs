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
        private UserData _currentUserAccount;
        private IUserRepository userRepository;
        private bool _isVisible = true;
        private bool _isEnabled = true;

        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; OnPropertyChanged(); }
        }

        public UserData CurrentUserAccount
        {
            get => _currentUserAccount;
            set
            {
                _currentUserAccount = value;
                OnPropertyChanged(nameof(CurrentUserAccount));
            }
        }

        public bool IsVisible
        {
            get => _isVisible;
            set { _isVisible = value; OnPropertyChanged(nameof(IsVisible)); }
        }
        public bool IsEnabled
        {
            get => _isEnabled;
            set { _isEnabled = value; OnPropertyChanged(nameof(IsEnabled)); }
        }

        public ICommand DishCommand { get; set; }
        public ICommand EmployeeCommand { get; set; }
        public ICommand KitchenCommand { get; set; }
        public ICommand MenuCommand { get; set; }
        public ICommand OrderCommand { get; set; }
        public ICommand ProfileCommand { get; set; }
        public ICommand StatisticCommand { get; set; }

        public ICommand ReturnCommand { get; set; }
        public ICommand CloseAppCommand { get; set; }

        private void Dish(object obj) => CurrentView = new DishVM();
        private void Employee(object obj) => CurrentView = new EmployeeVM();
        private void Kitchen(object obj) => CurrentView = new KitchenVM();
        private void Menu(object obj) => CurrentView = new MenuVM();
        private void Profile(object obj) => CurrentView = new ProfileVM();
        private void Order(object obj) => CurrentView = new OrderVM();
        private void Statistic(object obj) => CurrentView = new StatisticVM();

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

            CloseAppCommand = new RelayCommand(ExecuteCloseAppCommand);
            ReturnCommand = new RelayCommand(ExecuteReturnCommand);


            CurrentView = new ProfileVM();
        }

        private bool CanGoStatistic(object arg)
        {
            bool CanGoStatistic = false;
            if(CurrentUserAccount.RoleId == 1 || 
                CurrentUserAccount.RoleId == 2 ||
                CurrentUserAccount.RoleId == 3) CanGoStatistic = true;
            return CanGoStatistic;
        }

        private bool CanGoMenu(object arg)
        {
            bool CanGoMenu = false;
            if (CurrentUserAccount.RoleId == 1 ||
                CurrentUserAccount.RoleId == 2 ||
                CurrentUserAccount.RoleId == 3) CanGoMenu = true;
            return CanGoMenu;
        }

        private bool CanGoKitchen(object arg)
        {
            bool CanGoKitchen = false;
            if (CurrentUserAccount.RoleId == 1 ||
                CurrentUserAccount.RoleId == 2  ||
                CurrentUserAccount.RoleId == 3 ||
                CurrentUserAccount.RoleId == 4) CanGoKitchen = true;
            return CanGoKitchen;
        }

        private bool CanGoEmployee(object arg)
        {
            bool CanGoEmployee = false;
            if (CurrentUserAccount.RoleId == 1 ||
                CurrentUserAccount.RoleId == 2 ||
                CurrentUserAccount.RoleId == 3) CanGoEmployee = true;
            return CanGoEmployee;
        }

        private bool CanGoDish(object arg)
        {
            bool CanGoDish = false;
            if(CurrentUserAccount.RoleId == 1 ||
                CurrentUserAccount.RoleId == 2 ||
                CurrentUserAccount.RoleId == 3) CanGoDish=true;
            return CanGoDish;
        }

        private void LoadCurrentUserData()
        {
            CurrentUserAccount = userRepository.GetById(int.Parse(Thread.CurrentPrincipal.Identity.Name));
        }

        private void ExecuteReturnCommand(object obj)
        {
            IsVisible = false;
        }

        private void ExecuteCloseAppCommand(object obj)
        {
            IsEnabled= false;
            Application.Current.Shutdown();
        }
    }
}
