using Cafe_Managment.Model;
using Cafe_Managment.Repositories;
using Cafe_Managment.Utilities;
using Cafe_Managment.View;
using Cafe_Managment.View.DialogWindows;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Cafe_Managment.ViewModel
{
    public class ViewWindowVM : ViewModelBase
    {
        private object _activeWindow;
        private WindowState _windowSt;
        private int _windowWidth = 1000;
        private int _windowHeight = 1000;
        private Brush _panelBack;
        private ResizeMode _resizeMode;
        private bool _canResize;
        UserRepository userRepository;

        UserRepository repositoryBase;

        Login login;
        Navigation navigation;
        Loading loading = new Loading();

        private string _employeeFullName;

        public string EmployeeFullName
        {
            get { return _employeeFullName; }
            set
            {
                if (_employeeFullName != value)
                {
                    _employeeFullName = value;
                    OnPropertyChanged(nameof(EmployeeFullName));
                }
            }
        }
        private BitmapImage _employeePhoto;
        private EmpData _currentData;

        public BitmapImage EmployeePhoto
        {
            get { return _employeePhoto; }
            set
            {
                if (_employeePhoto != value)
                {
                    _employeePhoto = value;
                    OnPropertyChanged(nameof(EmployeePhoto));
                }
            }
        }


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
            set { _panelBack = value; OnPropertyChanged(nameof(PanelBack)); }
        }

        public int WindowWidth
        {
            get { return _windowWidth; }
            set { _windowWidth = value; OnPropertyChanged(nameof(WindowWidth)); }
        }

        public int WindowHeight
        {
            get { return _windowHeight; }
            set { _windowHeight = value; OnPropertyChanged(nameof(WindowHeight)); }
        }

        public ResizeMode ResizeMode
        { get { return _resizeMode; }
            set { _resizeMode = value; OnPropertyChanged(nameof(ResizeMode)); }
        }

        public bool CanResize
        {
            get { return _canResize; }
            set { _canResize = value; OnPropertyChanged(nameof(CanResize)); }
        }
        public EmpData CurrentData
        {
            get { return _currentData; }
            set { _currentData = value; OnPropertyChanged(nameof(CurrentData)); }
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
            userRepository = new UserRepository();
            CurrentData = new EmpData
            {
                Name = UserData.Name,
                Surname = UserData.Surname,
                Patronomic = UserData.Patronomic,
                Address = UserData.Address,
                Email = UserData.Email,
                BirthDay = UserData.BirthDay,
                PhoneNumber = UserData.PhoneNumber,
                CreatedAt = UserData.CreatedAt,
                ProfileImage = UserData.ProfileImage,
            };
            EmployeeFullName = UserData.Surname +" "+ UserData.Name + " " + UserData.Patronomic;

        }

        private BitmapImage LoadEmployeePhoto(BitmapImage bitmapImage)
        {
            throw new NotImplementedException();
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
            ResizeMode = ResizeMode.CanResize;
            CanResize = true;
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
            ResizeMode = ResizeMode.CanMinimize;
            CanResize = false;
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




