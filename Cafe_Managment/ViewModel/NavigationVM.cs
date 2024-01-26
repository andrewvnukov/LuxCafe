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
        private UserAccountData _currentUserAccount;
        private IUserRepository userRepository;

        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; OnPropertyChanged(); }
        }

        public UserAccountData CurrentUserAccount
        {
            get => _currentUserAccount;
            set
            {
                _currentUserAccount = value;
                OnPropertyChanged(nameof(CurrentUserAccount));
            }
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

            DishCommand = new RelayCommand(Dish,CanGoDish);
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
            if(CurrentUserAccount.Post == "admin" || 
                CurrentUserAccount.Post == "manager" ||
                CurrentUserAccount.Post == "owner") CanGoStatistic = true;
            return CanGoStatistic;
        }
        private bool CanGoMenu(object arg)
        {
            bool CanGoMenu = false;
            if (CurrentUserAccount.Post == "admin" ||
                CurrentUserAccount.Post == "manager" ||
                CurrentUserAccount.Post == "owner") CanGoMenu = true;
            return CanGoMenu;
        }
        private bool CanGoKitchen(object arg)
        {
            bool CanGoKitchen = false;
            if (CurrentUserAccount.Post == "admin" ||
                CurrentUserAccount.Post == "manager" ||
                CurrentUserAccount.Post == "owner" ||
                CurrentUserAccount.Post == "woman") CanGoKitchen = true;
            return CanGoKitchen;
        }
        private bool CanGoEmployee(object arg)
        {
            bool CanGoEmployee = false;
            if (//CurrentUserAccount.Post == "admin" ||
                CurrentUserAccount.Post == "manager" ||
                CurrentUserAccount.Post == "owner") CanGoEmployee = true;
            return CanGoEmployee;

        }
        private bool CanGoDish(object arg)
        {
            bool CanGoDish = false;
            if(CurrentUserAccount.Post == "admin" ||
                CurrentUserAccount.Post == "manager" ||
                CurrentUserAccount.Post == "owner") CanGoDish=true;
            return CanGoDish;
        }
        private void LoadCurrentUserData()
        {
            CurrentUserAccount = userRepository.GetById(int.Parse(Thread.CurrentPrincipal.Identity.Name));
        }

        private void ExecuteReturnCommand(object obj)
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        private void ExecuteCloseAppCommand(object obj)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
