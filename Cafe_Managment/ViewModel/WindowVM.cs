using Cafe_Managment.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe_Managment.ViewModel
{
    public class WindowVM : ViewModelBase
    {
        private object _activeWindow;

        public object ActiveWindow
        {
            get { return _activeWindow; }
            set { _activeWindow = value; OnPropertyChanged(); }
        }

        public WindowVM() { 
            ActiveWindow = new LoginVM();

            ActiveWindow
        }

        
    }
}
