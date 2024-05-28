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
        private static NavigationVM _instance;
        public static NavigationVM Instance => _instance ?? (_instance = new NavigationVM());

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

        public ICommand EmployeeCommand { get; set; }
        public ICommand KitchenCommand { get; set; }
        public ICommand MenuCommand { get; set; }
        public ICommand OrderCommand { get; set; }
        public ICommand ProfileCommand { get; set; }
        public ICommand StatisticCommand { get; set; }
        public ICommand CloseTabCommand { get; set; }
        public ICommand ReturnCommand { get; set; }

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

            EmployeeCommand = new RelayCommand(Employee, CanGoEmployee);
            KitchenCommand = new RelayCommand(Kitchen);
            MenuCommand = new RelayCommand(Menu);
            OrderCommand = new RelayCommand(Order, CanGoOrder);
            ProfileCommand = new RelayCommand(Profile);
            StatisticCommand = new RelayCommand(Statistic, CanGoStatistic);

            ReturnCommand = new RelayCommand(ExecuteReturnCommand);
            CloseTabCommand = new RelayCommand(ExecuteCloseTabCommand);


            CurrentView = new HelloPage();
        }

        private bool CanGoOrder(object arg)
        {
            return (UserData.RoleId == 7 ||
                UserData.RoleId == 6);
        }

        private void ExecuteCloseTabCommand(object obj)
        {
            IsMenuHidden = !IsMenuHidden;
        }

        private void LoadCurrentUserData()
        {
            userRepository.GetById();
        }

        private bool CanGoStatistic(object arg)
        {
            return (UserData.RoleId == 1 ||
                UserData.RoleId == 2 || UserData.RoleId == 7 ||
                UserData.RoleId == 3);
        }



        private bool CanGoEmployee(object arg)
        {
            bool CanGoEmployee = false;
            if (UserData.RoleId == 1 || UserData.RoleId == 7 ||
                UserData.RoleId == 2 ||
                UserData.RoleId == 3) CanGoEmployee = true;
            return CanGoEmployee;
        }


        private void ExecuteReturnCommand(object obj)
        {
            userRepository.ForgetCurrentUser();
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }
    }
}
