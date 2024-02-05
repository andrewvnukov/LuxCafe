using Cafe_Managment.Model;
using Cafe_Managment.Repositories;
using Cafe_Managment.Utilities;
using Cafe_Managment.View;
using Mysqlx.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Cafe_Managment.ViewModel
{
    public class ViewWindowVM : ViewModelBase
    {
        private object _activeWindow;
        private WindowState _windowSt;

        UserRepository repositoryBase;

        Login login;
        Navigation navigation;
        Loading loading = new Loading();



        public object ActiveWindow
        {
            get { return _activeWindow; }
            set { _activeWindow = value; OnPropertyChanged(nameof(ActiveWindow)); }

        }

        public WindowState WindowSt
        {
            get { return _windowSt; }
            set { _windowSt = value; OnPropertyChanged(nameof(WindowSt)); }
        }

        public ICommand CloseAppCommand { get; set; }
        public ICommand MaximizeCommand { get; set; }
        public ICommand MinimizeCommand { get; set; }

        public ViewWindowVM()
        {
            CloseAppCommand = new RelayCommand(CloseApp);
            MaximizeCommand = new RelayCommand(MaxWindow);
            MinimizeCommand = new RelayCommand(MinWindow);

            repositoryBase = new UserRepository();


            if (repositoryBase.GetByMac())
            {
                RememberedUserAsync();
            }
            else
            {
                AuthUser();
            }

        }

        private void MinWindow(object obj)
        {
            WindowSt = WindowState.Minimized;
        }

        private void MaxWindow(object obj)
        {
            if (WindowSt == WindowState.Normal)
            {
                WindowSt = WindowState.Maximized;
            }
            else WindowSt = WindowState.Normal;

        }

        private void CloseApp(object obj)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void RememberedUserAsync()
        {
            repositoryBase.GetById();

            navigation = new Navigation();
            ActiveWindow = navigation;
            navigation.IsVisibleChanged += (s1, ev1) =>
            {
                if (!navigation.IsVisible && navigation.IsLoaded)
                {
                    AuthUser();
                }
            };

        }

        private void AuthUser()
        {
            login = new Login();
            ActiveWindow = login;
            login.IsVisibleChanged += (s, ev) =>
            {
                if (!login.IsVisible && login.IsLoaded)
                {
                    RememberedUserAsync();
                }
            };
        }
    }



}




