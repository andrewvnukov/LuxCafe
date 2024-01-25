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

            DishCommand = new RelayCommand(Dish);
            EmployeeCommand = new RelayCommand(Employee);
            KitchenCommand = new RelayCommand(Kitchen);
            MenuCommand = new RelayCommand(Menu);
            OrderCommand = new RelayCommand(Order);
            ProfileCommand = new RelayCommand(Profile);
            StatisticCommand = new RelayCommand(Statistic);
            
            CloseAppCommand = new RelayCommand(ExecuteCloseAppCommand);


            CurrentView = new ProfileVM();
        }

        private void LoadCurrentUserData()
        {
            CurrentUserAccount = userRepository.GetById(int.Parse(Thread.CurrentPrincipal.Identity.Name));
        }

        private void ExecuteCloseAppCommand(object obj)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
