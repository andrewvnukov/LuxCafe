using Cafe_Managment.Repositories;
using Cafe_Managment.Utilities;
using Cafe_Managment.View;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Cafe_Managment.ViewModel
{
    public class ViewWindowVM : ViewModelBase
    {
        private object _activeWindow;
        private WindowState _windowSt;
        private int _windowWidth = 1000;
        private int _windowHeight = 1000;
        private Brush _panelBack;

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

        public Brush PanelBack
        {
            get { return _panelBack; }
            set { _panelBack = value; OnPropertyChanged(); }
        }

        public int WindowWidth
        {
            get { return _windowWidth; }
            set { _windowWidth = value; OnPropertyChanged(); }
        }

        public int WindowHeight
        {
            get { return _windowHeight; }
            set { _windowHeight = value; OnPropertyChanged(); }
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
            
            
            navigation = new Navigation();
            ActiveWindow = navigation;
            PanelBack = Application.Current.TryFindResource("MainPanelBack") as Brush;
            WindowHeight = 600;
            WindowWidth = 1000;
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
            PanelBack = (Brush)Application.Current.Resources["AuthPanelBack"];
            WindowHeight = 600;
            WindowWidth = 500;
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




