using Cafe_Managment.Utilities;
using Cafe_Managment.View;
using Mysqlx.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cafe_Managment.ViewModel
{
    public class ViewWindowVM : ViewModelBase
    {
        private object _activeWindow;

        Login login;
        Navigation navigation;


        public object ActiveWindow
        {
            get { return _activeWindow; }
            set { _activeWindow = value; OnPropertyChanged(nameof(ActiveWindow)); }
        }
        public ViewWindowVM()
        {
            login = new Login();
            ActiveWindow = login;

            login.IsVisibleChanged += (s, ev) =>
            {
                if (!login.IsVisible && login.IsLoaded)
                {
                    navigation = new Navigation();
                    ActiveWindow = navigation;

                    navigation.IsVisibleChanged += (s1, ev1) =>
                    {
                        login = new Login();
                        ActiveWindow = login;
                    };

                }
            };
        }
        
        }

        

    }




